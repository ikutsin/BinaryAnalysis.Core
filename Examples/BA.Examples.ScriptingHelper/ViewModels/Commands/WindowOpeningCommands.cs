using System;
using System.Windows.Input;

namespace BA.Examples.ScriptingHelper.ViewModels.Commands
{
    class WindowOpeningCommands
    {
        private static readonly Lazy<RelayCommand> openFiddlerHelperCommand
            = new Lazy<RelayCommand>(() => new RelayCommand(
            (name) =>
                {
                    new FiddlerWindow().ShowDialog();
                }, (name) => true)
        );

        public static ICommand OpenFiddlerHelperCommand
        {
            get { return openFiddlerHelperCommand.Value; }
        }

        private static readonly Lazy<RelayCommand> openStateBrowserCommand
            = new Lazy<RelayCommand>(() => new RelayCommand(
            (name) =>
            {
                new StateBrowserWindow().Show();
            }, (name) => true)
        );

        public static ICommand OpenStateBrowserCommand
        {
            get { return openStateBrowserCommand.Value; }
        }

        private static readonly Lazy<RelayCommand> openTaxonomyTreeWindowCommand
            = new Lazy<RelayCommand>(() => new RelayCommand(
            (name) =>
            {
                new TaxonomyTreeWindow().Show();
            }, (name) => true)
        );

        public static ICommand OpenTaxonomyTreeWindowCommand
        {
            get { return openTaxonomyTreeWindowCommand.Value; }
        }

        private static readonly Lazy<RelayCommand> openSimpleTextWindowCommand
            = new Lazy<RelayCommand>(() => new RelayCommand(
            (name) =>
            {
                new SimpleTextWindow().Show();
            }, (name) => true)
        );

        public static ICommand OpenSimpleTextWindowCommand
        {
            get { return openSimpleTextWindowCommand.Value; }
        }
    }
}
