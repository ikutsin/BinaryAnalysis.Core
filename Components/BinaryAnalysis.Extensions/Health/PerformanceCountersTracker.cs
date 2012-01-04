using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Extensions.Health
{
    public class PerformanceCountersTracker : AbstractFrequencyTrackHelper
    {
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public PerformanceCountersTracker(IHealthTrackingService service) : base(service)
        {
        }

        protected override void StartInternal()
        {
            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        #region Overrides of AbstractFrequencyTrackHelper

        protected override void ElapsedInternal()
        {
            _service.Track((decimal)cpuCounter.NextValue(), metric, "CPU");
            _service.Track((decimal)ramCounter.NextValue(), metric, "RAM");
            _service.Track((decimal)GC.GetTotalMemory(true), metric, "MEM");
        }


        #endregion
    }
}
