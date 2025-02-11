using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using lenen.Content.Items;
using lenen.Content.Items.Weapons;

namespace lenen.Common
{
    public class ChestGeneration : ModSystem
    {
        public override void PostWorldGen()
        {
            int[] woodChests = { ModContent.ItemType<BottleOpener>(), ModContent.ItemType<RustedKnife>() };
            int[] lumenItems = { ModContent.ItemType<LumenDiscFragment>() };
            //int[] goldChestItems;
            int[] lockedGoldChestItems = { ModContent.ItemType<BottleOpener>() };
            int[] shadowChestItems = { ModContent.ItemType<BottleOpener>() };
            int[] spiderChestItems = { ModContent.ItemType<RustedKnife>() };
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                int chestItemsChoice = 0;
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers)
                {
                    switch (Main.tile[chest.x, chest.y].TileFrameX)
                    {
                        // Wood chest
                        case 0:
                            PutInChest(chest, ref chestItemsChoice, lumenItems, !Main.rand.NextBool(40));
                            PutInChest(chest, ref chestItemsChoice, woodChests, !Main.rand.NextBool(12));
                            break;
                        // Gold chest
                        case 1 * 36:
                            PutInChest(chest, ref chestItemsChoice, lumenItems, !Main.rand.NextBool(40));
                            break;
                        // Locked gold chest
                        case 2 * 36:
                            PutInChest(chest, ref chestItemsChoice, lumenItems, !Main.rand.NextBool(3));
                            PutInChest(chest, ref chestItemsChoice, lockedGoldChestItems, !Main.rand.NextBool(6));
                            break;
                        // Shadow chest
                        case 3 * 36:
                            PutInChest(chest, ref chestItemsChoice, lumenItems, !Main.rand.NextBool(40));
                            break;
                        // Locked Shadow chest
                        case 4 * 36:
                            PutInChest(chest, ref chestItemsChoice, shadowChestItems, !Main.rand.NextBool(2));
                            break;
                        // Vine chest
                        case 12 * 36:
                            PutInChest(chest, ref chestItemsChoice, lumenItems, !Main.rand.NextBool(40));
                            PutInChest(chest, ref chestItemsChoice, spiderChestItems, !Main.rand.NextBool(12));
                            break;
                        // Spider chest
                        case 16 * 36:
                            PutInChest(chest, ref chestItemsChoice, spiderChestItems, !Main.rand.NextBool(12));
                            break;
                        // Ocean chest
                        case 18 * 36:
                            PutInChest(chest, ref chestItemsChoice, spiderChestItems, !Main.rand.NextBool(6));
                            break;
                    }
                }
            }
        }

        private void PutInChest(Chest chest, ref int chestItemsChoice, int[] itemPool, bool skip)
        {
            if (skip) return;
            for (int inventoryIndex = 0; inventoryIndex < chest.item.Length; inventoryIndex++)
            {
                if (chest.item[inventoryIndex].type == ItemID.None)
                {
                    chest.item[inventoryIndex].SetDefaults(itemPool[chestItemsChoice]);
                    chestItemsChoice = (chestItemsChoice + 1) % itemPool.Length;
                    break;
                }
            }
        }
    }
}
