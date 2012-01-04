using System;

namespace BinaryAnalysis.Scheduler.Task.Flow
{
    public class FlowAssertion
    {
        public bool IsPositive { get; protected set; }
        public bool IsNegative { get { return !IsPositive; } }
        ScriptFlow root;

        public static FlowAssertion Create(ScriptFlow root, bool assert)
        {
            return new FlowAssertion(root, assert);
        }
        protected FlowAssertion(ScriptFlow root, bool assert)
        {
            IsPositive = assert;
            this.root = root;
        }

        public FlowAssertion And(bool assert)
        {
            IsPositive &= assert;
            return this;
        }
        public FlowAssertion Otherwise()
        {
            IsPositive = !IsPositive;
            return this;
        }
        public FlowAssertion Or(bool assert)
        {
            IsPositive |= assert;
            return this;
        }

        //----------------------messages---------------//
        public FlowAssertion Warn(string message)
        {
            if (IsPositive)
            {
                root.AddMessage(message, ScheduleMessageState.Warn);
            }
            return this;
        }
        public FlowAssertion Info(string message)
        {
            if (IsPositive)
            {
                root.AddMessage(message, ScheduleMessageState.Info);
            }
            return this;
        }
        /*
        public void Error(string message)
        {
            if (IsPositove)
            {
                root.AddMessage(message, MessageType.Error);
            }
        }
        */
        //-------------------- Actions ----------------//
        public FlowAssertion Action(Action a)
        {
            if (IsPositive)
            {
                a();
            }
            return this;
        }
        public void Rerun(string message = null)
        {
            Rerun(TimeSpan.Zero, message);
        }
        public void Rerun(TimeSpan dueIn, string message = null)
        {
            if (IsPositive)
            {
                root.AddMessage(message, ScheduleMessageState.Info);
                root.Rerun(dueIn);
            }
        }
        public void Fail(string message = "AssertionFailed")
        {
            if (IsPositive)
            {
                root.Fail(message);
            }
        }
    }
}
