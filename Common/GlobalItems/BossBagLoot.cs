using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using lenen.Content.Items.Weapons;
using lenen.Content.Items.Accessories;

namespace lenen.Common.GlobalItems
{
    public class BossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                case ItemID.WallOfFleshBossBag:
                    itemLoot.Add(ItemDropRule.ByCondition(new DropInExpert(),
                        ModContent.ItemType<DimensionalFragment>(), 6));
                    break;
                case ItemID.SkeletronBossBag:
                    itemLoot.Add(ItemDropRule.ByCondition(new DropInExpert(),
                        ModContent.ItemType<AssassinKnife>(), 3));
                    break;
                case ItemID.MoonLordBossBag:
                    itemLoot.Add(ItemDropRule.ByCondition(new DropInExpert(),
                        ModContent.ItemType<AntiGravityCape>(), 4));
                    break;
            }
        }
    }
}