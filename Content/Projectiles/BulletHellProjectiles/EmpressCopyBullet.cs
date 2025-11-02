using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class EmpressCopyBullet : ModProjectile
    {
        int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        int Behavior { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        Vector2 initialVelocity;
        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 1f;
            Projectile.light = 0.375f;

            Projectile.DamageType = ModContent.GetInstance<UniversalHybrid>();
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 100;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 254;
        }

        public override string Texture => BasicBullet.emptyRoute;

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            initialVelocity = Projectile.velocity;
            if (Behavior == 0)
            {
                Projectile.timeLeft += 60;
                Projectile.penetrate = 1;
            }
        }

        public override void AI()
        {
            if (Behavior == 0)
            {
                Projectile.velocity = initialVelocity * MathHelper.Clamp(timer * 0.006f, 0, 2);
            } else
            {
                if (Projectile.timeLeft%15 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                        Projectile.velocity, Projectile.type, Projectile.damage, 
                        Projectile.knockBack, Projectile.owner, 0);
                }

                Projectile.velocity = Projectile.velocity.RotatedBy(0.025f * Behavior);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Sheet sprite = Sheet.Ofuda;
            if (Behavior == 0) sprite = Sheet.Default;
            Rectangle bounds = BasicBullet.GetBulletBounds(sprite, (SheetFrame)BulletColor);

            Main.EntitySpriteDraw(new DrawData(
                BasicBullet.GetBulletTexture(sprite),
                Projectile.Center - Main.screenPosition,
                bounds,
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                bounds.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None)
            );
            return false;
        }
    }
}
