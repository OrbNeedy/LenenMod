using lenen.Common.Players;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class MarksmanshipMedal : ModItem
    {
        float rangedDamage = 8;
        float critDamage = 100f;
        float critChance = 20f;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.accessory = true;

            Item.rare = ItemRarityID.Yellow;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(rangedDamage, critChance, critDamage);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += rangedDamage / 100f;
            player.GetCritChance(DamageClass.Ranged) += critChance;
            CriticalDamagePlayer critPlayer = player.GetModPlayer<CriticalDamagePlayer>();
            critPlayer.additionalCritDamage += critDamage / 100f;
            critPlayer.explosionUpgrade = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.type == ModContent.ItemType<BullseyeMedal>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("GoldBar", 20)
                .AddIngredient<BullseyeMedal>()
                .AddIngredient(ItemID.RifleScope)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
