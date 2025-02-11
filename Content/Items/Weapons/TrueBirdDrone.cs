using lenen.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using lenen.Common.Players;
using Terraria.Localization;

namespace lenen.Content.Items.Weapons
{
    public class TrueBirdDrone : ModItem
    {
        private int spellCardTimer = 400;
        private int ai2 = 0;

        public override void SetDefaults()
        {
			Item.CloneDefaults(ItemID.LastPrism);
            Item.mana = 0;
            Item.damage = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 4;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<KuroLaserHoldout>();

            Item.width = 22;
            Item.height = 24;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.LightRed;

            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(spellCardTimer);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }

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

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<KuroLaserHoldout>()] <= 0;
        }

        private void SpellCard(Player player)
        {
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            manager.spellCardTimer = spellCardTimer;

            // 35
            int dmg = (int)(player.GetWeaponDamage(Item) * 0.5f);

            if (!manager.desperateBomb)
            {
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item, "Spellcard"), player.Center,
                    Vector2.Zero, ModContent.ProjectileType<Retrovirus>(), dmg, Item.knockBack, player.whoAmI);
            } else
            {
                manager.spellCardTimer += 300;
                // 210
                dmg = (int)(player.GetWeaponDamage(Item) * 3f);
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item, "Spellcard"), player.Center,
                    player.Center.DirectionTo(Main.MouseWorld)*12, ModContent.ProjectileType<SmallBullet>(), dmg, 
                    Item.knockBack, player.whoAmI, 1, ai2: ai2);
                if (ai2 == 0) ai2 = 1;
                else ai2 = 0;
            }
        }
    }
}
