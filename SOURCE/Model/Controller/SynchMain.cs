﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        //message stuff

        #region MessagePriority enum

        public enum MessagePriority
        {
            Information,
            Low,
            High
        }

        #endregion

        public const char separator = '|';
        public static string startMessage = separator + ">" + separator;
        public static string endMessage = separator + "<" + separator;
        public List<ConnectedClient> connectedClients = new List<ConnectedClient>();
        public ConnectedServer connectedServer;

        /// <summary>
        /// aborts threads if set to false
        /// </summary>
        public bool enabled = true;

        public int myFaction = -102;
        public int myID = -100;

        //udpclient for client and server
        public UdpClient udpClient;

        public GameControlServer gcs { get; private set; }

        public bool Init(GameInitConfig cfg)
        {
            //create the game
            gcs = GameControlServer.CreateGameControlServer(this, cfg);

            //start udp/tcp and threads
            bool success = InitialiseConnections(cfg);
            if (success == false)
                return false;
            return true;
        }

        private bool InitialiseConnections(GameInitConfig gic)
        {
            //if the player is playing single player, dont need update thread
            if (gic.isServer())
            {
                if (gic.hostExternally() == false)
                    return true;

                //set up the base udp connection
                udpClient = InitUDPClient(gic.UDPport);

                //if the player is hosting externally, need to start the server to listen and respond to clients
                bool success = StartServer(gic);
                return success;
            }
            //otherwise the player is connecting to a server
            else
            {
                bool success = ConnectToIP(gic);
                return success;
            }
        }

        public static UdpClient InitUDPClientConnect(ConnectedIPs client, String ip, int udpPort)
        {
            UdpClient udpClient = InitUDPClient(udpPort, ip);

            var ipend = new IPEndPoint(IPAddress.Parse(ip), udpPort);
            client.UDPSendHere = ipend;

            return udpClient;
        }

        public static UdpClient InitUDPClient(int udpPort, String ip = null)
        {
            UdpClient udpClient = null;
            if (ip != null)
                udpClient = new UdpClient(ip, udpPort);
            else
                udpClient = new UdpClient(udpPort);

            SetUDPTimeout(udpClient, ConnectWF.HeartbeatTimeoutMS);
            return udpClient;
        }

        private static void SetUDPTimeout(UdpClient uc, int timeout)
        {
            uc.Client.ReceiveTimeout = timeout;
            uc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
        }

        public void AddMessageToWriteBuffer(Message message, ConnectedIPs client)
        {
            lock (client)
            {
                client.messageWriteQueue.Add(message);
            }
        }

        public void GetUDPMessages()
        {
            try
            {
                if (udpClient == null || udpClient.Available == 0)
                    return;

                //we need to get all messages, and find where they originated from, then add to the relevent message queue
                var _client = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref _client);
                string str = Encoding.ASCII.GetString(data, 0, data.Length);

                //determine origin
                ConnectedIPs client = null;
                if (connectedClients != null)
                {
                    foreach (ConnectedClient c in connectedClients)
                    {
                        if (c.UDPSendHere.Equals(_client))
                        {
                            client = c;
                            break;
                        }
                    }
                }

                if (connectedServer != null && connectedServer.UDPSendHere.Equals(_client))
                    client = connectedServer;

                if (client == null)
                {
                    Manager.FireLogEvent("UDP Message received from unknown source", MessagePriority.Low, true, myID);
                    return;
                }

                //add data to queue
                client.messageReadQueueRaw += str;
            }
            catch (AbandonedMutexException ex)
            {
                Manager.FireLogEvent("Error in UDP get messages", MessagePriority.High, true, myID);
                Logging.WriteExceptionToFile("error in udp get messages", ex);
            }
        }

        public void SendAndFlushWriteBuffer(ConnectedIPs client)
        {
            if (client == null)
                return;

            lock (client)
            {
                try
                {
                    //nothing in buffer
                    if (client.messageWriteQueue.Count == 0)
                        return;

                    //convert queued messages to string
                    string bufftcp = "";
                    string buffudp = "";

                    foreach (Message m in client.messageWriteQueue)
                    {
                        if (m.messageType == Message.Messages.SendingVars && m.IsUDPBoundMessage())
                            buffudp += m;
                        else
                            bufftcp += m;

                        //log
                        Manager.FireLogEvent("Sent Message", MessagePriority.Information, false, client.ID, m);
                    }

                    //TODO:fallback to tcp if udp not available?
                    if (String.IsNullOrEmpty(bufftcp) == false)
                    {
                        SendTCPString(bufftcp, client);
                    }

                    if (String.IsNullOrEmpty(buffudp) == false)
                    {
                        SendUDPString(buffudp, udpClient, client);
                    }

                    client.messageWriteQueue.Clear();
                }
                catch (AbandonedMutexException ex)
                {
                    Manager.FireLogEvent("crash in synch thread", MessagePriority.Low, true);
                    Logging.WriteExceptionToFile("crash in synch thread", ex);
                }
            }
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] EncryptCompress(string tosend)
        {
            return Encoding.ASCII.GetBytes(tosend);

            var bytes = Encoding.UTF8.GetBytes(tosend);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }
                var ret = mso.ToArray();
                return ret;
            }
        }

        public static string DecryptExpand(byte[] received, int toindex = -1)
        {
            
            if (toindex==-1)
                return Encoding.ASCII.GetString(received);

            return Encoding.ASCII.GetString(received, 0, toindex);
            
            MemoryStream msi;
            if (toindex == -1)
                msi = new MemoryStream(received);
            else
                msi = new MemoryStream(received, 0, toindex);

            var mso = new MemoryStream();

            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                //gs.CopyTo(mso);
                CopyTo(gs, mso);
            }

            var ret = Encoding.UTF8.GetString(mso.ToArray());
            return ret;
        }


        public static string ReadTCPString(TcpClient tcl)
        {
            var message = new byte[4096];
            int bytesRead;
            NetworkStream ns = tcl.GetStream();

            try
            {
                //blocks until a client sends a message
                bytesRead = ns.Read(message, 0, 4096);
            }
            catch
            {
                //a socket error has occured
                return "";
            }

            if (bytesRead == 0)
            {
                //the client has disconnected from the server
                return "";
            }

            //message has successfully been received
            return DecryptExpand(message, bytesRead);
        }

        public static void SendTCPString(string info, ConnectedIPs client)
        {
            var tosend = EncryptCompress(info);

            TcpClient tcl = client.tcpClient;
            //send queued messages - TCP
            NetworkStream stream = tcl.GetStream();
            stream.Write(tosend, 0, tosend.Length);
            //clear write queue
            stream.Flush();
        }

        public static String ReadUDPString(UdpClient uc, ConnectedIPs client, int? TimeoutMS = 500)
        {
            if (TimeoutMS != null)
            {
                SetUDPTimeout(uc, (int)TimeoutMS);
            }
            else if (uc.Client.ReceiveTimeout <= 0)
            {
                SetUDPTimeout(uc, ConnectWF.HeartbeatTimeoutMS);
            }

            byte[] ret = null;
            try
            {
                ret = uc.Receive(ref client.UDPSendHere);
            }
            catch (Exception ex)
            {
                return null;
            }

            return DecryptExpand(ret);
        }

        public static void SendUDPString(String text, UdpClient uc, ConnectedIPs client)
        {
            var tosend = EncryptCompress(text);

            //if we are sending to the server, just send without an endpoint
            if (client == null || client is ConnectedServer)
            {
                uc.Send(tosend, tosend.Length);
            }
            else
            {
                uc.Send(tosend, tosend.Length, client.UDPSendHere);
            }
        }

        #region connections

        public void StopServer()
        {
            enabled = false;
            gcs = null;

            //stop udp
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }

            //stop tcp
            foreach (ConnectedClient c in connectedClients)
            {
                c.tcpClient.Close();
            }
            if (connectedServer != null)
            {
                connectedServer.tcpClient.Close();
                connectedServer = null;
            }

            if (tcpListener != null)
            {
                tcpListener.Stop();
                tcpListener = null;
            }
        }

        public bool isConnected(ConnectedIPs client)
        {
            bool connected = true;
            if (client.tcpClient.Client.Poll(0, SelectMode.SelectRead))
            {
                if (!client.tcpClient.Connected) connected = false;
                else
                {
                    var b = new byte[1];
                    try
                    {
                        if (client.tcpClient.Client.Receive(b, SocketFlags.Peek) == 0)
                            connected = false;
                    }
                    catch
                    {
                        connected = false;
                    }
                }
            }
            return connected;
        }

        /// <summary>
        /// Get all the text in the TCP/UDP buffers, and convert to the message format
        /// </summary>
        /// <param name="clientStream"></param>
        private void getMessages(ConnectedIPs clientStream)
        {
            //add UDP messages
            GetUDPMessages();

            //add TCP messages        
            string str = "";
            while (clientStream.tcpClient.Available > 0)
                str += ReadTCPString(clientStream.tcpClient);

            if (String.IsNullOrEmpty(str) == false)
            {
                clientStream.messageReadQueueRaw += str;
            }

            ConvertBufferToMessages(clientStream);
        }

        private void ConvertBufferToMessages(ConnectedIPs c)
        {
            //keep getting messages while there is text in the buffer
            while (c.messageReadQueueRaw.Length > 0)
            {
                String orig = c.messageReadQueueRaw;

                //get one command
                try
                {
                    int startm = 0;
                    do
                    {
                        //get indexes of start and end
                        startm = c.messageReadQueueRaw.IndexOf(startMessage);
                        //if a message starts after 0, remove info before and retry
                        if (startm > 0)
                        {
                            c.messageReadQueueRaw = c.messageReadQueueRaw.Substring(startm);
                        }
                    } while (startm > 0);

                    //no message = error
                    if (startm == -1)
                        throw new Exception("Junk text received:" + orig);

                    int endm = c.messageReadQueueRaw.IndexOf(endMessage) + endMessage.Length;

                    string messagestr = c.messageReadQueueRaw.Substring(startm, endm - startm);
                    c.messageReadQueueRaw = c.messageReadQueueRaw.Remove(startm, endm - startm);

                    //remove prefix and suffix
                    messagestr = messagestr.Substring(0, messagestr.Length - endMessage.Length);
                    messagestr = messagestr.Substring(startMessage.Length);

                    Message m = Message.DeserialiseMessage(messagestr);
                    c.messageReadQueue.Add(m);
                }
                catch (Exception ex)
                {
                    Manager.FireLogEvent("error getting messages from buffer", MessagePriority.High, true);
                    Logging.WriteExceptionToFile("error getting messages from buffer", ex);
                    c.messageReadQueueRaw = "";
                }
            }

            if (c.messageReadQueue.Count > 0)
            {
                //remove duplicates of messages
                TrimDupes(c);
            }
        }

        private Message.sendVars? GetMessageInfo(Message m)
        {
            Message.sendVars svc;
            try
            {
                svc = (Message.sendVars)Enum.Parse(typeof(Message.sendVars), m.messageParams[0]);
                return svc;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void TrimDupes(ConnectedIPs c)
        {
        retry:
            //get message
            for (int a = c.messageReadQueue.Count - 1; a > 0; a--)
            {
                Message m = c.messageReadQueue[a];
                Message.sendVars? svc = GetMessageInfo(m);
                if (svc == null)
                    continue;

                if (m.messageType == Message.Messages.SendingVars && svc == Message.sendVars.UpdateShipPosition)
                {
                    if (TrimDupesRemove(c, (Message.sendVars)svc, a))
                        goto retry;
                }
            }
        }

        private bool TrimDupesRemove(ConnectedIPs c, Message.sendVars firstSV, int firstIndex)
        {
            bool removed = false;
            //get message
            for (int a = firstIndex - 1; a > 0; a--)
            {
                Message m = c.messageReadQueue[a];
                Message.sendVars? svc = GetMessageInfo(m);
                if (svc == null)
                    continue;

                if (svc == firstSV)
                {
                    Manager.FireLogEvent("from message:" + firstSV.ToString() + " index:" + firstIndex.ToString() +
                                         " we removed:" + svc.ToString() + " index:" + a.ToString(), MessagePriority.Low,
                                         false);
                    c.messageReadQueue.Remove(m);
                    removed = true;
                }
            }

            return removed;
        }

        public Message popMessage(ConnectedIPs c, bool waitForValidReponse = false,
                                  long getResponseToMessageID = -1)
        {
            //try and get a good message
            if (c.messageReadQueue.Count > 0)
            {
                foreach (Message m1 in c.messageReadQueue)
                {
                    if ((getResponseToMessageID == -1) ||
                        (getResponseToMessageID != -1 && m1.ResponseID == getResponseToMessageID))
                    {
                        Manager.FireLogEvent("Got Message:", MessagePriority.Information, false, c.ID, m1);
                        c.messageReadQueue.Remove(m1);
                        return m1;
                    }
                }
            }

            return null;
        }

        private bool HandshakeConnections(UdpClient uc, ConnectedIPs client)
        {
            //1:server sends tcp ok
            //2:client sends udp ok
            //3:server sends udp ok
            string okstr = Message.Messages.MessageOK.ToString();

            //HEARTBEAT, if true dont init udp further and continue
            //CLIENT
            if (client is ConnectedServer)
            {
                //send packet with whether we want a heartbeat only
                SendTCPString(gcs.gameConfig.heartbeatonly.ToString(), connectedServer);

                //wait for ok
                string okrecv = ReadTCPString(client.tcpClient);
                if (okrecv == null || okrecv.Equals(okstr) == false)
                {
                    Manager.FireLogEvent("error on heartbeat handshake", MessagePriority.High, true);
                    return false;
                }

                //if heartbeat, return and dont continue udp handshake
                if (gcs.gameConfig.heartbeatonly)
                    return true;
            }
            //SERVER
            else
            {
                String hb = ReadTCPString(client.tcpClient);
                ((ConnectedClient)client).heartbeatonly = bool.Parse(hb);

                //send ok
                SendTCPString(okstr, client);

                if (((ConnectedClient)client).heartbeatonly)
                    return true;
            }

            //UDP handshake
            //CLIENT
            if (client is ConnectedServer)
            {
                //wait until server sends tcp ok packet
                string okrecv = ReadTCPString(client.tcpClient);
                if (okrecv == null || okrecv.Equals(okstr) == false)
                {
                    Manager.FireLogEvent("Error on tcp connect. Are the ports forwarded?", MessagePriority.High, true);
                    return false;
                }

                //send ok string on udp
                SendUDPString(okstr, uc, client);

                //wait until server sends OK
                string okrecvudp = ReadUDPString(uc, client);
                if (okrecvudp == null || okrecvudp.Equals(okstr) == false)
                {
                    Manager.FireLogEvent("Error on udp connect. Are the ports forwarded?", MessagePriority.High, true);
                    return false;
                }
            }
            //FOR SERVER
            else
            {
                //send tcp packet telling client udp receive is ready
                SendTCPString(okstr, client);

                //read a udp packet sent from the client to get ip and port
                string r = ReadUDPString(uc, client);
                if (r == null || r.Equals(okstr) == false)
                {
                    Manager.FireLogEvent("Error on udp connect. Are the ports forwarded?", MessagePriority.High, true);
                    return false;
                }

                SendUDPString(okstr, uc, client);
            }

            return true;
        }

        #endregion connections
    }
}