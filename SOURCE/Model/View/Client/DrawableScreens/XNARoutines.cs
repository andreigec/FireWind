using Microsoft.Xna.Framework;
using Project.Model;
using Project.View.Client.Cameras;

namespace Project.View.Client.DrawableScreens
{
    public static class XNARoutines
    {
        public static void DrawImage(Camera2D cam, SpriteDraw sb, Vector2 middle, int size, Color tint)
        {
            var r = new Rectangle((int) middle.X, (int) middle.Y, size, size);

            cam.spriteBatch.Draw(sb.image, r, tint);
        }

        public static void DrawImage(Camera2D cam, SpriteBase sb, SpriteAnimation sa, Vector2 pos, Color c, float angle)
        {
            int cy = sa.startImageCount/sb.columnCount;
            int cx = sa.startImageCount%sb.columnCount;

            float r = angle/180f;
            r = r*3.14159f;

            var middle = new Vector2(((float) sb.FrameWidth/2), ((float) sb.FrameHeight/2));
            cam.spriteBatch.Draw(sb.image,
                                 pos,
                                 new Rectangle(cx*sb.FrameWidth,
                                               cy*sb.FrameHeight,
                                               sb.FrameWidth, sb.FrameHeight), c
                                 , -r, middle, 1f, 0, 0);
        }
    }
}