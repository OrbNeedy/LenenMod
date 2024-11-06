﻿using lenen.Content.Buffs;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class HaniwaMaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ProjectileID.Sets.MinionSacrificable[Item.shoot] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.damage = 68;
            Item.shoot = ModContent.ProjectileType<HaniwaCannon>();
            Item.buffType = ModContent.BuffType<HaniwaCannonBuff>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 3f;
            Item.mana = 10;

            Item.width = 54;
            Item.height = 54;
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ItemRarityID.Pink;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shootsEveryUse = true;
            Item.useTurn = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, 
                type, damage, knockback, Main.myPlayer, 0);
            projectile.damage = Item.damage;

            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ItemID.ClayBlock, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}