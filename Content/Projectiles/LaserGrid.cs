using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class LaserGrid : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.damage = 65;
            Projectile.knockBack = 2;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 480;
            Projectile.light = 0.6f;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = false;

            DrawOffsetX = -20;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
