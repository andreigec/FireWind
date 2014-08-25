using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project;
using Project.Model;
using Project.Networking;
using Project.View.Client.ClientScreens;
using Project.View.Client.DrawableScreens.Full_Screens;
using XNA_Winforms_Wrapper;

namespace FireWind
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private ConsoleCL myConsole;

        public Game1(bool isClient)
        {
            Init(isClient);
        }

        private void Init(bool isClient)
        {
            var gdm = new GraphicsDeviceManager(this);
            Logging.InitLogging(isClient);
            //logging
            Logging.LoggingEnabled = true;
            //ConsoleCL.ConsoleEnabled = false;
            Logging.HandleCrash = true;

            if (isClient)
            {
                GameControlClient.ParentGame = this;
                myConsole = new ConsoleCL();
                myConsole.HideConsole();

                //window
                gdm.PreferredBackBufferWidth = 640;
                gdm.PreferredBackBufferHeight = 580;
                //big
                //gdm.PreferredBackBufferWidth = 1700;
                //gdm.PreferredBackBufferHeight = 900;


                gdm.PreferMultiSampling = true;
                IsMouseVisible = true;
                Window.AllowUserResizing = true;
                Window.ClientSizeChanged += Window_ClientSizeChanged;

#if DEBUG
                if (ConsoleCL.ConsoleEnabled)
                {
                    myConsole.ShowConsole();
                }

                Manager.AddLogEventDelegate(ConsoleCL.console_LogAdd);
#else
                Logging.LoggingEnabled = false;
#endif
            }

            gdm.ApplyChanges();
            Content.RootDirectory = "Content";
        }


        protected override void OnExiting(Object sender, EventArgs args)
        {
            Manager.FireLogEvent("Quit from:" + sender, SynchMain.MessagePriority.High, false);
            if (GameControlClient.synchMain != null)
                GameControlClient.synchMain.enabled = false;

            base.OnExiting(sender, args);
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            GameControlClient.changeResolution(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitGame();
        }

        public void InitGame()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            XNA_WF_Wrapper.StaticInit(this);

            loadXML.initXML(this);

            #region fast options

            //QUICK LOAD SHIPS
            /*
            loadXML.LoadPlayerShips();
            if (loadXML.loadedPlayerShips.ContainsKey("test1") == false)
                PlayerShipClass.CreatePlayerShip("test1");
            var psc = loadXML.loadedPlayerShips["test1"];
            GameControlClient.SetPlayerShipClass(psc);

            var sc = new SectorConfigColosseum(SectorConfig.Size.medium);
            */
            //CLIENT
            //MainScreen.defaultIP = "118.209.132.83";
            //MainScreen.defaultIP = "127.0.0.1";
            //GameControlClient.CreateGame(new GameInitConfig(3003, 3001, MainScreen.defaultIP), ps);

            //SINGLE PLAYER
            //GameControlClient.CreateGame(new GameInitConfig(), sc);

            //LISTEN SERVER
            //GameControlClient.CreateGame(new GameInitConfig(GameInitConfig.ServerType.ListenServer, "aa", 8, 3000, 3001),ps);

            #endregion fast options

            //init clientside
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("");

            GameControlClient.ParentGame = this;
            if (GameControlClient.playerShipClass == null)
                GameControlClient.ShowMainScreen();

            texturing.generateLandGradient(this, 1000);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if the window has focus, do the inputs
            if (IsActive)
            {
                GameControlClient.KeyboardUpdate(gameTime);
                GameControlClient.MouseUpdate(gameTime);
            }

            if (GameControlClient.synchMain != null)
            {
                if (GameControlClient.synchMain.enabled == false)
                {
                    LogAndExit("Quitting Update Thread");
                }
                else
                {
                    GameControlClient.synchMain.ServerUpdateBlock(gameTime);
                }
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            //clear screen
            GraphicsDevice.Clear(Color.YellowGreen);

            GameControlClient.UpdateFPS();

            if (GameControlClient.synchMain != null && GameControlClient.synchMain.enabled == false)
            {
                LogAndExit("Quitting Display Thread");
            }

            try
            {
                GameControlClient.Draw(gameTime);
            }
            catch (AbandonedMutexException e)
            {
                Manager.FireLogEvent("crash in display thread", SynchMain.MessagePriority.High, true);
                Logging.WriteExceptionToFile("display thread crash", e);
            }
            
            base.Draw(gameTime);
        }

        public void LogAndExit(string msg)
        {
            const int ExitMS = 2000;
            Manager.FireLogEvent(msg, SynchMain.MessagePriority.High, false, -1, null);
            Thread.Sleep(ExitMS);
            GameControlClient.EndGame(false);
            GameControlClient.ResetToBaseScreen(true);
        }
    }
}

