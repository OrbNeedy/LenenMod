using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class BoneColumn : ModProjectile
    {
        SpriteEffects flipped = SpriteEffects.None;
        Vector2 initialPosition = Vector2.Zero;

        int boneLength { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        float growthSpeed { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        bool reversing = false;

        List<int> boneOrder = new List<int>();

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ArmorPenetration = 35;

            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 3;

            Main.projFrames[Projectile.type] = 13;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 1000;
            Projectile.hide = true;
            Projectile.timeLeft = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/hone_shot"), Projectile.Center);

            if (growthSpeed <= 0)
            {
                growthSpeed = 1;
            }

            if (Main.rand.NextBool())
            {
                flipped = SpriteEffects.FlipHorizontally;
            }
            Vector2 normalizedVelocity = Vector2.Normalize(Projectile.velocity);

            initialPosition = Projectile.Center - (normalizedVelocity * 10);

            Projectile.Center += normalizedVelocity * growthSpeed;
        }

        public override void AI()
        {
            //Main.NewText("Bone distance:" + Projectile.Center.Distance(initialPosition));
            if (Projectile.Center.Distance(initialPosition) <= growthSpeed && reversing)
            {
                //Main.NewText("Bone distance too low, killing");
                Projectile.Kill();
                return;
            }

            if (Projectile.Center.Distance(initialPosition) >= (boneLength*16))
            {
                reversing = true;
            }

            if (!reversing)
            {
                Projectile.Center += Vector2.Normalize(Projectile.velocity) * growthSpeed;
            } else
            {
                Projectile.Center += Vector2.Normalize(-Projectile.velocity) * growthSpeed;
            }

            Projectile.timeLeft = 2;

            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Unused
            float collisionPoint = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), 
                Projectile.Center, initialPosition, 16, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bone = ModContent.Request<Texture2D>("lenen/Content/Projectiles/BoneColumn").Value;

            //int div = (int)(boneLength / (0.001f + Projectile.Center.Distance(initialPosition)));
            //Main.NewText("Division: " + div);
            float distanceToCenter = Projectile.Center.Distance(initialPosition) + 8;

            Vector2 separation = Vector2.Normalize(Projectile.velocity) * 16;

            for (int i = 0; i < boneLength; i++)
            {
                if (16 * i > distanceToCenter) break;

                Main.EntitySpriteDraw(
                    bone, 
                    Projectile.Center - Main.screenPosition - (separation * i), 
                    GetBoneSegment(i, 1), 
                    lightColor, 
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, 
                    new Vector2(10, 10), 
                    Projectile.scale, 
                    flipped
                );
            }
            return false;
        }

        private Rectangle GetBoneSegment(int num, float fraction)
        {
            Rectangle returnValue = new Rectangle(0, 20 * num, 20, 20);

            if (num >= 4)
            {
                if (boneOrder.Count <= num - 4)
                {
                    boneOrder.Add(Main.rand.Next(5, 13));
                }
                returnValue.Y = 20 * boneOrder[num - 4];
            }

            return returnValue;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }
}
