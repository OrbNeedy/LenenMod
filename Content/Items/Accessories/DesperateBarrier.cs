using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    public class DesperateBarrier : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override string Texture => "Terraria/Images/Item_" + ItemID.TargetDummy;
    }
}
