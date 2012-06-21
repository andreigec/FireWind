using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Project.Model;
using Project.Model.Instances;
using Project.Model.Networking;
using Project.Networking.mapInfoSynch;

namespace Project.Networking
{
    public abstract class ConnectedIPs : SynchMainHolder
    {
        public static int UpdateTimeout = 20;
        public static int currentID = 1;
        public int ID;
        public bool ForceDisconnect = false;
        public List<Message> messageReadQueue = new List<Message>();
        public string messageReadQueueRaw;
        public List<Message> messageWriteQueue = new List<Message>();

        //getting this id will change these bools
        public Dictionary<long, List<VarToChangeOnOKReceipt>> responseMsgBoolChange =
            new Dictionary<long, List<VarToChangeOnOKReceipt>>();

        public TcpClient tcpClient;

        //when receiving a udp from a client, save their UDP details here to send in the future
        public IPEndPoint UDPSendHere=new IPEndPoint(IPAddress.Any, 0);
        public SynchMain parentSynchMain { get; set; }

        public ConnectedIPs()
        {
        }

        public ConnectedIPs(SynchMain sm, TcpClient tclient, UdpClient uclient)
        {
            parentSynchMain = sm;

            tcpClient = tclient;

            if (tcpClient == null)
                tcpClient = new TcpClient();

            ID = currentID++;
            messageReadQueueRaw = "";
        }

        public bool OKMessageHandled(Message m)
        {
            var foundref = false;
            //may be required to flip a variable
            if (m.ResponseID != -1 && responseMsgBoolChange.Count() > 0)
            {
                //get list of response messages that match the id
                var m2 =
                    responseMsgBoolChange.Where(s => s.Key == m.ResponseID);
                if (m2.Count() == 0)
                {
                    foundref = false;
                }
                else
                {
                    foundref = true;
                    foreach (var m3 in m2)
                    {
                        //loop through all the items that need to be changed
                        foreach (var m4 in m3.Value)
                        {
                            if (m4.wrapperref is Wrapper<bool>)
                            {
                                (m4.wrapperref as Wrapper<bool>).Value = (bool) m4.changeToThis;
                                (m4.wrapperref as Wrapper<bool>).WaitForResponse = false;
                            }

                            else if (m4.wrapperref is Wrapper<long>)
                            {
                                (m4.wrapperref as Wrapper<long>).Value = (long) m4.changeToThis;
                                (m4.wrapperref as Wrapper<long>).WaitForResponse = false;
                            }

                            else if (m4.wrapperref is Wrapper<int>)
                            {
                                (m4.wrapperref as Wrapper<int>).Value = (int) m4.changeToThis;
                                (m4.wrapperref as Wrapper<int>).WaitForResponse = false;
                            }

                            else if (m4.wrapperref is Wrapper<string>)
                            {
                                (m4.wrapperref as Wrapper<string>).Value = (string) m4.changeToThis;
                                (m4.wrapperref as Wrapper<string>).WaitForResponse = false;
                            }
                            else
                            {
                                Manager.FireLogEvent("error converting wrapper", SynchMain.MessagePriority.High, true,
                                                 -1, null);
                            }
                        }
                    }
                }

                while (m2.Count() != 0)
                {
                    responseMsgBoolChange.Remove(m2.First().Key);
                }
            }
            return foundref;
        }


        /// <summary>
        /// Require that a message is returned with value OK later before a bool value is changed
        /// </summary>
        /// <param name="cc"></param>
        /// The client/server the return requirement is imposed upon
        /// <param name="messageID"></param>
        /// The message id we are sending, and should be returned
        /// <param name="SS"></param>
        /// will change the valuesent in this structure to true, so we dont send multiple times
        /// <param name="bw"></param>
        /// the boolstructure we will change to the next value
        /// <param name="changeToThis"></param>
        /// change the boolstructures value to this
        public void AddResponseRequirement<T>(long messageID, SynchStatus SS, Wrapper<T> bw,
                                              T changeToThis)
        {
            bw.WaitForResponse = true;

            //create the basic struct if it doesnt exist/clear if it does
            if (responseMsgBoolChange.ContainsKey(messageID) == false)
                responseMsgBoolChange[messageID] = new List<VarToChangeOnOKReceipt>();

            responseMsgBoolChange[messageID].Add(new VarToChangeOnOKReceipt(bw, changeToThis));
        }

        #region Nested type: VarToChangeOnOKReceipt

        public class VarToChangeOnOKReceipt
        {
            public object changeToThis;
            public object wrapperref;

            public VarToChangeOnOKReceipt(object boolref, object changeToThis)
            {
                wrapperref = boolref;
                this.changeToThis = changeToThis;
            }
        }

        #endregion
    }
}