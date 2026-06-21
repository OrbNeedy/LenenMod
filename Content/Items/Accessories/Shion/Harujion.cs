using lenen.Common.Players;
using lenen.Common.Players.Barriers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories.Shion
{
    public class Harujion : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 46;
            Item.accessory = true;

            Item.rare = ItemRarityID.Purple;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<DeathManagingPlayer>().harujionRevival = true;
            base.UpdateAccessory(player, hideVisual);
        }
    }
}
