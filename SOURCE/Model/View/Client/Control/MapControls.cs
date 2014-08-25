using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model;
using Project.Model.Instances;
using Project.Model.mapInfo;
using Project.View.Client;
using Project.View.Client.ClientScreens;

namespace Project
{
    public partial class MapScreen : IScreenControls
    {
        #region IScreenControls Members

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            kbc.ClearKeyTimeout();
            kbc.InitialiseKeyHold(Keys.Left, 10);
            kbc.InitialiseKeyHold(Keys.Right, 10);
            kbc.InitialiseKeyHold(Keys.Up, 10);
            kbc.InitialiseKeyHold(Keys.Down, 10);
            kbc.InitialiseKeyHold(Keys.Space, 10);
            kbc.InitialiseKeyPress(Keys.LeftControl);
            kbc.InitialiseKeyPress(Keys.Escape);
        }

        /*
        public void KeyboardUpdateShip(GameTime gt, KeyboardClass kbs, ShipInstance ps)
        {
            bool performlookchange = false;
            bool lookleft = true;

            /////////////////
            if (ps.instanceOwner.BeingControlled() && ps.disabled == false)
            {
                var mins = ps.GetMinSpeed();
                //direction
                if (kbs.CanUseKey(Keys.Left) && ps.spriteinstance.move.Velocity > mins)
                {
                    performlookchange = true;
                    lookleft = true;
                }
                if (kbs.CanUseKey(Keys.Right) && ps.spriteinstance.move.Velocity > mins)
                {
                    performlookchange = true;
                    lookleft = false;
                }

                //speed
                if (kbs.CanUseKey(Keys.Up))
                {
                    //change the move angle so we move in the direction we want
                    ps.spriteinstance.move.UpdateAngle(ps.spriteinstance.LookAngle, ps.getTurningSpeed());
                    ps.ChangeSpeed(ps.GetAccelIncr());
                }

                if (kbs.CanUseKey(Keys.Down))
                {
                    var bm = ps.GetBrakeIncr();
                    if (ps.currentAccel > 0)
                        ps.currentAccel = bm;
                    ps.ChangeSpeed(bm);
                }
                else if (ps.currentAccel < 0)
                {
                    ps.currentAccel = 0;
                }

                if (performlookchange)
                    ps.spriteinstance.ChangeLook(lookleft, ps.getTurningSpeed());
            }
        }
        
        public void PlayerFireWeapon(GameTime gt, ShipInstance ps, bool isLeft)
        {
            if (ps.parentArea is map == false)
                return;

            var parentmap = ps.parentArea as map;

            if (isLeft && ps.LeftWeapon != null)
                ps.LeftWeapon.fire(gt, parentmap, ps);

            if (isLeft == false && ps.RightWeapon != null)
                ps.RightWeapon.fire(gt, parentmap, ps);
        }
        */

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
            if (kbc.CanUseKey(Keys.Escape))
            {
                GameControlClient.ShowInGamePopup();
            }

            /*
            if (GameControlClient.playerShipClass == null || GameControlClient.playerShipClass.PlayerShip == null)
                return;

            var ps = GameControlClient.playerShipClass.PlayerShip;
            //cant control the ship when the ai is
            if (ps.AIControllingPlane())
                return;

            KeyboardUpdateShip(gt, kbc, ps);
             * */
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            if (GameControlClient.playerShipClass == null || GameControlClient.playerShipClass.PlayerShip == null)
                return;

            ShipInstance ps = GameControlClient.playerShipClass.PlayerShip;

            //ship centric controls only for the matching map, and can control ship
            if (ps.AIControllingPlane() == false && ps.disabled == false &&
                GameControlClient.playerShipClass.PlayerShip.parentArea == drawThis)
            {
                Vector2 camera = mc.CurrentPos;

                //point ship towards cursor
                Vector2 localcam = thisCamera.ToLocalLocation(ps.spriteInstance.move.Position.Middle);
                float dif = VectorMove.getAngleToOtherVector(localcam, camera);
                float origAngle = ps.spriteInstance.LookAngle;

                ps.spriteInstance.ChangeLook(dif, ps.getTurningSpeed());

                //change speed depending on cursor dist from player ship
                double dist = VectorMove.getDistanceBetweenVectors(localcam, camera);
                double max = ShipBaseItemsMIXIN.GetMaxSpeed(ps);
                double min = ShipBaseItemsMIXIN.GetMinSpeed(ps);
                double changeto = min;

                float screensq = (thisCamera.ViewportHeight + thisCamera.ViewportWidth)/4f;
                if (dist > screensq) dist = screensq;
                float perc = ((float) (int) dist/(int) screensq);
                double diff = max - min;
                changeto = min + perc*diff;

                ps.ChangeToSpeed(changeto, origAngle);

                //left click
                if (mc.LeftButtonAny() && ps.Slots.ContainsKey(SlotLocation.SlotLocationEnum.LeftWeapon))
                    ps.EquipSlots[SlotLocation.SlotLocationEnum.LeftWeapon].fire(gt, drawThis, ps);

                //right click
                if (mc.RightButtonAny() && ps.Slots.ContainsKey(SlotLocation.SlotLocationEnum.RightWeapon))
                    ps.EquipSlots[SlotLocation.SlotLocationEnum.RightWeapon].fire(gt, drawThis, ps);
            }

            //scroll wheels
            bool mwup = mc.ButtonsDown.ContainsKey(MouseClass.mouseButtons.mouseWheelUp);
            bool mwdown = mc.ButtonsDown.ContainsKey(MouseClass.mouseButtons.mouseWheelDown);
            if (mwup || mwdown)
            {
                thisCamera.adjustZoom(mwup, gt);

                //if min zoom, zoom to sector
                if (thisCamera.scrollnum == 0)
                {
                    GameControlClient.ZoomToSector(drawThis.parentSector);
                }
                else
                {
                    Vector2 loc;
                    if (mwup)
                        loc = mc.ButtonsDown[MouseClass.mouseButtons.mouseWheelUp];
                    else
                        loc = mc.ButtonsDown[MouseClass.mouseButtons.mouseWheelDown];

                    IDrawableObject zoomto = GetNearestShipBuilding(thisCamera, loc);

                    if (zoomto != null)
                        loc = zoomto.spriteInstance.move.Position.Middle;

                    thisCamera.FocusOnPoint(loc);
                }
            }
        }

        #endregion
    }
}