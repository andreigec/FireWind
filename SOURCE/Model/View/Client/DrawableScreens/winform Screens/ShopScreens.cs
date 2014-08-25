using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Project.Model.Server.XML_Load;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public static class ShopScreens
    {
        public static void LoadSelectedParts(ref List<IPurchasable> typelist, Type currentType, bool onlyOwned)
        {
            typelist = new List<IPurchasable>();
            List<weapon> ow = GameControlClient.playerShipClass.OwnedWeapons;
            List<ShipPart> op = GameControlClient.playerShipClass.OwnedParts;
            List<shipBase> os = GameControlClient.playerShipClass.OwnedShipBases;

            if (currentType == typeof (weapon))
            {
                typelist.AddRange(
                    loadXML.loadedWeapons.Where(xx => ow.Contains(xx.Value) == onlyOwned).Select(x => x.Value));
            }
            else if (currentType == typeof (ShipBatteryPart))
            {
                typelist.AddRange(
                    loadXML.loadedShipParts.Where(
                        s => op.Contains(s.Value) == onlyOwned && s.Value.GetType() == typeof (ShipBatteryPart)).Select(
                            x => x.Value));
            }
            else if (currentType == typeof (ShipBoosterPart))
            {
                typelist.AddRange(
                    loadXML.loadedShipParts.Where(
                        s => op.Contains(s.Value) == onlyOwned && s.Value.GetType() == typeof (ShipBoosterPart)).Select(
                            x => x.Value));
            }
            else if (currentType == typeof (ShipEnginePart))
            {
                typelist.AddRange(
                    loadXML.loadedShipParts.Where(
                        s => op.Contains(s.Value) == onlyOwned && s.Value.GetType() == typeof (ShipEnginePart)).Select(
                            x => x.Value));
            }
            else if (currentType == typeof (ShipGeneratorPart))
            {
                typelist.AddRange(
                    loadXML.loadedShipParts.Where(
                        s => op.Contains(s.Value) == onlyOwned && s.Value.GetType() == typeof (ShipGeneratorPart)).
                        Select(x => x.Value));
            }
            else if (currentType == typeof (ShipHullPart))
            {
                typelist.AddRange(
                    loadXML.loadedShipParts.Where(
                        s => op.Contains(s.Value) == onlyOwned && s.Value.GetType() == typeof (ShipHullPart)).Select(
                            x => x.Value));
            }
            else if (currentType == typeof (shipBase))
            {
                typelist.AddRange(loadXML.loadedShips.Where(s => os.Contains(s.Value) == onlyOwned).Select(x => x.Value));
            }
            else if (currentType == typeof (ShipWingsPart))
            {
                typelist.AddRange(
                    loadXML.loadedShipParts.Where(
                        s => op.Contains(s.Value) == onlyOwned && s.Value.GetType() == typeof (ShipWingsPart)).Select(
                            x => x.Value));
            }
        }

        public static void ChangeCurrentType(ref IPurchasable currentPartEquip, SlotLocation.SlotLocationEnum sl)
        {
            currentPartEquip = null;
            if (sl == SlotLocation.SlotLocationEnum.None)
            {
            }
            else if (sl == SlotLocation.SlotLocationEnum.ShipModel)
            {
                currentPartEquip = GameControlClient.playerShipClass.PlayerShip.shipModel;
            }
            else if (sl == SlotLocation.SlotLocationEnum.LeftWeapon)
            {
                if (GameControlClient.playerShipClass.PlayerShip.Slots.ContainsKey(sl))
                    currentPartEquip =
                        GameControlClient.playerShipClass.PlayerShip.Slots[SlotLocation.SlotLocationEnum.LeftWeapon];
            }
            else if (sl == SlotLocation.SlotLocationEnum.RightWeapon)
            {
                if (GameControlClient.playerShipClass.PlayerShip.Slots.ContainsKey(sl))
                    currentPartEquip =
                        GameControlClient.playerShipClass.PlayerShip.Slots[SlotLocation.SlotLocationEnum.RightWeapon];
            }
            else
            {
                if (GameControlClient.playerShipClass.PlayerShip.Parts.ContainsKey(sl))
                    currentPartEquip = GameControlClient.playerShipClass.PlayerShip.Parts[sl];
            }
        }

        public static void InitSlotBox(ListBox slotbox)
        {
            List<string> el = SlotLocation.GetEnumStringList();

            slotbox.Items.Clear();
            foreach (string s in el)
            {
                if (s == SlotLocation.SlotLocationEnum.None.ToString())
                    continue;

                slotbox.Items.Add(s);
                if (slotbox.Items.Count == 1)
                    slotbox.SelectedItem = s;
            }
        }

        public static void RotatePartLists(ref IPurchasable currentPart, ref IPurchasable equipped,
                                           List<IPurchasable> PartList, int offset)
        {
            //change the current part and the equipped current part
            if (PartList == null || PartList.Count == 0)
            {
                currentPart = equipped = null;
                return;
            }

            //if the current buy part doesnt exist, make it the first item in the part list
            if (currentPart == null || PartList.Contains(currentPart) == false)
            {
                if (equipped != null && PartList.Contains(equipped))
                    currentPart = equipped;
                else
                    currentPart = PartList.First();
            }

            //sort by name
            PartList.Sort((p1, p2) => p1.name.CompareTo(p2.name));

            //if set and no offset, return
            if (currentPart != null && offset == 0)
                return;

            //if there is an offset
            if (currentPart != null)
            {
                if (offset < 0)
                {
                    offset = (PartList.Count + offset)%PartList.Count;
                }

                int t = PartList.Count;
                int currentpos = PartList.IndexOf(currentPart);
                int newpos = currentpos + offset;
                if (newpos < 0)
                    newpos = (t - newpos);

                newpos = newpos%t;

                currentPart = PartList[newpos];
            }
        }
    }
}