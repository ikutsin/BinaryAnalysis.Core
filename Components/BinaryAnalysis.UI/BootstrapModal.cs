using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Extensions.FileStorage;
using BinaryAnalysis.Extensions.Health;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using BinaryAnalysis.UI.BrowserContext;
using BinaryAnalysis.UI.Commons;
using BinaryAnalysis.UI.Modules;
using log4net;
using Microsoft.Win32;

namespace BinaryAnalysis.UI
{
    public partial class BootstrapModal : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BootstrapModal));

        public bool IsInitialized { get; protected set; }

        public BootstrapModal()
        {
            IsInitialized = false;
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            Task.Factory.StartNew(Initialize);
            base.OnLoad(e);
        }

        public void SetProgressAsync(int progress, string text = null)
        {
            BeginInvoke(new Action(
                                 () =>
                                     {
                                         if (progress >= 0) this.progressBar.Value = progress;
                                         if(text!=null)this.progressLbl.Text = text;
                                         if(progress>=100) this.Close();
                                     }));
        }

        private void Initialize()
        {
            using (var logSubscribe = FormsLogInjectionModule.NextLoggingEvent().ToObservable(
                System.Reactive.Concurrency.Scheduler.NewThread)
                .Where(l=>l!=null)
                .Subscribe(l => SetProgressAsync(-1, l.RenderedMessage)))
            {
                try
                {
                    SetProgressAsync(0, "Building factory");
                    var bs = new Bootstrap();
                    bs.Init();
                    SetProgressAsync(30, "Init database");

                    //trackers
                    //var pTracker = bs.Container.Resolve<PerformanceCountersTracker>();
                    //pTracker.Start(TimeSpan.FromSeconds(10), "Performance");

                    //var rTracker = bs.Container.Resolve<RandomTracker>();
                    //rTracker.Start(TimeSpan.FromSeconds(1), "Random");

                    var dbCtx = bs.Container.Resolve<IDbContext>();

                    int step = 50;
                    SetProgressAsync(step, "Unpacking zip files");

                    var packagesFolder = Path.Combine(Environment.CurrentDirectory, "JS", "Packages");
                    var packagesOutputFolder = Path.Combine(Environment.CurrentDirectory, "JS");
                    if (!Directory.Exists(packagesFolder)) throw new Exception("Component folder not found: " + packagesFolder);

                    if (!ModalBootstrapConfigSection.GetConfig().JsComponents.Disabled)
                    {
                        var packStep = 20 / Math.Max(1,ModalBootstrapConfigSection.GetConfig().JsComponents.Count);
                        Action<NameValueConfigElement> Unpack =
                            (element) =>
                            {
                                step += packStep;
                                SetProgressAsync(step, "Unpacking: " + element.Name);
                                ZipFunctions.ExtractZipFile(
                                    Path.Combine(packagesFolder, element.Name),
                                    Path.Combine(packagesOutputFolder, element.Value), ZipFunctions.Functions.IfNewer);
                            }; 
                        foreach (NameValueConfigElement elem in ModalBootstrapConfigSection.GetConfig().JsComponents)
                            Unpack(elem);
                    }
                    SetProgressAsync(70, "Unpack done");

                    step = 71;
                    SetProgressAsync(step, "Restoring");

                    if (!ModalBootstrapConfigSection.GetConfig().Restore.Disabled)
                    {
                        var packStep = 10 / Math.Max(1, ModalBootstrapConfigSection.GetConfig().Restore.Count);
                        Action<string, string> Restore =
                            (filename, stringType) =>
                                {
                                    var type = Type.GetType(stringType);
                                    if(type==null) throw new Exception(stringType +" can not be resolved.");
                                    if (File.Exists(filename))
                                    {
                                        step += packStep;
                                        SetProgressAsync(step, filename + " restore");
                                        INHibernateFileBackupBase httpProxyBackup = Bootstrap.Instance.Container.Resolve(type)
                                                                          as INHibernateFileBackupBase;
                                        httpProxyBackup.BackupFile = filename;
                                        httpProxyBackup.Restore();
                                    }
                                    else
                                    {
                                        MessageBox.Show(filename + " not found");
                                    }
                                };

                        ModalBootstrapConfigSection.GetConfig().Restore.Cast<NameValueConfigElement>()
                            .ToList().ForEach(e=>Restore(e.Name, e.Value));
                    }

                    //foreach (var restor in restore) Restore(restor.Item1, restor.Item2);
                    SetProgressAsync(80, "Build menu");

                    bs.Container.Resolve<CoreContextExtensions>().getMenu();

                    SetProgressAsync(99, "Setting up registry");
                    if (!ModalBootstrapConfigSection.GetConfig().Registry.Disabled)
                    {
                        throw new NotImplementedException("registry section read is not implemented, disable it!");
                    }
                    //TODO: if not mono
                    var executableFilename = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    var keys = new[] { executableFilename, "BinaryAnalysis.GUI.vshost.exe" };

                    Action<string, object, string> SetRegistryKeyOrFail =
                        (key, val, regStr) =>
                            {
                                var reg =
                                    Registry.CurrentUser.CreateSubKey(regStr);
                                if (reg == null) throw new Exception("Failed registry: " + regStr);
                                reg.SetValue(key, val);
                            };

                    foreach (var key in keys)
                    {
                        SetRegistryKeyOrFail(key, 0, @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BLOCK_LMZ_IMG");
                        SetRegistryKeyOrFail(key, 0, @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BLOCK_LMZ_SCRIPT");
                    }

                    Thread.Sleep(500);
                    SetProgressAsync(100, "All done");
                    IsInitialized = true;
                }catch(Exception ex)
                {
                    log.Error(ex);
                    MessageBox.Show(ex.Message, "Error in init.");
                    SetProgressAsync(100);
                }
            }
        }
    }
}
