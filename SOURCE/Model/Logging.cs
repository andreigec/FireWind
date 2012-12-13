using System;
using System.IO;
using System.Threading;
using Project.Model;
using Project.Networking;

namespace Project
{
    public static class Logging
    {
        private const string logdir = "Logs";
        private const string clientlogprefix = "Client_";
        private const string serverlogprefix = "Server_";
        private const string crashlogprefix = "Crash_";

        private static string logfilename = "";
        private static string crashlogfilename = "";

        public static bool LoggingEnabled = true;
        public static bool CrashLoggingEnabled = true;
        public static bool HandleCrash = true;

        public static void InitLogging(bool IsClient)
        {
            if (Directory.Exists(logdir) == false)
                Directory.CreateDirectory(logdir);

            if (IsClient)
                logfilename = GetLogFileName(logdir, clientlogprefix);
            else
                logfilename = GetLogFileName(logdir, serverlogprefix);

            crashlogfilename = GetLogFileName(logdir, crashlogprefix);
        }

        public static string GetSanitisedTimeString()
        {
            var t = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            t = t.Replace(':', '.');
            t = t.Replace('/', '.');
            return t;
        }

        private static string GetLogFileName(String logdir, String prefix)
        {
            var t = GetSanitisedTimeString();
            return logdir + "/" + prefix + t + ".txt";
        }

        public static void WriteLineToLogFile(String line)
        {
            if (LoggingEnabled == false)
                return;

            lock (logfilename)
            {
                var sw = new StreamWriter(logfilename, true);
                sw.WriteLine(line);
                sw.Close();
            }
        }

        public static void WriteExceptionToFile(String prefixline, Exception ex)
        {
            if (CrashLoggingEnabled == false)
                return;
            
            lock (crashlogfilename)
            {
                var sw = new StreamWriter(crashlogfilename, true);
                sw.WriteLine(prefixline);
                sw.WriteLine(ex.ToString());
                sw.WriteLine("");
                sw.Close();
            }

            if (HandleCrash == false)
                throw ex;
        }
    }
}