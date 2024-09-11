using System.ComponentModel;
using System.Windows;

namespace VideoInfoManager.Presentation.Wpf.Windows;

public partial class EditDialogWindow : Window, INotifyPropertyChanged
{
    public EditDialogWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        VideoInfoStatuses.Add("Pended");
        VideoInfoStatuses.Add("Backup");
        VideoInfoStatuses.Add("Saved");
        VideoInfoStatuses.Add("Deleted");
        VideoInfoStatuses.Add("Low Quality");
        cbStatus.ItemsSource = VideoInfoStatuses;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _videoInfoName = string.Empty;
    public Guid VideoInfoId { get; set; }
    public List<string> VideoInfoStatuses{ get; set; } = new List<string>();
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

    public void ShowDialogWindow(Guid id, string name, string selectedStatus)
    {
        VideoInfoId = id;
        VideoInfoName = name;
        VideoInfoSelectedStatus = selectedStatus;
        cbStatus.SelectedItem = VideoInfoSelectedStatus;
        this.ShowDialog();
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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        VideoInfoName = "Antonio";
    }
}
