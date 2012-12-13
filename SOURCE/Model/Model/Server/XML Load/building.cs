using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Project.Model;
using Project.Model.Server.XML_Load;

namespace Project.XML_Load
{
    public class building : ILoadXMLBase
    {
        [XmlIgnore] [ContentSerializerIgnore] public SpriteBase BaseSprite;

        [XmlIgnore] [ContentSerializerIgnore] //percentage to sprite animation
            public Dictionary<double, SpriteAnimation> DestructFrames = new Dictionary<double, SpriteAnimation>();

        [XmlIgnore] public Dictionary<double, String> ImagesIN = new Dictionary<double, String>();

        //<<
        [XmlIgnore] public double MaxArmour;
        [XmlIgnore] public string SpriteName;

        #region ILoadXMLBase Members

        public String name { get; set; }

        #endregion
    }
}