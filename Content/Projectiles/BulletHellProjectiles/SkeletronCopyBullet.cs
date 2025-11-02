using lenen.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class SkeletronCopyBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int Target { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Behavior { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        int sprite = 0;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0.375f;

            Projectile.DamageType = ModContent.GetInstance<UniversalHybrid>();
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 100;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 210;
        }

        public override string Texture => BasicBullet.route + "BasicBullet";

        public override void OnSpawn(IEntitySource source)
        {
            switch (Behavior)
            {
                // Bombs
                case 0:
                    sprite = (int)Sheet.Big;
                    Projectile.penetrate = 1;
                    Projectile.damage = (int)(Projectile.damage * 0.1f);
                    break;
                // Laser
                case 1:
                    sprite = (int)Sheet.Double;
                    break;
                // Saw
                case 2:
                    sprite = (int)Sheet.Ofuda;
                    Projectile.damage = (int)(Projectile.damage * 0.75f);
                    break;
                // Vice
                case 3:
                    sprite = (int)Sheet.Bullet;
                    Projectile.damage *= 2;
                    Projectile.knockBack = 3;
                    break;
            }
            Projectile.Size = BasicBullet.GetBulletBounds(sprite, BulletColor).Size() * 0.9f;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server) return;

            if (Projectile.velocity.Length() > 0 && Behavior != 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            bool localIsOwner = Main.myPlayer == Projectile.owner;

            if (Projectile.timeLeft > 180)
            {
                Projectile.Opacity = 0;
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame);
                }

                if (localIsOwner)
                {
                    NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                    Vector2 offset = new Vector2(180, 0).RotatedBy(-MathHelper.PiOver4 + (MathHelper.PiOver2 * Behavior));
                    float distance = 0;

                    if (target != null)
                    {
                        distance = Projectile.Center.Distance(target.Center + offset);
                        Projectile.velocity = Projectile.Center.DirectionTo(target.Center + offset) * 
                            (12 + (distance / 50f));
                        if (distance <= 12)
                        {
                            Projectile.velocity = Vector2.Zero;
                        }
                    } else
                    {
                        distance = Projectile.Center.Distance(Main.MouseWorld + offset);
                        Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld + offset) *
                            (12 + (distance / 50f));
                        if (distance <= 12)
                        {
                            Projectile.velocity = Vector2.Zero;
                        }
                    }
                }
            } else
            {
                Projectile.Opacity = 1;
            }

            if (Projectile.timeLeft == 180)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            }

            switch (Behavior)
            {
                // Bombs
                case 0:
                    if (Projectile.timeLeft == 180)
                    {
                        NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                        Vector2 targetPosition = Main.MouseWorld;

                        if (target != null)
                        {
                            targetPosition = target.Center;
                        }

                        float distanceX = Projectile.Center.Distance(targetPosition);
                        distanceX = (Projectile.Center.DirectionTo(targetPosition) * distanceX).X;

                        Projectile.velocity = new Vector2(0, -16)
                            .RotatedBy(MathHelper.Clamp(MathHelper.PiOver2 * distanceX * 0.0006f, 
                            -MathHelper.PiOver2, MathHelper.PiOver2));
                    }

                    Projectile.tileCollide = Projectile.timeLeft <= 180;
                    if (Projectile.timeLeft <= 180)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(0, 18), 0.01f);
                    }
                    break;
                // Laser
                case 1:
                    if (Projectile.timeLeft == 180)
                    {
                        NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                        Vector2 targetPosition = Main.MouseWorld;

                        if (target != null)
                        {
                            targetPosition = target.Center;
                        }

                        Projectile.velocity = Projectile.Center.DirectionTo(targetPosition) * 24;
                    }
                    break;
                // Saw
                case 2:
                    if (Projectile.timeLeft <= 180)
                    {
                        NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                        Vector2 targetPosition = Main.MouseWorld;

                        if (target != null)
                        {
                            targetPosition = target.Center;
                        }

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity,
                            Projectile.Center.DirectionTo(targetPosition) * 16, 0.02f);
                    }
                    break;
                // Vice
                case 3:
                    if (Projectile.timeLeft <= 180)
                    {
                        Projectile.velocity *= 0.975f;
                        if (Projectile.timeLeft % 60 == 0)
                        {
                            NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                            Vector2 targetPosition = Main.MouseWorld;

                            if (target != null)
                            {
                                targetPosition = target.Center;
                            }

                            Projectile.velocity = Projectile.Center.DirectionTo(targetPosition) * 24;
                        }
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Behavior != 0) return;

            bool crit = Projectile.CritChance >= Main._rand.NextFloat(0, 1);

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.friendly && npc.immortal && Projectile.Distance(npc.Center) < 120)
                {
                    //Main.NewText("Damage: " + Projectile.damage);
                    npc.SimpleStrikeNPC(Projectile.damage * 15, 
                        (int)MathHelper.Clamp(Projectile.Center.DirectionTo(npc.Center).X, -1, 1), crit, 
                        4.5f, Projectile.DamageType, true, 0, true);
                }
            }

            for (int i = 0; i < 120; i++)
            {
                Vector2 offset = new Vector2(Main._rand.Next(0, 120), 0).RotatedByRandom(MathHelper.TwoPi);
                Dust.NewDustPerfect(Projectile.Center + offset, DustID.Shadowflame);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft > 180) return false;

            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.timeLeft > 180) return false;

            return base.CanHitPlayer(target);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CheckAABBvAABBCollision(projHitbox.TopLeft(), projHitbox.Size(),
                targetHitbox.TopLeft(), targetHitbox.Size()))
            {
                return Projectile.Center.Distance(targetHitbox.Center.ToVector2()) <=
                    (Projectile.width / 2) + (targetHitbox.Size().X / 2);
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle bounds = BasicBullet.GetBulletBounds((Sheet)sprite, (SheetFrame)BulletColor);

            Main.EntitySpriteDraw(new DrawData(
                BasicBullet.GetBulletTexture((Sheet)sprite),
                Projectile.Center - Main.screenPosition,
                bounds,
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                bounds.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None)
            );
            return false;
        }
    }
}
