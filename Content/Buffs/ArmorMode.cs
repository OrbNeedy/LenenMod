using lenen.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Buffs
{
    public class ArmorMode : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense += 100;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Add more visuals
            player.endurance += 0.5f;
            player.statDefense += 14;
            player.noKnockback = true;
        }
    }
}
