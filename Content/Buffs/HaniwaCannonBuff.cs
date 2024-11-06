using lenen.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Buffs
{
    public class HaniwaCannonBuff : ModBuff
    {
		public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<HaniwaCannon>()] > 0)
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
