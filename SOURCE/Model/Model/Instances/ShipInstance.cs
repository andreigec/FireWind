using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ExternalUsage;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.Instances;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;

namespace Project
{
    public partial class ShipInstance : SharedID, IDrawableObject, ShipBaseItems
    {
        [XmlIgnore]
        public static float GravityMass = 1;

        #region current

        [XmlIgnore]
        private const double ShieldExtendSeconds = 5;
        [XmlIgnore]
        public double CurrentAccel;
        [XmlIgnore]
        public double CurrentArmour;

        [XmlIgnore]
        public double CurrentEnergy;
        [XmlIgnore]
        public double CurrentShield;

        public SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart> Parts { get; set; }

        [XmlIgnore]
        private double ShowShieldTill;

        public SerializableDictionary<SlotLocation.SlotLocationEnum, weapon> Slots { get; set; }

        [XmlIgnore]
        public SerializableDictionary<SlotLocation.SlotLocationEnum, weaponslot> EquipSlots { get; set; }

        [XmlIgnore]
        public bool disabled;

        [XmlIgnore]
        public InstanceOwner instanceOwner { get; set; }

        public shipBase shipModel { get; set; }

        [XmlIgnore]
        public IshipAreaSynch parentArea { get; set; }

        [XmlIgnore]
        public SpriteInstance spriteInstance { get; set; }
        [XmlIgnore]
        public GameControlServer parentGCS { get; set; }
        #endregion current

        public ShipInstance()
        {
        }

        private ShipInstance(GameControlServer gcs, shipBase shipmodel, SetID idcfg, IshipAreaSynch area = null, VectorMove move = null,
                             float look = -1, InstanceOwner controller = null)
        {
            parentGCS = gcs;
            Parts = new SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart>();
            Slots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weapon>();
            EquipSlots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weaponslot>();

            ISynchInterfaceMIXIN.InitClass(this);
            shipModel = shipmodel;
            spriteInstance = new SpriteInstance(shipmodel.basesprite, parentArea);
            //reset values now
            Reset();

            if (move != null)
                spriteInstance.move = move.Clone();

            spriteInstance.LookAngle = look;
            setID(idcfg);
            if (controller != null)
                instanceOwner = controller;
            parentArea = area;

            //give all ship the stock parts by default
            var stockparts = ShipInstanceShell.CreateStockShellShip();
            AssignShellPartsToThis(this, stockparts);
        }

        #region IDrawableObject Members



        public void Draw(Camera2D cam, GameTime gameTime)
        {
            //draw shield
            DrawShield(cam, gameTime);
            /*
            //debug:target square
            Rectangle r;
            r = new Rectangle((int)wantpos.X, (int)wantpos.Y, 20, 20);
            cam.spriteBatch.Draw(XNA.PixelTexture, r, Color.Pink);
            */
            spriteInstance.Draw(cam, gameTime);
        }

        #endregion

        /*
		public static ShipInstance addShip(string shipname, sector addto, InstanceOwner controller)
		{
			var sm = loadXML.loadedShips[shipname];
			var si = new ShipInstance(sm, addto, null, null, controller, -1);
			si.parentArea = addto;
			addto.ships.Add(si);
			return si;
		}
        
        public static ShipInstance addShip(ShipInstance oldsi, IshipArea addto, SetID idcfg)
        {
            var sm = loadXML.loadedShips[oldsi.shipModel.name];
            var si = new ShipInstance(sm,idcfg, addto, oldsi.spriteinstance.move, oldsi.spriteinstance.look, oldsi.instanceOwner);
            si.parentArea = addto;
            addto.ships.Add(si);
            return si;
        }
        */

        public static ShipInstance addShip(GameControlServer gcs, string shipname, SetID idcfg)
        {
            var sm = loadXML.loadedShips[shipname];
            var si = new ShipInstance(gcs, sm, idcfg);
            return si;
        }

        public static ShipInstance addShip(string shipname, SetID idcfg, IshipAreaSynch addto)
        {
            var si = addShip(addto.parentGCS, shipname, idcfg);
            addto.ships.Add(si);
            si.parentArea = addto;
            return si;
        }

        public static ShipInstance addShip(ShipInstanceShell shell, SetID idcfg, IshipAreaSynch addto)
        {
            var si = addShip(addto.parentGCS, shell.shipModel.name, idcfg);
            addto.ships.Add(si);
            si.parentArea = addto;

            AssignShellPartsToThis(si, shell);

            return si;
        }

        public void dealDamage(GameTime gt, WeaponInstance w)
        {
            var dmg = w.weapon.Damage;
            //offset damage by shield
            if (CurrentShield > 0)
            {
                //more shield than dmg
                if ((CurrentShield - dmg) >= 0)
                {
                    CurrentShield -= dmg;
                    ShowShieldTill = gt.TotalGameTime.TotalSeconds + ShieldExtendSeconds;
                    return;
                }
                //otherwise subtract what the shield can block, and continue with the rest
                var ch = CurrentShield;
                CurrentShield -= ch;
                dmg -= ch;
            }
            ShowShieldTill = 0;

            CurrentArmour -= dmg;
            if (CurrentArmour <= 0)
            {
                CurrentArmour = 0;
                disabled = true;
                parentGCS.GiveCreditForShipDestruction(w.parentID, this);
            }
        }

        /// <summary>
        /// assign all the stock weapons and parts to the ship, init weaponslots for each weapon
        /// </summary>
        /// <param name="si"></param>
        /// <param name="shell"></param>
        public static void AssignShellPartsToThis(ShipInstance si, ShipInstanceShell shell)
        {
            si.Parts = new SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart>();

            foreach (var p in shell.Parts)
            {
                si.AssignPart(p.Value);
            }

            si.Slots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weapon>();
            si.EquipSlots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weaponslot>();

            foreach (var w in shell.Slots)
            {
                si.AssignWeapon(w.Key, w.Value);
            }
        }

        public void AssignWeapon(SlotLocation.SlotLocationEnum port, weapon w)
        {
            if (Slots.ContainsKey(port))
                Slots[port] = null;

            EquipSlots[port] = new weaponslot(w);
            Slots[port] = w;
        }

        public void AssignBase(shipBase sb)
        {
            shipModel = sb;
        }

        public bool AssignPart(ShipPart addthis)
        {
            var sp = addthis.ShipPartToEnum();

            if (Parts.ContainsKey(sp))
                Parts[sp] = null;

            Parts[sp] = addthis;
            return true;
        }

        public void Reset()
        {
            CurrentShield = ShipBaseItemsMIXIN.GetMaxShield(this);
            CurrentEnergy = ShipBaseItemsMIXIN.GetMaxEnergy(this);
            CurrentArmour = ShipBaseItemsMIXIN.GetMaxArmour(this);

            parentArea = null;
            disabled = false;
            CurrentAccel = 0;
            currentTarget = null;

            spriteInstance.Reset();

            //reset weapon slot attributes like energy/cooldown etc
            EquipSlots = new SerializableDictionary<SlotLocation.SlotLocationEnum, weaponslot>();
            foreach (var s in Slots)
            {
                EquipSlots.Add(s.Key, new weaponslot(s.Value));
            }
        }

        public float getTurningSpeed()
        {
            var ms = ShipBaseItemsMIXIN.GetMaxSpeed(this);
            if (ms <= 0)
                return 0;

            var s1 = (float)spriteInstance.move.Velocity / (float)ms;
            var mts = ShipBaseItemsMIXIN.GetMaxTurnSpeed(this);
            var s2 =
                (s1 * mts);
            if (s2 > mts)
                s2 = mts;
            return (float)s2;
        }

        public void ChangeToSpeed(double WantSpeed, float origLookAngle)
        {
            //cant go faster than the max
            var maxspeed = ShipBaseItemsMIXIN.GetMaxSpeed(this);
            if (WantSpeed > maxspeed)
                WantSpeed = maxspeed;

            var currvel = spriteInstance.move.Velocity;

            //if there is no difference, return
            if ((int)WantSpeed == (int)currvel)
                return;

            //diff between current and wanted speed
            var dif = WantSpeed - currvel;

            //if there is negative diff, we are trying to brake, cap difference to max brake increment
            var maxturn = ShipBaseItemsMIXIN.GetMaxTurnSpeed(this);
            var turndif = VectorMove.angleInBetween(origLookAngle, spriteInstance.LookAngle);
            if (dif < 0)
            {
                var mb = ShipBaseItemsMIXIN.GetBrakeIncr(this);
                if ((-dif) > mb)
                    dif = -mb;
            }
            //otherwise its accel, cap to max accel increment
            else
            {
                var ai = ShipBaseItemsMIXIN.GetAccelIncr(this);
                if (dif > ai)
                    dif = ai;

                //remove some accel if turning
                dif = Shared.mapRange(turndif, 0, maxturn, dif / 2f, dif);
            }

            //change the current accel based on the capped diff
            CurrentAccel += dif;

            //cap the currentaccel to max accel, dont cap max brake if negative
            var ma = ShipBaseItemsMIXIN.GetMaxAccel(this);
            if (CurrentAccel > ma)
                CurrentAccel = ma;

            //update the velocity with the updated current acceleration
            spriteInstance.move.UpdateVelocity(CurrentAccel, 0, maxspeed);

            //get the new velocity
            currvel = spriteInstance.move.Velocity;

            //reset the brake if already going 0
            if (CurrentAccel < 0 && currvel == 0)
                CurrentAccel = 0;
        }

        public void DrawShield(Camera2D cam, GameTime gameTime)
        {
            var ts = gameTime.TotalGameTime.TotalSeconds;
            if (ShowShieldTill < ts)
                return;
            var shieldlength = ShowShieldTill - ts;
            //colour depends on the remaining length of the shield
            var b = (int)Shared.mapRange(shieldlength, 0, ShieldExtendSeconds, 0, 255);
            var c = new Color(50, 50, b);

            //width depends on remaining shield
            var width = (int)Shared.mapRange(CurrentShield, 0, ShipBaseItemsMIXIN.GetMaxShield(this), 1, 5);

            XNA.DrawEllipse(cam.spriteBatch, spriteInstance.basesprite.FrameWidth, spriteInstance.basesprite.FrameHeight,
                            0, c, width, spriteInstance.move.Position.Middle);
        }

        public bool AIControllingPlane()
        {
            return (outOfBounds || instanceOwner.BeingControlled() == false || forceAI);
        }

        public static bool IsOnMap(ShipInstance si, map mIN)
        {
            if (si == null || si.parentArea == null || ((si.parentArea is map) == false))
                return false;

            var m = si.parentArea as map;
            return m == mIN;
        }

        public void UpdateMap(GameTime gameTime, map m)
        {
            if (disabled == false)
            {
                if (instanceOwner.BeingControlled())
                {
                    if (m.isValidMapBounds(spriteInstance.move.Position.Middle) == false)
                        outOfBounds = true;
                    else
                    {
                        outOfBounds = false;
                    }
                }

                if (AIControllingPlane())
                    updateAI(gameTime);
            }

            //update player moving position based on look
            if (disabled)
            {
                //make sure when the player is disabled, the only moving force is gravity
                var origAngle = spriteInstance.LookAngle;
                VectorMove.UpdateAngle(ref spriteInstance.LookAngle, 270, 1);

                ChangeToSpeed(0, origAngle);

                VectorMove.UpdateAngle(ref spriteInstance.move.Angle, spriteInstance.LookAngle, getTurningSpeed());
            }
            else
            {
                UpdateShipEnergy(gameTime);
                VectorMove.UpdateAngle(ref spriteInstance.move.Angle, spriteInstance.LookAngle, getTurningSpeed());
            }

            spriteInstance.enforceGravity(gameTime, this);

                spriteInstance.Update(gameTime);
        }

        public void UpdateShipEnergy(GameTime gt)
        {
            var me = ShipBaseItemsMIXIN.GetMaxEnergy(this);
            var regen = ShipBaseItemsMIXIN.GetEnergyRegen(this);
            CurrentEnergy += regen * (gt.ElapsedGameTime.TotalMilliseconds * .01);
            if (CurrentEnergy > me)
                CurrentEnergy = me;

            var ms = ShipBaseItemsMIXIN.GetMaxShield(this);
            if (CurrentEnergy == me && CurrentShield < ms)
            {
                var dif = ms - CurrentShield;
                //cap energy regen to max energy
                if (dif > me)
                    dif = me;
                CurrentEnergy -= dif;
                CurrentShield += dif;
            }
        }

        public bool ChangeArea(IshipAreaSynch newarea)
        {
            parentArea.ships.Remove(this);
            newarea.ships.Add(this);
            parentArea = newarea;
            return true;
        }

        public static void CreateBuildingExitVector(BuildingInstance bi, ShipInstance si)
        {
            var m = bi.parentArea as map;
            //exit velocity is the max speed possible
            var maxs = ShipBaseItemsMIXIN.GetMaxSpeed(si);

            var r = new Rect(bi.spriteInstance.move.Position.Middle, bi.spriteInstance.basesprite.FrameWidth,
                             bi.spriteInstance.basesprite.FrameHeight);

            r.Middle.Y -= 100;
            si.spriteInstance.move = new VectorMove(90, maxs, r);
            si.spriteInstance.LookAngle = 90;
        }
    }
}