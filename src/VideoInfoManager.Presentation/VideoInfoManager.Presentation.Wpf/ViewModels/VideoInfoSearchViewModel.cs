using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Enums;
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
        
        InitializeStatusCheckBoxes();
        InititalizeCommands();
    }    

    public StatusCheckBoxesViewModel StatusCheckBoxes { get; set; } = new StatusCheckBoxesViewModel();
    public StatusCheckBoxesViewModel _statusCheckBoxes = new StatusCheckBoxesViewModel();
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

    public ICommand PasteToSearchCommand { get; private set; } = new CommandHandler(c => c.ToString(), _ => true); 
    public ICommand SearchCommand {  get;  private set; } = new CommandHandler(c => c.ToString(), _ => true);
    public ICommand EditCommand { get; private set; } = new CommandHandler(c => c.ToString(), _ => true);

    private void Search(object parameter)
    {
        String[] lines = SearchText.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
        var search = new string[] { SearchText };
        if (lines.Count() > 1)
        {
           search = lines;
        }

        _videoInfoManagerPresentationAppService.Search(search, GetActiveStatus());
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
            _videoInfoManagerPresentationAppService.LastSearch(GetActiveStatus(), true);
            VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_videoInfoManagerPresentationAppService.GetResults());
        }
    }

    private List<VideoInfoStatusEnum> GetActiveStatus()
    {
        var activeStatus = new List<VideoInfoStatusEnum>();
        if (this.StatusCheckBoxes.PendedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Pended);
        if (this.StatusCheckBoxes.SavedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Saved);
        if (this.StatusCheckBoxes.BackupedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Backuped);
        if (this.StatusCheckBoxes.DeletedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Deleted);
        if (this.StatusCheckBoxes.LowedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Lowed);

        return activeStatus;
    }

    private void InitializeStatusCheckBoxes()
    {
        this.StatusCheckBoxes = new StatusCheckBoxesViewModel();

        this.StatusCheckBoxes.PropertyChanged += StatusCheckBoxesViewModel_PropertyChanged;

        this.StatusCheckBoxes.PendedContent = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[0].ConfigurationName;
        this.StatusCheckBoxes.SavedContent = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[1].ConfigurationName; ;
        this.StatusCheckBoxes.BackupedContent = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[2].ConfigurationName;;
        this.StatusCheckBoxes.DeletedContent = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[3].ConfigurationName;;
        this.StatusCheckBoxes.LowedContent = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[4].ConfigurationName;;
        this.StatusCheckBoxes.PendedIsChecked = true;
        this.StatusCheckBoxes.SavedIsChecked = true;
        this.StatusCheckBoxes.BackupedIsChecked = true;
        this.StatusCheckBoxes.DeletedIsChecked = true;
        this.StatusCheckBoxes.LowedIsChecked = true;
    }

    private void InititalizeCommands()
    {
        PasteToSearchCommand = new CommandHandler(PasteToSearch, _ => true);
        SearchCommand = new CommandHandler(Search, _ => true);
        EditCommand = new CommandHandler(Edit, _ => true);
    }

    private void StatusCheckBoxesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not null && e.PropertyName.Contains("IsChecked"))
        {
            _videoInfoManagerPresentationAppService.LastSearch(GetActiveStatus(), true);
            VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_videoInfoManagerPresentationAppService.GetResults());
        }
    }

}


