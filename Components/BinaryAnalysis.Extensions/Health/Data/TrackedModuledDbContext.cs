using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Extensions.Health.Data
{
    public class TrackedModuledDbContext : ModuledDbContext
    {
        private readonly FrequencyTrackHelper _freqRead;
        private readonly FrequencyTrackHelper _freqWrite;

        public TrackedModuledDbContext(FrequencyTrackHelper freqRead, FrequencyTrackHelper freqWrite)
        {
            _freqRead = freqRead;
            _freqWrite = freqWrite;
        }

        public override BinaryAnalysis.Data.Core.SessionManagement.ISessionManager SessionManager
        {
            get
            {
                var sm = base.SessionManager;
                return new WrappedSessionManagerTracker(this, sm);
            }
        }


        public FrequencyTrackHelper FreqWrite
        {
            get { return _freqWrite; }
        }

        public FrequencyTrackHelper FreqRead
        {
            get { return _freqRead; }
        }
    }
}
