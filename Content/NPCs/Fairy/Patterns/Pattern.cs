using lenen.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.NPCs.Fairy.Patterns
{
    public class Pattern
    {
        // Returns the cooldown for the next attack
        // Size: Size of the fairy (Small, Shikigami, Big)
        // Type: The type of fairy (Mono, Slash, Shot, Magic)
        public virtual int Shoot(int size, int level, NPC npc, string type)
        {
            if (npc.target <= -1 || npc.target >= 500)
            {
                return 0;
            }
            Player target = Main.player[npc.target];
            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 
                npc.Center.DirectionTo(target.Center) * 12, ModContent.ProjectileType<EnemyBullet>(), 10, 2.5f);
            return 600 - (20*level*size);
        }
    }
}
