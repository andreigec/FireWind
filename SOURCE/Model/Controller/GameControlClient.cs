using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Model;
using Project.Model.Instances;
using Project.Model.Networking;
using Project.Model.Networking.Server;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using Project.View.Client.DrawableScreens.Full_Screens;
using Project.View.Client.DrawableScreens.Pop_Up_Screens;
using Project.View.Client.DrawableScreens.WPF_Screens;

namespace Project
{
    public static class GameControlClient
    {
        public static Game ParentGame;

        private static Camera2D mainCamera;
        private static readonly List<Camera2D> cameras = new List<Camera2D>();
        public static FPS myFPS = new FPS();

        public static PlayerShipClass playerShipClass;
        //public static int playerFaction;
        //public static sector PlayerHomeWorld;

        private static readonly KeyboardClass KBC = new KeyboardClass();
        private static readonly MouseClass MC = new MouseClass();

        public static SynchMain synchMain { get; private set; }

        public static bool PlayerShipSet()
        {
            return (!(playerShipClass == null || playerShipClass.PlayerShip == null));
        }

        public static bool GCSSet()
        {
            return synchMain != null && synchMain.gcs != null;
        }

        public static void SetPlayerShipClass(PlayerShipClass psc)
        {
            playerShipClass = psc;
        }

        public static void Draw(GameTime gameTime)
        {
            if (mainCamera != null)
            {
                mainCamera.Draw(gameTime);
                //show fps
                mainCamera.spriteBatch.Begin();
                var v = new Vector2(mainCamera.ViewportWidth - 100, 0);
                string fpsstr = "FPS:" + myFPS.getFPS().ToString();

                mainCamera.DrawString(fpsstr, Color.Red, v);
                mainCamera.spriteBatch.End();
            }

            foreach (Camera2D c in cameras)
            {
                c.Draw(gameTime);
            }
        }

        public static void changeResolution(int newW, int newH)
        {
            mainCamera.setSize(newW, newH);
        }


        public static void UpdateFPS()
        {
            myFPS.UpdateFPS();
        }

        public static void MouseUpdate(GameTime gt)
        {
            MC.UpdateButtons(Mouse.GetState(), gt, mainCamera);

            List<Camera2D> c1 = cameras;
            int c = c1.Count();
            if (c > 0)
            {
                c1.First().drawThis.MouseUpdate(gt, MC);
            }
            else if (mainCamera != null)
                mainCamera.drawThis.MouseUpdate(gt, MC);

            MC.SwitchStates();
        }

        public static void KeyboardUpdate(GameTime gt)
        {
            KBC.UpdateKeys(Keyboard.GetState(), gt);
            if (KBC.KeysPressed())
            {
                if (KBC.CanUseKey(Keys.Q, 0))
                    ParentGame.Exit();

                bool subcamera = cameras.Count > 0;
                for (int a = 0; a < cameras.Count; a++)
                    cameras[a].drawThis.KeyboardUpdate(gt, KBC);

                if (subcamera == false)
                    mainCamera.drawThis.KeyboardUpdate(gt, KBC);
            }
            KBC.SwitchStates();
        }

        public static bool chooseShip(ShipInstance sh)
        {
            //ship that can be controlled, is not owned by another player, and is not currently being controlled
            if (sh == null)
                return false;

            if (sh.instanceOwner.PlayerCanControl(synchMain.myID, synchMain.myFaction) == false)
                return false;

            ShipInstance old = playerShipClass.PlayerShip;
            playerShipClass.PlayerShip = sh;
            playerShipClass.PlayerShip.instanceOwner.ChangeControl(synchMain.myID, synchMain.myFaction);
            if (synchMain.gcs.gameConfig.isServer() == false)
            {
                synchMain.connectedServer.addControlShipToQueue(sh, old);
            }

            return true;
        }

        public static bool fireShot(Map m, ShipInstance si, weapon w)
        {
            if (m.parentGCS.gameConfig.isServer() == false)
            {
                m.parentGCS.parentSynch.connectedServer.requestShotCreation(m, si, w);
            }
            else
            {
                WeaponInstance.addShot(m, si, w);
            }
            return true;
        }

        public static void RemoveAllPlayerShipsFromArea(ShipInstance si, IshipAreaSynch isa, Region r)
        {
            List<ShipInstance> ships = r.GetShipByOwner(si.instanceOwner.PlayerOwnerID);
            for (int a = 0; a < ships.Count(); a++)
            {
                IShipAreaMIXIN.RemoveShipMixIn(isa, ships[a]);
            }
        }

        /// <summary>
        /// use this to remove ANY ship from the game, either client or server side
        /// </summary>
        /// <param name="si"></param>
        /// <param name="r"></param>
        public static void ResetShip(ShipInstance si, Region r, bool payToReturn)
        {
            GameControlServer gcs = si.parentGCS;
            SynchMain sm = gcs.parentSynch;

            IshipAreaSynch isa = si.parentArea;

            bool controlled = si.instanceOwner.BeingControlled();
            bool mycontrol = PlayerShipSet() && playerShipClass.PlayerShip == si;

            //server
            if (gcs.gameConfig.isServer())
            {
                if (controlled)
                {
                    //remove player ship and all support craft from map
                    RemoveAllPlayerShipsFromArea(si, isa, r);
                    /*
                    //if the ship has been destroyed, kick player
                    var cs = sm.connectedClients.Where(cc => cc.ID == si.instanceOwner.PlayerOwnerID);
                    if (cs.Count() != 1)
                    {
                        Manager.FireLogEvent("Cant find player to purge from cache", SynchMain.MessagePriority.Low, true);
                    }
                    else
                    {
                        var cc = cs.First();

                        sm.DisconnectClient(cc, false);
                    }
                     * */
                }
                else
                    IShipAreaMIXIN.RemoveShipMixIn(isa, si);

                Manager.FireLogEvent("Removed Ship:", SynchMain.MessagePriority.High, false);
            }
                //client
            else
            {
                IShipAreaMIXIN.RemoveShipMixIn(isa, si);
                Manager.FireLogEvent("Removed Ship:", SynchMain.MessagePriority.High, false);
            }

            //gui actions
            if (gcs.gameConfig.playingLocally() && mycontrol)
            {
                if (payToReturn)
                {
                    //first change screen so that the map doesnt try and draw the player ship
                    ShowDestroyPopup();
                }
                else
                {
                    ShowVictoryPopup();
                }

                if (payToReturn)
                {
                    playerShipClass.ResetPSC();
                }
            }
        }

        //temp: this should be server only later -auto eject on player getting closer
        /*
        public static void requestShipHangarEject(ShipInstance si)
        {
            if (si == null)
                return;

            if (ParentServerGame.gameConfig.isServer() == false)
            {
                synchMain.connectedServer.addShipEjectToQueue(si);
            }
            else
            {
                ActionList.AddReleaseShipFromBuilding(si);
            }
        }
        */

        private static bool chooseShip(Map m)
        {
            //ship that can be controlled, is not owned by another player, and is not currently being controlled
            ShipInstance sh = null;
            IEnumerable<ShipInstance> shs =
                m.ships.Where(
                    s => s.instanceOwner.PlayerCanControl(synchMain.myID, synchMain.myFaction));
            if (shs.Count() == 0)
                return false;

            if (shs.Count() > 0)
                sh = shs.FirstOrDefault();

            return chooseShip(sh);
        }

        #region start and end games

        /// <summary>
        /// create and join game
        /// </summary>
        /// <param name="gic"></param>
        /// <param name="sc"></param>
        public static void CreateGame(GameInitConfig gic, SectorConfig sc)
        {
            //1: create base game
            StartGame(gic, 1);

            //2: create level
            Sector sec = CreateGameSector(sc);

            if (sec == null)
            {
                Manager.FireLogEvent("Error creating game sector", SynchMain.MessagePriority.High, true);
                return;
            }

            //3: join this level
            JoinGameSector(sec.ID);
        }

        /// <summary>
        /// either create or request a sector(game) to be created
        /// </summary>
        /// <param name="cfg"></param>
        private static Sector CreateGameSector(SectorConfig cfg)
        {
            if (cfg == null)
                return null;

            GameControlServer gcs = synchMain.gcs;

            //if a gamble map, take away the money
            if (cfg is SectorConfigGambleMatch)
            {
                var gm = cfg as SectorConfigGambleMatch;
                int cost = gm.shipcost*gm.shipcount;

                if (playerShipClass.Credits < cost)
                    return null;

                playerShipClass.ChangeCredits(-cost);
            }

            if (gcs.gameConfig.isServer() == false)
            {
                synchMain.connectedServer.requestGameServerCreation(cfg);
            }
            else
            {
                return gcs.gameRegion.addSector(cfg);
            }
            return null;
        }

        /// <summary>
        /// wrapper for manager.startgame that sets synchmain
        /// </summary>
        public static void StartGame(GameInitConfig gic, int seed = 1)
        {
            SynchMain sm;
            Manager.StartGame(out sm, gic, seed);
            synchMain = sm;
        }

        /// <summary>
        /// wrapper for manager.endgame that sets synchmain
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="closewindow"></param>
        public static void EndGame(bool closewindow)
        {
            SynchMain sm = synchMain;
            Manager.EndGame(ref sm, true);
            synchMain = sm;

            if (ParentGame != null && closewindow)
            {
                Manager.FireLogEvent("End Game triggered with close window", SynchMain.MessagePriority.High, false);
                ParentGame.Exit();
            }
                //otherwise close all children windows
            else
                cameras.Clear();
        }

        /// <summary>
        /// called from listen server/1p intending to join own game. requires a pre startgame call
        /// </summary>
        /// <param name="sectorID"></param>
        private static void JoinGameSector(long sectorID)
        {
            var s = synchMain.gcs.gameRegion.getArea(sectorID) as Sector;
            if (s != null)
            {
                //if the sector exists-
                //otherwise, do the server steps here:
                // set the map the player and their support craft want to join
                playerShipClass.PlayerShip.parentArea = s.thismap;
                playerShipClass.PlayerShip.parentGCS = synchMain.gcs;
                foreach (ShipInstance s2 in playerShipClass.SupportCraft)
                {
                    s2.parentArea = s.thismap;
                    s2.parentGCS = synchMain.gcs;
                }

                //set playership ids and instance owners
                playerShipClass.PlayerShip.setID(SetID.CreateSetNew());
                playerShipClass.PlayerShip.instanceOwner = new InstanceOwner(synchMain.myID, synchMain.myFaction,
                                                                             InstanceOwner.ControlType.JustOwner,
                                                                             synchMain.myID);
                foreach (ShipInstance sh in playerShipClass.SupportCraft)
                {
                    sh.setID(SetID.CreateSetNew());
                    sh.instanceOwner = new InstanceOwner(synchMain.myID, synchMain.myFaction,
                                                         InstanceOwner.ControlType.NoPlayer);
                }

                //fire event to create starting location for ship in map
                ActionList.AddJoinGameToAction(playerShipClass);
                PostRequestJoinGameSector();
            }
            else
            {
                Manager.FireLogEvent("Error finding client sent sector", SynchMain.MessagePriority.High, true);
            }
        }

        /// <summary>
        /// called when a specific server game is requested to be joined by a client. will connect from scratch and connect to a game id
        /// </summary>
        /// <param name="s"></param>
        public static void ConnectAndJoinGameSector(GameInitConfig cfg, long sectorID)
        {
            StartGame(cfg);

            //show wait dialog
            ShowConnectDialogPopup(synchMain, cfg.CreateConnectDetails(), ConnectWF.CancelConnectWait);
            synchMain.connectedServer.AddJoinGameToQueue(playerShipClass, sectorID);
        }

        /// <summary>
        /// called after requestToJoinGameSector
        /// choose a ship in the map, and zoom to the map
        /// </summary>
        public static void PostRequestJoinGameSector()
        {
            var m = playerShipClass.PlayerShip.parentArea as Map;

            //choose a ship
            chooseShip(m);

            //close any connect wait dialogs
            ExitPopupScreen<ConnectWaitWF>();
            //zoom into map
            ZoomToMap(m);
        }

        #endregion start and end games

        #region screens

        public static void ShowTitlesScreen()
        {
            ShowIScreen(new TitlesScreen());
        }

        public static void ShowMainScreen()
        {
            ShowIScreen(new MainScreenWF(true));
        }

        private static void ShowBaseScreen()
        {
            ShowIScreen(new BaseScreenWF());
        }

        public static void ShowOptionsScreen()
        {
            ShowIScreen(new OptionsWF());
        }

        public static void ShowChooseShipScreen(string existingPscName = "")
        {
            ShowIScreen(new ChooseShipWF(existingPscName));
        }

        public static void ShowHangarScreen()
        {
            ShowIScreen(new HangarScreenWF());
        }

        public static void ShowBaseShopScreen()
        {
            ShowIScreen(new ShopScreenWF());
        }

        public static void ShowConnectScreen()
        {
            ShowIScreen(new ConnectWF());
        }

        public static void ShowCreateMPGameScreen()
        {
            ShowIScreen(new CreateMPGameWF());
        }

        public static void ShowCreateGambleGameScreen()
        {
            ShowIScreen(new CreateGambleGameWF());
        }

        public static void ShowChooseGameModeScreen()
        {
            ShowIScreen(new ChooseGameModeWF());
        }

        public static void ShowDestroyPopup()
        {
            ShowIScreen(new DestroyScreen(),true);
        }

        public static void ShowVictoryPopup()
        {
            ShowIScreen(new ShowVictoryScreen(), true);
        }

        public static void ShowGetInfoPopup(List<RequiredItem> RI, InputInformation.AcceptInputItemsDelegate retfunc)
        {
            ShowIScreen(new InputInformation(RI, retfunc), true);
        }

        public static void ShowInGamePopup()
        {
            ShowIScreen(new InGameMenuPopup(),true);
        }

        public static void ShowConnectDialogPopup(SynchMain sm, ConnectDetails CD, ConnectWaitWF.CancelPressed cp)
        {
            ShowIScreen(new ConnectWaitWF(sm, CD, cp),true);
        }

        public static void ShowIScreen(IScreenControls screen, bool popup = false)
        {
            Viewport vp = ParentGame.GraphicsDevice.Viewport;
            var newc = new CameraFlat(vp.Width, vp.Height, new Vector2(0, 0), screen);
            if (popup)
                cameras.Add(newc);
            else
                mainCamera = newc;

            //keys
            newc.drawThis.RegisterKeyboardKeys(KBC);
        }

        public static void ExitPopupScreen<T>()
        {
            if (cameras.Count > 0)
                cameras.RemoveAll(s => s.drawThis is T);

            //reload keys for main camera if we have one
            if (mainCamera != null)
                mainCamera.drawThis.RegisterKeyboardKeys(KBC);
        }

        public static void ExitPopupScreens()
        {
            cameras.Clear();
            mainCamera.drawThis.RegisterKeyboardKeys(KBC);
        }

        public static void ResetToBaseScreen(bool reloadPSC)
        {
            //offload PSC
            if (PlayerShipSet() && reloadPSC)
            {
                string name = playerShipClass.Name;
                PlayerShipClass.SavePlayerShipClassFile();
                playerShipClass = null;

                //reload PSC
                loadXML.LoadPlayerShips();
                PlayerShipClass choose = loadXML.loadedPlayerShips[name];
                SetPlayerShipClass(choose);
            }

            ExitPopupScreens();
            ShowBaseScreen();
        }

        public static void ZoomToMap(Map m)
        {
            Viewport vp = ParentGame.GraphicsDevice.Viewport;
            var ms = new MapScreen(m);
            mainCamera = new CameraMap(vp.Width, vp.Height, new Vector2(0, 0), ms);
            var cm = mainCamera as CameraMap;
            ms.thisCamera = cm;

            if (playerShipClass.PlayerShip != null)
                playerShipClass.PlayerShip.forceAI = false;

            cm.adjustZoom(true);
            ms.thisCamera.FocusOnPoint(new Vector2(2414, -857));
            //keys
            mainCamera.drawThis.RegisterKeyboardKeys(KBC);
        }

        public static void ZoomToSector(Sector s)
        {
            Viewport vp = ParentGame.GraphicsDevice.Viewport;
            var ss = new SectorScreen(s);
            mainCamera = new CameraSector(vp.Width, vp.Height, new Vector2(0, 0), ss);
            ss.thisCamera = (CameraSector) mainCamera;

            if (playerShipClass.PlayerShip != null)
                playerShipClass.PlayerShip.forceAI = true;

            //keys
            mainCamera.drawThis.RegisterKeyboardKeys(KBC);
        }

        /*
        public static void ZoomToRegion(region r, bool maxZoom, PlayerShipClass psc)
        {
            var vp = ParentGame.GraphicsDevice.Viewport;
            var rs = new JoinSectorScreen(r, psc);
            mainCamera = new CameraRegion(vp.Width, vp.Height, new Vector2(0, 0), rs);
            rs.thisCamera = ((CameraRegion)mainCamera);


            if (playerShipClass != null && playerShipClass.PlayerShip != null)
                playerShipClass.PlayerShip.forceAI = true;

            if (maxZoom)
                ((CameraRegion)mainCamera).ChangeScroll(((CameraRegion)mainCamera).maxScrollTimes);

            //keys
            mainCamera.drawThis.RegisterKeyboardKeys(KBC);
        }
        */

        #endregion screens
    }
}