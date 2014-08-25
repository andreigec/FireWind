using System;
using System.Collections.Generic;
using Project.Networking;

namespace Project.Model.Networking.Server.GameSynch
{
    /// <summary>
    /// used to tell clients the sectors(games) they can join
    /// </summary>
    public class Heartbeat
    {
        public ConnectDetails CD;
        public SectorConfig Config;
        public bool active = true;
        public long id;
        public string name;

        public Heartbeat(long idIN, SectorConfig configIN, string nameIN, ConnectDetails CDIN)
        {
            id = idIN;
            Config = configIN;
            name = nameIN;
            CD = CDIN;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var h = obj as Heartbeat;
            return id.Equals(h.id) && CD.Equals(h.CD);
        }

        public List<string> SerialiseCreate()
        {
            var ret = new List<String>();

            ret.Add(id.ToString());
            ret.AddRange(Config.SerialiseCreate());
            ret.Add(name);

            ret.Add(CD.ip);
            ret.Add(CD.TCPport.ToString());
            ret.Add(CD.UDPport.ToString());

            return ret;
        }

        private static Heartbeat DeserialiseCreateI(List<string> args)
        {
            long ID = long.Parse(Shared.PopFirstListItem(args));
            SectorConfig config = SectorConfig.DeserialiseCreate(args);
            string Name = Shared.PopFirstListItem(args);

            string ip = Shared.PopFirstListItem(args);
            int tcp = int.Parse(Shared.PopFirstListItem(args));
            int udp = int.Parse(Shared.PopFirstListItem(args));
            var cd = new ConnectDetails(ip, tcp, udp);

            var H = new Heartbeat(ID, config, Name, cd);
            return H;
        }

        public static List<Heartbeat> DeserialiseCreate(List<string> args)
        {
            int count = int.Parse(Shared.PopFirstListItem(args));
            var ret = new List<Heartbeat>();
            for (int a = 0; a < count; a++)
            {
                Heartbeat sl = DeserialiseCreateI(args);
                ret.Add(sl);
            }

            return ret;
        }
    }
}