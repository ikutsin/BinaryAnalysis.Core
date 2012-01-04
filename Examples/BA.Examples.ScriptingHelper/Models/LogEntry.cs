using System.Windows;

namespace BA.Examples.ScriptingHelper.Models
{
    public class LogEntry : DependencyObject
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (LogEntry));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public override string ToString()
        {
            return Text;
        }
    }
}
