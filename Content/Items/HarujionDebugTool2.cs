using lenen.Common.Systems;
using lenen.Content.Projectiles;
using lenen.Content.Tiles.Plants;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items
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

        public override string Texture => "lenen/Content/Items/LumenDiscFragment_3";
    }
}
