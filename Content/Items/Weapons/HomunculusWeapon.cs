using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using lenen.Content.Projectiles.BulletHellProjectiles;

namespace lenen.Content.Items.Weapons
{
    public class HomunculusWeapon : ModItem
    {
        private int spellCardTimer = 1200;

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3f;
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<MeleeKnife>();

            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Yellow;

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(spellCardTimer);

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<AfterimageEffectPlayer>().useAfterimages = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }

            ShootFromPosition(player, source, position, velocity, type, damage, knockback);

            AfterimageEffectPlayer afterimages = player.GetModPlayer<AfterimageEffectPlayer>();
            for (int i = 0; i < afterimages.positionStorage.Length; i++)
            {
                if (afterimages.shadowIndex[i] == -1)
                {
                    continue;
                }
                Vector2 pos = afterimages.positionStorage[i];
                ShootFromPosition(player, source, pos, velocity, type, damage, knockback);
            }
            
            return false;
        }

        public void ShootFromPosition(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i < 2; i++)
            {
                Vector2 otherVelocity = velocity.RotatedBy(i * MathHelper.Pi / 4f);
                Projectile.NewProjectile(source, position, otherVelocity, type, damage, 
                    knockback, player.whoAmI, (int)SheetFrame.Red, (int)Sheet.Knife);
            }
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
            manager.lastSpellCard = Common.Players.SpellCard.DharmaPower;
            manager.lastDesperate = manager.desperateBomb;
            int droplets = 20;

            int dmg = (int)(player.GetWeaponDamage(Item) * 2.2f);
            if (manager.desperateBomb)
            {
                dmg = (int)(player.GetWeaponDamage(Item) * 3f);
                manager.spellCardTimer = spellCardTimer + 600;
                droplets = 30;
            }

            manager.swordDropletsLeft = droplets;
            manager.storedDirection = player.Center.DirectionTo(Main.MouseWorld);
            manager.dropletsDamage = dmg;
            manager.cummulativeDropletTime = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddIngredient(ItemID.RichMahogany, 6)
                .AddIngredient(ItemID.Ectoplasm, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
