using lenen.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.CameraModifiers;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class BigLaser : ModProjectile
    {
        public static Asset<Texture2D> LightBrightness;
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float Direction { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int BurstType { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        
        bool moved = false;
        int timer = 0;

        public int initialTimer = 0;
        public int burstTimer = 0;
        public static float minWidth = 0.06f;
        public static int standardBurstTime = 60;
        public static int standardInitialTime = 45;

        public override void Load()
        {
            if (Main.dedServ) return;

            LightBrightness = ModContent.Request<Texture2D>("lenen/Content/Projectiles/BulletHellProjectiles/LaserGlow");
        }

        public override void Unload()
        {
            LightBrightness = null;
        }

        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2200;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = ModContent.GetInstance<UniversalHybrid>();
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = standardInitialTime + standardBurstTime + 90;
        }

        public override string Texture => BulletUtils.laserTexturePath;

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/charge_1") with
            {
                Volume = 0.5f
            }, Projectile.Center);
            Projectile.scale = minWidth;
            //Projectile.timeLeft += (burstTimer + 60) * BurstType;
            //burstTimer = 0 - initialTimer - 30;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.scale < 1) return false;

            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.scale < 1) return false;

            return base.CanHitPlayer(target);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = float.Clamp(Projectile.scale + (1 - minWidth), 0, 1);

            //Main.NewText("Timer: " + timer);
            if (!moved)
            {
                //Main.NewText("Moving the thing");
                Projectile.velocity = Projectile.velocity.RotatedBy(0.006f * Direction);
                if (timer >= standardInitialTime)
                {
                    moved = true;
                    timer = 0;
                    return;
                }
            }
            else
            {
                if (timer < standardBurstTime || BurstType <= 0)
                {
                    if (Projectile.scale > minWidth || BurstType <= 0)
                    {
                        Projectile.scale -= 0.04f;
                    }
                }
                else
                {
                    if (timer == standardBurstTime)
                    {
                        Projectile.scale = 1f;
                        SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/burst_00") with
                        {
                            Volume = 0.5f
                        }, Projectile.Center);

                        PunchCameraModifier modifier = new PunchCameraModifier(
                            Projectile.Center, new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi), 
                            30f, 20f, 45, 1000f);
                        Main.instance.CameraModifiers.Add(modifier);
                    }

                    Projectile.scale += 0.08f;

                    if (timer >= standardBurstTime + 30)
                    {
                        timer = 0;
                        BurstType--;
                        Projectile.timeLeft += 30;
                        if (BurstType > 0)
                        {
                            Projectile.timeLeft += standardBurstTime;
                        }
                    }
                }
            }

            timer++;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 end = new Vector2(0, 2080).RotatedBy(Projectile.rotation);
            float w = 200 * Projectile.scale;

            return BulletUtils.LineCollision(targetHitbox, Projectile, Vector2.Zero, end, w, out _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetLaserTexture(BulletColor) with { 
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation, 
                    scale = new Vector2(Projectile.scale, 2), 
                    color = Color.White * Projectile.Opacity
                }
                );

            Main.EntitySpriteDraw(
                LightBrightness.Value, 
                Projectile.Center - Main.screenPosition,
                LightBrightness.Frame(), 
                Color.White * float.Clamp(Projectile.scale - 0.5f, 0, 1) * Projectile.Opacity, 
                Projectile.rotation, 
                new(LightBrightness.Width() / 2f, 140),
                new Vector2(Projectile.scale, 2), 
                SpriteEffects.None
                );

            return false;
        }
    }
}
