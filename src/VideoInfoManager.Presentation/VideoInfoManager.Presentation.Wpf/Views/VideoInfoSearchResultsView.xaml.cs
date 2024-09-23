using System.Windows.Controls;
using VideoInfoManager.Presentation.CrossCutting.Extensions;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Views;

public partial class VideoInfoSearchResultsView : UserControl
{
    private readonly VideoInfoSearchViewModel? _videoInfoSearchViewModel;
    public VideoInfoSearchResultsView()
    {
        _videoInfoSearchViewModel = DependencyInjectionExtensions.GetService<VideoInfoSearchViewModel>();
        this.DataContext = _videoInfoSearchViewModel;
        InitializeComponent();
    }

    public void InitializeData()
    {
        if (_videoInfoSearchViewModel is not null)
            dgSearchResults.ItemsSource = _videoInfoSearchViewModel.VideoInfoResults;
    }

}