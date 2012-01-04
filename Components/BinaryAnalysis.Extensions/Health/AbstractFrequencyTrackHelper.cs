using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace BinaryAnalysis.Extensions.Health
{
    public abstract class AbstractFrequencyTrackHelper
    {
        protected readonly IHealthTrackingService _service;
        protected Timer timer;
        protected object locker = new object();

        #region Timerlogic
        public AbstractFrequencyTrackHelper(IHealthTrackingService service)
        {
            _service = service;
        }

        protected string metric;
        protected string name;
        public void Start(TimeSpan frequency, string metric, string name = null)
        {
            if (timer != null) throw new Exception("Already started");
            this.metric = metric;
            this.name = name;
            lock (locker)
            {
                StartInternal();
                timer = new Timer(frequency.TotalMilliseconds);
                timer.Elapsed += Elapsed;
                timer.Start();
            }
        }

        public void Stop()
        {
            if (timer == null) return;
            timer.Stop();
            timer.Dispose();
            timer = null;
            StopInternal();
        }

        protected void Elapsed(object sender, ElapsedEventArgs e)
        {
            {
                lock (locker)
                {
                    ElapsedInternal();
                }
            }
        } 
        #endregion

        protected void Track(TimeSpan counter)
        {
            _service.Track(counter, metric, name);
        }
        protected void Track(decimal counter)
        {
            _service.Track(counter, metric, name);
        }
        protected void Track(int counter)
        {
            _service.Track(counter, metric, name);
        }

        protected abstract void ElapsedInternal();
        protected virtual void StartInternal()
        {
        }

        protected virtual void StopInternal()
        {
        }
    }
}
