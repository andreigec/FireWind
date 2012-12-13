using Project.Model.Networking;

namespace Project.Networking.mapInfoSynch
{
    /// <summary>
    /// One per instance
    /// </summary>
    public class SynchStatusMain
    {
        /// <summary>
        /// if set to false, the client/server wont force the other to create or delete this item (useful for clients that get given objects)
        /// </summary>
        public bool SendCreateDelete = new Wrapper<bool>(true);

        public Wrapper<bool> StartUpdates = new Wrapper<bool>(false);
    }

    /// <summary>
    /// several per instance, one for each client
    /// </summary>
    public class SynchStatus
    {
        /// <summary>
        /// create the item remotely
        /// </summary>
        public Wrapper<bool> Created = new Wrapper<bool>(false);

        public Wrapper<bool> HeartbeatSent = new Wrapper<bool>(false);

        /// <summary>
        /// delete the item remotely
        /// </summary>
        public Wrapper<bool> Deleted = new Wrapper<bool>(false);
    }

    public class SynchStatusArea : SynchStatus
    {
    }

    public class SynchStatusConnectedClient : SynchStatus
    {
        public Wrapper<bool> GivenID = new Wrapper<bool>(false);
        
        public Wrapper<bool> TryForPostACK = new Wrapper<bool>(false);
        public map JoinMap = null;
        public PlayerShipClass JoinPSC = null;
    }

    public class SynchStatusMoveObjectInstance : SynchStatus
    {
        //public Wrapper<bool> UpdateArea = new Wrapper<bool>(true);
        public Wrapper<bool> UpdateLocation = new Wrapper<bool>(true);
    }

    public class SynchStatusShip : SynchStatusMoveObjectInstance
    {
        public Wrapper<long> ParentArea = new Wrapper<long>(-1);
    }

    public class SynchStatusConnectedServer : SynchStatus
    {
        /// <summary>
        /// received id from server
        /// </summary>
        public Wrapper<bool> ReceivedID = new Wrapper<bool>(false);
    }
}