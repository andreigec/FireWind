using System;
using System.Collections.Generic;
using System.Linq;

namespace Project
{
    public abstract partial class SectorConfig
    {
        #region SectorType enum

        public enum SectorType
        {
            Colosseum,GambleMatch
        }

        #endregion

        #region Size enum

        public enum Size
        {
            small,
            medium,
            large
        }

        #endregion

        private readonly Dictionary<Size, Tuple<int, int>> sizes = new Dictionary<Size, Tuple<int, int>>();

        public Size size;

        public SectorConfig()
        {
            sizes.Add(Size.small, new Tuple<int, int>(5000, 5000));
            sizes.Add(Size.medium, new Tuple<int, int>(10000, 5000));
            sizes.Add(Size.large, new Tuple<int, int>(20000, 5000));
        }

        public abstract bool SectorComplete(sector s);

        public int GetWidth()
        {
            return sizes[size].Item1;
        }

        public int GetHeight()
        {
            return sizes[size].Item2;
        }

        public bool HasBuildings()
        {
            return true;
        }

        public abstract override String ToString();

        public static SectorConfig CreateConfigMission()
        {
            var ret = new SectorConfigMission();
            return ret;
        }

        public static SectorConfig CreateConfigColosseum(Size s)
        {
            var ret = new SectorConfigColosseum(s);
            return ret;
        }

        public static SectorConfig CreateConfigGambleMatch(int shipcount,int shipcost)
        {
            var ret = new SectorConfigGambleMatch(Size.small,shipcount,shipcost);
            return ret;
        }
    }

    public partial class SectorConfigMission : SectorConfig
    {
        public override string ToString()
        {
            return "Mission";
        }

        public override bool SectorComplete(sector s)
        {
            return false;
        }
    }

    public partial class SectorConfigColosseum : SectorConfig
    {
        public SectorConfigColosseum(Size newsize)
        {
            size = newsize;
        }

        public override bool SectorComplete(sector s)
        {
            return false;
        }

        public override string ToString()
        {
            return "Colosseum";
        }
    }

    public partial class SectorConfigGambleMatch : SectorConfig
    {
        public int shipcount { get; private set; }
        public int shipcost { get; private set; }

        public SectorConfigGambleMatch(Size newsize, int shipcount, int shipcost)
        {
            size = newsize;
            this.shipcost = shipcost;
            this.shipcount = shipcount;
        }

        public override bool SectorComplete(sector s)
        {
            //map is complete if:
            //-contains player ship
            var firstplayership = s.thismap.ships.Find(sh => sh.instanceOwner.PlayerOwnerID != -1);
            if (firstplayership==null)
                return false;

            //-no ships with different factions than the first player ship
            var shl =
                s.thismap.ships.Where(sh => sh.instanceOwner.FactionOwner != firstplayership.instanceOwner.FactionOwner);

            if (shl.Count() == 0)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Gamble Match";
        }
    }
}