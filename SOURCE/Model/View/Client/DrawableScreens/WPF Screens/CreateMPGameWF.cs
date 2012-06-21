using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class CreateMPGameWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance = null;

        public CreateMPGameWF()
        {
            InitializeComponent();
            tcpTB.Text = GameInitConfig.DefaultTCPPort.ToString();
            udpTB.Text = GameInitConfig.DefaultUDPPort.ToString();

            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            var c = XnaWfWrapperInstance.MouseUpdate(gt);
            if (c == null)
                return;

            if (mc.ButtonsPressed() == false)
                return;

            if (c == backbutton)
            {
                GameControlClient.ShowChooseGameModeScreen();
            }
            else if (c==creategamebutton)
            {
                bool iscol = colradio.Checked;

                int tcpport = int.Parse(tcpTB.Text);
                int udpport = int.Parse(udpTB.Text);
                string servname = nameTB.Text;
                int maxplayers = int.Parse(maxTB.Text);

                var gic = new GameInitConfig(false, servname, maxplayers, udpport, tcpport);

                SectorConfig sc=null;
                if (iscol)
                    sc=new SectorConfigColosseum(SectorConfig.Size.medium);

                GameControlClient.CreateGame(gic,sc);
            }
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
        }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            XnaWfWrapperInstance.Draw();
        }
    }
}
