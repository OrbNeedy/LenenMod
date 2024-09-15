using lenen.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.GlobalNPCs
{
    public class ItemDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DropInNormal(), ModContent.ItemType<DimensionalFragment>(), 6));
            }
        }
    }
}
