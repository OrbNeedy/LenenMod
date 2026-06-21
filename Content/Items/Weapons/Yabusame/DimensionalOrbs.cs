using lenen.Content.Projectiles;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using lenen.Common.Players;
using lenen.Content.Projectiles.BulletHellProjectiles;
using lenen.Content.Items.Misc;

namespace lenen.Content.Items.Weapons.Yabusame
{
    public class DimensionalOrbs : SpellCardItem
    {
        protected override SpellCard SpellCardID => Common.Players.SpellCard.WarpKingdom;
        protected override int SpellCardCooldown => 900;
        protected override int DesperateCooldown => 1080;
        protected override int ManaUse => 100;
        protected override int DesperateManaUse => 100;

        private int manaCost = 2;
        int spellCardUse = 0;

        public override void SetDefaults()
        {
            Item.damage = 82;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 6;
            Item.shootSpeed = 32f;
            Item.shoot = ModContent.ProjectileType<DimensionalFragmentProjectile>();

            Item.width = 30;
            Item.height = 30;
            Item.value = 0;
            Item.rare = ItemRarityID.Lime;

            Item.useTime = 10;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaCost);

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player) && player.CheckMana(manaCost, false, false);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.itemAnimation == Item.useAnimation)
                {
                    SpellCardManagement scManager = player.GetModPlayer<SpellCardManagement>();

                    SetCooldown(player);
                    SpellCard(player, scManager.desperateBomb);
                }
                return false;
            }

            if (!player.CheckMana(manaCost, true, false))
            {
                return false;
            }
            player.manaRegenDelay = player.manaRegenCount;
            SoundEngine.PlaySound(SoundID.Item143 with { Volume = 0.5f, PitchVariance = 0.1f }); 
            
            Vector2 orbit = new Vector2(35f, 0).RotatedBy(Main.GameUpdateCount * MathHelper.Pi / 20);
            Vector2 offset = orbit * new Vector2(1.2f, 0.4f);

            offset = offset.RotatedBy(Math.Cos(Main.GameUpdateCount * MathHelper.Pi / 45) * 0.6);
            Vector2 alternatePosition = position + offset.RotatedBy(MathHelper.Pi);
            position += offset;

            Projectile.NewProjectile(source, position, 
                new Vector2(-1, 0).RotatedBy(MathHelper.Pi + position.AngleTo(Main.MouseWorld)) * velocity.Length(), 
                type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, alternatePosition,
                new Vector2(-1, 0).RotatedBy(MathHelper.Pi + 
                alternatePosition.AngleTo(Main.MouseWorld)) * velocity.Length(), 
                type, damage, knockback, player.whoAmI);
            return false;
        }

        /*public override bool AltFunctionUse(Player player)
        {
            return CanUseSpellCard(player);
        }*/

        protected override void SpellCard(Player player, bool desperate)
        {
            int dmg = (int)(player.GetWeaponDamage(Item) * 0.7f);
            float desperation = 6f;
            if (desperate)
            {
                dmg = (int)(player.GetWeaponDamage(Item) * 1.2f);
                desperation = 10f;
            }

            int projectileType = ModContent.ProjectileType<WarpingBullet>();
            float rotation = MathHelper.TwoPi / desperation;
            Vector2 dir = new Vector2(0, -10).RotatedByRandom(MathHelper.TwoPi);
            int rotationDirection = spellCardUse % 2 == 0 ? 1 : -1;

            for (int i = 0; i <= desperation; i++)
            {
                int col = (int)SheetFrame.Yellow;
                for (int k = 0; k < 14; k++)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item, "Spellcard"),
                        player.Center, dir, projectileType, dmg, Item.knockBack, player.whoAmI,
                        rotationDirection, k * -7, col);
                    col = (int)SheetFrame.White;
                }

                /*col = (int)SheetFrame.Yellow;
                for (int k = 0; k < 8; k++)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item, "Spellcard"),
                        player.Center, dir, projectileType, dmg, Item.knockBack, player.whoAmI,
                        0, k * -7, col);
                    col = (int)SheetFrame.White;
                }*/

                dir = dir.RotatedBy(rotation);
            }

            spellCardUse++;
        }
    }
}
