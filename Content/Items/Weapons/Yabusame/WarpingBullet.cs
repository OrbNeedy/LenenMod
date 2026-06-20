using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.IO;

namespace lenen.Content.Items.Weapons.Yabusame
{
    public class WarpingBullet : ModProjectile
    {
        public int RotationDirection { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int MoveTimer { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int BulletColor { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        Vector2 ownerPos = Vector2.Zero;
        Vector2 offsetToOwner = Vector2.Zero;

        List<Vector2> CopiesPosition = new();

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 630;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Default];

        public override void OnSpawn(IEntitySource source)
        {
            ownerPos = Projectile.Center;

            for (int i = 0; i < 6; i++)
            {
                CopiesPosition.Add(offsetToOwner);
            }
            Projectile.timeLeft -= MoveTimer;

            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(offsetToOwner);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            offsetToOwner = reader.ReadVector2();
        }

        public override void AI()
        {
            if (BulletUtils.OwnerCheck(Projectile.owner))
            {
                ownerPos = Main.player[Projectile.owner].Center;
            }

            Vector2 lastPosition = offsetToOwner;
            for (int i = 0; i < CopiesPosition.Count; i++)
            {
                Vector2 oldPosition = CopiesPosition[i];
                CopiesPosition[i] = lastPosition;
                lastPosition = oldPosition;
            }

            if (MoveTimer >= 0)
            {
                offsetToOwner += Projectile.velocity;
                Projectile.velocity = Projectile.velocity.RotatedBy(0.01f * RotationDirection);
            } else
            {
                MoveTimer++;
            }

            bool localOwner = Main.myPlayer == Projectile.owner;

            if (localOwner)
            {
                float xBounds = Main.screenWidth / 2f;
                float yBounds = Main.screenHeight / 2f;

                if (offsetToOwner.X < -xBounds)
                {
                    offsetToOwner.X += xBounds * 2;
                    Projectile.netUpdate = true;
                } else if (offsetToOwner.X > xBounds)
                {
                    offsetToOwner.X -= xBounds * 2;
                    Projectile.netUpdate = true;
                }

                if (offsetToOwner.Y < -yBounds)
                {
                    offsetToOwner.Y += yBounds * 2;
                    Projectile.netUpdate = true;
                }
                else if (offsetToOwner.Y > yBounds)
                {
                    offsetToOwner.Y -= yBounds * 2;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.Center = ownerPos + offsetToOwner;

            /*for (int i = 0; i < CopiesPosition.Count; i++)
            {
                if (CopiesTime[i] < 0)
                {
                    CopiesTime[i]++;
                    continue;
                }

                CopiesPosition[i] += CopiesVelocity[i];

                if (localOwner)
                {
                    float xBounds = Main.screenWidth / 2f;
                    float yBounds = Main.screenHeight / 2f;
                    float newX = CopiesPosition[i].X;
                    float newY = CopiesPosition[i].Y;

                    if (newX < -xBounds)
                    {
                        newX += xBounds * 2;
                    }
                    else if (newX > xBounds)
                    {
                        newX -= xBounds * 2;
                    }

                    if (newY < -yBounds)
                    {
                        newY += yBounds * 2;
                    }
                    else if (newY > yBounds)
                    {
                        newY -= yBounds * 2;
                    }

                    CopiesPosition[i] = new Vector2(newX, newY);
                }

                CopiesVelocity[i] = CopiesVelocity[i].RotatedBy(0.0255f * RotationDirection);
            }*/
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            /*if (CopiesPosition.Count <= 0) base.Colliding(projHitbox, targetHitbox);

            Vector2 lastPos = Projectile.Center;
            int count = 0;
            for (int i = 0; i < CopiesPosition.Count; i++)
            {
                if (count < 5 && CopiesPosition.Count - i >= 5)
                {
                    count++;
                    continue;
                }
                count = 0;

                Vector2 currentPos = ownerPos + CopiesPosition[i];

                // Create a hitbox from the two points in the line before checking line collision
                Rectangle hitbox = BulletUtils.GetHitboxFromLine(currentPos, lastPos);

                if (Collision.CheckAABBvAABBCollision(hitbox.TopLeft(), hitbox.Size(), 
                    targetHitbox.TopLeft(), targetHitbox.Size()))
                {
                    // If hit with line collision, return true, otherwise continue
                    bool hit = Collision.CheckAABBvLineCollision(
                        targetHitbox.TopLeft(), targetHitbox.Size(), 
                        lastPos, currentPos
                        );

                    if (hit)
                    {
                        return true;
                    }
                }

                lastPos = currentPos;
            }*/

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < CopiesPosition.Count; i++)
            {
                Main.EntitySpriteDraw(
                    BulletUtils.GetTexture((int)SheetFrame.White, (int)Sheet.Default) with
                    {
                        position = ownerPos + CopiesPosition[i] - Main.screenPosition
                    }
                    );
            }

            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(BulletColor, (int)Sheet.Default) with
                {
                    position = Projectile.Center - Main.screenPosition
                }
                );

            return false;
        }
    }
}
