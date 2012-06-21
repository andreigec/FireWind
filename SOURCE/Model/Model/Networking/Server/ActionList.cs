using System;
using System.Collections.Generic;
using Project.Model.Instances;
using Project.Networking;

namespace Project.Model.Networking.Server
{
    /// <summary>
    /// List of items that the server will continue trying to complete until all preconditions are met
    /// </summary>
    public static class ActionList
    {
        private static readonly List<Tuple<actionListEnum, object[]>> actionList;

        static ActionList()
        {
            actionList = new List<Tuple<actionListEnum, object[]>>();
        }

        private static void SynchActionList()
        {
            for (var a2 = 0; a2 < actionList.Count; a2++)
            {
                var a = actionList[a2];

                switch (a.Item1)
                {
                    case actionListEnum.ReleaseShipFromBuilding:
                        var si = a.Item2[0] as ShipInstance;
                        var bi = a.Item2[1] as BuildingInstance;
                        var m = si.parentArea as map;

                        if (bi == null)
                        {
                            //get hangar to eject ship from 
                            bi = m.getSuitableHangarShipRelease();
                        }

                        if (bi == null)
                        {
                            Manager.FireLogEvent("Error getting building for action building ship release",
                                             SynchMain.MessagePriority.Low, true);
                            return;
                        }

                        var ok = si.ChangeArea(m);

                        if (ok == false)
                            Manager.FireLogEvent("error releasing ship from hangar",
                                             SynchMain.MessagePriority.High, true);
                        else
                        {
                            ShipInstance.CreateBuildingExitVector(bi, si);
                            actionList.RemoveAt(a2);
                            Manager.FireLogEvent("ejected ship from hangar", SynchMain.MessagePriority.Low,
                                             false);
                        }
                        continue;

                    case actionListEnum.AddJoinPSC:
                        //get ship
                        var ok2 = true;
                        var psc = a.Item2[0] as PlayerShipClass;

                        if (psc == null)
                        {
                            ok2 = false;
                        }

                        if (ok2)
                            ok2 = ShipJoinMap(psc.PlayerShip);

                        if (ok2)
                        {
                            foreach (var sc in psc.SupportCraft)
                                ShipJoinMap(sc);
                        }

                        if (ok2 == false)
                        {
                            Manager.FireLogEvent("error joining game with player ship",
                                             SynchMain.MessagePriority.High, true);
                        }
                        else
                            actionList.RemoveAt(a2);

                        break;
                    case actionListEnum.AddJoinPS:

                        //get ship
                        var ok3 = true;
                        si = a.Item2[0] as ShipInstance;

                        if (si == null)
                        {
                            ok3 = false;
                        }

                        if (ok3)
                            ok3 = ShipJoinMap(si);

                        if (ok3 == false)
                        {
                            Manager.FireLogEvent("error joining game with player ship",
                                             SynchMain.MessagePriority.High, true);
                        }
                        else
                            actionList.RemoveAt(a2);


                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static bool ShipJoinMap(ShipInstance si)
        {
            var m = si.parentArea as map;
            if (m == null)
                return false;
            //get good entrance vector
            var vm = m.getSuitableJoinGameVector(si);
            if (vm != null)
            {
                var ok = si.ChangeArea(m);

                if (ok == false)
                {
                    return false;
                }

                si.spriteInstance.move = new VectorMove();
                si.spriteInstance.move = vm;
                si.spriteInstance.LookAngle = vm.Angle;
            }
            return true;
        }

        /*
        public static void AddReleaseShipFromBuilding(ShipInstance si, BuildingInstance bi = null)
        {
            if (si.parentArea is map == false)
            {
                Manager.FireLogEvent(Manager.synchMain.myID, "Error, release ship building action needs map parentarea", null, SynchMain.messagePriority.High, true);
                return;
            }

            var olist = new object[2];
            olist[0] = si;
            olist[1] = bi;

            actionList.Add(new Tuple<actionListEnum, object[]>(actionListEnum.ReleaseShipFromBuilding, olist));
            SynchActionList();
        }
        */

        public static void AddJoinGameToAction(PlayerShipClass si)
        {
            var olist = new object[1];
            olist[0] = si;
            actionList.Add(new Tuple<actionListEnum, object[]>(actionListEnum.AddJoinPSC, olist));
            SynchActionList();
        }

        public static void AddJoinGameToAction(ShipInstance si)
        {
            var olist = new object[1];
            olist[0] = si;
            actionList.Add(new Tuple<actionListEnum, object[]>(actionListEnum.AddJoinPS, olist));
            SynchActionList();
        }

        #region Nested type: actionListEnum

        private enum actionListEnum
        {
            ReleaseShipFromBuilding,
            AddJoinPSC,
            AddJoinPS
        }

        #endregion
    }
}