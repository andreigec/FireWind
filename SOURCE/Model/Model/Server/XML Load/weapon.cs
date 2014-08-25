using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Project.Model.Server.XML_Load;

namespace Project
{
    public class weapon : IPurchasable
    {
        [ContentSerializer(Optional = true)] [XmlIgnore] public float AngleOfFire;
        [XmlIgnore] [ContentSerializerIgnore] public SpriteDraw BaseSprite;
        [ContentSerializer(Optional = true)] [XmlIgnore] public float BeamRadius;
        //<
        //time between shots
        [XmlIgnore] public int CoolDown;
        [XmlIgnore] public double Damage;
        [XmlIgnore] public int EnergyPerShot;
        [ContentSerializer(Optional = true)] [XmlIgnore] public double ExplosiveRadius;
        [ContentSerializer(Optional = true)] [XmlIgnore] public bool IsBeam;
        [XmlIgnore] [ContentSerializer(Optional = true)] public string ParticleName;
        [XmlIgnore] public string SlotPictureName;

        [XmlIgnore] [ContentSerializerIgnore] public SpriteDraw SlotPictureTexture;
        [ContentSerializer(Optional = true)] [XmlIgnore] public string SpriteName;
        [XmlIgnore] public int Velocity;

        #region IPurchasable Members

        public String name { get; set; }

        [XmlIgnore]
        public int Cost { get; set; }

        #endregion

        //force the angle of fire to not be look - eg bombs that fall
    }
}