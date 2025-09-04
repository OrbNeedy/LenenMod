using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class SlowedBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        static string route = "lenen/Content/Projectiles/BulletHellProjectiles/";
        string? sourceContext = null;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0.375f;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300; 
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            if (source.Context == null)
            {
                sourceContext = null;
            } else
            {
                sourceContext = source.Context;
            }

            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server) return;
            if (Projectile.velocity.Length() > 0.01f)
            {
                Projectile.velocity *= 0.985f;
            }
            if (Projectile.velocity.Length() > 0 && ((Sheet)BulletSprite == Sheet.Double || (Sheet)BulletSprite == Sheet.Pellet))
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            HitEffects.NPCHitEffect(sourceContext, target, ref modifiers);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            HitEffects.PlayerHitEffect(sourceContext, target, ref modifiers);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Sprite;
            Vector2 SpriteSize;
            switch ((Sheet)BulletSprite)
            {
                case Sheet.Big:
                    Sprite = ModContent.Request<Texture2D>(route + "BigBulletSprites").Value;
                    SpriteSize = new Vector2(68, 68);
                    break;
                case Sheet.Double:
                    Sprite = ModContent.Request<Texture2D>(route + "DoubleBulletSprites").Value;
                    SpriteSize = new Vector2(34, 36);
                    break;
                case Sheet.Pellet:
                    Sprite = ModContent.Request<Texture2D>(route + "PelletBulletSprites").Value;
                    SpriteSize = new Vector2(24, 40);
                    break;
                case Sheet.Small:
                    Sprite = ModContent.Request<Texture2D>(route + "SmallBulletSprites").Value;
                    SpriteSize = new Vector2(28, 28);
                    break;
                default:
                    Sprite = ModContent.Request<Texture2D>(route + "DefaultBulletSprites").Value;
                    SpriteSize = new Vector2(44, 44);
                    break;
            }

            Main.EntitySpriteDraw(new DrawData(
                Sprite,
                Projectile.Center - Main.screenPosition,
                new Rectangle((int)(SpriteSize.X * BulletColor), 0, (int)SpriteSize.X, (int)SpriteSize.Y),
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                SpriteSize * 0.5f,
                Projectile.scale,
                SpriteEffects.None)
            );
            return false;
        }
    }
}
