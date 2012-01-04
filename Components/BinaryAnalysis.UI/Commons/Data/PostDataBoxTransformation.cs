using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using BinaryAnalysis.Box;
using BinaryAnalysis.Box.Transforations;

namespace BinaryAnalysis.UI.Commons.Data
{
    public class PostDataBoxTransformation<T> : IBaseBoxTransformation<T>
    {
        #region Implementation of IBaseBoxTransformation<T>

        public IBox<T> ToBox()
        {
            if (Results == null) return null;
            var box = new Box<T>();

            var tType = typeof (T);
            foreach (var paramz in Results.Select(HttpUtility.ParseQueryString))
            {
                var boxItem = (T)Activator.CreateInstance(typeof (T));
                foreach (string i in paramz)
                {
                    var pi = tType.GetProperty(i);
                    if (pi.PropertyType == typeof(Int32))
                    {
                        pi.SetValue(boxItem, String.IsNullOrEmpty(paramz[i])?0:Int32.Parse(paramz[i]), null);
                    }
                    else if (pi.PropertyType == typeof(Double))
                    {
                        pi.SetValue(boxItem, String.IsNullOrEmpty(paramz[i]) ? 0 : Double.Parse(paramz[i], CultureInfo.InvariantCulture), null);
                    }
                    else if (pi.PropertyType == typeof(Boolean))
                    {
                        pi.SetValue(boxItem, String.IsNullOrEmpty(paramz[i]) ? false : true, null);
                    }
                    else
                    {
                        pi.SetValue(boxItem, paramz[i], null);
                    }
                }
                var falseValues = tType.GetProperties()
                    .Where(p => p.PropertyType == typeof (Boolean))
                    .Where(p => !paramz.AllKeys.Contains(p.Name));
                foreach (var propertyInfo in falseValues)
                {
                    propertyInfo.SetValue(boxItem, false, null);
                }
                box.Add(boxItem);
            }
            return box;
        }

        public void Transform(IBox<T> box)
        {
            var ret = new List<string>();
            foreach (var o in box)
            {
                var paramz = typeof (T).GetProperties()
                    .Where(p => p.PropertyType != typeof (Boolean))
                    .Select(p => p.Name + "=" + p.GetValue(o, null)).ToList();
                paramz.AddRange(
                    typeof (T).GetProperties()
                        .Where(p => p.PropertyType == typeof (Boolean))
                        .Where(p=>(bool)p.GetValue(o, null))
                        .Select(p => p.Name + "=on")
                    );
                ret.Add(
                    String.Join("&", paramz));
            }
            Results = ret.ToArray();
        }

        public string[] Results { get; set; }

        #endregion
    }
}
