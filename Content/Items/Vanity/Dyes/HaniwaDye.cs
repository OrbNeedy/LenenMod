using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Items.Vanity.Dyes
{
    public class HaniwaDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(
                    Item.type,
                    new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/HaniwaTexture"), "Texture").
                        UseImage(Mod.Assets.Request<Texture2D>("Assets/Textures/HaniwaTexture"))
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
