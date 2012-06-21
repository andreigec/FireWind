
using ExternalUsage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Model.mapInfo;
using XNA_Winforms_Wrapper;

namespace Project.Model
{
    public static class texturing
    {
        public static Texture2D landTexture;

        public static void generateLandGradient(Game parentgame, int height)
        {
            var startColor = Color.White;
            var endColor = Color.White;
            // generate empty texture of the correct size
            landTexture = new Texture2D(parentgame.GraphicsDevice, 1, height, false, SurfaceFormat.Color);

            // the array stores the 2d pixels in a long 1d array
            var linearArray = new Color[height];

            // iterates through the array
            for (float j = 0; j < height; j++)
            {
                if (j < mapTerrain.SeaLevel)
                {
                    startColor = Color.Green;
                    endColor = Color.Black;
                }
                /*
					else if (j<mapTerrain.FieldLevelMax)
					{
						startColor = Color.Green;
						endColor = Color.LightGreen;
					}
					else
					{
						startColor = Color.LightGreen;
						endColor = Color.White;
					}
					*/
                // creating 'vector colours' to make the maths easier
                var startV = new Vector3(startColor.R, startColor.G, startColor.B);
                var endV = new Vector3(endColor.R, endColor.G, endColor.B);
                var diffV = endV - startV; // delta colour

                // calculates the new colour vector and converts to an actual colour
                var newV = (diffV*(j/1000)) + startV;
                var newColor = new Color((byte) newV.X, (byte) newV.Y, (byte) newV.Z, 255);
                linearArray[(int) j] = newColor;
            }

            landTexture.SetData(linearArray);
        }
    }
}