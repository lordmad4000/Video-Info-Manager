using System.Windows.Controls;
using VideoInfoManager.Presentation.CrossCutting.Extensions;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Views;

public partial class VideoInfoSearchStatusView : UserControl
{
    public VideoInfoSearchStatusView()
    {
        this.DataContext = DependencyInjectionExtensions.GetService(typeof(VideoInfoSearchViewModel));
        InitializeComponent();
    }
}
