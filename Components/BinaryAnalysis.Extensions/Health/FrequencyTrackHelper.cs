using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace BinaryAnalysis.Extensions.Health
{
    public class FrequencyTrackHelper : AbstractFrequencyTrackHelper
    {
        private long counter;

        public FrequencyTrackHelper(IHealthTrackingService service)
            : base(service)
        {
        }

        public void Notify(long size =1)
        {
            lock (locker)
            {
                counter += size;
            }
        }
        protected override void ElapsedInternal()
        {
            Track(counter);
            counter = 0;
        }

        protected override void StartInternal()
        {
            counter = 0;
        }

        protected override void StopInternal()
        {
            counter = 0;
        }
    }
}
