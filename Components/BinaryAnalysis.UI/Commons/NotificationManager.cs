using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BinaryAnalysis.UI.Controls.TestNotifyWindow;

namespace BinaryAnalysis.UI.Commons
{
    public class NotificationManager : IDisposable
    {
        private NotifyIcon notifyicon;
        private ContextMenu menu;

        public void ShowMessage(string message, Action callback = null, string title = null)
        {
            var c = SynchronizationContext.Current;
            var tid = Thread.CurrentThread.ManagedThreadId;
            NotifyWindow notify = new NotifyWindow(message,title);
            if (callback != null)
            {
                notify.TextClicked += (_, __) => callback.Invoke();
            }
            //notify.WaitTime = 6000;
            notify.Notify();
        }

        public void InitializeTray()
        {
            menu = new ContextMenu();
            menu.MenuItems.Add(new MenuItem("Show", (s, e) => { notifyicon.Icon = SystemIcons.Application; }));
            menu.MenuItems.Add(new MenuItem("Exit", (s, e) => { notifyicon.Icon = SystemIcons.Warning; }));
            menu.MenuItems.Add(new MenuItem("Show", (s, e) => { notifyicon.Icon = SystemIcons.Question; }));

            notifyicon = new NotifyIcon
                             {
                                 Text = "Right click for context menu", 
                                 Visible = true, 
                                 ContextMenu = menu
                             };
            notifyicon.Icon = SystemIcons.WinLogo;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.notifyicon.Dispose();
        }

        #endregion
    }
}
