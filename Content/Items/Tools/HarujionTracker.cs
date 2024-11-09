using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Tools
{
    public class HarujionTracker : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.value = Item.sellPrice(silver: 10, copper: 50);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<TrackingPlayer>().trackingHarujion = true;
            base.HoldItem(player);
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> glowmask = ModContent.Request<Texture2D>("lenen/Content/Items/Tools/HarujionTracker_Glowmask");
            spriteBatch.Draw(
                glowmask.Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - glowmask.Value.Height * 0.5f
                ),
                new Rectangle(0, 0, glowmask.Value.Width, glowmask.Value.Height),
                Color.White,
                rotation,
                glowmask.Value.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddRecipeGroup(RecipeGroupID.IronBar, 20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
