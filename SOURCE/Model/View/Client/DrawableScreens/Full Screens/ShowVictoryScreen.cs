using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;

namespace Project.View.Client.ClientScreens
{
    public class ShowVictoryScreen : IScreenControls
    {
        private MenuOptionsCreate menucreate;

        public ShowVictoryScreen()
        {
            generateMenu();
        }

        #region IScreenControls Members

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            /*
            cam.spriteBatch.Begin();
            cam.spriteBatch.Draw(XNA.PixelTexture,new Rectangle(0,0,700,300),Color.MediumPurple);
            cam.spriteBatch.End();
            */
            menucreate.Draw(cam, gameTime);
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            kbc.ClearKeyTimeout();
            kbc.InitialiseKeyPress(Keys.Enter);
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
            var handled = menucreate.HandleKey(kbc);
            if (handled)
                return;

            if (kbc.CanUseKey(Keys.Enter))
            {
                GameControlClient.ResetToBaseScreen(true);
            }
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            //throw new System.NotImplementedException();
        }

        #endregion

        private void generateMenu()
        {
            menucreate = new MenuOptionsCreate();
            menucreate.rootOptions = new MenuOptions(null, false);

            var one = menucreate.rootOptions.addChild(Localisation.CompletedLevel, false);
            menucreate.currentOption = one;
        }
    }
}