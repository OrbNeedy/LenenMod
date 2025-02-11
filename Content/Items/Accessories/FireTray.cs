using lenen.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class FireTray : ModItem
    {
        float addedDamage = 10f;

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.accessory = true;

            Item.rare = ItemRarityID.Purple;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(addedDamage);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += addedDamage/100f;
            player.GetModPlayer<SpiritFlamesPlayer>().spiritFires = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("GoldBar", 20)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.DemonAltar)
                .AddCondition(Condition.DownedCultist)
                .Register();

            CreateRecipe()
                .AddRecipeGroup("GoldBar", 20)
                .AddIngredient(ItemID.DemonTorch)
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(Condition.DownedCultist)
                .Register();

            CreateRecipe()
                .AddRecipeGroup("GoldBar", 10)
                .AddIngredient(ItemID.LivingDemonFireBlock)
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(Condition.DownedCultist)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.WaterCandle)
                .AddCondition(Condition.DownedCultist)
                .AddCondition(Condition.InGraveyard)
                .Register();
        }
    }
}
