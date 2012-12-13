using System;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.Instances;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using Project.View.Client.DrawableScreens;

namespace Project
{
    public partial class SectorScreen
    {
        private const string option1 = "release all ships from hangers";
        public sector DrawThis;
        private MenuOptionsCreate menucreate;

        public SectorScreen(sector drawThis)
        {
            DrawThis = drawThis;
            MapScreen.drawThis = drawThis.thismap;
            generateMenu();
        }

        public SectorScreen()
        {
        }

        public CameraSector thisCamera { get; set; }

        #region IScreenControls Members

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            ((CameraSector) cam).adjustZoom(DrawThis.thismap);
            ((CameraSector) cam).FocusOnPoint(new Vector2(DrawThis.thismap.width/2f, 0));
            MapScreen.DrawMap(cam, gameTime);
            cam.spriteBatch.Begin();
            DrawBuildings(cam, gameTime);
            DrawShips(cam, gameTime);
            cam.spriteBatch.End();
            menucreate.Draw(cam, gameTime);

            //adjust to ship that is being controlled
            if (GameControlClient.playerShipClass != null)
                MapScreen.drawHUD(cam, gameTime, GameControlClient.playerShipClass, false, DrawThis.thismap);
        }

        #endregion

        private void generateMenu()
        {
            menucreate = new MenuOptionsCreate();
            menucreate.rootOptions = new MenuOptions(null, false);

            var one = menucreate.rootOptions.addChild(option1, false);

            menucreate.currentOption = one;
        }

        private void DrawBuildings(Camera2D cam, GameTime gameTime)
        {
            foreach (var b in DrawThis.thismap.buildings)
            {
                var v = cam.ToLocalLocation(b.spriteInstance.move.Position.Middle);
                var label = "B";
                var use = Color.Purple;
                switch (b.buildingType)
                {
                    case BuildingInstance.BuildingType.Civilian:
                        break;
                    case BuildingInstance.BuildingType.ShipHangar:
                        use = Color.Red;
                        label = "H";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                cam.DrawString(label, use, v);
            }
        }

        private void DrawShip(Camera2D cam, GameTime gameTime, bool sameFaction, bool isPlayer, bool disabled,
                              Vector2 pos,
                              int angle)
        {
            var sb = loadXML.loadedSprites["Icons"] as SpriteBase;
            if (sb == null)
                return;
            SpriteAnimation sa;
            if (disabled)
                sa = sb.sprites["Icon_12"];
            else
                sa = sb.sprites["Icon_10"];

            Color c;
            if (isPlayer)
            {
                if (disabled)
                    c = Color.DarkBlue;
                else
                    c = Color.Blue;
            }
            else
            {
                if (sameFaction)
                {
                    if (disabled)
                        c = Color.DarkGreen;
                    else
                        c = Color.Green;
                }
                else
                {
                    if (disabled)
                        c = Color.DarkRed;
                    else
                        c = Color.Red;
                }
            }
            XNARoutines.DrawImage(cam, sb, sa, pos, c, angle);
        }

        private void DrawShips(Camera2D cam, GameTime gameTime)
        {
            var f = GameControlClient.synchMain.myID;
            foreach (var s in DrawThis.thismap.ships)
            {
                var v = cam.ToLocalLocation(s.spriteInstance.move.Position.Middle);
                var disabled = s.disabled;
                DrawShip(cam, gameTime,
                         f == s.instanceOwner.FactionOwner, GameControlClient.playerShipClass.PlayerShip == s,
                         disabled, v,
                         (int) s.spriteInstance.LookAngle);
            }
        }
    }
}