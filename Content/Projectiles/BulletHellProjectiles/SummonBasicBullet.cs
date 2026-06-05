using lenen.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class SummonBasicBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Rotate { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

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
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 210;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Default];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (timer < 90)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Rotate * 0.08f);
            }

            timer++;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(BulletColor, BulletSprite) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation
                }
                );

            return false;
        }
    }
}
