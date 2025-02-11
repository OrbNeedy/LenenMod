using lenen.Common.Players;
using lenen.Content.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class LumenBall : ModProjectile
    {
        Vector2 targetPosition = Vector2.Zero;
        bool hasTarget = false;

        int shootTimer = 0;
        int storedDamage { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.scale = 1f;
            Main.projFrames[Projectile.type] = 2;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
        }

        public override void OnSpawn(IEntitySource source)
        {
            storedDamage = Projectile.damage;
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
            Player player = Main.player[Projectile.owner];

            if (Projectile.frameCounter >= 8)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            } else
            {
                Projectile.frameCounter++;
            }

            if (!CheckActive(player))
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }

            Vector2 wantedPosition = player.Center + new Vector2(-22, -80);
            float distanceToPosition = Projectile.Center.Distance(wantedPosition);
            Projectile.position = wantedPosition;

            CheckTarget(player);
            PointToTarget();
            
            if (shootTimer <= 0 && hasTarget)
            {
                shootTimer = Shoot();
            }
            shootTimer--;

            Projectile.netUpdate = true;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active || !owner.GetModPlayer<BuffPlayer>().lumenBuff)
            {
                owner.ClearBuff(ModContent.BuffType<LumenBallBuff>());
                Projectile.Kill();
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<LumenBallBuff>()))
            {
                Projectile.timeLeft = 3;
            }

            return true;
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
                        targetPosition = npc.Center + (npc.velocity * 10);
                        hasTarget = true;
                        return;
                    }
                }

                targetPosition = Main.MouseWorld;
                hasTarget = false;
                NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                if (target != null)
                {
                    if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height,
                        target.position, target.width, target.height))
                    {
                        targetPosition = target.Center + (target.velocity * 10);
                        hasTarget = true;
                    }
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

        private int Shoot()
        {
            // If for some reason the beam velocity can't be correctly normalized, set it to a default value.
            Projectile.velocity = Projectile.Center.DirectionTo(targetPosition);
            
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
                ModContent.ProjectileType<LumenLaser>(), damage, knockback, Projectile.owner, ai1: uuid);

            Projectile.netUpdate = true;
            return 270;
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
