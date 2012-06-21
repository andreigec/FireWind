using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project
{
    public class SpriteParticle : SpriteDraw
    {
        public string rawPixelData;
        public float scale = 1f;

        public void initImage(Game g)
        {
            image = new Texture2D(g.GraphicsDevice, FrameWidth, FrameHeight);

            var count = 0;
            for (var a = 0; a < FrameHeight; a++)
                for (var b = 0; b < FrameWidth; b++)
                {
                    Color[] c = null;
                    if (rawPixelData[count] == 'X')
                        c = new[] {Color.Black};
                    else
                        c = new[] {Color.Transparent};

                    var r = new Rectangle(b, a, 1, 1);
                    image.SetData(0, r, c, 0, 1);
                    count++;
                }
            rawPixelData = null;
        }
    }
}