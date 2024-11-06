using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class Cannon : ModProjectile
    {
        Asset<Texture2D> body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/Cannon");
        public float CannonType { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        
        int shootTimer = 0;
        int barrageTimer = 0;
        int barrageLeft = 6;

        int baseDamage = 0;

        Vector2 targetPosition = Vector2.Zero;
        bool hasTarget = false;
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
            // Make it change types with the environment
        }

        public override void AI()
        {
            Projectile frame = Main.projectile[(int)Projectile.ai[0]];
            Player owner = Main.player[Projectile.owner];

            if (frame != null)
            {
                if (frame.active && frame.ModProjectile is HaniwaCannon)
                {
                    Projectile.timeLeft = 2;
                    Projectile.damage = frame.damage;
                    Positioning(frame);
                    CheckTarget(owner);
                    Shooting();
                    Projectile.rotation = Projectile.Center.DirectionTo(targetPosition).ToRotation() + 
                        MathHelper.PiOver2;
                }
            }

            if (Projectile.ai[2] == 0)
            {
                return;
            }
            if (owner.ZoneSnow)
            {
                body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/IceCannon");
                return;
            }
            else if (owner.ZoneRockLayerHeight)
            {
                body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/StoneCannon");
                return;
            }
            else
            {
                body = ModContent.Request<Texture2D>("lenen/Content/Projectiles/Cannon");
                return;
            }
        }

        private void Positioning(Projectile reference)
        {
            Vector2 offset = new Vector2(0, reference.height/2);
            switch (CannonType)
            {
                case -2:
                    offset.X = reference.width / 2;
                    break;
                case -1:
                    offset.X = -reference.width / 2;
                    break;
                case 1:
                    offset.X = reference.width / 2;
                    break;
                case 2:
                    offset.X = -reference.width / 2;
                    break;
                default:
                    offset.X = 0;
                    break;
            }
            offset.X *= 0.9f;
            Projectile.Center = reference.Center + offset;
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

                targetPosition = Main.MouseWorld;
                hasTarget = false;
                NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                if (target != null)
                {
                    if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height,
                        target.position, target.width, target.height))
                    {
                        targetPosition = target.Center;
                        hasTarget = true;
                    }
                }
            }
        }

        private void Shooting()
        {
            switch (CannonType)
            {
                case -2:
                case 1: // Semicircle
                    if (hasTarget)
                    {
                        if (shootTimer <= 0)
                        {
                            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
                            for (int i = 0; i < 6; i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                                    direction.RotatedBy((i * MathHelper.Pi/6) - MathHelper.PiOver2) * 13, 
                                    ModContent.ProjectileType<Bullet>(), Projectile.damage, Projectile.knockBack, 
                                    Projectile.owner, ai2: 0.45f);
                            }
                            shootTimer = 60;
                        }
                    }
                    break;
                case -1:
                case 2: // Barrage
                    if (hasTarget)
                    {
                        if (shootTimer <= 0)
                        {
                            barrageLeft = 6;
                            shootTimer = 115;
                        }
                        if (barrageLeft > 0 && barrageTimer <= 0)
                        {
                            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                direction * 13, ModContent.ProjectileType<Bullet>(), Projectile.damage,
                                Projectile.knockBack, Projectile.owner, ai1: 1, ai2: 0.65f);
                            barrageLeft--;
                            barrageTimer = 4;
                        }
                    }
                    barrageTimer--;
                    break;
                default: // Laser
                    if (hasTarget)
                    {
                        if (shootTimer <= 0)
                        {
                            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                direction * 13, ModContent.ProjectileType<Laser>(), Projectile.damage,
                                Projectile.knockBack, Projectile.owner);
                            shootTimer = 125;
                        }
                    }
                    break;
            }
            shootTimer--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.PlayerRenderer.DrawPlayer(Main.Camera, Main.player[Projectile.owner], 
            //    Projectile.Center + new Vector2(0, 200), 0, Vector2.Zero);

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
