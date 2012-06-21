using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;

namespace Project.View.Client.ClientScreens
{
    public class InGameMenuPopup : IScreenControls
    {
        private MenuOptionsCreate menucreate;

        public InGameMenuPopup()
        {
            generateMenu();
        }

        //private int currentTitle = 0;

        #region IScreenControls Members

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            menucreate.Draw(cam, gameTime);
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            kbc.ClearKeyTimeout();
            kbc.InitialiseKeyPress(Keys.Up);
            kbc.InitialiseKeyPress(Keys.Down);
            kbc.InitialiseKeyPress(Keys.Enter);
            kbc.InitialiseKeyPress(Keys.Escape);
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
            var handled = menucreate.HandleKey(kbc);
            if (handled)
                return;

            if (kbc.CanUseKey(Keys.Enter))
            {
                if (menucreate.currentOption.Text.Equals(Localisation.ReturnToGame))
                {
                    GameControlClient.ExitPopupScreen<InGameMenuPopup>();
                }
                if (menucreate.currentOption.Text.Equals(Localisation.BackToBase))
                {
                    GameControlClient.ResetShip(GameControlClient.playerShipClass.PlayerShip,
                                                GameControlClient.synchMain.gcs.gameRegion,false);
                    GameControlClient.EndGame(false);
                    GameControlClient.ResetToBaseScreen(true);
                }
            }
            else if (kbc.CanUseKey(Keys.Escape))
            {
                GameControlClient.ExitPopupScreen<InGameMenuPopup>();
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
            var one = menucreate.rootOptions.addChild(Localisation.ReturnToGame, false);

            menucreate.rootOptions.addChild(Localisation.BackToBase, false);


            menucreate.currentOption = one;
        }
    }
}