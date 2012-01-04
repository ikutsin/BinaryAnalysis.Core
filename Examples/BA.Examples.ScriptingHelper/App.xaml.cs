using System;
using System.Windows;
using System.Windows.Threading;
using Autofac;

namespace BA.Examples.ScriptingHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CurrentApp = this;
        }

        public static App CurrentApp { get; private set; }
        private Bootstrap bootstrap;

        public IContainer Container
        {
            get { return bootstrap.Container; }
        }

        public void InRenderAction(Action<object> action, object obj, bool wait = false)
        {
            var handle = Dispatcher.BeginInvoke(action, DispatcherPriority.Render, obj);
            if(wait) handle.Wait();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            bootstrap = new Bootstrap();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            bootstrap.Dispose();
        }

        #region Fiddler and log
        
        //public void Log(object obj)
        //{
        //    obj.DispatchInUI(o =>
        //    {
        //        var mw = FiddlerWindow.Instance;
        //        mw.AddLogEntry(o);
        //    });
        //}

        #endregion

    }
}
