using System;
using Project.Networking;

namespace FireWindServer
{
    public class FormMessageStore
    {
        public int clientID;
        public bool isError;
        public Message m;
        public SynchMain.MessagePriority priority;
        public string text;

        public FormMessageStore(int clientID, String text, Message m, SynchMain.MessagePriority priority, bool isError)
        {
            this.clientID = clientID;
            this.text = text;
            this.m = m;
            this.priority = priority;
            this.isError = isError;
        }
    }
}