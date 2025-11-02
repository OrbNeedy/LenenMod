using lenen.Content.Buffs;
using lenen.Content.Projectiles;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using lenen.Common.Players;

namespace lenen.Content.Items.Weapons
{
    public class PowerRodFan : ModItem
    {
        public bool sentryMode = false;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = false;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = false;

            ProjectileID.Sets.MinionSacrificable[Item.shoot] = false;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.shoot = ModContent.ProjectileType<InkBirdMinion>();
            Item.buffType = ModContent.BuffType<InkBirdMinionBuff>();
            Item.shootSpeed = 12f;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 1f;
            Item.mana = 10;

            Item.width = 14;
            Item.height = 54;
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ItemRarityID.Red;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shootsEveryUse = true;
            Item.useTurn = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (sentryMode)
                {
                    SoundEngine.PlaySound(SoundID.Item44, player.Center);
                    Item.shoot = ModContent.ProjectileType<InkBirdMinion>();
                } else
                {
                    SoundEngine.PlaySound(SoundID.Item83, player.Center);
                    Item.shoot = ModContent.ProjectileType<InkCore>();
                }
                Item.sentry = !sentryMode;
                sentryMode = !sentryMode;
            } else
            {
                // Main.NewText("Sentry: " + sentryMode);
                //Main.NewText("Player: " + player.Center);
                //Main.NewText("Mouse: " + Main.MouseWorld);
                if (sentryMode)
                {
                    var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero,
                        type, damage, knockback, Main.myPlayer);
                    projectile.damage = Item.damage; 
                    player.UpdateMaxTurrets();
                } else
                {
                    player.AddBuff(Item.buffType, 2);
                    var projectile = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero,
                            type, damage, knockback, Main.myPlayer, Main._rand.Next(-1, 3));
                    projectile.damage = Item.damage;
                }
            }

            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<SenriPlayer>().canDropSenri = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddRecipeGroup(RecipeGroupID.Wood, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 5)
                .AddIngredient(ItemID.Ectoplasm, 15)
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddRecipeGroup(RecipeGroupID.Wood, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.LihzahrdPowerCell)
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddRecipeGroup(RecipeGroupID.Wood, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
