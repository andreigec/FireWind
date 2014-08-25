using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Project.Model.Server.XML_Load;
using Project.Model.mapInfo;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class HangarScreenWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance;

        /// <summary>
        /// Current part from shop being displayed
        /// </summary>
        private IPurchasable currentPart;

        /// <summary>
        /// Matching currently equipped part of the same type as the current shop part type
        /// </summary>
        private IPurchasable currentPartEquip;

        /// <summary>
        /// Current slot for part
        /// </summary>
        private SlotLocation.SlotLocationEnum currentSlot;

        private Type currentType;

        private SlotLocation.SlotLocationEnum oldSlot;

        private List<IPurchasable> typelist;

        public HangarScreenWF()
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

            if (c == slotbox)
            {
                ScrollItems(0);
            }
            else if (c == gobacktobase)
            {
                GameControlClient.ResetToBaseScreen(false);
            }
            else if (c == applyitem)
            {
                if (currentType == typeof (weapon))
                {
                    GameControlClient.playerShipClass.PlayerShip.AssignWeapon(currentSlot, currentPart as weapon);
                }
                else if (currentType.BaseType == typeof (ShipPart))
                {
                    GameControlClient.playerShipClass.PlayerShip.AssignPart(currentPart as ShipPart);
                }
                else if (currentType == typeof (shipBase))
                {
                    GameControlClient.playerShipClass.PlayerShip.AssignBase(currentPart as shipBase);
                }

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
                showitem.Init(currentPart, currentPartEquip, true);

            XnaWfWrapperInstance.Draw();
        }

        #endregion

        public void ScrollItems(int offset)
        {
            //get the type and slotlocation from the choose box
            if (slotbox.SelectedItem != null)
                currentSlot =
                    (SlotLocation.SlotLocationEnum)
                    Enum.Parse(typeof (SlotLocation.SlotLocationEnum), slotbox.SelectedItem.ToString());
            else
                currentSlot = SlotLocation.SlotLocationEnum.None;

            currentType = SlotLocation.EnumToType(currentSlot);

            if (currentSlot != oldSlot)
                currentPart = null;

            if (currentSlot != SlotLocation.SlotLocationEnum.None)
            {
                //load all the parts for this type
                ShopScreens.LoadSelectedParts(ref typelist, currentType, true);

                //get the current equipped item
                ShopScreens.ChangeCurrentType(ref currentPartEquip, currentSlot);

                //get the current part
                ShopScreens.RotatePartLists(ref currentPart, ref currentPartEquip, typelist, offset);
            }
            else
                currentPart = currentPartEquip = null;

            //show the box
            showitem.Init(currentPart, currentPartEquip, true);

            InitApplyButton();
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

        public void InitApplyButton()
        {
            if (currentPart == null || currentPart == currentPartEquip)
            {
                applyitem.Enabled = false;
            }
            else
            {
                applyitem.Enabled = true;
            }
        }
    }
}