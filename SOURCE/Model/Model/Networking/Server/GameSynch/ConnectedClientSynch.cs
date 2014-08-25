using System.Collections.Generic;
using System.Xml.Serialization;
using Project.Model;
using Project.Model.Networking;
using Project.Model.Networking.Server.GameSynch;
using Project.Networking.mapInfoSynch;

namespace Project.Networking
{
    public partial class ConnectedClient :
        ISynchroniseInterfaceSub<ConnectedClient, SynchStatusConnectedClient>
    {
        public static Dictionary<int, SynchStatusConnectedClient> SynchInfo { get; set; }

        [XmlIgnore]
        public static SynchStatusMain SynchInfoMain { get; set; }

        #region ISynchroniseInterfaceSub<ConnectedClient,SynchStatusConnectedClient> Members

        public void Cleanup(ConnectedIPs client)
        {
            SynchInfo.Remove(client.ID);
        }

        public bool Synchronise(ConnectedClient client)
        {
            Init(client);
            SynchStatusConnectedClient SynchInfof = SynchInfo[client.ID];
            if (SynchInfof == null)
                return false;

            if (heartbeatonly)
                return true;

            if (SynchInfof.GivenID == false)
            {
                addGiveIDToQueue(client, SynchInfof);
                return true;
            }

            bool somecreate = RemoteUpdate(client, SynchInfof);

            //only prompt client to join map if all the objects have been created
            if (SynchInfof.TryForPostACK && somecreate == false)
            {
                addPostActivities(this, SynchInfof.JoinMap, SynchInfof.JoinPSC);

                Manager.FireLogEvent(
                    "client joined game:" + SynchInfof.JoinMap.ID + "-" + SynchInfof.JoinMap.parentSector.Config,
                    SynchMain.MessagePriority.High, false, ID);

                //we dont want to prompt client again
                SynchInfof.JoinMap = null;
                SynchInfof.JoinPSC = null;
                SynchInfof.TryForPostACK.Value = false;
            }

            return somecreate;
        }

        public void RemoteCreate(ConnectedClient client, SynchStatusConnectedClient SynchInfo)
        {
        }

        public bool RemoteUpdate(ConnectedClient client, SynchStatusConnectedClient SynchInfo)
        {
            return parentSynchMain.gcs.gameRegion.Synchronise(client);
        }

        #endregion

        public static void Init(ConnectedClient client = null)
        {
            if (SynchInfo == null)
                SynchInfo = new Dictionary<int, SynchStatusConnectedClient>();
            if (SynchInfoMain == null)
                SynchInfoMain = new SynchStatusMain();

            if (client != null && SynchInfo.ContainsKey(client.ID) == false)
            {
                SynchInfo[client.ID] = new SynchStatusConnectedClient
                                           {
                                               Created = new Wrapper<bool>(!SynchInfoMain.SendCreateDelete),
                                               Deleted = new Wrapper<bool>(!SynchInfoMain.SendCreateDelete)
                                           };
            }
        }

        public static void SynchroniseMain(GameControlServer gcs, List<ConnectedClient> clients)
        {
            Init();
            gcs.gameRegion.SynchroniseMain(clients);
        }

        private void addGiveIDToQueue(ConnectedClient cc, SynchStatusConnectedClient SynchInfo)
        {
            if (SynchInfo.GivenID.WaitForResponse)
                return;

            var o = new List<string>();
            o.Add(Message.sendVars.GiveID.ToString("d"));
            o.Add(cc.ID.ToString());
            o.Add(cc.faction.ToString());

            Message m = Message.CreateMessage(Message.Messages.SendingVars, o);
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(m, cc);
            cc.AddResponseRequirement(m.ID, SynchInfo, SynchInfo.GivenID, true);
        }

        public static void addPostActivities(ConnectedClient cc, Map m, PlayerShipClass psc) //CALLED FROM CODE
        {
            var o = new List<string>();

            o.AddRange(psc.SerialiseCreateLight());

            Message mess = Message.CreateMessage(Message.Messages.JoinGameACK, o);
            cc.parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, cc);
        }

        public static void GiveCredits(ConnectedClient cc, long victorID, ShipInstance dest)
        {
            var o = new List<string>();
            o.Add(Message.sendVars.GiveCredits.ToString("d"));

            o.Add(victorID.ToString());
            o.Add(dest.ID.ToString());

            Message mess = Message.CreateMessage(Message.Messages.SendingVars, o);
            cc.parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, cc);
        }

        public void SendJoinableSectors() //CALLED FROM CODE
        {
            var o = new List<string>();

            //can selectively send sectors the player can join here
            List<Sector> sl = parentSynchMain.gcs.gameRegion.sectors;

            o.Add(sl.Count.ToString());

            foreach (Sector s in sl)
            {
                var h = new Heartbeat(s.ID, s.Config, parentSynchMain.gcs.gameConfig.serverName,
                                      parentSynchMain.gcs.gameConfig.CreateConnectDetails());
                o.AddRange(h.SerialiseCreate());
            }

            Message mess = Message.CreateMessage(Message.Messages.SendSectors, o);
            parentSynchMain.gcs.parentSynch.AddMessageToWriteBuffer(mess, this);

            //synch now
            parentSynchMain.SendAndFlushWriteBuffer(this);
        }
    }
}