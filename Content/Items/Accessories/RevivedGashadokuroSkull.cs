using lenen.Common.Players;
using lenen.Common;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using lenen.Common.Players.Barriers;
using Terraria.Audio;

namespace lenen.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class RevivedGashadokuroSkull : ModItem
    {
        public int soulsCollected = 0;
        public int maxSouls = 250;
        private int soulConsumptionTimer = 0;
        private int transformedTimer = 0;
        private int maxTransformedTime = 3600;
        private bool revivedMode = false;
        private string soulsDescription;

        private int multiplicativeDamageIncrease = 20;
        private int baseDamageIncrease = 30;
        private int defensePenetrationIncrease = 50;
        private int damageReductionDecrease = 25;
        private int defenseDecrease = 25;

        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this, "RevivedGashadokuroSkull");
        }

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, "RevivedGashadokuroSkull", EquipType.Head);
            ArmorIDs.Head.Sets.DrawHatHair[equipSlotHead] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 16;
            Item.accessory = true;

            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 10;
            Item.hasVanityEffects = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxTransformedTime / 60, 
            multiplicativeDamageIncrease / 100f, baseDamageIncrease, defensePenetrationIncrease, damageReductionDecrease / 100f, 
            defenseDecrease);

        public override void OnCreated(ItemCreationContext context)
        {
            base.OnCreated(context);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Souls"] = soulsCollected;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Souls"))
            {
                soulsCollected = tag.GetInt("Souls");
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        public override void UpdateEquip(Player player)
        {
            Barrier barrier = player.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.SkullBarrier2];
            barrier.Active = true;
            barrier.Variation = 0;

            SoulAbsorptionPlayer soulSource = player.GetModPlayer<SoulAbsorptionPlayer>();

            if (revivedMode)
            {
                barrier.Variation = 1;
                player.GetDamage(DamageClass.Generic) *= 1 + (multiplicativeDamageIncrease / 100f);
                player.GetDamage(DamageClass.Generic).Base += baseDamageIncrease;
                player.GetArmorPenetration(DamageClass.Generic) += defensePenetrationIncrease;
                player.endurance -= damageReductionDecrease / 100f;
                player.statDefense -= defenseDecrease;
                soulSource.revivedState = true;
                if (transformedTimer <= 0)
                {
                    revivedMode = false;
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/rei_drain_00"), player.Center);
                }
                transformedTimer--;
            } else
            {
                //Main.NewText(soulSource.activateRevival);
                if (soulSource.activateRevival)
                {
                    if (soulsCollected >= maxSouls)
                    {
                        SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/charge_up"), player.Center);
                        revivedMode = true;
                        transformedTimer = maxTransformedTime;
                        soulsCollected = 0;
                    }
                }
                ConsumeSoul(soulSource);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            soulsDescription = Language.GetTextValue("Mods.lenen.SoulDescriptions.Reserve",
                soulsCollected); 

            int index = tooltips.FindIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                tooltips.Insert(index - 1, new TooltipLine(Mod, "SoulDescriptor", soulsDescription));
            }
            
            index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                Barrier barrier = Main.LocalPlayer.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.SkullBarrier2];
                tooltips.Insert(index - 1, new TooltipLine(Mod, "BarrierDescriptor",
                    Language.GetTextValue("Mods.lenen.BarrierStats", barrier.MaxLife, barrier.MaxCooldown / 60,
                    barrier.MaxRecovery / 60, barrier.MaxFullRecovery / 60)));
            }
            base.ModifyTooltips(tooltips);
        }

        public void ConsumeSoul(SoulAbsorptionPlayer soulSource)
        {
            if (soulSource.soulsCollected > 0 && soulsCollected < maxSouls)
            {
                if (soulConsumptionTimer <= 0)
                {
                    soulSource.soulsCollected--;
                    soulsCollected++;
                    soulConsumptionTimer = 20;
                } else
                {
                    soulConsumptionTimer--;
                }
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ModContent.ItemType<GashadokuroSkull>())
            {
                return false;
            }
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void AddRecipes()
        {
            Recipe.OnCraftCallback callback = new(TakeAwaySouls);
            
            CreateRecipe()
                .AddIngredient<GashadokuroSkull>()
                .AddTile(TileID.DemonAltar)
                .AddCondition(SoulsCondition.HasEnoughSouls)
                .AddCondition(Condition.DownedPlantera)
                .AddOnCraftCallback(callback)
                .Register();
        }

        private void TakeAwaySouls(Recipe recipe, Item item, List<Item> list, Item item2)
        {
            Main.LocalPlayer.GetModPlayer<SoulAbsorptionPlayer>().soulsCollected -= 1500;
        }
    }
}
