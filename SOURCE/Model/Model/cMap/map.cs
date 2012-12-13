using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.Instances;
using Project.Model.mapInfo;
using Project.Networking;

namespace Project
{
    public partial class map : SharedID, IshipAreaSynch
    {
        [XmlIgnore]
        public sector parentSector;

        #region map

        [XmlIgnore]
        public const int deadzone = 600;
        [XmlIgnore]
        public int deadzoneMaxX;
        [XmlIgnore]
        public int deadzoneMaxY;
        [XmlIgnore]
        public int deadzoneMinX = deadzone;
        [XmlIgnore]
        public SectorConfig Config;

        public int height;
        public mapTerrain terrain = new mapTerrain();
        public int terrainSeed = -1;
        public int width;

        private map()
        {
        }

        [XmlIgnore]
        public List<WeaponInstance> shots { get; set; }

        [XmlIgnore]
        public List<BuildingInstance> buildings { get; set; }

        [XmlIgnore]
        public List<ShipInstance> ships { get; set; }

        [XmlIgnore]
        public GameControlServer parentGCS { get; set; }

        #region gravity

        public float gravityIncrement = 2f;
        public float gravityKickIn = 4f;
        public float maxGravity = 100f;

        #endregion gravity

        /*
		public map(sector parentSector, int widthIN, int heightIN, bool createBuildings, int seed)
		{
			ships = new List<ShipInstance>();
			this.parentSector = parentSector;
			buildings = new List<BuildingInstance>();
			//map
			width = widthIN;
			height = heightIN;
			deadzoneMaxX = width - deadzone;
			deadzoneMaxY = -height + deadzone;
			terrainSeed = seed;
			terrain.init(this, createBuildings, seed);
			setID(-1);

			if (parentSector.parentRegion != null)
				parentgcs = parentSector.parentRegion.parentServer;
		}
        */

        public static map CreateMap(sector parentSector, int widthIN, int heightIN, SectorConfig config, int seed,
                                    SetID mapIDcfg)
        {
            var m = new map();
            IShipAreaMIXIN.InitClass(m);
            ISynchInterfaceMIXIN.InitClass(m);

            m.parentSector = parentSector;
            if (parentSector.parentRegion != null)
                m.parentGCS = parentSector.parentGCS;

            //map
            m.width = widthIN;
            m.height = heightIN;
            m.deadzoneMaxX = m.width - deadzone;
            m.deadzoneMaxY = -m.height + deadzone;
            m.terrainSeed = seed;
            m.terrain.init(m, config.HasBuildings(), seed);
            m.setID(mapIDcfg);
            m.Config = config;

            return m;
        }

        public void Update(GameTime gameTime)
        {
            var um = parentGCS.UPDATEFLAGS;
            //update map 

            terrain.Update(gameTime, um);

            updateShots(gameTime, um);
            updateShips(gameTime, um);
            updateBuildings(gameTime, um);
        }

        private void updateBuildings(GameTime gameTime, UpdateModes um)
        {
            if (Shared.flagChecked(um, UpdateModes.MoveObjects) == false)
                return;

            for (var a = 0; a < buildings.Count; a++)
            {
                var b = buildings[a];

                //check armour
                if (b.armour <= 0)
                {
                    RemoveBuilding(b);
                    a--;
                    continue;
                }
                b.Update(gameTime);
            }
        }

        private void updateShips(GameTime gameTime, UpdateModes um)
        {
            if (Shared.flagChecked(um, UpdateModes.MoveObjects) == false)
                return;

            for (var a = 0; a < ships.Count; a++)
            {
                var s = ships[a];

                //if the ship hasnt been given a position, dont update
                if (s.spriteInstance.move == null)
                    continue;

                //dont update a ship that is being controlled by someone else
                //if (s.instanceOwner.BeingControlled()&&s.instanceOwner.BeingControlledBy(Manager.synchMain.myID)==false)
                //continue;

                if (Shared.flagChecked(um, UpdateModes.CheckHitboxes))
                {
                    //check buildings
                    var remove = false;
                    /*
                    if (isValidMapBounds(s.spriteinstance.move.Position.Middle, true) == false)
                    {
                        RemoveShip(s);
                        goto redo;
                    }
                    */
                    foreach (var b in buildings)
                    {
                        if (Collisions.IsSpriteCollision(this, s.spriteInstance.move.Position.Middle,
                                                         b.spriteInstance.move.Position.Middle))
                        {
                            dealDamage(b, s);
                            remove = true;
                        }
                    }

                    //if the ship hits the terrain, explode
                    if (Shared.flagChecked(um, UpdateModes.RemoveOutOfBounds) &&
                        Collisions.isTerrainCollisionBlock(this, s.spriteInstance.move.Position.Middle,
                                                           ShipBaseItemsMIXIN.GetShipFrameSize(s)))
                    {
                        terrain.removeTerrain(s.spriteInstance.move.Position.Middle, 50);
                        remove = true;
                    }

                    if (remove)
                    {
                        //set armour to 0 so we can prompt client to charge for the ship regen
                        s.CurrentArmour = 0;
                        GameControlClient.ResetShip(s, parentSector.parentRegion, true);
                        a--;
                        continue;
                    }
                }
                s.UpdateMap(gameTime, this);
            }
        }

        public void RemoveBuilding(BuildingInstance bi)
        {
            RemoveBuildings.Add(bi);
            buildings.Remove(bi);
        }

        public void RemoveShot(WeaponInstance wi)
        {
            //previous->next(this)
            //remove the previous' ref to this item in the linked list
            if (wi.previousBeamInstanceID != -1)
            {
                var wip = GetShot(wi.previousBeamInstanceID);
                if (wip != null && wip.nextBeamInstanceID != -1)
                {
                    var win = GetShot(wip.nextBeamInstanceID);
                    if (win == wi)
                    {
                        wip.nextBeamInstanceID = -1;
                    }
                }
            }

            RemoveShots.Add(wi);
            if (shots.Contains(wi))
                shots.Remove(wi);
        }

        public WeaponInstance GetShot(long shotID)
        {
            return shots.Find(s => s.ID == shotID);
        }

        private bool CheckBuildingsRemoveShot(WeaponInstance wi, GameTime gameTime, UpdateModes um, bool checkrad,
                                              bool dealdamage)
        {
            var remove = false;
            if (Shared.flagChecked(um, UpdateModes.CheckHitboxes))
            {
                //check buildings
                foreach (var b in buildings)
                {
                    var col = false;

                    if (checkrad == false)
                        col = Collisions.IsSpriteCollision(this, b.spriteInstance.move.Position.Middle,
                                                           wi.spriteInstance.move.Position.Middle);
                    else
                        col = Collisions.CheckLineCollisionRadius(b.spriteInstance.move.Position.Middle,
                                                                  wi.spriteInstance.move.Position.Middle,
                                                                  wi.weapon.ExplosiveRadius);
                    if (col)
                    {
                        remove = true;

                        if (checkrad)
                        {
                            b.falling = true;
                            b.falldistance = 0;
                        }

                        if (dealdamage)
                            dealDamage(b, wi.weapon);
                    }
                }
            }
            return remove;
        }

        private bool CheckShipsRemoveShot(WeaponInstance wi, GameTime gameTime, UpdateModes um, bool checkrad,
                                          bool dealdamage)
        {
            var remove = false;
            if (Shared.flagChecked(um, UpdateModes.CheckHitboxes))
            {
                var hit = false;
                //check buildings
                foreach (var sh in ships)
                {
                    var col = false;

                    if (checkrad == false || wi.weapon.ExplosiveRadius == 0)
                        col = Collisions.IsSpriteCollision(this, sh.spriteInstance.move.Position.Middle,
                                                           wi.spriteInstance.move.Position.Middle);
                    else
                        col = Collisions.CheckLineCollisionRadius(sh.spriteInstance.move.Position.Middle,
                                                                  wi.spriteInstance.move.Position.Middle,
                                                                  wi.weapon.ExplosiveRadius);
                    if (col)
                    {
                        //dont hit player ship who fired
                        if (wi.TempInvinShipID == sh.ID)
                        {
                            hit = true;
                            continue;
                        }

                        remove = true;
                        if (dealdamage)
                            sh.dealDamage(gameTime, wi);
                    }
                }

                //if no ship was in the radius of a shot, remove the temp invun for that ship
                if (hit == false)
                    wi.TempInvinShipID = -1;
            }
            return remove;
        }

        private void updateShots(GameTime gameTime, UpdateModes um)
        {
            if (Shared.flagChecked(um, UpdateModes.MoveObjects) == false)
                return;

        redo:
            for (var a = 0; a < shots.Count; a++)
            {
                if (a >= shots.Count)
                    break;

                var s = shots[a];
                if (s == null)
                    continue;

                if (Shared.flagChecked(um, UpdateModes.RemoveOutOfBounds) &&
                    (isValidMapBounds(s.spriteInstance.move.Position.Middle, true) == false))
                {
                    RemoveShot(s);
                    goto redo;
                }

                if (Shared.flagChecked(um, UpdateModes.CheckHitboxes) &&
                    Shared.flagChecked(um, UpdateModes.UpdateTerrain))
                {
                    //first see if there is a direct impact with a ship or building
                    var removebuildhit = CheckBuildingsRemoveShot(s, gameTime, um, false, false);
                    var removeshiphit = CheckShipsRemoveShot(s, gameTime, um, false, false);
                    var terrainhit = Collisions.isTerrainCollisionRadius(this, s.spriteInstance.move.Position.Middle,
                                                                          s.GetWeaponFrameSize());

                    var remove = removebuildhit || removeshiphit || terrainhit;

                    //if an item was hit, project the explosive radius. if not, check to see if the terrain was hit
                    if (remove)
                    {
                        CheckBuildingsRemoveShot(s, gameTime, um, true, true);
                        CheckShipsRemoveShot(s, gameTime, um, true, true);
                        terrain.removeTerrain(s.spriteInstance.move.Position.Middle, s.weapon.ExplosiveRadius);

                        RemoveShot(s);
                        a--;
                        continue;
                    }
                }

                s.Update(gameTime);
            }
        }

        public void dealDamage(BuildingInstance b, int dmg)
        {
            b.armour -= dmg;
            if (b.armour < 0)
                b.armour = 0;
        }

        public void dealDamage(BuildingInstance b, weapon w)
        {
            b.armour -= w.Damage;
            if (b.armour < 0)
                b.armour = 0;
        }

        public void dealDamage(BuildingInstance b, ShipInstance s)
        {
            b.armour -= ShipBaseItemsMIXIN.GetMaxArmour(s);
            if (b.armour < 0)
                b.armour = 0;
        }

        public bool isValidMapBounds(bool isX, double val, bool realBounds = false)
        {
            if (isX)
            {
                if (realBounds == false)
                    return val > deadzoneMinX && val < deadzoneMaxX;
                else
                    return val > 0 && val < width;
            }
            else
            {
                if (realBounds == false)
                    return val < -mapTerrain.SeaLevel && val > deadzoneMaxY;
                else
                    return val < -mapTerrain.SeaLevel && val > -height;
            }
        }

        public bool isValidMapBounds(Vector2 pos, bool realBounds = false)
        {
            return isValidMapBounds(true, pos.X, realBounds) && isValidMapBounds(false, pos.Y, realBounds);
        }

        public void wrapBounds(ref int X, ref int Y)
        {
            if (X < 0)
                X = 0;
            else if (X > width)
                X = width;

            if (Y > 0)
                Y = 0;
            else if (Y < -height)
                Y = -height;
        }

        public BuildingInstance getSuitableHangarShipRelease()
        {
            //foreach (var bi in buildings.Where(s=>s.buildingType== BuildingInstance.BuildingType.ShipHangar))
            foreach (var bi in buildings)
            {
                //check clear sky before eject
                var col =
                    ships.Any(
                        sh =>
                        sh.spriteInstance.move != null &&
                        Collisions.CheckLineCollisionRadius(sh.spriteInstance.move.Position.Middle,
                                                            bi.spriteInstance.move.Position.Middle, 200));

                //we dont want to eject in a greyed out part of the map
                if (isValidMapBounds(bi.spriteInstance.move.Position.Middle) == false)
                    col = true;

                if (col)
                    continue;

                return bi;
            }
            return null;
        }

        public VectorMove getSuitableJoinGameVector(ShipInstance si)
        {
            var col = true;
            var r = new Random();
            var ret = new VectorMove();
            while (col)
            {
                try
                {
                    //get random pos
                    var x = deadzoneMinX + 1 + r.Next() % (deadzoneMaxX - deadzoneMinX - 5);
                    //var enterpos = new Vector2(x, deadzoneMaxY+1);
                    var enterpos = new Vector2(x, -3000);
                    //check clear sky before eject
                    col =
                        ships.Any(
                            sh =>
                            sh.spriteInstance.move != null &&
                            Collisions.CheckLineCollisionRadius(sh.spriteInstance.move.Position.Middle, enterpos, 200));

                    //we dont want to eject in a greyed out part of the map
                    if (isValidMapBounds(enterpos) == false)
                        col = true;

                    if (col == false)
                    {
                        //max speed
                        var maxs = ShipBaseItemsMIXIN.GetMaxSpeed(si);
                        ret = new VectorMove(270, maxs,
                                             new Rect(enterpos, si.shipModel.basesprite.FrameWidth,
                                                      si.shipModel.basesprite.FrameHeight));
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    Manager.FireLogEvent("error getting join game vector", SynchMain.MessagePriority.Low,
                                     true);
                    col = true;
                }
            }
            return null;
        }

        #endregion map
    }
}