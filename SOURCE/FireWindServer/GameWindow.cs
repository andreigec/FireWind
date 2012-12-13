using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ANDREICSLIB;
using FireWind;
using Project;
using Project.Model;
using Project.Networking;
using Project.View.Client.ClientScreens;
using Message = Project.Networking.Message;
using Messages = Project.Networking.Message.Messages;

namespace FireWindServer
{
    public partial class GameWindow : UserControl
    {
        public ServerWindow parentServer;
        public static int maxCount = 100;
        public sector sec;

        public GameWindow(sector s,ServerWindow parent)
        {
            InitializeComponent();
            sec = s;
            parentServer = parent;
        }
    }
}
