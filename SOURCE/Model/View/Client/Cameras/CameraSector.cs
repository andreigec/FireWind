using Microsoft.Xna.Framework;
using Project.Model.mapInfo;

namespace Project.View.Client.Cameras
{
    public class CameraSector : Camera2D
    {
        public CameraSector(int VPW, int VPH, Vector2 ss, IScreenControls DrawThis)
            : base(VPW, VPH, ss, DrawThis)
        {
        }

        public CameraSector()
        {
        }

        public void adjustZoom(Map m)
        {
            float w = ViewportWidth;
            float z = w/m.width;

            _zoom = z;
        }

        public void FocusOnPoint(Vector2 pos)
        {
            Pos = pos;
            float diff1 = ((ViewportHeight/_zoom)/2);
            float diff2 = pos.Y + diff1;
            if (diff2 > 0)
                Pos.Y = diff1*-1;
        }

        /*
        public CameraSector(int VPW, int VPH, Vector2 ss, IScreenControls<CameraSector> dt)
			: base(VPW, VPH, ss, dt)
		{
		}
         * */
    }
}