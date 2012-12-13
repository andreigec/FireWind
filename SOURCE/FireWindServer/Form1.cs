using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ANDREICSLIB;
using FireWind;
using Microsoft.Xna.Framework;
using Project;
using Project.Model;
using Project.Networking;
using Project.View.Client.ClientScreens;
using Message = Project.Networking.Message;
using Color = System.Drawing.Color;

namespace FireWindServer
{
    public partial class Form1 : Form
    {
        public static ServerWindow mainserver;

        #region licensing

        private const string AppTitle = "FireWind Dedicated Server";
        private const double AppVersion = 1.1;
        private const String HelpString = "";

        private const String RepoName = "FireWind";
        private const String UpdatePath = "https://github.com/EvilSeven/" + RepoName + "/zipball/master";
        private const String VersionPath = "https://raw.github.com/EvilSeven/" + RepoName + "/master/INFO/version.txt";

        private const String ChangelogPath =
            "https://raw.github.com/EvilSeven/" + RepoName + "/master/INFO/changelog.txt";

        private readonly String OtherText =
            @"©" + DateTime.Now.Year +
            @" Andrei Gec (http://www.andreigec.net)

Licensed under GNU LGPL (http://www.gnu.org/)

Zip Assets © SharpZipLib (http://www.sharpdevelop.net/OpenSource/SharpZipLib/)
";

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void stopAndExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //updateServerButtonText();
            //CheckForIllegalCrossThreadCalls = false;
            //init constants for game

            loadXML.initXML(new Game1(false));

            //turn on server
            mainserver = new ServerWindow(this);
            //TEMP
            mainserver.StartServer();
            mainserver.StartGame(new SectorConfigColosseum(SectorConfig.Size.medium));
            tabControl1.SelectedIndex = 0;

            var sd = new Licensing.SolutionDetails(HelpString, AppTitle, AppVersion, OtherText, VersionPath, UpdatePath,
                                                   ChangelogPath);
            Licensing.CreateLicense(this, sd, menuStrip1);

            //add log event
#if DEBUG
            Manager.AddLogEventDelegate(AddLog);
#endif

        }

        public static void AddLog(int clientID, String text, Message m, SynchMain.MessagePriority priority, bool isError)
        {
            AddLog(mainserver.consolelog, clientID, text, m, priority, isError);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mainserver != null)
                mainserver.StopServer();
        }

        public static void AddLog(ListView lv, int clientID, String text, Message m, SynchMain.MessagePriority priority, bool isError)
        {
            ConsoleCL.console_LogAdd(clientID,text,m,priority,isError);

            var fms = new FormMessageStore(clientID, text, m, priority, isError);
            lv.Invoke(addItemToLV, new object[] { lv, fms });
        }

        public static Tuple<Color, Color> getColour(SynchMain.MessagePriority priority, bool isError)
        {
            Color fore;
            Color back;
            switch (priority)
            {
                case SynchMain.MessagePriority.Information:
                    if (isError)
                    {
                        fore = Color.IndianRed;
                        back = Color.White;
                    }
                    else
                    {
                        fore = Color.Black;
                        back = Color.White;
                    }
                    break;

                case SynchMain.MessagePriority.Low:
                    if (isError)
                    {
                        fore = Color.DarkRed;
                        back = Color.White;
                    }
                    else
                    {
                        fore = Color.DarkBlue;
                        back = Color.White;
                    }
                    break;

                case SynchMain.MessagePriority.High:
                    if (isError)
                    {
                        fore = Color.Red;
                        back = Color.White;
                    }
                    else
                    {
                        fore = Color.Blue;
                        back = Color.White;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("priority");
            }

            return new Tuple<Color, Color>(fore, back);
        }

        public delegate void addItemToLVDel(ListView lv, FormMessageStore m2);
        public static addItemToLVDel addItemToLV = addItemToLVF;
        private static void addItemToLVF(ListView lv, FormMessageStore m2)
        {
            //remove if too many messages
            while (lv.Items.Count > GameWindow.maxCount)
            {
                lv.Items.Remove(lv.Items[0]);
            }

            var alias = mainserver.GetAlias(m2);
            String time = DateTime.Now.ToLongTimeString();

            String id = "";
            if (m2.m != null && m2.m.ID != -1)
                id = m2.m.ID.ToString();

            String resp = "";
            if (m2.m != null && m2.m.ResponseID != -1)
                resp = m2.m.ResponseID.ToString();

            String mt = "";
            String mp = "";
            if (m2.m != null)
            {
                mt = m2.m.getMessageTypeString();
                mp = m2.m.getMessageParamString();
            }

            var lvi = new ListViewItem(time) { Tag = m2 };
            lvi.SubItems.Add(alias);
            lvi.SubItems.Add(id);
            lvi.SubItems.Add(resp);
            lvi.SubItems.Add(m2.text);
            lvi.SubItems.Add(mt);
            lvi.SubItems.Add(mp);

            var t = getColour(m2.priority, m2.isError);
            lvi.ForeColor = t.Item1;
            lvi.BackColor = t.Item2;

            lv.Items.Add(lvi); // runs on UI thread
            lvi.EnsureVisible();
        }
    }
}
