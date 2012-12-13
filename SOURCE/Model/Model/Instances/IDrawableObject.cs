using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;

namespace Project.Model.Instances
{
    /// <summary>
    /// For map/sector/region
    /// </summary>
    public interface IDrawableObject:GCSHolder
    {
        [XmlIgnore]
        IshipAreaSynch parentArea { get; set; }
        SpriteInstance spriteInstance { get; set; }
        void Draw(Camera2D cam, GameTime gameTime);
    }

    public interface GCSHolder
    {
        [XmlIgnore]
        GameControlServer parentGCS { get; set; }
    }

    public interface  SynchMainHolder
    {
        [XmlIgnore]
        SynchMain parentSynchMain { get; set; }
    }
}