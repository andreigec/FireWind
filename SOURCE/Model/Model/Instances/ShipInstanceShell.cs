using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Networking;

namespace Project.Model.Instances
{
    public class ShipInstanceShell : ShipBaseItems
    {
        public SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart> Parts { get; set; }

        public SerializableDictionary<SlotLocation.SlotLocationEnum, weapon> Slots { get; set; }

        public shipBase shipModel { get; set; }

        private ShipInstanceShell()
        {

        }

        public static void AssignPart(ShipInstanceShell shell,ShipPart addthis)
        {
            var sp = addthis.ShipPartToEnum();

            if (shell.Parts.ContainsKey(sp))
                shell.Parts[sp] = null;

            shell.Parts[sp] = addthis;
        }

        public static ShipInstanceShell CreateStockShellShip()
        {
            var sis = new ShipInstanceShell();
            //stock parts have stock in the name

            sis.Parts = new SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart>();
            foreach (var p in loadXML.loadedShipParts.Where(s => s.Value.name.StartsWith("Stock ")))
            {
                AssignPart(sis, p.Value);
            }

            sis.Slots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weapon>();
            sis.Slots.Add(SlotLocation.SlotLocationEnum.LeftWeapon, loadXML.loadedWeapons["WEAPON_GAUSS"]);

            sis.shipModel = GetCostBase(true);
            
            return sis;
        }

        public static ShipInstanceShell CreateShellShipFromCost(int cost)
        {
            //equipped parts and slots - start with stock
            var sis = CreateStockShellShip();

            //create lists of items to cost and sort by cost
            var basecostlist = loadXML.loadedShips.Select(b => new Tuple<double, shipBase>(b.Value.Cost, b.Value)).ToList();
            basecostlist.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));

            var partscostlist =loadXML.loadedShipParts.Select(b => new Tuple<double, ShipPart>(b.Value.Cost, b.Value)).ToList();
            partscostlist.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));

            var weaponscostlist = loadXML.loadedWeapons.Select(b => new Tuple<double, weapon>(b.Value.Cost, b.Value)).ToList();
            weaponscostlist.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));


            //num of times to try to upgrade and fail before exit loop
            int failcount = 0;
            const int failmax = 5;
            
            while (failcount<failmax)
            {
                int rand =  Manager.r.Next()%2;

                var totalCurrentCost = ShipBaseItemsMIXIN.GetTotalItemsCost(sis);
                var dif = cost - totalCurrentCost;
                //this should never occur-over budget, so just create
                if (dif <= 0)
                    break;

                //equal chance to upgrade weapon part or base
                bool fail = false;
                switch (rand)
                {
                        //base
                    case 0:

                        var currbasecost = sis.shipModel.Cost;
                        //since we are replacing the part, we can actually spend the current difference + the part cost
                        var allowcost = dif + currbasecost;
                        //choose a more expensive part randomly
                        var cl=basecostlist.Where(s => s.Item1 > currbasecost && s.Item1 <= allowcost);

                        if (cl.Count()==0)
                        {
                            failcount++;
                            continue;
                        }

                        int r = Manager.r.Next()%cl.Count();

                        var i = cl.ElementAt(r);

                        sis.shipModel = i.Item2;
                        failcount = 0;
                        break;

                        //weapon
                    case 1:
                        //random between left and right slot
                        r = Manager.r.Next()%2;
                        
                        SlotLocation.SlotLocationEnum sl=SlotLocation.SlotLocationEnum.None;
                        if (r == 0)
                            sl = SlotLocation.SlotLocationEnum.LeftWeapon;
                        else if (r == 1)
                            sl = SlotLocation.SlotLocationEnum.RightWeapon;

                        if (sl==SlotLocation.SlotLocationEnum.None)
                        {
                            failcount++;
                            continue;
                        }

                        allowcost = dif;
                        if (sis.Slots.ContainsKey(sl))
                            allowcost += sis.Slots[sl].Cost;

                        currbasecost = sis.Slots[sl].Cost;

                        //choose a more expensive part randomly
                        var wl = weaponscostlist.Where(s => s.Item1 > currbasecost && s.Item1 <= allowcost);

                        if (wl.Count() == 0)
                        {
                            failcount++;
                            continue;
                        }

                        r = Manager.r.Next()%wl.Count();

                        var i2 = wl.ElementAt(r);

                        sis.Slots[sl] = i2.Item2;
                        failcount = 0;
                        break;

                        //part
                    case 2:

                        //random between parts
                        r = Manager.r.Next() % sis.Parts.Count;
                        sl = sis.Parts.ElementAt(r).Key;
                        
                        allowcost = dif+ sis.Parts[sl].Cost;

                        currbasecost = sis.Parts[sl].Cost;

                        //choose a more expensive part randomly
                        var pl = partscostlist.Where(s => s.Item1 > currbasecost && s.Item1 <= allowcost);

                        if (pl.Count() == 0)
                        {
                            failcount++;
                            continue;
                        }

                        r = Manager.r.Next() % pl.Count();

                        var i3 = pl.ElementAt(r);

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
                ll = loadXML.loadedShipParts.Where(s1 => s1.Value.ShipPartToEnum() == type).OrderByDescending(s => s.Value.Cost);

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
            sis.Slots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weapon>();
            sis.Parts = new SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart>();

            sis.Slots.Add(SlotLocation.SlotLocationEnum.LeftWeapon, GetCostWeapon(false));
            sis.Slots.Add(SlotLocation.SlotLocationEnum.RightWeapon, GetCostWeapon(true, sis.Slots[SlotLocation.SlotLocationEnum.LeftWeapon]));

            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipBatteryPart, GetCostPart(false, SlotLocation.SlotLocationEnum.ShipBatteryPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipBoosterPart, GetCostPart(false, SlotLocation.SlotLocationEnum.ShipBoosterPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipEnginePart, GetCostPart(false, SlotLocation.SlotLocationEnum.ShipEnginePart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipGeneratorPart, GetCostPart(false, SlotLocation.SlotLocationEnum.ShipGeneratorPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipHullPart, GetCostPart(false, SlotLocation.SlotLocationEnum.ShipHullPart));
            sis.Parts.Add(SlotLocation.SlotLocationEnum.ShipWingsPart, GetCostPart(false, SlotLocation.SlotLocationEnum.ShipWingsPart));

            sis.shipModel = GetCostBase(false);

            return ShipBaseItemsMIXIN.GetTotalItemsCost(sis);
        }

    }
}
