using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.NPCs.Fairy.Patterns
{
    public class Pattern
    {
        bool stackType = false;
        int maxStack = 1;
        int stack = 1;
        // Returns the cooldown for the next attack
        // Size: Size of the fairy (Small(0), Shikigami(1), Big(2))
        // Type: The type of fairy (Mono, Slash, Shot, Magic)
        public virtual void SetDefaults(int size, int level, NPC npc, FairyType type)
        {
            maxStack = 1;

            float selectedType = Main.rand.NextFloat();
            if (selectedType <= 0.3333f)
            {
                stackType = true;
                maxStack += size;
                if (level >= 10)
                {
                    maxStack += level / 3;
                }
            }
            if (selectedType >= 0.6666f)
            {
                stackType = false;
                maxStack += size;
                if (level >= 10)
                {
                    maxStack += level / 3;
                }
            }

            stack = maxStack;
        }

        public virtual int Shoot(int size, int level, NPC npc, FairyType type)
        {
            if (npc.target <= -1 || npc.target >= 500)
            {
                return 0;
            }
            Player target = Main.player[npc.target];

            if (!Collision.CanHitLine(npc.position, npc.width, npc.height,
                        target.position, target.width, target.height)) return 120;

            int speed = size * 60;
            int damage = 15 + (5 * size);
            int color;
            color = Main.rand.NextFromList<int>(0, 1);
            if (Main.expertMode)
            {
                color = Main.rand.NextFromList<int>(2, 3);
                speed += 60;
            }
            if (Main.masterMode)
            {
                color = Main.rand.NextFromList<int>(3, 4);
                speed += 60;
            }
            damage += (level * (1 + size));

            switch (type)
            {
                case FairyType.Shot:
                    float difference = 8f / (float)maxStack;
                    if (stackType)
                    {
                        for (int i = 0; i < maxStack; i++)
                        {
                            color = Main.rand.NextFromList<int>(0, 1);
                            if (Main.expertMode)
                            {
                                color = Main.rand.NextFromList<int>(2, 3);
                            }
                            if (Main.masterMode)
                            {
                                color = Main.rand.NextFromList<int>(3, 4);
                            }
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                npc.Center.DirectionTo(target.Center) * (12 - (difference * i)),
                                ModContent.ProjectileType<EnemyBullet>(), damage, 2.5f, ai1: color);
                        }
                    }
                    else
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 direction = npc.Center.DirectionTo(target.Center).RotatedBy(i * MathHelper.Pi / 5);
                            for (int k = 0; k < maxStack; k++)
                            {
                                color = Main.rand.NextFromList<int>(0, 1);
                                if (Main.expertMode)
                                {
                                    color = Main.rand.NextFromList<int>(2, 3);
                                }
                                if (Main.masterMode)
                                {
                                    color = Main.rand.NextFromList<int>(3, 4);
                                }
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                    direction * (12 - (difference * k)),
                                    ModContent.ProjectileType<EnemyBullet>(), damage, 2.5f, ai1: color);
                            }
                        }
                    }
                    break;
                case FairyType.Magic:
                    if (stack <= 0)
                    {
                        stack = maxStack;
                        return 600 - speed;
                    }
                    int bounces = 1 + (level / 5);
                    if (stackType)
                    {
                        for (int i = -3; i < 4; i += 2)
                        {
                            color = Main.rand.NextFromList<int>(0, 1);
                            if (Main.expertMode)
                            {
                                color = Main.rand.NextFromList<int>(2, 3);
                            }
                            if (Main.masterMode)
                            {
                                color = Main.rand.NextFromList<int>(3, 4);
                            }
                            Vector2 direction = npc.Center.DirectionTo(target.Center).RotatedBy(
                                i * MathHelper.Pi / 18);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                direction * 10, ModContent.ProjectileType<EnemyBullet>(),
                                damage, 2.5f, ai0: 1, ai1: color, ai2: bounces);
                        }
                    }
                    else
                    {
                        color = Main.rand.NextFromList<int>(0, 1);
                        if (Main.expertMode)
                        {
                            color = Main.rand.NextFromList<int>(2, 3);
                        }
                        if (Main.masterMode)
                        {
                            color = Main.rand.NextFromList<int>(3, 4);
                        }
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                            npc.Center.DirectionTo(target.Center) * 10, ModContent.ProjectileType<EnemyBullet>(),
                            damage, 2.5f, ai0: 1, ai1: color, ai2: bounces);
                    }
                    if (stack > 0)
                    {
                        stack--;
                        return 4;
                    }
                    break;
                case FairyType.Mono:

                    if (stack <= 0)
                    {
                        stack = maxStack;
                        return 600 - speed;
                    }
                    if (stackType)
                    {
                        for (int i = -3; i < 4; i += 2)
                        {
                            color = Main.rand.NextFromList<int>(5, 6);
                            Vector2 direction = npc.Center.DirectionTo(target.Center).RotatedBy(
                                i * MathHelper.Pi / 18);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                direction * 10, ModContent.ProjectileType<EnemyBullet>(),
                                damage, 2.5f, ai1: color);
                        }
                    }
                    else
                    {
                        color = Main.rand.NextFromList<int>(5, 6);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                            npc.Center.DirectionTo(target.Center) * 10, ModContent.ProjectileType<EnemyBullet>(),
                            damage, 2.5f, ai1: color);
                    }
                    if (stack > 0)
                    {
                        stack--;
                        return 4;
                    }
                    break;
                default:
                    if (stack <= 0)
                    {
                        stack = maxStack;
                        return 600 - speed;
                    }
                    if (stackType)
                    {
                        for (int i = -3; i < 4; i += 2)
                        {
                            color = Main.rand.NextFromList<int>(0, 1);
                            if (Main.expertMode)
                            {
                                color = Main.rand.NextFromList<int>(2, 3);
                            }
                            if (Main.masterMode)
                            {
                                color = Main.rand.NextFromList<int>(3, 4);
                            }
                            Vector2 direction = npc.Center.DirectionTo(target.Center).RotatedBy(
                                i * MathHelper.Pi / 18);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                direction * 10, ModContent.ProjectileType<EnemyBullet>(),
                                damage, 2.5f, ai1: color);
                        }
                    }
                    else
                    {
                        color = Main.rand.NextFromList<int>(0, 1);
                        if (Main.expertMode)
                        {
                            color = Main.rand.NextFromList<int>(2, 3);
                        }
                        if (Main.masterMode)
                        {
                            color = Main.rand.NextFromList<int>(3, 4);
                        }
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                            npc.Center.DirectionTo(target.Center) * 10, ModContent.ProjectileType<EnemyBullet>(),
                            damage, 2.5f, ai1: color);
                    }
                    if (stack > 0)
                    {
                        stack--;
                        return 4;
                    }
                    break;
            }
            return 600 - speed;
        }
    }
}
