using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace BinaryAnalysis.Terminal
{
    //TODO: http://stackoverflow.com/questions/6546509/detect-when-console-application-is-closing-killed
    public static class ProcessLock
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessLock));

        #region ConsoleLock
        public static void LockByLine()
        {
            log.Warn("to unlock: Press Enter");
            Console.ReadLine();
        }
        public static void LockByKey()
        {
            log.Warn("to unlock: Press any key");
            Console.ReadKey();
        }
        #endregion

        #region FileLock
        private static FileSystemWatcher watcher;
        private static string watchFilename;
        public static void LockByFile(string filename = "lock")
        {
            if (File.Exists(filename)) File.Delete(filename);
            File.WriteAllText(filename, "");

            watchFilename = filename;
            watcher = new FileSystemWatcher(Environment.CurrentDirectory);
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
            watcher.EnableRaisingEvents = true;


            log.Warn("to unlock: Delete file - " + filename);

            while (!String.IsNullOrEmpty(watchFilename)) Thread.Sleep(800);
        }

        private static void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!File.Exists(watchFilename)) watchFilename = null;
        }

        static void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (e.Name == watchFilename) watchFilename = null;
        } 
        #endregion
    }
}
