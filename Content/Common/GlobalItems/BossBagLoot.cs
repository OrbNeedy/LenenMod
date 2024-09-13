using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using lenen.Content.Items.Weapons;

namespace lenen.Content.Common.GlobalItems
{
    public class BossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            // Broken, doesn't do anything
            if (Main.expertMode)
            {
                if (item.type == ItemID.WallOfFleshBossBag)
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DimensionalFragment>(), 1));
                }
            }
        }
    }
}
