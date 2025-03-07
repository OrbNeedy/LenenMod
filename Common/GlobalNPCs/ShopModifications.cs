﻿using lenen.Content.Items;
using lenen.Content.Items.Consumables;
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
            nextSlot += 1;
            shop[nextSlot] = ModContent.ItemType<MinorBarrierPotion>();
            nextSlot += 1;
        }
    }
}
