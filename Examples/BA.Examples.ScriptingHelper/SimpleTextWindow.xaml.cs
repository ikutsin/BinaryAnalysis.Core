namespace BA.Examples.ScriptingHelper
{
    /// <summary>
    /// Interaction logic for SimpleTextWindow.xaml
    /// </summary>
    public partial class SimpleTextWindow : System.Windows.Window
    {
        public SimpleTextWindow() : this("")
        {
        }

        public SimpleTextWindow(string text, string title = "TextBox view")
        {
            InitializeComponent();
            Title = title;
            textView.Text = text;
        }
    }
}
