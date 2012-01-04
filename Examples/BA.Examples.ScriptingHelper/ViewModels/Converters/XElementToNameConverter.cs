using System;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml.Linq;

namespace BA.Examples.ScriptingHelper.ViewModels.Converters
{
    public class XElementToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as XElement;
            if (val==null) throw new NotImplementedException("Works onyl with nun nullable XElement");

            var b = new StringBuilder();
            b.Append(val.Name);
            if (val.HasAttributes)
            {
                b.Append(" ");
                b.Append(String.Join(",", val.Attributes().OrderBy(x=>x.Name.LocalName).Select(x => x.Name.LocalName.StartsWith("on")?
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
