using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Project.Model.mapInfo;
using Project.Networking;
using Project.Networking.mapInfoSynch;

namespace Project.Model.Instances
{
    public class WeaponDirty
    {
    }

    public partial class WeaponInstance :
        ISynchroniseInterfaceDrawObject<ConnectedIPs, SynchStatusMoveObjectInstance, SynchStatusMain, WeaponDirty>
    {
        #region ISynchroniseInterfaceDrawObject<ConnectedIPs,SynchStatusMoveObjectInstance,SynchStatusMain,WeaponDirty> Members

        [XmlIgnore]
        public Dictionary<int, SynchStatusMoveObjectInstance> SynchInfo { get; set; }

        [XmlIgnore]
        public SynchStatusMain SynchInfoMain { get; set; }

        [XmlIgnore]
        public Dictionary<int, WeaponDirty> LastServerPlayerUpdate { get; set; }

        public void Init(ConnectedIPs client = null)
        {
            ISynchInterfaceMIXIN.InitObjectType(this, client);
        }

        public void Cleanup(ConnectedIPs client)
        {
            ISynchInterfaceMIXIN.Cleanup(this, client);
        }

        public bool Synchronise(ConnectedIPs client)
        {
            Init(client);
            SynchStatusMoveObjectInstance SynchInfo = this.SynchInfo[client.ID];
            if (SynchInfo == null)
                return false;

            if (SynchInfo.Created == false)
            {
                RemoteCreate(client, SynchInfo);
                return true;
            }

            RemoteUpdate(client, SynchInfo);

            return false;
        }

        public void SynchroniseMain(List<ConnectedIPs> clients)
        {
            Init();
        }


        public void RemoteCreate(ConnectedIPs connection, SynchStatusMoveObjectInstance synchInfo)
        {
            addCreateShotToQueue(connection, synchInfo);
        }

        public void RemoteRemoveObject(ConnectedIPs connection, SynchStatusMoveObjectInstance SynchInfo)
        {
            if (SynchInfo.Deleted.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.ShotRemove.ToString("d"));
            o.Add(ID.ToString());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, connection);
            connection.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Deleted, false);
        }

        public bool RemoteUpdate(ConnectedIPs connection, SynchStatusMoveObjectInstance synchInfo)
        {
            if (synchInfo.UpdateLocation)
            {
                addUpdateShotPositionToQueue(connection);
                synchInfo.UpdateLocation.Value = false;
            }
            return false;
        }

        public bool IsDirty(ConnectedIPs ip)
        {
            return false;
        }

        #endregion

        private void addCreateShotToQueue(ConnectedIPs cc, SynchStatusMoveObjectInstance SynchInfo)
        {
            if (SynchInfo.Created.WaitForResponse)
                return;

            if (parentID == cc.ID && cc is ConnectedClient)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.CreateShot.ToString("d"));
            o.AddRange(SerialiseCreate());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
            SynchInfo.Created.Value = true;
            //cc.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Created, true);
        }

        public void addUpdateShotPositionToQueue(ConnectedIPs cc)
        {
            if (parentID == cc.ID && cc is ConnectedClient)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.UpdateShotPosition.ToString("d"));
            o.AddRange(SerialiseUpdate());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
        }

        /*
        public void ResendToClients(int ExceptPlayerID) //CALLED FROM CODE
        {
            foreach (var v in SynchInfo.Where(s => s.Key != ExceptPlayerID))
            {
                v.Value.UpdateLocation.Value = true;
            }
        }
        */


        public static List<string> SerialiseShotCreationRequest(Map m, ShipInstance si, weapon w)
        {
            var o = new List<string>();

            o.Add(m.ID.ToString());
            o.Add(si.ID.ToString());
            o.Add(w.name);
            return o;
        }

        public static SynchMain.returncodes DeserialiseShotCreationRequest(List<string> args, Region r)
        {
            var m = r.getArea(int.Parse(Shared.PopFirstListItem(args))) as Map;
            if (m == null)
                return SynchMain.returncodes.E_NOT_FOUND;

            ShipInstance ship = r.getShipInstance(int.Parse(Shared.PopFirstListItem(args)));
            if (ship == null)
                return SynchMain.returncodes.E_NOT_FOUND;

            string weaponname = Shared.PopFirstListItem(args);
            if (loadXML.loadedWeapons.ContainsKey(weaponname) == false)
                return SynchMain.returncodes.E_NOT_FOUND;

            weapon w = loadXML.loadedWeapons[weaponname];

            addShot(m, ship, w);
            return SynchMain.returncodes.S_OK;
        }

        public List<string> SerialiseCreate()
        {
            var o = new List<string>();
            o.Add(ID.ToString());
            o.AddRange(spriteInstance.SerialisePosition());
            o.Add(weapon.name);
            o.Add(((SharedID) parentArea).ID.ToString());
            o.Add(parentID.ToString());
            o.Add(nextBeamInstanceID.ToString());
            o.Add(previousBeamInstanceID.ToString());
            return o;
        }

        public static bool DeserialiseCreate(List<string> args, Region r)
        {
            int id = int.Parse(Shared.PopFirstListItem(args));
            Tuple<VectorMove, float, float> vm = SpriteInstance.DeserialisePosition(args);
            string name = Shared.PopFirstListItem(args);
            int mid = int.Parse(Shared.PopFirstListItem(args));
            long parentID = long.Parse(Shared.PopFirstListItem(args));

            long nextid = long.Parse(Shared.PopFirstListItem(args));
            long previd = long.Parse(Shared.PopFirstListItem(args));

            var m = r.getArea(mid) as Map;
            var wi = new WeaponInstance(r.parentGCS, loadXML.loadedWeapons[name], m, vm.Item1, parentID,
                                        SetID.CreateSetForce(id));

            wi.spriteInstance.LookAngle = vm.Item2;
            wi.spriteInstance.currentGravity = vm.Item3;

            m.shots.Add(wi);
            wi.TempInvinShipID = parentID;
            wi.Init();
            wi.SynchInfoMain.SendCreateDelete = false;
            wi.nextBeamInstanceID = nextid;
            wi.previousBeamInstanceID = previd;

            return true;
        }

        public List<string> SerialiseUpdate()
        {
            var o = new List<string>();
            //1
            o.Add(ID.ToString());

            o.AddRange(spriteInstance.SerialisePosition());
            return o;
        }

        public static void DeserialiseUpdate(List<string> args, Region r)
        {
            //1
            int id = int.Parse(Shared.PopFirstListItem(args));
            WeaponInstance wi_up = r.getShotInstance(id);
            if (wi_up == null)
            {
                Manager.FireLogEvent("shot not found:" + id.ToString(), SynchMain.MessagePriority.Low, true);
                return;
            }
            SpriteInstance.DeserialisePosition(args, wi_up.spriteInstance);
        }
    }
}