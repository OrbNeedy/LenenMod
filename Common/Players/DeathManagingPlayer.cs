using lenen.Content.Buffs;
using lenen.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class DeathManagingPlayer : ModPlayer
    {
        public bool harujionRevival = false;

        public override void ResetEffects()
        {
            harujionRevival = false;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            SoulAbsorptionPlayer soulManager = Player.GetModPlayer<SoulAbsorptionPlayer>();
            if (harujionRevival && soulManager.soulsCollected >= 500 && 
                !Player.HasBuff(ModContent.BuffType<ResurrectionCooldown>()))
            {
                soulManager.soulsCollected -= 500;
                Player.Heal(Player.statLifeMax + Player.statLifeMax2);
                Player.AddBuff(ModContent.BuffType<ResurrectionCooldown>(), 3600);
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/rei_drain_00"), Player.Center);
                Main.NewText(Language.GetTextValue("Mods.lenen.Death.Denied"), 
                    new Color(104, 64, 128));
                return false;
            }
            if (Main.rand.NextBool(40))
            {
                damageSource = PlayerDeathReason.ByCustomReason(
                    Language.GetTextValue("Mods.lenen.Death.BigMistake", Player.name));//$"{Player.name} Made a big mistake");
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            string mistake = Language.GetTextValue("Mods.lenen.Death.BigMistake", Player.name);
            if (damageSource.SourceCustomReason == mistake)
            {
                int stack = Main.rand.Next(1, 3);
                Main.LocalPlayer.QuickSpawnItem(Player.GetSource_GiftOrReward(),
                ModContent.ItemType<NoiseDye>(), stack);
            }

            SoulAbsorptionPlayer modPlayer = Player.GetModPlayer<SoulAbsorptionPlayer>();
            if (modPlayer.soulsCollected > 0)
            {
                modPlayer.soulsCollected -= 100;
                if (modPlayer.soulsCollected < 0)
                {
                    int difference = 0 - modPlayer.soulsCollected;
                    modPlayer.soulsCollected += 100 - difference;
                    int finalDifference = 100 - difference;
                    for (int i = finalDifference; i > 0; i -= 10)
                    {
                        int differenceWithi = 5;
                        if (i - 10 < 0)
                        {
                            differenceWithi = 10 - i;
                        }
                        Item.NewItem(Player.GetSource_Death(), Player.Center, ModContent.ItemType<SoulItem>(), 
                            differenceWithi);
                    }
                }
                for (int i = 0; i < 100; i += 10)
                {
                    Item.NewItem(Player.GetSource_Death(), Player.Center, ModContent.ItemType<SoulItem>(),
                        10);
                }
            }
        }
    }
}
