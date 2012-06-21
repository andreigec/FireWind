using System;
using System.Collections.Generic;
using Project.Model.Instances;
using Project.Networking;
using Project.Networking.mapInfoSynch;

namespace Project.Model.mapInfo
{
    /// <summary>
    /// map,sector,region
    /// </summary>
    public interface IshipAreaSynch : GCSHolder
    {
        List<ShipInstance> ships { get; set; }
        List<BuildingInstance> buildings { get; set; }
        List<WeaponInstance> shots { get; set; }

        List<ShipInstance> RemoveShips { get; set; }
        List<WeaponInstance> RemoveShots { get; set; }
        List<BuildingInstance> RemoveBuildings { get; set; }
    }

    public static class IShipAreaMIXIN
    {
        public static void RemoveObjectsList<TSynchInfo>(IshipAreaSynch isa, List<ConnectedIPs> clients,
                                                         Dictionary<int, TSynchInfo> ShipAreaSynchInfo)
            where TSynchInfo : SynchStatus
        {
            bool send = false;
            try
            {
                if (isa.RemoveShips.Count > 0)
                {
                    RemoteRemoveList(clients,
                                     new List
                                         <
                                         ISynchroniseInterfaceDrawObject
                                         <ConnectedIPs, SynchStatusShip, SynchStatusMain, ShipDirty>>(isa.RemoveShips),
                                     ShipAreaSynchInfo);
                    isa.RemoveShips.Clear();
                    send = true;
                }

                if (isa.RemoveShots.Count > 0)
                {
                    RemoteRemoveList(clients,
                                     new List
                                         <
                                         ISynchroniseInterfaceDrawObject
                                         <ConnectedIPs, SynchStatusMoveObjectInstance, SynchStatusMain, WeaponDirty>>(
                                         isa.RemoveShots), ShipAreaSynchInfo);
                    isa.RemoveShots.Clear();
                    send = true;
                }

                if (isa.RemoveBuildings.Count > 0)
                {
                    RemoteRemoveList(clients,
                                     new List
                                         <
                                         ISynchroniseInterfaceDrawObject
                                         <ConnectedIPs, SynchStatusMoveObjectInstance, SynchStatusMain, DirtyBuilding>>(
                                         isa.RemoveBuildings), ShipAreaSynchInfo);
                    isa.RemoveBuildings.Clear();
                    send = true;
                }
                if (send)
                    isa.parentGCS.parentSynch.SendToAll();
            }
            catch (Exception)
            {
            }
        }

        private static void RemoteRemoveList<TCL, TSynchStatus, TSynchStatusMain, TDCL, TSynchInfo>
            (List<TCL> clients,
             List<ISynchroniseInterfaceDrawObject<TCL, TSynchStatus, TSynchStatusMain, TDCL>> remlist,
             Dictionary<int, TSynchInfo> shipAreaSynchInfo)
            where TSynchStatus : SynchStatus, new()
            where TCL : ConnectedIPs
            where TSynchInfo : SynchStatus
        {
            while (remlist.Count > 0)
            {
                var s = remlist[0];

                foreach (var client in clients)
                {
                    //if the client id doesnt exist any more, it was already cleaned up
                    if (s.SynchInfo == null || s.SynchInfo.ContainsKey(client.ID) == false)
                        s.Init(client);
                    var SynchInfo = s.SynchInfo[client.ID];

                    //make sure the region has been sent to the ship before removing an object
                    if (shipAreaSynchInfo == null || shipAreaSynchInfo.ContainsKey(client.ID) == false ||
                        shipAreaSynchInfo[client.ID].Created == false)
                    {
                        continue;
                    }

                    if (SynchInfo.Created && SynchInfo.Deleted == false && SynchInfo.Deleted.WaitForResponse == false)
                    {
                        s.RemoteRemoveObject(client, SynchInfo);
                    }
                }
                remlist.Remove(s);
            }
        }

        public static void RemoveShipMixIn(IshipAreaSynch isa, ShipInstance si)
        {
            si.parentArea = null;
            isa.ships.Remove(si);
            isa.RemoveShips.Add(si);
        }

        public static void RemoveShotMixIn(IshipAreaSynch isa, WeaponInstance wi)
        {
            wi.parentArea = null;
            isa.shots.Remove(wi);
            isa.RemoveShots.Add(wi);
        }

        public static void RemoveBuildingMixIn(IshipAreaSynch isa, BuildingInstance bi)
        {
            bi.parentArea = null;
            isa.buildings.Remove(bi);
            isa.RemoveBuildings.Add(bi);
        }

        /// <summary>
        /// Init structures in this class
        /// </summary>
        /// <param name="isa"></param>
        public static void InitClass(IshipAreaSynch isa)
        {
            isa.ships = new List<ShipInstance>();
            isa.buildings = new List<BuildingInstance>();
            isa.shots = new List<WeaponInstance>();

            isa.RemoveShips = new List<ShipInstance>();
            isa.RemoveShots = new List<WeaponInstance>();
            isa.RemoveBuildings = new List<BuildingInstance>();
        }

        public static void Cleanup(IshipAreaSynch isa, ConnectedIPs client)
        {
            foreach (var s in isa.ships)
            {
                if (s == null)
                    continue;

                s.Cleanup(client);
            }

            foreach (var b in isa.buildings)
            {
                if (b == null)
                    continue;

                b.Cleanup(client);
            }

            foreach (var w in isa.shots)
            {
                if (w == null)
                    continue;

                w.Cleanup(client);
            }
        }
    }
}