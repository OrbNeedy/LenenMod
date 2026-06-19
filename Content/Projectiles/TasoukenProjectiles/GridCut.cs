using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.TasoukenProjectiles
{
    public class GridCut : ModProjectile
    {
        public int TasoukenIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int TargetPhase { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        int ownerType = -1;

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1400;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Pellet];

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_2"), Projectile.Center);
            Projectile.scale = 0.1f;

            if (RingCircle.CheckBoss(TasoukenIndex))
            {
                ownerType = Main.npc[TasoukenIndex].type;
            }

            // Main.NewText("Damage: " + Projectile.damage);
        }

        public override void AI()
        {
            if (!RingCircle.ValidatePhase(TasoukenIndex, ownerType, TargetPhase))
            {
                Projectile.timeLeft = 0;
                return;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft <= 60)
            {
                if (Projectile.timeLeft == 50)
                {
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_1"), Projectile.Center);
                }

                if (Projectile.timeLeft > 15)
                {
                    if (Projectile.scale < 1.25f) Projectile.scale += 0.05f;
                }
                else
                {
                    Projectile.scale -= 0.1f;
                }
            }

            if (Projectile.scale <= 0)
            {
                Projectile.timeLeft = 0;
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target) && Projectile.scale > 0.6f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.scale <= 0.6f)
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
            if (Projectile.scale <= 0.6f) return false;

            Vector2 end = new Vector2(0, 1080).RotatedBy(Projectile.rotation);
            Vector2 start = -end;
            float w = 8;

            return BulletUtils.LineCollision(targetHitbox, Projectile, start, end, w, out _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture((int)SheetFrame.Blue, (int)Sheet.Pellet) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation,
                    scale = new Vector2(Projectile.scale, 184)
                }
                );

            return false;
        }
    }
}
