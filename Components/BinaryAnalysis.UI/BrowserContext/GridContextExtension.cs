using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.UI.Commons.Data;
using Newtonsoft.Json;

namespace BinaryAnalysis.UI.BrowserContext
{
    [JsonObject]
    public class QjGridColumnFormat
    {
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public string index { get; set; }

        [JsonProperty]
        public string formatter { get; set; }
        [JsonProperty]
        public Dictionary<string, object> formatoptions { get; set; }

        [JsonProperty]
        public string defval { get; set; }
        [JsonProperty]
        public bool editable { get; set; }

        [JsonProperty]
        public bool resizable { get; set; }
        
        [JsonProperty]
        public bool sortable { get; set; }
        [JsonProperty]
        public string sorttype { get; set; }

        [JsonProperty]
        public bool search { get; set; }
        [JsonProperty]
        public Dictionary<string, object> searchoptions { get; set; }

        [JsonProperty]
        public bool hidden { get; set; }
        [JsonProperty]
        public bool title { get; set; }
        [JsonProperty]
        public bool viewable { get; set; }

        public QjGridColumnFormat(string index)
        {
            this.index = name = index;
            resizable = search = title = viewable = true;
            searchoptions = new Dictionary<string, object>();
            formatoptions = new Dictionary<string, object>();
        }

        public static QjGridColumnFormat Create(PropertyInfo pi)
        {
            return Create(pi.Name, pi.PropertyType);
        }
        public static QjGridColumnFormat Create(string name, Type type)
        {
            var result = new QjGridColumnFormat(name);
            if (type.IsEnum)
            {
                result.formatter = "enumer";
                result.formatoptions.Add("name", type.AssemblyQualifiedName);
                result.search = false;
            }
            else if (type.IsArray)
            {
                result.formatter = "array";
                result.formatoptions.Add("name", type.AssemblyQualifiedName);
                result.search = false;
            }
            else
            {
                switch (type.Name)
                {
                    case "DateTime":
                        result.formatter = "date";
                        result.search = false;
                        break;
                    case "Boolean":
                        result.formatter = "checkbox";
                        result.search = false;
                        break;
                    case "String":
                        result.formatter = "encode";
                        result.search = true;
                        break;
                    case "Int32":
                        result.formatter = "raw";
                        result.search = true;
                        break;
                    case "Object":
                        result.formatter = "variant";
                        result.search = true;
                        break;
                    default:
                        result.formatter = "raw";
                        result.search = false;
                        break;
                }
            }
            return result;
        }
    }

    [ComVisible(true)]
    public class GridContextExtension : IBrowserContextExtension
    {
        private readonly RepositoryFinder _repoFinder;
        private readonly IComponentContext _componentContext;

        public static T Cast<T>(object o)
        {
            return (T)o;
        }

        public GridContextExtension(RepositoryFinder repoFinder,
            IComponentContext componentContext)
        {
            _repoFinder = repoFinder;
            _componentContext = componentContext;
        }

        protected Dictionary<string, string> ParsePostData(string postdata)
        {
            return postdata.Split('|').Select(x => x.Split('='))
                .ToDictionary(k => k[0], v => v[1]);
        }

        protected IEnumerable<Tuple<string, string>> GetMappings()
        {
            return _repoFinder.Mappings
                .Select(x => new Tuple<string, string>
                    (x.Item2.AssemblyQualifiedName, x.Item1.AssemblyQualifiedName));
        }

        public string getAvailableMappings()
        {
            return JsonConvert.SerializeObject(_repoFinder.Mappings.Select(t => t.Item2.AssemblyQualifiedName));
        }
        public string getEnumValue(string typename, int value)
        {
            var type = Type.GetType(typename);
            return Enum.GetName(type, value);
        }
        public string getColNames(string boxname)
        {
            var bType = Type.GetType(boxname);
            return JsonConvert.SerializeObject(
                bType.GetProperties().Reverse().Select(p => p.Name));
        }

        public string getColModel(string boxname)
        {
            var bType = Type.GetType(boxname);
            return JsonConvert.SerializeObject(
                bType.GetProperties().Reverse().Select(QjGridColumnFormat.Create));
        }
        /*
        _search=false (boolean)
        nd=1312824491427 (number)
        rows=10 (number)
        page=1 (number)
        sidx= (string)
        sord=asc (string)
         */
        public string getEntries(string boxname, int rows, int page, string sidx, bool ascOrder, bool search, string postdata)
        {
            var bType = Type.GetType(boxname);
            var eType = _repoFinder.Mappings.First(t => t.Item2 == bType).Item1;

            var commandType = typeof(JqGridCustomCriteria<,>).MakeGenericType(bType, eType);
            var command = _componentContext.Resolve(commandType);

            IRepository repo = commandType.GetProperty("UsedRepository").GetValue(command, null) as IRepository;
            commandType.GetMethod("SetParameters")
                .Invoke(command,
                        new object[]
                            {
                                rows, page, sidx, ascOrder,
                                search ? ParsePostData(postdata) : null
                            });
            var box = commandType.GetMethod("ListResultBox").Invoke(command, null) as IBox;
            var obj = new
                          {
                              total = (int)Math.Ceiling(repo.Count() / (0.0 + rows)),
                              page = page,
                              records = box.Count,
                              rows = box
                          };
            return JsonConvert.SerializeObject(obj);
        }
    }
}
