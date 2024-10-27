using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoInfoManager.Presentation.CrossCutting.Extensions;
using VideoInfoManager.Presentation.Wpf.Helpers;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        public MainWindow()
        {
            _mainWindowViewModel = DependencyInjectionExtensions.GetService<MainWindowViewModel>();
            this.DataContext = _mainWindowViewModel;
            InitializeComponent();
            InitializeWindow();
            if (_mainWindowViewModel is not null)
            {
                this.AddDataTabItem.DragEnter += _mainWindowViewModel.AddDataTab_DragEnter;
            }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close the application?", "Confirm close", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

    }
}