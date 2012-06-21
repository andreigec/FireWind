using System;
using System.Net;
using System.Net.Sockets;

namespace Project.Networking
{
    public partial class ConnectedClient : ConnectedIPs
    {
        public static int maxFaction;
        public string alias;
        public int faction;
        public bool heartbeatonly = false;

        public ConnectedClient(SynchMain sm, String alias, TcpClient client, UdpClient uc)
            : base(sm,client, uc)
        {
            //client = new TcpClient();
            this.alias = alias;
            faction = maxFaction++;
        }

        public ConnectedClient()
        {
        }


    }
}