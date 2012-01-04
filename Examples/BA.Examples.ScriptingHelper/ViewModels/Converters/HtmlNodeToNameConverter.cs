using System;
using System.Linq;
using System.Text;
using System.Windows.Data;
using HtmlAgilityPack;

namespace BA.Examples.ScriptingHelper.ViewModels.Converters
{
    public class HtmlNodeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as HtmlNode;
            if (val == null) throw new NotImplementedException("Works onyl with nun nullable HtmlNode");

            var b = new StringBuilder();
            b.Append(val.Name);
            if (val.HasAttributes)
            {
                b.Append(" ");
                b.Append(String.Join(",", val.Attributes.OrderBy(x=>x.Name)
                    .Where(x => !x.Name.StartsWith(FiddlerPageHtmlDomInformationVm.CUSTOM_ATTR_PREFIX))
                    .Select(x => x.Name.StartsWith("on")?
                    String.Format("{0}='{1}'", x.Name, "[func]"):
                    String.Format("{0}='{1}'", x.Name, x.Value))));
            }
            return b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("No back conversion");
        }
    }
}
