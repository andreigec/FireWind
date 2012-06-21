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
    public partial class region : SharedID, IshipAreaSynch
    {
        public List<sector> sectors = new List<sector>();

        public int width { get; private set; }
        public int height { get; private set; }

        //public List<sector> HomePlanets = new List<sector>();

        [XmlIgnore]
        public List<WeaponInstance> shots { get; set; }

        [XmlIgnore]
        public List<BuildingInstance> buildings { get; set; }

        [XmlIgnore]
        public List<ShipInstance> ships { get; set; }

        [XmlIgnore]
        public GameControlServer parentGCS { get; set; }

        public region(GameControlServer parent, SetID cfg, int width = 200, int height = 200)
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
            foreach (var shi in ships)
            {
                if (shi.ID.Equals(ID))
                    return shi;
            }

            //assigned to a sector
            foreach (var sec in sectors)
            {
                foreach (var sh in sec.ships)
                {
                    if (sh.ID.Equals(ID))
                        return sh;
                }

                foreach (var sh in sec.thismap.ships)
                {
                    if (sh.ID.Equals(ID))
                        return sh;
                }
            }
            return null;
        }

        public List<ShipInstance> GetShipByOwner(long ownerID)
        {
            var ret = ships.Where(s => s.instanceOwner.PlayerOwnerID == ownerID).ToList();

            foreach (var sec in sectors)
            {
                foreach (var s1 in sec.ships.Where(s => s.instanceOwner.PlayerOwnerID == ownerID))
                {
                    ret.Add(s1);
                }

                foreach (var s1 in sec.thismap.ships.Where(s => s.instanceOwner.PlayerOwnerID == ownerID))
                {
                    ret.Add(s1);
                }
            }
            return ret;
        }

        public BuildingInstance getBuildingInstance(long buildID)
        {
            //assigned to a sector
            foreach (var sec in sectors)
            {
                foreach (var bi in sec.thismap.buildings)
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
            foreach (var sec in sectors)
            {
                foreach (var sh in sec.thismap.shots)
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
            foreach (var sec in sectors)
            {
                for (var a = 0; a < sec.thismap.shots.Count; a++)
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
            var s1 = sectors.Where(s => s.ID == areaID);
            var s2 = sectors.Where(s => s.thismap.ID == areaID);

            if (s1.Count() > 0)
                return s1.First();

            if (s2.Count() > 0)
                return s2.First().thismap;

            return null;
        }
        
        #endregion player
       
        public void addSector(sector sec)
        {
            var gcs = parentGCS;

            sec.parentRegion = gcs.gameRegion;
            sec.parentGCS = gcs;
            sec.thismap.parentSector = sec;
            sec.thismap.parentGCS = gcs;

            gcs.gameRegion.sectors.Add(sec);
        }

        public sector addSector(SectorConfig cfg)
        {
            var s = sector.addSector(cfg, this);
            sectors.Add(s);

            //add ships depending on config
            if (cfg is SectorConfigGambleMatch)
            {
                var gg = cfg as SectorConfigGambleMatch;

                for (int a=0;a<gg.shipcount;a++)
                {
                    var sish = ShipInstanceShell.CreateShellShipFromCost(gg.shipcost);

                    var si = ShipInstance.addShip(sish, SetID.CreateSetNew(),s.thismap);
                    si.instanceOwner = new InstanceOwner(-1, -1, InstanceOwner.ControlType.NoPlayer, -1);
                    ActionList.AddJoinGameToAction(si);
                }
            }
            return s;
        }

        public void RemoveSector(sector s)
        {
            //remove all player ships
            var ps = s.thismap.ships.Where(sh => sh.instanceOwner.PlayerOwnerID != -1);

            while (ps.Count()>0)
            {
                var ship = ps.First();
                GameControlClient.ResetShip(ship,this,false);
            }

            sectors.Remove(s);
        }

        public void Update(GameTime gameTime)
        {
            for (int a = 0; a < sectors.Count;a++ )
            {
                var s = sectors[a];

                    //see if the map has been completed
                    if (false)//s.Config.SectorComplete(s))
                    {
                        RemoveSector(s);
                        continue;
                    }

                    s.Update(gameTime);
                }
        }
    }
}