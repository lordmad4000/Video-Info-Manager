namespace VideoInfoManager.Presentation.Wpf.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public double Top { get; set; }
    public double Left { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    private string _statusBarText = $"Last message:";
    public string StatusBarText
    {
        get => _statusBarText;
        set
        {
            _statusBarText = $"Last message: {value}";
            OnPropertyChanged(nameof(StatusBarText));
        }
    }


}
