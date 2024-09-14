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
            if (item.type == ItemID.WallOfFleshBossBag)
            {
                itemLoot.Add(ItemDropRule.ByCondition(new DropInExpert(), ModContent.ItemType<DimensionalFragment>(), 6));
            }
        }
    }

    public class DropInExpert : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => Main.expertMode;
        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription()
        {
            return "";
        }
    }
}