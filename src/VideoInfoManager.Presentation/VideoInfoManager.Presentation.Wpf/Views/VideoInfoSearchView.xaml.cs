using System.Windows.Controls;
using VideoInfoManager.Presentation.Wpf.Configuration;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Views;

public partial class VideoInfoSearchView : UserControl
{
    public VideoInfoSearchView()
    {
        this.DataContext = ConfigureServices.GetService(typeof(VideoInfoSearchViewModel));
        InitializeComponent();
    }
}
