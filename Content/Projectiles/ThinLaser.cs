using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class ThinLaser : ModProjectile
    {
        bool isGreen = false;
        Vector2 visualScale = new Vector2(0.25f, 1);
        bool accelerated = false;

        public override void SetDefaults()
        {
            Projectile.width = 0; 
            Projectile.height = 0;
            Projectile.light = 1f;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;

            Projectile.hide = true;
            Projectile.timeLeft = 480;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_2") with
            {
                Volume = 0.5f, 
                PitchVariance = 0.1f
            }, Projectile.Center);
            isGreen = Main.rand.NextBool();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center -= Vector2.Normalize(Projectile.velocity) * 85;
            base.OnSpawn(source);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft > 390)
            {
                Projectile.Center += Projectile.velocity * 2.5f;
            } else
            {
                if (!accelerated)
                {
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_3") with
                    {
                        Volume = 0.5f, 
                        PitchVariance = 0.1f
                    }, Projectile.Center);
                    accelerated = true;
                }
                if (visualScale.X < 1)
                {
                    visualScale.X += 0.03f;
                } 
                if (visualScale.X > 1)
                {
                    visualScale.X = 1;
                }
                Projectile.Center += Projectile.velocity * 12;
            }
            Projectile.netUpdate = true;
            base.AI();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDrawExtras()
        {
            Texture2D externalLaser = 
                ModContent.Request<Texture2D>("lenen/Content/Projectiles/ThinLaserExternal").Value;

            Color color = Color.Cyan;
            if (isGreen)
            {
                color = Color.Green;
            }

            Main.EntitySpriteDraw(
                    externalLaser,
                    Projectile.Center - Main.screenPosition,
                    externalLaser.Bounds,
                    color,
                    Projectile.rotation,
                    externalLaser.Size() / 2,
                    visualScale,
                    SpriteEffects.None
                );
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D innerLaser = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(
                    innerLaser,
                    Projectile.Center - Main.screenPosition,
                    innerLaser.Bounds,
                    Color.White,
                    Projectile.rotation,
                    innerLaser.Size() / 2,
                    visualScale,
                    SpriteEffects.None
                );
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            Vector2 start = Projectile.Center + new Vector2(0, 85).RotatedBy(Projectile.rotation);
            Vector2 end = Projectile.Center + new Vector2(0, -85).RotatedBy(Projectile.rotation);

            return Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), start, end, 10, 
                ref collisionPoint);
        }
    }
}
