using System;
using System.Windows;
using static User;

namespace SigmaApp
{
    public partial class EditClientWindow : Window
    {
        public Client UpdatedClient { get; private set; }

        public EditClientWindow(Client client)
        {
            InitializeComponent();
            UpdatedClient = client;

            // Заполняем поля существующими данными клиента
            ClientNameTextBox.Text = client.ClientName;
            OrderIdTextBox.Text = client.OrderId.ToString();
            ContactInfoTextBox.Text = client.ContactInfo;
        }

        private void SaveClientButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что все обязательные поля заполнены
            if (!string.IsNullOrWhiteSpace(ClientNameTextBox.Text) && int.TryParse(OrderIdTextBox.Text, out int orderId))
            {
                UpdatedClient.ClientName = ClientNameTextBox.Text;
                UpdatedClient.OrderId = orderId;
                UpdatedClient.ContactInfo = ContactInfoTextBox.Text;

                DialogResult = true; // Закрываем окно и указываем, что данные успешно обновлены
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
