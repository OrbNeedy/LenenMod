using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public enum Sheet
    {
        Default, 
        Big, 
        Double, 
        Pellet, 
        Small, 
        Ofuda, 
        Bullet, 
        Knife, 
        ReverseDefault, 
        ReverseBig
    }

    public enum SheetFrame
    {
        White, 
        Red, 
        Pink, 
        Blue, 
        Cyan, 
        Green, 
        Yellow, 
        Black
    }

    public enum BulletBehavior
    {
        Default, 
        Penetrate, 
        NoGroundPenetrate, 
        Homing, 
        Bounce, 
        Serpentine,
        NegativeSerpentine
    }

    public class BasicBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Behavior { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        public static string route = "lenen/Content/Projectiles/BulletHellProjectiles/";
        public static string emptyRoute = "lenen/Assets/Textures/Empty";
        public static SheetFrame[] _AvailableColors = { 
            SheetFrame.White, SheetFrame.Red, SheetFrame.Pink, SheetFrame.Blue, SheetFrame.Cyan, 
            SheetFrame.Green, SheetFrame.Yellow, SheetFrame.Black
        };
        string? sourceContext = null;
        Vector2 initialVelocity = Vector2.Zero;
        Vector2 initialPosition = Vector2.Zero;
        public int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0.375f;

            Projectile.DamageType = ModContent.GetInstance<UniversalHybrid>();
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;
            Projectile.ArmorPenetration = 100;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.Read();
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);

            initialPosition = Projectile.Center;
            initialVelocity = Projectile.velocity;

            if (source.Context == null)
            {
                sourceContext = null;
            } else
            {
                sourceContext = source.Context;
            }

            switch (Behavior)
            {
                case (int)BulletBehavior.Homing:
                    ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
                    break;
                case (int)BulletBehavior.Penetrate:
                    Projectile.penetrate = -1;
                    break;
                case (int)BulletBehavior.NoGroundPenetrate:
                    Projectile.tileCollide = true;
                    break;
                case (int)BulletBehavior.Bounce:
                    Projectile.tileCollide = true;
                    Projectile.penetrate = -1;
                    break;
            }

            Projectile.Size = GetBulletBounds(BulletSprite, BulletColor).Size() * 0.9f;
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server) return;

            BulletSourceEffects.AI(sourceContext, Projectile);

            if (Projectile.velocity.Length() > 0 && ((Sheet)BulletSprite == Sheet.Double || 
                (Sheet)BulletSprite == Sheet.Pellet || (Sheet)BulletSprite == Sheet.Ofuda))
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            switch (Behavior)
            {
                case (int)BulletBehavior.Homing:
                    TargetPlayer targetPlayer = Main.LocalPlayer.GetModPlayer<TargetPlayer>();
                    if (targetPlayer.target != null)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity,
                            Projectile.Center.DirectionTo(targetPlayer.target.Center) * 16, 0.02f);
                        Projectile.netUpdate = true;
                    }
                    break;
                case (int)BulletBehavior.Serpentine:
                    float cosineValue = (float)Math.Cos(timer * 0.1f);
                    Projectile.velocity = Projectile.velocity.RotatedBy(cosineValue * 0.14f);
                    Projectile.netUpdate = true;
                    break;
                case (int)BulletBehavior.NegativeSerpentine:
                    cosineValue = (float)Math.Cos(timer * 0.1f);
                    Projectile.velocity = Projectile.velocity.RotatedBy(cosineValue * -0.14f);
                    Projectile.netUpdate = true;
                    break;
            }

            timer++;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            BulletSourceEffects.NPCHitEffect(sourceContext, target, ref modifiers);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            BulletSourceEffects.PlayerHitEffect(sourceContext, target, ref modifiers);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (Behavior)
            {
                case (int)BulletBehavior.Bounce:
                    Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                    //SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                    if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    {
                        Projectile.velocity.X = -oldVelocity.X;
                    }

                    if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    {
                        Projectile.velocity.Y = -oldVelocity.Y;
                    }
                    return false;
            }
            return base.OnTileCollide(oldVelocity);
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
            Rectangle bounds = GetBulletBounds((Sheet)BulletSprite, (SheetFrame)BulletColor);

            Main.EntitySpriteDraw(new DrawData(
                GetBulletTexture((Sheet)BulletSprite),
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

        public static Texture2D GetBulletTexture(Sheet BulletSprite)
        {
            string finalPath = "DefaultBulletSprites";
            switch (BulletSprite)
            {
                case Sheet.Big:
                    finalPath = "BigBulletSprites";
                    break;
                case Sheet.Double:
                    finalPath = "DoubleBulletSprites";
                    break;
                case Sheet.Pellet:
                    finalPath = "PelletBulletSprites";
                    break;
                case Sheet.Small:
                    finalPath = "SmallBulletSprites";
                    break;
                case Sheet.Ofuda:
                    finalPath = "OfudaBulletSprites";
                    break;
                case Sheet.Bullet:
                    finalPath = "BulletBulletSprites";
                    break;

            }
            return ModContent.Request<Texture2D>(route + finalPath).Value;
        }

        public static Rectangle GetBulletBounds(Sheet BulletSprite, SheetFrame BulletColor)
        {
            Vector2 SpriteSize;
            switch (BulletSprite)
            {
                case Sheet.Big:
                    SpriteSize = new Vector2(68, 68);
                    break;
                case Sheet.Double:
                    SpriteSize = new Vector2(34, 36);
                    break;
                case Sheet.Pellet:
                    SpriteSize = new Vector2(24, 40);
                    break;
                case Sheet.Small:
                    SpriteSize = new Vector2(28, 28);
                    break;
                case Sheet.Ofuda:
                    SpriteSize = new Vector2(42, 44);
                    break;
                case Sheet.Bullet:
                    SpriteSize = new Vector2(24, 36);
                    break;
                default:
                    SpriteSize = new Vector2(44, 44);
                    break;
            }
            return new Rectangle((int)(SpriteSize.X * (int)BulletColor), 0, (int)SpriteSize.X, (int)SpriteSize.Y);
        }

        public static Rectangle GetBulletBounds(int BulletSprite, int BulletColor)
        {
            Vector2 SpriteSize;
            switch ((Sheet)BulletSprite)
            {
                case Sheet.Big:
                    SpriteSize = new Vector2(68, 68);
                    break;
                case Sheet.Double:
                    SpriteSize = new Vector2(34, 36);
                    break;
                case Sheet.Pellet:
                    SpriteSize = new Vector2(24, 40);
                    break;
                case Sheet.Small:
                    SpriteSize = new Vector2(28, 28);
                    break;
                case Sheet.Ofuda:
                    SpriteSize = new Vector2(42, 44);
                    break;
                case Sheet.Bullet:
                    SpriteSize = new Vector2(24, 36);
                    break;
                default:
                    SpriteSize = new Vector2(44, 44);
                    break;
            }
            return new Rectangle((int)(SpriteSize.X * (int)BulletColor), 0, (int)SpriteSize.X, (int)SpriteSize.Y);
        }
    }

    public static class BulletSourceEffects
    {
        public static void AI(string? sourceContext, Projectile projectile)
        {
            if (sourceContext == null) return;
            switch (sourceContext)
            {
                case "IceHaniwa":
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Frost);
                    break;
                case "StoneHaniwa":
                    break;
                case "SenriSpazmatism":
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.CursedTorch);
                    break;
            }
        }

        public static void NPCHitEffect(string? sourceContext, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (sourceContext == null) return;
            switch (sourceContext)
            {
                case "IceHaniwa":
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case "StoneHaniwa":
                    modifiers.ArmorPenetration += 7;
                    modifiers.Knockback *= 2;
                    break;
                case "SenriSpazmatism":
                    target.AddBuff(BuffID.CursedInferno, 120);
                    break;
            }
        }

        public static void PlayerHitEffect(string? sourceContext, Player target, ref Player.HurtModifiers modifiers)
        {
            if (sourceContext == null) return;
            switch (sourceContext)
            {
                case "IceHaniwa":
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case "StoneHaniwa":
                    modifiers.ArmorPenetration += 7;
                    modifiers.Knockback *= 2;
                    break;
            }
        }
    }
}
