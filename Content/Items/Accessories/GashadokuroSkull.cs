using lenen.Common;
using lenen.Common.Players.Barriers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class GashadokuroSkull : ModItem
    {
        private Barrier barrier = BarrierLookups.BarrierDictionary[BarrierLookups.Barriers.SkullBarrier];

        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
        }

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHatHair[equipSlotHead] = true;
            ArmorIDs.Face.Sets.OverrideHelmet[Item.faceSlot] = true;
            ArmorIDs.Face.Sets.PreventHairDraw[Item.faceSlot] = true;
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

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            barrier.MaxLife, barrier.MaxCooldown/60, barrier.MaxRecovery/60);

        public override void OnCreated(ItemCreationContext context)
        {
            base.OnCreated(context);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        public override void UpdateEquip(Player player)
        {
            barrier.State = 1;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ModContent.ItemType<RevivedGashadokuroSkull>())
            {
                return false;
            }
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
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
