using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class CriticalDamagePlayer : ModPlayer
    {
        public bool explosionUpgrade = false;
        public float additionalCritDamage = 0;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (explosionUpgrade && hit.Crit && hit.DamageType == DamageClass.Ranged && 
                Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/critical"), target.Center);

                Vector2 baseDir = new Vector2(0, 8);
                int projectile = ModContent.ProjectileType<CriticalHitBullets>();
                int damage = (int)(damageDone * 0.8f);

                for (int i = 0; i < 2; i++)
                {
                    int color = Main._rand.Next([1, 5, 6]);
                    baseDir = baseDir.RotatedByRandom(MathHelper.TwoPi);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, 
                        baseDir, projectile, damage, 2f, Player.whoAmI, color, (int)Sheet.Bullet, 
                        target.whoAmI);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += additionalCritDamage;
        }

        public override void ResetEffects()
        {
            explosionUpgrade = false;
            additionalCritDamage = 0;
        }
    }
}
