using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Model.mapInfo;
using Project.View.Client.Cameras;

namespace Project.Model.Instances
{
    public class SpriteInstance
    {
        public SpriteInstance()
        {
        }

        public SpriteInstance(SpriteDraw sd, IshipAreaSynch m)
        {
            parentArea = m;
            scale = 1f;
            basesprite = sd;

            if (sd is SpriteBase)
            {
                currentanimation = ((SpriteBase) basesprite).sprites.First().Value;
                currentFrame = currentanimation.startImageCount;
            }
        }

        public float scale { get; set; }

        #region movement

        public VectorMove move = new VectorMove();
        [XmlIgnore] public IshipAreaSynch parentArea;

        #endregion movement

        #region look

        public float LookAngle;
        public float currentGravity;

        #endregion look

        #region spritebase exclusive

        [XmlIgnore] public readonly SpriteDraw basesprite;
        [XmlIgnore] public int currentFrame;
        [XmlIgnore] public SpriteAnimation currentanimation;

        #endregion spritebase exclusive

        #region animation

        [XmlIgnore] public double LastChangeTime;
        [XmlIgnore] private int millisecondsPerFrame = 1000;

        #endregion animation

        public void Reset()
        {
            move = null;
            LookAngle = -1;
        }

        public void enforceGravity(GameTime gameTime, IDrawableObject ty)
        {
            float gravitymass = 0;
            if (ty is ShipInstance)
                gravitymass = ShipInstance.GravityMass;
            else if (ty is BuildingInstance)
                gravitymass = BuildingInstance.GravityMass;
            else if (ty is WeaponInstance)
                gravitymass = WeaponInstance.GravityMass;

            IshipAreaSynch isa = ty.parentArea as map;

            //disabled
            if (gravitymass <= 0 || isa == null)
                return;
            var m = isa as map;

            if (ty is BuildingInstance)
            {
                var bty = ty as BuildingInstance;
                if (bty.falling)
                {
                    var h = bty.spriteInstance.move.Position.Middle.Y +
                              (int) (bty.buildingModel.BaseSprite.FrameHeight/2f);
                    var th = -1*
                             m.terrain.heightmap.heights[(int) bty.spriteInstance.move.Position.Middle.X].Last().Item2;
                    if (h < th)
                    {
                        bty.falldistance++;
                    }
                    else if (h >= th || h >= -mapTerrain.SeaLevel)
                    {
                        m.dealDamage(bty, bty.falldistance*BuildingInstance.falldistdmg);
                        bty.falling = false;
                    }
                }
                    //no gravity if the building is not falling
                else
                    return;
            }

            var gi = m.gravityIncrement*gravitymass;
            var gki = m.gravityKickIn /= gravitymass;
            var mg = m.maxGravity*gravitymass;

            currentGravity = (float) Shared.mapRange(move.Velocity, 0, gki, mg, 0);

            //move all items down with the gravity
            if (currentGravity > 0)
            {
                //TEMP?-only rotate ships to face down 
                if (ty is ShipInstance)
                    VectorMove.UpdateAngle(ref LookAngle, 270, gi);

                VectorMove.UpdatePosition(ref move.Position.Middle, 270, gi);
                //VectorMove.UpdateAngle(ref move.Angle,270,gi);
            }
        }

        public void changeCurrentAnimation(SpriteAnimation newanim)
        {
            currentanimation = newanim;
            currentFrame = newanim.startImageCount;
        }

        public List<string> SerialisePosition()
        {
            var o = new List<string>();
            o.AddRange(move.SerialisePosition());
            o.Add(LookAngle.ToString());
            o.Add(currentGravity.ToString());
            return o;
        }

        public static Tuple<VectorMove, float, float> DeserialisePosition(List<string> args)
        {
            var move = VectorMove.DeserialisePosition(args);
            var ang = float.Parse(Shared.PopFirstListItem(args));
            var cg = float.Parse(Shared.PopFirstListItem(args));

            return new Tuple<VectorMove, float, float>(move, ang, cg);
        }

        public static void DeserialisePosition(List<string> args, SpriteInstance si)
        {
            var tup = DeserialisePosition(args);
            si.move = tup.Item1;
            si.LookAngle = tup.Item2;
            si.currentGravity = tup.Item3;
        }

        public bool isFacingRight()
        {
            return ((move.Angle > 270 && move.Angle <= 360) || move.Angle < 90);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (move.Velocity == 0 && currentGravity == 0)
                return;

            move.UpdatePosition(gameTime);
        }

        public void ChangeLook(float desiredAngle, float turnSpeed)
        {
            VectorMove.UpdateAngle(ref LookAngle, desiredAngle, turnSpeed);
        }

        public void ChangeLook(bool lookleft, float turnSpeed)
        {
            VectorMove.UpdateAngle(ref LookAngle, lookleft, turnSpeed);
        }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            if (basesprite == null)
                return;

            var isSpriteBase = basesprite is SpriteBase;
            //bool isParticleBase = basesprite is SpriteParticle;
            var SB = basesprite;
            var SB1 = basesprite as SpriteBase;
            //var SB2 = basesprite as SpriteParticle;
            var cy = 0;
            var cx = 0;
            if (isSpriteBase)
            {
                cy = currentFrame/SB1.columnCount;
                cx = currentFrame%SB1.columnCount;
            }

            var r = LookAngle/180f;
            r = r*3.14159f;
            /*
            var FH = SpriteEffects.FlipHorizontally;
            if (true)
                FH = SpriteEffects.None;

            var FV = SpriteEffects.FlipVertically;
            if (true)
                FV = SpriteEffects.None;
            */
            //scale = 1f;
            var middle = new Vector2(((float) SB.FrameWidth/2), ((float) SB.FrameHeight/2));
            var load = SB.image;

            cam.spriteBatch.Draw(load,
                                 move.Position.Middle,
                                 new Rectangle(cx*SB.FrameWidth,
                                               cy*SB.FrameHeight,
                                               SB.FrameWidth, SB.FrameHeight), Color.White
                                 , -r, middle, scale, 0, 0);

            if (isSpriteBase && Shared.TimeSinceElapsed(ref LastChangeTime, gameTime, millisecondsPerFrame))
                incrementFrame(SB1);
        }

        private void incrementFrame(SpriteBase SB)
        {
            currentFrame++;
            if (currentFrame > currentanimation.endImageCount || currentFrame == SB.imageCount)
                currentFrame = currentanimation.startImageCount;
        }
    }
}