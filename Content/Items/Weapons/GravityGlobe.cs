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
    public class GravityGlobe : ModItem
    {
        private int spellCardTimer = 1020;
        private int spellCardCost = 150;
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.shoot = ModContent.ProjectileType<FriendlyBullet>();
            Item.shootSpeed = 6f;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;

            Item.width = 30;
            Item.height = 38;
            Item.value = Item.sellPrice(0, 0, 50, 80);
            Item.rare = ItemRarityID.Yellow;

            Item.useTime = 45;
            Item.useAnimation = 45;
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
            for (int i = 0; i < 25; i++)
            {
                int spriteType = Main.rand.Next((int)BulletSprites.Simple, (int)BulletSprites.Pellet + 1);
                int color = Main.rand.Next((int)BulletColors.White, (int)BulletColors.DarkBlue + 1);
                Projectile.NewProjectile(Item.GetSource_FromThis(), position,
                    velocity.RotatedBy(MathHelper.TwoPi * i / 25), type, damage, knockback, player.whoAmI,
                    (int)BulletAIs.Simple, color, spriteType);
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
            int desperation = (int)BulletAIs.SuperNova;
            if (manager.desperateBomb)
            {
                // 157
                desperation = (int)BulletAIs.DesperateSuperNova;
                manager.spellCardTimer = spellCardTimer + 540;
            }

            Vector2 vel = player.Center.DirectionTo(Main.MouseWorld);

            player.GetModPlayer<GravityPlayer>().SpawnExplosion();

            Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, vel,
                ModContent.ProjectileType<FriendlyBullet>(), 0, 0, player.whoAmI, desperation, 
                (int)BulletColors.Black, (int)BulletSprites.SuperNova);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 0.2f * Main.essScale);
        }
    }
}
