using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.GlobalProjectiles
{
    public class ProjectileGrazeLimit : GlobalProjectile
    {
        public int grazeAvailable = 1;
        public int grazeCooldown = 0;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            grazeAvailable = 1+(projectile.damage/50);
        }

        public override void PostAI(Projectile projectile)
        {
            if (grazeCooldown > 0)
            {
                grazeCooldown--;
            }
        }
    }
}
