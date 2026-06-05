using lenen.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class SummonLaser : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        float realLength = 1;

        int timer = 0;

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 210;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Pellet];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (realLength < 14)
            {
                realLength += Projectile.velocity.Length() * 0.075f;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_3"), Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(BulletColor, (int)Sheet.Pellet) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation, 
                    scale = new(1, realLength)
                }
                );

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = new Vector2(0, 16 * realLength).RotatedBy(Projectile.rotation);
            Vector2 end = new Vector2(0, -16 * realLength).RotatedBy(Projectile.rotation);

            return BulletUtils.LineCollision(targetHitbox, Projectile, start, end, 8, out _);
        }
    }
}
