using System.Collections.ObjectModel;
using Autofac;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class StateBrowserWindowVm : AbstractVm
    {
        private StatePageHtmlDomInformationVm pageHtmlDomInfo;
        public StatePageHtmlDomInformationVm PageHtmlDomInfo { get { return pageHtmlDomInfo; } set { pageHtmlDomInfo = value; NotifyPropertyChanged("PageHtmlDomInfo"); } }

        public ObservableCollection<LogEntry> LogEntries { get; set; }

        private IStateBrowsingService serviceClient;
        public IStateBrowsingService ServiceClient
        {
            get { return serviceClient ?? (serviceClient = App.CurrentApp.Container.Resolve<IStateBrowsingService>()); }
        }

        public StateBrowserWindowVm()
        {
            LogEntries = new ObservableCollection<LogEntry>();
            PageHtmlDomInfo = new StatePageHtmlDomInformationVm();
        }

    }
}
