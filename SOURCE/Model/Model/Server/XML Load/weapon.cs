using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Project.Model.Server.XML_Load;

namespace Project
{
    public class weapon :  IPurchasable
    {
        #region ILoadXMLBase Members

        public String name { get; set; }

        #endregion

        #region IPurchasable Members

        [XmlIgnore]
        public int Cost { get; set; }

        #endregion

        [ContentSerializerAttribute(Optional = true)] [XmlIgnore] public float AngleOfFire;
        [XmlIgnore] [ContentSerializerIgnore] public SpriteDraw BaseSprite;
        [ContentSerializer(Optional = true)] [XmlIgnore] public float BeamRadius;
        //<
        //time between shots
        [XmlIgnore] public int CoolDown;
        [XmlIgnore] public double Damage;
        [XmlIgnore] public int EnergyPerShot;
        [ContentSerializerAttribute(Optional = true)] [XmlIgnore] public double ExplosiveRadius;
        [ContentSerializerAttribute(Optional = true)] [XmlIgnore] public bool IsBeam;
        [XmlIgnore]
        [ContentSerializerAttribute(Optional = true)]
        public string ParticleName;
        [XmlIgnore] public string SlotPictureName;

        [XmlIgnore] [ContentSerializerIgnore] public SpriteDraw SlotPictureTexture;
        [ContentSerializerAttribute(Optional = true)]
        [XmlIgnore]
        public string SpriteName;
        [XmlIgnore] public int Velocity;

       

        //force the angle of fire to not be look - eg bombs that fall
    }
}