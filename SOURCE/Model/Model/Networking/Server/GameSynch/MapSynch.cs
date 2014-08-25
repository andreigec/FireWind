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
    public partial class Map : ISynchroniseInterfaceShipArea<ConnectedClient, SynchStatus, SynchStatusMain>
    {
        [XmlIgnore] public List<int> SynchTerrain = new List<int>();

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
        }

        public void SynchroniseMain(List<ConnectedClient> clients)
        {
            Init();
            IShipAreaMIXIN.RemoveObjectsList(this, new List<ConnectedIPs>(clients), SynchInfo);

            if (SynchTerrain.Count > 0)
                RemoteUpdateTerrain(clients, SynchTerrain);
        }

        public bool Synchronise(ConnectedClient client)
        {
            Init(client);
            SynchStatus SynchInfoL = SynchInfo[client.ID];
            if (SynchInfoL == null)
                return false;

            if (SynchInfoL.Created == false)
            {
                RemoteCreate(client, SynchInfoL);
                return true;
            }
            return RemoteUpdate(client, SynchInfoL);
        }

        public void RemoteCreate(ConnectedClient client, SynchStatus SynchInfo)
        {
            SynchInfo.Created.Value = true;
        }

        public bool RemoteUpdate(ConnectedClient client, SynchStatus SynchInfo)
        {
            bool somecreate = false;

            foreach (ShipInstance sh in ships)
            {
                somecreate = somecreate | sh.Synchronise(client);
            }

            foreach (WeaponInstance s in shots)
            {
                somecreate = somecreate | s.Synchronise(client);
            }

            foreach (BuildingInstance b in buildings)
            {
                somecreate = somecreate | b.Synchronise(client);
            }

            return somecreate;
        }

        #endregion

        public void RemoteUpdateTerrain(List<ConnectedClient> conns, List<int> mapindex)
        {
            foreach (ConnectedClient conn in conns)
            {
                if (SynchInfo.ContainsKey(conn.ID) == false || SynchInfo[conn.ID].Created == false)
                    continue;

                var o = new List<string>();
                o.Add(Message.sendVars.UpdateTerrain.ToString("d"));
                o.AddRange(SerialiseUpdateTerrain(mapindex, this));

                Message mess = Message.CreateMessage(Message.Messages.SendingVars, o);
                parentGCS.parentSynch.AddMessageToWriteBuffer(mess, conn);
            }
            mapindex.Clear();
        }

        /*
        public void UpdateVisibility(ConnectedClient cip) //CALLED FROM CODE
        {
            var cansee =
                parentSector.parentID == cip.ID ||
                ships.Any(s => s.instanceOwner.PlayerOwnerID == cip.ID || s.instanceOwner.FactionOwner == cip.faction) ||
                buildings.Any(
                    s => s.instanceOwner.PlayerOwnerID == cip.ID || s.instanceOwner.FactionOwner == cip.faction);

            if (SynchInfo == null || SynchInfo.ContainsKey(cip.ID) == false)
                Init(cip);

            SynchInfo[cip.ID].CanSee.Value = cansee;
        }
        */

        public List<string> SerialiseUpdateTerrain(List<int> mapindex, Map m)
        {
            lock (m)
            {
                var o = new List<string>();

                //1
                o.Add(m.ID.ToString());
                //2
                o.Add(mapindex.Count.ToString());
                foreach (int m1 in mapindex)
                {
                    List<Tuple<int, int>> t1 = m.terrain.heightmap.heights[m1];
                    //3
                    o.Add(m1.ToString());
                    //4
                    o.Add(t1.Count.ToString());

                    foreach (var t in t1)
                    {
                        o.Add(t.Item1.ToString());
                        o.Add(t.Item2.ToString());
                    }
                }
                return o;
            }
        }

        public static SynchMain.returncodes DeserialiseUpdateTerrain(List<string> args, Region r)
        {
            //1
            var m = r.getArea(long.Parse(Shared.PopFirstListItem(args))) as Map;
            if (m == null)
                return SynchMain.returncodes.E_NOT_FOUND;

            //2
            int indexcount = int.Parse(Shared.PopFirstListItem(args));

            for (int a = 0; a < indexcount; a++)
            {
                //3
                int index = int.Parse(Shared.PopFirstListItem(args));
                //add to dirty list for post updates
                m.terrain.dirty.Add(index);

                List<Tuple<int, int>> t1 = m.terrain.heightmap.heights[index];
                t1.Clear();
                //4
                int count = int.Parse(Shared.PopFirstListItem(args));
                for (int b = 0; b < count; b++)
                {
                    int i1 = int.Parse(Shared.PopFirstListItem(args));
                    int i2 = int.Parse(Shared.PopFirstListItem(args));
                    t1.Add(new Tuple<int, int>(i1, i2));
                }
            }

            return SynchMain.returncodes.S_OK;
        }
    }
}