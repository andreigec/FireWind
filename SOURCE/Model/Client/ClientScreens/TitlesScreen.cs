using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using KeyboardClass = Model.Client.ClientScreens.KeyboardClass;

namespace Model.Cameras
{
	public class TitlesScreen : drawableType
	{
		private static List<Texture2D> titles;

		public TitlesScreen()
		{
			if (titles != null)
				return;
			titles = new List<Texture2D>();
			titles.Add(texturing.SimpleTexture);
		}

		private int currentTitle = 0;

		public void Draw(Camera2D cam, GameTime gameTime)
		{
			var r = new Rectangle(cam.spriteBatch.GraphicsDevice.Viewport.X, cam.spriteBatch.GraphicsDevice.Viewport.Y,
			                            cam.spriteBatch.GraphicsDevice.Viewport.Width,
			                            cam.spriteBatch.GraphicsDevice.Viewport.Height);
			
			cam.spriteBatch.Begin();
			cam.spriteBatch.Draw(titles[currentTitle],r, Color.Green);
			cam.spriteBatch.End();
		}

		public void RegisterKeyboardKeys(KeyboardClass kbc)
		{

		}

		public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
		{
			GameControlClient.ShowMainScreen();
		}
	}
}
