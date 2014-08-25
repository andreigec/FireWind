using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ExternalUsage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Model.Instances;
using Project.Networking;
using Project.View.Client.Cameras;
using Project.XML_Load;

namespace Project.Model.mapInfo
{
    public class mapTerrain
    {
        #region terrainFeatures enum

        public enum terrainFeatures
        {
            Sea = 0,
            Mountain = 1,
            Field = 2
        }

        #endregion

        [XmlIgnore] public const int SeaLevel = 100;

        [XmlIgnore] private const int FieldLevelMin = 250;
        [XmlIgnore] public const int FieldLevelMax = 350;

        [XmlIgnore] private const int MountainLevelMin = 500;
        [XmlIgnore] public const int MountainLevelMax = 1000;
        [XmlIgnore] public List<int> changed = new List<int>();
        [XmlIgnore] public List<int> dirty = new List<int>();

        [XmlIgnore] public heightmapClass heightmap;

        [XmlIgnore] private Map parentmap;

        //set to be a copy of GameControlServer.R
        [XmlIgnore] private Random r;

        public void init(Map parent, bool createBuildings, int seed)
        {
            dirty = new List<int>();
            changed = new List<int>();

            parentmap = parent;

            r = new Random(seed);

            heightmap = new heightmapClass(parentmap.width + 1);
            var tf = new List<terrainFeatures>();
            tf.Add(terrainFeatures.Sea);
            /*
        tf.Add(terrainFeatures.Mountain);
        tf.Add(terrainFeatures.Mountain);
        tf.Add(terrainFeatures.Mountain);
        tf.Add(terrainFeatures.Mountain);
        tf.Add(terrainFeatures.Mountain);
        tf.Add(terrainFeatures.Mountain);
                                     * */
            for (int a = 0; a < 6; a++)
            {
                int type = r.Next()%3;
                tf.Add((terrainFeatures) type);
            }

            applyDAC(tf);

            if (createBuildings)
                applyBuildings();
        }

        private void applyBuildings()
        {
            List<KeyValuePair<string, building>> potBuild =
                loadXML.loadedBuildings.Where(s => s.Key.Contains(loadXML.buildingI)).ToList();
            for (int x = 0; x < parentmap.width; x++)
            {
                //get rand building
                building b = getRandBuilding(potBuild);
                float fh = b.BaseSprite.FrameHeight/2f;

                //get inital terrain height for this x
                int startheight = heightmap.heights[x][heightmap.heights[x].Count - 1].Item2;

                if (startheight > FieldLevelMin)
                {
                    //check that there is a free area for the build
                    bool success = true;
                    for (int x2 = x; x2 < b.BaseSprite.FrameWidth; x2 += 5)
                    {
                        int height = heightmap.heights[x2][heightmap.heights[x2].Count - 1].Item2;

                        if ((height + 3) < startheight || ((height - 3) > startheight))
                        {
                            success = false;
                            break;
                        }
                    }

                    if (success == false)
                        continue;
                    var vm = new VectorMove(0, 0,
                                            new Rect(x, -(startheight + fh), b.BaseSprite.FrameWidth,
                                                     b.BaseSprite.FrameHeight));
                    BuildingInstance.addBuilding(parentmap.parentGCS, b, parentmap, InstanceOwner.ReturnNoControl(), vm,
                                                 BuildingInstance.BuildingType.Civilian, SetID.CreateSetNew());
                    x += 500;
                }
                x += 100;
            }
        }

        private building getRandBuilding(List<KeyValuePair<string, building>> possible)
        {
            int rz = r.Next()%possible.Count;
            return possible[rz].Value;
        }


        public void Update(GameTime gt, UpdateModes um)
        {
            if (dirty.Count == 0)
                return;

            if (Shared.flagChecked(um, UpdateModes.UpdateTerrain) == false)
                return;

            const int heightdrop = 7;

            lock (parentmap)
            {
                for (int a = 0; a < dirty.Count; a++)
                {
                    int x = dirty[a];
                    //lower all segments by one 
                    int c = heightmap.heights[x].Count;
                    int miny = heightmap.heights[x][0].Item2;
                    for (int yseg = c - 1; yseg >= 1; yseg--)
                    {
                        if (heightmap.heights[x][yseg].Item1 > miny)
                        {
                            int dif1 = heightmap.heights[x][yseg].Item1 - heightdrop;
                            if (dif1 < miny)
                                dif1 = miny;

                            int dif2 = heightmap.heights[x][yseg].Item2 - heightdrop;
                            if (dif2 < miny)
                                dif2 = miny;

                            heightmap.heights[x][yseg] = new Tuple<int, int>(dif1, dif2);
                        }
                        else
                            mergeSegments(x);
                    }
                    if (heightmap.heights[x].Count == 1)
                    {
                        dirty.Remove(x);
                        a--;
                    }
                }
            }
        }

        private void mergeSegments(int x)
        {
            redo:
            int c = heightmap.heights[x].Count;
            if (c == 1)
                return;

            for (int a = 0; a < c - 1; a++)
            {
                int min1 = heightmap.heights[x][a].Item1;
                int max1 = heightmap.heights[x][a].Item2;

                int min2 = heightmap.heights[x][a + 1].Item1;
                int max2 = heightmap.heights[x][a + 1].Item2;

                if (max1 != min2)
                    continue;

                heightmap.heights[x].RemoveAt(a + 1);
                heightmap.heights[x][a] = new Tuple<int, int>(min1, max2);
                goto redo;
            }
        }

        public void removeTerrain(Vector2 pos, double radius)
        {
            lock (parentmap)
            {
                if (radius == 0)
                    return;

                int lastX = -1;
                for (int a = 180; a > 0; a--)
                {
                    var v1 = new Vector2(pos.X, pos.Y);
                    VectorMove.UpdatePosition(ref v1, a, radius, null);
                    var v2 = new Vector2(pos.X, pos.Y);
                    VectorMove.UpdatePosition(ref v2, 360 - a, radius, null);
                    var vx = (int) v1.X;

                    //make sure the terrain does go above 0 - below the map
                    double v1y = v1.Y;
                    if (v1y >= -20)
                        v1y = -20;

                    double v2y = v2.Y;
                    if (v2y >= -20)
                        v2y = -20;

                    removeTerrain(vx, v1y, v2y);
                    if (lastX != -1)
                    {
                        for (int b = lastX; b < vx; b++)
                        {
                            removeTerrain(b, v1y, v2y);
                        }
                    }
                    lastX = vx + 1;
                }
            }
        }

        public double getHighestTerrainPoint(double startX, double endX)
        {
            double highest = -1;
            for (var a = (int) startX; a < (int) endX; a++)
            {
                if (parentmap.isValidMapBounds(true, a, true) == false)
                    continue;
                Tuple<int, int> h = heightmap.heights[a][heightmap.heights[a].Count - 1];
                if (highest == -1 || h.Item2 > highest)
                    highest = h.Item2;
            }
            return highest;
        }


        private void removeTerrain(int x, double y1, double y2)
        {
            if (parentmap.isValidMapBounds(true, x, true) == false)
                return;
            List<Tuple<int, int>> h = heightmap.heights[x];
            for (int a = 0; a < h.Count; a++)
            {
                int h1 = h[a].Item1;
                int h2 = h[a].Item2;
                //3 cases
                //1: explosion occurs above
                if (h1 > y1 && y1 < -h2 && y2 >= -h2)
                {
                    h[a] = new Tuple<int, int>(h1, -(int) y2);
                }

                    //2:in the block
                else if (h1 > y1 && y1 >= -h2 && y2 >= -h2)
                {
                    h[a] = new Tuple<int, int>(-h1, -(int) y2);
                    h.Insert(a + 1, new Tuple<int, int>(-(int) y1, h2));
                    if (dirty.Contains(x) == false)
                    {
                        dirty.Add(x);
                    }

                    a += 2;
                }
                //TODO:3:from below
            }

            //perma changed blocks for update to new client
            if (changed.Contains(x) == false)
                changed.Add(x);

            //synch client side
            if (parentmap.SynchTerrain.Contains(x) == false)
                parentmap.SynchTerrain.Add(x);
        }

        public void drawTerrain(Camera2D cam, GameTime gameTime)
        {
            GameControlClient.ParentGame.GraphicsDevice.BlendState = BlendState.Opaque;

            Rectangle r;
            //sea
            r = new Rectangle(0, -SeaLevel, parentmap.width, SeaLevel);
            cam.spriteBatch.Draw(XNA.PixelTexture, r, Color.Blue);

            //land
            for (int a = 0; a < parentmap.width; a++)
            {
                int c = heightmap.heights[a].Count();
                for (int b = 0; b < c; b++)
                {
                    int y1 = -heightmap.heights[a][b].Item1;
                    int y2 = -heightmap.heights[a][b].Item2;

                    int diff = -(y2 - y1);

                    r = new Rectangle(a, y2, 1, diff);
                    cam.spriteBatch.Draw(XNA.PixelTexture, r, Color.Green);
                }
            }
        }

        private int getRandom(int min, int max)
        {
            if ((max - min) == 0)
                return max;

            int min2 = min < max ? min : max;
            int max2 = min > max ? min : max;
            int dif = max2 - min2;
            return min2 + r.Next()%(dif);
        }

        private int getRandom(terrainFeatures tf)
        {
            switch (tf)
            {
                case terrainFeatures.Mountain:
                    return getRandom(MountainLevelMin, MountainLevelMax);
                case terrainFeatures.Field:
                    return getRandom(FieldLevelMin, FieldLevelMax);
                case terrainFeatures.Sea:
                    return SeaLevel;
                default:
                    return 0;
            }
        }

        private void applyDAC(List<terrainFeatures> tf)
        {
            /*
            count=3
            1  2  3
            0 1/2 1

            count=4
            1  2   3  4
            0 1/3 1/3 1

            count=5
            1  2   3   4  5
            0 1/4 1/4 1/4 1
            */
            //general overview
            int countm = tf.Count - 1;
            int amount = parentmap.width/countm;

            int count = 0;
            for (int a = 0; a <= countm; a++)
            {
                heightmap.set(count, getRandom(tf[a]));
                count += amount;
            }

            count = 0;
            for (int a = 0; a <= countm; a++)
            {
                int max = count + amount;
                if (max > parentmap.width)
                    max = parentmap.width;
                applyDAC(count, max);
                count += amount;
            }
        }

        private void applyDAC(int x1, int x2)
        {
            //base case- where x1 and x2 are adjacent
            if (x2 == (x1 + 1))
            {
                x2 = x1;
                return;
            }
            //second base case - same items
            if (x1 == x2)
            {
                return;
            }
            //get the middle, set this as a random between the two ends
            int mid = (x1 + x2)/2;
            heightmap.set(mid, getRandom(heightmap.get(x1), heightmap.get(x2)));
            //divide and conquer
            applyDAC(x1, mid);
            applyDAC(mid, x2);
        }

        /*
        public void applyRandom(int x1,int x2,int min,int max)
        {
            for (int a=x1;a<x2;a++)
            {
                heightmap[a] = getRandom(min, max);
            }
        }

        public void applyRandom()
        {
            for (int a = 0; a < width; a++)
            {
                heightmap[a] = 100 + r.Next() % 100;
            }
        }

        public void applyBlockSmooth(int smoothwidth = 10)
        {
            for (int a = 0; a < width; a += smoothwidth)
            {
                if (a > width)
                    break;
                int avg = 0;
                for (int b = a; b < (a + smoothwidth) && b < width; b++)
                    avg += heightmap[b];
                avg /= smoothwidth;

                for (int b = a; b < (a + smoothwidth) && b < width; b++)
                    heightmap[b] = avg;
            }
        }
         * */
    }
}