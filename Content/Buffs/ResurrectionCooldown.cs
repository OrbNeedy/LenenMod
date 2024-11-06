using lenen.Common;
using lenen.Common.Players.Barriers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Buffs
{
    public class ResurrectionCooldown : ModBuff
    {
        private Barrier barrier = BarrierLookups.BarrierDictionary[BarrierLookups.Barriers.HarujionBarrier];
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override LocalizedText Description => base.Description.WithFormatArgs(
            barrier.MaxLife, barrier.MaxCooldown, barrier.MaxRecovery);

        public override void Update(Player player, ref int buffIndex)
        {
            barrier.State = 1;
            base.Update(player, ref buffIndex);
        }
    }
}
