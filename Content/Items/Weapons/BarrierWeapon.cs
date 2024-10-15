using lenen.Content.Projectiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using lenen.Common.Players;
using Terraria.Localization;

namespace lenen.Content.Items.Weapons
{
    public class BarrierWeapon : ModItem
    {
        private int spellCardTimer = 900;
        private int spellCardCost = 150;

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 2;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<OffensiveBarrier>();
            Item.ArmorPenetration = 40;
            Item.mana = 5;

            Item.width = 30;
            Item.height = 30;
            Item.value = 6000;
            Item.rare = ItemRarityID.Red;

            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(spellCardCost, spellCardTimer);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }
            for (int i = -1; i < 2; i++)
            {
                Vector2 otherVelocity = velocity.RotatedBy((MathHelper.Pi + MathHelper.Pi/5) * i);
                Projectile.NewProjectile(source, position, otherVelocity,
                    type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            if (player.CheckMana(spellCardCost, false, true) && manager.spellCardTimer <= 0)
            {
                SpellCard(player);
                return true;
            }
            return false;
        }

        private void SpellCard(Player player)
        {
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            player.CheckMana(spellCardCost, true, true);
            player.manaRegenDelay = player.manaRegenCount;
            manager.spellCardTimer = spellCardTimer;

            int dmg = (int)(player.GetTotalDamage(Item.DamageType).ApplyTo(120));
            if (manager.desperateBomb)
            {
                dmg = (int)(player.GetTotalDamage(Item.DamageType).ApplyTo(200));
                manager.spellCardTimer = spellCardTimer + 1200;

                Vector2 baseOffset = new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-250, 250));
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center + baseOffset, Vector2.Zero,
                    ModContent.ProjectileType<BlackGridedSquare>(), dmg, Item.knockBack, player.whoAmI, 1, 0, 20);
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center + baseOffset, Vector2.Zero,
                    ModContent.ProjectileType<BlackGridedSquare>(), dmg, Item.knockBack, player.whoAmI, 1, 1, 20);
                return;
            }

            for (int i = 1; i < 21; i++)
            {
                Vector2 baseOffset = new Vector2(-880 + (80 * i), -500);
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center + baseOffset, Vector2.Zero,
                    ModContent.ProjectileType<BlackGridedSquare>(), dmg, Item.knockBack, player.whoAmI, 0, 2 * i, 0);
                Vector2 baseOffset2 = new Vector2(-880 + (80 * i), 500);
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center + baseOffset2, Vector2.Zero, 
                    ModContent.ProjectileType<BlackGridedSquare>(), dmg, Item.knockBack, player.whoAmI, 0, 2 * i, 1);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.SpellTome)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
