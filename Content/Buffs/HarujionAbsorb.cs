using lenen.Common.GlobalNPCs;
using lenen.Common.Players;
using lenen.Common.Systems;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Buffs
{
    public class HarujionAbsorb : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            HarujionLocations locations = ModContent.GetInstance<HarujionLocations>();
            float radius = locations.GetRadius();
            float distance = npc.DistanceSQ(locations.harujionLocation.ToWorldCoordinates());
            float multiplier = locations.GetGrowth();
            float potency = 1 + (((radius*multiplier) - distance + 0.001f) / radius);

            npc.GetGlobalNPC<SoulDrops>().harujionPotency = potency;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            HarujionLocations locations = ModContent.GetInstance<HarujionLocations>();
            //Main.NewText($"Location instance souls: {locations.soulsAbsorbed}");
            float multiplier = locations.GetGrowth();
            float radius = locations.GetRadius();
            float distance = player.DistanceSQ(locations.harujionLocation.ToWorldCoordinates());
            float potency = 1 + (((radius * multiplier) - distance + 0.001f) / radius);
            //Main.NewText($"Final potency: {potency}");

            player.GetModPlayer<BuffPlayer>().harujionDebuff = potency;
        }
    }
}
