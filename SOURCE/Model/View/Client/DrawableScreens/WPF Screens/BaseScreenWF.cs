﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Project.Model.mapInfo;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.Full_Screens
{
    public partial class BaseScreenWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance;

        public BaseScreenWF()
        {
            InitializeComponent();

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

            if (c == FlyButton)
            {
                GameControlClient.ShowConnectScreen();
            }
            else if (c == GoToHangarButton)
            {
                GameControlClient.ShowHangarScreen();
            }

            else if (c == GoToShopButton)
            {
                GameControlClient.ShowBaseShopScreen();
            }
            else if (c==gobacktomainmenu)
            {
                GameControlClient.ShowMainScreen();
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
