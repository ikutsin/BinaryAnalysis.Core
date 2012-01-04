using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using BinaryAnalysis.Data;
using BinaryAnalysis.UI.BrowserContext;
using BinaryAnalysis.UI.Commons;
using BinaryAnalysis.UI.Commons.Data;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;

namespace BinaryAnalysis.UI.Modules
{
    public class CoreVisualsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //context
            builder.RegisterType<GridContextExtension>().As<IBrowserContextExtension>()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "grid")).SingleInstance();
            builder.RegisterType<LoggingContextExtensions>().As<IBrowserContextExtension>()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "logging")).SingleInstance();
            builder.RegisterType<CoreContextExtensions>().As<IBrowserContextExtension>()
                .PropertiesAutowired()
                .AsSelf()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "core")).SingleInstance();
            builder.RegisterType<TreeContextExtension>().As<IBrowserContextExtension>()
                .PropertiesAutowired()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "tree")).SingleInstance();
            builder.RegisterType<FormContextExtensions>().As<IBrowserContextExtension>()
                .PropertiesAutowired()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "form")).SingleInstance();
            builder.RegisterType<BackupContextExtensions>().As<IBrowserContextExtension>()
                .PropertiesAutowired()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "backup")).SingleInstance();

            //Visual dependencies
            builder.RegisterType<CoreVisualDependencies>()
                .As<IVisualDependencies>()
                .SingleInstance();

            //the rest
            builder.RegisterGeneric(typeof (JqGridCustomCriteria<,>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(PostDataBoxTransformation<>)).InstancePerDependency();
            builder.RegisterType<ContextExtensionsHolder>().InstancePerDependency();
        }
    }

    public class CoreVisualDependencies : IVisualDependencies
    {
        public Dictionary<string, List<string>> Dependencies
        {
            get
            {
                return new Dictionary<string, List<string>>
                    {
                        { "Templates/About.htm", new List<string> {"Redist/modernizr.js"} },
                        { "treeview/jquery.treeview.css", new List<string>
                            {
                                "treeview/jquery.treeview.js",
                                "treeview/jquery.treeview.sortable.js",
                                "treeview/jquery.treeview.edit.js",
                                "Redist/jquery.treeview.dynamic.js",
                            } 
                        },
                        { "jqgrid/js/jquery.jqGrid.src.js", new List<string>
                            {
                                "jqgrid/css/ui.jqgrid.css",
                                "jqgrid/js/i18n/grid.locale-en.js",
                            } 
                        },
                        { "BinaryAnalysis.Grid.js", new List<string>
                            {
                                "jqgrid/js/jquery.jqGrid.src.js",
                                "Redist/dateFormat.js",
                                "treeview/jquery.treeview.css",
                                "Redist/jquery.sparkline.js",
                                "jqplot/excanvas.js",
                                "jqplot/jquery.jqplot.js",
                                "jqplot/plugins/jqplot.dateAxisRenderer.js",
                                "jqplot/jquery.jqplot.css",
                            } 
                        },
                        { "Templates/Navigation.htm", new List<string> 
                            {
                                "Redist/jquery-showhide.js",
                                "Redist/jquery.hurl.js",
                                "jslinq/linq.min.js",
                                "Redist/jquery.tmpl.js",
                                "Redist/jquery.blockui.js",
                                "Redist/Superfish.js",
                                "BinaryAnalysis.css",
                                "jquery/css/smoothness/jquery-ui-1.8.14.custom.css",
                                "jquery/js/jquery-ui-1.8.14.custom.min.js",
                                "BinaryAnalysis.Grid.js",
                                "BinaryAnalysis.Form.js",
                                "Redist/jquery.deserialize.js",
                                "Redist/jquery.validate.js",
                                "Redist/jquery.validate.additional.js",
                            } 
                        },
                    };
            }
        }

        public List<VisualMenuItem> MenuItems
        {
            get
            {
                return new List<VisualMenuItem>
                           {
                               new VisualMenuItem
                                   {
                                       Action = new {action = "eval", data = "BAUI.actions.About()"},
                                       Path = "File/About", Weight = 0
                                   }
                           };
            }
        }
    }
}
