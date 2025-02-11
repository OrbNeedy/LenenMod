using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class SpiritFlamesPlayer : ModPlayer
    {
        public bool spiritFires = false;
        public int flamesCooldown = 0;
        public int singleQueueRequests = 0;

        int queuedCooldown = -1;

        public override void ResetEffects()
        {
            spiritFires = false;
        }

        public override void PreUpdate()
        {
            if (flamesCooldown > 0)
            {
                flamesCooldown--;
            }
        }

        public override void PostUpdate()
        {
            if (queuedCooldown != -1)
            {
                flamesCooldown = queuedCooldown;
                queuedCooldown = -1;
                singleQueueRequests = 0;
            }
        }

        public void QueueCooldown(int time)
        {
            if (time > queuedCooldown)
            {
                queuedCooldown = time;
            }
            singleQueueRequests++;
        }
    }
}
