using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Model.Server.XML_Load;
using Project.Model.mapInfo;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;
using Point = System.Drawing.Point;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class ShopScreenWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance;

        private Type currentType;

        /// <summary>
        /// Current part from shop being displayed
        /// </summary>
        private IPurchasable currentPart;

        /// <summary>
        /// Current slot for part
        /// </summary>
        private SlotLocation.SlotLocationEnum currentSlot;
        private SlotLocation.SlotLocationEnum oldSlot;

        /// <summary>
        /// Matching currently equipped part of the same type as the current shop part type
        /// </summary>
        private IPurchasable currentPartEquip;

        private List<IPurchasable> typelist = null;

        public ShopScreenWF()
        {
            InitializeComponent();

            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            ShopScreens.InitSlotBox(slotbox);
            ScrollItems(0);
        }

        public void ScrollItems(int offset)
        {
            //get the type and slotlocation from the choose box
            if (slotbox.SelectedItem != null)
                currentSlot = (SlotLocation.SlotLocationEnum)Enum.Parse(typeof(SlotLocation.SlotLocationEnum), slotbox.SelectedItem.ToString());
            else
                currentSlot = SlotLocation.SlotLocationEnum.None;

            currentType = SlotLocation.EnumToType(currentSlot);

            if (currentSlot != oldSlot)
                currentPart = null;

            if (currentSlot != SlotLocation.SlotLocationEnum.None)
            {
                //load all the parts for this type
                ShopScreens.LoadSelectedParts(ref typelist, currentType, false);

                //get the current equipped item
                ShopScreens.ChangeCurrentType(ref currentPartEquip, currentSlot);

                //get the current part
                ShopScreens.RotatePartLists(ref currentPart, ref currentPartEquip, typelist, offset);
            }
            else
                currentPart = currentPartEquip = null;

            //if no more items to buy, clear equip
            if (typelist.Count == 0)
                currentPart = currentPartEquip = null;

            //show the box
            showitem.Init(currentPart, currentPartEquip,false);

            InitBuyButton();
            InitRotateButtons();
            oldSlot = currentSlot;
        }

        public void InitRotateButtons()
        {
            if (typelist.Count > 1)
            {
                nextbutton.Enabled = true;
                prevbutton.Enabled = true;
            }
            else
            {
                nextbutton.Enabled = false;
                prevbutton.Enabled = false;
            }
        }

        public void InitBuyButton()
        {
            if (currentPart == null || (currentPart.Cost > GameControlClient.playerShipClass.Credits))
            {
                buybutton.Enabled = false;
            }
            else
            {
                buybutton.Enabled = true;
            }
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

            if (c == slotbox)
            {
                ScrollItems(0);
            }
            else if (c == gobacktobase)
            {
                GameControlClient.ResetToBaseScreen(false);
            }
            else if (c == buybutton && mc.ButtonsDown.ContainsKey(MouseClass.mouseButtons.left))
            {
                GameControlClient.playerShipClass.BuyItem(currentPart);

                //save changes
                PlayerShipClass.SavePlayerShipClassFile();

                ScrollItems(0);
            }
            else if (c == prevbutton && mc.ButtonsDown.ContainsKey(MouseClass.mouseButtons.left))
            {
                ScrollItems(-1);
            }
            else if (c == nextbutton && mc.ButtonsDown.ContainsKey(MouseClass.mouseButtons.left))
            {
                ScrollItems(1);
            }
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
        }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            if (XnaWfWrapperInstance.Resized())
                showitem.Init(currentPart, currentPartEquip, false);

            XnaWfWrapperInstance.Draw();
        }
    }
}
