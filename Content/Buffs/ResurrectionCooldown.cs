using lenen.Common.Players;
using lenen.Common.Players.Barriers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Buffs
{
    public class ResurrectionCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override LocalizedText Description => base.Description.WithFormatArgs();

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Barrier barrier = Main.LocalPlayer.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.ResurrectionBarrier];

            tip = Language.GetTextValue("Mods.lenen.BarrierStats", barrier.MaxLife, barrier.MaxCooldown / 60,
                    barrier.MaxRecovery / 60, barrier.MaxFullRecovery / 60);
            base.ModifyBuffText(ref buffName, ref tip, ref rare);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            Barrier barrier = player.GetModPlayer<PlayerBarrier>().barriers[BarrierTypes.ResurrectionBarrier];
            barrier.Active = true;
            base.Update(player, ref buffIndex);
        }
    }
}
