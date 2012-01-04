using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels;
using mshtml;

namespace BA.Examples.ScriptingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StateBrowserWindow : Window
    {
        public StateBrowserWindow()
        {
            InitializeComponent();
        }

        public void AddLogEntry(object o)
        {
            LogEntry entry = new LogEntry() { Text = o.ToString() };
            if (DataContext == null) return;
            ViewModel.LogEntries.Insert(0, entry);
        }

        private StateBrowserWindowVm ViewModel
        {
            get { return (DataContext as StateBrowserWindowVm); }
        }

        public void ClearLog()
        {
            ViewModel.LogEntries.Clear();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ServiceClient != null)
            {
                addressBar.TextRequestAction =
                    (t, s) =>
                        {
                            TryServiceAction(
                                () =>
                                    {
                                        var requests = ViewModel.ServiceClient.RequestContains(s);
                                        foreach (var request in requests)
                                        {
                                            t.AddItem(new AutoCompleteEntry(request,
                                                                            request.Split(new[] {'.', '\\', '/'})));
                                        }
                                    });
                        };

                btnLoadState.Click +=
                    (s, ea) => TryServiceAction(()=>SetBrowserContent(ViewModel.ServiceClient.LoadStateResponse(addressBar.Text)));

                btnLoadFixedState.Click +=
                    (s, ea) => TryServiceAction(()=>SetBrowserContent(ViewModel.ServiceClient.LoadFixedStateResponse(addressBar.Text)));

                webBrowser.Navigated += webBrowser_Navigated;
                webBrowser.Navigating += webBrowser_Navigating;
                webBrowser.LoadCompleted += webBrowser_LoadCompleted;
                HideScriptErrors(webBrowser, true);

                btnRegex.Click +=
                    (s, ea) =>
                        {
                            if (currentContent != null)
                            {
                                new SimpleTextWindow(currentContent, "State regexer").ShowDialog();
                            }
                        };
            }
        }
        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }


        void TryServiceAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Service endpoint can not be opened. Please reopen the window.");
                this.Close();
            }

        }

        private string currentContent;
        void SetBrowserContent(string content)
        {
            if (content == null) MessageBox.Show("State content not found");
            else
            {
                currentContent = content;
                //<!-- saved from url=(0016)http://localhost -->
                webBrowser.NavigateToString(content);
            }
        }

        void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            new Action(() =>
            {
                Thread.Sleep(500);

                App.CurrentApp.InRenderAction((_)=>
                {
                    HTMLDocument dom = (HTMLDocument)webBrowser.Document;
                    if (dom.all != null)
                    {
                        ViewModel.PageHtmlDomInfo.Bind(dom, currentContent);
                    }
                }, null, true);
            }).BeginInvoke(null, null);
        }

        void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
        }
    }
}
