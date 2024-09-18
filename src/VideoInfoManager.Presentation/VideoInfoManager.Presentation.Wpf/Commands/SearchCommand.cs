using System.Windows.Input;
using VideoInfoManager.Presentation.CrossCutting.Services;
using VideoInfoManager.Presentation.Wpf.Handlers;

namespace VideoInfoManager.Presentation.Wpf.Commands;

public class SearchCommand
{
    private readonly IVideoInfoManagerPresentationAppService _videoInfoManagerPresentationAppService;

    public SearchCommand(IVideoInfoManagerPresentationAppService videoInfoManagerPresentationAppService)
    {
        _videoInfoManagerPresentationAppService = videoInfoManagerPresentationAppService;
    }

    public ICommand SearchCommand1
    {
        get
        {
            return new CommandHandler(AddExecute, AddCanExecute);
        }
    }

    private void AddExecute(object parameter)
    {
        if (parameter is string)
        {
            var search = new string[] { (string)parameter };
            _videoInfoManagerPresentationAppService.Search(search);
        }
    }

    private bool AddCanExecute(object parameter)
    {
        return true;
    }

}
