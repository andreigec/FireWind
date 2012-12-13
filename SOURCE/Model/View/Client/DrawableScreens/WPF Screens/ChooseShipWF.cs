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
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using Project.View.Client.DrawableScreens.Pop_Up_Screens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.Full_Screens
{
    public partial class ChooseShipWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance = null;
        public Wrapper<object> CreateNewShipNameString = new Wrapper<object>("");

        public ChooseShipWF(String existingPSCName)
        {
            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            InitializeComponent();
            InitShipListBox();
            if (loadXML.loadedPlayerShips.ContainsKey(existingPSCName))
                ChooseShipListBox.SelectedItem = existingPSCName;
            UpdateChooseShipButton();
        }

        public void InitShipListBox()
        {
            ChooseShipListBox.Items.Clear();
            //get all the loaded ships
            
            foreach (var ps in loadXML.loadedPlayerShips)
            {
                ChooseShipListBox.Items.Add(ps.Key);
                //set default
                if (ps.Value.DefaultLoad)
                {
                    ChooseShipListBox.SelectedItem = ps.Key;
                }
            }
        }

        public void UpdateChooseShipButton()
        {
            if (ChooseShipListBox.SelectedIndex == -1)
            {
                ChooseSelectedShipButton.Enabled = false;
                selectdefault.Enabled = false;
            }
            else
            {
                ChooseSelectedShipButton.Enabled = true;
                selectdefault.Enabled = true;
                try
                {
                    var psc = loadXML.loadedPlayerShips[ChooseShipListBox.SelectedItem.ToString()];
                    selectdefault.Checked = psc.DefaultLoad;
                }
                catch (Exception)
                {
                }
            }
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
        }

        public void AcceptShipName(bool OKPressed)
        {
            if (OKPressed==false)
                return;
            
            PlayerShipClass.CreatePlayerShip(CreateNewShipNameString.Value.ToString());
            InitShipListBox();

            //if only one item, use it
            if (loadXML.loadedPlayerShips.Count==1)
                ChooseSelectedShip();
        }

        private void ChooseSelectedShip()
        {
            MainScreenWF.ChooseThisShipString = ChooseShipListBox.SelectedItem.ToString();
            GameControlClient.ShowMainScreen();
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            var c = XnaWfWrapperInstance.MouseUpdate(gt);
            if (c == null)
                return;

            if (mc.ButtonsPressed() == false)
                return;

            if (c == createnewshipbutton)
            {
                var ri = new List<RequiredItem>();
                CreateNewShipNameString = new Wrapper<object>("");
                ri.Add(RequiredItem.GetRIString(CreateNewShipNameString,"testtext"));
                GameControlClient.ShowGetInfoPopup(ri, AcceptShipName);

            }

            else if (c == gobacktomainmenu)
            {
                GameControlClient.ShowMainScreen();
            }
            else if (c == ChooseSelectedShipButton)
            {
                ChooseSelectedShip();
            }

            else if (c == selectdefault)
            {
                selectdefault.Checked = !selectdefault.Checked;

                var pscchoose = loadXML.loadedPlayerShips[ChooseShipListBox.SelectedItem.ToString()];
                pscchoose.DefaultLoad = selectdefault.Checked;
                PlayerShipClass.SavePlayerShipClassFile(pscchoose);

                //disable others if turning on
                if (selectdefault.Checked)
                {
                    foreach (var psc in loadXML.loadedPlayerShips.Where(s => s.Value.Name != pscchoose.Name))
                    {
                        psc.Value.DefaultLoad = false;
                        PlayerShipClass.SavePlayerShipClassFile(psc.Value);
                    }
                }
            }
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
        }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            UpdateChooseShipButton();

            XnaWfWrapperInstance.Draw();
        }
    }
}
