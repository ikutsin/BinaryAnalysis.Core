using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.UI.Commons;
using BinaryAnalysis.UI.Controls;
using log4net;
using Newtonsoft.Json;

namespace BinaryAnalysis.UI.BrowserContext
{
    [ComVisible(true)]
    public class CoreContextExtensions : IBrowserContextExtension
    {
        private const string _uiMenu_path = "/BinaryAnalysis.UI/Menu";
        private readonly TaxonomyTree _tree;
        private readonly BackupsResolver _backupsResolver;
        private static readonly ILog log = LogManager.GetLogger(typeof(CoreContextExtensions));

        public IEnumerable<IVisualDependencies> Dependencies { get; set; }

        public CoreContextExtensions(TaxonomyTree tree)
        {
            _tree = tree;
        }

        public string getDependencies()
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var dep in Dependencies.SelectMany(x=>x.Dependencies))
            {
                if (result.ContainsKey(dep.Key))
                {
                    result[dep.Key].AddRange(dep.Value.Where(v=>!result[dep.Key].Contains(v)));
                }
                else
                {
                    result.Add(dep.Key, dep.Value.ToList());
                }
                
            }
            return JsonConvert.SerializeObject(result);
        }

        object getMenuLocker = new object();
        private string getMenuResult = null;
        public string getMenu()
        {
            lock (getMenuLocker)
            {
                if (getMenuResult==null)
                {
                    var rootnode = _tree.FindOne(_uiMenu_path);
                    if(rootnode!=null) rootnode.Remove(true);
                    rootnode = _tree.GetOrCreatePath(_uiMenu_path, "UI menu root");
                    foreach (var mitem in Dependencies.SelectMany(x => x.MenuItems))
                    {
                        _tree.GetOrCreatePath(_uiMenu_path + "/" + mitem.Path, JsonConvert.SerializeObject(mitem.Action));
                    }
                    getMenuResult = JsonConvert.SerializeObject(GetMenuItems(rootnode));
                }
                return getMenuResult;

            }
        }
        IEnumerable<dynamic> GetMenuItems(TaxonomyNode node)
        {
            return node.Children.Select(
                n => new
                {
                    name = n.Name,
                    action = n.Description,
                    menuitems = GetMenuItems(n),
                    weight = 0
                });
        }
        public void showBrowserDialog(string title = "Browser", object content = null)
        {
            if (content == null) MessageBox.Show("Nothing to show", "Show Browser action");
            else new BrowserWindow(title, content.ToString()).ShowDialog();
        }
        public void showTextDialog(object args)
        {
            if (args == null) MessageBox.Show("Nothing to show", "ShowText action");
            else new TextViewerWindow("Backchannel text", args.ToString()).ShowDialog();
        }
    }
}
