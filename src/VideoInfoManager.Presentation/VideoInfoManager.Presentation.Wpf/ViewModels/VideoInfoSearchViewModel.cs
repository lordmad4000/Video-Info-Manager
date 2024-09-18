using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.CrossCutting.Services;
using VideoInfoManager.Presentation.Wpf.Handlers;
using VideoInfoManager.Presentation.Wpf.Helpers;
using VideoInfoManager.Presentation.Wpf.Windows;

namespace VideoInfoManager.Presentation.Wpf.ViewModels;

public class VideoInfoSearchViewModel : ViewModelBase
{
    private const int MinSearchLength = 3;

    private readonly IVideoInfoManagerPresentationAppService _videoInfoManagerPresentationAppService;
    private readonly IAbstractFactory<EditDialogWindow> _editDialogWindowFactory;
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly List<string> _statuses;

    public VideoInfoSearchViewModel(IVideoInfoManagerPresentationAppService videoInfoManagerPresentationAppService, IAbstractFactory<EditDialogWindow> editDialogWindowFactory, MainWindowViewModel mainWindowViewModel)
    {
        _videoInfoManagerPresentationAppService = videoInfoManagerPresentationAppService;        
        _editDialogWindowFactory = editDialogWindowFactory;
        _mainWindowViewModel = mainWindowViewModel;
        _statuses = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses().Select(c => c.ConfigurationName)
                                                           .ToList();

        PasteToSearchCommand = new CommandHandler(PasteToSearch, _ => true);
        SearchCommand = new CommandHandler(Search, _ => true);
        EditCommand = new CommandHandler(Edit, _ => true);
    }

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

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            if (_searchText.Length > MinSearchLength)
            {
                Search(new object());
            }
        }
    }

    public ICommand PasteToSearchCommand { get; private set; }
    public ICommand SearchCommand {  get;  private set; }
    public ICommand EditCommand { get; private set; }

    private void Search(object parameter)
    {
        var search = new string[] { SearchText };
        _videoInfoManagerPresentationAppService.Search(search);
        VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_videoInfoManagerPresentationAppService.GetResults());
    }

    private void PasteToSearch(object parameter)
    {
        if (Clipboard.ContainsText())
        {
            string clipBoardText = Clipboard.GetText();
            SearchText = clipBoardText;
        }
    }

    private void Edit(object videoInfoRow)
    {
        var videoInfoDTO = (VideoInfoDTO) videoInfoRow;

        int updated = _editDialogWindowFactory.Create()
                                              .ShowDialogWindow(videoInfoDTO.Id, videoInfoDTO.Name, videoInfoDTO.Status, _statuses);

        _mainWindowViewModel.StatusBarText = updated switch
        {
            0 => $"Failed to Update {videoInfoDTO.Name}",
            1 => $"{videoInfoDTO.Name} Updated",
            _ => $"Edit canceled"
        };

        if (updated == 1)
        {
            _videoInfoManagerPresentationAppService.LastSearch(true);
            VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_videoInfoManagerPresentationAppService.GetResults());
        }
    }

}


