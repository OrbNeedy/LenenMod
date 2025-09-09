using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using lenen.Common.Players;

namespace lenen.Content.Items.Weapons
{
    public class BarrierSupport : ModItem
    {
        public override void SetDefaults()
        {
            Item.mana = 60;

            Item.width = 30;
            Item.height = 30;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Pink;

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int ai1 = (int)BarrierType.Deflection;
                int targetID = -1;
                if (player.altFunctionUse == 2)
                {
                    ai1 = (int)BarrierType.Redirection;
                    targetID = TargetPlayer.CalculateWeight(1000, player);
                }

                int exitId = Projectile.NewProjectile(player.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero,
                    ModContent.ProjectileType<TeleportBarrier>(), 0, 0, player.whoAmI,
                    1, ai1, targetID);
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<TeleportBarrier>(), 0, 0, player.whoAmI,
                    0, ai1, exitId);

                return true;
            }
            return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<TeleportBarrier>()] <= 0;
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<TeleportBarrier>()] <= 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.SpellTome)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
