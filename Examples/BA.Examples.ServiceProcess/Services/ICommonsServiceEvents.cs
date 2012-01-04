using System;
using System.ServiceModel;

namespace BA.Examples.ServiceProcess.Services
{
    public interface ICommonsServiceEvents
    {
        [OperationContract(IsOneWay = true)]
        void OnTick(DateTime serverTime);

        [OperationContract(IsOneWay = true)]
        void OnBroadcastText(string text);
    }
    public class CommonsServiceEventsDefaultHandler : ICommonsServiceEvents
    {
        public delegate void TickHandler(DateTime serverTime);
        public event TickHandler Tick;
        public void OnTick(DateTime serverTime)
        {
            if (Tick != null) Tick(serverTime);
        }

        public delegate void BroadcastTextHandler(String text);
        public event BroadcastTextHandler BroadcastText;
        public void OnBroadcastText(string text)
        {
            if (BroadcastText != null) BroadcastText(text);
        }
    }
}
