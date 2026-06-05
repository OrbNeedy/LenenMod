using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class SuperHaniwaFist : ModProjectile
    {
        public bool Desperate { get => Projectile.ai[0] != 0; set => Projectile.ai[0] = value ? 1 : 0; }
        public int Side { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int HeadIndex { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        public int attackCooldown = 45;
        public Vector2 target = Vector2.Zero;
        public bool attacking = false;

        public override void SetDefaults()
        {
            Projectile.width = 190;
            Projectile.height = 176;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 20;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.extraUpdates = 4;
            Projectile.hide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            attackCooldown = 60;

            //Main.NewText($"Spawned fist with side {Side}");
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!attacking)
            {
                modifiers.FinalDamage *= 0.1f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!attacking)
            {
                modifiers.FinalDamage *= 0.1f;
            }
        }

        public bool HeadActive()
        {
            if (HeadIndex == -1) return false;
            Projectile head = Main.projectile[HeadIndex];
            int headType = ModContent.ProjectileType<SuperHaniwaHead>();

            return head.type == headType && head.active && head.owner == Projectile.owner;
        }

        public override void AI()
        {
            bool activeOwner = SuperHaniwaHead.OwnerActive(Projectile.owner);
            bool headActive = HeadActive();
            float speed = 10;
            //Main.NewText($"Fist {Side} AI");
            //Main.NewText($"Time left: {Projectile.timeLeft}");
            if (!headActive)
            {
                //Main.NewText($"Owner: {activeOwner}, Head: {headActive}");
                //Main.NewText($"Deleting fist");
                return;
            } else
            {
                Projectile.timeLeft = 2;
            }

            Projectile head = Main.projectile[HeadIndex];
            Vector2 offset = new(256 * Side, 320);
            Vector2 idlePos = head.Center + offset;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Side;

            if (Main.myPlayer == Projectile.owner)
            {
                Player owner = Main.LocalPlayer;

                if (attacking)
                {
                    //Main.NewText($"Attacking");
                    if (Projectile.Center.Distance(target) <= speed ||
                        Projectile.Center.Distance(idlePos) > 800)
                    {
                        attacking = false;
                    }
                }
                else
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(target) * speed;

                    if (Projectile.Center.Distance(idlePos) <= speed * 4)
                    {
                        //Main.NewText($"Idle position");
                        target = Main.MouseWorld;
                        Projectile.Center = head.Center + offset;

                        if (attackCooldown <= 0 && activeOwner)
                        {
                            //Main.NewText($"Attacking");
                            attacking = true;
                        }
                    }
                    else
                    {
                        //Main.NewText($"Returning");
                        Projectile.Center += Projectile.Center.DirectionTo(idlePos) * speed;

                        attackCooldown = 45;
                    }
                }
                Projectile.netUpdate = true;
            }

            attackCooldown--;
        }

        public override bool ShouldUpdatePosition()
        {
            return attacking;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
