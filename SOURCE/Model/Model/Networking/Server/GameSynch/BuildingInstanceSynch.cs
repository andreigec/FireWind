using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Project.Model.mapInfo;
using Project.Networking;
using Project.Networking.mapInfoSynch;

namespace Project.Model.Instances
{
    public class DirtyBuilding
    {
        public double LastArmour = -1;
    }

    public partial class BuildingInstance :
        ISynchroniseInterfaceDrawObject<ConnectedIPs, SynchStatusMoveObjectInstance, SynchStatusMain, DirtyBuilding>
    {
        #region ISynchroniseInterfaceDrawObject<ConnectedIPs,SynchStatusMoveObjectInstance,SynchStatusMain,DirtyBuilding> Members

        [XmlIgnore]
        public Dictionary<int, SynchStatusMoveObjectInstance> SynchInfo { get; set; }

        [XmlIgnore]
        public SynchStatusMain SynchInfoMain { get; set; }

        [XmlIgnore]
        public Dictionary<int, DirtyBuilding> LastServerPlayerUpdate { get; set; }

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
            var SynchInfo = this.SynchInfo[client.ID];
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


        public void RemoteCreate(ConnectedIPs connection, SynchStatusMoveObjectInstance SynchInfo)
        {
            addCreateBuildingToQueue(connection, SynchInfo);
        }

        public void RemoteRemoveObject(ConnectedIPs connection, SynchStatusMoveObjectInstance SynchInfo)
        {
            if (SynchInfo.Deleted.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.BuildingRemove.ToString("d"));
            o.Add(ID.ToString());

            var m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, connection);
            connection.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Deleted, false);
        }

        public bool IsDirty(ConnectedIPs connection)
        {
            var sd = LastServerPlayerUpdate[connection.ID];
            if (sd == null)
                return true;

            var armourdirty = armour != sd.LastArmour;

            if (armourdirty)
            {
                sd.LastArmour = armour;
                return true;
            }
            return false;
        }

        public bool RemoteUpdate(ConnectedIPs connection, SynchStatusMoveObjectInstance SynchInfo)
        {
            //check if we need to update this client
            if (IsDirty(connection))
            {
                addUpdateToQueue(connection);
            }
            return false;
        }

        #endregion

        private void addCreateBuildingToQueue(ConnectedIPs cc, SynchStatusMoveObjectInstance SynchInfo)
        {
            if (SynchInfo.Created.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.CreateBuilding.ToString("d"));
            o.AddRange(SerialiseCreate());

            var m = Message.CreateMessage(Message.Messages.SendingVars, o);
            
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
            cc.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Created, true);
        }

        public void addUpdateToQueue(ConnectedIPs cc)
        {
            if (cc is ConnectedServer)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.UpdateBuilding.ToString("d"));
            o.AddRange(SerialiseUpdate());

            var m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
        }

        public List<string> SerialiseCreate()
        {
            var o = new List<string>();
            //1
            o.Add(buildingModel.name);
            //2
            o.AddRange(spriteInstance.SerialisePosition());
            //3
            o.AddRange(instanceOwner.SerialiseCreate());
            //4
            o.Add(((SharedID) parentArea).ID.ToString());
            //5
            o.Add(ID.ToString());
            //6
            o.Add(buildingType.ToString("d"));
            return o;
        }

        /// <summary>
        /// deserialise a building from a list of variables
        /// </summary>
        /// <param name="args">list of variables serialised previously</param>
        /// <param name="r">region to use to find the parent area/map referenced in the serialised variables</param>
        /// <returns></returns>
        public static BuildingInstance DeserialiseCreate(List<string> args, region r, bool stripID = false,
                                                         map overload = null)
        {
            var name = Shared.PopFirstListItem(args);
            var sprite = SpriteInstance.DeserialisePosition(args);
            var instance = InstanceOwner.DeserialiseCreate(args);
            var pid = long.Parse(Shared.PopFirstListItem(args));
            var id = long.Parse(Shared.PopFirstListItem(args));
            var bid = (BuildingType) int.Parse(Shared.PopFirstListItem(args));
            if (stripID)
                id = -1;

            //create building
            map a;
            if (overload != null)
                a = overload;
            else
                a = r.getArea(pid) as map;
            SetID type;
            if (stripID)
                type = SetID.CreateSetNew();
            else
                type = SetID.CreateSetForce(id);

            var bi = new BuildingInstance(r.parentGCS, loadXML.loadedBuildings[name], a, sprite.Item1, sprite.Item2, bid, type,
                                          instance);
            bi.spriteInstance.currentGravity = sprite.Item3;
            a.buildings.Add(bi);

            //dont sent to sender
            //si.Init(null);
            //si.SynchInfoMain.SendCreateDelete = false;
            return bi;
        }

        public List<string> SerialiseUpdate()
        {
            var o = new List<string>();
            //1
            o.Add(ID.ToString());

            o.AddRange(spriteInstance.SerialisePosition());
            //2
            o.Add(armour.ToString());
            return o;
        }

        public static void DeserialiseUpdate(List<string> args, region r)
        {
            //1
            var id = int.Parse(Shared.PopFirstListItem(args));
            var bi_up = r.getBuildingInstance(id);
            if (bi_up == null)
            {
                Manager.FireLogEvent("building not found:" + id.ToString(), SynchMain.MessagePriority.Low, true);
                return;
            }

            SpriteInstance.DeserialisePosition(args, bi_up.spriteInstance);

            //2
            bi_up.armour = double.Parse(Shared.PopFirstListItem(args));
            bi_up.updateDestructAnimation();
        }
    }
}