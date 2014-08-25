using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.View.Client
{
    public class FPS
    {
        private readonly List<int> fpsDTL = new List<int>();
        private readonly int maxsamples;
        private DateTime fpsDT;

        public FPS(int samples = 10)
        {
            maxsamples = samples;
        }

        public int getFPS()
        {
            if (fpsDTL.Count <= Math.Ceiling(maxsamples*.1f))
                return 0;

            //avg
            float avgtot = fpsDTL.Sum();
            avgtot /= fpsDTL.Count;
            return (int) avgtot;
        }

        public void UpdateFPS()
        {
            if (fpsDTL.Count >= maxsamples)
                fpsDTL.RemoveAt(0);

            DateTime now = DateTime.Now;

            TimeSpan ts = now - fpsDT;
            fpsDT = now;

            var ms = (int) ts.TotalMilliseconds;

            if (ms > 0)
            {
                ms = 1000/ms;
                fpsDTL.Add(ms);
            }
        }
    }
}