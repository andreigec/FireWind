using Microsoft.Xna.Framework;

namespace Project.Model
{
	public static class Collisions
	{
		//collision between sprites
		public static bool IsSpriteCollision(Map p, Vector2 testthis, Vector2 withthis, int Xradius = 30,
											 int Yradius = 10)
		{
			int x = (int)testthis.X - Xradius;
			int x1 = (int)testthis.X + Xradius;
			int y = (int)testthis.Y - Yradius;
			int y1 = (int)testthis.Y + Yradius;

			p.wrapBounds(ref x, ref y);
			p.wrapBounds(ref x1, ref y1);

			for (int a = y; a < y1; a++)
				for (int b = x; b < x1; b++)
				{
					if (b == (int)withthis.X && a == (int)withthis.Y)
						return true;
				}

			return false;
		}

		//distance check
		public static bool CheckLineCollisionRadius(Vector2 testthis, Vector2 fromthis, double radius)
		{
			double dist = VectorMove.getDistanceBetweenVectors(testthis, fromthis);
			return dist <= radius;
		}

		public static bool isSpriteCollisionBlock(Map p, Vector2 testthis, Vector2 withthis, int Xradius = 30, int Yradius = 10)
		{
			int x = (int)testthis.X - Xradius;
			int x1 = (int)testthis.X + Xradius;
			int y = (int)testthis.Y - Yradius;
			int y1 = (int)testthis.Y + Yradius;

			p.wrapBounds(ref x, ref y);
			p.wrapBounds(ref x1, ref y1);

			for (int a = y; a < y1; a++)
				for (int b = x; b < x1; b++)
				{
					if (b == (int)withthis.X && a == (int)withthis.Y)
						return true;
				}

			return false;
		}



		public static bool isBlockSpriteCollisionRadius(Map p, Vector2 testthis, Vector2 withthis, double radius)
		{
			int x = (int)testthis.X - 30;
			int x1 = (int)testthis.X + 30;
			int y = (int)testthis.Y - 10;
			int y1 = (int)testthis.Y + 10;

			if (radius <= 0)
				radius = 1;

			p.wrapBounds(ref x, ref y);
			p.wrapBounds(ref x1, ref y1);

			for (int a = 180; a > 0; a--)
			{
				var v1 = new Vector2(withthis.X, withthis.Y);
				VectorMove.UpdatePosition(ref v1, a, radius, null);
				var v2 = new Vector2(withthis.X, withthis.Y);
				VectorMove.UpdatePosition(ref v2, 360 - a, radius, null);

				if (((v1.X >= x && v1.X < x1) && (v1.Y >= y && v1.Y < y1)) ||
					((v2.X >= x && v2.X < x1) && (v2.Y >= y && v2.Y < y1)))
					return true;
			}
			return false;
		}
		public static bool isTerrainCollisionBlock(Map m, Vector2 pos, int spriteWidth)
		{
			int x = (int)pos.X - spriteWidth;
			if (x < 0) x = 0;
			int x2 = (int)pos.X + spriteWidth;
			if (x2 > m.width) x2 = m.width;

			for (int a = x; a < x2; a++)
			{
				for (int b = 0; b < m.terrain.heightmap.heights[a].Count; b++)
				{
					if (-m.terrain.heightmap.heights[a][b].Item2 < pos.Y &&
						-m.terrain.heightmap.heights[a][b].Item1 > pos.Y)
						return true;
				}
			}
			return false;
		}

		public static bool isTerrainCollisionRadius(Map m, Vector2 testthis, double radius)
		{
			//if (radius <= 0)
			//radius = 1;

			for (int a = 180; a > 0; a--)
			{
				//extend two vectors out around the circle and test impact
				var v1 = new Vector2(testthis.X, testthis.Y);
				VectorMove.UpdatePosition(ref v1, a, radius, null);
				var v2 = new Vector2(testthis.X, testthis.Y);
				VectorMove.UpdatePosition(ref v2, 360 - a, radius, null);
				var x = (int)v1.X;
				if (m.isValidMapBounds(true, x) == false)
					continue;

				for (int b = 0; b < m.terrain.heightmap.heights[x].Count; b++)
				{
					if ((-m.terrain.heightmap.heights[x][b].Item2 < v1.Y &&
						 -m.terrain.heightmap.heights[x][b].Item1 > v1.Y) ||
						(-m.terrain.heightmap.heights[x][b].Item2 < v2.Y &&
						 -m.terrain.heightmap.heights[x][b].Item1 > v2.Y))
						return true;
				}
			}
			return false;
		}


		public static bool RectangleCollision(Rectangle a, Rectangle b)
		{
			return a.Intersects(b);
		}
	}
}