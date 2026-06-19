using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class Cut : ModProjectile
    {
        public float length { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletColor { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;

            Projectile.damage = 65;
            Projectile.knockBack = 6;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 30;
            Projectile.light = 0.5f;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;

            DrawOffsetX = 60;
            DrawOriginOffsetY = 0;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Pellet];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.05f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft == 20)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/hone_shot"), Projectile.Center);
            }

            if (Projectile.timeLeft > 20)
            {
                if (Projectile.scale < 1) Projectile.scale += 1f / 20f;
            } 

            if (Projectile.timeLeft <= 15)
            {
                Projectile.scale -= 1f / 15f;
            }

            if (Projectile.scale <= 0)
            {
                Projectile.timeLeft = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestraType type = Main._rand.Next([
                ParticleOrchestraType.Excalibur, ParticleOrchestraType.NightsEdge,
                ParticleOrchestraType.TrueExcalibur]);

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, type,
                new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
                Projectile.owner);

            hit.HitDirection = Main.player[Projectile.owner].Center.X < target.Center.X ? 1 : -1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ParticleOrchestraType type = Main._rand.Next([
                ParticleOrchestraType.Excalibur, ParticleOrchestraType.NightsEdge,
                ParticleOrchestraType.TrueExcalibur]);

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, type,
                new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
                Projectile.owner);

            info.HitDirection = Main.player[Projectile.owner].Center.X < target.Center.X ? 1 : -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 end = new Vector2(0, 120 * length).RotatedBy(Projectile.rotation);
            Vector2 start = -end;
            float w = 4;

            return BulletUtils.LineCollision(targetHitbox, Projectile, start, end, w, out _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(BulletColor, (int)Sheet.Pellet) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation,
                    scale = new Vector2(Projectile.scale * 0.65f, 10 * length)
                }
                );

            return false;
        }
    }
}
