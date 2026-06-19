using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using lenen.Content.NPCs.TasoukenBoss;

namespace lenen.Content.Projectiles.TasoukenProjectiles
{
    public class ThrillBullet : ModProjectile
    {
        public int TasoukenIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int Segments { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int TargetPhase { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int color = 0;
        public int splitTimer = 0;
        public int ownerType = -1;
        public Vector2 initialVel = Vector2.Zero;
        public Vector2 initialPos = Vector2.Zero;

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
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

            initialPos = Projectile.Center;

            initialVel = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            color = BulletUtils.GetRandomColor(TasoukenBoss.GetDifficulty());
        }

        public override void AI()
        {
            if (!RingCircle.ValidatePhase(TasoukenIndex, ownerType, TargetPhase))
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bullet_del"), Projectile.Center);
                Projectile.timeLeft = 0;
                return;
            }

            // Boss already validated
            if (Projectile.Center.Distance(Main.npc[TasoukenIndex].Center) <= 32)
            {
                Projectile.timeLeft = 0;
                return;
            }

            // Create another 
            if (splitTimer >= 4 && Segments > 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                    initialPos, initialVel, Type, Projectile.damage, 
                    Projectile.knockBack, Projectile.owner, Projectile.ai[0], 
                    Projectile.ai[1] - 1, Projectile.ai[2]);
                Segments = 0;
            }

            if (Projectile.velocity.Length() < 14)
            {
                Projectile.velocity += initialVel;
            }

            splitTimer++;
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
            int shape = (int)Sheet.Big;

            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(color, shape) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    scale = new Vector2(1, 1), 
                    color = Color.White * 0.4f
                });

            return false;
        }
    }
}
