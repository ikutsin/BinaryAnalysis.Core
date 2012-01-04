using System;
using System.Windows.Input;
using BA.Examples.ScriptingHelper.ViewModels.Utils;

namespace BA.Examples.ScriptingHelper.ViewModels.Commands
{
    public static class FiddlerHistoryCommands
    {
        private static readonly Lazy<RelayCommand> clearHistoryCommand
            = new Lazy<RelayCommand>(() => new RelayCommand(
            (_) => FiddlerHelper.StartNewSessionsStack(), (_) => true)
        );
        public static ICommand ClearHistoryCommand
        {
            get { return clearHistoryCommand.Value; }
        }


    }
}
