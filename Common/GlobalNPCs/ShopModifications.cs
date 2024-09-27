using lenen.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.GlobalNPCs
{
    public class ShopModifications : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.SkeletonMerchant)
            {
                shop.Add<BrokenBirdDrone>();
            }
        }

        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            shop[nextSlot] = ModContent.ItemType<BrokenBirdDrone>();
        }
    }
}
