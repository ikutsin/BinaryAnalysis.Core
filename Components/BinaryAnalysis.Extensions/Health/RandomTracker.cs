using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Extensions.Health
{
    public class RandomTracker : AbstractFrequencyTrackHelper
    {
        Random random = new Random();
        public RandomTracker(IHealthTrackingService service) : base(service)
        {
        }

        protected override void ElapsedInternal()
        {
            Track(random.Next());
        }
    }
}
