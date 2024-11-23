using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace SigmaApp
{
    /// <summary>
    /// </summary>
    public partial class ImageUploadWindow : Window
    {
        public ImageUploadWindow()
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
               
                string selectedFilePath = openFileDialog.FileName;

                
                string projectDirectory = Directory.GetCurrentDirectory(); 
                string destinationPath = Path.Combine(projectDirectory, "Images", Path.GetFileName(selectedFilePath));

                
                if (!Directory.Exists(Path.Combine(projectDirectory, "Images")))
                {
                    Directory.CreateDirectory(Path.Combine(projectDirectory, "Images"));
                }

                
                File.Copy(selectedFilePath, destinationPath, true);

                
                BitmapImage bitmap = new BitmapImage(new Uri(destinationPath, UriKind.Absolute));
                SelectedImage.Source = bitmap;

                MessageBox.Show($"Изображение сохранено в {destinationPath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}