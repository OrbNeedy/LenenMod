using lenen.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class MeleeKnife : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Knife];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_shot_02"), Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(BulletColor, BulletSprite) with
                {
                    position = Projectile.Center - Main.screenPosition,
                    rotation = Projectile.rotation
                }
                );

            return false;
        }
    }
}
