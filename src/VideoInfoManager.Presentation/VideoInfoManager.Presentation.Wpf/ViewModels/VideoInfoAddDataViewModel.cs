using System.Text;
using System.Windows.Controls;
using System.Windows;
using VideoInfoManager.Presentation.CrossCutting.Services;
using VideoInfoManager.Presentation.CrossCutting.Models;
using VideoInfoManager.Domain.Enums;
using System.Windows.Input;
using VideoInfoManager.Presentation.Wpf.Handlers;
using VideoInfoManager.Presentation.Crosscutting.Extensions;
using System.Collections.ObjectModel;
using VideoInfoManager.Application.DTOs;

namespace VideoInfoManager.Presentation.Wpf.ViewModels;

public class VideoInfoAddDataViewModel : ViewModelBase
{
    private readonly IVideoInfoManagerPresentationAppService _videoInfoManagerPresentationAppService;
    private readonly VideoInfoSearchViewModel _videoInfoSearchViewModel;
    private readonly MainWindowViewModel _mainWindowViewModel;

    public VideoInfoAddDataViewModel(IVideoInfoManagerPresentationAppService videoInfoManagerPresentationAppService, VideoInfoSearchViewModel videoInfoSearchViewModel, MainWindowViewModel mainWindowViewModel)
    {
        _videoInfoManagerPresentationAppService = videoInfoManagerPresentationAppService;
        _videoInfoSearchViewModel = videoInfoSearchViewModel;
        _mainWindowViewModel = mainWindowViewModel;
        AddDataCommand = new CommandHandler(AddDataFromClipboard, _ => true);
        PasteToMultiSearchTextBoxCommand = new CommandHandler(PasteToMultiSearchTextBox, _ => true);
        CutFirstFromMultiSearchTextBoxCommand = new CommandHandler(CutFirstFromMultiSearchTextBox, _ => true);
        SearchCommand = new CommandHandler(SearchByAuthor, _ => true);

        InitializeButtons();
    }

    public ICommand AddDataCommand { get; private set; } = new CommandHandler(c => c.ToString(), _ => true);
    public ICommand PasteToMultiSearchTextBoxCommand { get; private set; } = new CommandHandler(c => c.ToString(), _ => true);
    public ICommand CutFirstFromMultiSearchTextBoxCommand { get; private set; } = new CommandHandler(c => c.ToString(), _ => true);
    public ICommand SearchCommand { get; private set; } = new CommandHandler(c => c.ToString(), _ => true);

    private string _multiSearchTextBoxText = string.Empty;
    public string MultiSearchTextBoxText
    {
        get => _multiSearchTextBoxText;
        set
        {
            _multiSearchTextBoxText = value;
            OnPropertyChanged(nameof(MultiSearchTextBoxText));
        }
    }

    private string _pendedButtonContent = string.Empty;
    public string PendedButtonContent
    {
        get => _pendedButtonContent;
        set
        {
            _pendedButtonContent = value;
            OnPropertyChanged(nameof(PendedButtonContent));
        }
    }

    private string _savedButtonContent = string.Empty;
    public string SavedButtonContent
    {
        get => _savedButtonContent;
        set
        {
            _savedButtonContent = value;
            OnPropertyChanged(nameof(SavedButtonContent));
        }
    }

    private string _backupedButtonContent = string.Empty;
    public string BackupedButtonContent
    {
        get => _backupedButtonContent;
        set
        {
            _backupedButtonContent = value;
            OnPropertyChanged(nameof(BackupedButtonContent));
        }
    }

    private string _deletedButtonContent = string.Empty;
    public string DeletedButtonContent
    {
        get => _deletedButtonContent;
        set
        {
            _deletedButtonContent = value;
            OnPropertyChanged(nameof(DeletedButtonContent));
        }
    }

    private string _lowedButtonContent = string.Empty;
    public string LowedButtonContent
    {
        get => _lowedButtonContent;
        set
        {
            _lowedButtonContent = value;
            OnPropertyChanged(nameof(LowedButtonContent));
        }
    }

    public void AddData_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data == null)
        {
            return;
        }
        if (e.Data.GetDataPresent(DataFormats.Text))
        {
            string? data = (string)e.Data.GetData(DataFormats.Text);
            if (data != null)
            {
                AddData(sender, data.ToString());
            }
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[]? data = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            if (data != null)
            {
                AddData(sender, NormalizeAndConvertToStringWithNewLineSeparators(data));
            }
        }
    }

    private void PasteToMultiSearchTextBox(object parameter)
    {
        if (Clipboard.ContainsText())
        {
            string clipBoardText = Clipboard.GetText();
            var splitText = clipBoardText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (splitText is not null && splitText.Length > 0)
            {
                var text = new StringBuilder();
                foreach (string item in splitText)
                {
                    string videoName = _videoInfoManagerPresentationAppService.RenameVideoInfoName(item);
                    text.Append($"{videoName}{Environment.NewLine}");
                }

                MultiSearchTextBoxText += text;
            }
        }
        if (Clipboard.ContainsFileDropList())
        {
            var fileList = Clipboard.GetFileDropList();
            if (fileList is not null && fileList.Count > 0)
            {
                var text = new StringBuilder();
                foreach (var file in fileList)
                {
                    if (file is not null)
                    {
                        string videoName = _videoInfoManagerPresentationAppService.NormalizeFileName(file);
                        text.Append($"{_videoInfoManagerPresentationAppService.RenameVideoInfoName(videoName)}{Environment.NewLine}");
                    }
                }

                MultiSearchTextBoxText += text;
            }
        }
    }

    private void CutFirstFromMultiSearchTextBox(object parameter)
    {
        if (string.IsNullOrEmpty(MultiSearchTextBoxText))
        {
            return;
        }
        string result = _videoInfoManagerPresentationAppService.RemoveFirstItem(MultiSearchTextBoxText);
        string cutText = string.IsNullOrEmpty(result) is true
                         ? MultiSearchTextBoxText
                         : MultiSearchTextBoxText.Replace(result, ""); ;

        if (string.IsNullOrEmpty(cutText) is false)
        {
            Clipboard.SetText(cutText.RemoveNewLine());
            MultiSearchTextBoxText = result;
        }
    }

    private void SearchByAuthor(object parameter)
    {
        if (string.IsNullOrEmpty(MultiSearchTextBoxText))
        {
            return;
        }
        String[] lines = MultiSearchTextBoxText.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
        var search = new string[] { MultiSearchTextBoxText };
        if (lines.Count() > 1)
        {
            search = lines;
        }

        _videoInfoManagerPresentationAppService.Search(search, GetActiveStatus(), true);
        _videoInfoSearchViewModel.VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_videoInfoManagerPresentationAppService.GetResults());

        if (_mainWindowViewModel.TabControlIndex != 0)
        {
            _mainWindowViewModel.TabControlIndex = 0;
        }
    }

    private void AddDataFromClipboard(object parameter)
    {
        if (Clipboard.ContainsText())
        {
            string data = Clipboard.GetText();
            if (string.IsNullOrEmpty(data) == false)
            {
                AddData(parameter, data);
            }
        }
        if (Clipboard.ContainsFileDropList())
        {
            string[]? data = (string[])Clipboard.GetData("FileDrop");
            if (data != null)
            {
                AddData(parameter, NormalizeAndConvertToStringWithNewLineSeparators(data));
            }
        }
    }

    private string? NormalizeAndConvertToStringWithNewLineSeparators(string[] data)
    {
        if (data is null)
        {
            return null;
        }
        var text = new StringBuilder();
        foreach (var file in data)
        {
            string normalizedFileName = _videoInfoManagerPresentationAppService.NormalizeFileName(file);
            text.Append($"{normalizedFileName}{Environment.NewLine}");
        }

        return text.ToString();
    }

    private void AddData(object sender, string? textData)
    {
        if (textData is null)
        {
            return;
        }
        if (sender.GetType() == typeof(Button))
        {
            var button = sender as Button;
            if (button != null)
            {
                string buttonText = (string)button.Content;
                if (MessageBox.Show(App.Current.MainWindow, $"Add/Update {buttonText} Data to Data Base?", "Add/Update Data", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var results = _videoInfoManagerPresentationAppService.ProcessData(textData, buttonText);
                    if (string.IsNullOrEmpty(results) == false)
                    {
                        string state = results.Split('[', ']')[1];
                        results = results.Replace(state, buttonText);
                        MessageBox.Show(results);
                    }
                }
            }
        }
        if (sender.GetType() == typeof(TextBox))
        {
            var rtb = sender as TextBox;
            if (rtb != null)
            {
                MultiSearchTextBoxText += textData;
            }
        }
    }

    private string GetVideoInfoStatusByConfigurationName(string configurationName) =>
        _videoInfoManagerPresentationAppService.GetVideoInfoStatusByConfigurationName(configurationName).ConfigurationName;


    private string GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum videoInfoStatusEnum)
    {
        VideoInfoStatus? videoInfoStatus = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()
                                                                                  .FirstOrDefault(c => c.Status == videoInfoStatusEnum);

        if (videoInfoStatus == null)
        {
            return "Pended";
        }

        return videoInfoStatus.ConfigurationName;
    }

    private List<VideoInfoStatusEnum> GetActiveStatus()
    {
        var activeStatus = new List<VideoInfoStatusEnum>();
        if (_videoInfoSearchViewModel.StatusCheckBoxes.PendedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Pended);
        if (_videoInfoSearchViewModel.StatusCheckBoxes.SavedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Saved);
        if (_videoInfoSearchViewModel.StatusCheckBoxes.BackupedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Backuped);
        if (_videoInfoSearchViewModel.StatusCheckBoxes.DeletedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Deleted);
        if (_videoInfoSearchViewModel.StatusCheckBoxes.LowedIsChecked) activeStatus.Add(VideoInfoStatusEnum.Lowed);

        return activeStatus;
    }


    private void InitializeButtons()
    {
        PendedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Pended);
        SavedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Saved);
        BackupedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Backuped);
        DeletedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Deleted);
        LowedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Lowed);
    }

}


