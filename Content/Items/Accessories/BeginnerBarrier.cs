using lenen.Common.Players;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using lenen.Common.Players.Barriers;

namespace lenen.Content.Items.Accessories
{
    public class BeginnerBarrier : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.accessory = true;

            Item.rare = ItemRarityID.Blue;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                Barrier barrier = Main.LocalPlayer.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.Beginner];
                tooltips.Insert(index - 1, new TooltipLine(Mod, "BarrierDescriptor",
                    Language.GetTextValue("Mods.lenen.BarrierStats", barrier.MaxLife, barrier.MaxCooldown / 60,
                    barrier.MaxRecovery / 60, barrier.MaxFullRecovery / 60)));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Barrier barrier = player.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.Beginner];
            barrier.Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.BlackInk, 4)
                .AddIngredient(ItemID.Obsidian, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
