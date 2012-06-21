using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class ConnectWaitWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance = null;
        /*
        public ConnectWaitWF()
        {
            InitializeComponent();
        }
        */
        //for return
        public delegate void CancelPressed(SynchMain sm, String ip);

        private CancelPressed cp;
        private SynchMain sm;

        public ConnectWaitWF(SynchMain sm, ConnectDetails CD,CancelPressed cp)
        {
            InitializeComponent();
            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            this.sm = sm;
            iplabel.Text = CD.ip;
            tcpport.Text= CD.TCPport.ToString();
            udpport.Text = CD.UDPport.ToString();
            this.cp = cp;
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

            if (c==cancelbutton)
            {
                cp(sm,iplabel.Text);
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
