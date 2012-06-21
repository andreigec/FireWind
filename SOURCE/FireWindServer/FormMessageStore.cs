using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ANDREICSLIB;
using Project;
using Project.Model;
using Project.Networking;
using Message = Project.Networking.Message;

namespace FireWindServer
{
    public class FormMessageStore
    {
        public int clientID;
        public string text;
        public Message m;
        public SynchMain.MessagePriority priority;
        public bool isError;

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
