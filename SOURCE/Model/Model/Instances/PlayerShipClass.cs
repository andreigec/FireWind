using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Project.Model;
using Project.Model.Instances;
using Project.Model.Server.XML_Load;
using Project.Model.mapInfo;
using Project.Networking;

namespace Project
{
    public class PlayerShipClass
    {
        public const int StartCredits = 10000;
        public int Credits;
        public string Name;
        public List<ShipPart> OwnedParts = new List<ShipPart>();
        public List<weapon> OwnedWeapons = new List<weapon>();
        public List<shipBase> OwnedShipBases = new List<shipBase>();

        public ShipInstance PlayerShip;
        public bool DefaultLoad = false;

        public List<ShipInstance> SupportCraft = new List<ShipInstance>();

        public void ChangeCredits(int difAmount)
        {
            Credits += difAmount;

            //save change to credits
            SavePlayerShipClassFile();
        }

        public void ResetPSC()
        {
            PlayerShip.Reset();
            foreach (var s in SupportCraft)
            {
                s.Reset();
            }

            ChargeForReset();
        }

        public bool BuyItem(IPurchasable item)
        {
            if (item.Cost > Credits)
                return false;

            if (item is shipBase)
            {
                if (OwnedShipBases.Contains(item as shipBase))
                    return false;

                OwnedShipBases.Add(item as shipBase);
            }
            else if (item is ShipPart)
            {
                if (OwnedParts.Contains(item as ShipPart))
                    return false;

                OwnedParts.Add(item as ShipPart);
            }
            else if (item is weapon)
            {
                if (OwnedWeapons.Contains(item as weapon))
                    return false;

                OwnedWeapons.Add(item as weapon);
            }
            else
                return false;

            ChangeCredits(-item.Cost);
            Manager.FireLogEvent("Bought Item:" + item.name, SynchMain.MessagePriority.High, false);
            return true;
        }

        private void ChargeForReset()
        {
            var rebuildval = ShipBaseItemsMIXIN.GetRebuildCost(PlayerShip);
            int cost;
            if (Credits < rebuildval)
                cost = Credits;
            else
                cost = rebuildval;

            ChangeCredits(-cost);
        }

        #region creation

        public static void CreatePlayerShip(String name)
        {
            var psc = new PlayerShipClass();
            psc.Name = name;
            psc.PlayerShip = ShipInstance.addShip(null,"SHIP1", SetID.CreateNoID());
            //give default amount of credits
            psc.Credits = StartCredits;

            /*
            var sc = ShipInstance.addShip("SHIP2", SetID.CreateNoID());
            sc.AssignStockParts();
             * psc.SupportCraft.Add(sc);
            */


            //add all the parts we have assigned to ships to the owned list
            AssignPurchasedToOwned(psc, psc.PlayerShip);

            foreach (var s in psc.SupportCraft)
            {
                AssignPurchasedToOwned(psc, s);
            }

            //if there are no other player ships, make this the default
            if (loadXML.loadedPlayerShips.Count == 0)
                psc.DefaultLoad = true;

            //save ship created
            SavePlayerShipClassFile(psc);
        }

        private static void AssignPurchasedToOwned(PlayerShipClass psc, ShipInstance si)
        {
            psc.OwnedWeapons = new List<weapon>();
            psc.OwnedParts = new List<ShipPart>();
            psc.OwnedShipBases = new List<shipBase>();

            if (psc.OwnedShipBases.Contains(si.shipModel) == false)
                psc.OwnedShipBases.Add(si.shipModel);

            foreach (var w in si.Slots)
            {
                if (psc.OwnedWeapons.Contains(w.Value) == false)
                    psc.OwnedWeapons.Add(w.Value);
            }

            foreach (var w in si.Parts)
            {
                if (psc.OwnedParts.Contains(w.Value) == false)
                    psc.OwnedParts.Add(w.Value);
            }
        }

        #endregion creation

        #region file

        private static ShipPart ReInitialisePart(ShipPart sp)
        {
            var newsp = loadXML.loadedShipParts[sp.name];
            return newsp;
        }

        private static weaponslot ReInitialiseWeaponSlot(weaponslot SetFrom)
        {
            if (SetFrom == null)
                return null;
            var wname = SetFrom.w.name;
            var ws = new weaponslot(loadXML.loadedWeapons[wname]);
            return ws;
        }

        private static weapon ReInitialiseWeapon(weapon SetFrom)
        {
            if (SetFrom == null)
                return null;
            var wname = SetFrom.name;
            return loadXML.loadedWeapons[wname];
        }

        private static ShipInstance ReInitialiseShip(ShipInstance oldSi)
        {
            var newps = ShipInstance.addShip(null, ShipBaseItemsMIXIN.GetShipModelName(oldSi), SetID.CreateNoID());
            var partlist = new SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart>();
            foreach (var kvp in oldSi.Parts)
            {
                partlist.Add(kvp.Key, ReInitialisePart(kvp.Value));
            }

            newps.Parts = partlist;

            foreach (var s in oldSi.Slots)
            {
                newps.EquipSlots[s.Key] = ReInitialiseWeaponSlot(new weaponslot(s.Value));
                newps.Slots[s.Key] = ReInitialiseWeapon(s.Value);
            }

            //set hud elements here as the parts will have affected the values
            newps.CurrentArmour = ShipBaseItemsMIXIN.GetMaxArmour(newps);
            newps.CurrentEnergy = ShipBaseItemsMIXIN.GetMaxEnergy(newps);

            return newps;
        }

        private static void ReInitialisePSC(ref PlayerShipClass psc)
        {
            psc.PlayerShip = ReInitialiseShip(psc.PlayerShip);
            var supportlist = new List<ShipInstance>();
            foreach (var s in psc.SupportCraft)
            {
                supportlist.Add(ReInitialiseShip(s));
            }

            psc.SupportCraft = supportlist;

            //parts
            psc.OwnedParts = psc.OwnedParts.Select(p => loadXML.loadedShipParts[p.name]).ToList();

            //weapons
            psc.OwnedWeapons = psc.OwnedWeapons.Select(w => loadXML.loadedWeapons[w.name]).ToList();

            //bases
            psc.OwnedShipBases = psc.OwnedShipBases.Select(sb => loadXML.loadedShips[sb.name]).ToList();
        }

        private static void SerialiseCreate(string mainfilename, PlayerShipClass psc)
        {
            Shared.serialiseFile(psc, mainfilename);
        }

        /// <summary>
        /// read the PSC file
        /// </summary>
        /// <param name="shipFileName"></param>
        /// <returns></returns>
        public static PlayerShipClass LoadPlayerShipClassFile(String shipFileName)
        {
            var sectext = Shared.GetFileText(shipFileName);

            var s2 = Shared.DeserialiseFile<PlayerShipClass>(sectext);
            ReInitialisePSC(ref s2);
            Manager.FireLogEvent("Loaded PSC:" + shipFileName, SynchMain.MessagePriority.Low, false);
            return s2;
        }

        /// <summary>
        /// save the PSC to file
        /// </summary>
        /// <param name="overload"></param>
        public static void SavePlayerShipClassFile(PlayerShipClass overload = null)
        {
            PlayerShipClass psc;
            if (overload != null)
                psc = overload;
            else
                psc = GameControlClient.playerShipClass;

            var main = loadXML.userships + "\\" + psc.Name + ".xml";
            //save to file
            SerialiseCreate(main, psc);
            Manager.FireLogEvent("Saved PSC:" + main, SynchMain.MessagePriority.Low, false);

            //reload sectors
            loadXML.LoadPlayerShips();
        }

        #endregion file

        #region serialisation

        public List<string> SerialiseCreate()
        {
            var ret = new List<string>();
            ret.Add(Name);

            ret.AddRange(PlayerShip.SerialiseCreate(true));

            ret.Add(SupportCraft.Count.ToString());
            foreach (var sp in SupportCraft)
            {
                ret.AddRange(sp.SerialiseCreate(true));
            }
            return ret;
        }

        public static PlayerShipClass DeserialiseCreate(List<string> args, region r, int playerid, int faction,
                                                        IshipAreaSynch overload)
        {
            var ret = new PlayerShipClass();

            ret.Name = Shared.PopFirstListItem(args);
            ret.PlayerShip = ShipInstance.DeserialiseCreate(args, r, overload);

            var supportcount = Int32.Parse(Shared.PopFirstListItem(args));
            ret.SupportCraft = new List<ShipInstance>();
            for (var a = 0; a < supportcount; a++)
            {
                ret.SupportCraft.Add(ShipInstance.DeserialiseCreate(args, r, overload));
            }

            ret.SetInstanceOwner(playerid, faction);

            return ret;
        }

        private List<string> SerialiseShipLight(ShipInstance si)
        {
            var ret = new List<string>();
            //2
            ret.Add(si.ID.ToString());
            //3
            ret.AddRange(si.instanceOwner.SerialiseCreate());
            return ret;
        }

        /// <summary>
        /// after receiving the playershipclass from the client, the server needs to give them IDs and resend to the client
        /// </summary>
        /// <returns></returns>
        public List<string> SerialiseCreateLight()
        {
            var ret = new List<string>();
            ret.AddRange(SerialiseShipLight(PlayerShip));

            //4
            ret.Add(SupportCraft.Count.ToString());
            foreach (var sp in SupportCraft)
            {
                //5,6+
                ret.AddRange(SerialiseShipLight(sp));
            }
            return ret;
        }

        /// <summary>
        /// the client needs to add the server given ids to their playershipclass before joining a sector
        /// </summary>
        /// <param name="args"></param>
        /// <param name="current"></param>
        /// <param name="r"></param>
        /// <param name="from"></param>
        public static void DeserialiseCreateLight(List<string> args, PlayerShipClass current, region r,
                                                  ConnectedIPs from)
        {
            var psid = Int32.Parse(Shared.PopFirstListItem(args));
            current.PlayerShip = r.getShipInstance(psid);
            //3
            current.PlayerShip.instanceOwner = InstanceOwner.DeserialiseCreate(args);
            //dont send back to the owner
            current.PlayerShip.Init(from);
            current.PlayerShip.SynchInfoMain.SendCreateDelete = false;

            //4
            var supportcount = Int32.Parse(Shared.PopFirstListItem(args));
            current.SupportCraft = new List<ShipInstance>();
            for (var a = 0; a < supportcount; a++)
            {
                //5
                var sid = Int32.Parse(Shared.PopFirstListItem(args));
                var newship = r.getShipInstance(sid);
                //6+
                newship.instanceOwner = InstanceOwner.DeserialiseCreate(args);
                //dont send back to the owner
                newship.Init(from);
                newship.SynchInfoMain.SendCreateDelete = false;
                current.SupportCraft.Add(newship);
            }
        }

        public void SetInstanceOwner(int playerid, int faction)
        {
            //player is currently controlling the craft
            var ow = new InstanceOwner(playerid, faction, InstanceOwner.ControlType.JustOwner);
            //set the instance ids
            PlayerShip.instanceOwner = ow;

            //no one can control support craft
            ow = new InstanceOwner(playerid, faction, InstanceOwner.ControlType.NoPlayer);
            foreach (var sp in SupportCraft)
            {
                sp.instanceOwner = ow;
            }
        }

        #endregion serialisation
    }
}