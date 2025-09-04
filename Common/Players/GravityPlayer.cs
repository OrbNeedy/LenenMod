using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.GameInput;
using lenen.Common.Systems;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using lenen.Content.Projectiles.BulletHellProjectiles;

namespace lenen.Common.Players
{
    public class GravityPlayer : ModPlayer
    {
        public bool antiGravity = false;
        private bool manualGravity = false;
        public bool levitation = true;
        public bool gravityResist = false;

        public override void ResetEffects()
        {
            antiGravity = false;
            levitation = false;
            gravityResist = false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.gravityAdjust.JustPressed)
            {
                manualGravity = !manualGravity;
            }
        }

        public override void PreUpdate()
        {
            if (Player.stoned) return;
            if (levitation)
            {
                Player.AddBuff(BuffID.Featherfall, 2);
                Player.wingTime = Player.wingTimeMax-1;
                Player.noFallDmg = true;
            }
            if (gravityResist)
            {
                Player.buffImmune[BuffID.VortexDebuff] = true;
            }
            base.PreUpdate();
        }

        public void SpawnExplosion(bool christmas = false, IEntitySource? source = null)
        {
            // Check source, add one if there is none
            Main.NewText("Exploding with source: " + source);
            IEntitySource useSource;
            if (source == null)
            {
                useSource = Player.GetSource_FromThis("Explosion");
                Main.NewText("Source changed");
            } else
            {
                useSource = source;
                Main.NewText("Source kept");
            }

            // Play sound effect
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/burst_01") with
            {
                Volume = 0.7f
            }, Player.Center);

            // Set damage and knockback, increase damage if it's desperate
            int damage = (int)(Player.GetWeaponDamage(Player.HeldItem) * 1.5f);
            if (Player.GetModPlayer<SpellCardManagement>().desperateBomb)
            {
                damage = (int)(Player.GetWeaponDamage(Player.HeldItem) * 2.25f);
            }
            float knockback = (int)(Player.GetWeaponKnockback(Player.HeldItem) * 0.75f);
            
            // Spawn bullets
            for (int i = 0; i < 40; i++)
            {
                // Set the sprite type and color
                int spriteType = Main.rand.Next((int)Sheet.Default, (int)Sheet.Pellet + 1);
                int color = (int)SheetFrame.White;
                // Christmas version only selects white and red colors
                if (christmas)
                {
                    color = Main.rand.NextBool()? (int)SheetFrame.White : (int)SheetFrame.Red;
                } else
                {
                    color = Main.rand.Next((int)SheetFrame.White, (int)SheetFrame.Blue + 1);
                }

                Projectile.NewProjectile(useSource, Player.Center, 
                    new Vector2(Main.rand.NextFloat(4, 22), 0).RotatedByRandom(MathHelper.TwoPi), 
                    ModContent.ProjectileType<SlowedBullet>(), damage, knockback, Player.whoAmI, 
                    color, spriteType);
            }
        }

        public void HaveGravityField(Vector2 position, float strength, float range = 200, bool projectiles = true, int timeDecay = 2)
        {
            if (projectiles)
            {
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    float distance = projectile.Distance(position);
                    if (distance <= range && projectile.friendly &&
                        GravityExceptions.Instance.gravityAffectedProjectiles.Contains(projectile.type))
                    {
                        projectile.velocity = Vector2.Lerp(projectile.velocity, 
                            projectile.DirectionTo(position) * strength, 0.06f);
                        if (timeDecay > 0)
                        {
                            if (Main.rand.NextBool(timeDecay)) projectile.timeLeft += 1;
                        }
                        projectile.netUpdate = true;
                    }
                }
            } else
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    float distance = npc.Distance(position);
                    // Not a boss, not friendly, knockback restist over 0, in range, not a gravity resistant NPC
                    if (!npc.boss && !npc.friendly && npc.knockBackResist > 0 && distance <= range * 2 &&
                        !GravityExceptions.Instance.gravityResistantNPCs.Contains(npc.type))
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, 
                            npc.DirectionTo(position) * strength * npc.knockBackResist, 0.12f);
                        npc.netUpdate = true;
                    }
                }
            }
        }

        public override void PreUpdateMovement()
        {
            if (Player.stoned) return;
            if (antiGravity && !Player.shimmering && manualGravity)
            {
                Player.fallStart = (int)Player.Center.Y;
                int width = (int)(Player.Size.X + 10);
                int height = (int)(Player.Size.Y + 26);
                Vector2 offset = new Vector2((Player.Size.X / -2) - 5,
                    ((Player.Size.Y / 2) - 16));
                if (Player.gravDir == -1) offset.Y = ((Player.Size.Y * -2f) + 16);
                //Main.NewText("Offset: " + offset);
                if (Collision.SolidCollision(Player.Center + offset, width, height))
                {
                    if (Math.Abs(Player.velocity.Y * Player.gravDir) < Math.Abs(20 * Player.gravDir))
                    {
                        Player.velocity += new Vector2(0, Player.gravity * -2f);
                    }
                }
            }
            base.PreUpdateMovement();
        }
    }
}
