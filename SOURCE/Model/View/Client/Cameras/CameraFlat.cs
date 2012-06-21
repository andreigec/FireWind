using Microsoft.Xna.Framework;
using Project.Model.mapInfo;

namespace Project.View.Client.Cameras
{
    public class CameraFlat : Camera2D
    {
        public CameraFlat(int VPW, int VPH, Vector2 ss, IScreenControls dt)
            : base(VPW, VPH, ss, dt)
        {
        }

        public override void adjustZoom()
        {
            _zoom = 1f;
        }
    }
}