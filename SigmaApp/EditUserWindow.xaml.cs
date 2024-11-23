using System.Data.SQLite;
using System.Windows;

namespace SigmaApp
{
    public partial class EditUserWindow : Window
    {
        private User user;
        private DatabaseHelper databaseHelper = new DatabaseHelper();

        public EditUserWindow(User selectedUser)
        {
            InitializeComponent();
            user = selectedUser;

            // Populate fields with existing user data
            UsernameTextBox.Text = user.Username;
            RoleComboBox.ItemsSource = databaseHelper.ExecuteQuery(
                "SELECT * FROM Roles",
                reader => new User.Role
                {
                    RoleId = Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"].ToString()
                }
            );
            RoleComboBox.DisplayMemberPath = "RoleName";
            RoleComboBox.SelectedValuePath = "RoleId";
            RoleComboBox.SelectedValue = user.RoleId;

            DepartmentComboBox.ItemsSource = databaseHelper.ExecuteQuery(
                "SELECT * FROM Departments",
                reader => new User.Department
                {
                    DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                    DepartmentName = reader["DepartmentName"].ToString()
                }
            );
            DepartmentComboBox.DisplayMemberPath = "DepartmentName";
            DepartmentComboBox.SelectedValuePath = "DepartmentId";
            DepartmentComboBox.SelectedValue = user.DepartmentId;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            int? roleId = RoleComboBox.SelectedValue as int?;
            int? departmentId = DepartmentComboBox.SelectedValue as int?;

            if (string.IsNullOrWhiteSpace(username) || !roleId.HasValue || !departmentId.HasValue) return;

            string passwordHash = string.IsNullOrEmpty(password) ? user.PasswordHash : User.HashPassword(password);

            string query = "UPDATE Users SET Username = @Username, PasswordHash = @PasswordHash, " +
                           "RoleId = @RoleId, DepartmentId = @DepartmentId WHERE UserId = @UserId";

            using (var connection = databaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.Parameters.AddWithValue("@RoleId", roleId.Value);
                    command.Parameters.AddWithValue("@DepartmentId", departmentId.Value);
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.ExecuteNonQuery();
                }
            }

            DialogResult = true; // Close the window and indicate success
            Close();
        }

        private void UsernameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
