using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Model;

namespace Model.Cameras
{
	public class CameraSector:Camera2D
	{
		public void adjustZoom(map m)
		{
			float w = this.ViewportWidth;
			float z = w/m.width;

			_zoom = z;
		}

		public CameraSector(int VPW, int VPH,Vector2 ss,drawableType dt):base(VPW,VPH,ss,dt)
		{
			
		}
	}
}
