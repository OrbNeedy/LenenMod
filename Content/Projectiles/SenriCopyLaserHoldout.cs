using lenen.Common.Players;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using lenen.Content.Projectiles.BulletHellProjectiles;

namespace lenen.Content.Projectiles
{
    class SenriCopyLaserHoldout : ModProjectile
    {
        public int BeamColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BeamStartAngle { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        Vector2 targetPosition = Vector2.Zero;
        bool hasTarget = false;

        int shootTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.scale = 1f;

            Projectile.DamageType = ModContent.GetInstance<UniversalHybrid>();
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }

        public override string Texture => "lenen/Assets/Textures/Empty";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/charge_1") with {
                Volume = 0.5f
            }, Projectile.Center);
            //Main.NewText("New Moonlord Laser Instance.");
            //Projectile.damage = 0;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            if (Main.myPlayer != Projectile.owner) return;

            Player player = Main.player[Projectile.owner];

            if (!CheckActive(player))
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }

            Vector2 wantedPosition = player.Center;
            Projectile.Center = wantedPosition;

            int dustID = Dust.NewDust(
                Projectile.position, Projectile.width, Projectile.height, DustID.TintableDust, 
                newColor: BeamColor == (int)SheetFrame.White ? Color.White : Color.Black
                );
            Main.dust[dustID].noGravity = true;

            CheckTarget(player);

            if (shootTimer == 60)
            {
                //Main.NewText("Shooting laser.");
                Shoot();
            }
            if (shootTimer > 60)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2 / 300 * -BeamStartAngle);
            }
            else
            {
                PointToTarget();
            }

            shootTimer++;

            Projectile.netUpdate = true;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active || !owner.GetModPlayer<SenriPlayer>().senriActive)
            {
                //Main.NewText("Killing because no suitable owner.");
                Projectile.Kill();
                return false;
            }

            return true;
        }

        private void CheckTarget(Player owner)
        {
            targetPosition = Main.MouseWorld;
            hasTarget = true;
            NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
            if (target != null)
            {
                if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height,
                    target.position, target.width, target.height))
                {
                    targetPosition = target.Center;
                }
            }
        }

        private void PointToTarget()
        {
            if (hasTarget)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity,
                    Projectile.Center.DirectionTo(targetPosition), 0.08f);
            }
        }

        private void Shoot()
        {
            // If for some reason the beam velocity can't be correctly normalized, set it to a default value.
            Projectile.velocity = Projectile.Center.DirectionTo(targetPosition)
                .RotatedBy(MathHelper.PiOver4 * BeamStartAngle);

            Vector2 beamVelocity = Vector2.Normalize(Projectile.velocity);
            if (beamVelocity.HasNaNs())
            {
                beamVelocity = -Vector2.UnitY;
            }

            // This UUID will be the same between all players in multiplayer, ensuring that the beams are properly anchored on the Prism on everyone's screen.
            int uuid = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);

            int damage = Projectile.damage;
            float knockback = Projectile.knockBack;
            // Position doesn't matter as the beam itself should correct it's position on it's own
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity,
                ModContent.ProjectileType<SenriCopyLaser>(), damage, knockback, Projectile.owner, BeamColor, uuid);
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_1") with
            {
                Volume = 0.5f
            }, Projectile.Center);

            Projectile.netUpdate = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}
