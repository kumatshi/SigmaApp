using System.Security.Cryptography;
using System.Text;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int RoleId { get; set; }
    public int DepartmentId { get; set; }

    // Свойства для отображения роли и отдела
    public string RoleName { get; set; }
    public string DepartmentName { get; set; }

    // Метод для хеширования пароля
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int DepartmentId { get; set; }
    }
  
    public class Task
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public string OrderName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; } // Полный путь к изображению
    }

    public class Client
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int OrderId { get; set; }
        public string ContactInfo { get; set; }
    }




}
