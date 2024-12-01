using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class BlackGridedSquare : ModProjectile
    {
        bool growing = false;
        bool sound = false;
        int growth = 20;
        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
            Projectile.width = 80;
            Projectile.height = 80;

            Projectile.damage = 30;
            Projectile.knockBack = 12;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.light = 0.65f;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 50;

            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] != 0)
            {
                Projectile.scale = 0;
            }
            sound = Projectile.ai[1]%10 == 0 && Projectile.ai[0] == 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.timeLeft >= 450)
            {
                if (Projectile.ai[0] == 1)
                {
                    if (Projectile.ai[2] == 20)
                    {
                        player.itemTime = 2;
                        player.itemAnimation = 2;
                    }
                } else
                {
                    player.itemTime = 2;
                    player.itemAnimation = 2;
                }
            }

            if (Projectile.scale < 1)
            {
                Projectile.rotation += 2*MathHelper.TwoPi/60;
                Projectile.scale += 1f/60f;
            } else
            {
                Projectile.rotation = MathHelper.TwoPi;
                Projectile.scale = 1f;
            }

            switch (Projectile.ai[0])
            {
                //bom_flash_01
                case 0:
                    // ai1: Growth timer
                    // ai2: Direction of growth (Up or down)
                    if (Projectile.timeLeft == 60)
                    {
                        growing = true;
                    }

                    if (Projectile.timeLeft > 120 && Projectile.ai[1] >= 0)
                    {
                        Projectile.ai[1] -= 1;
                        if (Projectile.ai[1] == 0)
                        {
                            growing = true;
                        }
                    }

                    if (growing)
                    {
                        Projectile.height += growth;
                        if (Projectile.ai[2] == 1) Projectile.position.Y -= growth;
                        if (Projectile.height >= 580)
                        {
                            if (sound)
                            {
                                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bom_flash_01") with
                                {
                                    Volume = 1f
                                }, Projectile.Center);
                            }
                            growing = false;
                            Projectile.ai[1] = 60;
                        }
                    } else
                    {
                        if (Projectile.height > 80)
                        {
                            Projectile.height -= growth;
                            if (Projectile.ai[2] == 1) Projectile.position.Y += growth;
                        }
                    }
                    break;
                case 1:
                    // ai1: Orientation of growth (Vertical or Horizontal)
                    // ai2: Amount of spawns left
                    if (Projectile.timeLeft == 480)
                    {
                        growing = true;
                        SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/haniwa_00") with
                        {
                            Volume = 0.35f
                        }, Projectile.Center);
                    }

                    // Evaluate new Spawn
                    EvaluateAndSpawn(player);

                    if (growing)
                    {
                        // Vertical growing
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.height += growth;
                            Projectile.position.Y -= growth/2;
                            if (Projectile.height >= 1740)
                            {
                                growing = false;
                            }
                        } else // Horizontal growing
                        {
                            Projectile.width += growth;
                            Projectile.position.X -= growth/2;
                            if (Projectile.width >= 1740)
                            {
                                growing = false;
                            }
                        }
                    } else
                    {
                        // Vertical shrinking
                        if (Projectile.ai[1] == 0)
                        {
                            if (Projectile.height > 80)
                            {
                                Projectile.height -= growth;
                                Projectile.position.Y += growth/2;
                            } else
                            {
                                if (Projectile.timeLeft < 450)
                                {
                                    Projectile.timeLeft = 0;
                                }
                            }
                        } else // Horizontal shrinking
                        {
                            if (Projectile.width > 80)
                            {
                                Projectile.width -= growth;
                                Projectile.position.X += growth/2;
                            } else
                            {
                                if (Projectile.timeLeft < 450) Projectile.timeLeft = 0;
                            }
                        }
                    }
                    break;
            }
        }

        public void EvaluateAndSpawn(Player player)
        {
            if (Projectile.ai[1] == 0 && Projectile.ai[2] > 0)
            {
                if (Projectile.timeLeft == (400 + ((10 - Projectile.ai[2]) * 20)))
                {
                    Vector2 spawn = player.Center + new Vector2(Main.rand.Next(-400, 400),
                        Main.rand.Next(-250, 250));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn,
                        Vector2.Zero, Type, Projectile.damage, Projectile.knockBack,
                        Projectile.owner, 1, 0, Projectile.ai[2] - 1);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn,
                        Vector2.Zero, Type, Projectile.damage, Projectile.knockBack,
                        Projectile.owner, 1, 1, Projectile.ai[2] - 1);
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Asset<Texture2D> squareTexture = ModContent.Request<Texture2D>("lenen/Content/Projectiles/BlackGridedSquare");

            Main.EntitySpriteDraw(squareTexture.Value,
                Projectile.Center - Main.screenPosition,
                squareTexture.Value.Bounds,
                lightColor,
                Projectile.rotation,
                squareTexture.Size() * 0.5f,
                Projectile.scale * Projectile.Size/80,
                SpriteEffects.None
            );
            //return base.PreDraw(ref lightColor);
        }
    }
}
