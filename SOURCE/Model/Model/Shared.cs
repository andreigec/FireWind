using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Networking;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Project.Model
{
    public class double2
    {
        public double X;
        public double Y;
    }

    public class Comparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _compareFn;

        public Comparer(Func<T, T, int> fn)
        {
            _compareFn = fn;
        }

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return _compareFn(x, y);
        }

        #endregion
    }

    public static class Shared
    {

        private static string GetWebPage(String address)
        {
            // used to build entire input
            var sb = new StringBuilder();

            // used on each read operation
            var buf = new byte[8192];

            // prepare the web page we will be asking for
            var request = (HttpWebRequest)
                WebRequest.Create(address);

            // execute the request
            var response = (HttpWebResponse)
                request.GetResponse();

            // we will read data via the response stream
            var resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            // print out page source
            return sb.ToString();
        }

        private static string GetLocalAddress()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }

        private static string GetExternalAddress()
        {
            try
            {
                String addr = GetWebPage("http://checkip.dyndns.org");
                if (string.IsNullOrEmpty(addr) == false)
                {
                    //remove up to :
                    addr = addr.Substring(addr.IndexOf(':') + 2);

                    //remove html
                    addr = addr.Substring(0, addr.IndexOf('<'));

                    return addr;
                }
                else
                    return null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static string GetMyIPAddress()
        {
            string ret = null;
            ret = GetExternalAddress();
            if (ret == null)
                ret = GetLocalAddress();

            return ret;
        }

        public static void serialiseFile<T>(T obj, String outputFilename)
        {
            var f = serialiseText(obj);
            var sw = new StreamWriter(outputFilename);
            sw.Write(f);
            sw.Close();
        }

        private static string serialiseText<T>(T obj)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            var ser = new XmlSerializer(obj.GetType());
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            ser.Serialize(writer, obj);
            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            return sb.ToString();
        }

        public static T DeserialiseFile<T>(String text)
        {
            var doc = new XmlDocument();
            doc.LoadXml(text);
            var reader = new XmlNodeReader(doc.DocumentElement);
            var ser = new XmlSerializer(typeof(T));
            var obj = ser.Deserialize(reader);
            return ((T)obj);
        }

        public static string GetFileText(String filename)
        {
            var ret = "";
            var sr = new StreamReader(filename);
            ret = sr.ReadToEnd();
            sr.Close();
            return ret;
        }

        public static T PopFirstListItem<T>(List<T> list)
        {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static char GetCharFromKeys(Keys keyData)
        {
            char KeyValue;
            switch (keyData)
            {
                case Keys.Add:
                case Keys.OemPlus:
                    KeyValue = '+';
                    break;
                case Keys.OemPeriod:
                    KeyValue = '.';
                    break;
                case Keys.OemComma:
                    KeyValue = ',';
                    break;
                case Keys.OemBackslash:
                    KeyValue = '\\';
                    break;
                case Keys.OemPipe:
                    KeyValue = '|';
                    break;
                case Keys.OemSemicolon:
                    KeyValue = ';';
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    KeyValue = '-';
                    break;
                case Keys.OemQuestion:
                    KeyValue = '?';
                    break;
                case Keys.Divide:
                    KeyValue = '/';
                    break;
                default:
                    if ((0x60 <= (int)keyData) && (0x69 >= (int)keyData))
                    {
                        KeyValue = (char)((int)keyData - 0x30);
                    }
                    else
                    {
                        KeyValue = (char)keyData;
                    }
                    break;
            }
            return KeyValue;
        }

        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        public static bool flagChecked(UpdateModes value, UpdateModes comp)
        {
            var res = (((value & comp) == comp) || ((value & UpdateModes.All) == UpdateModes.All));
            return res;
        }

        public static string GetStackTrace()
        {
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            if (stackFrames == null)
                return "";

            return stackFrames.Aggregate("", (current, stackFrame) => current + (stackFrame.GetMethod().Name + " \n"));
        }

        public static double mapRange(double value, double rawValueStart, double rawValueEnd, double mapRangeStart,
                                      double mapRangeEnd)
        {
            var dif = rawValueEnd - rawValueStart;
            if (dif == 0)
                dif = 1;

            var p = (value-rawValueStart) / (dif);
            if (p > 1)
                p = 1;
            if (p < 0)
                p = 0;

            var o = p * (mapRangeEnd - mapRangeStart);
            o += mapRangeStart;

            return o;
        }

        public static bool TimeSinceElapsed(ref double LastShotTimeStamp, GameTime GT, int millisecondsTrip)
        {
            var now = GT.TotalGameTime.TotalMilliseconds;
            var dif = now - LastShotTimeStamp;
            if (dif > millisecondsTrip)
            {
                LastShotTimeStamp = now;
                return true;
            }
            return false;
        }

        public static bool TimeSinceElapsed(double LastShotTimeStamp, GameTime GT, int millisecondsTrip)
        {
            var now = GT.TotalGameTime.TotalMilliseconds;
            var dif = now - LastShotTimeStamp;
            if (dif > millisecondsTrip)
            {
                return true;
            }
            return false;
        }

        public static void copyTo<T>(T fromV, T to)
        {
            var t = fromV.GetType();
            var pi = t.GetProperties();
            var fi = t.GetFields();

            foreach (var prop in pi)
            {
                var prop2 = to.GetType().GetProperty(prop.Name);
                var o = prop.GetValue(fromV, null);
                prop2.SetValue(to, o, null);
            }

            foreach (var field in fi)
            {
                var prop2 = to.GetType().GetField(field.Name);
                var o = field.GetValue(fromV);
                prop2.SetValue(to, o);
            }
        }

        public static void MergeDictionaries<A, B>(ref Dictionary<A, B> orig, Dictionary<A, B> copyfrom)
        {
            foreach (var x in copyfrom)
            {
                if (orig.ContainsKey(x.Key) == false)
                    orig.Add(x.Key, x.Value);
            }
        }

        public static int SmoothInt(int value, int toPlace)
        {
            var str = value.ToString();
            var strc = str.ToCharArray();

            if (str.Length < toPlace)
                toPlace = str.Length;
            for (var a = 0; a < toPlace; a++)
            {
                var pos = str.Length - 1 - a;
                strc[pos] = '0';
            }

            var comb = strc.Aggregate("", (current, c) => current + c);

            try
            {
                var p = Int32.Parse(comb);
                return p;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public static void createDictionaryNestedKey<T>(String key1, String key2, T value,
                                                        ref Dictionary<string, Dictionary<string, T>> dict)
            where T : new()
        {
            if (dict.ContainsKey(key1) == false)
                dict[key1] = new Dictionary<string, T>();

            if (dict[key1].ContainsKey(key2) == false)
                dict[key1][key2] = new T();

            dict[key1][key2] = value;
        }

        public static float DegreeToRadian(int angle)
        {
            return (float)(Math.PI * angle / 180.0f);
        }


        public static void removeStringFromArray(ref string[] arr, int index, int removeCount = 1)
        {
            var arr2 = new string[arr.Length - removeCount];
            var count = 0;
            for (var b = 0; b < arr.Length; b++)
            {
                if (b < index || b >= (index + removeCount))
                {
                    arr2[count] = arr[b];
                    count++;
                }
            }
            arr = arr2;
        }

        public static void addStringToArray(ref string[] arr, String add, int index)
        {
            var arr2 = new string[arr.Length + 1];
            var cindex = 0;
            foreach (var s in arr)
            {
                if (index == cindex)
                {
                    arr2[cindex] = add;
                    cindex++;
                }
                arr2[cindex] = s;
            }
            arr = arr2;
        }



        /*
        public static void ensureDictKeyExists<T,X,Y>(X key, ref T dict)where T:Dictionary<X,Y> where Y:new()
        {
            if (dict.ContainsKey(key) == false)
                dict[key] = new Y();
        }
        */

        /*
        public static String concatVars(object[] o,char separator=SynchMain.separator)
        {
            String ret = "";
            foreach (var o2 in o)
            {
                ret += o2.ToString();
                ret += separator;
            }

                ret=ret.TrimEnd(separator);
            return ret;
        }
         * */
    }
}