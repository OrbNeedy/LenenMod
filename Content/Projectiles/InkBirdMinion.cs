using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using lenen.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using lenen.Common.Players;
using lenen.Content.Projectiles.BulletHellProjectiles;
using System.IO;
using Terraria.DataStructures;

namespace lenen.Content.Projectiles
{
    class InkBirdMinion : ModProjectile
    {
        //Asset<Texture2D> body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/");
        int frame = 0;
        int frameTimer = 0;
        int sineTimer = 0;
        float stage { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
        float timer { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        float sineOffset { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }


        Vector2 targetPosition = Vector2.Zero;
        bool hasTarget = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; 

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 50;
            Projectile.knockBack = 1;
            Projectile.penetrate = -1;
            
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPosition);
            writer.Write(hasTarget);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPosition = reader.ReadVector2();
            hasTarget = reader.ReadBoolean();
        }

        public override void OnSpawn(IEntitySource source)
        {
            sineOffset = Main.rand.Next(-1000, 1000);
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            //Main.NewText("Existing at " + Projectile.Center);
            //Main.NewText("Target " + targetPosition);
            Player player = Main.player[Projectile.owner];

            if (!CheckActive(player))
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }

            float sineWaveValue = (float)Math.Sin(sineTimer + sineOffset);
            float speed = 8;
            float inertia = 20;

            CheckTarget(player);
            Vector2 finalTargetPosition = Vector2.Zero;
            if (hasTarget)
            {
                Attack(out finalTargetPosition, out speed, out inertia, targetPosition, sineTimer + sineOffset);
            } else
            {
                Idle(out finalTargetPosition, out speed, out inertia, targetPosition, sineTimer + sineOffset);
            }
            Movement(finalTargetPosition, speed, inertia);

            if (frameTimer++ >= 6)
            {
                if (frame++ >= 5)
                {
                    frame = 0;
                }
                frameTimer = 0;
            }
            sineTimer++;
            timer++;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<InkBirdMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<InkBirdMinionBuff>()))
            {
                Projectile.timeLeft = 2;
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
                    targetPosition = targetPlayer.target.Center;
                    hasTarget = true;
                }
            }
            Projectile.netUpdate = true;
            Projectile.friendly = hasTarget;
        }

        public void Idle(out Vector2 finalPosition, out float speed, out float inertia, 
            Vector2 playerPosition, float sineTimer)
        {
            speed = 8;
            inertia = 20;
            float sineValue = (float)Math.Sin((sineTimer + sineOffset) * 0.01);
            //Main.NewText(sineValue);
            Vector2 offset = new Vector2(60 * sineValue, 0).RotatedBy(sineTimer * 0.0008f);

            finalPosition = playerPosition + offset;
            //Main.NewText("Offset: " + offset); 
            //Main.NewText("Sine offset: " + sineOffset);

            if (Projectile.Center.Distance(finalPosition) > 600)
            {
                //Main.NewText("Speeding up");
                speed = Projectile.Center.Distance(finalPosition) / 40f;
                inertia = 20;
            }
        }

        public void Attack(out Vector2 finalPosition, out float speed, out float inertia, 
            Vector2 targetPosition, float sineTimer)
        {
            Vector2 offset = Vector2.Zero;
            float sineValue = (float)Math.Sin((sineTimer + sineOffset) * 0.08f);
            switch (stage)
            {
                case -1:
                case 1:
                    speed = 12;
                    inertia = 10;
                    offset = new Vector2(80 * stage, 0);

                    if (timer >= 180)
                    {
                        //Main.NewText("Change from: " + stage);
                        if (Main._rand.NextBool(4))
                        {
                            stage = Main._rand.NextBool() ? 0 : 2;
                        } else
                        {
                            stage = -stage;
                        }
                        //Main.NewText("To: " + stage);
                        timer = 0;
                        Projectile.netUpdate = true;
                        break;
                    }

                    if (timer%30 == 0 && timer > 60)
                    {
                        Vector2 baseDirection = Projectile.Center.DirectionTo(targetPosition) * 8;
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                baseDirection.RotatedBy(MathHelper.PiOver4 / 2 * i), 
                                ModContent.ProjectileType<BasicBullet>(),
                                (int)(Projectile.damage * 0.75f), 1f, Projectile.owner, (int)SheetFrame.Black, 
                                (int)Sheet.Default, (int)BulletBehavior.Penetrate);
                        }
                    }
                    break;
                case 2:
                    speed = 14;
                    inertia = 30;
                    offset = new Vector2(35 * sineValue, -80);

                    if (timer >= 300)
                    {
                        if (Main._rand.NextBool(40))
                        {
                            // What is this abomination I just made
                            stage = Main._rand.NextBool() ? 0 : Main._rand.NextBool() ? -1 : 1;
                            timer = 0;
                            Projectile.netUpdate = true;
                            break;
                        }
                    }

                    if (timer%10 == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                            new Vector2(0, 3.5f), ModContent.ProjectileType<BasicBullet>(),
                            (int)(Projectile.damage * 1.75f), 1f, Projectile.owner, (int)SheetFrame.Black, 
                            (int)Sheet.Double, (int)BulletBehavior.Penetrate);
                    }

                    break;
                default:
                    speed = 12;
                    inertia = 10;
                    offset = new Vector2(60 * sineValue, 0).RotatedBy(sineTimer * 0.008f);
                    
                    if (timer >= 360)
                    {
                        if (Main._rand.NextBool(40))
                        {
                            stage = Main._rand.NextBool() ? 2 : Main._rand.NextBool() ? -1 : 1;
                            timer = 0;
                            Projectile.netUpdate = true;
                        }
                    }
                    break;
            }
            finalPosition = targetPosition + offset;
        }

        public void Movement(Vector2 targetPosition, float speed, float inertia)
        {
            if (targetPosition == Projectile.Center)
            {
                targetPosition += new Vector2(1, 0).RotatedByRandom(MathHelper.TwoPi);
            }
            Vector2 direction = Projectile.Center.DirectionTo(targetPosition) * speed;
            //Main.NewText("Direction: " + direction);
            Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
            //Main.NewText("Velocity: " + Projectile.velocity);
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.NewText("Drawing");
            Asset<Texture2D> body = Terraria.GameContent.TextureAssets.Projectile[Type];

            Rectangle boundaries = body.Value.Bounds;
            boundaries.Height /= 6;
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
