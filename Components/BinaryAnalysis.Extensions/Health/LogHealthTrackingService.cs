using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data;
using log4net;

namespace BinaryAnalysis.Extensions.Health
{
    public class LogHealthTrackingService : IHealthTrackingService
    {
        private readonly ILog _log;

        public LogHealthTrackingService(ILog log)
        {
            _log = log;
        }

        public void Track(TimeSpan value, string metrixName, string name)
        {
            _log.Info(String.Format("TrackTime: {0}|{1}: {2}", metrixName, name, value));
        }

        public void Track(int value, string metrixName, string name)
        {
            _log.Info(String.Format("TrackTime: {0}|{1}: {2}", metrixName, name, value));
        }

        public void Track(decimal value, string metrixName, string name)
        {
            _log.Info(String.Format("TrackTime: {0}|{1}: {2}", metrixName, name, value));
        }
    }
}
