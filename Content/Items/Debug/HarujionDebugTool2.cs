using lenen.Content.Tiles.Plants;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items.Debug
{
    class HarujionDebugTool2 : ModItem
    {
        enum ToolMode
        {
            Create, 
            Grow, 
            Destroy, 
            ForceUpdate 
        }

        private ToolMode mode = ToolMode.Create;
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HarujionMultitile>());
        }

        public override string Texture => "lenen/Content/Items/Materials/LumenDiscFragment_3";
    }
}
