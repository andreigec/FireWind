using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Project.Model;
using Project.View.Client.DrawableScreens.WPF_Screens;

namespace Project.Networking
{
    public partial class SynchMain
    {
        public List<Thread> threads = new List<Thread>();

        private void StartClient()
        {
            if (Manager.ConfigSingleThread == false)
            {
                var readthread = new Thread(() => ClientSendUpdateThread(true, false));
                readthread.Start();

                var sendthread = new Thread(() => ClientSendUpdateThread(false, true));
                sendthread.Start();

                threads.Add(readthread);
                threads.Add(sendthread);
            }
            else
            {
                var singlethreadt = new Thread(() => ClientSendUpdateThread(true, true));
                singlethreadt.Start();
                threads.Add(singlethreadt);
            }

            return;
        }

        private void EndClient()
        {
            //end the game
            Manager.EndGame(ref gcs.parentSynch, false);
        }

        public void ClientSendUpdateThread(bool send, bool update)//THREADED
        {
            while (enabled)
            {
                bool ok;
                //update client with values from server
                if (update)
                {
                    ok=ClientUpdateBlock();
                    if (ok == false)
                    {
                        enabled = false;
                        break;
                    }
                }

                //update server with client set values
                if (send)
                    ClientSendBlock();

                Thread.Sleep(Manager.ConfigUpdateMS);
            }
            //Manager.FireLogEvent("end client thread,send=" + send.ToString() + " update=" + update.ToString(), MessagePriority.Information, false);
        }

        private void ClientSendBlock()
        {
            if (connectedServer == null)
                return;

            try
            {
                var cs = new List<ConnectedServer>();
                cs.Add(connectedServer);

                connectedServer.SynchroniseMain(cs);
                connectedServer.Synchronise(connectedServer);
                SendAndFlushWriteBuffer(connectedServer);
            }
            catch (AbandonedMutexException e)
            {
                Manager.FireLogEvent("crash in client send. enabled=" + enabled, MessagePriority.High, true, myID, null);
                Logging.WriteExceptionToFile("client send crash", e);
            }
        }

        private bool ClientUpdateBlock()
        {
            try
            {
                if (isConnected(connectedServer) == false)
                    return false;

                Message m;
                getMessages(connectedServer);

                while (connectedServer != null && connectedServer.messageReadQueue.Count > 0)
                {
                    m = popMessage(connectedServer);

                    if (m != null)
                    {
                        handleComm(m, true, connectedServer);
                    }
                }

                SendAndFlushWriteBuffer(connectedServer);
            }
            catch (AbandonedMutexException e)
            {
                Manager.FireLogEvent("crash in client update. enabled=" + enabled, MessagePriority.High, true, myID, null);
                Logging.WriteExceptionToFile("client update crash", e);
            }
            return true;
        }

        private bool ConnectToIP(GameInitConfig gic)
        {
            try
            {
                var serverEndPoint = new IPEndPoint(IPAddress.Parse(gic.ip), gic.TCPport);
                var server = new ConnectedServer(this);
                var endtime = Manager.GetMillisecondsNow() + ConnectWF.HeartbeatTimeoutMS;
                var success = false;

                double timenow = 0;
                //just setup the connection - TCP
                while (timenow < endtime)
                {
                    timenow = Manager.GetMillisecondsNow();
                    try
                    {
                        server.tcpClient.Connect(serverEndPoint);
                        success = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(Manager.ConfigUpdateMS);
                    }
                }

                if (success == false)
                    return false;

                connectedServer = server;

                //connect to the server with udp, save ip and port for later
                udpClient = InitUDPClientConnect(connectedServer, gic.ip, gic.UDPport);

                if (HandshakeConnections(udpClient, connectedServer) == false)
                    return false;

                //start up
                StartClient();

                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}