using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.TasoukenProjectiles
{
    class MovingBullets : ModProjectile
    {
        public int TasoukenIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int MoveTimer { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int TargetPhase { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int spriteType = 0;
        public int ownerType = -1;
        public Vector2 initialVel = Vector2.Zero;

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;

            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.ReverseDefault];

        public override void OnSpawn(IEntitySource source)
        {
            if (RingCircle.CheckBoss(TasoukenIndex))
            {
                ownerType = Main.npc[TasoukenIndex].type;
            }

            initialVel = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;

            Projectile.timeLeft -= MoveTimer;
        }

        public override void AI()
        {
            if (!RingCircle.ValidatePhase(TasoukenIndex, ownerType, TargetPhase))
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bullet_del"), Projectile.Center);
                Projectile.timeLeft = 0;
                return;
            }

            // Every 3 seconds
            if (Projectile.timeLeft <= 420)
            {
                if (Projectile.velocity.Length() < 10)
                {
                    Projectile.velocity += initialVel;
                }
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
            // return true; 
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int color = (int)SheetFrame.Yellow;
            int shape = (int)Sheet.Default;

            if (spriteType != 0)
            {
                color = (int)SheetFrame.Blue;
            }

            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(color, shape) with
                {
                    position = Projectile.Center - Main.screenPosition, 
                    scale = new Vector2(1, 1)
                });

            return false;
        }
    }
}
