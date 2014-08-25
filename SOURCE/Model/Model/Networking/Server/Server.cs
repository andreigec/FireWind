using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Project.Model;

namespace Project.Networking
{
    public partial class SynchMain
    {
        //use this to listen for all clients via TCP
        public static TcpListener tcpListener;

        private bool StartServer(GameInitConfig gic)
        {
            //try and stop if already running
            if (tcpListener != null)
            {
                tcpListener.Stop();
                tcpListener = null;
            }
            //to listen to new clients
            tcpListener = new TcpListener(IPAddress.Any, gic.TCPport);
            tcpListener.Start();

            if (Manager.ConfigSingleThread == false)
            {
                var connectionsLoop = new Thread(ConnectionsLoop); //accept new clients
                connectionsLoop.Start();

                var mainthread = new Thread(ServerUpdateThread); //update the game
                mainthread.Start();

                var updatethread = new Thread(synchroniseClientsLoop); //handle sending/receiving messages from clients
                updatethread.Start();

                threads.Add(connectionsLoop);
                threads.Add(mainthread);
                threads.Add(updatethread);
            }

            else
            {
                var singlethreadt = new Thread(SingleThread); //handle sending/receiving messages from clients
                singlethreadt.Start();
                threads.Add(singlethreadt);
            }

            return true;
        }

        public void SingleThread()
        {
            DateTime orig = DateTime.Now;
            DateTime old = DateTime.Now;

            while (enabled)
            {
                //update server internally
                DateTime newTime = DateTime.Now;
                //var ts = two - orig;

                var gt = new GameTime(newTime - orig, newTime - old, false);
                old = DateTime.Now;

                //listen for new clients, add if any
                ConnectionsBlock();
                //update the server world
                ServerUpdateBlock(gt);
                //synch with clients
                SynchroniseClientsBlock();
                Thread.Sleep(Manager.ConfigDisplayMS);
            }
        }

        /// <summary>
        /// server calls this either single/multi thread, game calls this manually
        /// </summary>
        /// <param name="gt"></param>
        public void ServerUpdateBlock(GameTime gt)
        {
            try
            {
                if (gcs != null)
                {
                    gcs.Update(gt);
                }
            }
            catch (AbandonedMutexException ex)
            {
                Manager.FireLogEvent("crash in update thread", MessagePriority.High, true, myID, null);
                Logging.WriteExceptionToFile("update thread crash", ex);
            }
        }


        public void ServerUpdateThread() //THREADED BY StartServer
        {
            DateTime orig = DateTime.Now;
            DateTime old = DateTime.Now;

            while (enabled)
            {
                //update server internally
                DateTime newTime = DateTime.Now;
                //var ts = two - orig;

                var gt = new GameTime(newTime - orig, newTime - old, false);
                old = DateTime.Now;

                ServerUpdateBlock(gt);
                Thread.Sleep(Manager.ConfigUpdateMS);
            }
        }

        public void SendToAll()
        {
            foreach (ConnectedClient c in connectedClients)
            {
                SendAndFlushWriteBuffer(c);
            }
        }

        private void SynchroniseClientsBlock()
        {
            if (gcs == null || gcs.gameRegion == null)
                return;

            //handle shiparea wide synchronisations, like removal
            ConnectedClient.SynchroniseMain(gcs, connectedClients);

            for (int a = 0; a < connectedClients.Count; a++)
            {
                ConnectedClient c = connectedClients[a];
                if (isConnected(c) == false || c.ForceDisconnect)
                {
                    Manager.FireLogEvent("disconnect of client" + c.ID, MessagePriority.High, false, myID);
                    DisconnectClient(c, false);
                    continue;
                }

                try
                {
                    //get messages from the client
                    getMessages(c);

                    //read all and handle messages
                    while (c.messageReadQueue.Count > 0)
                    {
                        Message m = popMessage(c);
                        handleComm(m, false, c);
                    }

                    SendAndFlushWriteBuffer(c);

                    //update client
                    c.Synchronise(c);
                    SendAndFlushWriteBuffer(c);
                }
                catch (Exception ex)
                {
                    Manager.FireLogEvent("crash in client update thread", MessagePriority.High, true, myID, null);
                    Logging.WriteExceptionToFile("update client update crash", ex);
                }
            }
        }

        public void DisconnectClient(ConnectedClient client, bool muteEx)
        {
            if (muteEx)
            {
                string timeout = "Client timeout:";
                Manager.FireLogEvent(timeout + client.alias, MessagePriority.High, false, client.ID);
            }

            //run one last synch main to tell player to remove objects
            ConnectedClient.SynchroniseMain(gcs, connectedClients);
            SendAndFlushWriteBuffer(client);

            //cleanup the game/cache
            gcs.gameRegion.Cleanup(client);
            gcs.removePlayer(client.ID);
            client.Cleanup(client);

            //close connections and actually remove client
            client.tcpClient.Close();
            connectedClients.Remove(client);
            client.ForceDisconnect = true;
        }

        public void synchroniseClientsLoop() //THREADED BY StartServer
        {
            while (enabled)
            {
                SynchroniseClientsBlock();
                Thread.Sleep(Manager.ConfigUpdateMS);
            }
        }

        public void ConnectionsLoop() //THREADED BY StartServer
        {
            tcpListener.Start();
            while (enabled)
            {
                ConnectionsBlock(); //new players
                Thread.Sleep(Manager.ConfigConnectionListenMS);
            }
        }

        public void ConnectionsBlock()
        {
            try
            {
                //blocks until a client has connected to the server
                if (tcpListener.Pending())
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    client.SendTimeout = 5;

                    var cc = new ConnectedClient(this, "temp", client, null);
                    cc.UDPSendHere = new IPEndPoint(IPAddress.Any, 0);

                    if (HandshakeConnections(udpClient, cc) == false)
                    {
                        client.Close();
                        return;
                    }

                    connectedClients.Add(cc);
                    Manager.FireLogEvent("Opened connection to client:ID" + cc.ID, MessagePriority.High, false, cc.ID,
                                         null);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteExceptionToFile("crash in connection block", ex);
            }
        }
    }
}