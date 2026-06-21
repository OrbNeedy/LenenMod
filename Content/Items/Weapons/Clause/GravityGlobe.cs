using lenen.Common.Players;
using lenen.Common.Utils;
using lenen.Content.Items.Misc;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons.Clause
{
    public class GravityGlobe : SpellCardItem
    {
        protected override SpellCard SpellCardID => Common.Players.SpellCard.SuperNova;
        protected override int SpellCardCooldown => 1020;
        protected override int DesperateCooldown => 1560;
        protected override int ManaUse => 150;
        protected override int DesperateManaUse => 200;
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
            Item.rare = ItemRarityID.Red;

            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                SpellCardManagement scManager = player.GetModPlayer<SpellCardManagement>();

                SetCooldown(player);
                SpellCard(player, scManager.desperateBomb);
                return false;
            }

            for (int i = 0; i < 16; i++)
            {
                int spriteType = BulletUtils.GetRandomShape([
                    Sheet.Default, Sheet.Big, Sheet.Small, Sheet.ReverseBig, Sheet.Pellet]);

                int color = (int)SheetFrame.Pink;
                switch (spriteType)
                {
                    case (int)Sheet.ReverseBig:
                    case (int)Sheet.Small:
                        color = (int)SheetFrame.White;
                        break;
                    case (int)Sheet.Big:
                        color = (int)SheetFrame.Red;
                        break;
                    case (int)Sheet.Pellet:
                        color = (int)SheetFrame.Blue;
                        break;
                }

                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.TwoPi * i / 16), type, damage, knockback,
                    player.whoAmI, color, spriteType);
            }
            return false;
        }

        protected override void SpellCard(Player player, bool desperate)
        {
            int desperation = 1;

            int damage = (int)(player.GetWeaponDamage(Item) * 1.8f);
            if (desperate)
            {
                damage = (int)(player.GetWeaponDamage(Item) * 2.6f);
                desperation = 2;
            }

            // Spawn bullets
            // I WILL change you soon, Slowed Bullet projectile type
            int bulletType = ModContent.ProjectileType<SlowedBullet>();
            for (int i = 0; i < 22; i++)
            {
                // Set the sprite type and color
                int spriteType = BulletUtils.GetRandomShape([
                    Sheet.Default, Sheet.Big, Sheet.Small, Sheet.ReverseBig, Sheet.Pellet]);

                int color = (int)SheetFrame.Pink;
                switch (spriteType)
                {
                    case (int)Sheet.ReverseBig:
                    case (int)Sheet.Small:
                        color = (int)SheetFrame.White;
                        break;
                    case (int)Sheet.Big:
                        color = (int)SheetFrame.Red;
                        break;
                    case (int)Sheet.Pellet:
                        color = (int)SheetFrame.Blue;
                        break;
                }

                Vector2 direction = new Vector2(Main.rand.NextFloat(4, 22), 0).
                    RotatedByRandom(MathHelper.TwoPi);
                Projectile.NewProjectile(player.GetSource_ItemUse(Item, "SpellCard"), player.Center,
                    direction, bulletType, damage, 4f, player.whoAmI, color, spriteType);
            }

            Vector2 vel = player.Center.DirectionTo(Main.MouseWorld);

            int gravityType = ModContent.ProjectileType<SuperNovaBullet>();
            Projectile.NewProjectile(player.GetSource_ItemUse(Item, "Spellcard"),
                player.Center, vel, gravityType, 0, 0, player.whoAmI, desperation, 1);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 0.2f * Main.essScale);
        }
    }
}
