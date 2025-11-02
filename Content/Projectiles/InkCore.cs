using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using lenen.Common.Players;

namespace lenen.Content.Projectiles
{
    class InkCore : ModProjectile
    {
        int frame = 0;
        int frameTimer = 0;
        Vector2 targetPosition = Vector2.Zero;
        bool hasTarget = false;
        bool attacking = false;

        public ref float ShootTimer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; 
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Main.NewText("Attacking: " + attacking);

            if (!attacking)
            {
                CheckTarget(player);
            }

            if (hasTarget)
            {
                if (ShootTimer >= 150)
                {
                    attacking = false;
                    ShootTimer = 0;
                }

                if (ShootTimer % 30 == 0 && ShootTimer > 60)
                {
                    AttackPattern(ShootTimer == 120, targetPosition);
                }
            }

            if (frameTimer++ >= 30)
            {
                if (frame++ >= 3)
                {
                    frame = 0;
                }
                frameTimer = 0;
            }
            ShootTimer++;
        }

        private void CheckTarget(Player owner)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (owner.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                    float between = Vector2.Distance(npc.Center, Projectile.Center);

                    if (between < 4000f)
                    {
                        targetPosition = npc.Center;
                        hasTarget = true;
                        return;
                    }
                }

                hasTarget = false;
                targetPosition = owner.Center;
                TargetPlayer targetPlayer = Main.LocalPlayer.GetModPlayer<TargetPlayer>();
                if (targetPlayer.target != null)
                {
                    if (Collision.CanHitLine(Projectile.Center, Projectile.width, Projectile.height,
                        targetPlayer.target.position, targetPlayer.target.width, targetPlayer.target.height) ||
                        targetPlayer.targetDistance < 150f)
                    {
                        targetPosition = targetPlayer.target.Center;
                        hasTarget = true;
                    }
                }
            }
            Projectile.friendly = hasTarget;
        }

        public void AttackPattern(bool singleRing, Vector2 targetPosition)
        {
            attacking = !singleRing;
            int layers = singleRing ? 1 : 6;
            int sprite = singleRing ? (int)Sheet.Big : (int)Sheet.Pellet;
            int color = (int)SheetFrame.White;
            Vector2 baseDirection = Projectile.Center.DirectionTo(targetPosition);
            float angle = MathHelper.TwoPi / 8;
            float minSpeed = 1.5f;
            float speedChange = singleRing ? 4.5f : 1.25f;
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < layers; k++)
                {
                    minSpeed += speedChange;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                        baseDirection.RotatedBy(angle * i) * minSpeed, 
                        ModContent.ProjectileType<SentryBullet>(), Projectile.damage, 1f, 
                        Projectile.owner, color, sprite, (int)BulletBehavior.Penetrate);
                }
                minSpeed = 1.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> body = Terraria.GameContent.TextureAssets.Projectile[Type];

            Rectangle boundaries = body.Value.Bounds;
            boundaries.Height /= 4;
            boundaries.Y = boundaries.Height * frame;

            Main.EntitySpriteDraw(body.Value,
                Projectile.Center - Main.screenPosition,
                boundaries,
                Color.White,
                Projectile.rotation,
                boundaries.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );

            return false;
        }
    }
}
