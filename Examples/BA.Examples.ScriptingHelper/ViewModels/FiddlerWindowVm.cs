using System.Collections.ObjectModel;
using System.Windows.Input;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels.Commands;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class FiddlerWindowVm : AbstractVm
    {
        private FiddlerHistoryOutputVm output;
        public FiddlerHistoryOutputVm Output { get { return output; } set { output = value; NotifyPropertyChanged("Output"); } }

        private FiddlerPageInformationVm pageInfo;
        public FiddlerPageInformationVm PageInfo { get { return pageInfo; } set { pageInfo = value; NotifyPropertyChanged("PageInfo"); } }

        private FiddlerPageHtmlDomInformationVm pageHtmlDomInfo;
        public FiddlerPageHtmlDomInformationVm PageHtmlDomInfo { get { return pageHtmlDomInfo; } set { pageHtmlDomInfo = value; NotifyPropertyChanged("PageHtmlDomInfo"); } }

        public ObservableCollection<LogEntry> LogEntries { get; set; }

        private FiddlerPageDetailVm pageDetail;
        public FiddlerPageDetailVm PageDetail { get { return pageDetail; } set { pageDetail = value; NotifyPropertyChanged("PageDetail"); } }


        public FiddlerWindowVm()
        {
            LogEntries = new ObservableCollection<LogEntry>();
            Output = new FiddlerHistoryOutputVm();
            PageInfo = new FiddlerPageInformationVm();
            PageHtmlDomInfo = new FiddlerPageHtmlDomInformationVm();
            //PageDomInfo = new FiddlerPageHtmlDomInformationVm();
            pageDetail = new FiddlerPageDetailVm();
        }
        public ICommand NavigateNewHistory
        {
            get { return FiddlerBrowserNavigationCommands.NavigateNewHistoryCommand;  }
        }
    }
}
