using System;
using System.Collections.Generic;
using System.IO;
using BinaryAnalysis.Box.Presentations;
using log4net;

namespace BinaryAnalysis.Box.Transforations
{
    public class FileBoxTransformation<T> : IBaseBoxTransformation<T>
    {
        private string path;
        protected readonly ILog log;

        public FileBoxTransformation(ILog log)
        {
            this.log = log;
            Presentation = new XmlBoxPresentation<T>();
            BackupFile = "Restore" + Path.DirectorySeparatorChar + (typeof(T).Name)+".xml";
        }

        public IBoxPresentation<T> Presentation { get; set; }
        public string BackupFile
        {
            get { return path; }
            set
            {
                if (!String.IsNullOrEmpty(Path.GetDirectoryName(path))
                    &&Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                path = Path.GetFullPath(value);
            }
        }

        #region Implementation of IBaseBoxTransformation<T>

        public IBox<T> ToBox()
        {
            if (String.IsNullOrWhiteSpace(BackupFile)) throw new InvalidOperationException("BackupFile is not set");
            return Presentation.FromFile(BackupFile);
        }

        public void Transform(IBox<T> box)
        {
            if (String.IsNullOrWhiteSpace(BackupFile)) throw new InvalidOperationException("BackupFile is not set");
            Presentation.ToFile(box, BackupFile);
        }

        #endregion
    }
}
