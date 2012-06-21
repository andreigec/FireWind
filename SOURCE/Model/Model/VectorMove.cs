using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Project.Networking;

namespace Project.Model
{
    public class Rect
    {
        public int Height = -1;
        public Vector2 Middle = Vector2.Zero;
        public int Width = -1;

        public Rect(float middleX, float middleY, int width, int height)
        {
            Middle = new Vector2(middleX, middleY);
            Width = width;
            Height = height;
        }

        public Rect(Vector2 xy, int width, int height)
        {
            Middle = xy;
            Width = width;
            Height = height;
        }

        public Rect()
        {
        }

        public Vector2 GetLowestBottomLeft()
        {
            return new Vector2(GetLeftX(), GetLowestY());
        }

        public Rectangle AsRectangle()
        {
            return new Rectangle((int) Middle.X, (int) Middle.Y, Width, Height);
        }

        public float GetLeftX()
        {
            return Middle.X - (Width/2f);
        }

        public float GetRightX()
        {
            return Middle.X + (Width/2f);
        }

        public float GetLowestY()
        {
            return Middle.Y + (Height/2f);
        }

        public float GetHighestY()
        {
            return Middle.Y - (Height/2f);
        }
    }

    public class VectorMove
    {
        public float Angle = -1;
        public Rect Position = new Rect();
        public double Velocity = -1;

        public VectorMove(Rect pos)
        {
            Position = pos;
            Velocity = 0f;
            Angle = 0f;
        }

        public VectorMove(float angle, double velocity, Rect position)
        {
            Angle = angle;
            Velocity = velocity;
            Position = position;
        }

        public VectorMove()
        {
        }

        public List<string> SerialisePosition()
        {
            var o = new List<string>();
            o.Add(Angle.ToString());
            o.Add(Velocity.ToString());
            o.Add(Position.Width.ToString());
            o.Add(Position.Height.ToString());
            o.Add(Position.Middle.X.ToString());
            o.Add(Position.Middle.Y.ToString());
            return o;
        }

        public static VectorMove DeserialisePosition(List<string> args)
        {
            var angle = float.Parse(Shared.PopFirstListItem(args));
            var velocity = double.Parse(Shared.PopFirstListItem(args));
            var width = int.Parse(Shared.PopFirstListItem(args));
            var height = int.Parse(Shared.PopFirstListItem(args));
            var mx = float.Parse(Shared.PopFirstListItem(args));
            var my = float.Parse(Shared.PopFirstListItem(args));

            return new VectorMove(angle, velocity, new Rect(mx, my, width, height));
        }

        /*
        public static Rect rotateRectangle(Rect r)
        {
            var matrix = new Matrix();
            var rectWidth =  r.Width / 2f;
            var rectHeight = r.Height / 2f;
            Matrix.l
            //Changed value is the value of angle giving  from the  numericUpdownController in windows form.
            matrix.RotateAt(changedValue, new PointF(clientRectangle.Left + rectWidth, clientRectangle.Top + rectHeight));

            e.Graphics.Transform = matrix;
        }

         //bool Intersects(Point x1, Point x2, Point x3, Point[] otherQuadPoints)
             bool Intersects(VectorMove other)
         {Position.
             Vector vec = x2 - x1;
             Vector rotated = new Vector(-vec.Y, vec.X);

             bool refSide = (rotated.X * (x3.X - x1.X)
                           + rotated.Y * (x3.Y - x1.Y)) >= 0;

             foreach (Point pt in otherQuadPoints)
             {
                 bool side = (rotated.X * (pt.X - x1.X)
                            + rotated.Y * (pt.Y - x1.Y)) >= 0;
                 if (side == refSide)
                 {
                     // At least one point of the other quad is one the same side as x3. Therefor the specified edge can't be a
                     // separating axis anymore.
                     return false;
                 }
             }

             // All points of the other quad are on the other side of the edge. Therefor the edge is a separating axis and
             // the quads don't intersect.
             return true;
         }
        */

        public VectorMove Clone()
        {
            return new VectorMove(Angle, Velocity, new Rect(Position.Middle, Position.Width, Position.Height));
        }

        public void setAngleAndVelocity(float Angle, double Velocity)
        {
            this.Angle = Angle;
            this.Velocity = Velocity;
        }

        public void UpdateVelocity(double ChangeAmount, double min = 0, double max = -1)
        {
            var newvel = Velocity + ChangeAmount;
            if (newvel < min)
                Velocity = min;
            else if (newvel > max && max != -1)
                Velocity = max;
            else
                Velocity = newvel;
        }

        public static void UpdateAngle(ref float currentangle, float desiredangle, float maxChangeAmount)
        {
            if (currentangle == desiredangle)
                return;

            //if the desired angle is between the current angle and the max change, just set
            var dif = Math.Abs(currentangle - desiredangle);
            if (maxChangeAmount >= dif)
            {
                currentangle = desiredangle;
                return;
            }

            var currentangle2 = currentangle;
            if (currentangle2 > 0)
            {
                desiredangle -= currentangle;
                currentangle2 = 0;
                desiredangle = wrapAngle(desiredangle);
            }

            if ((desiredangle - currentangle2) < 180)
                currentangle += maxChangeAmount;
            else
                currentangle -= maxChangeAmount;

            currentangle = wrapAngle(currentangle);
        }

        public static void UpdateAngle(ref float Angle, bool left, float changeAmount)
        {
            float desiredangle = 0;

            if (left)
                desiredangle = wrapAngle(Angle + 90f);
            else
                desiredangle = wrapAngle(Angle - 90f);

            UpdateAngle(ref Angle, desiredangle, changeAmount);
        }

        public static float wrapAngle(float ang)
        {
            ang %= 360;
            if (ang < 0)
                ang = 360 + ang;
            return ang;
        }

        public void UpdatePosition(GameTime gt, double value = -1)
        {
            if (value == -1)
                value = Velocity;

            UpdatePosition(ref Position.Middle, Angle, value, gt);
        }

        public static void UpdatePosition(ref Vector2 val, float Angle, double value = -1, GameTime gt = null)
        {
            double p = 1;

            if (gt != null)
            {
                p = gt.ElapsedGameTime.TotalMilliseconds/100;
            }

            var ang = Angle;
            ang = (ang/180f)*3.14159f;
            var x = (float) Math.Cos(ang);
            var y = (float) Math.Sin(ang);
            val.X += x*(float) value*(float) p;
            val.Y -= y*(float) value*(float) p;
        }

        public static void ExtendPoint(ref Vector2 point, float angle, double radius)
        {
            var ang = angle;
            ang = (ang/180f)*3.14159f;
            var x = (float) Math.Cos(ang);
            var y = (float) Math.Sin(ang);
            point.X += x*(float) radius;
            point.Y -= y*(float) radius;
        }

        public float getAngleToOtherVector(Vector2 dest)
        {
            return getAngleToOtherVector(Position.Middle, dest);
        }

        public static float getAngleToOtherVector(Vector2 dest1, Vector2 dest2)
        {
            float v = (int) (Math.Atan2(dest2.Y - dest1.Y, dest2.X - dest1.X)*180f/3.14159);
            return wrapAngle(360 - v);
        }

        public static float angleInBetween(float Angle, float angle2)
        {
            var dif = Math.Abs(Angle - angle2)%360;

            if (dif > 180f)
                return 360f - dif;
            else
                return dif;
        }

        public static bool AngleIsBetween(float Angle, float angle2, float padding)
        {
            return (angleInBetween(Angle, angle2) < padding);
        }

        public static double getDistanceBetweenVectors(Vector2 one, Vector2 two)
        {
            double a = Math.Abs(one.X - two.X);
            double b = Math.Abs(one.Y - two.Y);
            var c = Math.Sqrt(a*a + b*b);
            return c;
        }
    }
}