using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project.Model;

namespace Project
{
    public class SpriteBase : SpriteDraw
    {
        #region xml

        public String path;
        public SerializableDictionary<string, SpriteAnimation> sprites = new SerializableDictionary<string, SpriteAnimation>();

        #endregion xml

        public const string contentDir = "Content/";
        [ContentSerializerIgnore] public int columnCount;
        [ContentSerializerIgnore] public int imageCount;

        public SpriteAnimation this[shipAnimations SA]
        {
            get { return sprites[SA.ToString()]; }
        }

        public void loadTexture2D(Game parent, String name)
        {
            image = parent.Content.Load<Texture2D>(name);
            var imagesX = image.Width/FrameWidth;
            var imagesY = image.Height/FrameHeight;
            imageCount = imagesX*imagesY;
            columnCount = imagesX;
        }
    }
}