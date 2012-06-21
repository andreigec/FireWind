using System.Net.Sockets;

namespace Project.Networking
{
    public partial class ConnectedServer : ConnectedIPs
    {
        public ConnectedServer(SynchMain sm)
            : base(sm,null, null)
        {
        }
    }
}