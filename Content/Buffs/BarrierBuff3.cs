using lenen.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Buffs
{
    public class BarrierBuff3 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override string Texture => "lenen/Content/Buffs/BarrierBuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense += 30;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().barrierBuff = 3;
            if (player.HasBuff(ModContent.BuffType<BarrierBuff>()))
            {
                player.ClearBuff(ModContent.BuffType<BarrierBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<BarrierBuff2>()))
            {
                player.ClearBuff(ModContent.BuffType<BarrierBuff2>());
            }
        }
    }
}
