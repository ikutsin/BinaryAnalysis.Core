using System;
using System.CodeDom.Compiler;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Autofac;
using BinaryAnalysis.UI.BrowserContext;
using BinaryAnalysis.UI.Commons;
using log4net;
using Newtonsoft.Json;
using RazorEngine;

namespace BinaryAnalysis.UI.Controls
{
    public partial class BrowserControl : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BrowserControl));

        public BrowserControl()
        {
            InitializeComponent();
            InitBrowser();
            InitModel();
            
            Context = ProgramContext.Container.Resolve<ContextExtensionsHolder>();
        }

        private void InitModel()
        {
            Razor.SetTemplateBase(typeof (RazorTemplateBase<>));
            BaseModel = new ExpandoObject();
            BaseModel.BaseUrl = JsUrl("")+"/";
            BaseModel.Title = "Title";
        }

        public ContextExtensionsHolder Context { get; set; }

        private void InitBrowser()
        {
            browser.AllowWebBrowserDrop = false;
            browser.WebBrowserShortcutsEnabled = false;

            browser.ScriptErrorsSuppressed = false;
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            browser.Navigated += new WebBrowserNavigatedEventHandler(browser_Navigated);

            browser.IsWebBrowserContextMenuEnabled = false;
            browser.WebBrowserShortcutsEnabled = true;

            contextMenu.Items.Add("Cut", null, (s, e) => browser.Document.ExecCommand("Cut", false, null));
            contextMenu.Items.Add("Copy", null, (s, e) => browser.Document.ExecCommand("Copy", false, null));
            contextMenu.Items.Add("Paste", null, (s, e) => browser.Document.ExecCommand("Paste", false, null));
            contextMenu.Items.Add(new ToolStripSeparator());

            //todo debug
            contextMenu.Items.Add("Refresh", null,
                                  (s, e) =>
                                      {
                                          log.Debug("Refreshing document.");
                                          browser.Refresh(WebBrowserRefreshOption.Normal);
                                      });
            contextMenu.Items.Add("Restart", null,
                                  (s, e) =>
                                  {
                                      log.Debug("Restarting document.");
                                      DisplayDocument(currentDisplayDocument);
                                  });
            contextMenu.Items.Add("Document source", null, (s, e) => new TextViewerWindow(browser.DocumentTitle, browser.DocumentText).Show());
            contextMenu.Items.Add("Rendered source", null, (s, e) => new TextViewerWindow(browser.DocumentTitle, browser.Document.All[1].OuterHtml).Show());
            browser.ContextMenuStrip = contextMenu;
        }

        void browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            var b = ((WebBrowser)sender);
            if (Context == null) throw new Exception("Oops.. Browsing context not set");
            b.ObjectForScripting = Context;
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs ea)
        {
            var b = ((WebBrowser) sender);
            b.Document.Window.Error +=
                (s, e) =>
                    {
                        log.Warn(e.Description);
                        //e.Handled = true;
                    };
        }

        public string JsDirectory
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "JS");
            }
        }
        protected string JsUrl(string name)
        {
            return "file://" + Path.Combine(JsDirectory, name).Replace("\\", "/");
        }

        public dynamic BaseModel { get; set; }

        public void DisplayTemplate(string templateName, dynamic model = null)
        {
            if (model == null) model = BaseModel;
            try
            {
                var templatePath = Path.Combine(JsDirectory, "Templates", templateName + ".html");
                var filename = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-ffffff") + ".html";
                var result = Razor.Parse(File.ReadAllText(templatePath), model, filename);

                var tempFolder = Path.Combine(Environment.CurrentDirectory, "JS", "Temp");

                if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
                File.WriteAllText(Path.Combine(tempFolder, filename), result);
                DisplayDocument(JsUrl("Temp/" + filename));
            }
            catch (RazorEngine.Templating.TemplateCompilationException ex)
            {
                var sb = new StringBuilder(ex.Message +"(");
                foreach (CompilerError compilerError in ex.Errors)
                {
                    sb.Append(compilerError);
                }
                sb.Append(")");
                throw new InvalidOperationException(sb.ToString(), ex);
            }
        }

        private string currentDisplayDocument;
        public void DisplayDocument(string url)
        {
            currentDisplayDocument = url;
            browser.Navigate(url);
        }
    }
}
