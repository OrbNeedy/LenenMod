using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.GlobalProjectiles
{
    public class ProjectileClonePlayer : GlobalProjectile
    {
        public Player clonedPlayer;
        public bool usesClonedPlayer = false;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
        }
    }
}
