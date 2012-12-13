using System;
using System.Collections.Generic;
using Project.Model.Networking;
using Project.Networking.mapInfoSynch;

namespace Project.Networking
{
    /// <summary>
    /// Connected server/client
    /// </summary>
    /// <typeparam name="TIPType"></typeparam>
    /// <typeparam name="TSynchStatus"></typeparam>
    public interface ISynchroniseInterfaceSub<TIPType, TSynchStatus>
    {
        /// <summary>
        /// used by server to keep clients in sync
        /// </summary>
        /// <param name="connection"></param>
        /// <returns>bool-true if something has been created in this synch run</returns>
        bool Synchronise(TIPType connection);

        /// <summary>
        /// Create and queue all required messages to create the object remotely
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="synchInfo"></param>
        void RemoteCreate(TIPType connection, TSynchStatus synchInfo);

        /// <summary>
        /// create and queue all required messages to synch the remote object
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="synchInfo"></param>
        bool RemoteUpdate(TIPType connection, TSynchStatus synchInfo);

        /// <summary>
        /// Remove all references to the connection - has to be ConnectedIPs because we init servers and client with this
        /// </summary>
        /// <param name="connection"></param>
        void Cleanup(ConnectedIPs connection);
    }

    /// <summary>
    /// region map sector
    /// </summary>
    /// <typeparam name="TIPType"></typeparam>
    /// <typeparam name="TSynchStatus"></typeparam>
    /// <typeparam name="TSynchStatusMain"></typeparam>
    public interface ISynchroniseInterfaceShipArea<TIPType, TSynchStatus, TSynchStatusMain> :
        ISynchroniseInterfaceSub<TIPType, TSynchStatus>
    {
        /// <summary>
        /// contains the basic synch status
        /// </summary>
        Dictionary<int, TSynchStatus> SynchInfo { get; set; }

        /// <summary>
        /// contains instance wide info
        /// </summary>
        TSynchStatusMain SynchInfoMain { get; set; }

        /// <summary>
        /// used by server to keep clients in sync
        /// </summary>
        /// <param name="clients"></param>
        void SynchroniseMain(List<TIPType> clients);

        /// <summary>
        /// Make sure that the synchinfo array contains the connection
        /// </summary>
        /// <param name="connection"></param>
        void Init(TIPType connection);
    }

    /// <summary>
    /// ship/building/weapon instance
    /// </summary>
    /// <typeparam name="TIPType"></typeparam>
    /// <typeparam name="TSynchStatus"></typeparam>
    /// <typeparam name="TSynchStatusMain"></typeparam>
    /// <typeparam name="TDirtyClass"></typeparam>
    public interface ISynchroniseInterfaceDrawObject<TIPType, TSynchStatus, TSynchStatusMain, TDirtyClass> :
        ISynchroniseInterfaceShipArea<TIPType, TSynchStatus, TSynchStatusMain>
    {
        Dictionary<int, TDirtyClass> LastServerPlayerUpdate { get; set; }
        void RemoteRemoveObject(TIPType connection, TSynchStatus synchInfo);
        bool IsDirty(TIPType connection);
    }


    public static class ISynchInterfaceMIXIN
    {
        public static void InitClass<TCL, TSynchStatus, TSynchStatusMain>(
            ISynchroniseInterfaceShipArea<TCL, TSynchStatus, TSynchStatusMain> isi)
            where TSynchStatusMain : SynchStatusMain, new()
        {
            if (isi.SynchInfo == null)
                isi.SynchInfo = new Dictionary<int, TSynchStatus>();

            if (isi.SynchInfoMain == null)
                isi.SynchInfoMain = new TSynchStatusMain();
        }

        public static void Init<TCL, TSynchStatus, TSynchStatusMain>(
            ISynchroniseInterfaceShipArea<TCL, TSynchStatus, TSynchStatusMain> isi, TCL client = null)
            where TSynchStatus : SynchStatus, new()
            where TCL : ConnectedIPs
            where TSynchStatusMain : SynchStatusMain, new()
        {
            if (client != null && isi.SynchInfo.ContainsKey(client.ID) == false)
            {
                var ok = false;
                while (ok == false)
                {
                    try
                    {
                        isi.SynchInfo[client.ID] = new TSynchStatus
                                                       {
                                                           Created =
                                                               new Wrapper<bool>(!isi.SynchInfoMain.SendCreateDelete),
                                                           Deleted =
                                                               new Wrapper<bool>(!isi.SynchInfoMain.SendCreateDelete)
                                                       };
                        ok = true;
                    }
                    catch (Exception)
                    {
                        InitClass(isi);
                    }
                }
            }
        }

        public static void InitObjectType<TCL, TSynchStatus, TSynchStatusMain, TDCL>(
            ISynchroniseInterfaceDrawObject<TCL, TSynchStatus, TSynchStatusMain, TDCL> isi, TCL client = null)
            where TSynchStatus : SynchStatus, new()
            where TCL : ConnectedIPs
            where TSynchStatusMain : SynchStatusMain, new()
            where TDCL : new()
        {
            Init(isi, client);

            if (isi.LastServerPlayerUpdate == null)
                isi.LastServerPlayerUpdate = new Dictionary<int, TDCL>();

            if (client != null && isi.LastServerPlayerUpdate.ContainsKey(client.ID) == false)
            {
                var ok = false;
                while (ok == false)
                {
                    try
                    {
                        isi.LastServerPlayerUpdate[client.ID] = new TDCL();
                        ok = true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public static void Cleanup<TCL, TSynchStatus, TSynchStatusMain>(
            ISynchroniseInterfaceShipArea<TCL, TSynchStatus, TSynchStatusMain> isi, ConnectedIPs client)
        {
            isi.SynchInfo.Remove(client.ID);
        }
    }
}