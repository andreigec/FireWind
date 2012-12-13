using System.Collections.Generic;
using ExternalUsage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Model.mapInfo;
using Project.View.Client.Cameras;

namespace Project.View.Client.ClientScreens
{
    public class TitlesScreen : IScreenControls
    {
        private static List<Texture2D> titles;
        private int currentTitle;

        public TitlesScreen()
        {
            if (titles != null)
                return;
            titles = new List<Texture2D>();
            titles.Add(XNA.PixelTexture);
        }

        #region IScreenControls Members

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            var r = new Rectangle(cam.spriteBatch.GraphicsDevice.Viewport.X, cam.spriteBatch.GraphicsDevice.Viewport.Y,
                                  cam.spriteBatch.GraphicsDevice.Viewport.Width,
                                  cam.spriteBatch.GraphicsDevice.Viewport.Height);

            cam.spriteBatch.Begin();
            cam.spriteBatch.Draw(titles[currentTitle], r, Color.Green);
            cam.spriteBatch.End();
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            //throw new System.NotImplementedException();
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            kbc.ClearKeyTimeout();
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
            GameControlClient.ShowMainScreen();
        }

        #endregion

        public void GenerateMenu()
        {
        }
    }
}