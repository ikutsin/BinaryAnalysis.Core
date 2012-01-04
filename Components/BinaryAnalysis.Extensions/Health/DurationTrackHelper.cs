using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Extensions.Health
{
    public class DurationTrackHelper
    {
        private readonly IHealthTrackingService _service;
        private Stopwatch stopwatch;

        public DurationTrackHelper(IHealthTrackingService service)
        {
            _service = service;
        }

        private string metric;
        private string name;
        public void Start(string metric, string name = null)
        {
            if (stopwatch != null) throw new Exception("Already started");
            this.metric = metric;
            this.name = name;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void TrackAndStop()
        {
            stopwatch.Stop();
            _service.Track(stopwatch.Elapsed, metric, name);
            stopwatch = null;
        }
    }
}
