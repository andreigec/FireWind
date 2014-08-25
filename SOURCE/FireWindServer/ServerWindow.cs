using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Project;
using Project.Model;
using Project.Networking;

namespace FireWindServer
{
    public partial class ServerWindow : UserControl
    {
        public Form1 baseform;
        public List<GameWindow> gamewindows = new List<GameWindow>();
        public SynchMain sm;

        public ServerWindow(Form1 baseform)
        {
            InitializeComponent();
            this.baseform = baseform;
            CreateServerTabPage();
        }

        private void toggleserverbutton_Click(object sender, EventArgs e)
        {
            if (sm == null)
                StartServer();
            else
                StopServer();
        }

        public GameWindow GetGameSectorWindow(long sectorID)
        {
            //get matching synchmain
            IEnumerable<GameWindow> s1 = gamewindows.Where(s => s.sec.ID == sectorID);
            if (s1.Count() != 1)
                return null;

            GameWindow gw = s1.First();
            return gw;
        }

        public string GetAlias(FormMessageStore fms)
        {
            IEnumerable<ConnectedClient> clientL = sm.connectedClients.Where(s => s.ID == fms.clientID);
            ConnectedClient clientI = null;
            if (clientL.Count() > 0)
                clientI = clientL.FirstOrDefault();

            String client = "ERROR";

            if (clientI != null)
                client = clientI.alias;

            return client;
        }

        public void StartServer()
        {
            if (sm != null)
                return;

            int udp = int.Parse(udpportText.Text);
            int tcp = int.Parse(tcpread.Text);
            int mp = int.Parse(maxplayer.Text);
            try
            {
                //init logwindow
                var gic = new GameInitConfig(true, servernametext.Text, mp,lanonlyCB.Checked, udp, tcp);

                //init actual game
                Manager.StartGame(out sm, gic);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating server:\n" + ex, "error");
                if (sm != null)
                    sm.StopServer();
                return;
            }
            UpdateServerButtons();
        }

        private void CreateServerTabPage()
        {
            var tw = new TabPage();
            tw.Text = "Server Config";

            Dock = DockStyle.Fill;

            tw.Controls.Add(this);

            baseform.tabControl1.TabPages.Add(tw);
            UpdateServerButtons();
        }

        public void StartGame(SectorConfig sc)
        {
            Sector sec = sm.gcs.gameRegion.addSector(sc);

            GameWindow gw = CreateGameTabPage(sec);
            Dock = DockStyle.Fill;

            var lvi = new ListViewItem();
            lvi.Text = sec.ID.ToString();
            lvi.Name = GetSectorNameKey(gw.sec);
            lvi.SubItems.Add(sec.Config.ToString());
            lvi.SubItems.Add("0");
            lvi.Tag = gw;
            gamewindowlist.Items.Add(lvi);

            UpdateServerButtons();
        }

        private GameWindow CreateGameTabPage(Sector s)
        {
            var gw = new GameWindow(s, this);
            var tp = new TabPage(sm.gcs.gameConfig.serverName + " " + s.ID.ToString());
            tp.Name = GetSectorNameKey(s);
            tp.Controls.Add(gw);
            tp.Dock = DockStyle.Fill;

            baseform.tabControl1.TabPages.Add(tp);
            baseform.tabControl1.SelectedIndex = baseform.tabControl1.TabPages.Count - 1;
            gamewindows.Add(gw);

            return gw;
        }

        private void StopGame(GameWindow lw)
        {
            TabControl tc = baseform.tabControl1;
            for (int a = 0; a < tc.TabPages.Count; a++)
            {
                if (tc.TabPages[a].Controls[0] is GameWindow)
                {
                    var lw2 = tc.TabPages[a].Controls[0] as GameWindow;
                    if (lw == lw2)
                    {
                        tc.TabPages.Remove(tc.TabPages[a]);
                        break;
                    }
                }
                a++;
            }

            string k = GetSectorNameKey(lw.sec);
            baseform.tabControl1.TabPages.RemoveByKey(k);
            gamewindows.Remove(lw);
            gamewindowlist.Items.RemoveByKey(k);
        }

        private String GetSectorNameKey(Sector s)
        {
            return "s" + s.ID.ToString();
        }

        public void StopServer()
        {
            while (gamewindows.Count > 0)
            {
                GameWindow gw = gamewindows.First();
                StopGame(gw);
            }

            Manager.EndGame(ref sm, true);
            gamewindows = new List<GameWindow>();
            gamewindowlist.Items.Clear();
            sm = null;

            UpdateServerButtons();
        }

        public void UpdateServerButtons()
        {
            if (sm == null)
            {
                toggleserverbutton.Text = "Start Server";
            }
            else
            {
                toggleserverbutton.Text = "Stop Server";
            }

            stopselectedgamesbutton.Enabled =
                creategamebutton.Enabled =
                (sm != null);

            servernametext.Enabled =
                maxplayer.Enabled =
                Rconpwtext.Enabled =
                tcpread.Enabled =
                udpportText.Enabled =
                lanonlyCB.Enabled=
                (sm == null);
        }

        private void stopselectedgamesbutton_Click(object sender, EventArgs e)
        {
            var selsectors = new List<GameWindow>();
            foreach (ListViewItem lvi in gamewindowlist.SelectedItems)
            {
                if (lvi.Tag is GameWindow)
                    selsectors.Add(lvi.Tag as GameWindow);
            }

            foreach (GameWindow s in selsectors)
            {
                StopGame(s);
            }
        }

        private void creategamebutton_Click(object sender, EventArgs e)
        {
            var sc = new SectorConfigColosseum(SectorConfig.Size.medium);
            StartGame(sc);
        }

        private void gamewindowlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            stopselectedgamesbutton.Enabled = gamewindowlist.SelectedItems.Count > 0;
        }
    }
}