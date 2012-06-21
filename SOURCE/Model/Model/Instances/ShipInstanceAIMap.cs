using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.Instances;
using Project.Networking;

namespace Project
{
    public partial class ShipInstance
    {
        [XmlIgnore] private ShipInstance currentTarget;
        //force on or off for AI routines
        [XmlIgnore] public bool forceAI;
        [XmlIgnore] private double maxHoverLevel = 1000f;

        //anythng lower than this is dangerous
        [XmlIgnore] private double minHoverLevel = 200f;

        //force ai for players if the ship is out of map bounds  to get it back in
        [XmlIgnore] public bool outOfBounds;

        //DEBUG
        [XmlIgnore] private Vector2 wantpos;

        public void updateAIMap(GameTime gt)
        {
            //only works for map
            if (!(parentArea is map))
                return;

            var parentmap = parentArea as map;

            //only perform AI if localhost, or on player ship
            //being controlled
            if (forceAI == false && instanceOwner.PlayerOwnerID == parentGCS.parentSynch.myID)
            {
                //check invalid bounds
                if (invalidFuturePosition(spriteInstance.move.Angle))
                {
                    FixAndLookAngle(spriteInstance.move.Angle);
                    return;
                }
                return;
            }

            //no ai update allowed
            if (forceAI == false && parentArea.parentGCS.flagChecked(UpdateModes.UpdateAI) == false)
                return;

            //get target-octree later?
            var targets = new List<ShipInstance>();

            foreach (
                var si in
                    parentmap.ships.Where(
                        s =>
                        this != s && instanceOwner.FactionOwner != s.instanceOwner.FactionOwner && s.disabled == false &&
                        s.spriteInstance.move != null))
            {
                targets.Add(si);
            }

            if (targets.Count > 0)
            {
                currentTarget = getTarget(targets);
                if (currentTarget != null)
                    AILogicAttack(gt);
            }
            else
                AILogicWander(gt);
        }

        private void AILogicWander(GameTime gt)
        {
            var m = parentArea as map;
            //get the slowest possible speed - the gravity kick in
            var min = ShipBaseItemsMIXIN.GetMinSpeed(this);
            var origAngle = spriteInstance.LookAngle;

            if (spriteInstance.move.Angle < 90 || spriteInstance.move.Angle > 270)
                FixAndLookAngle(315);
            else
                FixAndLookAngle(225);

            ChangeToSpeed(min, origAngle);
        }

        private void AILogicAttack(GameTime gt)
        {
            var m = parentArea as map;
            var parentmap = parentArea as map;
            var dest = currentTarget.spriteInstance.move.Position.Middle;

            var angleToTarget = spriteInstance.move.getAngleToOtherVector(dest);
            var distance = Vector2.Distance(spriteInstance.move.Position.Middle, dest);

            //change speed depending on distance from target
            var ms = ShipBaseItemsMIXIN.GetMaxSpeed(this);
            var s = Shared.mapRange(distance, 0f, 1000f, ShipBaseItemsMIXIN.GetMinSpeed(this), ms);
            var origAngle = spriteInstance.LookAngle;
            
            FixAndLookAngle(angleToTarget);
            ChangeToSpeed(s,origAngle);

            //test distance
            weaponslot w = null;
            if (testRanges(spriteInstance, currentTarget.spriteInstance, distance, 1500))
            {
                w = getAppropriateWeapon(gt, distance, currentTarget);
                if (w != null)
                {
                    w.fire(gt, parentmap, this);
                }
            }
        }

        private Tuple<float, float> distanceAngleToValid(float startangle, double distance, float changeAmount)
        {
            var workangle = startangle;
            float startdiff = 0;
            while (startdiff < 360 && invalidFuturePosition(workangle, distance))
            {
                startdiff += Math.Abs(changeAmount);
                workangle = VectorMove.wrapAngle(workangle + changeAmount);
            }
            //TODO: value angle, but the turn circle means the ai hits the ground
            if (startdiff >= 360)
                return new Tuple<float, float>(-1, -1);
            return new Tuple<float, float>(startdiff, workangle);
        }

        private bool tooNearGround(VectorMove pos)
        {
            var parentmap = parentArea as map;
            //if the player is near the ground, always turn up
            var highest = parentmap.terrain.getHighestTerrainPoint(pos.Position.GetLeftX(), pos.Position.GetRightX());

            var difheight = -pos.Position.Middle.Y - highest;

            const double minhpP = .1f;
            var MA = ShipBaseItemsMIXIN.GetMaxArmour(this);
            var minhpP2 = MA*minhpP;

            var mindistance = Shared.mapRange(CurrentArmour, minhpP2, MA, minHoverLevel, maxHoverLevel);

            if (difheight < mindistance)
                return true;
            return false;
        }

        private void FixAndLookAngle(float angleToTarget)
        {
            var speed = getTurningSpeed();
            double distance = 70;
            if (invalidFuturePosition(angleToTarget, distance) == false)
            {
                spriteInstance.ChangeLook(angleToTarget, speed);
                return;
            }

            var facingright = spriteInstance.isFacingRight();
            var rotleft = true;

            if (tooNearGround(spriteInstance.move) == false)
            {
                //distance,angle
                var sourceCW = new Tuple<float, float>(-1, -1);
                var sourceCCW = new Tuple<float, float>(-1, -1);

                for (var a = 0; a < 2; a++)
                {
                    //rotate CW from current angle
                    sourceCW = distanceAngleToValid(spriteInstance.move.Angle, distance, -speed);
                    //rotate CCW from current angle
                    sourceCCW = distanceAngleToValid(spriteInstance.move.Angle, distance, speed);
                    distance *= 2;
                    if (sourceCW.Item1 != -1 || sourceCCW.Item1 != -1)
                        break;
                }

                //using the current angle, no change is required
                if (sourceCW.Item1 == -1 && sourceCCW.Item1 == -1)
                {
                    var ang = VectorMove.getAngleToOtherVector(spriteInstance.move.Position.Middle, wantpos);
                    var dif = VectorMove.angleInBetween(spriteInstance.move.Angle, ang);
                    sourceCCW = new Tuple<float, float>(dif, ang);
                    ang = 360 - ang;
                    dif = VectorMove.angleInBetween(spriteInstance.move.Angle, ang);
                    sourceCW = new Tuple<float, float>(dif, ang);
                }

                //if there is no change angle, return, as nothing is required to be changed
                if ((sourceCW.Item1 < sourceCCW.Item2 && sourceCW.Item1 == 0) ||
                    (sourceCW.Item1 > sourceCCW.Item2 && sourceCW.Item2 == 0))
                    return;

                //if a small difference/none always go up to avoid deadlocks
                var diff = VectorMove.angleInBetween(sourceCW.Item1, sourceCCW.Item1);
                if (diff <= 5)
                    rotleft = facingright;
                else
                {
                    //facing right
                    if ((spriteInstance.move.Angle > 270 && spriteInstance.move.Angle <= 360) ||
                        spriteInstance.move.Angle < 90)
                    {
                        if (sourceCW.Item1 < sourceCCW.Item1)
                            rotleft = false;
                        else
                            rotleft = true;
                    }
                    else
                    {
                        if (sourceCW.Item1 < sourceCCW.Item1)
                            rotleft = false;
                        else
                            rotleft = true;
                    }
                }
                spriteInstance.ChangeLook(rotleft, speed);
            }
                //the plane is too close to terrain to be able to do a loop towards the ground, always turn up
            else
            {
                spriteInstance.ChangeLook(90f, speed);
            }
        }

        private ShipInstance getTarget(List<ShipInstance> si)
        {
            double targetdest = -1;
            if (currentTarget != null)
                targetdest = Vector2.Distance(spriteInstance.move.Position.Middle,
                                              currentTarget.spriteInstance.move.Position.Middle);

            //dont constantly switch between targets, allow some distance first
            const int targetchangedist = 2000;
            if (targetdest < targetchangedist && targetdest != -1)
                return currentTarget;

            var postarget = currentTarget;
            //still going after the same target, dont change
            if (si.Count == 1 && si[0] == currentTarget)
                return currentTarget;

            Tuple<ShipInstance, double> shipdist = null;

            foreach (var s in si)
            {
                //sort by distance
                var distance = Vector2.Distance(spriteInstance.move.Position.Middle,
                                                  s.spriteInstance.move.Position.Middle);
                var newsd = new Tuple<ShipInstance, double>(s, distance);

                if (shipdist == null ||
                    (distance < shipdist.Item2 && (distance < targetdest || targetdest == -1)))
                {
                    shipdist = newsd;
                    postarget = s;
                }
            }
            return postarget;
        }

        private weaponslot getAppropriateWeapon(GameTime gt, float distance, ShipInstance target)
        {
            var shoot = (from s in EquipSlots
                                      let w = s.Value.w
                                      //test1-cooldown time
                                      where Shared.TimeSinceElapsed(s.Value.LastShotTimeStamp, gt, w.CoolDown)
                                      //test2-current energy is enough
                                      where CurrentEnergy >= w.EnergyPerShot
                                      let fireangle = WeaponInstance.GetFireAngle(w, this)
                                      let angleToTarget =
                                          VectorMove.getAngleToOtherVector(spriteInstance.move.Position.Middle,
                                                                           target.spriteInstance.move.Position.Middle)
                                      let inAngle = VectorMove.AngleIsBetween(fireangle, angleToTarget, 15)
                                      //test3-firing angle is fine
                                      where inAngle
                                      select s.Value).ToList();

            //no avail weapons
            if (shoot.Count == 0)
                return null;

            //if a beam weapon was used last, and is an option now, use it
            if (WeaponInstance.LastFireWeapon.ContainsKey(ID) && WeaponInstance.LastFireWeapon[ID].weapon.IsBeam)
                return shoot.Find(s => s.w == WeaponInstance.LastFireWeapon[ID].weapon);

            //use a random weapon
            var r = 0;
            if (shoot.Count > 1)
                r = Manager.r.Next()%(shoot.Count - 1);

            /*
			//organise by power?
			if (shoot.Count > 1)
			{
				shoot.Sort(new Model.Comparer<weaponslot>((x, y) => x.w.damage < y.w.damage ? 0 : 1));
			}
            */
            //use first weapon
            return shoot[r];
        }

        private static bool testRanges(SpriteInstance fromsi, SpriteInstance to, float distance, float distMax)
        {
            if (distance > distMax)
                return false;

            //extend a line, and test if it intersects the target
            var ex = new Vector2(fromsi.move.Position.Middle.X, fromsi.move.Position.Middle.Y);
            VectorMove.UpdatePosition(ref ex, fromsi.move.Angle, distance, null);

            var variance = 20;
            var d = getLargerFrameDimension(to.basesprite.FrameWidth, to.basesprite.FrameHeight) + variance;
            if (ex.X > (to.move.Position.Middle.X - d) && ex.X < (to.move.Position.Middle.X + d))
                return true;
            return false;
        }

        private bool invalidFuturePosition(float angle, double distance = 5f)
        {
            var parentmap = parentArea as map;
            var vt = new Vector2(spriteInstance.move.Position.Middle.X, spriteInstance.move.Position.Middle.Y);
            //project the players movement given the current velocity
            VectorMove.UpdatePosition(ref vt, angle, spriteInstance.move.Velocity*distance, null);
            //get the lowest y value possible given any rotation
            var y = getLowestY(vt, spriteInstance.basesprite.FrameWidth, spriteInstance.basesprite.FrameHeight);
            var x1 = (int) vt.X - spriteInstance.basesprite.FrameWidth;
            var x2 = (int) vt.X + spriteInstance.basesprite.FrameWidth;

            if (parentmap.isValidMapBounds(vt) == false)
                return true;

            //test terrain height
            if (tooNearGround(spriteInstance.move))
                return true;

            //test the terrain against this
            for (var x = x1; x < x2; x++)
            {
                if (parentmap.isValidMapBounds(true, x) == false)
                    continue;

                var c = parentmap.terrain.heightmap.heights[x].Count;
                if (parentmap.terrain.heightmap.heights[x][c - 1].Item2 > -y)
                {
                    return true;
                }
            }

            //test ships against this
            foreach (var s in parentmap.ships.Where(s => s.spriteInstance.move != null && s != this))
            {
                if (Collisions.IsSpriteCollision(parentmap, vt, s.spriteInstance.move.Position.Middle))
                    return true;
            }
            wantpos = vt;
            return false;
        }

        private static float getLowestY(Vector2 pos, int FW, int FH)
        {
            var v = getLargerFrameDimension(FW, FH);
            return pos.Y + v + 100;
        }

        private static int getLargerFrameDimension(int FW, int FH)
        {
            return FW > FH
                       ? FW
                       : FH;
        }
    }
}