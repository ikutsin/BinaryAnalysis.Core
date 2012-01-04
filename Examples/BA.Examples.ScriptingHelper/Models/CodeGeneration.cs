using System;
using System.Windows;
using System.Windows.Input;
using BA.Examples.ScriptingHelper.ViewModels.Commands;

namespace BA.Examples.ScriptingHelper.Models
{
    public class CodeGeneration : DependencyObject
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CodeGeneration));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(CodeGeneration));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CodeGeneration));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public override string ToString()
        {
            return Name + " " + Header;
        }

        private Lazy<RelayCommand> openSimpleTextWindowCommand;

        public CodeGeneration(string name)
        {
            openSimpleTextWindowCommand = new Lazy<RelayCommand>(
                () => new RelayCommand(
                          (n) =>
                              {
                                  new SimpleTextWindow(Text, Name).Show();
                              }, (n) => true)
                );
            Name = name;
        }

        public ICommand ShowInTextEditor
        {
            get { return openSimpleTextWindowCommand.Value; }
        }
    }
}
