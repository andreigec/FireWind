using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project.Model.Server.XML_Load;

namespace Project
{
    /* this will be either a particle or a sprite
	 */

    public class SpriteDraw : ILoadXMLBase
    {
        public int FrameHeight;
        public int FrameWidth;

        [ContentSerializerIgnore] public Texture2D image;

        #region ILoadXMLBase Members

        public String name { get; set; }

        #endregion
    }
}