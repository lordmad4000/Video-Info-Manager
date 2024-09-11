using System.Collections.ObjectModel;
using System.Windows.Input;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.Wpf.Handlers;
using VideoInfoManager.Presentation.Wpf.Helpers;
using VideoInfoManager.Presentation.Wpf.Services;
using VideoInfoManager.Presentation.Wpf.Windows;

namespace VideoInfoManager.Presentation.Wpf.ViewModels;

public class VideoInfoSearchViewModel : ViewModelBase
{
    private readonly SearchService _searchService;
    private readonly IAbstractFactory<EditDialogWindow> _editDialogWindowFactory;

    public VideoInfoSearchViewModel(SearchService searchService, IAbstractFactory<EditDialogWindow> editDialogWindowFactory)
    {
        _searchService = searchService;
        _editDialogWindowFactory = editDialogWindowFactory;
        SearchCommand = new CommandHandler(Search, _ => true);
        EditCommand = new CommandHandler(Edit, _ => true);
    }

    private string _searchText = string.Empty;
    private ObservableCollection<VideoInfoDTO> _videoInfoResults = new ObservableCollection<VideoInfoDTO>();
    public ObservableCollection<VideoInfoDTO> VideoInfoResults
    {
        get => _videoInfoResults;
        set
        {
            _videoInfoResults = value;
            OnPropertyChanged("VideoInfoResults");
        }
    }                    

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
        }
    }

    public ICommand SearchCommand {  get;  private set; }
    public ICommand EditCommand { get; private set; }

    private void Search(object parameter)
    {
        var search = new string[] { SearchText };
        _searchService.Search(search);
        VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_searchService.Results);
    }

    private void Edit(object videoInfoRow)
    {
        var videoInfoDTO = (VideoInfoDTO) videoInfoRow;

        var search = new string[] { SearchText };
        _editDialogWindowFactory.Create()
                                .ShowDialogWindow(videoInfoDTO.Id, videoInfoDTO.Name, videoInfoDTO.Status);
        //_searchService.Search(search);
        //VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_searchService.Results);
    }


}


