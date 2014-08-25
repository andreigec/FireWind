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
        public String name { get; set; }

		public int FrameHeight;
		public int FrameWidth;

		public int ScaledWidth;
		public int ScaledHeight;

        [ContentSerializerIgnore] public Texture2D image;

        #region ILoadXMLBase Members

        

        #endregion
    }
}