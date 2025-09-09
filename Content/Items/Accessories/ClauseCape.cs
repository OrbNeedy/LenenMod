using lenen.Common.Players;
using lenen.Common.Players.Barriers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class ClauseCape : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 48;
            Item.accessory = true;

            Item.rare = ItemRarityID.Purple;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not AntiGravityCape;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                Barrier barrier = Main.LocalPlayer.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.GravityBarrier];
                tooltips.Insert(index - 1, new TooltipLine(Mod, "BarrierDescriptor",
                    Language.GetTextValue("Mods.lenen.BarrierStats", barrier.MaxLife, barrier.MaxCooldown / 60,
                    barrier.MaxRecovery / 60, barrier.MaxFullRecovery / 60)));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Barrier barrier = player.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.GravityBarrier];
            barrier.Active = true;
            barrier.Variation = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AntiGravityCape>(1)
                .AddIngredient(ItemID.GingerbreadCookie, 1)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
