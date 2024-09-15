using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class Cut : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;

            Projectile.damage = 65;
            Projectile.knockBack = 6;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 50;
            Projectile.light = 0.5f;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;

            DrawOffsetX = 60;
            DrawOriginOffsetY = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float widthMultiplier = 25f;
            float collisionPoint = 0f;

            Vector2 tip = (Projectile.Center + new Vector2(0, 60)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 bottom = (Projectile.Center + new Vector2(0, -60)).RotatedBy(Projectile.rotation, Projectile.Center);

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip, 
                bottom, widthMultiplier * Projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }
    }
}
