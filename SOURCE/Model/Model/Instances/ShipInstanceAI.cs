﻿using Microsoft.Xna.Framework;

namespace Project
{
    public partial class ShipInstance
    {
        public void updateAI(GameTime gt)
        {
            if (parentArea is Map)
                updateAIMap(gt);
        }
    }
}