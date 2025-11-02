using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using lenen.Common.Players;

namespace lenen.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Head)]
    class SenriPriestHeadpiece : ModItem
    {
        float genericPercent = 6f;
        public static OfudaModes[] _availableModes = new[] { 
                OfudaModes.Default, OfudaModes.KingSlime, OfudaModes.EyeOfCthulhu, 
                OfudaModes.EaterOfWorlds, OfudaModes.BrainOfCthulhu, OfudaModes.QueenBee, 
                OfudaModes.Skeletron, OfudaModes.Deerclops, OfudaModes.WallOfFlesh, 
                OfudaModes.QueenSlime, OfudaModes.Retinazer, OfudaModes.Spazmatism, OfudaModes.Twins, 
                OfudaModes.TheDestroyer, OfudaModes.SkeletronPrime, OfudaModes.Plantera, 
                OfudaModes.Golem, OfudaModes.DukeFishron, OfudaModes.EmpressOfLight, 
                OfudaModes.LunaticCultist, OfudaModes.LunarEvent, OfudaModes.MoonlordsCore
            };
        private string copyDescription;

        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            // if (Main.netMode == NetmodeID.Server) return;*/

            //Item.headSlot = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this, "SenriHead");
        }

        public override void SetStaticDefaults()
        {
            /*if (Main.netMode == NetmodeID.Server) return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, "SenriHead", EquipType.Head);
            // Item.headSlot = equipSlotHead;*/
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Master;
            Item.hasVanityEffects = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(genericPercent);

        public override void OnCreated(ItemCreationContext context)
        {
            base.OnCreated(context);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<SenriPlayer>().senriActive = true;

            player.GetDamage(DamageClass.Generic) += genericPercent / 100f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string currentMode = Main.LocalPlayer.GetModPlayer<SenriPlayer>().currentMode.ToString();
            copyDescription = Language.GetTextValue($"Mods.lenen.SenriCopy.{currentMode}.Description");
            string name = Language.GetTextValue($"Mods.lenen.SenriCopy.{currentMode}.OfudaName");

            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "StateDescriptor0", name));
                tooltips.Insert(index + 2, new TooltipLine(Mod, "StateDescriptor1", copyDescription));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void UpdateVisibleAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual && player.head == -1)
            {
                player.head = Item.headSlot;
            }
        }
    }
}
