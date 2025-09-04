using lenen.Content.Projectiles;
using lenen.Content.Projectiles.BulletHellProjectiles;
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

        public virtual bool CanShoot(int size, int level, NPC npc, FairyType type, Vector2 distraction, bool distracted)
        {
            if (npc.target <= -1 || npc.target >= 500)
            {
                return false;
            }
            Player target = Main.player[npc.target];
            Vector2 targetPosition = target.position;
            Vector2 targetCenter = target.Center;
            if (distracted)
            {
                targetCenter = distraction;
                targetPosition = distraction - target.Size / 2;
            }

            return Collision.CanHitLine(npc.position, npc.width, npc.height,
                        targetPosition, target.width, target.height);
        }

        public virtual int Shoot(int size, int level, NPC npc, FairyType type, Vector2 distraction, bool distracted)
        {
            if (npc.target <= -1 || npc.target >= 500)
            {
                return 0;
            }
            Player target = Main.player[npc.target];
            Vector2 targetPosition = target.position;
            Vector2 targetCenter = target.Center;
            if (distracted)
            {
                targetCenter = distraction;
                targetPosition = distraction - target.Size/2;
            }

            if (!Collision.CanHitLine(npc.position, npc.width, npc.height,
                        targetPosition, target.width, target.height)) return 120;

            int speed = size * 60;
            int damage = 10 + (5 * size);
            int color;
            color = GetRandColor();
            if (Main.expertMode)
            {
                speed += 60;
            }
            if (Main.masterMode)
            {
                speed += 60;
            }
            damage += level;

            switch (type)
            {
                case FairyType.Shot:
                    float difference = 8f / (float)maxStack;
                    if (stackType)
                    {
                        for (int i = 0; i < maxStack; i++)
                        {
                            color = GetRandColor();
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                npc.Center.DirectionTo(targetCenter) * (12 - (difference * i)),
                                ModContent.ProjectileType<EnemyBullet>(), damage, 2.5f, ai1: color);
                        }
                    }
                    else
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 direction = npc.Center.DirectionTo(targetCenter).RotatedBy(i * MathHelper.Pi / 5);
                            for (int k = 0; k < maxStack; k++)
                            {
                                color = GetRandColor();
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
                            color = GetRandColor();
                            Vector2 direction = npc.Center.DirectionTo(targetCenter).RotatedBy(
                                i * MathHelper.Pi / 18);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                direction * 10, ModContent.ProjectileType<EnemyBullet>(),
                                damage, 2.5f, ai0: 1, ai1: color, ai2: bounces);
                        }
                    }
                    else
                    {
                        color = GetRandColor();
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                            npc.Center.DirectionTo(targetCenter) * 10, ModContent.ProjectileType<EnemyBullet>(),
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
                            color = GetRandColor(true);
                            Vector2 direction = npc.Center.DirectionTo(targetCenter).RotatedBy(
                                i * MathHelper.Pi / 18);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                direction * 10, ModContent.ProjectileType<EnemyBullet>(),
                                damage, 2.5f, ai1: color);
                        }
                    }
                    else
                    {
                        color = GetRandColor(true);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                            npc.Center.DirectionTo(targetCenter) * 10, ModContent.ProjectileType<EnemyBullet>(),
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
                            color = GetRandColor(type is FairyType.Mono);
                            Vector2 direction = npc.Center.DirectionTo(targetCenter).RotatedBy(
                                i * MathHelper.Pi / 18);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                direction * 10, ModContent.ProjectileType<EnemyBullet>(),
                                damage, 2.5f, ai1: color);
                        }
                    }
                    else
                    {
                        color = GetRandColor(type is FairyType.Mono);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                            npc.Center.DirectionTo(targetCenter) * 10, ModContent.ProjectileType<EnemyBullet>(),
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

        public int GetRandColor(bool monochrome = false)
        {
            if (monochrome) return (int)Main.rand.NextFromList(SheetFrame.White, SheetFrame.Black);

            SheetFrame color = Main.rand.NextFromList(SheetFrame.Blue, SheetFrame.Cyan, SheetFrame.Green);
            switch (Main.GameMode)
            {
                case GameModeID.Creative: 
                    return (int)Main.rand.NextFromList(SheetFrame.Blue, SheetFrame.Cyan);
                default:
                case GameModeID.Normal:
                    return (int)Main.rand.NextFromList(SheetFrame.Blue, SheetFrame.Cyan, SheetFrame.Green);
                case GameModeID.Expert:
                    return (int)Main.rand.NextFromList(SheetFrame.Green, SheetFrame.Yellow, SheetFrame.Red);
                case GameModeID.Master:
                    return (int)Main.rand.NextFromList(SheetFrame.Pink, SheetFrame.Red);
            }
        }
    }
}
