using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Project.Model.Instances;
using Project.Model.Networking;
using Project.Networking.mapInfoSynch;

namespace Project.Networking
{
    public partial class ConnectedServer :
        ISynchroniseInterfaceSub<ConnectedServer, SynchStatusConnectedServer>
    {
        public static Dictionary<int, SynchStatusConnectedServer> SynchInfo { get; set; }

        [XmlIgnore]
        public static SynchStatusMain SynchInfoMain { get; set; }

        #region ISynchroniseInterfaceSub<ConnectedServer,SynchStatusConnectedServer> Members

        public void Cleanup(ConnectedIPs server)
        {
            SynchInfo.Remove(server.ID);

            if (parentSynchMain.gcs != null && parentSynchMain.gcs.gameRegion != null)
            {
                parentSynchMain.gcs.gameRegion.Cleanup(server);
            }
        }

        public bool Synchronise(ConnectedServer server)
        {
            Init(server);
            SynchStatusConnectedServer SynchInfoL = SynchInfo[server.ID];
            if (SynchInfo == null)
                return false;

            if (SynchInfoL.Created == false)
            {
                RemoteCreate(server, SynchInfoL);
                return true;
            }
            return RemoteUpdate(server, SynchInfoL);
        }

        public void RemoteCreate(ConnectedServer server, SynchStatusConnectedServer synchInfo)
        {
            if (parentSynchMain.gcs.gameConfig.heartbeatonly)
                addHeartbeatToQueue(server, synchInfo);
            else
                addConnectToQueue(server, synchInfo);
        }


        public bool RemoteUpdate(ConnectedServer server, SynchStatusConnectedServer synchInfo)
        {
            bool somecreate = false;
            //update all player ships that are in a map
            if (GameControlClient.playerShipClass != null)
            {
                ShipInstance ps = GameControlClient.playerShipClass.PlayerShip;
                if (ps != null && ps.parentArea is Map)
                {
                    somecreate = somecreate | ps.Synchronise(server);
                }

                List<ShipInstance> ss = GameControlClient.playerShipClass.SupportCraft;
                if (ss != null)
                {
                    foreach (ShipInstance sss in ss)
                    {
                        if (sss.parentArea is Map)
                        {
                            somecreate = somecreate | sss.Synchronise(server);
                        }
                    }
                }
            }

            //update all player shots with server
            if (parentSynchMain.gcs != null && parentSynchMain.gcs.gameRegion != null)
                foreach (
                    WeaponInstance sh in
                        parentSynchMain.gcs.gameRegion.GetPlayerShots(parentSynchMain.gcs.parentSynch.myID))
                {
                    somecreate = somecreate | sh.Synchronise(server);
                }

            return somecreate;
        }

        #endregion

        public void Init(ConnectedServer server = null)
        {
            if (SynchInfo == null)
                SynchInfo = new Dictionary<int, SynchStatusConnectedServer>();
            if (SynchInfoMain == null)
                SynchInfoMain = new SynchStatusMain();

            if (server != null && SynchInfo.ContainsKey(server.ID) == false)
            {
                SynchInfo[server.ID] = new SynchStatusConnectedServer
                                           {
                                               Created = new Wrapper<bool>(!SynchInfoMain.SendCreateDelete),
                                               Deleted = new Wrapper<bool>(!SynchInfoMain.SendCreateDelete)
                                           };
            }
        }

        public void SynchroniseMain(List<ConnectedServer> clients)
        {
            Init();
        }

        private void addConnectToQueue(ConnectedServer server, SynchStatusConnectedServer SynchInfo)
        {
            if (SynchInfo.Created.WaitForResponse)
                return;

            Message m = Message.CreateMessage(Message.Messages.Connect, "tempnickname" + DateTime.Now.ToLongTimeString());
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(m, server);
            server.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.Created, true);
        }


        private void addHeartbeatToQueue(ConnectedServer server, SynchStatusConnectedServer SynchInfo)
        {
            if (SynchInfo.HeartbeatSent.Value)
                return;

            Message mess = Message.CreateMessage(Message.Messages.RequestSectors, "");
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, this);

            SynchInfo.HeartbeatSent.Value = true;
        }

        public void requestGameServerCreation(SectorConfig cfg)
        {
            var param = new List<string>();
            param.Add(Message.sendVars.RequestGameSectorCreation.ToString("d"));
            param.AddRange(cfg.SerialiseCreate());

            //send message
            Message mess = Message.CreateMessage(Message.Messages.SendingVars, param);
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, this);
        }

        public void addControlShipToQueue(ShipInstance newShip, ShipInstance oldShip) //CALLED FROM CODE
        {
            //var server = parentSynchMain.gcs.parentSynch.connectedServer;
            //var SynchInfo = server.SynchInfo.FirstOrDefault();

            var param = new List<string>();
            param.Add(Message.sendVars.ClientControlShip.ToString("d"));
            param.Add(newShip.ID.ToString());
            //old ship?
            if (oldShip != null)
                param.Add(oldShip.ID.ToString());
            else
                param.Add("-1");

            //send message
            Message mess = Message.CreateMessage(Message.Messages.SendingVars, param);
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, this);
            //parentSynchMain.gcs.parentSynch.AddResponseRequirement(server, mess.ID, SynchInfo.Value, SynchInfo.Value.Created, true);
        }

        public void requestShotCreation(Map m, ShipInstance si, weapon w) //called from code
        {
            var o = new List<string>();
            o.Add(Message.sendVars.RequestShotCreate.ToString("d"));
            o.AddRange(WeaponInstance.SerialiseShotCreationRequest(m, si, w));

            Message mess = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, this);
        }

        /*
        public void addShipEjectToQueue(ShipInstance si) //CALLED FROM CODE
        {
            var param = new List<string>();
            param.Add(Message.sendVars.EjectShipFromHangarPost.ToString("d"));
            param.Add(si.ID.ToString());

            //send message
            Message mess = Message.CreateMessage(Message.Messages.SendingVars, param);
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, this);
        }
        */

        public void AddJoinGameToQueue(PlayerShipClass psc, long sectorID) //CALLED FROM CODE
        {
            var param = new List<string>();
            param.Add(sectorID.ToString());
            param.AddRange(psc.SerialiseCreate());

            //send message
            Message mess = Message.CreateMessage(Message.Messages.JoinGame, param);
            parentSynchMain.AddMessageToWriteBuffer(mess, this);
        }
    }
}