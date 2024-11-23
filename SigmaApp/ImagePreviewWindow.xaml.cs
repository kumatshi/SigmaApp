using System.Windows;
using System.Windows.Media.Imaging;

namespace SigmaApp
{
    public partial class ImagePreviewWindow : Window
    {
        public ImagePreviewWindow(string imagePath)
        {
            InitializeComponent();

            // Загрузка изображения в окно
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                PreviewImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show("Изображение отсутствует или путь некорректен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
