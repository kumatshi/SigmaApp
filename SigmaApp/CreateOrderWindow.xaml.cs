using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using static User;

namespace SigmaApp
{
    public partial class CreateOrderWindow : Window
    {
        public string ImagePath { get; private set; }
        public Order NewOrder { get; private set; }

        public CreateOrderWindow()
        {
            InitializeComponent();
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png; *.jpg; *.jpeg; *.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;

                BitmapImage bitmap = new BitmapImage(new Uri(ImagePath, UriKind.Absolute));
                SelectedImage.Source = bitmap;
            }
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(IdTextBox.Text, out int id) && !string.IsNullOrWhiteSpace(OrderNameTextBox.Text))
            {
                NewOrder = new Order
                {
                    Id = id,
                    OrderName = OrderNameTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    ImagePath = ImagePath
                };

                DialogResult = true; // Закрыть окно и указать, что данные успешно сохранены
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
