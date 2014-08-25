using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.Networking;
using Project.Model.Networking.Server.GameSynch;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using Project.View.Client.DrawableScreens.Pop_Up_Screens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class ConnectWF : Form, IScreenControls
    {
        public const string serversfile = "ServersFile.txt";
        public const int HeartbeatTimeoutMS = 5000;
        private const int SMWaitMS = 100;
        public static XNA_WF_Wrapper XnaWfWrapperInstance = null;
        public static Wrapper<object> IPAdd;

        public static List<ConnectDetails> MasterServers = new List<ConnectDetails>();
        private static bool masterservdirty = true;

        private static readonly Dictionary<ConnectDetails, Heartbeat> AvailableSectors =
            new Dictionary<ConnectDetails, Heartbeat>();

        /// <summary>
        /// list of ipaddresses we have requested game sectors from, and are waiting for a response from
        /// </summary>
        public static Dictionary<ConnectDetails, WaitingThread> Waiting =
            new Dictionary<ConnectDetails, WaitingThread>();

        private static bool availsecdirty = true;
        private static Thread updateThread;

        public ConnectWF()
        {
            InitializeComponent();

            //set all existing heartbeat servers to non active
            foreach (var h in AvailableSectors)
            {
                h.Value.active = false;
            }

            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            masterservdirty = true;
            availsecdirty = true;
            MasterServers = GetFileServers();
        }

        #region IScreenControls Members

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            Control c = XnaWfWrapperInstance.MouseUpdate(gt);
            if (c == null)
                return;

            if (mc.ButtonsPressed() == false)
                return;

            if (c == gobacktobase)
            {
                CloseConnections();
                GameControlClient.ResetToBaseScreen(false);
            }
            else if (c == creategamebutton)
            {
                GameControlClient.ShowChooseGameModeScreen();
            }
            else if (c == connectbutton)
            {
                var ri = new List<RequiredItem>();
                IPAdd = new Wrapper<object>("");
                ri.Add(RequiredItem.GetRIString(IPAdd, "IP Address"));
                GameControlClient.ShowGetInfoPopup(ri, AcceptIP);
            }

            else if (c.Tag != null && c.Tag is Heartbeat)
            {
                var h = c.Tag as Heartbeat;
                var gic = new GameInitConfig(h.CD.ip, false, h.CD.UDPport, h.CD.TCPport);

                GameControlClient.ConnectAndJoinGameSector(gic, h.id);
            }
        }

        public void AcceptIP(bool OKPressed)
        {
            var v = IPAdd.Value.ToString();
            if (OKPressed == false||IPAdd==null||string.IsNullOrEmpty(v))
                return;

            if (MasterServers.Any(s=>s.ip.Equals(v))==false)
            {
                MasterServers.Add(new ConnectDetails(v));
            }

            SaveFileServers(MasterServers);
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
        }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            if (Waiting.Count > 0)
            {
                EndFinishedRequests();
            }

            if (masterservdirty)
            {
                StandaloneRequestJoinableSectors();
                masterservdirty = false;
            }

            if (availsecdirty)
            {
                UpdateIPPanel();
                availsecdirty = false;
            }
            XnaWfWrapperInstance.Draw();
        }

        #endregion

        public void CloseConnections()
        {
            if (updateThread != null)
                updateThread.Abort();

            updateThread = null;
        }

        public static void CancelConnectWait(SynchMain sm, String ip)
        {
            //close waiting screen
            GameControlClient.ExitPopupScreen<ConnectWaitWF>();

            if (sm != null)
                //kill synch main
                sm.StopServer();
            else
            {
                //debug msg
                Manager.FireLogEvent("cancel connect wait not sent synch main", SynchMain.MessagePriority.Low, true);
            }

            return;
        }

        public void UpdateIPPanel()
        {
            foreach (var h in AvailableSectors)
            {
                bool existing = false;
                foreach (Control c in ippanel.Controls)
                {
                    if (c.Tag != null && c.Tag is Heartbeat)
                    {
                        existing = true;
                        break;
                    }
                }

                if (existing == false)
                {
                    var b = new Button();
                    b.Name = h.Value.id.ToString();
                    b.Tag = h.Value;
                    b.Size = new Size(100, 50);
                    b.Text = h.Value.Config + ":" + h.Value.CD.ip;
                    ippanel.addControl(b, true);
                }
                //get control to set backcolour etc
                List<Control> conlist = (from c in ippanel.Controls.Cast<Control>()
                                         where c.Tag != null && c.Tag is Heartbeat
                                         let hb = c.Tag as Heartbeat
                                         where hb.CD.Equals(h.Value.CD)
                                         select c
                                        ).ToList();

                if (conlist.Count != 1)
                {
                    Manager.FireLogEvent("error updating ippanel", SynchMain.MessagePriority.High, true);
                    return;
                }
                conlist.First().Enabled = h.Value.active;
            }
        }

        /// <summary>
        /// get the master servers from the file
        /// </summary>
        /// <returns></returns>
        public static List<ConnectDetails> GetFileServers()
        {
            FileStream fs = null;
            StreamReader sr = null;
            var servers = new List<ConnectDetails>();

            try
            {
                fs = new FileStream(serversfile, FileMode.OpenOrCreate);
                sr = new StreamReader(fs);
                string s = "a";
                var split = new[] { "\t" };
                while (string.IsNullOrEmpty(s) == false)
                {
                    s = sr.ReadLine();
                    if (string.IsNullOrEmpty(s))
                        break;

                    string[] s2 = s.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    var CD = new ConnectDetails(s2[0], int.Parse(s2[1]), int.Parse(s2[2]));
                    if (servers.Contains(CD) == false)
                        servers.Add(CD);
                }
            }
            catch (Exception ex)
            {
                Manager.FireLogEvent("Error reading servers from file", SynchMain.MessagePriority.High, true);
                Logging.WriteExceptionToFile("file read error", ex);
            }

            if (sr != null)
                sr.Close();

            if (fs != null)
                fs.Close();

            return servers;
        }

        public static void SaveFileServers(List<ConnectDetails> cd)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            char sep = '\t';

            try
            {
                fs = new FileStream(serversfile, FileMode.Create);
                sw = new StreamWriter(fs);

                foreach (var c in cd)
                {
                    sw.WriteLine(c.ip+sep+c.TCPport+sep+c.UDPport);    
                }
                sw.Close();
                fs.Close();
            }
            catch(Exception e)
            {
                
            }
        }

        /// <summary>
        /// called from handlecomm event handler
        /// </summary>
        /// <param name="servers"></param>
        public static void AddAvailSectors(List<Heartbeat> servers)
        {
            foreach (Heartbeat h in servers)
            {
                if (AvailableSectors.ContainsKey(h.CD) == false)
                    AvailableSectors.Add(h.CD, h);
                else
                {
                    //update the info stored in the control in case the sector id changed on the server
                    Heartbeat asec = AvailableSectors[h.CD];
                    asec.id = h.id;
                    asec.name = h.name;
                    asec.Config = h.Config;
                    AvailableSectors[h.CD].active = true;
                }

                if (Waiting.ContainsKey(h.CD))
                {
                    StopWaitingServer(Waiting[h.CD]);
                }
            }
            availsecdirty = true;
        }

        private static void StopWaitingServer(WaitingThread wt)
        {
            wt.Wsynch.Value.StopServer();
            Waiting.Remove(wt.CD);
        }

        /// <summary>
        /// request game sectors from all master servers
        /// </summary>
        public static void StandaloneRequestJoinableSectors()
        {
            if (updateThread != null)
            {
                updateThread.Abort();
                updateThread = null;
            }
            updateThread = new Thread(StandaloneRequestJoinableSectorsThread);
            updateThread.Start();
        }

        private static void StandaloneRequestJoinableSectorsThread()
        {
            //for each master server
            foreach (ConnectDetails s in MasterServers)
            {
                EndFinishedRequests();

                //check to see if we have already asked, and are waiting for a response
                if (Waiting.ContainsKey(s))
                    continue;
                StartWaitingThread(s);
            }
        }

        private static void StartWaitingThread(ConnectDetails CD)
        {
            //1 connect to ip
            var gic = new GameInitConfig(CD.ip, true, CD.UDPport, CD.TCPport);
           
            SynchMain sm = null;

            var t = new Thread(() => Manager.StartGame(out sm, gic));
            t.Start();

            //wait for the synch main to be created
            while (sm == null)
            {
                Thread.Sleep(SMWaitMS);
            }

            //make sure we dont request again before being resolved
            Waiting.Add(CD, new WaitingThread(CD, t, ref sm, Manager.GetMillisecondsNow()));
        }

        private static void EndFinishedRequests()
        {
            double nowms = Manager.GetMillisecondsNow();

            List<WaitingThread> l = (from s in Waiting
                                     where
                                         (s.Value.Wthread.IsAlive == false &&
                                          (s.Value.Wsynch == null || s.Value.Wsynch.Value.enabled == false)) ||
                                         ((nowms - s.Value.StartMS) > (HeartbeatTimeoutMS))
                                     select s.Value).ToList();

            //remove all waiting where the thread has stopped
            while (l.Count() > 0)
            {
                WaitingThread ll = l.First();
                StopWaitingServer(ll);
                l.Remove(ll);
            }
        }

        #region Nested type: WaitingThread

        public class WaitingThread
        {
            public ConnectDetails CD;
            public double StartMS;
            public Wrapper<SynchMain> Wsynch;
            public Thread Wthread;

            public WaitingThread(ConnectDetails CDIN, Thread NWthread, ref SynchMain NWsynch, double NStartMS)
            {
                CD = CDIN;
                Wthread = NWthread;

                Wsynch = new Wrapper<SynchMain>(NWsynch);
                StartMS = NStartMS;
            }
        }

        #endregion

    }
}