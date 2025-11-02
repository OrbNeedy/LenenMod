using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class DisruptionBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public static string route = "lenen/Content/Projectiles/BulletHellProjectiles/";
        string? sourceContext = null;

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
            Projectile.localNPCHitCooldown = 4;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;

            Projectile.Opacity = 0;
        }

        public override string Texture => BasicBullet.route + "BasicBullet";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);

            Projectile.Size = BasicBullet.GetBulletBounds(BulletSprite, BulletColor).Size() * 0.9f;
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

            if (Projectile.Opacity < 1)
            {
                Projectile.Opacity += 0.067f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            BulletSourceEffects.NPCHitEffect(sourceContext, target, ref modifiers);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            BulletSourceEffects.PlayerHitEffect(sourceContext, target, ref modifiers);
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
            Rectangle bounds = BasicBullet.GetBulletBounds((Sheet)BulletSprite, (SheetFrame)BulletColor);

            Main.EntitySpriteDraw(new DrawData(
                BasicBullet.GetBulletTexture((Sheet)BulletSprite),
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
    }
}
