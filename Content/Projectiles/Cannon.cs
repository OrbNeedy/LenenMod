using lenen.Common.Players;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class Cannon : ModProjectile
    {
        Asset<Texture2D> body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/Cannon");
        public int HeadIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int ParentAnchor { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int CannonType { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        HaniwaMaterial material = HaniwaMaterial.Clay;

        int shootTimer = 0;
        int barrageTimer = 0;
        int barrageLeft = 6;

        bool hasTarget = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 34;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
        }

        public override void OnSpawn(IEntitySource source)
        {
            switch (source.Context)
            {
                case "IceHaniwa":
                    body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/IceCannon");
                    material = HaniwaMaterial.Ice;
                    break;
                case "StoneHaniwa":
                    body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/StoneCannon");
                    material = HaniwaMaterial.Stone;
                    break;
            }
        }

        public bool CheckParentProjectile(int index)
        {
            if (index == -1) return false;

            Projectile head = Main.projectile[index];

            return head.active && head.owner == Projectile.owner && 
                head.type == ModContent.ProjectileType<HaniwaCannon>();
        }

        public override void AI()
        {
            if (!CheckParentProjectile(HeadIndex))
            {
                Projectile.timeLeft = 0;
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile head = Main.projectile[HeadIndex];

            Vector2 offset = new Vector2(head.width * 0.4f * ParentAnchor, head.height / 2f);
            Projectile.Center = head.Center + offset;

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 targetPos = Vector2.Zero;
                SetTarget(out targetPos, Main.LocalPlayer);

                Projectile.velocity = Projectile.Center.DirectionTo(targetPos);

                if (shootTimer <= 0 && hasTarget)
                {
                    string sourceString = "ClayHaniwa";
                    switch (material)
                    {
                        case HaniwaMaterial.Ice:
                            sourceString = "IceHaniwa";
                            break;
                        case HaniwaMaterial.Stone:
                            sourceString = "StoneHaniwa";
                            break;
                    }

                    int addedDamage = (int)(Projectile.minionSlots * 10);
                    if (head.ai[0] != 0) addedDamage = (int)(Projectile.minionSlots * 45);
                    int finalDamage = Projectile.damage + addedDamage;

                    switch (CannonType)
                    {
                        case 1:
                            ShootFan(Projectile.velocity, sourceString, finalDamage);
                            break;
                        case 2:
                            ShootBarrage(Projectile.velocity, sourceString, finalDamage);
                            break;
                        default:
                            ShootLaser(Projectile.velocity, sourceString, finalDamage);
                            break;
                    }
                }
                shootTimer--;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void SetTarget(out Vector2 targetPos, Player owner)
        {
            targetPos = Main.MouseWorld;
            hasTarget = false;
            if (Projectile.owner == Main.myPlayer)
            {
                targetPos = Main.MouseWorld;

                if (owner.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                    float between = Vector2.Distance(npc.Center, Projectile.Center);

                    if (between < 4000f)
                    {
                        targetPos = npc.Center + (npc.velocity * 18);
                        hasTarget = true;
                        return;
                    }
                }

                targetPos = Main.MouseWorld;
                hasTarget = false;
                NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                if (target != null)
                {
                    if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height,
                        target.position, target.width, target.height))
                    {
                        targetPos = target.Center + (target.velocity * 18);
                        hasTarget = true;
                    }
                }
            }
        }

        private void ShootLaser(Vector2 direction, string sourceString, int damage)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(sourceString), Projectile.Center,
                direction * 13, ModContent.ProjectileType<SummonLaser>(), damage,
                Projectile.knockBack, Projectile.owner);
            shootTimer = 125;
        }

        private void ShootFan(Vector2 direction, string sourceString, int damage)
        {
            for (int i = 0; i < 6; i++)
            {
                // All colored
                Projectile.NewProjectile(Projectile.GetSource_FromThis(sourceString), Projectile.Center,
                    direction.RotatedBy((i * MathHelper.Pi / 6f) - MathHelper.PiOver2) * 13,
                    ModContent.ProjectileType<SummonBasicBullet>(), damage,
                    Projectile.knockBack, Projectile.owner, (int)SheetFrame.White, (int)Sheet.Small);
            }
            shootTimer = 60;
        }

        private void ShootBarrage(Vector2 direction, string sourceString, int damage)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(sourceString), Projectile.Center,
                direction * 13, ModContent.ProjectileType<SummonBasicBullet>(), damage,
                Projectile.knockBack, Projectile.owner, (int)SheetFrame.Yellow, (int)Sheet.Small);
            barrageLeft--;

            if (barrageLeft <= 0)
            {
                barrageLeft = 6;
                shootTimer = 115;
            } else
            {
                shootTimer = 5;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float alpha = 1;

            Main.EntitySpriteDraw(body.Value,
                Projectile.Center - Main.screenPosition,
                body.Value.Bounds,
                Color.White * alpha,
                Projectile.rotation,
                body.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );

            return false;
        }
    }
}
