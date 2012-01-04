using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BA.Examples.ScriptingHelper.Logic;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels;
using BA.Examples.ScriptingHelper.ViewModels.Utils;
using System.Threading;
using mshtml;

namespace BA.Examples.ScriptingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FiddlerWindow : Window
    {
        public const int FIDDLER_PORT = 55555;
        public static WebBrowser BrowserInstance
        {
            get { return Instance.webBrowser; }
        }

        public static FiddlerWindow Instance { get; private set; }

        public FiddlerWindow()
        {
            if (Instance != null) throw new Exception("Can be only one FiddlerWindow instance");
            Instance = this;

            FiddlerHelper.ClearCache();
            FiddlerHelper.StartListening(FIDDLER_PORT);
            this.Closing += (s, e) =>
                                {
                                    FiddlerHelper.StopListening(); 
                                    Instance = null;
                                };

            InitializeComponent();
        }

        public void StartStep()
        {
            var mw = FiddlerWindow.Instance;
            mw.ClearSteps();
        } 

        public void AddLogEntry(object o)
        {
            LogEntry entry = new LogEntry() { Text = o.ToString() };
            if (DataContext == null) return;
            ViewModel.LogEntries.Insert(0, entry);
        }

        private FiddlerWindowVm ViewModel
        {
            get { return (DataContext as FiddlerWindowVm); }
        }

        public void ClearLog()
        {
            ViewModel.LogEntries.Clear();
        }
        public void AddSessionStep(FiddlerSessionHolder session)
        {
            ViewModel.Output.AddSession(session);
        }
        public void ClearSteps()
        {
            ViewModel.Output.Clear();
            ViewModel.PageInfo = new FiddlerPageInformationVm();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FiddlerHelper.RefreshIESettings("localhost:" + FiddlerWindow.FIDDLER_PORT);
            webBrowser.Navigated += webBrowser_Navigated;
            webBrowser.Navigating += webBrowser_Navigating;
            webBrowser.LoadCompleted += webBrowser_LoadCompleted;
        }

        void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            new Action(() =>
            {
                Thread.Sleep(500);
                FiddlerHelper.WaitForSessionStackComplete();
                FiddlerHelper.Log("all done");

                App.CurrentApp.InRenderAction((_)=>
                {
                    HTMLDocument dom = (HTMLDocument)webBrowser.Document;
                    Analyse(dom);
                }, null, true);
            }).BeginInvoke(null, null);
        }

        private void Analyse(HTMLDocument dom)
        {
            if (dom.all != null)
            {
                ViewModel.PageInfo.Bind(dom);
                ViewModel.PageHtmlDomInfo.Bind(dom, null);
                ViewModel.PageDetail.Bind(dom);
            }
        }

        Uri LastRequest = new Uri("http://non-existent.gg");
        void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if(LastRequest==e.Uri) return; 
            
            LastRequest = e.Uri;
            FiddlerHelper.StartNewSessionsStack();
            (DataContext as FiddlerWindowVm).Output.StartStep(e.Uri.ToString());
        }

        void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
        }
    }
}
