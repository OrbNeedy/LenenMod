using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.TasoukenProjectiles
{
    class PunishmentCut : ModProjectile
    {
        public int DeductedTime { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 75;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Pellet];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.1f;
            Projectile.timeLeft -= DeductedTime;

            // Main.NewText("Damage: " + Projectile.damage);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft <= 45)
            {
                if (Projectile.timeLeft == 30)
                {
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/hone_shot"), Projectile.Center);
                }

                if (Projectile.timeLeft > 20)
                {
                    if (Projectile.scale < 1) Projectile.scale += 0.15f;
                }
                else
                {
                    Projectile.scale -= 1f;
                }
            }

            if (Projectile.scale <= 0)
            {
                Projectile.timeLeft = 0;
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target) && Projectile.scale > 0.5f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.scale <= 0.5f)
            {
                return false;
            }

            return base.CanHitNPC(target);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.scale <= 0.5f) return false;

            Vector2 end = new Vector2(0, 120).RotatedBy(Projectile.rotation);
            Vector2 start = -end;
            float w = 4;

            return BulletUtils.LineCollision(targetHitbox, Projectile, start, end, w, out _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture((int)SheetFrame.White, (int)Sheet.Pellet) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation,
                    scale = new Vector2(Projectile.scale * 0.8f, 12)
                }
                );

            return false;
        }
    }
}
