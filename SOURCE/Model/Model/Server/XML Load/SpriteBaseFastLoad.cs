using System;
using Project.Model.Server.XML_Load;

namespace Project.XML_Load
{
    public class SpriteBaseFastLoad : ILoadXMLBase
    {
        public int FrameHeight;
        public int FrameWidth;
        public string frameName;
        public int framesPerImage;
        public String path;

        #region ILoadXMLBase Members

        public String name { get; set; }

        #endregion
    }
}