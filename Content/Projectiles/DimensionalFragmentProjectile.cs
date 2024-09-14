using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class DimensionalFragmentProjectile : ModProjectile
    {
        private static Asset<Texture2D> Extra = null;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1200;
            Projectile.light = 0.25f;

            DrawOffsetX = 2;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (MathHelper.Pi/2);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item143 with { Volume = 0.25f, PitchVariance = 0.1f });
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(SoundID.Item143 with { Volume = 0.25f, PitchVariance = 0.1f });
            base.OnHitPlayer(target, info);
        }

        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float widthMultiplier = 14f;
            float collisionPoint = 1f;

            Rectangle fragmentHitboxBounds = new Rectangle(0, 0, 100, 100);

            fragmentHitboxBounds.X = (int)Projectile.position.X - fragmentHitboxBounds.Width / 2;
            fragmentHitboxBounds.Y = (int)Projectile.position.Y - fragmentHitboxBounds.Height / 2;

            Vector2 tip1 = Projectile.Top.RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 tip2 = Projectile.Bottom.RotatedBy(Projectile.rotation, Projectile.Center);

            if (fragmentHitboxBounds.Intersects(targetHitbox)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip1, tip2, widthMultiplier * Projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }*/
    }
}
