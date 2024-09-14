using lenen.Content.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class DesperateBarrier : ModItem
    {
        private int defense = 15;
        private float dmgReduction = 0.1f;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.accessory = true;

            Item.rare = ItemRarityID.Red;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(defense, dmgReduction*100);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SpellCardManagement management = player.GetModPlayer<SpellCardManagement>();
            player.statDefense += defense;
            if (player.statLife <= (player.statLifeMax2 / 2))
            {
                management.desperateBomb = true;
                player.endurance += dmgReduction;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.Ectoplasm, 10)
                .AddIngredient(ItemID.FragmentVortex, 5)
                .AddIngredient(ItemID.PaladinsShield, 1)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
