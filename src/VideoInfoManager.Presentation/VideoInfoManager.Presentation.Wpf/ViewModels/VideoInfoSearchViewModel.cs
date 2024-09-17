using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Models;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Presentation.Wpf.Handlers;
using VideoInfoManager.Presentation.Wpf.Helpers;
using VideoInfoManager.Presentation.Wpf.Models;
using VideoInfoManager.Presentation.Wpf.Services;
using VideoInfoManager.Presentation.Wpf.Windows;

namespace VideoInfoManager.Presentation.Wpf.ViewModels;

public class VideoInfoSearchViewModel : ViewModelBase
{
    private const int MinSearchLength = 3;

    private readonly SearchService _searchService;
    private readonly IAbstractFactory<EditDialogWindow> _editDialogWindowFactory;
    private readonly List<string> _statuses;

    public VideoInfoSearchViewModel(SearchService searchService, IAbstractFactory<EditDialogWindow> editDialogWindowFactory)
    {
        _searchService = searchService;        
        _editDialogWindowFactory = editDialogWindowFactory;
        _statuses = _searchService.VideoInfoStatuses.Select(c => c.ConfigurationName)
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
        _searchService.Search(search);
        VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_searchService.Results);
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

        _editDialogWindowFactory.Create()
                                .ShowDialogWindow(videoInfoDTO.Id, videoInfoDTO.Name, videoInfoDTO.Status, _statuses);
    }

}


