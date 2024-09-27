using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items
{
    public class BrokenBirdDrone : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.value = Item.sellPrice(0, 1, 20, 50);
            Item.rare = ItemRarityID.White;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}
