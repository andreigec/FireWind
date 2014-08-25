using System;
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
        /*
        public ConnectWaitWF()
        {
            InitializeComponent();
        }
        */
        //for return

        #region Delegates

        public delegate void CancelPressed(SynchMain sm, String ip);

        #endregion

        public static XNA_WF_Wrapper XnaWfWrapperInstance = null;

        private readonly CancelPressed cp;
        private readonly SynchMain sm;

        public ConnectWaitWF(SynchMain sm, ConnectDetails CD, CancelPressed cp)
        {
            InitializeComponent();
            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            this.sm = sm;
            iplabel.Text = CD.ip;
            tcpport.Text = CD.TCPport.ToString();
            udpport.Text = CD.UDPport.ToString();
            this.cp = cp;
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

            if (c == cancelbutton)
            {
                cp(sm, iplabel.Text);
            }
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
        }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            XnaWfWrapperInstance.Draw();
        }

        #endregion
    }
}