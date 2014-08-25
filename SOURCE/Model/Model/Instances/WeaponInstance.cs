using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;

namespace Project.Model.Instances
{
    public class PreviousWeaponInstance
    {
        public WeaponInstance instance;
        public weapon weapon;
    }

    public partial class WeaponInstance : SharedID, IDrawableObject
    {
        public static float GravityMass;

        /// <summary>
        /// Ship parent ID to their last fired weapon to check to see if a beam can be formed
        /// </summary>
        public static Dictionary<long, WeaponInstance> LastFireWeapon = new Dictionary<long, WeaponInstance>();

        public static double LastFireTime;

        #region stats

        //make sure the player ship doesnt hit itself when firing, it can when its out of immediate danger once though
        public long TempInvinShipID;

        #endregion endstats

        /// <summary>
        /// for beam weapons - set these internally
        /// </summary>
        public long nextBeamInstanceID = -1;

        public long parentID;
        public long previousBeamInstanceID = -1;

        private WeaponInstance(GameControlServer gcs, weapon w, Map m, VectorMove vm, long parentShipID, SetID cfg)
        {
            parentGCS = gcs;
            ISynchInterfaceMIXIN.InitClass(this);

            weapon = w;
            spriteInstance = new SpriteInstance(w.BaseSprite, m);

            if (w.BaseSprite != null)
                spriteInstance.scale = ((SpriteParticle) w.BaseSprite).scale;

            if (vm != null)
                spriteInstance.move = vm.Clone();

            setID(cfg);
            parentID = parentShipID;
            parentArea = m;
            nextBeamInstanceID = previousBeamInstanceID = -1;

            //for beam weapons
            double tms = Manager.GetMillisecondsNow();

            if (w.IsBeam)
            {
                double dif = tms - LastFireTime;
                //make sure the last fired weapon is the same type, and that the cooldown is not too much to make a beam
                if (LastFireWeapon.ContainsKey(parentShipID) && LastFireWeapon[parentShipID].weapon == w &&
                    dif < w.CoolDown*1.5)
                {
                    previousBeamInstanceID = LastFireWeapon[parentShipID].ID;
                    LastFireWeapon[parentShipID].nextBeamInstanceID = ID;
                }
            }
            if (LastFireWeapon.ContainsKey(parentShipID) == false)
                LastFireWeapon.Add(parentShipID, this);
            else
                LastFireWeapon[parentShipID] = this;

            LastFireTime = tms;
        }

        public weapon weapon { get; private set; }

        #region IDrawableObject Members

        [XmlIgnore]
        public GameControlServer parentGCS { get; set; }

        public IshipAreaSynch parentArea { get; set; }
        public SpriteInstance spriteInstance { get; set; }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            spriteInstance.Draw(cam, gameTime);
        }

        #endregion

        public int GetWeaponFrameSize()
        {
            if (weapon.BaseSprite != null)
                return weapon.BaseSprite.FrameHeight > weapon.BaseSprite.FrameWidth
                           ? weapon.BaseSprite.FrameHeight
                           : weapon.BaseSprite.FrameWidth;
            else
                return 10;
        }

        public void Update(GameTime gameTime)
        {
            //for now weapons always spin one angle per update
            //spriteinstance.ChangeLook(true, 1);

            spriteInstance.enforceGravity(gameTime, this);
            //move shot
            spriteInstance.Update(gameTime);
        }

        public static float GetFireAngle(weapon w, ShipInstance si)
        {
            return VectorMove.wrapAngle(si.spriteInstance.LookAngle + w.AngleOfFire);
        }

        public static WeaponInstance addShot(Map m, ShipInstance si, weapon w)
        {
            var vm = new VectorMove(si.spriteInstance.move.Position);
            float ang = si.spriteInstance.LookAngle;
            ang = GetFireAngle(w, si);

            vm.setAngleAndVelocity(ang,
                                   si.spriteInstance.move.Velocity + si.spriteInstance.currentGravity + w.Velocity);

            long owner = si.ID;
            var wi = new WeaponInstance(m.parentGCS, w, m, vm, owner, SetID.CreateSetNew());
            wi.TempInvinShipID = owner;

            m.shots.Add(wi);
            return wi;
        }
    }
}