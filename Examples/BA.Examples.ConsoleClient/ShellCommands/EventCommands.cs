using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using BA.Examples.ServiceProcess.Services;
using BA.Examples.ServiceProcess.Services.Core;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.ConsoleClient.ShellCommands
{
    public class EventEnabledClientInformation
    {
        public Type ClientType { get; set; }
        public Type HandlerType { get; set; }
        public ISubscriptionService ClientInstance { get; set; }
        public string[] Events { get; set; }
    }
    public class EventCommands : ShellCommandSet
    {
        public static Dictionary<string, EventEnabledClientInformation> eventEnabledClients
            = new Dictionary<string, EventEnabledClientInformation>
            {
                { "commons", new EventEnabledClientInformation
                    {
                        ClientType=typeof(ICommonsService),
                        HandlerType=typeof(CommonsServiceEventsDefaultHandler),
                        Events = new[] { "OnTick", "OnBroadcastText"}
                    } 
                }
            };

        [CommandDescription("Subscribe to service events")]
        public string[] Subscribe(string input)
        {
            if (input == null)
            {
                return new List<string> {"all"}.Concat(eventEnabledClients.Select(k => k.Key)).ToArray();
            }

            if (input == "all")
            {
                foreach (var clientInformation in eventEnabledClients)
                {
                    SubscribeService(clientInformation.Value);
                }
            }
            else if (eventEnabledClients.ContainsKey(input))
            {
                SubscribeService(eventEnabledClients[input]);
            }
            else
            {
                throw new Exception("Unknown service name");
            }
            return null;
        }
        [CommandDescription("Unsubscribe from to service events")]
        public string[] Unsubscribe(string input)
        {
            if (input == null)
            {
                return new List<string> { "all" }.Concat(eventEnabledClients.Select(k => k.Key)).ToArray();
            }

            if (input == "all")
            {
                foreach (var clientInformation in eventEnabledClients)
                {
                    UnsubscribeService(clientInformation.Value);
                }
            }
            else if (eventEnabledClients.ContainsKey(input))
            {
                UnsubscribeService(eventEnabledClients[input]);
            }
            else
            {
                throw new Exception("Unknown service name");
            }
            return null;
        }

        void SubscribeService(EventEnabledClientInformation service)
        {
            if (service.ClientInstance == null)
            {
                service.ClientInstance = (ISubscriptionService)Context.Resolve(service.ClientType);
                RegisterEventHandlers(service);
            }
            
            foreach (var evt in service.Events)
            {
                service.ClientInstance.Subscribe(evt);
            }
        }
        void UnsubscribeService(EventEnabledClientInformation service)
        {
            if (service.ClientInstance == null) return;
            foreach (var evt in service.Events)
            {
                service.ClientInstance.Unsubscribe(evt);
            }
        }

        private void RegisterEventHandlers(EventEnabledClientInformation service)
        {
            if (service.HandlerType == typeof(CommonsServiceEventsDefaultHandler))
            {
                var commonsHandler = Context.Resolve<CommonsServiceEventsDefaultHandler>();
                commonsHandler.Tick += (date) => Writer.WriteLine(date);
                commonsHandler.BroadcastText += (txt) => Writer.WriteLine(txt);
            }
        }
    }
}
