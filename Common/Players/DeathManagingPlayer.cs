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
                Main.NewText(NetworkText.FromKey("Mods.lenen.Death.Denied"), 
                    new Color(104, 64, 128));
                return false;
            }
            if (Main.rand.NextBool(50))
            {
                damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.lenen.Death.BigMistake", Player.name));
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            NetworkText deathText = NetworkText.FromKey("Mods.lenen.Death.BigMistake", Player.name);
            if (damageSource.CustomReason == deathText)
            {
                int stack = Main.rand.Next(1, 3);
                Main.LocalPlayer.QuickSpawnItem(Player.GetSource_GiftOrReward(),
                ModContent.ItemType<NoiseDye>(), stack);
            }

            SoulAbsorptionPlayer modPlayer = Player.GetModPlayer<SoulAbsorptionPlayer>();
            if (modPlayer.soulsCollected > 0)
            {
                if (modPlayer.soulsCollected - 100 >= 0)
                {
                    modPlayer.soulsCollected -= 100;

                    for (int i = 0; i < 100; i += 10)
                    {
                        Item.NewItem(Player.GetSource_Death(), Player.Center, ModContent.ItemType<SoulItem>(),
                            10);
                    }
                } else
                {
                    int diff = modPlayer.soulsCollected;

                    while (diff > 0)
                    {
                        int stack = (int)MathHelper.Clamp(diff, 0, 10);

                        // A bit redundant, but it's fine
                        if (stack > 0)
                        {
                            Item.NewItem(Player.GetSource_Death(), Player.Center, ModContent.ItemType<SoulItem>(),
                                stack);
                        } else
                        {
                            break;
                        }

                        diff -= stack;
                    }

                    /*int count = 1;
                    for (int i = 0; i < diff; i += 10)
                    {
                        int targetStack = (10 * count) - i;

                        Item.NewItem(Player.GetSource_Death(), Player.Center, ModContent.ItemType<SoulItem>(),
                            targetStack);

                        count++;
                    }*/

                    modPlayer.soulsCollected = 0;
                }
            }
        }
    }
}
