using lenen.Common.Players;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories.Sanra
{
    public class MarksmanshipMedal : ModItem
    {
        float critDamage = 100f;
        float critChance = 20f;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.accessory = true;

            Item.rare = ItemRarityID.Yellow;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critChance, critDamage);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float extraDamage = player.GetCritChance(DamageClass.Ranged);
            player.GetCritChance(DamageClass.Ranged) += critChance;
            player.GetDamage(DamageClass.Ranged) += (extraDamage + critChance) / 200f;

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
