using lenen.Common.Players;
using lenen.Common.Systems;
using lenen.Content.Projectiles;
using lenen.Content.Tiles.Plants;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace lenen.Content.Items.Weapons
{
    public class ExtendedGrab : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.shoot = ModContent.ProjectileType<HaniwaFist>();
            Item.shootSpeed = 8.5f;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 3.5f;

            Item.width = 54;
            Item.height = 54;
            Item.value = Item.sellPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.LightPurple;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.staff[Type] = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shootsEveryUse = true;
            Item.useTurn = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            OptionsManagingPlayer fistManagerPlayer = player.GetModPlayer<OptionsManagingPlayer>();

            fistManagerPlayer.ShootFists(player.altFunctionUse);

            /*Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            WorldGen.PlaceTile(pos.X, pos.Y, 
                ModContent.TileType<HarujionSapling>(), false);
            bool success = Main.tile[pos.X, pos.Y].TileType == ModContent.TileType<HarujionSapling>();
            ModContent.GetInstance<HarujionLocations>().UpdateHarujion();
            Main.NewText("Harujion's final location: " + ModContent.GetInstance<HarujionLocations>().harujionLocation);
            Main.NewText("Current player location: " + player.Center.ToTileCoordinates16());
            Main.NewText("Was the placing successful? " + success);*/
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.RichMahogany, 15)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddRecipeGroup("MythrilBar", 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
