using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class TargetPlayer : ModPlayer
    {
        public NPC target = null;

        public override void PostUpdate()
        {
            // Do not run this in the server
            // The server has no say in what will the projectiles target
            if (Main.dedServ) return;
            // If there is no target, or every few seconds, search a new one
            if (target == null || Player.GetModPlayer<OptionsDrawingPlayer>().UpdateCount%300 == 0)
            {
                Dictionary<NPC, float> targetWeights = new Dictionary<NPC, float>();
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    // Only search an NPC if it's at a certain distance from the mouse
                    if (npc.CanBeChasedBy() && npc.Center.DistanceSQ(Main.MouseWorld) <= 80000)
                    {
                        // Weight the npc's state to get a value from 0 to 1 and add them to a dictionary
                        float finalWeight = (WeighDistance(npc) * 0.5f) + (WeighLife(npc) * 0.3f) +
                            (WeighPlayerDistance(npc) * 0.2f);
                        targetWeights.Add(npc, finalWeight);
                    }
                }
                if (targetWeights.Count > 0)
                {
                    target = targetWeights.MaxBy(entry => entry.Value).Key;
                }
            }
            else
            {
                // If there is a target, make sure it is a valid target
                if (!target.CanBeChasedBy() || target.life <= 0 || target.Center.DistanceSQ(Player.Center) > 1500000)
                {
                    target = null;
                }
            }
        }

        public float WeighDistance(NPC target)
        {
            float weight = 0f;
            float distance = Vector2.DistanceSquared(target.Center, Main.MouseWorld);
            weight = (80000f - distance) / 80000f;
            return weight;
        }

        public float WeighPlayerDistance(NPC target)
        {
            float weight = 0f;
            float distance = Vector2.DistanceSquared(target.Center, Player.Center);
            weight = (16000f - distance) / 16000f;
            if (weight < 0)
            {
                weight = 0;
            }
            return weight;
        }

        public float WeighLife(NPC target)
        {
            float weight = (target.lifeMax - target.life) / target.lifeMax;
            if (target.boss)
            {
                weight += 0.3f;
                if (weight > 1)
                {
                    weight = 1;
                }
            }
            return weight;
        }
    }
}
