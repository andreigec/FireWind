using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Project.Model;
using Project.Model.Instances;
using Project.Model.mapInfo;
using Project.Networking;
using Project.Networking.mapInfoSynch;

namespace Project
{
    public class ShipDirty
    {
        public double LastArmour;
        public bool LastDisabled;
        public double LastShieldTime;
        public VectorMove LastVM = new VectorMove();
        public double lastTimeUpdate;
    }

    public partial class ShipInstance :
        ISynchroniseInterfaceDrawObject<ConnectedIPs, SynchStatusShip, SynchStatusMain, ShipDirty>
    {
        #region ISynchroniseInterfaceDrawObject<ConnectedIPs,SynchStatusShip,SynchStatusMain,ShipDirty> Members

        [XmlIgnore]
        public Dictionary<int, SynchStatusShip> SynchInfo { get; set; }

        [XmlIgnore]
        public SynchStatusMain SynchInfoMain { get; set; }

        [XmlIgnore]
        public Dictionary<int, ShipDirty> LastServerPlayerUpdate { get; set; }

        public void Init(ConnectedIPs client = null)
        {
            ISynchInterfaceMIXIN.InitObjectType(this, client);
        }

        public void Cleanup(ConnectedIPs client)
        {
            ISynchInterfaceMIXIN.Cleanup(this, client);

            if (LastServerPlayerUpdate != null && LastServerPlayerUpdate.ContainsKey(client.ID))
                LastServerPlayerUpdate.Remove(client.ID);
        }

        public bool Synchronise(ConnectedIPs client)
        {
            Init(client);
            SynchStatusShip SynchInfo = this.SynchInfo[client.ID];
            if (SynchInfo == null)
                return false;

            ShipDirty LastServerPlayerUpdate = this.LastServerPlayerUpdate[client.ID];
            if (LastServerPlayerUpdate == null)
                return false;

            if (SynchInfo.Created == false && SynchInfoMain.SendCreateDelete)
            {
                RemoteCreate(client, SynchInfo);
                return true;
            }
            RemoteUpdate(client, SynchInfo);
            return false;
        }

        //addUpdateShipStatsToQueue

        //dirty - addUpdateShipPositionToQueue
        public bool IsDirty(ConnectedIPs client)
        {
            if (spriteInstance == null || spriteInstance.move == null)
                return false;

            ShipDirty sd = LastServerPlayerUpdate[client.ID];
            if (sd == null)
                return true;

            const float poff = 50;
            const float aoff = 5;

            bool posdirty =
                (
                    (spriteInstance.move.Position.Middle.X > (sd.LastVM.Position.Middle.X + poff)) ||
                    (spriteInstance.move.Position.Middle.X < (sd.LastVM.Position.Middle.X - poff))
                );

            bool angledirty = VectorMove.angleInBetween(spriteInstance.move.Angle, sd.LastVM.Angle) > aoff;

            double timenow = DateTime.Now.Second;
            bool timedirty = sd.lastTimeUpdate != timenow;

            if (posdirty || angledirty || timedirty)
            {
                sd.LastVM = spriteInstance.move.Clone();
                sd.lastTimeUpdate = timenow;
                return true;
            }

            return false;
        }

        public void SynchroniseMain(List<ConnectedIPs> clients)
        {
            Init();
        }

        public void RemoteCreate(ConnectedIPs client, SynchStatusShip SynchInfo)
        {
            if (SynchInfoMain.SendCreateDelete == false)
                return;

            addCreateShipToQueue(client, SynchInfo);
        }

        public void RemoteRemoveObject(ConnectedIPs connection, SynchStatusShip SynchInfo)
        {
            if (SynchInfo.Deleted.WaitForResponse || SynchInfoMain.SendCreateDelete == false)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.ShipRemove.ToString("d"));
            o.Add(ID.ToString());
            //destroyed?
            o.Add((CurrentArmour <= 0).ToString());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, connection);
            connection.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Deleted, false);
        }

        public bool RemoteUpdate(ConnectedIPs client, SynchStatusShip SynchInfo)
        {
            //send ship pop/push message from hangar
            if (SynchInfo.ParentArea.Value != ((SharedID) parentArea).ID && SynchInfoMain.SendCreateDelete)
            {
                addSendShipAreaToQueue(client, SynchInfo);
            }

            //TEMP: for now dont change players own ship position
            if (!(instanceOwner.PlayerOwnerID == client.ID && client is ConnectedClient))
            {
                //check if we need to update this client
                if (IsDirty(client))
                {
                    addUpdateShipPositionToQueue(client);
                    SynchInfo.UpdateLocation.Value = false;
                }
            }

            if (IsDirtyStats(client))
            {
                addUpdateShipStatsToQueue(client);
            }
            return false;
        }

        #endregion

        public bool IsDirtyStats(ConnectedIPs client)
        {
            ShipDirty sd = LastServerPlayerUpdate[client.ID];
            if (sd == null)
                return true;

            bool armourdirty = sd.LastArmour != CurrentArmour;
            //only send shield show on update
            bool shieldtimedirty = sd.LastShieldTime < ShowShieldTill;

            bool disableddirty = sd.LastDisabled != disabled;

            if (armourdirty || shieldtimedirty || disableddirty)
            {
                sd.LastShieldTime = ShowShieldTill;
                sd.LastArmour = CurrentArmour;
                sd.LastDisabled = disabled;
                return true;
            }
            return false;
        }

        public void addUpdateShipStatsToQueue(ConnectedIPs cc) //ALSO ACCESS FROM PLAYER CONNECTEDSERVERSYNCH
        {
            //TEMP: for now dont update server
            if (cc is ConnectedServer)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.UpdateShipStats.ToString("d"));
            o.AddRange(SerialiseUpdateStats());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
        }

        public void addUpdateShipPositionToQueue(ConnectedIPs cc) //ALSO ACCESS FROM PLAYER CONNECTEDSERVERSYNCH
        {
            var o = new List<string>();
            o.Add(Message.sendVars.UpdateShipPosition.ToString("d"));
            o.AddRange(SerialiseUpdatePosition());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
        }

        private void addSendShipAreaToQueue(ConnectedIPs cc, SynchStatusShip SynchInfo)
        {
            if (SynchInfo.ParentArea.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.ShipAreaUpdate.ToString("d"));
            o.AddRange(SerialiseSendShipArea());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
            cc.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.ParentArea, ((SharedID) parentArea).ID);
        }

        private void addCreateShipToQueue(ConnectedIPs cc, SynchStatusShip SynchInfo)
        {
            if (SynchInfo.Created.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.CreateShip.ToString("d"));
            o.AddRange(SerialiseCreate(false));

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
            cc.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Created, true);
        }

        public List<string> SerialiseSendShipArea()
        {
            var o = new List<string>();
            o.Add(ID.ToString());
            o.Add(((SharedID) parentArea).ID.ToString());
            o.AddRange(spriteInstance.SerialisePosition());
            return o;
        }

        public static bool DeserialiseSendShipArea(List<string> args, Region r)
        {
            int id = int.Parse(Shared.PopFirstListItem(args));
            int areaid = int.Parse(Shared.PopFirstListItem(args));

            IshipAreaSynch area = r.getArea(areaid);
            ShipInstance sh = r.getShipInstance(id);

            bool errorb = true;
            if (sh != null && area != null)
                errorb = !sh.ChangeArea(area);

            if (errorb == false)
            {
                SpriteInstance.DeserialisePosition(args, sh.spriteInstance);
            }
            else
            {
                Manager.FireLogEvent("error deserialising send ship area", SynchMain.MessagePriority.High,
                                     true);
                return false;
            }
            return true;
        }

        public List<string> SerialiseCreate(bool initial)
        {
            var o = new List<string>();
            //1
            o.Add(shipModel.name);
            //1.5
            o.Add(initial.ToString());
            //2
            if (initial == false)
                o.AddRange(spriteInstance.SerialisePosition());
            //3
            o.AddRange(instanceOwner.SerialiseCreate());
            //4
            if (initial == false)
                o.Add(((SharedID) parentArea).ID.ToString());
            //5
            o.Add(ID.ToString());
            //6
            o.AddRange(SerialiseUpdateStats(false));
            //7
            o.Add(Slots.Count.ToString());
            //8
            foreach (var s in Slots)
            {
                o.Add(s.Key.ToString("d"));
                o.Add(s.Value.name);
            }
            return o;
        }

        /// <summary>
        /// deserialise a ship from a list of variables
        /// </summary>
        /// <param name="args">list of variables serialised previously</param>
        /// <param name="r">region to use to find the parent area/map referenced in the serialised variables</param>
        /// <returns></returns>
        public static ShipInstance DeserialiseCreate(List<string> args, Region r, IshipAreaSynch overload = null)
        {
            //1
            string name = Shared.PopFirstListItem(args);

            //1.5
            bool initial = bool.Parse(Shared.PopFirstListItem(args));

            //2
            VectorMove pos = null;
            float look = 0;
            float currentgravity = 0;
            if (initial == false)
            {
                Tuple<VectorMove, float, float> sprite = SpriteInstance.DeserialisePosition(args);
                pos = sprite.Item1;
                look = sprite.Item2;
                currentgravity = sprite.Item3;
            }
            //3
            InstanceOwner instance = InstanceOwner.DeserialiseCreate(args);
            long pid = -1;
            if (initial == false)
                pid = long.Parse(Shared.PopFirstListItem(args));

            long id = long.Parse(Shared.PopFirstListItem(args));

            if (initial)
                id = -1;

            //create ship
            IshipAreaSynch shiparea;
            if (overload != null || initial)
                shiparea = overload;
            else
                shiparea = r.getArea(pid);

            SetID type;
            if (initial)
                type = SetID.CreateSetNew();
            else
                type = SetID.CreateSetForce(id);

            var si = new ShipInstance(r.parentGCS, loadXML.loadedShips[name], type, shiparea, pos, look, instance);
            si.spriteInstance.currentGravity = currentgravity;
            shiparea.ships.Add(si);
            //6
            DeserialiseUpdateStats(args, r, si);

            //add weapons
            //7
            int slotcount = int.Parse(Shared.PopFirstListItem(args));
            si.EquipSlots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weaponslot>();
            si.Slots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weapon>();

            for (int a = 0; a < slotcount; a++)
            {
                //8
                var wfp = (SlotLocation.SlotLocationEnum) int.Parse(Shared.PopFirstListItem(args));
                string names = Shared.PopFirstListItem(args);
                si.AssignWeapon(wfp, loadXML.loadedWeapons[names]);
            }

            //dont sent to sender
            //si.Init(null);
            //si.SynchInfoMain.SendCreateDelete = false;
            return si;
        }

        public List<string> SerialiseUpdatePosition()
        {
            var o = new List<string>();
            //1
            o.Add(ID.ToString());

            o.AddRange(spriteInstance.SerialisePosition());
            o.Add(CurrentShield.ToString());
            return o;
        }

        public static SynchMain.returncodes DeserialiseUpdatePosition(List<string> args, Region r, int testowner,
                                                                      int testfaction)
        {
            //1
            int id = int.Parse(Shared.PopFirstListItem(args));
            ShipInstance si_up = r.getShipInstance(id);
            if (si_up == null)
                return SynchMain.returncodes.E_NOT_FOUND;

            //make sure client isnt sending updates they shouldnt
            if (testowner != -1 || testfaction != -1)
            {
                if (si_up.instanceOwner.PlayerCanControl(testowner, testfaction, true) == false)
                    return SynchMain.returncodes.E_INSUF_PERM;
            }

            SpriteInstance.DeserialisePosition(args, si_up.spriteInstance);
            si_up.CurrentShield = double.Parse(Shared.PopFirstListItem(args));
            return SynchMain.returncodes.S_OK;
        }

        public List<string> SerialiseUpdateStats(bool addID = true)
        {
            var o = new List<string>();
            //1
            if (addID)
                o.Add(ID.ToString());
            //2
            o.Add(CurrentArmour.ToString());
            //3
            o.Add(disabled.ToString());
            //4
            o.Add(ShowShieldTill.ToString());
            //5
            o.Add(CurrentShield.ToString());
            return o;
        }

        public static void DeserialiseUpdateStats(List<string> args, Region r)
        {
            //1
            int id = int.Parse(Shared.PopFirstListItem(args));
            ShipInstance si_up = r.getShipInstance(id);
            if (si_up == null)
            {
                Manager.FireLogEvent("ship not found:" + id.ToString(), SynchMain.MessagePriority.Low, true);
                return;
            }

            DeserialiseUpdateStats(args, r, si_up);
        }

        public static void DeserialiseUpdateStats(List<string> args, Region r, ShipInstance si)
        {
            //2
            si.CurrentArmour = double.Parse(Shared.PopFirstListItem(args));
            //3
            si.disabled = bool.Parse(Shared.PopFirstListItem(args));
            //4
            si.ShowShieldTill = double.Parse(Shared.PopFirstListItem(args));
            //5
            si.CurrentShield = double.Parse(Shared.PopFirstListItem(args));
        }
    }
}