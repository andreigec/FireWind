using System;
using ExternalUsage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Model.mapInfo;

namespace Project.View.Client.Cameras
{
    public abstract class Camera2D
    {
        private static string SegoeUIMono = "Segoe UI Mono";

        #region screen info

        public int ViewportHeight;
        public int ViewportWidth;

        public Vector2 screenStart;

        #endregion screen info

        #region drawing

        public Vector2 Pos; // Camera Position
        public float _rotation; // Camera Rotation
        public Matrix _transform; // Matrix Transform
        public float _zoom; // Camera Zoom
        public SpriteBatch spriteBatch;
        public IScreenControls drawThis { get; set; }

        #endregion drawing

        public Camera2D(int VPW, int VPH, Vector2 ss, IScreenControls drawthis)
        {
            GameControlClient.myFPS = new FPS();
            drawThis = drawthis;
            screenStart = ss;
            _zoom = 1.0f;
            _rotation = 0.0f;
            Pos = Vector2.Zero;
            ViewportWidth = VPW;
            ViewportHeight = VPH;
            spriteBatch = new SpriteBatch(GameControlClient.ParentGame.GraphicsDevice);
        }

        public Camera2D()
        {
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public bool InFrame(Vector2 screenvec)
        {
            return
                (!(screenvec.X < 0 || screenvec.X > ViewportWidth || screenvec.Y < 0 || screenvec.Y > ViewportHeight));
        }

        public void setSize(int w, int h)
        {
            ViewportHeight = h;
            ViewportWidth = w;
        }

        public void Draw(GameTime gt)
        {
            drawThis.Draw(this, gt);
        }

        public void DrawString(String s, Color c, Vector2 pos)
        {
            SpriteFont font = XNA.GetFont(SegoeUIMono);
            float fontscale = 12/100f;
            spriteBatch.DrawString(font, s, pos, c, 0, Vector2.Zero, fontscale, SpriteEffects.None, 0);
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            Pos += amount;
        }

        /*
		// Get set position
		public Vector2 Pos
		{
			get { return _pos; }
			set { _pos = value; }
		}
		*/

        public Matrix get_transformation()
        {
            _transform =
                Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0))*
                Matrix.CreateRotationZ(Rotation)*
                Matrix.CreateScale(new Vector3(_zoom, _zoom, 1))*
                Matrix.CreateTranslation(new Vector3(ViewportWidth*0.5f, ViewportHeight*0.5f, 0));
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