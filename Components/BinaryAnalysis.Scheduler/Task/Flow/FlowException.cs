using System;

namespace BinaryAnalysis.Scheduler.Task.Flow
{
    public enum FlowExceptionType
    {
        Fail, Normal
    }
    [Serializable]
    public class FlowException : Exception
    {
        public FlowExceptionType ExceptionType { get; set; }
        public FlowException(string message, FlowExceptionType ExceptionType = FlowExceptionType.Fail) : base(message)
        {
            this.ExceptionType = ExceptionType;
        }
        public FlowException(string message, Exception innerException) : base(message, innerException) { }
    }
}
