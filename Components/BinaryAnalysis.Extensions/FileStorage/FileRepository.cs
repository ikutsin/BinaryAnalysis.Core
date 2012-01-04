using System;
using System.IO;
using log4net;

namespace BinaryAnalysis.Extensions.FileStorage
{
    public class FileRepository
    {
        private string folderName;
        private readonly ILog log;

        public string WorkingFolder
        {
            get
            {
                return (Path.Combine(Environment.CurrentDirectory, folderName));
            }
        }

        public FileRepository(string folderName, ILog log)
        {
            this.folderName = folderName;
            this.log = log;
            if (!Directory.Exists(WorkingFolder))
            {
                Directory.CreateDirectory(WorkingFolder);
            }
        }
        private static object fsLocker = new object();
        protected string Store(Stream stream, string fullPath)
        {
            if (!fullPath.StartsWith(WorkingFolder)) throw new InvalidOperationException("Attempt to store the file outside the working directory");

            lock (fsLocker)
            {
                try
                {
                    stream.Position = 0;
                    if (File.Exists(fullPath)) File.Delete(fullPath);
                    var dir = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    var file = File.Create(fullPath);
                    stream.CopyTo(file);
                    file.Close();
                }
                catch (Exception ex)
                {
                    fullPath = null;
                    log.Warn(ex);
                }

            }
            return fullPath;
        }
        public string StoreByDate(Stream stream, string extension, string tag = null)
        {
            var currentDir = WorkingFolder;
            var filename = String.Format("{0}.{1}",DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"),extension);
            if(tag!=null)
            {
                currentDir = Path.Combine(currentDir, tag);
                if (!Directory.Exists(currentDir))
                {
                    Directory.CreateDirectory(currentDir);
                }
            }
            return Store(stream, Path.Combine(currentDir, filename));
        }
    }
}
