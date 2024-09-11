using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.Wpf.Configuration;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Views;

public partial class VideoInfoSearchResults : UserControl
{
    private readonly VideoInfoSearchViewModel? _videoInfoSearchViewModel;
    public VideoInfoSearchResults()
    {
        _videoInfoSearchViewModel = ConfigureServices.GetService<VideoInfoSearchViewModel>();
        this.DataContext = _videoInfoSearchViewModel;
        InitializeComponent();
    }

    public void InitializeData()
    {
        if (_videoInfoSearchViewModel is not null)
            dgSearchResults.ItemsSource = _videoInfoSearchViewModel.VideoInfoResults;
    }

}