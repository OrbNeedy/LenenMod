using lenen.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items.Debug
{
    public class CheatingEye : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<BuffPlayer>().Cheating = true;
        }
    }
}
