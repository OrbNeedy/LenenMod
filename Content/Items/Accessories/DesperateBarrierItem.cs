using lenen.Common.Players;
using lenen.Common.Players.Barriers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class DesperateBarrierItem : ModItem
    {
        private float dmgReduction = 0.1f;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.accessory = true;

            Item.rare = ItemRarityID.Red;
            Item.defense = 10;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(dmgReduction * 100);

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                Barrier barrier = Main.LocalPlayer.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.DesperateBarrier];
                tooltips.Insert(index - 1, new TooltipLine(Mod, "BarrierDescriptor",
                    Language.GetTextValue("Mods.lenen.BarrierStats", barrier.MaxLife, barrier.MaxCooldown / 60,
                    barrier.MaxRecovery / 60, barrier.MaxFullRecovery / 60)));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Barrier barrier = player.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.DesperateBarrier];
            barrier.Active = true;
            if (player.statLife <= player.statLifeMax2 / 2)
            {
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
