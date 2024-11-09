using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class TrackingPlayer : ModPlayer
    {
        public bool trackingHarujion = false;

        public override void ResetEffects()
        {
            trackingHarujion = false;
        }
    }
}
