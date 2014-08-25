using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.mapInfo;

namespace Project.View.Client.Cameras
{
    public class CameraMap : Camera2D
    {
        public double LastScrollTime;

        /// <summary>
        /// dont update auto ship zoom until this time in seconds
        /// </summary>
        public double autoZoomTimeout;

        public int maxScrollTimes = 10;
        private float maxZoom = 1f;
        private float minZoom = .3f;

        public CameraMap(int VPW, int VPH, Vector2 ss, IScreenControls DrawThis)
            : base(VPW, VPH, ss, DrawThis)
        {
        }

        public CameraMap()
        {
        }

        public int scrollnum { get; private set; }

        public void ChangeScroll(int newScrollVal)
        {
            scrollnum = newScrollVal;
            adjustZoom();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoomin"></param>
        public void adjustZoom(bool zoomin, GameTime gameTime = null, bool changeTimeout = true)
        {
            if ((zoomin == false && scrollnum == 0) || (zoomin && scrollnum == maxScrollTimes))
                return;

            if (zoomin)
                scrollnum++;
            else
                scrollnum--;

            adjustZoom();
            if (gameTime != null && changeTimeout)
                autoZoomTimeout = gameTime.TotalGameTime.TotalSeconds + 3;
        }

        /// <summary>
        /// automatically zoom onto player ship
        /// </summary>
        /// <param name="s"></param>
        public void adjustZoom(ShipInstance s, GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds < autoZoomTimeout)
                return;

            double vel = s.spriteInstance.move.Velocity; // +s.currentGravity;

            double ms = ShipBaseItemsMIXIN.GetMaxSpeed(s);

            var cl = (int) Shared.mapRange(vel, 0, ms, 6, 3);

            if (cl < scrollnum)
                adjustZoom(false, gameTime, false);
            else if (cl > scrollnum)
                adjustZoom(true, gameTime, false);
        }

        /// <summary>
        /// Get the zoom level from the click amount
        /// </summary>
        public override void adjustZoom()
        {
            var z = (float) Shared.mapRange(scrollnum, 0, maxScrollTimes, minZoom, maxZoom);
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
    }
}