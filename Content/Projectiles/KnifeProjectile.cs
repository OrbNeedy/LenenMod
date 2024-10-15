using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace lenen.Content.Projectiles
{
    public class KnifeProjectile : ModProjectile
    {
        public List<float[]> afterimageAlpha = new List<float[]>();
        Predicate<float[]> deleteConditions;
        bool rewinding = false;
        bool makeAfterimages = true;
        float rotation = 0;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.damage = 20;
            Projectile.knockBack = 5;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 360;

            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<MeleeRangedHybrid>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = true;

            DrawOriginOffsetX = -4;
        }
             
        public override string Texture => "lenen/Assets/Textures/MemoryKnife_0";

        public override void OnSpawn(IEntitySource source)
        {
            deleteConditions = DeleteConditions;
            if (Projectile.ai[1] == 1)
            {
                Projectile.alpha = 35;
                Projectile.tileCollide = false;
            }
            if (Projectile.ai[0] == 2)
            {
                Projectile.localNPCHitCooldown = 3;
                Projectile.tileCollide = false;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[1] == 1)
            {
                if (Projectile.timeLeft <= 300) Projectile.alpha += 1;
                if (Projectile.alpha >= 255)
                {
                    Projectile.timeLeft = 0;
                }
                return;
            }

            switch (Projectile.ai[0])
            {
                case 0:
                    if (Projectile.timeLeft == 210)
                    {
                        rewinding = true;
                        Projectile.velocity *= -2;
                        rotation = Projectile.rotation;
                    }
                    if (Projectile.timeLeft == 135)
                    {
                        rewinding = false;
                        Projectile.velocity *= -0.5f;
                        rotation = Projectile.rotation;
                    }
                    if (rewinding)
                    {
                        Projectile.alpha = 55;
                        Projectile.rotation = rotation;
                        Projectile.tileCollide = false;
                    } else
                    {
                        Projectile.alpha = 0;
                        Projectile.tileCollide = true;
                    }
                    break;
                case 1:
                    if (Projectile.timeLeft != 360 && Projectile.timeLeft % 30 == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                            Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Projectile.owner, 1, 1);
                    }
                    break;
                case 2:
                    foreach (var afterimage in afterimageAlpha)
                    {
                        afterimage[2] -= 0.04f;
                    }
                    afterimageAlpha.RemoveAll(deleteConditions);
                    if (!makeAfterimages && afterimageAlpha.Count <= 0) Projectile.timeLeft = 0;

                    if (Projectile.timeLeft % 7 == 0 && makeAfterimages)
                    {
                        afterimageAlpha.Add([Projectile.Center.X, Projectile.Center.Y, 1, Projectile.rotation]);
                    }
                    break;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[1] != 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
        }

        public override bool PreDrawExtras()
        {
            Texture2D knife = ModContent.Request<Texture2D>("lenen/Assets/Textures/MemoryKnife_0").Value;
            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1 || Projectile.ai[0] == 2)
            {
                knife = ModContent.Request<Texture2D>("lenen/Assets/Textures/MemoryKnife_" + Projectile.ai[0]).Value;
            }

            foreach (var afterimage in afterimageAlpha)
            {
                Vector2 position = new Vector2(afterimage[0], afterimage[1]);

                Rectangle sourceRectangle = new Rectangle(0, 0, knife.Width, knife.Height);

                Vector2 origin = sourceRectangle.Size() / 2f;

                float offsetY = 26f;
                origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

                Main.EntitySpriteDraw(
                    knife,
                    position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
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
            Texture2D knife = ModContent.Request<Texture2D>("lenen/Assets/Textures/MemoryKnife_0").Value;
            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1 || Projectile.ai[0] == 2)
            {
                knife = ModContent.Request<Texture2D>("lenen/Assets/Textures/MemoryKnife_" + Projectile.ai[0]).Value;
            }
            Rectangle sourceRectangle = new Rectangle(0, 0, knife.Width, knife.Height);

            Vector2 origin = sourceRectangle.Size() / 2f;

            float offsetY = 26f;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

            Color color = Projectile.GetAlpha(lightColor);
            if (Projectile.ai[0] == 2) color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(
                knife,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle,
                color,
                Projectile.rotation,
                origin,
                1f,
                SpriteEffects.None
            );
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            makeAfterimages = false;
            Projectile.velocity = Vector2.Zero;
            Projectile.alpha = 255;
            Projectile.damage = 0;
            return afterimageAlpha.Count <= 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public bool DeleteConditions(float[] afterimage)
        {
            return afterimage[2] <= 0;
        }
    }
}
