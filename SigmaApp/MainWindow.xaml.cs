using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static User;

namespace SigmaApp
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper databaseHelper = new DatabaseHelper();
        private User selectedUser;

        public MainWindow()
        {
            InitializeComponent();
            LoadDepartments();
            LoadRoles();
            LoadUsers();
            LoadRoleFilter();
            LoadOrders();
            LoadClients();
        }

        private void LoadDepartments()
        {
            var departments = databaseHelper.ExecuteQuery(
                "SELECT * FROM Departments",
                reader => new Department
                {
                    DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                    DepartmentName = reader["DepartmentName"].ToString()
                }
            );
            DepartmentComboBox.ItemsSource = departments;
            DepartmentComboBox.DisplayMemberPath = "DepartmentName";
            DepartmentComboBox.SelectedValuePath = "DepartmentId";
        }

        private void LoadRoles()
        {
            var roles = databaseHelper.ExecuteQuery(
                "SELECT * FROM Roles",
                reader => new Role
                {
                    RoleId = Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"].ToString()
                }
            );
            RoleComboBox.ItemsSource = roles;
            RoleComboBox.DisplayMemberPath = "RoleName";
            RoleComboBox.SelectedValuePath = "RoleId";
        }

        private void LoadUsers(int? departmentId = null, int? roleId = null)
        {
            string query = @"
                SELECT u.UserId, u.Username, u.PasswordHash, r.RoleName, d.DepartmentName
                FROM Users u
                JOIN Roles r ON u.RoleId = r.RoleId
                JOIN Departments d ON u.DepartmentId = d.DepartmentId";

            if (departmentId.HasValue || roleId.HasValue)
            {
                query += " WHERE";
                if (departmentId.HasValue) query += $" u.DepartmentId = {departmentId.Value}";
                if (departmentId.HasValue && roleId.HasValue) query += " AND";
                if (roleId.HasValue) query += $" u.RoleId = {roleId.Value}";
            }

            var users = databaseHelper.ExecuteQuery(
                query,
                reader => new User
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Username = reader["Username"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString(),
                    RoleName = reader["RoleName"].ToString(),
                    DepartmentName = reader["DepartmentName"].ToString()
                }
            );

            DataGrid.ItemsSource = users;
        }
        private void LoadRoleFilter()
        {
            // Загружаем все роли в фильтр
            var roles = databaseHelper.ExecuteQuery(
                "SELECT * FROM Roles",
                reader => new User.Role
                {
                    RoleId = Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"].ToString()
                }
            );

            RoleFilterComboBox.ItemsSource = roles;
            RoleFilterComboBox.DisplayMemberPath = "RoleName";
            RoleFilterComboBox.SelectedValuePath = "RoleId";
        }

        private void RoleFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleFilterComboBox.SelectedValue is int selectedRoleId)
            {
                LoadFilteredUsersByRole(selectedRoleId); // Загружаем пользователей по выбранной роли
            }
            else
            {
                FilteredDataGrid.ItemsSource = null; // Очищаем таблицу, если роль не выбрана
            }
        }

        private void LoadFilteredUsersByRole(int roleId)
        {
            // Создаем список пользователей с фильтрацией по роли
            var users = new List<User>();
            string query = @"
        SELECT u.UserId, u.Username, u.PasswordHash, r.RoleName, d.DepartmentName
        FROM Users u
        JOIN Roles r ON u.RoleId = r.RoleId
        JOIN Departments d ON u.DepartmentId = d.DepartmentId
        WHERE u.RoleId = @RoleId";

            using (var connection = databaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    // Добавляем параметр для защиты от SQL-инъекций
                    command.Parameters.AddWithValue("@RoleId", roleId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                RoleName = reader["RoleName"].ToString(),
                                DepartmentName = reader["DepartmentName"].ToString()
                            });
                        }
                    }
                }
            }

            FilteredDataGrid.ItemsSource = users; // Привязываем отфильтрованных пользователей к таблице
        }


        private void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadUsers();
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadUsers();
        }

        private void ReloadUsers()
        {
            int? departmentId = DepartmentComboBox.SelectedValue as int?;
            int? roleId = RoleComboBox.SelectedValue as int?;
            LoadUsers(departmentId, roleId);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem is User user)
            {
                selectedUser = user;
                UsernameTextBox.Text = user.Username;
                RoleComboBox.SelectedValue = user.RoleId;
                DepartmentComboBox.SelectedValue = user.DepartmentId;
            }
            else
            {
                selectedUser = null;
                ClearForm();
            }
        }
        private void LoadClients()
        {
            var clients = databaseHelper.ExecuteQuery(
                "SELECT * FROM Clients",
                reader => new Client
                {
                    ClientId = Convert.ToInt32(reader["ClientId"]),
                    ClientName = reader["ClientName"].ToString(),
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    ContactInfo = reader["ContactInfo"].ToString()
                }
            );

            ClientsDataGrid.ItemsSource = clients;
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            CreateClientWindow createClientWindow = new CreateClientWindow
            {
                Owner = this
            };

            if (createClientWindow.ShowDialog() == true)
            {
                var newClient = createClientWindow.NewClient;

                string query = @"
            INSERT INTO Clients (ClientName, OrderId, ContactInfo) 
            VALUES (@ClientName, @OrderId, @ContactInfo)";

                using (var connection = databaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClientName", newClient.ClientName);
                        command.Parameters.AddWithValue("@OrderId", newClient.OrderId);
                        command.Parameters.AddWithValue("@ContactInfo", newClient.ContactInfo);

                        command.ExecuteNonQuery();
                    }
                }

                LoadClients();
            }
        }

        private void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client client)
            {
                string query = "DELETE FROM Clients WHERE ClientId = @ClientId";

                using (var connection = databaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", client.ClientId);
                        command.ExecuteNonQuery();
                    }
                }

                LoadClients();
            }
            else
            {
                MessageBox.Show("Выберите заказчика для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client client)
            {
                EditClientWindow editClientWindow = new EditClientWindow(client)
                {
                    Owner = this
                };

                if (editClientWindow.ShowDialog() == true)
                {
                    var updatedClient = editClientWindow.UpdatedClient;

                    string query = @"
                UPDATE Clients SET ClientName = @ClientName, OrderId = @OrderId, ContactInfo = @ContactInfo 
                WHERE ClientId = @ClientId";

                    using (var connection = databaseHelper.GetConnection())
                    {
                        connection.Open();
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ClientName", updatedClient.ClientName);
                            command.Parameters.AddWithValue("@OrderId", updatedClient.OrderId);
                            command.Parameters.AddWithValue("@ContactInfo", updatedClient.ContactInfo);
                            command.Parameters.AddWithValue("@ClientId", updatedClient.ClientId);

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadClients();
                }
            }
            else
            {
                MessageBox.Show("Выберите заказчика для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            int? roleId = RoleComboBox.SelectedValue as int?;
            int? departmentId = DepartmentComboBox.SelectedValue as int?;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || !roleId.HasValue || !departmentId.HasValue)
            {
                MessageBox.Show("Заполните все поля и выберите значения из списков!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string passwordHash = User.HashPassword(password);

            try
            {
                databaseHelper.ExecuteNonQuery($"INSERT INTO Users (Username, PasswordHash, RoleId, DepartmentId) " +
                                               $"VALUES ('{username}', '{passwordHash}', {roleId.Value}, {departmentId.Value})");

                ReloadUsers();
                ClearForm();
                MessageBox.Show("Пользователь успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is not User user) return; 

            
            EditUserWindow editWindow = new EditUserWindow(user)
            {
                Owner = this
            };

            
            if (editWindow.ShowDialog() == true)
            {
                ReloadUsers();
            }
        }
      
        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            CreateOrderWindow createOrderWindow = new CreateOrderWindow
            {
                Owner = this 
            };

            if (createOrderWindow.ShowDialog() == true) 
            {
                var newOrder = createOrderWindow.NewOrder;

               
                string query = @"
            INSERT INTO Orders (Id, OrderName, Description, ImagePath) 
            VALUES (@Id, @OrderName, @Description, @ImagePath)";

                using (var connection = databaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", newOrder.Id);
                        command.Parameters.AddWithValue("@OrderName", newOrder.OrderName);
                        command.Parameters.AddWithValue("@Description", newOrder.Description);
                        command.Parameters.AddWithValue("@ImagePath", newOrder.ImagePath ?? string.Empty);

                        command.ExecuteNonQuery();
                    }
                }

                LoadOrders(); 
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is not User user) return;
            string query = "DELETE FROM Users WHERE UserId = @UserId";

            using (var connection = databaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.ExecuteNonQuery();
                }
            }

            ReloadUsers(); 
            ClearForm();
            DataGrid.SelectedItem = null;
        }




        private void ClearForm()
        {
            UsernameTextBox.Clear();
            PasswordBox.Clear();
            RoleComboBox.SelectedIndex = -1;
            DepartmentComboBox.SelectedIndex = -1;
            DataGrid.SelectedItem = null;
        }
        private void LoadOrders()
        {
            var orders = databaseHelper.ExecuteQuery(
                "SELECT * FROM Orders",
                reader => new Order
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    OrderName = reader["OrderName"].ToString(),
                    Description = reader["Description"].ToString(),
                    ImagePath = reader["ImagePath"].ToString() // Путь к изображению
                }
            );

            OrdersDataGrid.ItemsSource = orders;
        }



        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order order)
            {
                databaseHelper.ExecuteNonQuery($"DELETE FROM Orders WHERE Id = {order.Id}");
                LoadOrders(); 
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        
        private void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order order)
            {
                
                LoadOrders();
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ImageWindow_Click(object sender, RoutedEventArgs e)
        {
            ImageUploadWindow imageWindow = new ImageUploadWindow();
            imageWindow.Show();
        }

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (sender is Image image && image.Source is BitmapImage bitmapImage)
                {
                    string imagePath = bitmapImage.UriSource?.LocalPath;

                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        MessageBox.Show($"Путь к изображению: {imagePath}");
                    }
                    else
                    {
                        throw new Exception("Путь к изображению пуст.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }
}
