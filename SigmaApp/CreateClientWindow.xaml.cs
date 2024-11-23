using System;
using System.Windows;
using static User;

namespace SigmaApp
{
    public partial class CreateClientWindow : Window
    {
        public Client NewClient { get; private set; }

        public CreateClientWindow()
        {
            InitializeComponent();
        }

        private void SaveClientButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что все обязательные поля заполнены
            if (!string.IsNullOrWhiteSpace(ClientNameTextBox.Text) && int.TryParse(OrderIdTextBox.Text, out int orderId))
            {
                NewClient = new Client
                {
                    ClientName = ClientNameTextBox.Text,
                    OrderId = orderId,
                    ContactInfo = ContactInfoTextBox.Text
                };

                DialogResult = true; // Закрываем окно и указываем, что данные успешно сохранены
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
