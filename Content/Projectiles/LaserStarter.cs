﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class LaserStarter : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.damage = 70;
            Projectile.knockBack = 5;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ownerHitCheck = false;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 10)
            {
                Projectile.velocity.Y += 0.2f;
            }
            if (Projectile.timeLeft < 510 && Main.rand.NextBool(45))
            {
                Projectile.timeLeft = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 velocity = new Vector2(6, 0);
            Vector2 offset = new Vector2(10, 10);
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                            Projectile.Center, velocity.RotatedBy(MathHelper.PiOver2 * i),
                            ModContent.ProjectileType<LaserGrid>(), 75, 2, Projectile.owner);
                }
                /*Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        Projectile.Center, new Vector2(0, 6),
                        ModContent.ProjectileType<LaserGrid>(), 75, 2, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        Projectile.Center, new Vector2(0, -6),
                        ModContent.ProjectileType<LaserGrid>(), 75, 2, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        Projectile.Center, new Vector2(-6, 0),
                        ModContent.ProjectileType<LaserGrid>(), 75, 2, Projectile.owner);*/
            } else
            {
                if (Main.rand.NextBool()) velocity *= -1;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<LaserGrid>(), 65, 2, Projectile.owner);
            }
        }
    }
}
