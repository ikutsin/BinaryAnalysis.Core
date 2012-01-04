using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Index;
using BinaryAnalysis.Data.Log;
using log4net;

namespace BinaryAnalysis.Data
{
    public delegate void LogHandler(ActivityLogEntity log);

    public class ActivityLogService
    {
        private readonly ActivityLogRepository logRepo;
        private readonly ILog log;

        public ActivityLogService(ActivityLogRepository logRepo, ILog log)
        {
            this.logRepo = logRepo;
            this.log = log;
        }
        public ActivityLogEntity AddLog(string descriminator, 
            ActivityLogLevel level = ActivityLogLevel.Info, 
            string message = null,
            IClassifiable classifiable = null,
            object contract = null)
        {
            var entry = new ActivityLogEntity();
            entry.Descriminator = descriminator;
            entry.Message = message;
            entry.Level = level;
            if(contract!=null)entry.SetValue(contract);
            if (classifiable != null)
            {
                entry.ClassifiableId = classifiable.Id;
                entry.ClassifiableName = classifiable.ObjectName;
            }
            entry = logRepo.Save(entry);
            InvokeOnNewLog(entry);
            return entry;
        }

        public event LogHandler OnNewLog;

        public void InvokeOnNewLog(ActivityLogEntity log)
        {
            LogHandler handler = OnNewLog;
            if (handler != null) handler(log);
        }
    }
}
