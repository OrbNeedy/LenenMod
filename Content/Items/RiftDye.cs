using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items
{
    public class RiftDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(
                    Item.type,
                    new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/RiftEffect"), "RiftShader").UseColor(0f, 1f, 0f) // Be sure to update the effect path and pass name here.
                );
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;

            Item.CloneDefaults(ItemID.AcidDye);

            Item.dye = dye;
        }
    }
}
