using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;

namespace BA.Examples.ServiceProcess.Services.Core
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/cc163537.aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public abstract class SubscriptionManager<T> // : ISubscriptionService
        where T : class
    {
        static Dictionary<string, List<T>> m_TransientStore;

        static SubscriptionManager()
        {
            m_TransientStore = new Dictionary<string, List<T>>();
            string[] methods = GetOperations();
            Array.ForEach(methods, delegate(string methodName)
            {
                m_TransientStore.Add(methodName, new List<T>());
            });
        }

        static string[] GetOperations()
        {
            MethodInfo[] methods = typeof(T).GetMethods(
               BindingFlags.Public | BindingFlags.FlattenHierarchy |
               BindingFlags.Instance);
            List<string> operations = new List<string>(methods.Length);

            Array.ForEach(methods, delegate(MethodInfo method)
            {
                operations.Add(method.Name);
            });
            return operations.ToArray();
        }

        static void AddTransient(T subscriber, string eventOperation)
        {
            List<T> list = m_TransientStore[eventOperation];
            if (list.Contains(subscriber)) return;
            list.Add(subscriber);
        }

        static void RemoveTransient(T subscriber, string eventOperation)
        {
            List<T> list = m_TransientStore[eventOperation];
            list.Remove(subscriber);
        }

        internal static T[] GetTransientList(string eventOperation)
        {
            lock (typeof(SubscriptionManager<T>))
            {
                List<T> list = m_TransientStore[eventOperation];
                return list.ToArray();
            }
        }

        public void Subscribe(string eventOperation)
        {
            lock (typeof(SubscriptionManager<T>))
            {
                T subscriber = OperationContext.Current.GetCallbackChannel<T>();
                if (!String.IsNullOrEmpty(eventOperation))
                {
                    AddTransient(subscriber, eventOperation);
                }
                else
                {
                    string[] methods = GetOperations();
                    Array.ForEach(methods, delegate(string methodName)
                    {
                        AddTransient(subscriber, methodName);
                    });
                }
            }
        }

        public void Unsubscribe(string eventOperation)
        {
            lock (typeof(SubscriptionManager<T>))
            {
                T subscriber = OperationContext.Current.GetCallbackChannel<T>();
                if (!String.IsNullOrEmpty(eventOperation))
                {
                    RemoveTransient(subscriber, eventOperation);
                }
                else
                {
                    string[] methods = GetOperations();
                    Array.ForEach(methods, delegate(string methodName)
                    {
                        RemoveTransient(subscriber, methodName);
                    });
                }
            }
        }
    }
}
