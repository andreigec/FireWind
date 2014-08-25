using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Model.Instances
{
    public class ShipInstanceShell : ShipBaseItems
    {
        private ShipInstanceShell()
        {
            Parts = new SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart>();
            Slots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weapon>();
        }

        #region ShipBaseItems Members

        public SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart> Parts { get; set; }

        public SerializableDictionary<SlotLocation.SlotLocationEnum, weapon> Slots { get; set; }

        public shipBase shipModel { get; set; }

        #endregion

        public static void AssignPart(ShipInstanceShell shell, ShipPart addthis)
        {
            SlotLocation.SlotLocationEnum sp = addthis.ShipPartToEnum();

            if (shell.Parts.ContainsKey(sp))
                shell.Parts[sp] = null;

            shell.Parts[sp] = addthis;
        }

        public static ShipInstanceShell CreateStockShellShip()
        {
            var sis = new ShipInstanceShell();
            //stock parts have stock in the name

            foreach (var p in loadXML.loadedShipParts.Where(s => s.Value.name.StartsWith("Stock ")))
            {
                AssignPart(sis, p.Value);
            }

            sis.Slots.Add(SlotLocation.SlotLocationEnum.LeftWeapon, loadXML.loadedWeapons["WEAPON_GAUSS"]);

            sis.shipModel = GetCostBase(true);

            return sis;
        }

        public static ShipInstanceShell CreateShellShipFromCost(int cost)
        {
            //equipped parts and slots - start with stock
            ShipInstanceShell sis = CreateStockShellShip();

            //create lists of items to cost and sort by cost
            List<Tuple<double, shipBase>> basecostlist =
                loadXML.loadedShips.Select(b => new Tuple<double, shipBase>(b.Value.Cost, b.Value)).ToList();
            basecostlist.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));

            List<Tuple<double, ShipPart>> partscostlist =
                loadXML.loadedShipParts.Select(b => new Tuple<double, ShipPart>(b.Value.Cost, b.Value)).ToList();
            partscostlist.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));

            List<Tuple<double, weapon>> weaponscostlist =
                loadXML.loadedWeapons.Select(b => new Tuple<double, weapon>(b.Value.Cost, b.Value)).ToList();
            weaponscostlist.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));


            //num of times to try to upgrade and fail before exit loop
            int failcount = 0;
            const int failmax = 5;

            while (failcount < failmax)
            {
                int totalCurrentCost = ShipBaseItemsMIXIN.GetTotalItemsCost(sis);
                int dif = cost - totalCurrentCost;
                //this should never occur-over budget, so just create
                if (dif <= 0)
                    break;

                //equal chance to upgrade weapon part or base
                int rand = Manager.r.Next() % 3;
                switch (rand)
                {
                    //base
                    case 0:

                        int currbasecost = sis.shipModel.Cost;
                        //since we are replacing the part, we can actually spend the current difference + the part cost
                        int allowcost = dif + currbasecost;
                        //choose a more expensive part randomly
                        IEnumerable<Tuple<double, shipBase>> cl =
                            basecostlist.Where(s => s.Item1 > currbasecost && s.Item1 <= allowcost);

                        if (cl.Count() == 0)
                        {
                            failcount++;
                            continue;
                        }

                        int r = Manager.r.Next() % cl.Count();

                        Tuple<double, shipBase> i = cl.ElementAt(r);

                        sis.shipModel = i.Item2;
                        failcount = 0;
                        break;

                    //weapon
                    case 1:
                        //random between left and right slot
                        r = Manager.r.Next() % 2;

                        var sl = SlotLocation.SlotLocationEnum.None;
                        if (r == 0)
                            sl = SlotLocation.SlotLocationEnum.LeftWeapon;
                        else if (r == 1)
                            sl = SlotLocation.SlotLocationEnum.RightWeapon;

                        allowcost = dif;
                        if (sis.Slots.ContainsKey(sl))
                            allowcost += sis.Slots[sl].Cost;

                        currbasecost = 0;
                        if (sis.Slots.ContainsKey(sl))
                            currbasecost = sis.Slots[sl].Cost;

                        //choose a more expensive part randomly
                        IEnumerable<Tuple<double, weapon>> wl =
                            weaponscostlist.Where(s => s.Item1 > currbasecost && s.Item1 <= allowcost);

                        if (wl.Count() == 0)
                        {
                            failcount++;
                            continue;
                        }

                        r = Manager.r.Next() % wl.Count();

                        Tuple<double, weapon> i2 = wl.ElementAt(r);

                        sis.Slots[sl] = i2.Item2;
                        failcount = 0;
                        break;

                    //part
                    case 2:

                        //random between parts
                        r = Manager.r.Next() % sis.Parts.Count;
                        sl = sis.Parts.ElementAt(r).Key;

                        allowcost = dif + sis.Parts[sl].Cost;

                        currbasecost = 0;
                        if (sis.Parts.ContainsKey(sl))
                            currbasecost = sis.Parts[sl].Cost;

                        //choose a more expensive part randomly
                        IEnumerable<Tuple<double, ShipPart>> pl =
                            partscostlist.Where(s => s.Item1 > currbasecost && s.Item1 <= allowcost);

                        if (pl.Count() == 0)
                        {
                            failcount++;
                            continue;
                        }

                        r = Manager.r.Next() % pl.Count();

                        Tuple<double, ShipPart> i3 = pl.ElementAt(r);

                        sis.Parts[sl] = i3.Item2;
                        failcount = 0;
                        break;
                }
            }

            return sis;
        }

        public static weapon GetCostWeapon(bool FindMin, weapon excluding = null)
        {
            IOrderedEnumerable<KeyValuePair<string, weapon>> ll;
            if (FindMin)
                ll = loadXML.loadedWeapons.OrderBy(s => s.Value.Cost);
            else
                ll = loadXML.loadedWeapons.OrderByDescending(s => s.Value.Cost);

            //return the first item that isnt excluded
            foreach (var l in ll)
            {
                if (l.Value != excluding)
                    return l.Value;
            }
            //if no others, just return excluded
            return ll.First().Value;
        }


        public static shipBase GetCostBase(bool FindMin, shipBase excluding = null)
        {
            IOrderedEnumerable<KeyValuePair<string, shipBase>> ll;
            if (FindMin)
                ll = loadXML.loadedShips.OrderBy(s => s.Value.Cost);
            else
                ll = loadXML.loadedShips.OrderByDescending(s => s.Value.Cost);

            //return the first item that isnt excluded
            foreach (var l in ll)
            {
                if (l.Value != excluding)
                    return l.Value;
            }
            //if no others, just return excluded
            return ll.First().Value;
        }

        public static ShipPart GetCostPart(bool FindMin, SlotLocation.SlotLocationEnum type, ShipPart excluding = null)
        {
            IOrderedEnumerable<KeyValuePair<string, ShipPart>> ll;
            if (FindMin)
                ll = loadXML.loadedShipParts.Where(s1 => s1.Value.ShipPartToEnum() == type).OrderBy(s => s.Value.Cost);
            else
                ll =
                    loadXML.loadedShipParts.Where(s1 => s1.Value.ShipPartToEnum() == type).OrderByDescending(
                        s => s.Value.Cost);

            //return the first item that isnt excluded
            foreach (var l in ll)
            {
                if (l.Value != excluding)
                    return l.Value;
            }
            //if no others, just return excluded
            return ll.First().Value;
        }

        public static double GetMinCostShip()
        {
            var sis = CreateStockShellShip();
            return ShipBaseItemsMIXIN.GetTotalItemsCost(sis);
        }

        public static double GetMaxCostShip()
        {
            var sis = new ShipInstanceShell();

            sis.Slots.Add(SlotLocation.SlotLocationEnum.LeftWeapon, GetCostWeapon(false));
            sis.Slots.Add(SlotLocation.SlotLocationEnum.RightWeapon,
                          GetCostWeapon(false));

            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipBatteryPart,
                          GetCostPart(false, SlotLocation.SlotLocationEnum.ShipBatteryPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipBoosterPart,
                          GetCostPart(false, SlotLocation.SlotLocationEnum.ShipBoosterPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipEnginePart,
                          GetCostPart(false, SlotLocation.SlotLocationEnum.ShipEnginePart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipGeneratorPart,
                          GetCostPart(false, SlotLocation.SlotLocationEnum.ShipGeneratorPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipHullPart,
                          GetCostPart(false, SlotLocation.SlotLocationEnum.ShipHullPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipWingsPart,
                          GetCostPart(false, SlotLocation.SlotLocationEnum.ShipWingsPart));

            sis.shipModel = GetCostBase(false);

            return ShipBaseItemsMIXIN.GetTotalItemsCost(sis);
        }
    }
}