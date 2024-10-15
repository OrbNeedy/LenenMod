using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace lenen.Content.Projectiles
{
    public class ConcealedKnife : ModProjectile
    {
        float speed = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.damage = 20;
            Projectile.knockBack = 5;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 480;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
            if (Projectile.ai[0] > 0)
            {
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.scale = 2;
            }
            if (Projectile.ai[2] == 5)
            {
                Projectile.localNPCHitCooldown = 5;
                Projectile.scale = 2.5f;
            }
            Projectile.Center -= Projectile.Size * Projectile.scale * 0.5f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.timeLeft <= 420)
                {
                    Vector2 down = new Vector2(0, 16);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, down, 0.015f);
                }
            } else
            {
                if (Projectile.timeLeft > (390 - (Projectile.ai[0]*5)))
                {
                    float rate = Projectile.ai[1] * (Projectile.timeLeft - (390 - (Projectile.ai[0] * 5))) * 0.0025f;
                    Projectile.velocity = Projectile.velocity.RotatedBy(rate);
                }
                if (Projectile.timeLeft > 270 && Projectile.timeLeft%40 == 0)
                {
                    Vector2 vel = Projectile.velocity
                        .RotatedBy(MathHelper.PiOver2);
                    vel.Normalize();
                    for (int i = 0; i < 6; i++)
                    {
                        vel = vel.RotatedByRandom(MathHelper.PiOver4);
                        if (i%2 == 0)
                        {
                            vel = vel.RotatedBy(MathHelper.Pi);
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, vel*6, 
                            ModContent.ProjectileType<Pellet>(), Projectile.damage, 4f, Projectile.owner);
                    }
                }
            }
        }
    }
}
