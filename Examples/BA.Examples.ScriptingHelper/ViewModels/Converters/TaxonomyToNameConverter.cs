using System;
using System.Text;
using System.Windows.Data;
using BinaryAnalysis.Data;

namespace BA.Examples.ScriptingHelper.ViewModels.Converters
{
    public class TaxonomyToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as TaxonomyNodeBoxMap;
            if (val == null) throw new NotImplementedException("Works only with non nullable TaxonomyContract");

            var b = new StringBuilder();
            b.Append(val.Name);
            return b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("No back conversion");
        }
    }
}
