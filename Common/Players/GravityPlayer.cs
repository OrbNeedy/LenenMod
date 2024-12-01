using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.GameInput;
using lenen.Common.Systems;
using System;
using lenen.Content.Projectiles;
using Terraria.Audio;

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

        public void SpawnExplosion(bool christmas = false)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/burst_01") with
            {
                Volume = 0.7f
            }, Player.Center);

            int damage = (int)(Player.GetWeaponDamage(Player.HeldItem) * 1.5f);
            if (Player.GetModPlayer<SpellCardManagement>().desperateBomb)
            {
                damage = (int)(Player.GetWeaponDamage(Player.HeldItem) * 2.25f);
            }
            float knockback = (int)(Player.GetWeaponKnockback(Player.HeldItem) * 0.75f);
            for (int i = 0; i < 65; i++)
            {
                int spriteType = Main.rand.Next((int)BulletSprites.Simple, (int)BulletSprites.Pellet + 1);
                int color = (int)BulletColors.White;
                if (christmas)
                {
                    color = Main.rand.NextBool()? (int)BulletColors.White : (int)BulletColors.Red;
                } else
                {
                    color = Main.rand.Next((int)BulletColors.White, (int)BulletColors.DarkBlue + 1);
                }
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, 
                    new Vector2(Main.rand.NextFloat(4, 22), 0).RotatedByRandom(MathHelper.TwoPi), 
                    ModContent.ProjectileType<FriendlyBullet>(), damage, knockback, Player.whoAmI, 
                    (int)BulletAIs.Slowing, color, spriteType);
            }
        }

        public override void PreUpdateMovement()
        {
            if (Player.stoned) return;
            if (antiGravity && !Player.shimmering && manualGravity)
            {
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
