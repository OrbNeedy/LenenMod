using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using lenen.Content.Projectiles.BulletHellProjectiles;
using lenen.Common.Utils;

namespace lenen.Content.Projectiles
{
    public class SuperHaniwaHead : ModProjectile
    {
        public bool Desperate { get => Projectile.ai[0] != 0; set => Projectile.ai[0] = value ? 1 : 0; }
        int[] laserIndex = new int[] { -1, -1, -1 };
        public int attackCooldown = 90;
        public int miniCooldown = 120;
        public int despawnTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 294;
            Projectile.height = 164;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 0;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.hide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            attackCooldown = 90;
            miniCooldown = 120;

            int fistType = ModContent.ProjectileType<SuperHaniwaFist>();
            for (int i = -1; i < 2; i += 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                    Projectile.Center, Vector2.Zero, fistType, Projectile.damage,
                    6f, Projectile.owner, Projectile.ai[0], i, Projectile.whoAmI);
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public static bool OwnerActive(Player owner)
        {
            //Main.NewText("Owner check");
            return owner.active && owner.statLife > 0 && !owner.DeadOrGhost;
        }

        public static bool OwnerActive(int who)
        {
            //Main.NewText("Who: " + who);
            if (who == -1) return false;

            Player owner = Main.player[who];

            //Main.NewText("Check: " + (owner.active && owner.statLife > 0 && !owner.DeadOrGhost));
            return OwnerActive(owner);
        }

        public override void AI()
        {
            if (!OwnerActive(Projectile.owner) || Projectile.timeLeft <= 2)
            {
                //Main.NewText("Despawning");
                despawnTimer++;
                
                Projectile.timeLeft = 2;

                Projectile.velocity += new Vector2(0, 2);
                Projectile.velocity *= 0.92f;

                if (despawnTimer >= 300 || Projectile.Center.Y >= Main.maxTilesY * 16)
                {
                    Projectile.timeLeft = 0;
                }
                return;
            }
            Vector2 offset = new(0, -480);
            Player owner = Main.player[Projectile.owner];
            int laserType = ModContent.ProjectileType<BigLaser>();

            //Main.NewText("Cooldown: " + attackCooldown);
            Movement(owner);

            bool hasLaser = ProjectileCheck(laserType);

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 direction = Projectile.Center.DirectionTo(Main.MouseWorld);
                int colorIndex = 1;
                if (Desperate) colorIndex = 3;

                if (attackCooldown <= 0)
                {
                    //Main.NewText("Firing at will");
                    int color = BulletUtils.GetRandomColor(colorIndex);
                    int indexCount = 0;
                    int damage = (int)(Projectile.damage * 2f);
                    for (int i = -1; i < 2; i += 2)
                    {
                        laserIndex[indexCount] = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                            Projectile.Center, direction.RotatedBy(-MathHelper.PiOver2), laserType,
                            damage, 6f, owner.whoAmI, color, i, 1);
                        color = BulletUtils.GetRandomColor(colorIndex);

                        indexCount++;
                    }

                    if (Desperate) color = BulletUtils.GetRandomColor(colorIndex);

                    laserIndex[indexCount] = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        Projectile.Center, direction.RotatedBy(-MathHelper.PiOver2), laserType,
                        damage, 6f, owner.whoAmI, color, 0, 1);

                    attackCooldown = 900;
                }

                if (miniCooldown <= 0 && !hasLaser)
                {
                    Vector2 baseVel = new Vector2(0, 4).RotatedBy(-Projectile.timeLeft * 0.255f);
                    Vector2 baseVel2 = new Vector2(0, 7).RotatedBy(Projectile.timeLeft * 0.510f);
                    int damage = (int)(Projectile.damage * 0.25f);
                    int projectileType = ModContent.ProjectileType<SummonBasicBullet>();
                    for (int i = 0; i < 7; i++)
                    {
                        int color = BulletUtils.GetRandomColor(colorIndex);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                            baseVel.RotatedBy(i * MathHelper.TwoPi / 7f), projectileType, 
                            Projectile.damage, 3f, Projectile.owner, color, (int)Sheet.Big);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                            baseVel2.RotatedBy(i * MathHelper.TwoPi / 7f), projectileType,
                            Projectile.damage, 3f, Projectile.owner, color, (int)Sheet.Small);
                    }

                    miniCooldown = 20;
                }
            }

            attackCooldown--;
            miniCooldown--;
        }

        public bool ProjectileCheck(int type)
        {
            bool hasLasers = false;
            for (int i = 0; i < laserIndex.Length; i++)
            {
                int index = laserIndex[i];
                if (index != -1)
                {
                    Projectile proj = Main.projectile[index];
                    if (proj.type == type && proj.owner == Projectile.owner &&
                        proj.active)
                    {
                        proj.Center = Projectile.Center;
                        hasLasers = true;
                    }
                }
            }
            return hasLasers;
        }

        public void Movement(Player owner)
        {
            //Main.NewText($"Moving with speed {Projectile.velocity}");
            Vector2 target = owner.Center + new Vector2(0, -480);

            if (Projectile.Center != target)
            {
                float distance = Projectile.Distance(target);
                Vector2 direction = Projectile.DirectionTo(target) * 0.5f;

                if (distance < 50)
                {
                    Projectile.velocity += direction;
                } else
                {
                    Projectile.velocity += direction * (distance / 50f);
                }
            }
            Projectile.velocity *= 0.92f;
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return base.PreDraw(ref lightColor);
        }
    }
}
