using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.mapInfo;

namespace Project.Networking
{
    [Flags]
    public enum UpdateModes
    {
        None = 0,
        All = 1,
        UpdateAI = 2,
        CheckHitboxes = 4,
        UpdateTerrain = 8,
        MoveObjects = 16,
        RemoveOutOfBounds = 32,
        CreateBuildings = 64,
    }

    public class GameControlServer
    {
        public GameInitConfig gameConfig;
        public Region gameRegion;

        public SynchMain parentSynch;
        private UpdateModes updateFlags = UpdateModes.None;

        //use static constructor
        private GameControlServer()
        {
        }

        public UpdateModes UPDATEFLAGS
        {
            get { return updateFlags; }
        }

        public bool flagChecked(UpdateModes comp)
        {
            return Shared.flagChecked(updateFlags, comp);
        }

        public static GameControlServer CreateGameControlServer(SynchMain sm, GameInitConfig cfg)
        {
            var gcs = new GameControlServer();
            gcs.parentSynch = sm;
            gcs.gameConfig = cfg;

            if (cfg.isServer())
            {
                //server should do everything
                gcs.updateFlags = UpdateModes.All;
                //create the base region, and start updates
                gcs.gameRegion = new Region(gcs, SetID.CreateSetNew());
            }
            else
            {
                gcs.updateFlags = UpdateModes.MoveObjects | UpdateModes.UpdateTerrain;
            }

            return gcs;
        }

        public void removePlayer(int playerID)
        {
            //remove players from map
            //remove player homeworld?

            //change ships to not be controlled
            List<ShipInstance> sl = gameRegion.GetShipByOwner(playerID);
            for (int a = 0; a < sl.Count; a++)
            {
                ShipInstance sh = sl[a];
                //TEMP REMOVE PLAYER SHIP
                var m = sh.parentArea as Map;
                sh.instanceOwner.ReleaseControl();
                IShipAreaMIXIN.RemoveShipMixIn(m, sh);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (gameRegion != null)
                gameRegion.Update(gameTime);
        }

        #region sent to clients

        public bool GiveCreditForShipDestruction(long victorID, ShipInstance destroyed)
        {
            int am = ShipBaseItemsMIXIN.GetTotalItemsCost(destroyed)*2;

            if (GameControlClient.PlayerShipSet() && GameControlClient.playerShipClass.PlayerShip.ID == victorID)
            {
                GameControlClient.playerShipClass.ChangeCredits(am);
                return true;
            }

            if (gameConfig.isServer())
            {
                //get client
                IEnumerable<ConnectedClient> cc = parentSynch.connectedClients.Where(s => s.ID == victorID);

                //an ai ship destroyed another, ignore
                if (cc.Count() != 1)
                {
                    return false;
                }

                ConnectedClient ccl = cc.First();
                ConnectedClient.GiveCredits(ccl, victorID, destroyed);
                return true;
            }

            Manager.FireLogEvent("Give credit called incorrectly", SynchMain.MessagePriority.Low, true);
            return false;
        }

        #endregion sent to clients
    }
}