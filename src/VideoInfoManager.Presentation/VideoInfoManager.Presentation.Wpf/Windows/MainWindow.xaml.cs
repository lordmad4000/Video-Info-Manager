using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoInfoManager.Presentation.Wpf.Helpers;

namespace VideoInfoManager.Presentation.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            this.Height = 500;
            this.Width = 560;
            var icon = IconExtractor.Extract("shell32.dll", 218, true);
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            this.Icon = imageSource;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {            
        }

    }
}