using System.Collections.Generic;
using System.IO;
using lenen.Common.Graphics;
using lenen.Common.Players;
using lenen.Content.Buffs;
using lenen.Content.Items.Vanity.Dyes;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class HaniwaClone : ModProjectile
    {
        public bool FromCreationTools { get => Projectile.ai[0] != 0; }

        Vector2 targetPosition = Vector2.Zero;
        Vector2 movementTarget = Vector2.Zero;
        bool hasTarget = false;
        float moveSpeed = 6f;

        public Player clonedPlayer = new();
        int movementTimer = 0;

        int attackTimer = 0;
        int attackType = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 56;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 50;
            Projectile.knockBack = 1;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
        }

        public override string Texture => "lenen/Assets/Textures/Empty";

        public bool OwnerActive(Player owner)
        {
            //Main.NewText("Owner check");
            bool cloneBuff = owner.HasBuff(ModContent.BuffType<HaniwaCloneBuff>());
            bool commanderBuff = owner.HasBuff(ModContent.BuffType<HaniwaCommander>());
            bool buffCheck = (!FromCreationTools && cloneBuff) || (FromCreationTools && commanderBuff);

            return owner.active && owner.statLife > 0 && !owner.DeadOrGhost &&
                buffCheck;
        }

        public bool OwnerActive(int who)
        {
            //Main.NewText("Who: " + who);
            if (who == -1) return false;

            Player owner = Main.player[who];

            //Main.NewText("Check: " + (owner.active && owner.statLife > 0 && !owner.DeadOrGhost));
            return OwnerActive(owner);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPosition);
            writer.Write(hasTarget);
            writer.Write7BitEncodedInt(attackTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPosition = reader.ReadVector2();
            hasTarget = reader.ReadBoolean();
            attackTimer = reader.Read();
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (OwnerActive(Projectile.owner))
            {
                Player owner = Main.player[Projectile.owner];
                clonedPlayer.CopyVisuals(owner);

                BuffPlayer buff = owner.GetModPlayer<BuffPlayer>();

                if (FromCreationTools)
                {
                    Projectile.minionSlots = buff.prevSummonSlots * 0.3f;
                }
                else
                {
                    Projectile.minionSlots = buff.prevSummonSlots;
                }
            }
        }

        public override void AI()
        {
            //Main.NewText("AI start");
            Vector2 target = movementTarget;

            if (!OwnerActive(Projectile.owner))
            {
                Projectile.timeLeft = 0;
                return;
            }

            //Main.NewText("Owner check successful");
            Projectile.timeLeft = 2;
            Player owner = Main.player[Projectile.owner];
            BuffPlayer buff = owner.GetModPlayer<BuffPlayer>();

            if (FromCreationTools)
            {
                Projectile.minionSlots = buff.prevSummonSlots * 0.3f;
            }
            else
            {
                Projectile.minionSlots = buff.prevSummonSlots;
            }

            CheckTarget(owner);
            if (hasTarget)
            {
                Attack(out target, targetPosition);
            }
            else
            {
                Idle(out target, targetPosition);
            }
            movementTarget = target;
            Movement(movementTarget);

            clonedPlayer.Center = Projectile.Center;
            clonedPlayer.velocity = Projectile.velocity;
            clonedPlayer.direction = Projectile.direction;

            clonedPlayer.PlayerFrame();
            clonedPlayer.WingFrame(clonedPlayer.velocity.Y != 0);
        }

        public void Idle(out Vector2 newTarget, Vector2 target)
        {
            //Main.NewText("Idleing");
            if (movementTimer <= 0)
            {
                Vector2 offset1 = new(0, -96);
                Vector2 offset2 = new(Main._rand.NextFloat(-96, 97), Main._rand.NextFloat(-64, 65));

                newTarget = target + offset1 + offset2;
                Projectile.netUpdate = true;
                return;
            }

            newTarget = movementTarget;
        }

        public void Attack(out Vector2 newTarget, Vector2 target)
        {
            //Main.NewText("Attacking");
            if (movementTimer <= 0)
            {
                Vector2 offset = new(128, 0);

                newTarget = target + offset.RotatedByRandom(MathHelper.TwoPi);
                Projectile.netUpdate = true;
            } else
            {
                newTarget = movementTarget;
            }

            if (Main.myPlayer != Projectile.owner) return;

            if (Projectile.Center == movementTarget && attackTimer <= 0)
            {
                Vector2 baseDir = Projectile.Center.DirectionTo(targetPosition);

                int addedDamage = (int)(Projectile.minionSlots * 10);
                if (FromCreationTools) addedDamage = (int)(Projectile.minionSlots * 45);
                int finalDamage = Projectile.damage + addedDamage;

                bool laserShooting = Main.player[Projectile.owner].
                    ownedProjectileCounts[ModContent.ProjectileType<SuperHaniwaHead>()] > 0;
                List<int> typeSelectionPool = new([0, 1, 2, 3, 4]);

                switch (attackType)
                {
                    // Big fan
                    case 0:
                    default:
                        int projType = ModContent.ProjectileType<SummonBasicBullet>();
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 dir = baseDir.RotatedBy(i * MathHelper.PiOver4 / 2f) * 6;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                                Projectile.Center, dir, projType, finalDamage, 
                                3.5f, Projectile.owner, (int)SheetFrame.Yellow, (int)Sheet.ReverseBig);
                        }
                        typeSelectionPool.Remove(0);
                        break;
                    case 1: // Random knife
                        projType = ModContent.ProjectileType<SummonBasicBullet>();
                        for (int i = 0; i < 22; i++)
                        {
                            Vector2 dir = baseDir.RotatedByRandom(MathHelper.TwoPi) *
                                Main._rand.NextFloat(3.5f, 8f);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                                Projectile.Center, dir, projType, finalDamage,
                                1.5f, Projectile.owner, (int)SheetFrame.Yellow, (int)Sheet.Knife);
                        }
                        typeSelectionPool.Remove(1);
                        break;
                    case 2: // Rotated big
                        projType = ModContent.ProjectileType<SummonBasicBullet>();
                        float rot = MathHelper.TwoPi / 12f;
                        for (int i = 0; i < 12; i++)
                        {
                            Vector2 dir = baseDir.RotatedBy(i * rot) * 8;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                                Projectile.Center, dir, projType, finalDamage,
                                1.5f, Projectile.owner, (int)SheetFrame.Red, (int)Sheet.ReverseBig,
                                Projectile.localAI[0] % 2 == 0 ? 1 : -1);
                        }
                        typeSelectionPool.Remove(2);
                        break;
                    case 3: // Stacked fan
                        projType = ModContent.ProjectileType<SummonBasicBullet>();
                        rot = MathHelper.TwoPi / 32f;
                        int bulletAmount = Projectile.localAI[0] % 2 == 0 ? 3 : 5;
                        int min = bulletAmount / 2;
                        for (int k = 0; k < 6; k++)
                        {
                            for (int i = -min; i < min + 1; i++)
                            {
                                Vector2 dir = baseDir.RotatedBy(i * rot) * (6 + (k));
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                                    Projectile.Center, dir, projType, finalDamage,
                                    1.5f, Projectile.owner, (int)SheetFrame.Yellow, (int)Sheet.ReverseDefault);
                            }
                        }
                        typeSelectionPool.Remove(3);
                        break;
                    case 4: // Lasers
                        projType = ModContent.ProjectileType<SummonLaser>();
                        rot = MathHelper.TwoPi / 12f;
                        for (int i = 0; i < 12; i++)
                        {
                            Vector2 dir = baseDir.RotatedBy(i * rot) * 6;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                                Projectile.Center, dir, projType, finalDamage,
                                2.5f, Projectile.owner, (int)SheetFrame.Red);
                        }
                        typeSelectionPool.Remove(4);
                        break;
                    case 5: // Aimed laser
                        projType = ModContent.ProjectileType<SummonLaser>();
                        rot = MathHelper.TwoPi / 28f;
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 dir = baseDir.RotatedBy(i * rot) * 6;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                                Projectile.Center, dir, projType, finalDamage,
                                2.5f, Projectile.owner, (int)SheetFrame.Cyan);
                        }
                        break;
                }

                if (laserShooting && FromCreationTools) typeSelectionPool.AddRange([5, 5, 5, 5]);
                attackType = Main._rand.Next(typeSelectionPool);
                attackTimer = 60;
                Projectile.localAI[0]++;
                Projectile.netUpdate = true;
            }
        }

        public void Movement(Vector2 target)
        {
            //Main.NewText("Moving");
            attackTimer--;
            if (Projectile.Center == target)
            {
                movementTimer--;
                return;
            }

            if (movementTimer <= 0)
            {
                float distance = Projectile.Center.Distance(target);

                if (distance > 360)
                {
                    moveSpeed = distance / 60f;
                }
                else
                {
                    moveSpeed = 6f;
                }
            }

            Vector2 vel = Projectile.Center.DirectionTo(target);
            movementTimer = 90;
            Projectile.Center += vel * moveSpeed;
            Projectile.velocity = vel;

            if (Projectile.Center.Distance(target) < moveSpeed)
            {
                Projectile.Center = target;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float increase = 10;

            if (FromCreationTools)
            {
                // Creation tools divide minion slots by 3 so all three minions will fit
                // This is equal to 15 per minion slot if it was a single minion 
                increase = 45;
            }

            modifiers.SourceDamage.Base += Projectile.minionSlots * increase;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            float increase = 10;

            if (FromCreationTools)
            {
                increase = 45;
            }

            modifiers.SourceDamage.Base += Projectile.minionSlots * increase;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreKill(int timeLeft)
        {
            Player owner = Main.player[Projectile.owner];
            BuffPlayer buff = owner.GetModPlayer<BuffPlayer>();
            
            int maxMinions = buff.prevSummonSlots;
            
            float sample = Projectile.minionSlots;
            return base.PreKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!RenderTarget.canUseHaniwa) return false;

            SpriteBatchState tempState = SpriteBatchExt.GetState(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState, SpriteSortMode.Immediate);

            Rectangle playerRect = RenderTarget.
                GetHaniwaRect(Projectile.owner);
            Rectangle sourceRectangle = new Rectangle(Projectile.owner * playerRect.Width, 0,
                playerRect.Width, playerRect.Height);

            MiscShaderData shader = GameShaders.Misc["Haniwa"];

            Vector2 position = Projectile.Center - Main.screenPosition - playerRect.Size() / 2f;

            DrawData data = new(
                RenderTarget.haniwaRenderTarget,
                position - (clonedPlayer.Size / 2f),
                playerRect,
                Color.White);

            shader.Apply(data);

            data.Draw(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState);

            return false;
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
    }
}
