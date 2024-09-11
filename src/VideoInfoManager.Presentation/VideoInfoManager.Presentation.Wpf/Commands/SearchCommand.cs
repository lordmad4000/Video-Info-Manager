using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.Wpf.Handlers;
using VideoInfoManager.Presentation.Wpf.Services;

namespace VideoInfoManager.Presentation.Wpf.Commands
{
    public class SearchCommand
    {
        private readonly SearchService _searchService;

        public SearchCommand(SearchService searchService)
        {
            _searchService = searchService;
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
                _searchService.Search(search);
                //VideoInfoResults = new ObservableCollection<VideoInfoDTO>(_searchService.Results);
            }
            //_searchService.Search(new string[] { _searchText });
        }

        private bool AddCanExecute(object parameter)
        {
            return true;
        }

    }
}
