using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Model.Instances;
using Project.Model.mapInfo;
using Project.Networking;

//using Project.Cameras;

namespace Project
{
    public partial class Sector : SharedID, IshipAreaSynch
    {
        public SectorConfig Config;
        [XmlIgnore] public long parentID = -1;
        [XmlIgnore] public Region parentRegion;
        public Map thismap;

        private Sector()
        {
        }

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

        private static Sector CreateSector(GameControlServer gcs, int mapWidth, int mapHeight, SectorConfig config,
                                           int mapSeed,
                                           SetID secIDcfg, SetID mapIDcfg, Region parent = null)
        {
            var s = new Sector();
            IShipAreaMIXIN.InitClass(s);
            ISynchInterfaceMIXIN.InitClass(s);

            s.parentGCS = gcs;

            s.Config = config;
            s.parentRegion = parent;
            s.thismap = Map.CreateMap(s, mapWidth, mapHeight, config, mapSeed, mapIDcfg);
            s.setID(secIDcfg);

            return s;
        }


        public void SetInstanceOwnerIDs(long id, int faction)
        {
            parentID = ID;

            //ships
            foreach (ShipInstance s in ships)
            {
                s.instanceOwner = new InstanceOwner(id, faction, InstanceOwner.ControlType.JustOwner);
            }

            //buildings
            foreach (BuildingInstance b in thismap.buildings)
            {
                b.instanceOwner = new InstanceOwner(id, faction, InstanceOwner.ControlType.JustOwner);
            }
        }

        /// <summary>
        /// Create Homeworld
        /// </summary>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        public static Sector addSector(SectorConfig config, Region r)
        {
            int width = config.GetWidth();
            int height = config.GetHeight();
            Sector s = CreateSector(r.parentGCS, width, height, config, DateTime.Now.Millisecond, SetID.CreateSetNew(),
                                    SetID.CreateSetNew(), r);
            return s;
        }


        public void Update(GameTime gameTime)
        {
            thismap.Update(gameTime);
        }
    }
}