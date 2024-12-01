using lenen.Common;
using lenen.Common.Players.Barriers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class AntiGravityCape : ModItem
    {
        private Barrier barrier = BarrierLookups.BarrierDictionary[BarrierLookups.Barriers.GravityBarrier];
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 46;
            Item.accessory = true;

            Item.rare = ItemRarityID.Purple;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            barrier.MaxLife / 60, barrier.MaxCooldown / 60, barrier.MaxRecovery / 60);

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not ClauseCape;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            barrier.State = 1;
        }
    }
}
