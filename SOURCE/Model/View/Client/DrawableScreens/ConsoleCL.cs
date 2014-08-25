using System;
using System.Runtime.InteropServices;
using Project.Networking;

namespace Project.View.Client.ClientScreens
{
    public class ConsoleCL
    {
        private const string consolename = "FIREWINDSRVCONSOLE";
        private static IntPtr hWnd;

        public static bool ConsoleEnabled = true;
        private readonly bool initOK;

        public ConsoleCL()
        {
            try
            {
                Console.Title = consolename;
                hWnd = FindWindow(null, consolename); //put your console window caption here
                initOK = true;
            }
                //error means console wasnt opened
            catch (Exception)
            {
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public void HideConsole()
        {
            if (initOK)
                ShowWindow(hWnd, 0); // 0 = SW_HIDE
        }

        public void ShowConsole()
        {
            if (initOK)
                ShowWindow(hWnd, 1); //1 = SW_SHOWNORMA
        }

        public static void changeColour(SynchMain.MessagePriority priority, bool isError)
        {
            ConsoleColor fore;
            ConsoleColor back;
            switch (priority)
            {
                case SynchMain.MessagePriority.Information:
                    if (isError)
                    {
                        fore = ConsoleColor.White;
                        back = ConsoleColor.Black;
                    }
                    else
                    {
                        fore = ConsoleColor.White;
                        back = ConsoleColor.Black;
                    }
                    break;

                case SynchMain.MessagePriority.Low:
                    if (isError)
                    {
                        fore = ConsoleColor.DarkRed;
                        back = ConsoleColor.Black;
                    }
                    else
                    {
                        fore = ConsoleColor.DarkYellow;
                        back = ConsoleColor.Black;
                    }
                    break;

                case SynchMain.MessagePriority.High:
                    if (isError)
                    {
                        fore = ConsoleColor.Red;
                        back = ConsoleColor.Black;
                    }
                    else
                    {
                        fore = ConsoleColor.Yellow;
                        back = ConsoleColor.Black;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("priority");
            }

            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
        }

        public static void console_LogAdd(int clientID, String text, Message m, SynchMain.MessagePriority priority,
                                          bool isError)
        {
            string id = " ID:";
            if (m == null)
                id = "\t";
            else
                id += m.ID.ToString() + "\t";

            string rid = " RF:";
            if (m == null)
                rid = "\t";
            else
                rid += m.ResponseID.ToString() + "\t";

            string type = "";
            if (m == null)
                type = "\t";
            else
                type = m.getMessageTypeString() + "\t";

            string mt = "";
            if (m != null)
            {
                mt = m.ToString();
                if (mt.Length > 100)
                {
                    mt = mt.Substring(0, 100) + "...";
                }
            }

            changeColour(priority, isError);

            string s = DateTime.Now.ToLongTimeString() + id + rid + text + type
                       + mt + "\tpri:" + priority + "\tiserror?:" + isError.ToString();

            if (ConsoleEnabled)
                Console.WriteLine(s);

            //write to file
            Logging.WriteLineToLogFile(s);
        }
    }
}