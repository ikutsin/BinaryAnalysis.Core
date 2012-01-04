using System;
using System.Collections.Generic;

namespace BinaryAnalysis.Scheduler.Task.Flow
{
    public delegate void NewScriptAssertionMessageHandler(ScriptAssertionMessage message);
    public partial class ScriptFlow
    {
        public event NewScriptAssertionMessageHandler OnNewMessage;

        private List<ScriptAssertionMessage> messages;
        public IList<ScriptAssertionMessage> Messages { get { return messages.AsReadOnly(); } }

        private ScheduleMessageState currentMessagesState = ScheduleMessageState.None;
        public ScheduleMessageState MessagesState { get { return currentMessagesState; } }


        public void AddMessage(Exception exception, ScheduleMessageState type = ScheduleMessageState.Error)
        {
            Dump(exception);
            AddMessage(exception.Message, type);
        }

        public void AddMessage(string message, ScheduleMessageState type = ScheduleMessageState.Info)
        {
            if(String.IsNullOrEmpty(message)) throw new InvalidOperationException("Message can not be empty");
            if (type > currentMessagesState) currentMessagesState = type;
            var assertionMessage = new ScriptAssertionMessage()
                                             {
                                                 Date = DateTime.Now,
                                                 Message = message,
                                                 Type = type
                                             };
            messages.Add(assertionMessage);
            if (OnNewMessage != null) OnNewMessage(assertionMessage);
        }
    }
}
