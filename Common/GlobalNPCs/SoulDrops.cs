using lenen.Common.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.GlobalNPCs
{
    public class SoulDrops : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            int playerIndex = npc.lastInteraction;
            if (playerIndex == 255) return;
            SoulAbsorptionPlayer player = Main.player[playerIndex].GetModPlayer<SoulAbsorptionPlayer>();

            int souls = 1;
            souls += (npc.life * npc.rarity) / 1000;

            if (npc.boss) souls += Main.rand.Next(20, 41);

            if (NPC.downedPlantBoss) souls = (int)(souls * Main.rand.NextFloat(1f, 2f));

            player.AddSouls(souls);
        }
    }
}
