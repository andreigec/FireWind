﻿using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Project.Model.mapInfo;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class ChooseGameModeWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance = null;

        public ChooseGameModeWF()
        {
            InitializeComponent();
            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);
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

            if (c == backtoservers)
            {
                GameControlClient.ShowConnectScreen();
            }
            else if (c == colbutton)
            {
                GameControlClient.ShowCreateMPGameScreen();
            }
            else if (c == gamblebutton)
            {
                GameControlClient.ShowCreateGambleGameScreen();
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