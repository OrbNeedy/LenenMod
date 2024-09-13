using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using lenen.Content.NPCs;

namespace lenen.Content.Items
{
    public class UnstableRock : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 12;
            Item.value = 0;
            Item.rare = ItemRarityID.LightPurple;

            ItemID.Sets.ShimmerTransformToItem[Item.type] = ModContent.ItemType<DimensionalFracture>();
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 20)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
