using lenen.Common.GlobalNPCs;
using lenen.Common.Players;
using lenen.Common.Systems;
using Terraria;
using Terraria.ID;
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
            float radius = HarujionLocations.instance.GetRadius();
            float distance = npc.DistanceSQ(HarujionLocations.instance.harujionLocation.ToWorldCoordinates());
            float distancePotency = (radius - distance) / radius;
            float multiplier = HarujionLocations.instance.GetGrowth();
            float potency = (distancePotency * 0.65f) + (multiplier * 0.35f);

            npc.GetGlobalNPC<SoulDrops>().harujionPotency = potency;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            float multiplier = HarujionLocations.instance.GetGrowth();
            float radius = HarujionLocations.instance.GetRadius();
            float distance = player.DistanceSQ(HarujionLocations.instance.harujionLocation.ToWorldCoordinates());
            float distancePotency = (radius - distance)/radius;
            float potency = (distancePotency * 0.9f) + (multiplier* 0.4f);

            player.GetModPlayer<BuffPlayer>().harujionDebuff = potency;
        }
    }
}
