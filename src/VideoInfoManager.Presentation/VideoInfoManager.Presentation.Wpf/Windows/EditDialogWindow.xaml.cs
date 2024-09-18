using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.CrossCutting.Extensions;
using VideoInfoManager.Presentation.CrossCutting.Services;
using VideoInfoManager.Presentation.Wpf.Helpers;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Windows;

public partial class EditDialogWindow : Window, INotifyPropertyChanged
{
    private readonly MainWindowViewModel? _mainWindowViewModel;
    private readonly IVideoInfoManagerPresentationAppService? _videoInfoManagerPresentationAppService;
    private int _result = -1;

    public EditDialogWindow()
    {
        _mainWindowViewModel = DependencyInjectionExtensions.GetService<MainWindowViewModel>();
        _videoInfoManagerPresentationAppService = DependencyInjectionExtensions.GetService<IVideoInfoManagerPresentationAppService>();
        InitializeComponent();
        var icon = IconExtractor.Extract("shell32.dll", 218, true);
        ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        this.Icon = imageSource;
        this.DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _videoInfoName = string.Empty;
    public Guid VideoInfoId { get; set; }
    public List<string> VideoInfoStatuses { get; set; } = new List<string>();
    public string VideoInfoSelectedStatus { get; set; } = string.Empty;
    public string VideoInfoName
    {
        get => _videoInfoName;
        set
        {
            _videoInfoName = value;
            OnPropertyChanged("VideoInfoName");
        }
    }

    public int ShowDialogWindow(Guid id, string name, string selectedStatus, List<string> statuses)
    {
        VideoInfoId = id;
        VideoInfoName = name;
        cbStatus.ItemsSource = statuses;
        VideoInfoSelectedStatus = selectedStatus;
        cbStatus.SelectedItem = VideoInfoSelectedStatus;
        if (_mainWindowViewModel is not null)
        {
            this.Top = _mainWindowViewModel.Top + (_mainWindowViewModel.Height - this.Height) / 2;
            this.Left = _mainWindowViewModel.Left + (_mainWindowViewModel.Width - this.Width) / 2;
        }

        this.ShowDialog();

        return _result;
    }

    private void CenterPosition(double parentLeft, double parentTop, double parentWidth, double parentHeight)
    {
        this.Left = parentLeft + (parentWidth - this.ActualWidth) / 2;
        this.Top = parentTop + (parentHeight - this.ActualHeight) / 2;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        //e.Cancel = true;
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ButtonSave_Click(object sender, RoutedEventArgs e)
    {
        var videoInfoDTO = new VideoInfoDTO
        {
            Id = VideoInfoId,
            Name = VideoInfoName,
            Status = VideoInfoSelectedStatus,
        };

        if (MessageBox.Show($"Update Data to Data Base?", "Update Data", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        {
            return;
        }

        if (_videoInfoManagerPresentationAppService?.Update(videoInfoDTO) == true)
        {
            _result = 1;
        }
        else
        {
            _result = 0;
        }

        this.Close();
    }
}
