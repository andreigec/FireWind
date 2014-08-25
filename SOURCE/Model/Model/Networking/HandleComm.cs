using System;
using System.Collections.Generic;
using Project.Model;
using Project.Model.Instances;
using Project.Model.Networking.Server;
using Project.Model.Networking.Server.GameSynch;
using Project.Model.mapInfo;
using Project.Networking.mapInfoSynch;
using Project.View.Client.DrawableScreens.WPF_Screens;

namespace Project.Networking
{
    public partial class SynchMain
    {
        #region returncodes enum

        public enum returncodes
        {
            S_OK,
            E_NOT_FOUND,
            E_INSUF_PERM
        }

        #endregion

        public bool handleComm(Message m, bool ReceiverIsClient, ConnectedIPs cip)
        {
            returncodes RC;
            int faction = -1;
            if (cip is ConnectedClient)
                faction = ((ConnectedClient) cip).faction;
            int Vint1;
            int Vint2;
            ShipInstance sh;

            bool error = false;
            string errormsg = "";
            const string inappropriateMessage = "inappropriate Message";
            //save the message to a string now, because we will remove args from the message as we are working through them
            string mps = m.getMessageParamString();
            const int maxlen = 95;
            if (mps.Length > maxlen)
            {
                mps = mps.Substring(0, maxlen);
                mps = mps + "...";
            }

            switch (m.messageType)
            {
                case Message.Messages.MessageOK:
                    if (cip.OKMessageHandled(m) == false)
                        Manager.FireLogEvent("Hanging OK", MessagePriority.Low, true, cip.ID, m);
                    break;

                case Message.Messages.Say:
                    Manager.FireLogEvent("Chat", MessagePriority.High, false, cip.ID, m);
                    break;

                case Message.Messages.RequestSectors:
                    if (ReceiverIsClient)
                    {
                        error = true;
                        errormsg = inappropriateMessage;
                        break;
                    }

                    //construct string to send back
                    ((ConnectedClient) cip).SendJoinableSectors();
                    Manager.FireLogEvent("Sending sectors to client", MessagePriority.Low, false, cip.ID, m);

                    //remove client
                    DisconnectClient(cip as ConnectedClient, true);
                    break;

                case Message.Messages.SendSectors:
                    if (ReceiverIsClient == false)
                    {
                        error = true;
                        errormsg = inappropriateMessage;
                        break;
                    }

                    //get the list of sectors the player can join
                    List<Heartbeat> sectorlightlist = Heartbeat.DeserialiseCreate(m.messageParams);

                    //add them to the display list
                    ConnectWF.AddAvailSectors(sectorlightlist);

                    Manager.FireLogEvent("Received sectors from server", MessagePriority.Low, false, cip.ID, m);
                    EndClient();
                    break;

                case Message.Messages.Connect:
                    if (ReceiverIsClient)
                    {
                        error = true;
                        errormsg = inappropriateMessage;
                        break;
                    }

                    Manager.FireLogEvent("Added new client:", MessagePriority.High, false, cip.ID, m);
                    ((ConnectedClient) cip).alias = Shared.PopFirstListItem(m.messageParams);
                    //TEMP
                    //gcs.addPlayer(cip.ID);
                    AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                    break;

                case Message.Messages.JoinGame: //SERVER
                    if (ReceiverIsClient)
                    {
                        error = true;
                        errormsg = inappropriateMessage;
                        break;
                    }
                    var cipc = cip as ConnectedClient;

                    //get the sector the player wants to join
                    long secid = Int64.Parse(Shared.PopFirstListItem(m.messageParams));

                    var sec = gcs.gameRegion.getArea(secid) as Sector;
                    if (sec == null)
                    {
                        error = true;
                        errormsg = "Could not find sector player wants to join with";
                        break;
                    }

                    //get all the planes the player is joining with, give them all ids and instance owners
                    PlayerShipClass psc = PlayerShipClass.DeserialiseCreate(m.messageParams, gcs.gameRegion,
                                                                            cip.ID, faction, sec.thismap);
                    //give them all positions
                    ActionList.AddJoinGameToAction(psc);

                    var mapv = psc.PlayerShip.parentArea as Map;

                    //send sector to client
                    sec.Init(cipc);
                    //gcs.gameRegion.SynchInfo[cipc.ID].CanSee.Value = true;

                    SynchStatusConnectedClient sy = ConnectedClient.SynchInfo[cipc.ID];
                    sy.TryForPostACK.Value = true;
                    sy.JoinPSC = psc;
                    sy.JoinMap = mapv;

                    //force synchronisation of ships to client now
                    //cipc.Synchronise(cipc);
                    break;

                case Message.Messages.JoinGameACK: //CLIENT

                    if (ReceiverIsClient == false)
                    {
                        error = true;
                        errormsg = inappropriateMessage;
                        break;
                    }

                    PlayerShipClass.DeserialiseCreateLight(m.messageParams, GameControlClient.playerShipClass,
                                                           gcs.gameRegion, cip);

                    GameControlClient.PostRequestJoinGameSector();

                    Manager.FireLogEvent("completed post join activities:" + mps, MessagePriority.High, false, cip.ID);
                    break;

                case Message.Messages.SendingVars:
                    //first arg is the type of send var
                    var svc =
                        (Message.sendVars)
                        Enum.Parse(typeof (Message.sendVars), Shared.PopFirstListItem(m.messageParams));

                    switch (svc)
                    {
                            //remove ship
                        case Message.sendVars.ShipRemove:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }
                            Vint1 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));
                            ShipInstance sh1 = gcs.gameRegion.getShipInstance(Vint1);
                            if (sh1 == null)
                            {
                                error = true;
                                errormsg = "remove ship found no match";
                                break;
                            }
                            bool destroyed = Boolean.Parse(Shared.PopFirstListItem(m.messageParams));

                            GameControlClient.ResetShip(sh1, gcs.gameRegion, destroyed);
                            break;

                        case Message.sendVars.ShotRemove:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            Vint1 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));
                            WeaponInstance sh2 = gcs.gameRegion.getShotInstance(Vint1);
                            if (sh2 == null)
                            {
                                error = true;
                                errormsg = "remove shot found no match";
                                break;
                            }

                            var mm = sh2.parentArea as Map;
                            if (mm == null)
                            {
                                error = true;
                                errormsg = "parentarea is not map";
                                break;
                            }

                            mm.RemoveShot(sh2);

                            Manager.FireLogEvent("Removed Shot:" + mps, MessagePriority.High, false, cip.ID, null);
                            break;

                        case Message.sendVars.BuildingRemove:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }
                            Vint1 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));
                            BuildingInstance sh3 = gcs.gameRegion.getBuildingInstance(Vint1);
                            if (sh3 == null)
                            {
                                error = true;
                                errormsg = "remove ship found no match";
                                break;
                            }
                            IShipAreaMIXIN.RemoveBuildingMixIn(sh3.parentArea, sh3);
                            Manager.FireLogEvent("Removed building:" + mps, MessagePriority.High, false, cip.ID, null);
                            break;

                        case Message.sendVars.GiveID:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            Vint1 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));
                            myID = Vint1;
                            Vint2 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));
                            myFaction = Vint2;
                            AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                        case Message.sendVars.UpdateShipPosition:

                            int testowner = -1;
                            int testfaction = -1;
                            if (ReceiverIsClient == false)
                            {
                                testowner = cip.ID;
                                testfaction = faction;
                            }

                            RC = ShipInstance.DeserialiseUpdatePosition(m.messageParams, gcs.gameRegion, testowner,
                                                                        testfaction);
                            Manager.FireLogEvent("ship update return:" + RC.ToString() + mps, MessagePriority.Low,
                                                 RC != returncodes.S_OK, cip.ID, null);
                            break;

                        case Message.sendVars.UpdateShipStats:

                            ShipInstance.DeserialiseUpdateStats(m.messageParams, gcs.gameRegion);
                            Manager.FireLogEvent("Updated Ship stats:" + mps, MessagePriority.Low, false, cip.ID, null);
                            break;

                        case Message.sendVars.UpdateShotPosition:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            WeaponInstance.DeserialiseUpdate(m.messageParams, gcs.gameRegion);
                            Manager.FireLogEvent("Updated shot position:" + mps, MessagePriority.Low, false, cip.ID,
                                                 null);
                            break;

                        case Message.sendVars.UpdateBuilding:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }
                            BuildingInstance.DeserialiseUpdate(m.messageParams, gcs.gameRegion);
                            Manager.FireLogEvent("Updated building:" + mps, MessagePriority.Low, false, cip.ID, null);
                            break;

                        case Message.sendVars.CreateRegion:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            if (Region.DeserialiseCreate(m.messageParams, gcs) == false)
                                break;

                            Manager.FireLogEvent("Created Region:" + mps, MessagePriority.High, false, cip.ID, null);
                            AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                        case Message.sendVars.CreateShip:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            if (ShipInstance.DeserialiseCreate(m.messageParams, gcs.gameRegion) == null)
                                break;

                            Manager.FireLogEvent("Created Ship:" + mps, MessagePriority.High, false, cip.ID, null);
                            AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                        case Message.sendVars.CreateBuilding:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            if (BuildingInstance.DeserialiseCreate(m.messageParams, gcs.gameRegion) == null)
                                break;

                            Manager.FireLogEvent("Created Building:" + mps, MessagePriority.High, false, cip.ID, null);
                            AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                        case Message.sendVars.RequestShotCreate:
                            if (ReceiverIsClient)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            RC = WeaponInstance.DeserialiseShotCreationRequest(m.messageParams, gcs.gameRegion);
                            if (RC == returncodes.S_OK)
                                Manager.FireLogEvent("Created Shot:" + mps, MessagePriority.Low, false, cip.ID, null);
                            else
                                Manager.FireLogEvent("weapon create failed:" + mps, MessagePriority.Low, true, cip.ID,
                                                     null);

                            break;

                        case Message.sendVars.CreateShot:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            if (WeaponInstance.DeserialiseCreate(m.messageParams, gcs.gameRegion) == false)
                            {
                                Manager.FireLogEvent("weapon create failed:" + mps, MessagePriority.Low, true, cip.ID,
                                                     null);
                                break;
                            }

                            Manager.FireLogEvent("Created Shot:" + mps, MessagePriority.Low, false, cip.ID, null);
                            //AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                        case Message.sendVars.UpdateTerrain:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            RC = Map.DeserialiseUpdateTerrain(m.messageParams, gcs.gameRegion);

                            Manager.FireLogEvent("update terrain:" + RC.ToString() + mps, MessagePriority.Low,
                                                 RC != returncodes.S_OK, cip.ID, null);
                            break;

                        case Message.sendVars.ShipAreaUpdate:

                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            if (ShipInstance.DeserialiseSendShipArea(m.messageParams, gcs.gameRegion) == false)
                                break;

                            Manager.FireLogEvent("Adding ship to area:" + mps, MessagePriority.Low, false, cip.ID, null);
                            AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                        case Message.sendVars.CreateSectorMap:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            Sector s1 = Sector.DeserialiseCreate(m.messageParams, gcs, cip, false);
                            if (s1 == null)
                                break;

                            Manager.FireLogEvent("client:" + cip.ID + " sent create map", MessagePriority.High, false,
                                                 cip.ID, null);
                            AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                        case Message.sendVars.RequestGameSectorCreation:
                            if (ReceiverIsClient)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }

                            //temp - just authorise all game sector creation for now
                            SectorConfig cfg = SectorConfig.DeserialiseCreate(m.messageParams);
                            gcs.gameRegion.addSector(cfg);
                            Manager.FireLogEvent("Created requested game mode" + mps, MessagePriority.High, false,
                                                 cip.ID, null);
                            break;

                        case Message.sendVars.GiveCredits:
                            if (ReceiverIsClient == false)
                            {
                                error = true;
                                errormsg = inappropriateMessage;
                                break;
                            }
                            Vint1 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));

                            Vint2 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));
                            sh1 = gcs.gameRegion.getShipInstance(Vint2);

                            if (sh1 == null)
                            {
                                Manager.FireLogEvent("Error getting planes to give credit to", MessagePriority.Low, true,
                                                     myID);
                            }
                            else
                            {
                                gcs.GiveCreditForShipDestruction(Vint1, sh1);
                            }

                            Manager.FireLogEvent("Got credit for ship destruction:" + mps, MessagePriority.High, false,
                                                 cip.ID);
                            break;


                        case Message.sendVars.ClientControlShip:
                            bool ok = false;

                            Vint1 = Int32.Parse(Shared.PopFirstListItem(m.messageParams));
                            sh = gcs.gameRegion.getShipInstance(Vint1);
                            if (sh == null)
                            {
                                Manager.FireLogEvent("ship not found:" + mps, MessagePriority.Low, true, cip.ID);
                            }
                            else
                            {
                                if (sh.instanceOwner.PlayerCanControl(cip.ID, faction) == false)
                                    sh = null;

                                if (sh != null)
                                {
                                    ok = sh.instanceOwner.ChangeControl(cip.ID, faction);
                                }

                                if (sh == null || ok == false)
                                {
                                    Manager.FireLogEvent("error changing ship:" + mps, MessagePriority.High, true,
                                                         cip.ID);
                                }
                                else
                                {
                                    Manager.FireLogEvent("player changed ship:" + mps, MessagePriority.High, false,
                                                         cip.ID);
                                }
                            }

                            //AddMessageToWriteBuffer(Message.CreateOKMessage(m), cip);
                            break;

                            /*
                    case Message.sendVars.EjectShipFromHangarPost:
                        Vint1 = int.Parse(Shared.PopFirstListItem(m.messageParams));
                        sh = gcs.gameRegion.getShipInstance(Vint1);
                        if (sh.instanceOwner.PlayerCanControl(cip.ID, faction) == false)
                            sh = null;

                        if (sh != null)
                        {
                            ActionList.AddReleaseShipFromBuilding(sh);
                            Manager.FireLogEvent(cip.ID, "added player eject ship request:" + mps, null, messagePriority.High, false);
                        }
                        else
                        {
                            Manager.FireLogEvent(cip.ID, "error ejecting ship:" + mps, null, messagePriority.High, true);
                        }
                        break;

                    default:
                        {
                            Manager.FireLogEvent(cip.ID, "Out Of Range Message", m, messagePriority.High, true);
                            //   throw new ArgumentOutOfRangeException();
                            break;
                        }
                         * */
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (error)
            {
                Manager.FireLogEvent("Error:" + errormsg + ":IsClient:" + ReceiverIsClient.ToString(),
                                     MessagePriority.High, true,
                                     -1, m);
                return false;
            }
            return true;
        }
    }
}