using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;
using Project.XML_Load;

namespace Project.Model.Instances
{
    public partial class BuildingInstance : SharedID, IDrawableObject
    {
        #region BuildingType enum

        public enum BuildingType
        {
            Civilian,
            ShipHangar
        }

        #endregion

        public const int falldistdmg = 10;

        public static float GravityMass = 1;
        public double armour;
        public building buildingModel;
        public BuildingType buildingType = BuildingType.Civilian;
        public int falldistance;
        public bool falling;

        public BuildingInstance()
        {
        }

        private BuildingInstance(GameControlServer gcs, building b, IshipAreaSynch m, VectorMove move, float lookAng,
                                 BuildingType btype,
                                 SetID cfg,
                                 InstanceOwner controller = null)
        {
            parentGCS = gcs;
            ISynchInterfaceMIXIN.InitClass(this);
            buildingModel = b;
            spriteInstance = new SpriteInstance(b.BaseSprite, m);
            if (move != null)
                spriteInstance.move = move.Clone();
            spriteInstance.LookAngle = lookAng;
            armour = b.MaxArmour;
            buildingType = btype;
            setID(cfg);
            if (controller != null)
                instanceOwner = controller;
            parentArea = m;
        }

        public InstanceOwner instanceOwner { get; set; }

        #region IDrawableObject Members

        [XmlIgnore]
        public GameControlServer parentGCS { get; set; }

        [XmlIgnore]
        public IshipAreaSynch parentArea { get; set; }

        public SpriteInstance spriteInstance { get; set; }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            spriteInstance.Draw(cam, gameTime);
        }

        #endregion

        public static BuildingInstance addBuilding(GameControlServer gcs, BuildingInstance oldBI, IshipAreaSynch m,
                                                   SetID cfg)
        {
            building bm = loadXML.loadedBuildings[oldBI.buildingModel.name];
            var bi = new BuildingInstance(gcs, bm, m, oldBI.spriteInstance.move, oldBI.spriteInstance.LookAngle,
                                          oldBI.buildingType, cfg,
                                          oldBI.instanceOwner);
            bi.armour = oldBI.armour;
            bi.parentArea = m;
            m.buildings.Add(bi);
            return bi;
        }

        public static BuildingInstance addBuilding(GameControlServer gcs, building b, IshipAreaSynch parent,
                                                   InstanceOwner controller,
                                                   VectorMove position,
                                                   BuildingType btype, SetID cfg)
        {
            var bi = new BuildingInstance(gcs, b, parent, position, 0, btype, cfg, controller);
            parent.buildings.Add(bi);
            bi.parentArea = parent;
            return bi;
        }

        public void Update(GameTime gameTime)
        {
            updateDestructAnimation();
            spriteInstance.enforceGravity(gameTime, this);

            spriteInstance.Update(gameTime);
        }

        private void updateDestructAnimation()
        {
            if (armour == buildingModel.MaxArmour)
                return;
            double p = armour/buildingModel.MaxArmour;

            foreach (var s in buildingModel.DestructFrames)
            {
                if (p <= s.Key && spriteInstance.currentanimation != s.Value)
                {
                    spriteInstance.changeCurrentAnimation(s.Value);
                    break;
                }
                else if (s.Key >= p)
                    return;
            }
        }
    }
}