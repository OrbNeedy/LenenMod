using lenen.Common.Players;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class Tasouken : ModItem
    {
        private int spellCardTimer = 720;
        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.shoot = ModContent.ProjectileType<Swing>();
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 10;
            Item.ArmorPenetration = 10;

            Item.width = 64;
            Item.height = 66;
            Item.value = Item.sellPrice(0, 1, 10, 50);
            Item.rare = ItemRarityID.Blue;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shootsEveryUse = true;
            Item.useTurn = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(spellCardTimer);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }

            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f),
                type, damage, knockback, player.whoAmI, player.direction * player.gravDir,
                player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);

            Projectile.NewProjectile(source, Main.MouseWorld + new Vector2(0, 0), Vector2.Zero,
                ModContent.ProjectileType<Cut>(), damage, knockback, player.whoAmI);

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool AltFunctionUse(Player player)
        {
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            if (manager.spellCardTimer <= 0)
            {
                SpellCard(player);
                return true;
            }
            return false;
        }

        private void SpellCard(Player player)
        {
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            manager.spellCardTimer = spellCardTimer;

            // 20
            int dmg = (int)(player.GetWeaponDamage(Item)/3);
            float desperation = MathHelper.TwoPi / 210;
            if (manager.desperateBomb)
            {
                // 35
                dmg = (int)(player.GetWeaponDamage(Item) * 0.58333f);
                desperation = MathHelper.TwoPi / 70;
                manager.spellCardTimer = spellCardTimer + 300;
            }

            float scale = 7;
            int direction = player.Center.X <= Main.MouseWorld.X ? 1 : -1;
            Vector2 vel = new Vector2(-1, 0);
            Vector2 offset = new Vector2(0, -100 * scale);//new Vector2(-117 * 5 * direction, 70 * 5);

            Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center + offset, vel,
                ModContent.ProjectileType<InfiniteLaser>(), dmg, Item.knockBack, player.whoAmI, 8,
                desperation * direction, scale);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.SoulofFlight, 10)
                .AddIngredient<SwordShapedBottleOpener>(1)
                .AddTile(TileID.DemonAltar)
                .AddCondition(Condition.DownedMechBossAny)
                .Register();
        }
    }
}
