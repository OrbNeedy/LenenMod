using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using lenen.Common.Players;
using System.Collections.Generic;
using Terraria.ID;

namespace lenen.Content.Projectiles
{
    class CthulhuBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletBehavior { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int DashCooldown { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public List<float[]> afterimageAlpha = new List<float[]>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

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
            Projectile.localNPCHitCooldown = 8;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
        }

        public override string Texture => "lenen/Content/Projectiles/BulletHellProjectiles/BigBulletSprites";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);

            if (BulletBehavior == 1)
            {
                Projectile.timeLeft = 120;
                Projectile.Opacity = 0;
                Projectile.localNPCHitCooldown = 14;
                //Main.NewText("Special spawn at " + Projectile.Center);
            }
        }

        public override void AI()
        {

            switch (BulletBehavior)
            {
                case 1:
                    Projectile.velocity *= 0.975f;

                    if (Projectile.timeLeft > 15)
                    {
                        if (Projectile.Opacity < 1)
                        {
                            Projectile.Opacity += 0.067f;
                        }
                        else
                        {
                            Projectile.Opacity = 1;
                        }
                    } else
                    {
                        if (Projectile.Opacity > 1)
                        {
                            Projectile.Opacity -= 0.067f;
                        }
                        else
                        {
                            Projectile.Opacity = 0;
                        }
                    }

                    if (DashCooldown >= 45 && DashCooldown < 75 && Main.myPlayer == Projectile.owner)
                    {
                        TargetPlayer targetPlayer = Main.LocalPlayer.GetModPlayer<TargetPlayer>();
                        Vector2 targetPosition = Main.MouseWorld;
                        if (targetPlayer.target != null)
                        {
                            targetPosition = targetPlayer.target.Center;
                        }

                        Projectile.velocity *= 1.3f;
                        BulletBehavior--;

                        Projectile.netUpdate = true;
                    }
                    break;
                default:
                    Projectile.velocity *= 0.95f;

                    if (DashCooldown % 30 == 0 && Main.myPlayer == Projectile.owner && 
                        Projectile.timeLeft < 150)
                    {
                        TargetPlayer targetPlayer = Main.LocalPlayer.GetModPlayer<TargetPlayer>();
                        Vector2 targetPosition = Main.MouseWorld;
                        if (targetPlayer.target != null)
                        {
                            targetPosition = targetPlayer.target.Center;
                        }

                        Projectile.velocity = Projectile.Center.DirectionTo(targetPosition) * 20;
                        BulletBehavior--;

                        Projectile.netUpdate = true;
                    }
                    break;
            }

            foreach (var afterimage in afterimageAlpha)
            {
                afterimage[2] -= 0.1f;
            }
            afterimageAlpha.RemoveAll((afterimage) => afterimage[2] <= 0);

            if (Projectile.timeLeft % 2 == 0)
            {
                afterimageAlpha.Add([Projectile.Center.X, Projectile.Center.Y, Projectile.Opacity, Projectile.rotation]);
            }

            DashCooldown++;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
        }

        public override bool PreDrawExtras()
        {
            Texture2D laser = ModContent.Request<Texture2D>(
                "lenen/Content/Projectiles/BulletHellProjectiles/BigBulletSprites").Value;
            Vector2 SpriteSize = new Vector2(68, 68);

            foreach (var afterimage in afterimageAlpha)
            {
                Vector2 position = new Vector2(afterimage[0], afterimage[1]);

                Rectangle sourceRectangle = new Rectangle((int)(SpriteSize.X * BulletColor), 0,
                    (int)SpriteSize.X, (int)SpriteSize.Y);

                Vector2 origin = sourceRectangle.Size() / 2f;

                Main.EntitySpriteDraw(
                    laser,
                    position - Main.screenPosition,
                    sourceRectangle,
                    Color.White * afterimage[2],
                    afterimage[3],
                    origin,
                    1f,
                    SpriteEffects.None
                );
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D laser = ModContent.Request<Texture2D>(
                "lenen/Content/Projectiles/BulletHellProjectiles/BigBulletSprites").Value;
            Vector2 SpriteSize = new Vector2(68, 68);
            Rectangle sourceRectangle = new Rectangle((int)(SpriteSize.X * BulletColor), 0, 
                (int)SpriteSize.X, (int)SpriteSize.Y);

            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(
                laser,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                origin,
                1f,
                SpriteEffects.None
            );
            return false;
        }
    }
}
