using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;

namespace Project.View.Client.ClientScreens
{
    public class DestroyScreen : IScreenControls
    {
        private MenuOptionsCreate menucreate;

        public DestroyScreen()
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
                GameControlClient.EndGame(false);
                GameControlClient.ResetToBaseScreen(true);
                Manager.FireLogEvent("Removed Ship is my ship, returning to base",
                                 SynchMain.MessagePriority.High, false);
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

            //select world
            var rebuildval = ShipBaseItemsMIXIN.GetRebuildCost(GameControlClient.playerShipClass.PlayerShip);
            int cost;
            if (GameControlClient.playerShipClass.Credits < rebuildval)
                cost = GameControlClient.playerShipClass.Credits;
            else
                cost = rebuildval;

            var coststr = Localisation.DestroyShip;
            if (cost > 0)
                coststr += Environment.NewLine + Localisation.For + " " + cost.ToString() + " " + Localisation.Credits;

            coststr += Environment.NewLine + Localisation.PressToContinue;

            var one = menucreate.rootOptions.addChild(coststr, false);
            menucreate.currentOption = one;
        }
    }
}