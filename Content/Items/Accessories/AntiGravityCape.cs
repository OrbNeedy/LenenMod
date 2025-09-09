using lenen.Common.Players;
using lenen.Common.Players.Barriers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class AntiGravityCape : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 46;
            Item.accessory = true;

            Item.rare = ItemRarityID.Purple;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

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

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not ClauseCape;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.GravityBarrier].Active = true;
        }
    }
}
