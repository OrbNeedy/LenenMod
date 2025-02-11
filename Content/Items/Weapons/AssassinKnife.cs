using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class AssassinKnife : ModItem
    {
        private int knifeCooldown = 0;
        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.shoot = ModContent.ProjectileType<ConcealedKnife>();
            Item.shootSpeed = 8f;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Ranged;

            Item.width = 30;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 0, 45);
            Item.rare = ItemRarityID.Green;

            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            knifeCooldown -= knifeCooldown > 0 ? 1 : 0;
        }

        public override void UpdateInventory(Player player)
        {
            knifeCooldown -= knifeCooldown > 0 ? 1 : 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                knifeCooldown = 300;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 vel = velocity
                        .RotatedBy((MathHelper.PiOver4 * i * 0.5f) - MathHelper.PiOver2 + (MathHelper.PiOver4));
                    Projectile.NewProjectile(source, position, vel, type,
                    damage, knockback, player.whoAmI);
                }
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool AltFunctionUse(Player player)
        {
            return knifeCooldown <= 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 5)
                .AddIngredient(ItemID.PurificationPowder, 5)
                .AddIngredient(ItemID.BottledWater, 1)
                .AddIngredient<RustedKnife>(1)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddRecipeGroup("GoldBar", 15)
                .AddIngredient(ItemID.Obsidian, 20)
                .AddIngredient(ItemID.MeteoriteBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
