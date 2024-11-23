using SigmaApp;
using System.Windows;
using System.Windows.Controls;
namespace SigmaApp;
public partial class LoginWindow : Window
{
    private DatabaseHelper databaseHelper = new DatabaseHelper();

    public LoginWindow()
    {
        InitializeComponent();
    }

    private void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text;
        string password = PasswordBox.Password;

        // Запрос к базе данных для поиска пользователя по имени
        var user = databaseHelper.ExecuteQuery<User>(
            "SELECT * FROM Users WHERE Username = '" + username + "'",
            reader => new User
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                Username = reader["Username"].ToString(),
                PasswordHash = reader["PasswordHash"].ToString(),
                RoleId = Convert.ToInt32(reader["RoleId"]),
                DepartmentId = Convert.ToInt32(reader["DepartmentId"])
            }
        ).FirstOrDefault();

        // Проверка, что пользователь существует и пароль совпадает
        if (user != null && user.PasswordHash == User.HashPassword(password))
        {
            // Открываем главное окно и закрываем окно авторизации
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        else
        {
            MessageBox.Show("Неверные данные!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
