using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace lenen.Content.Projectiles
{
    public class EnemyBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.scale = 1f;
            Projectile.light = 0.35f;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;

            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 210;
        }

        public override string Texture => "lenen/Content/Projectiles/Bullet";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);

            // Sprite changer
            switch(Projectile.ai[0])
            {
                default:
                    break;
            }

            if (Projectile.ai[2] > 0)
            {
                Projectile.scale = Projectile.ai[2];
            }

            base.OnSpawn(source);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Color changer
            switch(Projectile.ai[1])
            {
                case 1:
                    lightColor = Color.Yellow;
                    break;
                default:
                    lightColor = Color.White;
                    break;
            }
            return base.PreDraw(ref lightColor);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 ellipsePosition = new Vector2(projHitbox.Left, projHitbox.Top);
            Vector2 ellipseDimentions = new Vector2(projHitbox.Width, projHitbox.Height);
            Vector2 ellipseCenter = ellipsePosition + 0.5f * ellipseDimentions;
            ellipseDimentions *= Projectile.scale;
            float x = 0f;
            float y = 0f;
            if (targetHitbox.Left > ellipseCenter.X)
            {
                x = targetHitbox.Left - ellipseCenter.X;
            }
            else if (targetHitbox.Left + targetHitbox.Width < ellipseCenter.X)
            {
                x = targetHitbox.Left + targetHitbox.Width - ellipseCenter.X;
            }
            if (targetHitbox.Top > ellipseCenter.Y)
            {
                y = targetHitbox.Top - ellipseCenter.Y;
            }
            else if (targetHitbox.Top + targetHitbox.Height < ellipseCenter.Y)
            {
                y = targetHitbox.Top + targetHitbox.Height - ellipseCenter.Y;
            }
            float a = ellipseDimentions.X / 2f;
            float b = ellipseDimentions.Y / 2f;
            return (x * x) / (a * a) + (y * y) / (b * b) <= 1;
        }
    }
}
