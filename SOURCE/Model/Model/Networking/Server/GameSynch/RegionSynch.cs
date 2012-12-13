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
    public partial class region : ISynchroniseInterfaceShipArea<ConnectedClient, SynchStatusArea, SynchStatusMain>
    {
        #region IshipAreaSynch Members

        [XmlIgnore]
        public List<ShipInstance> RemoveShips { get; set; }

        [XmlIgnore]
        public List<WeaponInstance> RemoveShots { get; set; }

        [XmlIgnore]
        public List<BuildingInstance> RemoveBuildings { get; set; }

        #endregion

        #region ISynchroniseInterfaceShipArea<ConnectedClient,SynchStatusArea,SynchStatusMain> Members

        public Dictionary<int, SynchStatusArea> SynchInfo { get; set; }

        [XmlIgnore]
        public SynchStatusMain SynchInfoMain { get; set; }

        public void Init(ConnectedClient client = null)
        {
            ISynchInterfaceMIXIN.Init(this, client);
        }

        public void Cleanup(ConnectedIPs client)
        {
            IShipAreaMIXIN.Cleanup(this, client);
            ISynchInterfaceMIXIN.Cleanup(this, client);

            if (sectors != null)
            {
                foreach (var s in sectors)
                {
                    s.Cleanup(client);
                }
            }
        }

        public void SynchroniseMain(List<ConnectedClient> clients)
        {
            Init();
            IShipAreaMIXIN.RemoveObjectsList(this, new List<ConnectedIPs>(clients), SynchInfo);

            foreach (var s in sectors)
            {
                s.SynchroniseMain(clients);
            }
        }

        public bool Synchronise(ConnectedClient client)
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

            return RemoteUpdate(client, SynchInfo);
        }

        public void RemoteCreate(ConnectedClient client, SynchStatusArea SynchInfo)
        {
                addCreateRegionToQueue(client, SynchInfo);
        }

        public bool RemoteUpdate(ConnectedClient client, SynchStatusArea SynchInfo)
        {
            bool somecreated = false;
            foreach (var s in sectors)
            {
                somecreated = somecreated | s.Synchronise(client);

            }
            return somecreated;
        }

        #endregion

        public void RemoveBuilding(BuildingInstance bi)
        {
            throw new Exception("no handler from removing buildings at sector level");
        }

        public void RemoveShip(ShipInstance s)
        {
            throw new Exception("no handler from removing shots at region level");
            //RemoveShips.Add(s);
            //ships.Remove(s);
        }

        public void RemoveShot(WeaponInstance wi)
        {
            throw new Exception("no handler from removing shots at region level");
        }

        private void addCreateRegionToQueue(ConnectedClient cc, SynchStatusArea SynchInfo)
        {
            if (SynchInfo.Created.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.CreateRegion.ToString("d"));
            o.AddRange(SerialiseCreate());

            var m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
            cc.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Created, true);
        }

        public List<string> SerialiseCreate()
        {
            var o = new List<string>();
            //1
            o.Add(ID.ToString());
            //2
            o.Add(width.ToString());
            //3
            o.Add(height.ToString());
            return o;
        }

        public static bool DeserialiseCreate(List<string> args, GameControlServer gcs)
        {
            var ID = int.Parse(Shared.PopFirstListItem(args));
            var width = int.Parse(Shared.PopFirstListItem(args));
            var height = int.Parse(Shared.PopFirstListItem(args));

            gcs.gameRegion = new region(gcs, SetID.CreateSetForce(ID), width, height);

            gcs.gameRegion.Init();
            gcs.gameRegion.SynchInfoMain.SendCreateDelete = false;
            return true;
        }
    }
}