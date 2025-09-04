using lenen.Common.GlobalProjectiles;
using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class OffensiveBarrier : ModProjectile
    {
        float speed = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.alpha = 127;

            Projectile.damage = 20;
            Projectile.knockBack = 5;
            Projectile.penetrate = 1;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
            Projectile.scale = Main.rand.NextFloat(0.6f, 1.8f);
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f;
            if (Projectile.owner == Main.myPlayer)
            {
                NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                if (target != null)
                {
                    HomeIn(target);
                }
            }
        }

        private void HomeIn(NPC target)
        {
            Vector2 desiredDirection = Projectile.Center.DirectionTo(target.Center) * speed;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredDirection, 0.075f);
        }
    }
}
