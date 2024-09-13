using lenen.Content.Items;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Common.GlobalNPCs
{
    public class CurtainSpawn : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (npc.boss)
            {
                if (Main.rand.NextBool(6))
                {
                    Item.NewItem(npc.GetSource_FromThis(), npc.Center, ModContent.ItemType<DimensionalFracture>());
                }
            }
        }
    }
}