using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class SwordShapedBottleOpener : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.shoot = ModContent.ProjectileType<Swing>();
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 6;

            Item.width = 64;
            Item.height = 66;
            Item.value = Item.sellPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.Blue;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true; 
            Item.shootsEveryUse = true;
            Item.useTurn = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item); 
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), 
                type, damage, knockback, player.whoAmI, player.direction * player.gravDir, 
                player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); 

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 20)
                .AddRecipeGroup("CrimtaneBar", 10)
                .AddIngredient(ItemID.ClayBlock, 5)
                .AddIngredient<BottleOpener>(1)
                .AddTile(TileID.Hellforge)
                .AddCondition(Condition.NearWater)
                .Register();
        }
    }
}
