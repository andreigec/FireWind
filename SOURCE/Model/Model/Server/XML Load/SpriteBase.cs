using System;
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

        public SerializableDictionary<string, SpriteAnimation> sprites =
            new SerializableDictionary<string, SpriteAnimation>();

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
            int imagesX = image.Width/FrameWidth;
            int imagesY = image.Height/FrameHeight;
            imageCount = imagesX*imagesY;
            columnCount = imagesX;
        }
    }
}