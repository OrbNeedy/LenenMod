using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Enums;
using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework.Graphics;

namespace lenen.Content.Projectiles
{
    public class InfiniteLaser : ModProjectile
    {
        public int ExtendedTime { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float RotationPerFrame { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int BulletColor { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
            Projectile.width = 70;
            Projectile.height = 234;

            Projectile.damage = 30;
            Projectile.knockBack = 12;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 2;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.light = 1f;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 60;

            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft += ExtendedTime;
            Projectile.scale = BigLaser.minWidth;

            Projectile.rotation = Projectile.velocity.ToRotation();

            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_3"), Projectile.Center);
        }

        public override void AI()
        {
            if (Projectile.owner != -1)
            {
                Player owner = Main.player[Projectile.owner];
                if (owner.active && !owner.DeadOrGhost)
                {
                    Projectile.Center = owner.Center;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Projectile.timeLeft > 130 && Projectile.timeLeft <= 150)
            {
                if (Projectile.scale < 2) Projectile.scale += 2 / 20f;
            }

            if (Projectile.timeLeft > 20 && Projectile.timeLeft <= 130)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(RotationPerFrame);
            }

            if (Projectile.timeLeft <= 15)
            {
                Projectile.scale -= 2f / 15f;
            }

            Vector2 end = new Vector2(0, 1460).RotatedBy(Projectile.rotation);

            DelegateMethods.v3_1 = Color.White.ToVector3() * 1;
            Utils.PlotTileLine(Projectile.Center, end, 40f*Projectile.scale, 
                new Utils.TileActionAttempt(DelegateMethods.CastLight));
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void CutTiles()
        {
            Vector2 end = new Vector2(0, 1460).RotatedBy(Projectile.rotation);
            float width = 40f * Projectile.scale;

            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;

            Utils.PlotTileLine(Projectile.Center, end, width * Projectile.scale, 
                new Utils.TileActionAttempt(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 end = new Vector2(0, 1460).RotatedBy(Projectile.rotation);
            float w = 70 * Projectile.scale;

            return BulletUtils.LineCollision(targetHitbox, Projectile, Vector2.Zero, end, w, out _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetLaserTexture(BulletColor) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation,
                    scale = new Vector2(Projectile.scale, 2),
                    color = Color.White * Projectile.Opacity * (1f - Projectile.scale)
                }
                );

            Main.EntitySpriteDraw(
                BigLaser.LightBrightness.Value,
                Projectile.Center - Main.screenPosition,
                BigLaser.LightBrightness.Frame(),
                Color.White * float.Clamp((Projectile.scale + 0.08f) * 1.25f, 0, 1) * Projectile.Opacity,
                Projectile.rotation,
                new Vector2(BigLaser.LightBrightness.Width() / 2f, 140),
                new Vector2(Projectile.scale, 2),
                SpriteEffects.None
                );

            return false;
        }
    }
}
