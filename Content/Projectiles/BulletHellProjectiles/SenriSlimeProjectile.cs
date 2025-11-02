using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class SenriSlimeProjectile : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Behavior { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        string route = "lenen/Content/Projectiles/BulletHellProjectiles/";

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
            Projectile.tileCollide = true;
            Projectile.timeLeft = 210;
        }

        public override string Texture => route + "BasicBullet";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server) return;

            if (Projectile.velocity.Length() > 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(0, 18), 0.01f);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer != Projectile.owner || Behavior == -1) return;

            Vector2 direction = new Vector2(18, 0);

            for (int i = 0; i < 12; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                    direction.RotatedByRandom(MathHelper.TwoPi), Projectile.type, (int)(Projectile.damage * 0.5f), 1, 
                    Projectile.owner, BulletColor, (int)Sheet.Ofuda, -1);
            }
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
