using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;

namespace lenen.Content.Projectiles
{
    public class SmallBullet : ModProjectile
    {
        // ai0: The type
        // ai1: Position on which the flip is based on
        // ai2: Either the flip is vertical or horizontal
        private bool flipping = false;
        private float offset = 0f;
        private float flipOrigin { 
            get => Projectile.ai[1]; 
            set => Projectile.ai[1] = value; }
        private float flipNature
        {
            get => Projectile.ai[2];
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.damage = 20;
            Projectile.knockBack = 5;
            Projectile.penetrate = 2;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 390;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = false;
            Projectile.light = 0.8f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1)
            {
                Projectile.penetrate = -1;
                if (flipNature == 0)
                {
                    flipOrigin = Projectile.Center.X;
                }
                else
                {
                    flipOrigin = Projectile.Center.Y;
                }
            } else
            {
                Projectile.timeLeft = -(int)Projectile.ai[0];
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            }
            base.OnSpawn(source);
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1)
            {
                if (Projectile.timeLeft%6 == 0 && Projectile.timeLeft >= 210)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                            new Vector2(1, 0).RotatedByRandom(MathHelper.TwoPi)*2,
                            ModContent.ProjectileType<SmallBullet>(), Projectile.damage, 4f, Projectile.owner, 
                            -Projectile.timeLeft, flipOrigin, flipNature);
                    }
                }
                if (Projectile.timeLeft % 40 == 0 && Projectile.timeLeft >= 210)
                {
                    Projectile.velocity = Projectile.Center
                        .DirectionTo(Main.MouseWorld) * Projectile.velocity.Length();
                }
            } else
            {
                if (flipping)
                {
                    if (flipNature == 0)
                    {
                        if (Projectile.timeLeft > 120)
                        {
                            float rate = offset / 15;
                            Projectile.position.X += rate;
                        }
                        else
                        {
                            flipping = false;
                            Projectile.penetrate = 1;
                        }
                    }
                    else
                    {
                        if (Projectile.timeLeft > 120)
                        {
                            float rate = offset / 15;
                            Projectile.position.Y += rate;
                        }
                        else
                        {
                            flipping = false;
                            Projectile.penetrate = 1;
                        }
                    }
                }
                if (Projectile.timeLeft == 150)
                {
                    flipping = true;
                    Projectile.penetrate = -1;
                    if (flipNature == 0)
                    {
                        Projectile.velocity.X *= -1; 
                        offset = flipOrigin - Projectile.Center.X;
                    }
                    else
                    {
                        Projectile.velocity.Y *= -1;
                        offset = flipOrigin - Projectile.Center.Y;
                    }
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return !flipping;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1)
            {
                lightColor = Color.Black;
            }
            return base.PreDraw(ref lightColor);
        }

        public override bool PreDrawExtras()
        {
            if (Projectile.ai[0] == 1)
            {
                Asset<Texture2D> bulletRing = ModContent.Request<Texture2D>("lenen/Content/Projectiles/HollowBullet");
                float scale = (270 - Projectile.timeLeft)/120;

                Main.EntitySpriteDraw(bulletRing.Value,
                    Projectile.Center - Main.screenPosition,
                    bulletRing.Value.Bounds,
                    Color.Black,
                    -Projectile.rotation,
                    bulletRing.Size() * 0.5f,
                    Projectile.scale * scale,
                    SpriteEffects.None
                );
            }
            return false;
        }
    }
}
