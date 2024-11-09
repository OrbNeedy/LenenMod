using lenen.Common.Players;
using lenen.Content.Buffs;
using lenen.Content.Items;
using lenen.Content.NPCs.Fairy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.GlobalNPCs
{
    public class SoulDrops : GlobalNPC
    {
        public float harujionPotency = 0f;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            harujionPotency = 0f;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (harujionPotency > 0f)
            {
                npc.lifeRegen -= (int)(60*harujionPotency);
            }
        }

        public override void AI(NPC npc)
        {
            if (harujionPotency > 0) npc.AddBuff(ModContent.BuffType<HarujionAbsorb>(), 2);
            base.AI(npc);
        }

        public override void OnKill(NPC npc)
        {
            if (npc.SpawnedFromStatue) return;
            if (npc.lifeMax <= 20) return;
            int souls = 1;
            int volume = 1;
            souls *= (1 + npc.rarity);

            volume += npc.life / 900;

            souls += npc.defense / 20;

            if (Main.hardMode) volume *= 2;

            if (npc.boss) souls += Main.rand.Next(5, 16);

            if (NPC.downedPlantBoss) souls += (int)(2 * Main.rand.NextFloat(1f, 2f));

            if (npc.type == ModContent.NPCType<SmallFairy>())
            {
                souls += Main.rand.Next(5);
                volume += Main.rand.Next(1, 3);
            }

            Vector2 position = npc.position;
            for (int i = 0; i < souls; i++)
            {
                Item.NewItem(npc.GetSource_DropAsItem(), new Rectangle((int)position.X, (int)position.Y,
                npc.width, npc.height), ModContent.ItemType<SoulItem>(), Main.rand.Next(1, 1 + volume));
            }
            /*int playerIndex = npc.lastInteraction;
            if (playerIndex != 255)
            {
                SoulAbsorptionPlayer player = Main.player[playerIndex].GetModPlayer<SoulAbsorptionPlayer>();
                player.AddSouls(souls);
            }*/
        }
    }
}
