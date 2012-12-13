using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.mapInfo;

namespace Project.View.Client.Cameras
{
    public class CameraRegion : Camera2D
    {
        private const float minZoom = 1f;
        private const float maxZoom = 4f;
        //max times we can scroll
        public int maxScrollTimes = 5;
        //count of scrolls

        public CameraRegion(int VPW, int VPH, Vector2 ss, IScreenControls DrawThis)
            : base(VPW, VPH, ss, DrawThis)
        {
        }

        public CameraRegion()
        {
        }

        public int scrollnum { get; private set; }

        public void ChangeScroll(int newScrollVal)
        {
            scrollnum = newScrollVal;
            adjustZoom();
        }

        /*
		public void FocusOnPoint(Vector2 pos)
		{
			//get location of mouse cursor
			var p = ToWorldLocation(pos);
			Pos = p;
		}
		*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoomin"></param>
        public void adjustZoom(bool zoomin)
        {
            if ((zoomin == false && scrollnum == 0) || (zoomin && scrollnum == maxScrollTimes))
                return;

            if (zoomin)
                scrollnum++;
            else
                scrollnum--;
            adjustZoom();
        }

        public override void adjustZoom()
        {
            var z = (float) Shared.mapRange(scrollnum, 0, maxScrollTimes, minZoom, maxZoom);
            _zoom = z;
        }

        /*
        public CameraRegion(int VPW, int VPH, Vector2 ss, IScreenControls<CameraRegion> dt)
			: base(VPW, VPH, ss, dt)
		{
		}
         * */
    }
}