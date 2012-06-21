using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Project.Networking;

namespace Project.Model
{
    public class Manager
    {
        /*
        public static SynchMain GetSynchMain()
        {
            return synchMain;
        }

        private static SynchMain synchMain;
         * */
        public static Random r;

        #region logging
        public delegate void LogAddDel(int clientID, String text, Message m, SynchMain.MessagePriority priority, bool isError);
        private static event LogAddDel LogAdd;

        public static void FireLogEvent(int clientID, String text, Message m, SynchMain.MessagePriority priority, bool isError)
        {
            try
            {
                if (LogAdd != null)
                    LogAdd(clientID, text, m, priority, isError);
            }
            catch (Exception)
            {
            }
        }

        public static void FireLogEvent(string prependMessage, SynchMain.MessagePriority priority, bool isError, int clientID = -1, Message m = null)
        {
            if (LogAdd == null)
                return;

            if (clientID == -1 && GameControlClient.synchMain != null)
                clientID = GameControlClient.synchMain.myID;

            FireLogEvent(clientID, prependMessage, m, priority, isError);
        }

        public static void AddLogEventDelegate(LogAddDel l)
        {
            LogAdd += l;
        }

        public static void RemoveLogEventDelegate(LogAddDel l)
        {
            LogAdd -= l;
        }
        #endregion logging

        #region game

        public static void StartGame(out SynchMain sm, GameInitConfig gic, int seed = 1)
        {
            sm = new SynchMain();

            //update the seed
            r = new Random(seed);

            //create the synching class, and init tcp/udp
            sm.Init(gic);

            //add my ip
            gic.ip = Shared.GetMyIPAddress();
        }

        public static void EndGame(ref SynchMain sm, bool closewindow)
        {
            if (sm != null)
            {
                sm.StopServer();
            }

            sm = null;
        }

        public static double GetMillisecondsNow()
        {
            var ts = new TimeSpan(DateTime.Now.Ticks);
            return ts.TotalMilliseconds;
        }

        /// <summary>
        /// on first get, will extract from config file and use for future ref
        /// </summary>
        public static bool ConfigSingleThread = true;
        public static int ConfigUpdateMS = 10;//100 fps
        public static int ConfigConnectionListenMS = 2000;//2 sec
        public static int ConfigConnectionRetryMS = 1000;//1 sec

        private static int? DisplayFPS = 17;//60 fps
        public static int ConfigDisplayMS
        {
            get
            {
                if (DisplayFPS == null)
                {
                    DisplayFPS = 1000/GetAppConfigValueInt(AppConfigValues.DisplayFPS);
                }
                return (int)DisplayFPS;
            }
        }

        public enum AppConfigValues
        {
            DisplayFPS
        }

        private static string GetAppConfigDefault(AppConfigValues type)
        {
            switch (type)
            {
                    case AppConfigValues.DisplayFPS:
                    return "60";

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private static string GetAppConfigValueBack(AppConfigValues type)
        {
            String types = type.ToString();
            //if it doesnt exist, create default
            if (ConfigurationManager.ConnectionStrings[types] == null)
            {
                var c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                String def = GetAppConfigDefault(type);
                c.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(types, def));
                // Save the configuration file.
                c.Save(ConfigurationSaveMode.Modified);
                // Force a reload of the changed section.
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            //return value
            return ConfigurationManager.ConnectionStrings[types].ConnectionString;
        }

        private static string GetAppConfigValueString(AppConfigValues type)
        {
            return GetAppConfigValueBack(type);
        }

        private static bool GetAppConfigValueBool(AppConfigValues type)
        {
            string v = GetAppConfigValueBack(type);
            return bool.Parse(v);
        }

        private static int GetAppConfigValueInt(AppConfigValues type)
        {
            string v = GetAppConfigValueBack(type);
            return int.Parse(v);
        }

        #endregion game
    }


}
