using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class SpiritFlame : ModProjectile
    {
        int spawnInvincibility = 15;

        //bool[] nearTiles = new bool[] { false, false, false, false, false, false, false, false };
        Dictionary<Vector2, bool> availableSpace = new Dictionary<Vector2, bool>();

        int flameFlickerTimer = 0;
        int flameSprite = 0;
        bool flipFlame = false;

        bool goingRight = false;
        int directionChangeChoiceTime = 0;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.scale = 1f;
            Projectile.light = 0.5f;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1800;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.velocity.X > 0) goingRight = true;
            directionChangeChoiceTime = Main.rand.Next(30, 61);
            flameSprite = Main.rand.Next(0, 3);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(goingRight);
            writer.Write7BitEncodedInt(directionChangeChoiceTime);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            goingRight = reader.ReadBoolean();
            directionChangeChoiceTime = reader.Read7BitEncodedInt();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            if (Main.dedServ) return;

            if (Main.rand.NextBool(5))
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(Projectile.position + new Vector2(-4, -10), Projectile.width + 2, 
                        Projectile.height + 5, DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y, 
                        165);
                }
            }

            if (Projectile.wet)
            {
                Projectile.timeLeft = 0;
                Projectile.Kill();
                Projectile.netUpdate = true;
            }

            Projectile.velocity.Y = float.Lerp(Projectile.velocity.Y, 2, 0.02f);
            if (goingRight)
            {
                Projectile.velocity.X = float.Lerp(Projectile.velocity.X, 4, 0.02f);
            } else
            {
                Projectile.velocity.X = float.Lerp(Projectile.velocity.X, -4, 0.02f);
            }

            if (directionChangeChoiceTime <= 0)
            {
                goingRight = Main.rand.NextBool();
                directionChangeChoiceTime = Main.rand.Next(60, 151);
                Projectile.netUpdate = true;
            } else
            {
                directionChangeChoiceTime--;
            }

            spawnInvincibility--;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.dedServ) return true;

            if (Projectile.owner != Main.myPlayer) return true;

            Vector2 projectileVelocity = new Vector2(-1, 0);
            int[] directionValues = { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < directionValues.Length; i++)
            {
                Vector2 direction = new Vector2(0, -1).RotatedBy(-MathHelper.PiOver4*i);
                for (int k = -2; k < 3; k++)
                {
                    Vector2 tempDirection = direction.RotatedBy(MathHelper.PiOver4*k);
                    if (!Collision.SolidCollision(Projectile.Center + (tempDirection * 16), 1, 
                        1))
                    {
                        directionValues[i]++;
                    } else
                    {
                    }
                }
            }

            int highestValue = 0;
            int index = 0;
            for (int i = 0; i < directionValues.Length; i++)
            {
                if (directionValues[i] > highestValue)
                {
                    highestValue = directionValues[i];
                    index = i;
                }
            }

            projectileVelocity = new Vector2(0, -1).RotatedBy(-MathHelper.PiOver4 * index);

            if (Projectile.ai[0] == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projectileVelocity,
                    ModContent.ProjectileType<ThinLaser>(), Projectile.damage, 16, Projectile.owner);
            } else
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                    projectileVelocity.RotatedByRandom(0.15 * highestValue),
                    ModContent.ProjectileType<BoneColumn>(), (int)(Projectile.damage*0.375f), 16, Projectile.owner,
                    Main.rand.Next(30, 85), Main.rand.Next(12, 29));
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (spawnInvincibility > 0)
            {
                return false;
            }
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 240);
        }

        public override bool CanHitPlayer(Player target)
        {
            if (spawnInvincibility > 0)
            {
                return false;
            }
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.ShadowFlame, 240);
        }

        public override bool PreDrawExtras()
        {
            Texture2D flame = ModContent.Request<Texture2D>("lenen/Content/Projectiles/SpiritFlame_Flame").Value;

            SpriteEffects flip = flipFlame? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (flameFlickerTimer <= 0)
            {
                flameSprite++;
                if (flameSprite >= 3) flameSprite = 0;
                flipFlame = Main.rand.NextBool();
                flameFlickerTimer = 10;
            } else
            {
                flameFlickerTimer--;
            }

            Main.EntitySpriteDraw(
                flame,
                Projectile.Center - Main.screenPosition + new Vector2(0, -10),
                new Rectangle(42 * flameSprite, 0, 42, 66),
                Color.White,
                0f,
                new Vector2(42, 66) / 2,
                Projectile.scale,
                flip
            );
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D body = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(
                body, 
                Projectile.Center - Main.screenPosition, 
                body.Bounds, 
                Color.White, 
                0f, 
                body.Size()/2f, 
                Projectile.scale, 
                SpriteEffects.None
            );
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }
}
