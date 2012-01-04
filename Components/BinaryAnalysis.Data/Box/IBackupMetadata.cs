using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BinaryAnalysis.Data.Box
{
    public class BackupsResolver
    {
        IEnumerable<Lazy<IBackupBase, IBackupMetadata>> _backups { get; set; }
        public BackupsResolver(IEnumerable<Lazy<IBackupBase, IBackupMetadata>> backups)
        {
            _backups = backups;
        }

        public INHibernateFileBackupBase GetFileBackupInstanceForEntity<TE>()
        {
            return GetFileBackupInstanceForEntity(typeof (TE));
        }
        public INHibernateFileBackupBase GetFileBackupInstanceForEntity(Type type)
        {
            var result = _backups.FirstOrDefault(
                b => b.Metadata.BackupType == BackupType.File && b.Metadata.EntityType == type);
            if (result == null) return null;
            return (INHibernateFileBackupBase) result.Value;
        }
        public INHibernateFileBackupBase GetFileBackupInstanceForBox<TM>()
        {
            return GetFileBackupInstanceForBox(typeof(TM));
        }
        public INHibernateFileBackupBase GetFileBackupInstanceForBox(Type type)
        {
            var result = _backups.FirstOrDefault(
                       b => b.Metadata.BackupType == BackupType.File && b.Metadata.MapType == type);
            if (result == null) return null;
            return (INHibernateFileBackupBase) result.Value;
        }
    }


    public interface IBackupBase
    {
        void Backup();
        void Restore();
    }

    public enum BackupType
    {
        File
    }
    public interface IBackupMetadata
    {
        BackupType BackupType { get; }
        Type EntityType { get; }
        Type MapType { get; }
    }
}
