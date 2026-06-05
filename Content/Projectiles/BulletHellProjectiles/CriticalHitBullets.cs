using lenen.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class CriticalHitBullets : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int HitNPC { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0.375f;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;
            Projectile.ArmorPenetration = 20;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Bullet];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.CritChance = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (BulletUtils.DisableInitialHit(HitNPC, target.whoAmI)) return false;

            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(BulletColor, BulletSprite) with { 
                    position = Projectile.Center - Main.screenPosition, 
                    rotation = Projectile.rotation
                }
                );

            return false;
        }
    }
}
