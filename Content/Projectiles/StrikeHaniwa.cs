using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class StrikeHaniwa : ModProjectile
    {
        Asset<Texture2D> body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 300;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/haniwa_00"), Projectile.Center);
            Player player = Main.player[Projectile.owner];
            if (player.ZoneSnow)
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/IceHaniwaFrame");
                return;
            }
            else if (player.ZoneRockLayerHeight)
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
                return;
            }
            else
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
                return;
            }
        }

        public override void AI()
        {
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.PlayerRenderer.DrawPlayer(Main.Camera, Main.player[Projectile.owner], 
            //    Projectile.Center + new Vector2(0, -200), 0, Vector2.Zero);

            Main.EntitySpriteDraw(body.Value,
                Projectile.Center - Main.screenPosition,
                body.Value.Bounds,
                Color.White,
                Projectile.rotation,
                body.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );
            return base.PreDraw(ref lightColor);
        }
    }
}
