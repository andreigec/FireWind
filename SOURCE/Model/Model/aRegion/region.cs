using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Model.Instances;
using Project.Model.Networking.Server;
using Project.Model.mapInfo;
using Project.Networking;

namespace Project
{
    public partial class Region : SharedID, IshipAreaSynch
    {
        public List<Sector> sectors = new List<Sector>();

        public Region(GameControlServer parent, SetID cfg, int width = 200, int height = 200)
        {
            IShipAreaMIXIN.InitClass(this);
            ISynchInterfaceMIXIN.InitClass(this);

            parentGCS = parent;
            this.width = width;
            this.height = height;

            setID(cfg);
        }

        #region player

        public ShipInstance getShipInstance(long ID)
        {
            foreach (ShipInstance shi in ships)
            {
                if (shi.ID.Equals(ID))
                    return shi;
            }

            //assigned to a sector
            foreach (Sector sec in sectors)
            {
                foreach (ShipInstance sh in sec.ships)
                {
                    if (sh.ID.Equals(ID))
                        return sh;
                }

                foreach (ShipInstance sh in sec.thismap.ships)
                {
                    if (sh.ID.Equals(ID))
                        return sh;
                }
            }
            return null;
        }

        public List<ShipInstance> GetShipByOwner(long ownerID)
        {
            List<ShipInstance> ret = ships.Where(s => s.instanceOwner.PlayerOwnerID == ownerID).ToList();

            foreach (Sector sec in sectors)
            {
                foreach (ShipInstance s1 in sec.ships.Where(s => s.instanceOwner.PlayerOwnerID == ownerID))
                {
                    ret.Add(s1);
                }

                foreach (ShipInstance s1 in sec.thismap.ships.Where(s => s.instanceOwner.PlayerOwnerID == ownerID))
                {
                    ret.Add(s1);
                }
            }
            return ret;
        }

        public BuildingInstance getBuildingInstance(long buildID)
        {
            //assigned to a sector
            foreach (Sector sec in sectors)
            {
                foreach (BuildingInstance bi in sec.thismap.buildings)
                {
                    if (bi.ID.Equals(buildID))
                        return bi;
                }
            }
            return null;
        }

        public WeaponInstance getShotInstance(long shotID)
        {
            //assigned to a sector
            foreach (Sector sec in sectors)
            {
                foreach (WeaponInstance sh in sec.thismap.shots)
                {
                    if (sh.ID.Equals(shotID))
                        return sh;
                }
            }
            return null;
        }

        public IEnumerable<WeaponInstance> GetPlayerShots(int playerID)
        {
            //assigned to a sector
            foreach (Sector sec in sectors)
            {
                for (int a = 0; a < sec.thismap.shots.Count; a++)
                {
                    if (sec.thismap.shots[a].parentID == playerID)
                        yield return sec.thismap.shots[a];
                }
            }
        }

        public IshipAreaSynch getArea(long areaID)
        {
            if (ID == areaID)
                return this;
            IEnumerable<Sector> s1 = sectors.Where(s => s.ID == areaID);
            IEnumerable<Sector> s2 = sectors.Where(s => s.thismap.ID == areaID);

            if (s1.Count() > 0)
                return s1.First();

            if (s2.Count() > 0)
                return s2.First().thismap;

            return null;
        }

        #endregion player

        public int width { get; private set; }
        public int height { get; private set; }

        //public List<sector> HomePlanets = new List<sector>();

        #region IshipAreaSynch Members

        [XmlIgnore]
        public List<WeaponInstance> shots { get; set; }

        [XmlIgnore]
        public List<BuildingInstance> buildings { get; set; }

        [XmlIgnore]
        public List<ShipInstance> ships { get; set; }

        [XmlIgnore]
        public GameControlServer parentGCS { get; set; }

        #endregion

        public void addSector(Sector sec)
        {
            GameControlServer gcs = parentGCS;

            sec.parentRegion = gcs.gameRegion;
            sec.parentGCS = gcs;
            sec.thismap.parentSector = sec;
            sec.thismap.parentGCS = gcs;

            gcs.gameRegion.sectors.Add(sec);
        }

        public Sector addSector(SectorConfig cfg)
        {
            Sector s = Sector.addSector(cfg, this);
            sectors.Add(s);

            //add ships depending on config
            if (cfg is SectorConfigGambleMatch)
            {
                var gg = cfg as SectorConfigGambleMatch;

                for (int a = 0; a < gg.shipcount; a++)
                {
                    ShipInstanceShell sish = ShipInstanceShell.CreateShellShipFromCost(gg.shipcost);

                    ShipInstance si = ShipInstance.addShip(sish, SetID.CreateSetNew(), s.thismap);
                    si.instanceOwner = new InstanceOwner(-1, -1, InstanceOwner.ControlType.NoPlayer, -1);
                    ActionList.AddJoinGameToAction(si);
                }
            }
            return s;
        }

        public void RemoveSector(Sector s)
        {
            //remove all player ships
            IEnumerable<ShipInstance> ps = s.thismap.ships.Where(sh => sh.instanceOwner.PlayerOwnerID != -1);

            while (ps.Count() > 0)
            {
                ShipInstance ship = ps.First();
                GameControlClient.ResetShip(ship, this, false);
            }

            sectors.Remove(s);
        }

        public void Update(GameTime gameTime)
        {
            for (int a = 0; a < sectors.Count; a++)
            {
                Sector s = sectors[a];

                //see if the map has been completed
                if (false) //s.Config.SectorComplete(s))
                {
                    RemoveSector(s);
                    continue;
                }

                s.Update(gameTime);
            }
        }
    }
}