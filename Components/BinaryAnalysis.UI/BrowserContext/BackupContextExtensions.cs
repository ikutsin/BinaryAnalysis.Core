using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BinaryAnalysis.Data.Box;

namespace BinaryAnalysis.UI.BrowserContext
{
    [ComVisible(true)]
    public class  BackupContextExtensions : IBrowserContextExtension
    {
        private readonly BackupsResolver _backupsResolver;

        public BackupContextExtensions(BackupsResolver backupsResolver)
        {
            _backupsResolver = backupsResolver;
        }

        public bool hasBackupFor(string typename)
        {
            var bType = Type.GetType(typename);
            return _backupsResolver.GetFileBackupInstanceForBox(bType) != null;
        }
        public bool backupDefault(string typename)
        {
            var bType = Type.GetType(typename);
            var backup = _backupsResolver.GetFileBackupInstanceForBox(bType);
            if (bType == null || backup == null) return false;
            backup.BackupFile = @"Backup\"+bType.Name + ".xml";
            backup.Backup();
            return true;
        }
    }
}
