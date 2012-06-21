using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model;
using Project.Model.Networking;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using Project.View.Client.DrawableScreens.Pop_Up_Screens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.Full_Screens
{
    public partial class MainScreenWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance;
        /// <summary>
        /// if this is set, the next time the mainscreenwf is initialised, it will load the chosen playershipclass by this value and reset to null if correct or not
        /// </summary>
        public static string ChooseThisShipString = null;


        public MainScreenWF()
        {
            InitializeComponent();

            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);
        }

        public MainScreenWF(bool loadplayership)
        {
            InitializeComponent();

            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            //store current PSC name
            if (GameControlClient.PlayerShipSet())
                ChooseThisShipString = GameControlClient.playerShipClass.Name;

            //load ships
            loadXML.LoadPlayerShips();

            if (loadplayership&&ChooseThisShipString != null)
            {
                try
                {
                    var choose = loadXML.loadedPlayerShips[ChooseThisShipString];
                    GameControlClient.SetPlayerShipClass(choose);
                }
                catch (Exception)
                {
                    GameControlClient.SetPlayerShipClass(null);
                }

                ChooseThisShipString = null;
            }

            else if (loadplayership)
            {
                var defaultPSC = loadXML.loadedPlayerShips.Where(s => s.Value.DefaultLoad);
                if (defaultPSC.Count()>=1)
                {
                    var dPSC = defaultPSC.First();
                    GameControlClient.SetPlayerShipClass(dPSC.Value);
                }
            }
            
            SetBaseButton();
        }


        //public static string defaultIP = "127.0.0.1";
        //private MenuOptionsCreate menucreate;

        #region view

        public void SetBaseButton()
        {
            if (GameControlClient.PlayerShipSet()==false)
            {
                basebutton.Enabled = false;
                basebutton.Text = Localisation.GoToBaseDenied;
            }
            else
            {
                basebutton.Enabled = true;
                basebutton.Text = Localisation.GoToBase;
            }
        }

        #endregion view

        #region IScreenControls Members

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            XnaWfWrapperInstance.Draw();
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            /*
            kbc.ClearKeyTimeout();
            kbc.InitialiseKeyPress(Keys.Down);
            kbc.InitialiseKeyPress(Keys.Up);
            kbc.InitialiseKeyPress(Keys.Enter);
            kbc.InitialiseKeyPress(Keys.Escape);
             * */
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
            /*
            var handled = menucreate.HandleKey(kbc);
            if (handled)
                return;

            if (kbc.CanUseKey(Keys.Enter))
            {
                //choose ship
                if (menucreate.currentOption.Parent.Text != null &&
                    menucreate.currentOption.Text.Equals(Localisation.NewPlayerShip) == false &&
                    menucreate.currentOption.Parent.Text.Equals(Localisation.SelectShipAndPlayGame))
                {
                    ChosenPlayerShip = loadXML.loadedPlayerShips[menucreate.currentOption.Text];
                    ChosenPlayerShip.Name = menucreate.currentOption.Text;
                    foreach (var c in menucreate.currentOption.children)
                    {
                        c.Parent = menucreate.currentOption;
                    }

                    if (ChosenPlayerShip != null)
                        menucreate.traverse(ref menucreate.currentOption, true, 1);
                }

                    //create ship
                else if (menucreate.currentOption.Text == Localisation.CreateNewPlayerShip &&
                         menucreate.currentOption.isLeaf())
                {
                    CreatePlayerShip();
                    return;
                }

                else if (menucreate.currentOption.Text.Equals(Localisation.SinglePlayer))
                {
                    var gic2 = new GameInitConfig();
                    GameControlClient.CreateGame(gic2, ChosenPlayerShip);
                }

                else if (menucreate.currentOption.Text.Equals(Localisation.CreateListenServer))
                {
                    //get variables
                    var tcp1 = menucreate.currentOption.getNodeValueText(Localisation.TCPPort);
                    if (tcp1 == null)
                        return;
                    var tcp2 = int.Parse(tcp1);

                    var udp1 = menucreate.currentOption.getNodeValueText(Localisation.UDPPort);
                    if (udp1 == null)
                        return;
                    var udp2 = int.Parse(udp1);

                    var sn1 = menucreate.currentOption.getNodeValueText(Localisation.ServerName);
                    if (sn1 == null)
                        return;

                    var mp1 = menucreate.currentOption.getNodeValueText(Localisation.MaxPlayers);
                    if (mp1 == null)
                        return;
                    var mp2 = int.Parse(mp1);

                    var gic = new GameInitConfig(GameInitConfig.ServerType.ListenServer, sn1, mp2, udp2, tcp2);
                    GameControlClient.CreateGame(gic, ChosenPlayerShip);
                }

                else if (menucreate.currentOption.Text.Equals(Localisation.JoinServer))
                {
                    var ctcp1 = menucreate.currentOption.getNodeValueText(Localisation.TCPPort);
                    if (ctcp1 == null)
                        return;
                    var ctcp2 = int.Parse(ctcp1);

                    var cudp1 = menucreate.currentOption.getNodeValueText(Localisation.UDPPort);
                    if (cudp1 == null)
                        return;
                    var cudp2 = int.Parse(cudp1);

                    var ip1 = menucreate.currentOption.getNodeValueText(Localisation.IPAddress);
                    if (ip1 == null)
                        return;
                    IPAddress ip2 = null;
                    try
                    {
                        ip2 = IPAddress.Parse(ip1);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    var ip3 = ip2.ToString();
                    if (string.IsNullOrEmpty(ip3))
                        return;

                    var gic1 = new GameInitConfig(cudp2, ctcp2, ip3);
                    GameControlClient.CreateGame(gic1, ChosenPlayerShip);
                }
                else if (menucreate.currentOption.Text.Equals(Localisation.Exit))
                {
                    GameControlClient.EndGame(true);
                    return;
                }
                    //start editing
                else if (menucreate.currentOption.isLeaf() && menucreate.currentOption.hasValueText &&
                         menucreate.currentOption.options == false)
                {
                    if (menucreate.currentOption.Text.Equals(Localisation.ShipName))
                        MenuOptionsCreate.RegisterKeyboardValueEntry(kbc, true, true, false, false);
                    if (menucreate.currentOption.Text.Equals(Localisation.IPAddress))
                        MenuOptionsCreate.RegisterKeyboardValueEntry(kbc, true, true, true, false);
                    else if (menucreate.currentOption.Text.Equals(Localisation.ServerName))
                        MenuOptionsCreate.RegisterKeyboardValueEntry(kbc, true, false, false, true);
                    else if (menucreate.currentOption.Text.Equals(Localisation.TCPPort) ||
                             menucreate.currentOption.Text.Equals(Localisation.UDPPort) ||
                             menucreate.currentOption.Text.Equals(Localisation.MaxPlayers))
                        MenuOptionsCreate.RegisterKeyboardValueEntry(kbc, false, true, false, false);
                    else
                        MenuOptionsCreate.RegisterKeyboardValueEntry(kbc, true, true, true, true);

                    menucreate.StartEditingCurrent();
                }
                    //enter option
                else
                {
                    menucreate.traverse(ref menucreate.currentOption, false, 1);
                }
            }
             * */
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            var c = XnaWfWrapperInstance.MouseUpdate(gt);
            if (c == null)
                return;

            if (mc.ButtonsPressed() == false)
                return;

            if (c == exitbutton)
            {
                GameControlClient.EndGame(true);
            }

            else if (c == basebutton)
            {
                GameControlClient.ResetToBaseScreen(false);
            }

            else if (c==chooseshipbutton)
            {
                string pscname = "";
                if (GameControlClient.PlayerShipSet())
                    pscname = GameControlClient.playerShipClass.Name;

                GameControlClient.ShowChooseShipScreen(pscname);
            }
            else if (c==GoToOptionsButton)
            {
                GameControlClient.ShowOptionsScreen();
            }
        }

        #endregion

        private void MainScreenWF_Load(object sender, EventArgs e)
        {

        }


        /*
        private static string getRandomShipName()
        {
            var fcount = Directory.GetFiles(loadXML.userships).Length;

            String t;
            do
            {
                fcount++;
                t = "NewShip" + fcount.ToString();
            } while (File.Exists(loadXML.userships + "/" + t));

            return t;
        }
        /*
        private void generateMenu()
        {
            menucreate = new MenuOptionsCreate();
            menucreate.rootOptions = new MenuOptions(null, false);
            //select world
            var one = menucreate.rootOptions.addChild(Localisation.SelectShipAndPlayGame, false);
            menucreate.rootOptions.addChild(Localisation.Exit, false);

            //1p
            var three = new MenuOptions("r", false);
            three.addChild(Localisation.SinglePlayer, false);
            //mp
            var mp = three.addChild(Localisation.MultiPlayer, false);

            //player chooses a planet they want to use
            foreach (var s in loadXML.loadedPlayerShips)
            {
                var two = one.addChild(s.Key, false);
                foreach (var cin in three.children)
                {
                    two.addChild(cin);
                }
            }

            var create = one.addChild(Localisation.NewPlayerShip, true);

            create.addChild(Localisation.ShipName, true).valueText = getRandomShipName();

            create.addChild(Localisation.CreateNewPlayerShip, false);

            //LS
            var ls = mp.addChild(Localisation.HostLocalListenServer, true);
            ls.addChild(Localisation.TCPPort, true).valueText = "3001";
            ls.addChild(Localisation.UDPPort, true).valueText = "3003";
            ls.addChild(Localisation.ServerName, true).valueText = "Test Server Name";
            ls.addChild(Localisation.MaxPlayers, true).valueText = "8";
            ls.addChild(Localisation.CreateListenServer, false);

            //client
            var cl = mp.addChild(Localisation.JoinGame, false);
            cl.addChild(Localisation.TCPPort, true).valueText = "3001";
            cl.addChild(Localisation.UDPPort, true).valueText = "3003";
            cl.addChild(Localisation.IPAddress, true).valueText = defaultIP;
            cl.addChild(Localisation.JoinServer, false);


            menucreate.currentOption = one;
        }
        

        public void CreatePlayerShip()
        {
            //get variables
            var name = menucreate.currentOption.getNodeValueText(Localisation.ShipName);
            if (name == null)
                return;

            PlayerShipClass.CreatePlayerShip(name);

            generateMenu();
            menucreate.currentOption = menucreate.rootOptions.getNode(name).Parent.children[0];
        }
         * */
    }
}

