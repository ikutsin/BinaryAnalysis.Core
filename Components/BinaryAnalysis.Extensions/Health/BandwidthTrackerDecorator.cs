using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BinaryAnalysis.Browsing.Windowless.Decorators;

namespace BinaryAnalysis.Extensions.Health
{
    //http://www.codeproject.com/KB/IP/Bandwidth_throttling.aspx
    //http://stackoverflow.com/questions/442409/c-bandwidth
    //http://www.m0interactive.com/archives/2008/02/06/how_to_calculate_network_bandwidth_speed_in_c_/
    public class BandwidthTrackerDecorator : EmptyDecorator
    {
        private readonly FrequencyTrackHelper _freq;
        private readonly FrequencyTrackHelper calls;

        public BandwidthTrackerDecorator(FrequencyTrackHelper freq, FrequencyTrackHelper calls)
        {
            _freq = freq;
            this.calls = calls;
            freq.Start(TimeSpan.FromSeconds(60), "Bandwidth");
            calls.Start(TimeSpan.FromSeconds(60), "Calls");
        }

        public override bool OnAfterRequestRerun(BinaryAnalysis.Browsing.Windowless.IBrowsingSession session, Uri uri, BinaryAnalysis.Browsing.Windowless.IBrowsingResponse response)
        {
            calls.Notify();
            if(response.ResponseStream!=null)
            {
                _freq.Notify(response.ResponseStream.Length);
            }
            return base.OnAfterRequestRerun(session, uri, response);
        }
    }
}
