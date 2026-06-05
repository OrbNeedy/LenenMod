using lenen.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class SwordDroplet : ModProjectile
    {
        public bool CanLaser { get => Projectile.ai[0] == 0; set => Projectile.ai[0] = value ? 0 : 1; }
        public int ExtraTime { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public static float length = 1288;

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1288;

            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 0;
            Projectile.ArmorPenetration = 35;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Knife];

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);

            Projectile.timeLeft -= ExtraTime;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 90 && CanLaser)
            {
                Vector2 dir = -Projectile.velocity;

                if (Projectile.owner != -1)
                {
                    dir = Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center);
                }
                Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;

                Vector2 offset = dir.RotatedByRandom(MathHelper.PiOver4 / 2f);
                Projectile.position += offset * length;

                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bom_flash_00") with { 
                    Pitch = -1, 
                    PitchVariance = 0.1f
                }, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/burst_00") with
                {
                    PitchVariance = 0.1f
                }, Projectile.Center);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            // Stop moving after two seconds 
            // Projectiles that are not selected will continue regardless
            return Projectile.timeLeft > 240 || !CanLaser;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft <= 30) return false;

            if (Projectile.timeLeft <= 90 && CanLaser)
            {
                return BulletUtils.LineCollision(targetHitbox, Projectile, 
                    Projectile.velocity * length * 0.5f, 
                    -Projectile.velocity * length * 0.5f, 4, out _);
            }

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw as a laser after a while
            if (Projectile.timeLeft <= 90 && CanLaser)
            {
                float visiblePercent = float.Clamp(Projectile.timeLeft / 60f, 0, 1);
                Main.EntitySpriteDraw(
                BulletUtils.GetTexture((int)SheetFrame.Red, (int)Sheet.Pellet) with
                    {
                        position = Projectile.Center - Main.screenPosition,
                        rotation = Projectile.rotation, 
                        scale = new Vector2(1, 46 * 4), 
                        color = Color.White * visiblePercent
                    }
                    );
                return false;
            }

            Main.EntitySpriteDraw(
                BulletUtils.GetTexture((int)SheetFrame.Red, (int)Sheet.Knife) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation
                }
                );

            return false;
        }
    }
}
