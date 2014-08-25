using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model.mapInfo;
using Project.View.Client;
using Project.View.Client.ClientScreens;

namespace Project
{
    public partial class SectorScreen : IScreenControls
    {
        #region IScreenControls Members

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            kbc.ClearKeyTimeout();
            kbc.InitialiseKeyPress(Keys.Down);
            kbc.InitialiseKeyPress(Keys.Up);
            kbc.InitialiseKeyPress(Keys.Enter);
            kbc.InitialiseKeyPress(Keys.Escape);
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
            bool handled = menucreate.HandleKey(kbc);
            if (handled)
                return;

            if (kbc.CanUseKey(Keys.Escape))
            {
                GameControlClient.ShowInGamePopup();
            }
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            bool mwup = mc.ButtonsDown.ContainsKey(MouseClass.mouseButtons.mouseWheelUp);
            bool mwdown = mc.ButtonsDown.ContainsKey(MouseClass.mouseButtons.mouseWheelDown);
            if (mwdown)
            {
                //GameControlClient.ZoomToRegion(DrawThis.parentRegion,true);
            }
            else if (mwup)
            {
                //Vector2 loc= mc.ButtonsDown[MouseClass.mouseButtons.mouseWheelUp];

                GameControlClient.ZoomToMap(DrawThis.thismap);
            }
        }

        #endregion
    }
}