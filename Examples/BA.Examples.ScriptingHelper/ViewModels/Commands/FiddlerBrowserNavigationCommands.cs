using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using BA.Examples.ScriptingHelper.ViewModels.Utils;

namespace BA.Examples.ScriptingHelper.ViewModels.Commands
{
    public static class FiddlerBrowserNavigationCommands
    {
        static WebBrowser WebBrowserInstance
        {
            get { return FiddlerWindow.BrowserInstance; }
        }

        private static readonly Lazy<RelayCommand> navigateNewHistoryCommand = new Lazy<RelayCommand>(
        () => new RelayCommand(
            (url) =>
                {
                    try
                    {
                        App.CurrentApp.InRenderAction((_) =>
                                                          {
                                                              FiddlerHelper.StartNewSessionsStack();
                                                              FiddlerWindow.Instance.ClearLog();
                                                              FiddlerWindow.Instance.StartStep();
                                                              WebBrowserInstance.Navigate(url.ToString());
                                                          }, null, true);
                    } catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                },
            (_)=> FiddlerHistoryCommands.ClearHistoryCommand.CanExecute(_))
        );
        public static ICommand NavigateNewHistoryCommand
        {
            get { return navigateNewHistoryCommand.Value; }
        }
    }
}
