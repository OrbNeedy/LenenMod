using lenen.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Buffs
{
    public class HaniwaCommander : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int cloneType = ModContent.ProjectileType<HaniwaClone>();
            int haniwaType = ModContent.ProjectileType<HaniwaCannon>();
            if (player.ownedProjectileCounts[cloneType] > 0 || 
                player.ownedProjectileCounts[haniwaType] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
