using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.NPCs.Fairy.Patterns
{
    public class Shapes : Pattern
    {
        int shape = 0;
        public override void SetDefaults(int size, int level, NPC npc, FairyType type)
        {
            int maxType = 2;
            if (type == FairyType.Slash) maxType = 3;
            shape = Main.rand.Next(maxType);
        }

        public override int Shoot(int size, int level, NPC npc, FairyType type)
        {
            if (npc.target <= -1 || npc.target >= 500)
            {
                return 0;
            }
            Player target = Main.player[npc.target];

            if (!Collision.CanHitLine(npc.position, npc.width, npc.height,
                        target.position, target.width, target.height)) return 120;

            int speed = size * 90;
            int damage = 15 + (5 * size);
            float bulletSpeed = 10f;
            int color;
            color = Main.rand.NextFromList<int>(0, 1);
            damage += (level * (1 + size));

            float baseDistance = 50f;
            Vector2 offset = new Vector2(baseDistance, 0);
            Vector2 direction = npc.Center.DirectionTo(target.Center);
            int ai0 = 0;
            int ai2 = 0;

            switch (type)
            {
                case FairyType.Magic:
                    ai0 = 1;
                    ai2 = 1 + (level / 5);
                    break;
                case FairyType.Shot:
                    direction = npc.Center.DirectionTo(target.Center + (target.velocity*20));
                    bulletSpeed = 12.5f;
                    baseDistance = 30f;
                    offset = new Vector2(baseDistance, 0);
                    break;
            }

            switch (shape)
            {
                case 0:
                    for (int i = 0; i < 20; i++)
                    {
                        if (type == FairyType.Mono)
                        {
                            color = Main.rand.NextFromList<int>(5, 6);
                        } else
                        {
                            if (Main.expertMode)
                            {
                                color = Main.rand.NextFromList<int>(2, 3);
                            }
                            if (Main.masterMode)
                            {
                                color = Main.rand.NextFromList<int>(3, 4);
                            }
                        }
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + offset,
                            direction * bulletSpeed, ModContent.ProjectileType<EnemyBullet>(), damage, 2.5f,
                            ai0: ai0, ai1: color, ai2: ai2);

                        offset = offset.RotatedBy(MathHelper.TwoPi / 20);
                    }
                    break;
                case 1:
                    offset = direction.RotatedBy(MathHelper.PiOver4) * baseDistance;
                    for (int i = -5; i < 6; i++)
                    {
                        if (type == FairyType.Mono)
                        {
                            color = Main.rand.NextFromList<int>(5, 6);
                        }
                        else
                        {
                            if (Main.expertMode)
                            {
                                color = Main.rand.NextFromList<int>(2, 3);
                            }
                            if (Main.masterMode)
                            {
                                color = Main.rand.NextFromList<int>(3, 4);
                            }
                        }
                        float distance = (float)i / 5f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + (offset * distance),
                            direction * bulletSpeed, ModContent.ProjectileType<EnemyBullet>(), damage, 2.5f,
                            ai0: ai0, ai1: color, ai2: ai2);
                    }

                    offset = offset.RotatedBy(MathHelper.PiOver2);
                    for (int i = -5; i < 6; i++)
                    {
                        if (type == FairyType.Mono)
                        {
                            color = Main.rand.NextFromList<int>(5, 6);
                        }
                        else
                        {
                            if (Main.expertMode)
                            {
                                color = Main.rand.NextFromList<int>(2, 3);
                            }
                            if (Main.masterMode)
                            {
                                color = Main.rand.NextFromList<int>(3, 4);
                            }
                        }
                        float distance = (float)i / 5f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + (offset * distance),
                            direction * bulletSpeed, ModContent.ProjectileType<EnemyBullet>(), damage, 2.5f, 
                            ai0: ai0, ai1: color, ai2: ai2);
                    }
                    break;
                case 2:
                    Vector2 positionMovement = direction;
                    offset = direction.RotatedBy(MathHelper.PiOver4 * 3) * baseDistance;
                    for (int i = 0; i < 21; i++)
                    {
                        if (type == FairyType.Mono)
                        {
                            color = Main.rand.NextFromList<int>(5, 6);
                        }
                        else
                        {
                            if (Main.expertMode)
                            {
                                color = Main.rand.NextFromList<int>(2, 3);
                            }
                            if (Main.masterMode)
                            {
                                color = Main.rand.NextFromList<int>(3, 4);
                            }
                        }
                        if (i > 5 && i < 15)
                        {
                            positionMovement = positionMovement.RotatedBy(-MathHelper.Pi / 9);
                        }
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + offset,
                            direction * bulletSpeed, ModContent.ProjectileType<EnemyBullet>(), damage, 2.5f, 
                            ai0: ai0, ai1: color, ai2: ai2);
                        offset += positionMovement*12;
                    }
                    break;
            }

            return 600 - speed;
        }
    }
}
