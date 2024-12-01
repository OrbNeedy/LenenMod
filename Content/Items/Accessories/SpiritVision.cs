using lenen.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class SpiritVision : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 30;
            Item.accessory = true;

            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(BuffID.Hunter, 2);
            if (!hideVisual)
            {
                player.GetModPlayer<SoulAbsorptionPlayer>().seeSpirits = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Glass, 5)
                .AddRecipeGroup("IronBar", 10)
                .AddIngredient(ItemID.Wire, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
