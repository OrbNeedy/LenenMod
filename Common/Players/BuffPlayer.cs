using lenen.Common.Systems;
using lenen.Common.Utils;
using lenen.Content.Projectiles;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class BuffPlayer : ModPlayer
    {
        public bool virusDebuff { get; set; }
        public float HarujionDebuff { get; set; }
        private int harujionTimer = 0;
        public int BarrierBuff { get; set; }
        public bool LumenBuff { get; set; }

        private bool BeingAbsorbed { get; set; }
        private int absorptionTimer = 0;
        public int prevSummonSlots = 0;

        public bool CanCut { get; set; }
        private int[] CutCooldown { get; set; } = { 0, 0, 0 };

        public override void ResetEffects()
        {
            int sample = Player.maxMinions;

            LumenBuff = false;
            virusDebuff = false;
            HarujionDebuff = 0;
            BarrierBuff = 0;

            CanCut = false;
            for (int i = 0; i < CutCooldown.Length; i++)
            {
                if (CutCooldown[i] > 0) CutCooldown[i]--;
            }
        }

        public override void PreUpdate()
        {
            int sample = Player.maxMinions;

            prevSummonSlots = Player.maxMinions;
        }

        public override void PostUpdate()
        {
            if (CutCooldown.Any((num) => { return num <= 0; }) && CanCut)
            {
                // Cut aura
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.friendly || npc.immortal || 
                        npc.immune[Player.whoAmI] > 0) continue;

                    if (npc.Center.Distance(Player.Center) - (npc.width / 2f) <= 64)
                    {
                        for (int i = 0; i < CutCooldown.Length; i++)
                        {
                            if (CutCooldown[i] > 0) continue;

                            int type = ModContent.ProjectileType<Cut>();
                            int damage = Player.GetWeaponDamage(Player.HeldItem);
                            Vector2 vel = new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi);

                            int color = BulletUtils.GetRandomColor([SheetFrame.White, SheetFrame.Pink, SheetFrame.Yellow]);
                            Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem),
                                npc.Center, vel, type, damage, 6f, Player.whoAmI, 0.6f, color);

                            CutCooldown[i] = 10;
                            break;
                        }
                    }
                }
            }

            int ballType = ModContent.ProjectileType<LumenBall>();
            if (LumenBuff && Player.ownedProjectileCounts[ballType] < 1)
            {
                EntitySource_Parent source = new EntitySource_Parent(Player);
                var projectile = Projectile.NewProjectileDirect(source, Player.Center, Vector2.Zero,
                    ballType, (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(35),
                    0, Main.myPlayer, 0);
                projectile.damage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(35);
            }

            if (harujionTimer > 0)
            {
                harujionTimer--;
            }

            if (BeingAbsorbed)
            {
                //Main.NewText("Collecting SOULS " + absorptionTimer);
                //Main.NewText("Collecting SOULS " + beingAbsorbed);
                if (absorptionTimer <= 0)
                {
                    BeingAbsorbed = false;
                    return;
                }
                SoulAbsorptionPlayer soulManager = Player.GetModPlayer<SoulAbsorptionPlayer>();
                if (soulManager.soulsCollected <= 0)
                {
                    BeingAbsorbed = false;
                    absorptionTimer = 0;
                    return;
                }

                soulManager.soulsCollected -= 1;
                ModContent.GetInstance<HarujionLocations>().soulsAbsorbed += 1;

                absorptionTimer--;
            }
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            if (Main.dedServ) return;
            //Main.NewText("Absorbed: " + beingAbsorbed);
            //Main.NewText("Timer: " + absorptionTimer);
            if (HarujionDebuff > 0 && !BeingAbsorbed)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/rei_drain_00"), Player.Center);
                BeingAbsorbed = true;
                absorptionTimer = 120;
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (virusDebuff)
            {
                //Language.GetTextValue("Mods.lenen.Death.RNA", Player.name)
                damageSource.CustomReason = NetworkText.FromKey("Mods.lenen.Death.RNA", Player.name);
            }

            if (HarujionDebuff > 0f)
            {
                damageSource.CustomReason = NetworkText.FromKey("Mods.lenen.Death.Harujion", Player.name);
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void UpdateBadLifeRegen()
        {
            if (HarujionDebuff > 0)
            {
                //Main.NewText("Potency: " + harujionDebuff);
                SoulAbsorptionPlayer soulManager = Player.GetModPlayer<SoulAbsorptionPlayer>();
                if (soulManager.soulsCollected <= 0)
                {
                    Player.lifeRegen -= (int)(12 * HarujionDebuff) - 6;
                } else
                {
                    if (harujionTimer <= 0)
                    {
                        soulManager.soulsCollected -= 1;
                        ModContent.GetInstance<HarujionLocations>().soulsAbsorbed += (int)HarujionDebuff;
                        harujionTimer = 15;
                    }
                    Player.lifeRegen -= (int)(HarujionDebuff) - 2;
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
