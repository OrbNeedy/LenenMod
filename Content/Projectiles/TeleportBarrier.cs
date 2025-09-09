using lenen.Common.Players;
using lenen.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public enum BarrierType
    {
        Deflection, // Teleports hostile projectiles away from the player 
        Redirection // Teleports friendly projectiles to the Enemy
    }

    public class TeleportBarrier : ModProjectile
    {
        private bool Entry { get => Projectile.ai[0] == 0; }
        private int TeleportType { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        private int ConnectedBarrier { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        private int teleportValue = 100;
        private int teleportTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 280;
            Projectile.height = 280;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1800;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_2"), Projectile.Center);
        }

        public override void AI()
        {
            // Does nothing on other clients
            if (Main.myPlayer != Projectile.owner) return;

            if (Entry)
            {
                if (Main.player[Projectile.owner].active)
                {
                    Projectile.Center = Main.player[Projectile.owner].Center;
                    Projectile.netUpdate = true;
                }

                if (ConnectedBarrier >= 0)
                {
                    Projectile end = Main.projectile[ConnectedBarrier];

                    if (end.ModProjectile is TeleportBarrier otherBarrier && end.whoAmI != Projectile.whoAmI &&
                        end.owner == Projectile.owner && !otherBarrier.Entry && teleportValue > 0)
                    {
                        foreach (Projectile projectile in Main.ActiveProjectiles)
                        {
                            if (projectile.whoAmI == Projectile.whoAmI || projectile.whoAmI == end.whoAmI || 
                                TeleportExceptions.Instance.teleportImmuneProjectiles.Contains(projectile.whoAmI) || 
                                projectile.minion || projectile.damage <= 0 || 
                                ProjectileID.Sets.IsAGolfBall[projectile.type] || 
                                ProjectileID.Sets.IsADD2Turret[projectile.type] ||
                                ProjectileID.Sets.IsAGravestone[projectile.type] ||
                                ProjectileID.Sets.IsAWhip[projectile.type] ||
                                ProjectileID.Sets.IsInteractable[projectile.type] ||
                                ProjectileID.Sets.LightPet[projectile.type] ||
                                ProjectileID.Sets.MinionSacrificable[projectile.type]) continue;

                            if (TeleportType == (int)BarrierType.Deflection && !projectile.hostile) continue;
                            if (TeleportType == (int)BarrierType.Redirection && !projectile.friendly) continue;

                            if (CircularCollision(Projectile.Hitbox, Projectile.scale, projectile.Hitbox) &&
                                !CircularCollision(Projectile.Hitbox, Projectile.scale * 0.9f, projectile.Hitbox))
                            {
                                Vector2 collisionDirection = Projectile.Center.DirectionTo(projectile.Center);
                                float collisionDistance = Projectile.Center.Distance(projectile.Center) * end.scale;

                                if (collisionDistance >= Projectile.width * Projectile.scale * 0.9f)
                                {
                                    collisionDistance *= -1;
                                }

                                projectile.Center = end.Center + (collisionDirection * collisionDistance);
                                projectile.velocity *= -1;
                                projectile.netUpdate = true;

                                teleportValue -= projectile.damage / 10;
                            }
                        }
                    }
                }
                if (teleportTimer >= 60)
                {
                    if (teleportValue < 120)
                    {
                        teleportValue += 45;
                    }

                    teleportTimer = 0;
                }

                teleportTimer++;
            }
            else
            {
                if (TeleportType == (int)BarrierType.Redirection)
                {
                    if (ConnectedBarrier >= 0)
                    {
                        NPC target = Main.npc[ConnectedBarrier];

                        if (target.active)
                        {
                            Projectile.Center = target.Center;
                            Projectile.netUpdate = true;
                        }
                    } else
                    {
                        TargetPlayer targeting = Main.player[Projectile.owner].GetModPlayer<TargetPlayer>();
                        if (targeting.target != null)
                        {
                            ConnectedBarrier = targeting.target.whoAmI;
                        }
                    }

                    if (Projectile.timeLeft <= 900f * 1.5f)
                    {
                        float percent = (float)Projectile.timeLeft / 900f;
                        Projectile.scale = 1 - ((float)Math.Cos(percent) * 0.5f);
                    }
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;

            if (Entry && Main.myPlayer == Projectile.owner)
            {
                if (ConnectedBarrier >= 0)
                {
                    Projectile end = Main.projectile[ConnectedBarrier];
                    if (!end.active || end.ModProjectile is not TeleportBarrier || end.whoAmI == Projectile.whoAmI)
                    {
                        lightColor = Color.Black;
                    }
                } else
                {
                    lightColor = Color.Black;
                }
            }

            Asset<Texture2D> barrierTexture = ModContent.Request<Texture2D>("lenen/Content/Projectiles/TeleportBarrier");
            Rectangle bounds = new Rectangle(0, 0, barrierTexture.Value.Bounds.Width / 2, barrierTexture.Value.Bounds.Height);
            if (!Entry) bounds.X = bounds.Width;

            Main.EntitySpriteDraw(barrierTexture.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                lightColor,
                0f,
                bounds.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );
            return false;
        }

        public static bool CircularCollision(Rectangle projHitbox, float projScale, Rectangle targetHitbox)
        {
            Vector2 ellipsePosition = new Vector2(projHitbox.Left, projHitbox.Top);
            Vector2 ellipseDimentions = new Vector2(projHitbox.Width, projHitbox.Height);
            Vector2 ellipseCenter = ellipsePosition + 0.5f * ellipseDimentions;
            ellipseDimentions *= projScale;
            float x = 0f;
            float y = 0f;
            if (targetHitbox.Left > ellipseCenter.X)
            {
                x = targetHitbox.Left - ellipseCenter.X;
            }
            else if (targetHitbox.Left + targetHitbox.Width < ellipseCenter.X)
            {
                x = targetHitbox.Left + targetHitbox.Width - ellipseCenter.X;
            }
            if (targetHitbox.Top > ellipseCenter.Y)
            {
                y = targetHitbox.Top - ellipseCenter.Y;
            }
            else if (targetHitbox.Top + targetHitbox.Height < ellipseCenter.Y)
            {
                y = targetHitbox.Top + targetHitbox.Height - ellipseCenter.Y;
            }
            float a = ellipseDimentions.X / 2f;
            float b = ellipseDimentions.Y / 2f;
            return (x * x) / (a * a) + (y * y) / (b * b) <= 1;
        }
    }
}
