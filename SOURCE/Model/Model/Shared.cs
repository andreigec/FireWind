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
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Networking;

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
        public static void serialiseFile<T>(T obj, String outputFilename)
        {
            string f = serialiseText(obj);
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
            var ser = new XmlSerializer(typeof (T));
            object obj = ser.Deserialize(reader);
            return ((T) obj);
        }

        public static string GetFileText(String filename)
        {
            string ret = "";
            var sr = new StreamReader(filename);
            ret = sr.ReadToEnd();
            sr.Close();
            return ret;
        }

        public static T PopFirstListItem<T>(List<T> list)
        {
            T item = list[0];
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
                    if ((0x60 <= (int) keyData) && (0x69 >= (int) keyData))
                    {
                        KeyValue = (char) ((int) keyData - 0x30);
                    }
                    else
                    {
                        KeyValue = (char) keyData;
                    }
                    break;
            }
            return KeyValue;
        }

        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var body = (MemberExpression) expression.Body;
            return body.Member.Name;
        }

        public static bool flagChecked(UpdateModes value, UpdateModes comp)
        {
            bool res = (((value & comp) == comp) || ((value & UpdateModes.All) == UpdateModes.All));
            return res;
        }

        public static string GetStackTrace()
        {
            var stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames == null)
                return "";

            return stackFrames.Aggregate("", (current, stackFrame) => current + (stackFrame.GetMethod().Name + " \n"));
        }

        public static double mapRange(double value, double rawValueStart, double rawValueEnd, double mapRangeStart,
                                      double mapRangeEnd)
        {
            double dif = rawValueEnd - rawValueStart;
            if (dif == 0)
                dif = 1;

            double p = (value - rawValueStart)/(dif);
            if (p > 1)
                p = 1;
            if (p < 0)
                p = 0;

            double o = p*(mapRangeEnd - mapRangeStart);
            o += mapRangeStart;

            return o;
        }

        public static bool TimeSinceElapsed(ref double LastShotTimeStamp, GameTime GT, int millisecondsTrip)
        {
            double now = GT.TotalGameTime.TotalMilliseconds;
            double dif = now - LastShotTimeStamp;
            if (dif > millisecondsTrip)
            {
                LastShotTimeStamp = now;
                return true;
            }
            return false;
        }

        public static bool TimeSinceElapsed(double LastShotTimeStamp, GameTime GT, int millisecondsTrip)
        {
            double now = GT.TotalGameTime.TotalMilliseconds;
            double dif = now - LastShotTimeStamp;
            if (dif > millisecondsTrip)
            {
                return true;
            }
            return false;
        }

        public static void copyTo<T>(T fromV, T to)
        {
            Type t = fromV.GetType();
            PropertyInfo[] pi = t.GetProperties();
            FieldInfo[] fi = t.GetFields();

            foreach (PropertyInfo prop in pi)
            {
                PropertyInfo prop2 = to.GetType().GetProperty(prop.Name);
                object o = prop.GetValue(fromV, null);
                prop2.SetValue(to, o, null);
            }

            foreach (FieldInfo field in fi)
            {
                FieldInfo prop2 = to.GetType().GetField(field.Name);
                object o = field.GetValue(fromV);
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
            string str = value.ToString();
            char[] strc = str.ToCharArray();

            if (str.Length < toPlace)
                toPlace = str.Length;
            for (int a = 0; a < toPlace; a++)
            {
                int pos = str.Length - 1 - a;
                strc[pos] = '0';
            }

            string comb = strc.Aggregate("", (current, c) => current + c);

            try
            {
                int p = Int32.Parse(comb);
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
            return (float) (Math.PI*angle/180.0f);
        }


        public static void removeStringFromArray(ref string[] arr, int index, int removeCount = 1)
        {
            var arr2 = new string[arr.Length - removeCount];
            int count = 0;
            for (int b = 0; b < arr.Length; b++)
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
            int cindex = 0;
            foreach (string s in arr)
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