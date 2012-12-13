using System;
using System.Collections.Generic;

namespace Project.Model
{
    public class heightmapClass
    {
        //private int width = 0;
        //starts,ends

        public List<Tuple<int, int>>[] heights;

        public heightmapClass(int width)
        {
            heights = new List<Tuple<int, int>>[width];
            for (var a = 0; a < width; a++)
                heights[a] = new List<Tuple<int, int>>();
            //this.width = width;
        }

        public String Serialise()
        {
            var r = "";
            foreach (var k in heights)
            {
                if (k == null)
                    continue;

                foreach (var v in k)
                {
                    r += v.Item1 + "," + v.Item2;
                    r += "\t";
                }
                r += "\n";
            }
            return r;
        }

        public static heightmapClass Deserialise(String r)
        {
            //split into columns
            var col = r.Split('\n');
            var w = col.Length - 1;

            var v = new List<Tuple<int, int>>[w];

            var count = 0;
            foreach (var c in col)
            {
                if (string.IsNullOrEmpty(c))
                    continue;

                v[count] = new List<Tuple<int, int>>();
                //split into tuples
                var tup = c.Split('\t');

                foreach (var t in tup)
                {
                    if (string.IsNullOrEmpty(t))
                        continue;


                    var i = t.Split(',');
                    var i1 = int.Parse(i[0]);
                    var i2 = int.Parse(i[1]);
                    v[count].Add(new Tuple<int, int>(i1, i2));
                }
                //split into item 1/2
                count++;
            }

            var ret = new heightmapClass(w);
            ret.heights = v;
            return ret;
        }

        public void set(int index, int value)
        {
            if (heights[index].Count == 0)
                heights[index].Add(new Tuple<int, int>(0, value));
            else
                heights[index][heights[index].Count - 1] = new Tuple<int, int>(0, value);
        }

        public int get(int index)
        {
            if (heights[index].Count == 0)
                return 1;
            return heights[index][heights[index].Count - 1].Item2;
        }
    }
}