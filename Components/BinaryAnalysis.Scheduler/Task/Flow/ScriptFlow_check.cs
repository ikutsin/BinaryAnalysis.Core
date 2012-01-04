using System;

namespace BinaryAnalysis.Scheduler.Task.Flow
{
    public partial class ScriptFlow
    {
        #region Checking
        public FlowAssertion IsTrue(bool assert)
        {
            return FlowAssertion.Create(this, assert);
        }
        public FlowAssertion IsFalse(bool assert)
        {
            return FlowAssertion.Create(this, !assert);
        }
        public FlowAssertion IsNull(object obj)
        {
            return FlowAssertion.Create(this, obj == null);
        }
        public FlowAssertion IsNotNull(object obj)
        {
            return FlowAssertion.Create(this, obj != null);
        }
        #endregion
    }
}
