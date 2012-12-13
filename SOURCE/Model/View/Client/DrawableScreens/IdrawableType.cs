using Microsoft.Xna.Framework;
using Project.View.Client;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;

namespace Project.Model.mapInfo
{
    public interface IScreenControls
    {
        void KeyboardUpdate(GameTime gt, KeyboardClass kbc);
        void MouseUpdate(GameTime gt, MouseClass mc);
        void RegisterKeyboardKeys(KeyboardClass kbc);
        void Draw(Camera2D cam, GameTime gameTime);
    }
}