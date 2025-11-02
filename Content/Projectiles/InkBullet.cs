using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    class InkBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

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

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() > 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Main.myPlayer != Projectile.owner) return;

            if (Projectile.timeLeft%8 == 0)
            {
                Vector2 direction = new Vector2(4, 0);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                        direction.RotatedByRandom(MathHelper.TwoPi), 
                        ModContent.ProjectileType<BasicBullet>(), Projectile.damage, 1, Projectile.owner,
                        Projectile.ai[0], (int)Sheet.Double);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D laser = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = laser.Bounds;

            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(
                laser,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                Projectile.ai[0] == (int)SheetFrame.Black ? Color.Black :Color.White,
                Projectile.rotation,
                origin,
                1f,
                SpriteEffects.None
            );
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float width = 12f;

            Vector2 start = (Projectile.Center + new Vector2(86, 0)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 end = (Projectile.Center - new Vector2(86, 0)).RotatedBy(Projectile.rotation, Projectile.Center);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end,
                width * Projectile.scale, ref collisionPoint);
        }
    }
}
