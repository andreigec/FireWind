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
    public partial class sector : ISynchroniseInterfaceShipArea<ConnectedClient, SynchStatus, SynchStatusMain>
    {
        #region IshipAreaSynch Members

        [XmlIgnore]
        public List<ShipInstance> RemoveShips { get; set; }

        [XmlIgnore]
        public List<WeaponInstance> RemoveShots { get; set; }

        [XmlIgnore]
        public List<BuildingInstance> RemoveBuildings { get; set; }

        #endregion

        #region ISynchroniseInterfaceShipArea<ConnectedClient,SynchStatus,SynchStatusMain> Members

        [XmlIgnore]
        public Dictionary<int, SynchStatus> SynchInfo { get; set; }

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

            if (thismap != null)
                thismap.Cleanup(client);
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

        public void SynchroniseMain(List<ConnectedClient> clients)
        {
            Init();
            IShipAreaMIXIN.RemoveObjectsList(this, new List<ConnectedIPs>(clients), SynchInfo);

            thismap.SynchroniseMain(clients);
        }


        public void RemoteCreate(ConnectedClient client, SynchStatus synchInfo)
        {
            addCreateSectorToQueue(client, synchInfo);
        }

        public bool RemoteUpdate(ConnectedClient client, SynchStatus synchInfo)
        {
            bool somecreate = false;
            foreach (var sh in ships)
            {
                somecreate=somecreate| sh.Synchronise(client);
            }
            somecreate=somecreate|thismap.Synchronise(client);
            
            return somecreate;
        }

        #endregion

        private void addCreateSectorToQueue(ConnectedClient cc, SynchStatus SynchInfo)//called from code
        {
            if (SynchInfo.Created.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.CreateSectorMap.ToString("d"));
            o.AddRange(SerialiseCreate(false));

            var m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentGCS.parentSynch.AddMessageToWriteBuffer(m, cc);
            cc.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Created, true);
        }

        public List<string> SerialiseCreate(bool includeBuildingsAndShips)
        {
            var ret = new List<String>();

            ret.Add(thismap.width.ToString());
            ret.Add(thismap.height.ToString());
            ret.Add(thismap.terrainSeed.ToString());
            ret.Add(ID.ToString());
            ret.Add(thismap.ID.ToString());
            ret.AddRange(Config.SerialiseCreate());
            //send terrain
            ret.AddRange(thismap.SerialiseUpdateTerrain(thismap.terrain.changed, thismap));
            //number of ships
            var numships = 0;
            if (includeBuildingsAndShips)
                numships = ships.Count;
            ret.Add(numships.ToString());
            if (includeBuildingsAndShips)
            {
                foreach (var sh in ships)
                {
                    ret.AddRange(sh.SerialiseCreate(false));
                }
            }

            //number of buildings
            var numbuild = 0;
            if (includeBuildingsAndShips)
                numbuild = thismap.buildings.Count;
            ret.Add(numbuild.ToString());
            if (includeBuildingsAndShips)
            {
                foreach (var bl in thismap.buildings)
                {
                    ret.AddRange(bl.SerialiseCreate());
                }
            }

            return ret;
        }

        public static sector DeserialiseCreate(List<string> args, GameControlServer gcs, ConnectedIPs client,
                                               bool isHomePlanet,
                                               int faction = -1)
        {
            var width = int.Parse(Shared.PopFirstListItem(args));
            var height = int.Parse(Shared.PopFirstListItem(args));
            var seed = int.Parse(Shared.PopFirstListItem(args));
            var sid = int.Parse(Shared.PopFirstListItem(args));
            var mid = int.Parse(Shared.PopFirstListItem(args));
            var cfg = SectorConfig.DeserialiseCreate(args);


            //strip ids if homeplanet from client, we want to give server ids back
            if (isHomePlanet)
            {
                sid = mid = -1;
            }

            var sec = CreateSector(gcs,width, height, cfg, seed, SetID.CreateSetForce(sid), SetID.CreateSetForce(mid),
                                      gcs.gameRegion);
            gcs.gameRegion.addSector(sec);

            //update terrain
            map.DeserialiseUpdateTerrain(args, gcs.gameRegion);

            //ships
            var sc = int.Parse(Shared.PopFirstListItem(args));
            while (sc > 0 && isHomePlanet)
            {
                ShipInstance.DeserialiseCreate(args, gcs.gameRegion, sec);
                sc--;
            }

            //buildings
            var bc = int.Parse(Shared.PopFirstListItem(args));
            while (bc > 0 && isHomePlanet)
            {
                BuildingInstance.DeserialiseCreate(args, gcs.gameRegion, true, sec.thismap);
                bc--;
            }

            sec.SetInstanceOwnerIDs(client.ID, faction);

            //dont send again to sender(usually server) if non home planet
            if (isHomePlanet == false)
            {
                sec.Init(null);
                sec.SynchInfoMain.SendCreateDelete = false;
            }

            return sec;
        }
    }
}