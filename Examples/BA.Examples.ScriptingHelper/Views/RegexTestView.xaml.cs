using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for RegexTestView.xaml
    /// </summary>
    public partial class RegexTestView : UserControl
    {
        public RegexTestView()
        {
            InitializeComponent();

            btnApplyRegex.Click +=
                (s, e) =>
                    {
                        Highlight(tbxRegex.Text);
                        ShowMatchInfo(tbxRegex.Text);
                    };
        }

        Regex CreateRegex(string regex)
        {
            var opts = RegexOptions.ECMAScript;
            if (chkMultiline.IsChecked.Value) opts |= RegexOptions.Multiline;
            if (chkIgnorecase.IsChecked.Value) opts |= RegexOptions.IgnoreCase;
            
            try
            {
                return new Regex(regex, opts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
                return new Regex("");
            }
        }

        private void ShowMatchInfo(string regexStr)
        {
            var regex = CreateRegex(regexStr);
            StringBuilder result = new StringBuilder();

            var matches = regex.Matches(Text);


            foreach (Match match in matches)
            {
                
                result.Append("Match("+match.Index+"):");
                result.Append(" Groups:");
                result.Append(match.Groups.Count);
                result.Append(" Captures:");
                result.Append(match.Captures.Count);
                result.AppendLine();
                result.AppendLine(match.Value);
            }

            result.AppendLine();
            result.Append("Matches: ");
            result.AppendLine(matches.Count.ToString());
            result.Append("GroupNames: ");
            result.AppendLine(String.Join(", ", regex.GetGroupNames()));

            tbxMatches.Text = result.ToString();
        }

        public void Highlight(string regexStr)
        {
            Text = Text; //clean 

            var regex = CreateRegex(regexStr);
            TextPointer navigator = tbxRich.Document.ContentStart;
            while (navigator.CompareTo(tbxRich.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    HighlightRun((Run)navigator.Parent, regex);
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);                
            }
        }

        private void HighlightRun(Run run, Regex regex)
        {
            var tags = new List<Tuple<TextPointer, TextPointer, string>>();

            var matches = regex.Matches(run.Text);
            foreach (Match match in matches)
            {
                tags.Add(new Tuple<TextPointer, TextPointer, string>(
                    run.ContentStart.GetPositionAtOffset(match.Index, LogicalDirection.Forward),
                    run.ContentStart.GetPositionAtOffset(match.Index+match.Length, LogicalDirection.Backward),
                    match.Value
                    ));
            }
            foreach (var tag in tags)
            {
                TextRange range = new TextRange(tag.Item1, tag.Item2);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }

        public string Text
        {
            get
            {
                TextRange textRange = new TextRange(tbxRich.Document.ContentStart, tbxRich.Document.ContentEnd);
                return textRange.Text;
            }
            set
            {
                TextRange range = new TextRange(tbxRich.Document.ContentStart, tbxRich.Document.ContentEnd);
                range.ClearAllProperties();
                range.Text = value;
            }
        }
    }
}
