using lenen.Common.Players;
using lenen.Content.Items.Vanity.Tsurubami;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Vanity.Tsubakura
{
    [AutoloadEquip(EquipType.Head)]
    class EyelessTsubaHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Item.type] = ModContent.ItemType<TsubaHat>();
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.vanity = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<SenriPlayer>().canDropSenri = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 15)
                .AddIngredient(ItemID.BlackDye)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
