using System;
using Project.View.Client.DrawableScreens.WPF_Screens;

namespace Project.Networking
{
    public interface IConnectDetails
    {
        int TCPport { get; set; }
        int UDPport { get; set; }
        string ip { get; set; }
    }

    public class ConnectDetails : IConnectDetails
    {
        public int TCPport { get; set; }
        public int UDPport { get; set; }
        public string ip { get; set; }

        public ConnectDetails(string ipIN, int TCPIN = GameInitConfig.DefaultTCPPort, int UDPIN = GameInitConfig.DefaultUDPPort)
        {
            ip = ipIN;
            TCPport = TCPIN;
            UDPport = UDPIN;
        }

        public override int GetHashCode()
        {
            if (ip == null) return 0;
            return ip.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var cd = obj as ConnectDetails;
            return ip.Equals(cd.ip)&&TCPport.Equals(cd.TCPport)&&UDPport.Equals(cd.UDPport);
        }
    }

    public class GameInitConfig : IConnectDetails
    {
        //client updates game for themself and runs AI

        #region ServerType enum

        public enum ServerType
        {
            Client,
            DedicatedServer,
            ListenServer,
            SinglePlayerServer
        }

        #endregion

        public int TCPport { get; set; }
        public int UDPport { get; set; }
        public string ip { get; set; }
        public ConnectDetails CreateConnectDetails()
        {
            return new ConnectDetails(ip, TCPport, UDPport);
        }
        
        /// <summary>
        /// ask for servers, then quit when we get them
        /// </summary>
        public bool heartbeatonly = false;

        public int maxPlayers = -1;
        public string rconPW = "";
        public string serverName = "FireWind Server";
        public ServerType serverType;

        public const int DefaultTCPPort = 3001;
        public const int DefaultUDPPort = 3003;
        /// <summary>
        /// SERVER
        /// </summary>
        public GameInitConfig(bool dedicated, string serverName, int maxPlayers, int udpPort =DefaultUDPPort, int tcpPort =DefaultTCPPort,
                              string rconPW = "zz"
                              )
        {
            TCPport = tcpPort;
            UDPport = udpPort;
            if (dedicated)
                serverType= ServerType.DedicatedServer;
            else
                serverType= ServerType.ListenServer;

            this.serverName = serverName;
            this.maxPlayers = maxPlayers;
            this.rconPW = rconPW;
        }

        /// <summary>
        /// CLIENT
        /// </summary>
        public GameInitConfig(String IPAddress, bool heartbeatonly = false, int udpPort =DefaultUDPPort, int tcpPort =DefaultTCPPort)
        {
            serverType = ServerType.Client;
            TCPport = tcpPort;
            UDPport = udpPort;
            ip = IPAddress;
            this.heartbeatonly = heartbeatonly;
        }

        /// <summary>
        /// SINGLE PLAYER
        /// </summary>
        public GameInitConfig()
        {
            serverType = ServerType.SinglePlayerServer;
        }

        /// <summary>
        /// dedicated, listen or 1p server
        /// </summary>
        /// <returns></returns>
        public bool isServer()
        {
            return serverType == ServerType.DedicatedServer ||
                   serverType == ServerType.ListenServer ||
                   serverType == ServerType.SinglePlayerServer;
        }

        /// <summary>
        /// listen server, 1p server or client
        /// </summary>
        /// <returns></returns>
        public bool playingLocally()
        {
            return serverType == ServerType.ListenServer ||
                   serverType == ServerType.SinglePlayerServer ||
                   serverType == ServerType.Client;
        }

        /// <summary>
        /// dedicated or listen - host server externally and allow other people to join
        /// </summary>
        /// <returns></returns>
        public bool hostExternally()
        {
            return serverType == ServerType.DedicatedServer ||
                   serverType == ServerType.ListenServer;
        }
    }
}