using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Tools
{
    public class HammerShapedBottleOpener : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(silver: 5, copper: 10);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true; 

            Item.axe = 30;
            Item.hammer = 100; 
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BottleOpener>())
                .AddIngredient(ItemID.ClayBlock, 5)
                .AddRecipeGroup(RecipeGroupID.IronBar, 5)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
