using System;
using System.Collections.Generic;
using ExternalUsage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Model;
using Project.Model.Instances;
using Project.View.Client.Cameras;
using Project.View.Client.DrawableScreens;

namespace Project
{
    public partial class MapScreen
    {
        public static Map drawThis;
        //private readonly MenuOptionsCreate menucreate;

        public MapScreen(Map toDraw)
        {
            drawThis = toDraw;
        }

        public MapScreen()
        {
        }

        public CameraMap thisCamera { get; set; }


        public static IDrawableObject GetNearestShipBuilding(Camera2D thisCamera, Vector2 originalScreenPoint)
        {
            Vector2 worldloc = thisCamera.ToWorldLocation(originalScreenPoint);
            //get the nearest planet and zoom to that
            double mindist = -1;
            IDrawableObject minsec = null;

            var obs = new List<IDrawableObject>();
            obs.AddRange(drawThis.buildings);
            obs.AddRange(drawThis.ships);
            foreach (IDrawableObject s in obs)
            {
                double tempmindist = VectorMove.getDistanceBetweenVectors(worldloc,
                                                                          s.spriteInstance.move.Position.Middle);
                if (minsec == null || mindist > tempmindist)
                {
                    mindist = tempmindist;
                    minsec = s;
                }
            }

            if (mindist == -1)
                return null;

            return minsec;
        }

        #region draw

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            var cm = cam as CameraMap;
            //adjust to ship that is being controlled
            PlayerShipClass ps = GameControlClient.playerShipClass;
            if (GameControlClient.playerShipClass != null &&
                ShipInstance.IsOnMap(GameControlClient.playerShipClass.PlayerShip, drawThis))
            {
                cm.adjustZoom(ps.PlayerShip, gameTime);
                cm.FocusOnPoint(ps.PlayerShip.spriteInstance.move.Position.Middle);
            }
            DrawMap(cam, gameTime);

            cam.spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null,
                                  cam.get_transformation());
            drawBuildings(cam, gameTime);
            drawShips(cam, gameTime);
            drawShots(cam, gameTime);
            cam.spriteBatch.End();

            cam.spriteBatch.Begin();
            DrawWeaponBeams(cam, gameTime);
            cam.spriteBatch.End();

            if (ps != null)
                drawHUD(cam, gameTime, ps, true, drawThis);
        }

        public static void DrawMap(Camera2D cam, GameTime gameTime)
        {
            //draw sky
            cam.spriteBatch.Begin();
            XNA.DrawCamRectangle(cam.spriteBatch, Color.Black);
            cam.spriteBatch.End();

            cam.spriteBatch.Begin(0, BlendState.Opaque, SamplerState.LinearClamp, null, null, null,
                                  cam.get_transformation());
            //end zones
            Rectangle r;
            int height = drawThis.height;
            int width = drawThis.width;
            int deadzone = Map.deadzone;
            int deadzoneMaxX = drawThis.deadzoneMaxX;
            int deadzoneMinX = drawThis.deadzoneMinX;

            r = new Rectangle(0, -height, deadzone, height);
            cam.spriteBatch.Draw(XNA.PixelTexture, r, Color.LightSalmon);
            r = new Rectangle(deadzoneMaxX, -height, deadzone, height);
            cam.spriteBatch.Draw(XNA.PixelTexture, r, Color.LightSalmon);
            r = new Rectangle(deadzoneMinX, -height, width - deadzone, deadzone);
            cam.spriteBatch.Draw(XNA.PixelTexture, r, Color.LightSalmon);
            /////

            drawThis.terrain.drawTerrain(cam, gameTime);
            cam.spriteBatch.End();
        }

        private static void drawBuildings(Camera2D cam, GameTime gt)
        {
            bool ok = false;
            while (ok == false)
            {
                try
                {
                    int a = 0;
                    foreach (BuildingInstance b in drawThis.buildings)
                    {
                        b.Draw(cam, gt);
                        a++;
                    }
                    ok = true;
                }
                catch (Exception)
                {
                }
            }
        }

        private static void drawShips(Camera2D cam, GameTime gt)
        {
            bool ok = false;
            while (ok == false)
            {
                try
                {
                    foreach (ShipInstance s in drawThis.ships)
                    {
                        s.Draw(cam, gt);
                    }
                    ok = true;
                }
                catch (Exception)
                {
                }
            }
        }

        private static void drawShots(Camera2D cam, GameTime gt)
        {
            bool ok = false;
            while (ok == false)
            {
                try
                {
                    foreach (WeaponInstance s in drawThis.shots)
                    {
                        s.Draw(cam, gt);
                    }
                    ok = true;
                }
                catch (Exception)
                {
                }
            }
        }

        private static void DrawWeaponBeams(Camera2D cam, GameTime gt)
        {
            for (int a = 0; a < drawThis.shots.Count; a++)
            {
                WeaponInstance s = drawThis.shots[a];

                if (s.weapon.IsBeam == false)
                    continue;

                //if a single shot of a beam, draw a rectangle going the same was as multiple shots would
                if (s.nextBeamInstanceID == -1 && s.previousBeamInstanceID == -1)
                {
                    Vector2 pos1 = s.spriteInstance.move.Position.Middle;
                    Vector2 pos2 = pos1;
                    VectorMove.UpdatePosition(ref pos2, VectorMove.wrapAngle(s.spriteInstance.move.Angle + 90f), 5);

                    Vector2 spos1 = cam.ToLocalLocation(pos1);
                    Vector2 spos2 = cam.ToLocalLocation(pos2);

                    XNA.DrawLine(cam.spriteBatch, 10, Color.Red, spos1, spos2);
                }
                else if (s.nextBeamInstanceID != -1)
                {
                    Vector2 pos1 = s.spriteInstance.move.Position.Middle;
                    if (s.nextBeamInstanceID != -1)
                    {
                        WeaponInstance p = drawThis.GetShot(s.nextBeamInstanceID);
                        if (p != null)
                        {
                            Vector2 pos2 = p.spriteInstance.move.Position.Middle;
                            Vector2 spos1 = cam.ToLocalLocation(pos1);
                            Vector2 spos2 = cam.ToLocalLocation(pos2);

                            //check both occur in the screen
                            if (cam.InFrame(spos1) || cam.InFrame(spos2))
                            {
                                XNA.DrawLine(cam.spriteBatch, s.weapon.BeamRadius, Color.Red, spos1, spos2);
                            }
                        }
                    }
                }
            }
        }

        private static void DrawPointerToShip(Camera2D cam, ShipInstance ps, ShipInstance si)
        {
            Vector2 start = ps.spriteInstance.move.Position.Middle;
            Vector2 end = si.spriteInstance.move.Position.Middle;

            const int dist = 300;
            float angle = VectorMove.getAngleToOtherVector(start, end);
            //extend
            Vector2 visend = start;
            VectorMove.UpdatePosition(ref visend, angle, dist);
            //get local pos
            Vector2 local = cam.ToLocalLocation(visend);

            //get the image we want to show
            var sb = loadXML.loadedSprites["Icons"] as SpriteBase;
            if (sb == null)
                return;
            SpriteAnimation sa = sb.sprites["Icon_12"];

            //colour
            Color drawColour = Color.Green;
            if (ps.instanceOwner.FactionOwner != si.instanceOwner.FactionOwner)
                drawColour = Color.Red;

            XNARoutines.DrawImage(cam, sb, sa, local, drawColour, angle);
        }

        public static void drawHUD(Camera2D cam, GameTime gt, PlayerShipClass playerShipClass, bool ShipDirections,
                                   Map m)
        {
            if (playerShipClass == null)
                return;

            Vector2 start = Vector2.Zero;
            double current = 0;
            double max = 100;
            float percentage = 0f;

            const int startx = 5;
            const int meterheight = 30;
            const int meterwidth = 150;

            cam.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            //player ship has to be init for this and on the same map
            if (ShipInstance.IsOnMap(playerShipClass.PlayerShip, m))
            {
                //TODO:scale for window height

                ShipInstance playerShip = playerShipClass.PlayerShip;

                //Energy
                start = new Vector2(startx + 35, cam.ViewportHeight - meterheight);
                current = playerShip.CurrentEnergy;
                max = ShipBaseItemsMIXIN.GetMaxEnergy(playerShip);
                percentage = (float) (current/max);
                DrawMeter(cam, start, meterwidth, meterheight, percentage, Color.Black, Color.LightBlue);

                //weapon icons
                DrawWeaponMeterAddons(cam, start, meterwidth, meterheight, playerShip);

                //battery icon
                start = new Vector2(start.X - meterheight, start.Y);
                XNARoutines.DrawImage(cam, loadXML.loadedSprites["BATTERY"], start, meterheight, Color.White);

                //Armour
                current = playerShip.CurrentArmour;
                max = ShipBaseItemsMIXIN.GetMaxArmour(playerShip);
                percentage = (float) (current/max);
                start = new Vector2(startx + meterwidth*1.5f, cam.ViewportHeight - meterheight);
                DrawMeter(cam, start, meterwidth, meterheight, percentage, Color.Red, Color.Brown);

                //armour icon
                start = new Vector2(start.X - meterheight, start.Y);
                XNARoutines.DrawImage(cam, loadXML.loadedSprites["ARMOUR"], start, meterheight, Color.White);

                //Shield
                current = playerShip.CurrentShield;
                max = ShipBaseItemsMIXIN.GetMaxShield(playerShip);
                percentage = (float) (current/max);
                start = new Vector2(startx + meterwidth*3f, cam.ViewportHeight - meterheight);
                DrawMeter(cam, start, meterwidth, meterheight, percentage, Color.Red, Color.Blue);

                //armour icon
                start = new Vector2(start.X - meterheight, start.Y);
                XNARoutines.DrawImage(cam, loadXML.loadedSprites["SHIELD"], start, meterheight, Color.White);

                if (ShipDirections)
                {
                    //draw icons to ships outside player screen
                    foreach (ShipInstance s in drawThis.ships)
                    {
                        Vector2 pos = cam.ToLocalLocation(s.spriteInstance.move.Position.Middle);
                        if (cam.InFrame(pos) == false)
                            DrawPointerToShip(cam, playerShip, s);
                    }
                }
            }

            //always show this

            //money
            start = new Vector2(startx + meterwidth*4.5f, cam.ViewportHeight - meterheight);
            current = playerShipClass.Credits;
            string creditstr = "Credits:" + current.ToString();
            cam.DrawString(creditstr, Color.Thistle, start);

            //close sprite batch at the end
            cam.spriteBatch.End();
        }

        private static void DrawMeter(Camera2D cam, Vector2 startposIN, int length, int width, float percentage,
                                      Color backColour, Color foreColour)
        {
            Vector2 startleftraw = startposIN;
            var endright = new Vector2(startposIN.X + length, startposIN.Y);
            float mod = startposIN.X + (length*percentage);

            //avail
            var startleft = new Vector2(mod, startposIN.Y);
            XNA.DrawLine(cam.spriteBatch, width, foreColour, startleftraw, startleft);

            //unavail
            XNA.DrawLine(cam.spriteBatch, width, backColour, startleft, endright);
        }

        private static void DrawWeaponMeterAddons(Camera2D cam, Vector2 startposIN, int length, int width,
                                                  ShipInstance si)
        {
            double me = ShipBaseItemsMIXIN.GetMaxEnergy(si);
            double ce = si.CurrentEnergy;
            bool top = true;
            bool setonce = false;
            Vector2 lastmid = Vector2.Zero;
            const int mouseiconsize = 32;

            foreach (var w in si.EquipSlots)
            {
                int e = w.Value.w.EnergyPerShot;

                if (e > me)
                    continue;

                var p = (float) (e/me);
                float p2 = (length*p) + startposIN.X;

                float starty;
                float endy;

                if (setonce && Collisions.CheckLineCollisionRadius(lastmid, startposIN, width))
                    top = !top;

                if (top)
                {
                    starty = startposIN.Y;
                    endy = starty - 5;
                }
                else
                {
                    starty = startposIN.Y + width;
                    endy = starty + 5;
                }

                var vertlinestart = new Vector2(p2, starty);
                var vertlineend = new Vector2(p2, endy);

                XNA.DrawLine(cam.spriteBatch, 2, Color.Teal, vertlinestart, vertlineend);

                Vector2 vertpicmid;
                if (top)
                {
                    vertpicmid = new Vector2(p2, endy - mouseiconsize);
                }
                else
                {
                    vertpicmid = new Vector2(p2, endy);
                }

                Color tint = Color.Red;
                if (ce >= e)
                    tint = Color.White;

                if (w.Key == SlotLocation.SlotLocationEnum.LeftWeapon)
                    XNARoutines.DrawImage(cam, loadXML.loadedSprites["LMB"], vertpicmid, mouseiconsize, tint);
                else
                    XNARoutines.DrawImage(cam, loadXML.loadedSprites["RMB"], vertpicmid, mouseiconsize, tint);

                lastmid = new Vector2(p2, startposIN.Y);
                setonce = true;
            }
        }

        #endregion draw
    }
}