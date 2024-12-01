﻿using lenen.Common.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class BuffPlayer : ModPlayer
    {
        public bool virusDebuff { get; set; }
        public float harujionDebuff { get; set; }
        private int harujionTimer = 0;
        public int barrierBuff { get; set; }

        public override void ResetEffects()
        {
            virusDebuff = false;
            harujionDebuff = 0;
            barrierBuff = 0;
        }

        public override void PostUpdate()
        {
            if (harujionTimer > 0)
            {
                harujionTimer--;
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (virusDebuff)
            {
                damageSource = PlayerDeathReason.ByCustomReason(
                    Language.GetTextValue("Mods.lenen.Death.RNA", Player.name));//$"{Player.name}'s DNA was scrambled");
            }

            if (harujionDebuff > 0f)
            {
                damageSource = PlayerDeathReason.ByCustomReason(
                    Language.GetTextValue("Mods.lenen.Death.Harujion", Player.name));//$"{Player.name}'s soul became plant food");
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void UpdateBadLifeRegen()
        {
            if (harujionDebuff > 0)
            {
                //Main.NewText("Potency: " + harujionDebuff);
                SoulAbsorptionPlayer soulManager = Player.GetModPlayer<SoulAbsorptionPlayer>();
                if (soulManager.soulsCollected <= 0)
                {
                    Player.lifeRegen -= (int)(16 * harujionDebuff) - 8;
                } else
                {
                    if (harujionTimer <= 0)
                    {
                        soulManager.soulsCollected -= (int)harujionDebuff;
                        HarujionLocations.instance.soulsAbsorbed += (int)harujionDebuff;
                        harujionTimer = 30;
                    }
                    Player.lifeRegen -= (int)(4 * harujionDebuff) - 2;
                    //Main.NewText("Souls: " + harujionLocations.soulsAbsorbed);
                    //Main.NewText("Player souls: " + soulManager.soulsCollected);
                }
            }

            if (virusDebuff)
            {
                if (Player.lifeRegen > 0) Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 20;
            }
        }
    }
}
