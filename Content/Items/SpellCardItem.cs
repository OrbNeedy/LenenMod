using lenen.Common.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items
{
    // After decompiling Gensokyo mod, I realized this could be good for my own mod as well 
    // An abstract class that makes it simpler to create weapons with Spell Cards
    public abstract class SpellCardItem : ModItem
    {
        // Abstract is not loaded 
        public const string CooldownKey = "[SCCooldown]";
        public const string ManaCooldownKey = "[SCCooldownMana]";

        protected abstract SpellCard SpellCardID { get; }
        protected abstract int SpellCardCooldown { get; }
        protected abstract int DesperateCooldown { get; }
        protected abstract int ManaUse { get; }
        protected abstract int DesperateManaUse { get; }

        protected virtual bool CanUseSpellCard(Player player)
        {
            SpellCardManagement scManager = player.GetModPlayer<SpellCardManagement>();
            bool manaCheck = true;

            if (scManager.desperateBomb)
            {
                manaCheck = player.CheckMana(DesperateManaUse, false, true);
            } else
            {
                manaCheck = player.CheckMana(ManaUse, false, true);
            }

            return scManager.spellCardTimer <= 0 && manaCheck;
        }

        protected virtual void SpellCard(Player player, bool desperate) { }

        protected void SetCooldown(Player player)
        {
            SpellCardManagement scManager = player.GetModPlayer<SpellCardManagement>();

            scManager.lastDesperate = scManager.desperateBomb;
            scManager.lastSpellCard = SpellCardID;
            if (scManager.desperateBomb)
            {
                scManager.spellCardTimer = DesperateCooldown;
                if (DesperateManaUse > 0)
                {
                    player.CheckMana(DesperateManaUse, true, true);
                    player.manaRegenDelay = player.manaRegenCount;
                }
            } else
            {
                scManager.spellCardTimer = SpellCardCooldown;
                if (ManaUse > 0)
                {
                    player.CheckMana(ManaUse, true, true);
                    player.manaRegenDelay = player.manaRegenCount;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool desperate = Main.LocalPlayer.GetModPlayer<SpellCardManagement>().desperateBomb;
            // Find the first index that contains the desired text key
            int index = tooltips.FindIndex((x) => x.Text.Contains(CooldownKey) && x.Mod == "Terraria");
            if (index != -1)
            {
                float finalCooldown = desperate ? DesperateCooldown : SpellCardCooldown;
                finalCooldown = float.Round(finalCooldown / 60f, 1);

                string newDescription = Language.GetTextValue(
                    "Mods.lenen.SpellCardDescription", finalCooldown);
                tooltips[index].Text = tooltips[index].Text.Replace(CooldownKey, newDescription);
            }

            index = tooltips.FindIndex((x) => x.Text.Contains(ManaCooldownKey) && x.Mod == "Terraria");
            if (index != -1)
            {
                float finalCooldown = desperate ? DesperateCooldown : SpellCardCooldown;
                finalCooldown = float.Round(finalCooldown / 60f, 1);

                string newDescription = Language.GetTextValue(
                    "Mods.lenen.SpellCardManaDescription", ManaUse, finalCooldown);
                tooltips[index].Text = tooltips[index].Text.Replace(ManaCooldownKey, newDescription);
            }
        }
    }
}
