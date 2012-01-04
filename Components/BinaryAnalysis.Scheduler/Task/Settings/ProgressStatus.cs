using System;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Scheduler.Task.Settings
{
    public delegate void OnProgressUpdateHandler(ProgressStatus progress);
    [DataContract, Serializable]
    public class ProgressStatus
    {
        public const int PROGRESS_MAX = 100;

        string message;
        [DataMember]
        public string Message
        {
            get { return message; }
            protected set
            {
                message = value;
            }
        }

        int completeness;
        [DataMember]
        public int Completeness
        {
            get
            {
                return completeness;
            }
            protected set
            {
                if (value < 0 || value > PROGRESS_MAX) throw new Exception("Invalud completeness value");
                completeness = value;
            }
        }

        public event OnProgressUpdateHandler Update;

        public ProgressStatus() { }
        public ProgressStatus(string message = null, int completeness = 0)
        {
            this.Message = message;
            this.Completeness = completeness;
        }

        public virtual void OnUpdate(ProgressStatus _this) 
        {
            if (Update != null) Update(this);
        }

        public void AddProgress(string message = null, int delta = 0)
        {
            SetProgress(message, Math.Min(PROGRESS_MAX, Completeness + delta));
        }
        public void SetProgress(string message = null, int completeness = 0) 
        {
            Message = message;
            if (completeness > 0) Completeness = completeness;
            OnUpdate(this);
        }
        public void SetProgressRemaining(string message = null, int remaining = 0)
        {
            SetProgress(message, PROGRESS_MAX - remaining);
        }
    }
}
