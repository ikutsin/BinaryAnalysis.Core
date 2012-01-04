using System;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace BA.Examples.ServiceProcess.Services.Core
{
    public abstract class PublishService<T> where T : class
    {
        protected static void FireEvent(params object[] args)
        {
            StackFrame stackFrame = new StackFrame(1);
            string methodName = stackFrame.GetMethod().Name;

            PublishTransient(methodName, args);
        }
        static void PublishTransient(string methodName, params object[] args)
        {
            T[] subscribers =
               SubscriptionManager<T>.GetTransientList(methodName);
            Publish(subscribers, false, methodName, args);
        }

        static void Publish(T[] subscribers, bool closeSubscribers,
           string methodName, params object[] args)
        {
            WaitCallback fire = delegate(object subscriber)
            {
                Invoke(subscriber as T, methodName, args);
                if (closeSubscribers)
                {
                    using (subscriber as IDisposable)
                    { }
                }
            };
            Action<T> queueUp = delegate(T subscriber)
            {
                ThreadPool.QueueUserWorkItem(fire, subscriber);
            };
            Array.ForEach(subscribers, queueUp);
        }

        static void Invoke(T subscriber, string methodName, object[] args)
        {
            Type type = typeof(T);
            MethodInfo methodInfo = type.GetMethod(methodName);
            try { methodInfo.Invoke(subscriber, args); }
            catch { }
        }
    }
}
