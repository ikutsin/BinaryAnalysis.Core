using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Box.Transforations;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;

namespace BinaryAnalysis.Data.Box
{
    public interface INHibernateFileBackupBase : IBackupBase
    {
        string BackupFile { get; set; }
        BoxImporterStrategy ImportStrategy { get; set; }
    }

    public class NHibernateFileBackupBase<T, TE> : INHibernateFileBackupBase where T : EntityBoxMap
        where TE : Entity
    {
        private FileBoxTransformation<T> _fileTransform;
        public FileBoxTransformation<T> FileTransform
        {
            get { return _fileTransform; }
            private set { _fileTransform = value; }
        }

        private NHibernateBoxTransformation<T, TE> _dbTransform;
        public NHibernateBoxTransformation<T, TE> DbTransform
        {
            get { return _dbTransform; }
            private set { _dbTransform = value; }
        }


        public NHibernateFileBackupBase(
            FileBoxTransformation<T> fileTransform, 
            NHibernateBoxTransformation<T,TE> dbTransform)
        {
            _fileTransform = fileTransform;
            _dbTransform = dbTransform;

            GetEntitiesToBackup = new Func<IRepository<TE>, IList<TE>>(repo => repo.GetAll());
            ImportStrategy = BoxImporterStrategy.SkipExisting;
        }

        public Func<IRepository<TE>, IList<TE>> GetEntitiesToBackup { get; set; }

        public string BackupFile
        {
            get { return FileTransform.BackupFile; }
            set { FileTransform.BackupFile = value; }
        }

        public BoxImporterStrategy ImportStrategy
        {
            get { return DbTransform.ImportStrategy; }
            set { DbTransform.ImportStrategy = value; }
        }

        public void Backup()
        {
            _dbTransform.Entries = GetEntitiesToBackup(_dbTransform._repoFinder.CreateRepository<TE>());
            var box = _dbTransform.ToBox();
            _fileTransform.Transform(box);
        }

        public void Restore()
        {
            var box = _fileTransform.ToBox();
            _dbTransform.Transform(box);
        }
    }
}
