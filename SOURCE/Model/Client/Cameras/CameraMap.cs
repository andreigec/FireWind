using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Model.Instances;

namespace Model.Cameras
{
	public class CameraMap : Camera2D
	{
		public void adjustZoom(ShipInstance s)
		{
			var vel = s.spriteinstance.move.Velocity;// +s.currentGravity;
			int w = 768;
			float z1 = ((float)w / (float)s.spriteinstance.basesprite.FrameWidth) / 17f;
			float z2 = ((float)w / (float)s.spriteinstance.basesprite.FrameWidth) / 20f;

			float z = (float)Shared.mapRange(vel, s.shipModel.minSpeed, s.shipModel.maxSpeed, z1, z2);
			_zoom = z;
		}

		public CameraMap(int VPW, int VPH,Vector2 ss,drawableType dt):base(VPW,VPH,ss,dt)
		{
			
		}
	}
}
