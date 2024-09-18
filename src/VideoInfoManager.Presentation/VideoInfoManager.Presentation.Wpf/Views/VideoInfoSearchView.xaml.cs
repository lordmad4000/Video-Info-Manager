using System.Windows.Controls;
using VideoInfoManager.Presentation.CrossCutting.Extensions;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Views;

public partial class VideoInfoSearchView : UserControl
{
    public VideoInfoSearchView()
    {
        this.DataContext = DependencyInjectionExtensions.GetService(typeof(VideoInfoSearchViewModel));
        InitializeComponent();
    }
}
