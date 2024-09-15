using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items
{
    public class BottleOpener : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 5;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 5)
                .AddTile(TileID.Anvils)
                .AddTile(TileID.Furnaces)
                .AddCondition(Condition.NearWater)
                .Register();
        }
    }
}
