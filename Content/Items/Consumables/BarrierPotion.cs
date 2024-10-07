using lenen.Content.Buffs;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Consumables
{
    public class BarrierPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(240, 240, 240),
                new Color(200, 200, 200),
                new Color(140, 140, 140)
            };
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(silver: 80);
            Item.buffType = ModContent.BuffType<BarrierBuff2>();
            Item.buffTime = 18000;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MinorBarrierPotion>(1)
                .AddIngredient(ItemID.PixieDust, 10)
                .AddRecipeGroup("EvilBiomeHardmodeMaterial", 10)
                .AddTile(TileID.AlchemyTable)
                .Register();
        }
    }
}
