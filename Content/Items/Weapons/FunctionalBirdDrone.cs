using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class FunctionalBirdDrone : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 4;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<WeakBirdProjectile>();

            Item.width = 22;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.LightRed;

            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 10)
                .AddRecipeGroup("MythrilBar", 10)
                .AddIngredient(ItemID.Wire, 20)
                .AddIngredient<BrokenBirdDrone>(1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
