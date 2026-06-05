using lenen.Content.Buffs;
using lenen.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using lenen.Common.Players;
using lenen.Common;
using System.Collections.Generic;
using lenen.Content.Items.Accessories;
using Terraria.Localization;

namespace lenen.Content.Items.Weapons
{
    public class HaniwaCreationTools : ModItem
    {
        private int spellCardTimer = 2400;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            Item.staff[Type] = true;

            ProjectileID.Sets.MinionSacrificable[Item.shoot] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = false;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = false;
            Item.damage = 84;
            Item.shoot = ModContent.ProjectileType<HaniwaCannon>();
            Item.buffType = ModContent.BuffType<HaniwaCommander>();
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
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shootsEveryUse = true;
            Item.useTurn = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(spellCardTimer);

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player) && player.ownedProjectileCounts[ModContent.ProjectileType<SuperHaniwaHead>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SuperHaniwaHead>()] > 0 || 
                player.maxMinions < 1) return false;

            if (player.altFunctionUse == 2)
            {
                //Main.NewText("Alt func");
                SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
                if (manager.spellCardTimer <= 0)
                {
                    SpellCard(player);
                }
                return false;
            }
            player.AddBuff(Item.buffType, 2);
            for (int i = -1; i < 2; i += 2)
            {
                Projectile projectile = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero,
                    type, damage, knockback, Main.myPlayer, i);
                projectile.damage = Item.damage;
            }

            Projectile clone = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero,
                ModContent.ProjectileType<HaniwaClone>(), damage, knockback, Main.myPlayer, 1);
            clone.damage = Item.damage;

            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public void SpellCard(Player player)
        {
            //Main.NewText("Super Haniwa");
            SpellCardManagement manager = player.GetModPlayer<SpellCardManagement>();
            int desperate = 0;
            spellCardTimer = 2100;
            manager.spellCardTimer = spellCardTimer;
            manager.lastSpellCard = Common.Players.SpellCard.SuperHaniwa;
            manager.lastDesperate = manager.desperateBomb;

            if (manager.desperateBomb) desperate = 1;

            int dmg = (int)(player.GetWeaponDamage(Item) * (1.3f + (desperate * 0.5f)));

            Vector2 pos = player.Center + new Vector2(0, 960);

            Projectile.NewProjectile(player.GetSource_ItemUse(Item, "Spellcard"), pos, 
                Vector2.Zero, ModContent.ProjectileType<SuperHaniwaHead>(), dmg, 
                4f, player.whoAmI, desperate);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HaniwaMaker>()
                .AddIngredient<HaniwaPerson>()
                .AddIngredient(ItemID.Ectoplasm, 4)
                .AddIngredient(ItemID.ClayBlock, 12)
                .AddCondition(LenenConditions.HasEnoughSouls2)
                .AddOnCraftCallback((Recipe recipe, Item item, List<Item> list, Item item2) => {
                    RevivedGashadokuroSkull.TakeAwaySouls(LenenConditions.HaniwaUpgradeSpirits);
                })
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
