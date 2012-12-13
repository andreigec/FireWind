using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Model
{
	public abstract class Camera2D
	{
		#region screen info
		protected  int ViewportWidth;
		protected int ViewportHeight;

		public Vector2 screenStart;
		#endregion screen info

		#region drawing
		public SpriteBatch spriteBatch;
		public drawableType drawThis;
		protected float _zoom; // Camera Zoom
		public Matrix _transform; // Matrix Transform
		public Vector2 _pos; // Camera Position
		public float _rotation; // Camera Rotation
		#endregion drawing

		public void setSize(int w,int h)
		{
			ViewportHeight = h;
			ViewportWidth = w;
		}

		public Camera2D(int VPW, int VPH, Vector2 ss,drawableType drawthis)
		{
			drawThis = drawthis;
			screenStart = ss;
        	_zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        	ViewportWidth = VPW;
        	ViewportHeight =VPH;
			spriteBatch = new SpriteBatch(GameControlClient.ParentGame.GraphicsDevice);
        }

		public virtual void Draw(GameTime gt)
		{
			drawThis.Draw(this,gt);
		}

		public void DrawString(String s, Color c, Vector2 pos)
		{
			spriteBatch.DrawString(texturing.helvetica, s, pos,c);
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

		public void FocusOnPoint(Vector2 pos)
		{
			_pos = pos;
			var diff1 = ((ViewportHeight / _zoom) / 2);
			var diff2 = pos.Y + diff1;
			if (diff2 > 0)
				_pos.Y = diff1*-1;
			}

		public Matrix get_transformation()
		{
			_transform =
			  Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
										 Matrix.CreateRotationZ(Rotation) *
										 Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) *
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

		public virtual void adjustZoom()
		{
			_zoom = 1f;
		}

	}
}
