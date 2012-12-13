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
    public partial class sector : SharedID, IshipAreaSynch
    {
        public SectorConfig Config;
        [XmlIgnore] public long parentID = -1;
        [XmlIgnore] public region parentRegion;
        public map thismap;

        private sector()
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

        private static sector CreateSector(GameControlServer gcs, int mapWidth, int mapHeight, SectorConfig config, int mapSeed,
                                           SetID secIDcfg, SetID mapIDcfg, region parent = null)
        {
            var s = new sector();
            IShipAreaMIXIN.InitClass(s);
            ISynchInterfaceMIXIN.InitClass(s);

            s.parentGCS = gcs;

            s.Config = config;
            s.parentRegion = parent;
            s.thismap = map.CreateMap(s, mapWidth, mapHeight, config, mapSeed, mapIDcfg);
            s.setID(secIDcfg);

            return s;
        }


        public void SetInstanceOwnerIDs(long id, int faction)
        {
            parentID = ID;

            //ships
            foreach (var s in ships)
            {
                s.instanceOwner = new InstanceOwner(id, faction, InstanceOwner.ControlType.JustOwner);
            }

            //buildings
            foreach (var b in thismap.buildings)
            {
                b.instanceOwner = new InstanceOwner(id, faction, InstanceOwner.ControlType.JustOwner);
            }
        }

        /// <summary>
        /// Create Homeworld
        /// </summary>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        public static sector addSector(SectorConfig config, region r)
        {
            var width = config.GetWidth();
            var height = config.GetHeight();
            var s = CreateSector(r.parentGCS,width, height, config, DateTime.Now.Millisecond, SetID.CreateSetNew(),
                                    SetID.CreateSetNew(), r);
            return s;
        }


        public void Update(GameTime gameTime)
        {
            thismap.Update(gameTime);
        }
    }
}