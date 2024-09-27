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

namespace lenen.Content.Items.Weapons
{
    public class DimensionalOrbs : ModItem
    {
        private int manaCost = 4;
        private int spellCardTimer = 300;
        private int spellCardCost = 100;

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 6;
            Item.shootSpeed = 32f;
            Item.shoot = ModContent.ProjectileType<DimensionalFragmentProjectile>();

            Item.width = 30;
            Item.height = 30;
            Item.value = 0;
            Item.rare = ItemRarityID.LightRed;

            Item.useTime = 5;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaCost, spellCardCost, spellCardTimer);

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player) && player.CheckMana(manaCost, false, false);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity, int type, int damage, float knockback)
        {
            // Debug more if mana regen delay causes issues
            if (player.altFunctionUse == 2)
            {
                return false;
            }

            if (!player.CheckMana(manaCost, true, false))
            {
                return false;
            }
            player.manaRegenDelay = player.manaRegenCount;
            SoundEngine.PlaySound(SoundID.Item143 with { Volume = 0.5f, PitchVariance = 0.1f }); 
            
            Vector2 orbit = new Vector2(35f, 0).RotatedBy(((Main.GameUpdateCount * MathHelper.Pi) / 20));
            Vector2 offset = orbit * new Vector2(1.2f, 0.4f);

            offset = offset.RotatedBy(Math.Cos(Main.GameUpdateCount * MathHelper.Pi / 45) * 0.6);
            Vector2 alternatePosition = position + offset.RotatedBy(Math.PI);
            position += offset;
            Projectile.NewProjectile(source, position, 
                new Vector2(-1, 0).RotatedBy(Math.PI + position.AngleTo(Main.MouseWorld)) * velocity.Length(), 
                type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, alternatePosition,
                new Vector2(-1, 0).RotatedBy(Math.PI + 
                alternatePosition.AngleTo(Main.MouseWorld)) * velocity.Length(), 
                type, damage, knockback, player.whoAmI);
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

        private bool SpellCard(Player player)
        {
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            player.CheckMana(spellCardCost, true, true);
            player.manaRegenDelay = player.manaRegenCount;
            manager.spellCardTimer = spellCardTimer;

            int dmg = (int)(player.GetTotalDamage(Item.DamageType).ApplyTo(30));
            float desperation = 0f;
            if (manager.desperateBomb)
            {
                dmg = (int)(player.GetTotalDamage(Item.DamageType).ApplyTo(45));
                desperation = 1f;
                manager.spellCardTimer = spellCardTimer + 180;
            }

            for (int i = 0; i <= 30; i++)
            {
                Vector2 vel = new Vector2(10, 0).RotatedBy(
                    -Main.rand.NextFloat((int)Math.PI));
                int c = player.GetWeaponCrit(Item);
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center,
                    vel * Main.rand.NextFloat(0.75f, 1.25f),
                    ModContent.ProjectileType<LaserStarter>(), dmg,
                    Item.knockBack, player.whoAmI, desperation);
            }
            return false;
        }
    }
}
