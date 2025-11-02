using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class Emitter
    {
        public Sheet sprite = Sheet.Default;
        public SheetFrame color = SheetFrame.White;
        public BulletBehavior behavior = BulletBehavior.Default;
        public float minSpeed = 10;
        public float maxSpeed = 10;
        public int amount = 1;
        public int layers = 1;
        public Vector2 position = Vector2.Zero;
        public float aimAngle = 0; 
        public float bulletAngle = 0;
        public int owner = -1;
        public int timer = 0;
        public int maxTimer = 10;
        public int initialTimer = 0;

        public void ResetStats()
        {
            sprite = Sheet.Default;
            color = SheetFrame.White;
            behavior = BulletBehavior.Default;
            minSpeed = 10;
            maxSpeed = 10;
            amount = 1;
            layers = 1;
            position = Vector2.Zero;
            aimAngle = 0;
            bulletAngle = 0;
            owner = -1;
            timer = 0;
            maxTimer = 10;
            initialTimer = 0;
        }

        public void Update(IEntitySource source, int damage, float knockback)
        {
            if (timer < maxTimer) return;

            float difference = maxSpeed - minSpeed;
            float angle = aimAngle;
            float separation = (amount - 1) / 2f;
            for (int i = 0; i < layers; i++)
            {
                float layerSpeed = minSpeed + (i * difference);
                for (int k = 0; k < amount; k++)
                {
                    angle = aimAngle + (bulletAngle * (-separation + k));
                    Projectile proj = Projectile.NewProjectileDirect(source, position, new Vector2(layerSpeed, 0).RotatedBy(angle), 
                        ModContent.ProjectileType<BasicBullet>(), damage, knockback, owner, (int)color, 
                        (int)sprite, (int)behavior);

                    if (proj.ModProjectile is BasicBullet bullet)
                    {
                        bullet.timer = initialTimer;
                        proj.netUpdate = true;
                    }
                }
            }
        }

        public void FinalUpdate()
        {
            if (timer++ >= maxTimer)
            {
                timer = 0;
            }
        }
    }
}
