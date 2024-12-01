using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace lenen.Content.Projectiles
{
    public class EnemyBullet : ModProjectile
    {
        private Vector2 origin = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
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
            switch (Projectile.ai[0])
            {
                case 1:
                    int width = 1200;
                    int height = 850;
                    if (Projectile.Center.X <= origin.X - width && Projectile.ai[2] > 0)
                    {
                        Projectile.velocity.X *= -1;
                        Projectile.Center = new Vector2(origin.X - width + 1, Projectile.Center.Y);
                        Projectile.ai[2] -= 1;
                    }
                    if (Projectile.Center.Y <= origin.Y - height && Projectile.ai[2] > 0)
                    {
                        Projectile.velocity.Y *= -1;
                        Projectile.Center = new Vector2(Projectile.Center.X, origin.Y - height + 1);
                        Projectile.ai[2] -= 1;
                    }
                    if (Projectile.Center.X >= origin.X + width && Projectile.ai[2] > 0)
                    {
                        Projectile.velocity.X *= -1;
                        Projectile.Center = new Vector2(origin.X + width - 1,
                            Projectile.Center.Y);
                        Projectile.ai[2] -= 1;
                    }
                    if (Projectile.Center.Y >= origin.Y + width && Projectile.ai[2] > 0)
                    {
                        Projectile.velocity.Y *= -1;
                        Projectile.Center = new Vector2(Projectile.Center.X,
                            origin.Y + width - 1);
                        Projectile.ai[2] -= 1;
                    }
                    break;
            }
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Color changer
            Color color = Color.White;
            Texture2D texture = ModContent.Request<Texture2D>("lenen/Content/Projectiles/ColoredEnemyBullet").Value;

            lightColor = Color.White;

            switch (Projectile.ai[1])
            {
                case 0:
                    color = Color.DarkCyan;
                    break;
                case 1:
                    color = Color.GreenYellow;
                    break;
                case 2:
                    color = Color.Yellow;
                    break;
                case 3:
                    color = Color.Red;
                    break;
                case 4:
                    color = Color.Purple;
                    break;
                case 5:
                    color = Color.Black;
                    lightColor = Color.Black;
                    break;
                default:
                    color = Color.White;
                    break;
            }

            Main.EntitySpriteDraw(new DrawData(texture,
                Projectile.Center - Main.screenPosition - (new Vector2(16) * Projectile.scale),
                null,
                color,
                0f,
                Vector2.Zero,
                Projectile.scale,
                SpriteEffects.None)
            );

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
