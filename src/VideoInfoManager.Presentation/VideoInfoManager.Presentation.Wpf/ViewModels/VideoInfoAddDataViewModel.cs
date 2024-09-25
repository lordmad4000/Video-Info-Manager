using System.Text;
using System.Windows.Controls;
using System.Windows;
using VideoInfoManager.Presentation.CrossCutting.Services;
using VideoInfoManager.Presentation.CrossCutting.Models;
using VideoInfoManager.Domain.Enums;

namespace VideoInfoManager.Presentation.Wpf.ViewModels;

public class VideoInfoAddDataViewModel : ViewModelBase
{
    private readonly IVideoInfoManagerPresentationAppService _videoInfoManagerPresentationAppService;

    public VideoInfoAddDataViewModel(IVideoInfoManagerPresentationAppService videoInfoManagerPresentationAppService)
    {
        _videoInfoManagerPresentationAppService = videoInfoManagerPresentationAppService;

        InitializeButtons();
    }

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
        string? textData = null;
        if (e.Data.GetDataPresent(DataFormats.Text))
        {
            var dragDropData = e.Data.GetData(DataFormats.Text);
            if (dragDropData != null)
            {
                textData = dragDropData.ToString();
            }
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var dragDropData = e.Data.GetData(DataFormats.FileDrop, true);
            if (dragDropData != null)
            {
                var fileList = (string[])dragDropData;
                var text = new StringBuilder();
                foreach (var file in fileList)
                {
                    string normalizedFileName = _videoInfoManagerPresentationAppService.NormalizeFileName(file);
                    text.Append($"{normalizedFileName}{Environment.NewLine}");
                }
                textData = text.ToString();
            }
        }
        if (textData != null)
        {
            if (sender.GetType() == typeof(Button))
            {
                var button = sender as Button;
                if (button != null)
                {
                    string buttonText = (string)button.Content;
                    if (MessageBox.Show($"Add/Update {buttonText} Data to Data Base?", "Add/Update Data", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                    //rtb.Text += textData;
                }
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

    private void InitializeButtons()
    {
        PendedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Pended);
        SavedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Saved);
        BackupedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Backuped);
        DeletedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Deleted);
        LowedButtonContent = GetConfigurationNameByVideoInfoStatus(VideoInfoStatusEnum.Lowed);
    }

}


