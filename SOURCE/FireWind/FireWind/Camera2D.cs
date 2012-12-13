using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FireWind
{
	public class Camera2D
	{
		protected float          _zoom; // Camera Zoom
        public Matrix             _transform; // Matrix Transform
        public Vector2          _pos; // Camera Position
        protected float         _rotation; // Camera Rotation
		private int ViewportWidth ;
		private int ViewportHeight;
 
        public Camera2D(int VPW,int VPH)
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        	ViewportWidth = VPW;
        	ViewportHeight = VPH;
        }

		// Sets and gets zoom
		public float Zoom
		{
			get { return _zoom; }
			set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
		}

		public float Rotation
		{
			get { return _rotation; }
			set { _rotation = value; }
		}

		// Auxiliary function to move the camera
		public void Move(Vector2 amount)
		{
			_pos += amount;
		}
		// Get set position
		public Vector2 Pos
		{
			get { return _pos; }
			set { _pos = value; }
		}

		public Matrix get_transformation(GraphicsDevice graphicsDevice)
		{
			_transform =       // Thanks to o KB o for this solution
			  Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
										 Matrix.CreateRotationZ(Rotation) *
										 Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
										 Matrix.CreateTranslation(new Vector3(ViewportWidth * 0.5f, ViewportHeight * 0.5f, 0));
			return _transform;
		}

		public Vector2 ToWorldLocation(Vector2 position)
		{
			return Vector2.Transform(position, Matrix.Invert(_transform));
		}

		public Vector2 ToLocalLocation(Vector2 position)
		{
			return Vector2.Transform(position, _transform);
		}
	}
}
