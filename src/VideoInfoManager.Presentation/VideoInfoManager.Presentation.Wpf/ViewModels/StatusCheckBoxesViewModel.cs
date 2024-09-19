using System.ComponentModel;

namespace VideoInfoManager.Presentation.Wpf.ViewModels;

public class StatusCheckBoxesViewModel : ViewModelBase
{
    private string _pendedContent = string.Empty;
    private bool _pendedIsChecked;

    private string _savedContent = string.Empty;
    private bool _savedIsChecked;

    private string _backupedContent = string.Empty;
    private bool _backupedIsChecked;

    private string _deletedContent = string.Empty;
    private bool _deletedIsChecked;

    private string _lowedContent = string.Empty;
    private bool _lowedIsChecked;

    public string PendedContent
    {
        get => _pendedContent;
        set
        {
            _pendedContent = value;
            OnPropertyChanged("PendedContent");
        }
    }

    public string SavedContent
    {
        get => _savedContent;
        set
        {
            _savedContent = value;
            OnPropertyChanged("SavedContent");
        }
    }

    public string BackupedContent
    {
        get => _backupedContent;
        set
        {
            _backupedContent = value;
            OnPropertyChanged("BackupedContent");
        }
    }

    public string DeletedContent
    {
        get => _deletedContent;
        set
        {
            _deletedContent = value;
            OnPropertyChanged("DeletedContent");
        }
    }

    public string LowedContent
    {
        get => _lowedContent;
        set
        {
            _lowedContent = value;
            OnPropertyChanged("LowedContent");
        }
    }

    public bool PendedIsChecked
    {
        get => _pendedIsChecked;
        set
        {
            _pendedIsChecked = value;
            OnPropertyChanged("PendedIsChecked");
        }
    }

    public bool SavedIsChecked
    {
        get => _savedIsChecked;
        set
        {
            _savedIsChecked = value;
            OnPropertyChanged("SavedIsChecked");
        }
    }

    public bool BackupedIsChecked
    {
        get => _backupedIsChecked;
        set
        {
            _backupedIsChecked = value;
            OnPropertyChanged("BackupedIsChecked");
        }
    }

    public bool DeletedIsChecked
    {
        get => _deletedIsChecked;
        set
        {
            _deletedIsChecked = value;
            OnPropertyChanged("DeletedIsChecked");
        }
    }

    public bool LowedIsChecked
    {
        get => _lowedIsChecked;
        set
        {
            _lowedIsChecked = value;
            OnPropertyChanged("LowedIsChecked");
        }
    }

}