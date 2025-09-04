using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using lenen.Content.Projectiles.BulletHellProjectiles;

namespace lenen.Content.Projectiles
{
    public enum EnemyBulletEffects
    {
        Default,
        MagicBounce
    }

    public class EnemyBullet : ModProjectile
    {
        private int BulletEffect { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private int BulletColor { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        private float BulletMisc { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        private Vector2 origin = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
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
            Projectile.timeLeft = 300;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            origin = Projectile.Center;
            base.OnSpawn(source);
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            switch (BulletEffect)
            {
                case (int)EnemyBulletEffects.MagicBounce:
                    int width = 1200;
                    int height = 900;
                    if (Projectile.Center.X <= origin.X - width && BulletMisc > 0)
                    {
                        Projectile.velocity.X *= -1;
                        Projectile.Center = new Vector2(origin.X - width + 1, Projectile.Center.Y);
                        BulletMisc -= 1;
                    }
                    if (Projectile.Center.Y <= origin.Y - height && BulletMisc > 0)
                    {
                        Projectile.velocity.Y *= -1;
                        Projectile.Center = new Vector2(Projectile.Center.X, origin.Y - height + 1);
                        BulletMisc -= 1;
                    }
                    if (Projectile.Center.X >= origin.X + width && BulletMisc > 0)
                    {
                        Projectile.velocity.X *= -1;
                        Projectile.Center = new Vector2(origin.X + width - 1,
                            Projectile.Center.Y);
                        BulletMisc -= 1;
                    }
                    if (Projectile.Center.Y >= origin.Y + width && BulletMisc > 0)
                    {
                        Projectile.velocity.Y *= -1;
                        Projectile.Center = new Vector2(Projectile.Center.X,
                            origin.Y + width - 1);
                        BulletMisc -= 1;
                    }
                    break;
            }
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("lenen/Content/Projectiles/EnemyBullet_Colored").Value;

            Main.EntitySpriteDraw(new DrawData(texture,
                Projectile.Center - Main.screenPosition,
                new Rectangle(30 * BulletColor, 0, 30, 30),
                Color.White,
                0f,
                new Vector2(15, 15),
                Projectile.scale,
                SpriteEffects.None)
            );

            return false;
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
