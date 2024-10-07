using lenen.Content.Items;
using lenen.Content.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.GlobalProjectiles
{
    public class ProjectileLoot : GlobalProjectile
    {
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            switch (projectile.type)
            {
                case ProjectileID.FallingStar:
                    if (Main.rand.NextBool(40) && Main.hardMode)
                    {
                        Item.NewItem(projectile.GetSource_DropAsItem(), projectile.getRect(),
                            ModContent.ItemType<DimensionalFragment>());
                    }
                    break;
            }
        }
    }
}
