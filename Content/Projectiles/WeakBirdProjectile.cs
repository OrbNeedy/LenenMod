using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class WeakBirdProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 100;

            Projectile.damage = 20;
            Projectile.knockBack = 5;
            Projectile.penetrate = 1;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 300;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.25f;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
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

        public override void CutTiles()
        {
            Vector2 starting = (Projectile.Center + new Vector2(0, 50 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 ending = (Projectile.Center + new Vector2(0, -50 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);
            float width = 4f * Projectile.scale;

            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);

            Utils.PlotTileLine(starting, ending, width * Projectile.scale, cut);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 30;
            height = 30;

            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float widthMultiplier = 30f;
            float collisionPoint = 0f;

            Vector2 tip = (Projectile.Center + new Vector2(0, 50 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 bottom = (Projectile.Center + new Vector2(0, -50 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip,
                bottom, widthMultiplier * Projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }
    }
}
