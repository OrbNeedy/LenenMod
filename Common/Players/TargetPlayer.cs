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
        private int range = 1000;

        public override void PostUpdate()
        {
            // Do not run this in the server
            // The server has no say in what will the projectiles target
            if (Main.dedServ) return;
            //Main.NewText("Range: " + range);
            // If there is no target, or every few seconds, search a new one
            if (target == null || Player.GetModPlayer<OptionsManagingPlayer>().UpdateCount % 300 == 0)
            {
                Dictionary<NPC, float> targetWeights = new Dictionary<NPC, float>();
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    // Only search an NPC if it's at a certain distance from the mouse
                    if (npc.CanBeChasedBy() && npc.Center.Distance(Main.MouseWorld) <= range)
                    {
                        // Weight the npc's state to get a value from 0 to 1 and add them to a dictionary
                        float finalWeight = (WeighDistance(npc, range) * 0.7f) + (WeighLife(npc, range) * 0.1f) +
                            (WeighPlayerDistance(npc, range, Player) * 0.2f);
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
                if (!target.CanBeChasedBy() || target.life <= 0 || target.Center.Distance(Player.Center) > range*2)
                {
                    target = null;
                }
            }
        }

        public static float WeighDistance(NPC target, float range)
        {
            float weight = 0f;
            float distance = Vector2.Distance(target.Center, Main.MouseWorld);
            weight = (range - distance) / range;
            return weight;
        }

        public static float WeighPlayerDistance(NPC target, float range, Player player)
        {
            float weight = 0f;
            float distance = Vector2.Distance(target.Center, player.Center);
            weight = (range - distance) / range;
            if (weight < 0)
            {
                weight = 0;
            }
            return weight;
        }

        public static float WeighLife(NPC target, float range)
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

        public static int CalculateWeight(float range, Player player)
        {
            int returnValue = -1;

            Dictionary<NPC, float> targetWeights = new Dictionary<NPC, float>();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                // Only search an NPC if it's at a certain distance from the mouse
                if (npc.CanBeChasedBy() && npc.Center.Distance(Main.MouseWorld) <= range)
                {
                    // Weight the npc's state to get a value from 0 to 1 and add them to a dictionary
                    float finalWeight = (WeighDistance(npc, range) * 0.7f) + (WeighLife(npc, range) * 0.1f) +
                        (WeighPlayerDistance(npc, range, player) * 0.2f);
                    targetWeights.Add(npc, finalWeight);
                }
            }
            if (targetWeights.Count > 0)
            {
                returnValue = targetWeights.MaxBy(entry => entry.Value).Key.whoAmI;
            }

            return returnValue;
        }
    }
}
