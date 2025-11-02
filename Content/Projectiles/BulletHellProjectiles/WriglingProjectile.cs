using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class WriglingProjectile : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int NextBullet { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Behavior { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        Vector2 initialPosition = Vector2.Zero;
        Vector2 baseDirection = Vector2.Zero;
        float speed = 0;
        float sineTimer = 0;

        string route = "lenen/Content/Projectiles/BulletHellProjectiles/";

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
            Projectile.localNPCHitCooldown = 16;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
        }

        public override string Texture => route + "BasicBullet";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            
            //Main.NewText("Behavior: " + Behavior);
            initialPosition = Projectile.Center;
            speed = Projectile.velocity.Length();
            baseDirection = Vector2.Normalize(Projectile.velocity);
            Projectile.timeLeft -= Behavior;
        }

        public override void AI()
        {
            if (Main.myPlayer != Projectile.owner) return;

            if (Projectile.velocity.Length() > 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            Projectile next;
            switch (Behavior)
            {
                case -1:
                    break;
                default:
                    //Main.NewText("Behavior: " + Behavior);
                    //Main.NewText("Time left: " + (Projectile.timeLeft));
                    //Main.NewText("Target time: " + (230 - (Behavior * 2)));
                    if (Behavior < 10 && Projectile.timeLeft == 230 - (Behavior))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), initialPosition,
                            baseDirection * speed, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Projectile.owner, BulletColor, Projectile.whoAmI, Behavior + 1);
                    }
                    break;
            }

            TargetPlayer targetPlayer = Main.LocalPlayer.GetModPlayer<TargetPlayer>();
            Vector2 targetPosition = Main.MouseWorld;
            switch (NextBullet)
            {
                case -2:
                    if (targetPlayer.target != null) targetPosition = targetPlayer.target.Center;

                    baseDirection = Vector2.Lerp(baseDirection,
                        Projectile.Center.DirectionTo(targetPosition), 0.05f);
                    break;
                case -1:
                    if (Main._rand.NextBool(25))
                    {
                        if (targetPlayer.target != null) targetPosition = targetPlayer.target.Center;

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                            Projectile.Center, Projectile.Center.DirectionTo(targetPosition) * 28, 
                            ModContent.ProjectileType<BasicBullet>(), (int)(Projectile.damage), 
                            1, Projectile.owner, BulletColor, (int)Sheet.Default);
                    }
                    break;
                default:
                    next = Main.projectile[NextBullet];
                    //Main.NewText("Next: " + NextBullet);
                    float distance = Projectile.Center.Distance(next.Center);
                    if (next.active && next.owner == Projectile.owner &&
                        next.ModProjectile is WriglingProjectile projectile &&
                        next.ai[2] < Behavior)
                    {
                        Vector2 direction = Projectile.Center.DirectionTo(next.Center);
                        Projectile.velocity = direction;
                        if (distance >= 44)
                        {
                            Projectile.velocity = direction * next.velocity.Length();
                        }
                    }
                    else
                    {
                        //Main.NewText("Behavior fail");
                        NextBullet = -2;
                        Behavior = 0;
                    }
                    break;
            }

            float sineValue = (float)Math.Cos(sineTimer);
            if (NextBullet < 0) Projectile.velocity = baseDirection.RotatedBy(sineValue) * speed;
            sineTimer += 0.092f;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle bounds = BasicBullet.GetBulletBounds(Sheet.Ofuda, (SheetFrame)BulletColor);

            Main.EntitySpriteDraw(new DrawData(
                BasicBullet.GetBulletTexture(Sheet.Ofuda),
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
