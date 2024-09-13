using lenen.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Common.GlobalNPCs
{
    public class ItemDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Broken, always happens
            if (!Main.expertMode)
            {
                if (npc.type == NPCID.WallofFlesh)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DimensionalFragment>(), 6));
                }
            }
        }
    }
}
