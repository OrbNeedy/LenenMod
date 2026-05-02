using lenen.Common.Players;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class BullseyeMedal : ModItem
    {
        float critDamage = 50f;
        float critChance = 10f;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.accessory = true;

            Item.rare = ItemRarityID.Orange;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critChance, critDamage);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Ranged) += critChance;
            CriticalDamagePlayer critPlayer = player.GetModPlayer<CriticalDamagePlayer>();
            critPlayer.additionalCritDamage += critDamage / 100f;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.type == ModContent.ItemType<MarksmanshipMedal>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("SilverBar", 10)
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddIngredient(ItemID.Silk, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
