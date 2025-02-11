using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace lenen.Content.Items
{
    public class LumenDiscFragment : ModItem
    {
        int fragmentFrame = 0;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;

            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.White;
        }

        public override void Load()
        {
            fragmentFrame = Main.rand.Next(0, 6);
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override string Texture => $"lenen/Content/Items/LumenDiscFragment_{fragmentFrame}";

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D fragment = 
                ModContent.Request<Texture2D>($"lenen/Content/Items/LumenDiscFragment_{fragmentFrame}").Value;

            spriteBatch.Draw(
                fragment,
                position,
                new Rectangle(0, 0, Item.width, Item.height),
                drawColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D fragment =
                ModContent.Request<Texture2D>($"lenen/Content/Items/LumenDiscFragment_{fragmentFrame}").Value;

            spriteBatch.Draw(
                fragment,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - fragment.Height * 0.5f
                ),
                new Rectangle(0, 0, Item.width, Item.height),
                Lighting.GetColor(Item.Center.ToTileCoordinates(), Color.White),
                rotation,
                fragment.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
        }
    }
}
