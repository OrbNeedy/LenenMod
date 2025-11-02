using lenen.Common.Players;
using lenen.Common.Players.Barriers;
using SteelSeries.GameSense.DeviceZone;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class GashadokuroSkull : ModItem
    {
        int headSlot = -1;
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this, "GashadokuroSkull");
        }

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, "GashadokuroSkull", EquipType.Head);
            headSlot = equipSlotHead;
            ArmorIDs.Head.Sets.DrawHatHair[equipSlotHead] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;

            Item.rare = ItemRarityID.LightRed;
            Item.defense = 6;
            Item.hasVanityEffects = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                Barrier barrier = Main.LocalPlayer.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.SkullBarrier];
                tooltips.Insert(index - 1, new TooltipLine(Mod, "BarrierDescriptor",
                    Language.GetTextValue("Mods.lenen.BarrierStats", barrier.MaxLife, barrier.MaxCooldown / 60,
                    barrier.MaxRecovery / 60, barrier.MaxFullRecovery / 60)));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void UpdateEquip(Player player)
        {
            Barrier barrier = player.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.SkullBarrier];
            barrier.Active = true;
        }

        public override void UpdateVisibleAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual && player.head == -1)
            {
                player.head = headSlot;
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ModContent.ItemType<RevivedGashadokuroSkull>())
            {
                return false;
            }
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 20)
                .AddTile(TileID.DemonAltar)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Skull, 1)
                .AddIngredient(ItemID.Bone, 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
