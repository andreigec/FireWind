using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model.Cameras
{
	public class CameraFlat : Camera2D
	{
		public override void adjustZoom()
		{
			_zoom = 1f;
		}

		public CameraFlat(int VPW, int VPH, Vector2 ss, drawableType dt)
			: base(VPW, VPH, ss, dt)
		{

		}

	}
}
