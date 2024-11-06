using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class Laser : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 132;
            Projectile.height = 22;

            Projectile.damage = 20;
            Projectile.knockBack = 5;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 300;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_3"), Projectile.Center);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D laser = ModContent.Request<Texture2D>("lenen/Content/Projectiles/Laser").Value;
            Rectangle sourceRectangle = new Rectangle(0, 0, laser.Width, laser.Height);

            Vector2 origin = sourceRectangle.Size() / 2f;

            float offsetY = 11f;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

            Color color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(
                laser,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle,
                color,
                Projectile.rotation,
                origin,
                1f,
                SpriteEffects.None
            );
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float width = 12f;

            Vector2 start = Projectile.Left.RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 end = Projectile.Right.RotatedBy(Projectile.rotation, Projectile.Center);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end,
                width * Projectile.scale, ref collisionPoint);
        }
    }
}
