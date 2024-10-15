using lenen.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class DeathSpawnPlayer : ModPlayer
    {
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Main.rand.NextBool(40))
            {
                int stack = Main.rand.Next(1, 3);
                Main.LocalPlayer.QuickSpawnItem(Player.GetSource_GiftOrReward(),
                ModContent.ItemType<NoiseDye>(), stack);
            }
        }
    }
}
