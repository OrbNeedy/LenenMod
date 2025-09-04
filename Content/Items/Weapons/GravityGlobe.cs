using lenen.Common.Players;
using lenen.Common.Systems;
using lenen.Content.Projectiles;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class GravityGlobe : ModItem
    {
        private int spellCardTimer = 1020;
        private int spellCardCost = 150;
        public override void SetDefaults()
        {
            Item.damage = 165;
            Item.shoot = ModContent.ProjectileType<BasicBullet>();
            Item.shootSpeed = 6f;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 11;

            Item.width = 30;
            Item.height = 38;
            Item.value = Item.sellPrice(0, 0, 50, 80);
            Item.rare = ItemRarityID.Yellow;

            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(spellCardCost, spellCardTimer);

        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) return false;
            for (int i = 0; i < 16; i++)
            {
                int spriteType = Main.rand.Next((int)Sheet.Default, (int)Sheet.Pellet + 1);
                int color = Main.rand.Next((int)SheetFrame.White, (int)SheetFrame.Blue + 1);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.TwoPi * i / 16), type, damage, knockback,
                    player.whoAmI, color, spriteType);
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

            // 90
            int desperation = 1;
            if (manager.desperateBomb)
            {
                // 157
                desperation = 2;
                manager.spellCardTimer = spellCardTimer + 540;
            }

            Vector2 vel = player.Center.DirectionTo(Main.MouseWorld);

            player.GetModPlayer<GravityPlayer>().SpawnExplosion(source: 
                new EntitySource_ItemUse(player, Item, "Spellcard"));

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item, "Spellcard"), player.Center, vel,
                ModContent.ProjectileType<SuperNovaBullet>(), 0, 0, player.whoAmI, desperation, 1);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 0.2f * Main.essScale);
        }
    }
}
