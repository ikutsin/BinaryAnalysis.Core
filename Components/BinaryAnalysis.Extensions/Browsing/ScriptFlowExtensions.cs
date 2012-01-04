using System;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Browsing.Windowless.Proxies;
using BinaryAnalysis.Scheduler.Task.Flow;

namespace BinaryAnalysis.Extensions.Browsing
{
    public static class ScriptFlowExtensions
    {
        public static FlowAssertion IsCached(this ScriptFlow flow, IBrowsingResponse resp)
        {
            return FlowAssertion.Create(flow, (resp is StateBrowsingResponse));
        }
        public static FlowAssertion IsError(this ScriptFlow flow, IBrowsingResponse resp)
        {
            return FlowAssertion.Create(flow, (resp is ErrorBrowsingResponse));
        }
        public static FlowAssertion IsStatusNotOK(this ScriptFlow flow, IBrowsingResponse response)
        {
            var message =
                String.Format("Response from '{0}' not ok ({1})", response.ResponseUrl, response.StatusCode);

            return flow.IsStatusOK(response).Otherwise().Warn(message);
        }
        public static FlowAssertion IsStatusOK(this ScriptFlow flow, IBrowsingResponse response)
        {
            return FlowAssertion
                .Create(flow, response.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
