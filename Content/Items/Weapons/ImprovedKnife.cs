﻿using lenen.Common.Players;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class ImprovedKnife : ModItem
    {
        private int spellCardTimer = 420;
        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.shoot = ModContent.ProjectileType<ConcealedKnife>();
            Item.shootSpeed = 10f;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;

            Item.width = 30;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 5, 15);
            Item.rare = ItemRarityID.Green;

            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(spellCardTimer);

        public override void UpdateInventory(Player player)
        {
        }

        public override void HoldItem(Player player)
        {
            player.aggro -= 250;
            if (player.GetModPlayer<SpellCardManagement>().desperateBomb)
            {
                player.aggro -= 250;
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }

            int[] types = [ ModContent.ProjectileType<ConcealedKnife>(), 
                ModContent.ProjectileType<BoneKnife>(), ModContent.ProjectileType<DarkKnife>() ];
            for (int i = 0; i < 5; i++)
            {
                Vector2 vel = velocity.RotatedByRandom(MathHelper.PiOver4);
                Projectile.NewProjectile(source, position, vel, 
                    types[Main.rand.Next(0, types.Length)], damage, knockback, player.whoAmI);
            }
            return false;
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

        public void SpellCard(Player player)
        {
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            manager.spellCardTimer = spellCardTimer;

            // 40
            int dmg = (int)(player.GetWeaponDamage(Item) * 1.53846f);
            int desperation = 3;
            if (manager.desperateBomb)
            {
                // 125
                dmg = (int)(player.GetWeaponDamage(Item) * 4.80769f);
                desperation = 5;
                manager.spellCardTimer = spellCardTimer + 480;
            }

            int direction = player.Center.X <= Main.MouseWorld.X ? 1 : -1;
            Vector2 vel = new Vector2(direction*12, 0);

            for (int i = desperation; i > 0; i--)
            {
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item, "Spellcard"), player.Center, vel,
                ModContent.ProjectileType<ConcealedKnife>(), dmg, Item.knockBack, player.whoAmI, i, direction, 
                desperation);
            }
        }
    }
}
