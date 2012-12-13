using System;
using System.Collections.Generic;
using System.Linq;
using Project.Model;

namespace Project.Networking
{
    public class Message
    {
        #region Messages enum

        public enum Messages
        {
            /// <summary>
            /// CASE:[client] connecting to server
            /// VARS:
            /// [1]=client alias (>0 length)
            /// RETURN:
            /// server:MessageOK
            /// </summary>
            Connect = 100,

            /// <summary>
            /// CASE:server/client accepting message
            /// VARS:
            /// NONE
            /// RETURN:
            /// NONE
            /// </summary>
            MessageOK = 101,

            /// <summary>
            /// CASE:[server/client] updating other
            /// VARS:
            /// [1]:SendVars[Server/Client] for type
            /// [2]:Array of values correct for sendVarsType
            /// RETURN:
            /// MessageOK
            /// </summary>
            SendingVars = 102,

            /// <summary>
            /// CASE: [server/client] talking to others
            /// VARS:
            /// [0]:playername
            /// [1]:text
            /// </summary>
            Say = 104,

            /// <summary>
            /// client-> server asking for sectors they can join.
            /// </summary>
            RequestSectors=105,

            /// <summary>
            /// server->client with sectors
            /// </summary>
            SendSectors=106,

            /// <summary>
            /// CASE:[client] telling server it wants to join a game mode 
            /// </summary>
            JoinGame = 107,

            /// <summary>
            /// CASE:[server] telling a client to do post join activites such as:
            /// zoom to map
            /// set gameclient - player ship variables
            /// choose ship
            /// </summary>
            JoinGameACK = 108,
        }

        #endregion

        #region sendVars enum

        public enum sendVars
        {
            #region other

            /// VAR 0 = sendvars type
            /// <summary>
            /// CASE: [server] -> client
            /// VARS:
            /// [1]:ID
            /// </summary>
            GiveID = 51,

            /// <summary>
            /// CASE: [server] -> client
            /// </summary>
            ShipAreaUpdate = 52,

            /// <summary>
            /// CASE:[client/server] telling client ship control change
            /// </summary>
            ClientControlShip = 53,

            /// <summary>
            /// CASE:[client] asking server to prompt when a ship can be ejected out of a hangar
            /// </summary>
            //EjectShipFromHangarPost = 54,
            /// <summary>
            /// CASE:[client] asking server to create a game mode
            /// </summary>
            RequestGameSectorCreation = 55,

            /// <summary>
            /// CASE:server giving credits to player for ship destruction
            /// </summary>
            GiveCredits=59,


            #endregion other

            #region create

            /// <summary>
            /// CASE: [server] -> client
            /// </summary>
            CreateRegion = 100,

            /// <summary>
            /// CASE: [server] -> client
            /// </summary>
            CreateSectorMap = 101,

            /// <summary>
            /// CASE: [server] -> client
            /// </summary>
            CreateShip = 102,

            /// <summary>
            /// CASE: server->client to create shot
            /// </summary>
            CreateShot = 103,

            /// <summary>
            /// CASE: client requesting server to fire their shot
            /// </summary>
            RequestShotCreate = 104,

            /// <summary>
            /// CASE: [client/server]
            /// </summary>
            CreateBuilding = 105,

            #endregion create

            #region update

            /// <summary>
            /// CASE:[client/server] updating ship position
            /// </summary>
            UpdateShipPosition = 201,

            /// <summary>
            /// CASE:[client/server] updating shot position
            /// </summary>
            UpdateShotPosition = 202,

            /// <summary>
            /// CASE:[client/server] updating building info
            /// </summary>
            UpdateBuilding = 203,

            UpdateShipStats = 204,

            UpdateTerrain = 205,

            #endregion update

            #region remove

            /// <summary>
            /// CASE:[server] -> client telling ship is destroyed
            /// </summary>
            ShipRemove = 301,

            /// <summary>
            /// CASE:[server] -> client telling shot is destroyed
            /// </summary>
            ShotRemove = 302,

            /// <summary>
            /// CASE:[server] -> client telling building is destroyed
            /// </summary>
            BuildingRemove = 303,

            #endregion remove
        }

        public static List<sendVars> UDPMessages;

        #endregion

        private static long lastID;
        public long ID = -1;
        public long ResponseID = -1;
        public List<String> messageParams;
        public Messages messageType;

        static Message()
        {
            lastID = Manager.r.Next()%10;
            UDPMessages = new List<sendVars>();
            UDPMessages.Add(sendVars.UpdateShipPosition);
            UDPMessages.Add(sendVars.UpdateShotPosition);
            UDPMessages.Add(sendVars.UpdateBuilding);
            UDPMessages.Add(sendVars.UpdateShipStats);
            UDPMessages.Add(sendVars.UpdateTerrain);
        }

        public bool IsUDPBoundMessage()
        {
            if (messageType != Messages.SendingVars)
                return false;
            try
            {
                var svc =
                        (sendVars)
                        Enum.Parse(typeof(sendVars), messageParams[0]);

                return (UDPMessages.Contains(svc));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Message(bool assignID = true)
        {
            if (assignID)
                ID = lastID++;
        }

        public string getMessageTypeString()
        {
            var ret = messageType.ToString();
            if (messageType != Messages.SendingVars ||
                (messageType == Messages.SendingVars && messageParams.Count() < 1))
                return ret;

            var p = int.Parse(messageParams[0]);
            ret += ((sendVars) p).ToString();
            return ret;
        }

        public Message Clone()
        {
            var newm = new Message(false)
                           {
                               ID = ID,
                               messageParams = messageParams,
                               messageType = messageType,
                               ResponseID = ResponseID
                           };
            return newm;
        }

        public String getMessageParamString()
        {
            if (messageParams.Count == 0)
                return "";
            var concat = messageParams.Aggregate((a, b) => a + SynchMain.separator + b);
            return concat;
        }

        public override string ToString()
        {
            var msg = SynchMain.startMessage +
                         ID + SynchMain.separator +
                         ResponseID + SynchMain.separator +
                         messageType.ToString("d") + SynchMain.separator +
                         getMessageParamString() + SynchMain.endMessage;
            return msg;
        }

        public static Message DeserialiseMessage(string joined)
        {
            try
            {
                //split by separators
                var msgpart = joined.Split(SynchMain.separator);
                var msgpartList = new List<string>();

                //message ID
                var gotID = Int64.Parse(msgpart[0]);
                //responding to message ID
                var respID = Int64.Parse(msgpart[1]);
                //type
                var type = Int64.Parse(msgpart[2]);

                //remove the first three elements
                Shared.removeStringFromArray(ref msgpart, 0, 3);

                //add rest to params
                msgpartList.AddRange(msgpart);

                var m = new Message(false)
                            {
                                messageType = (Messages) type,
                                messageParams = msgpartList,
                                ID = gotID,
                                ResponseID = respID
                            };
                return m;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public static Message CreateMessage(Messages mType, List<String> paramsS, Message respondingToThis = null)
        {
            long rID = -1;
            if (respondingToThis != null)
                rID = respondingToThis.ID;

            return new Message {ResponseID = rID, messageType = mType, messageParams = paramsS};
        }

        public static Message CreateMessage(Messages mType, String param1, Message respondingToThis = null)
        {
            return CreateMessage(mType, new List<string> {param1}, respondingToThis);
        }

        public static Message CreateOKMessage(Message respondingToThis)
        {
            return CreateMessage(Messages.MessageOK, "", respondingToThis);
        }
    }
}