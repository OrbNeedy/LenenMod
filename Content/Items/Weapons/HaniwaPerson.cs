using lenen.Content.Buffs;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class HaniwaPerson //: ModItem
    {
        /*public override void SetDefaults()
        {
            Item.damage = 68;
            Item.shoot = ModContent.ProjectileType<HaniwaClone>(); 
            Item.buffType = ModContent.BuffType<HaniwaCloneBuff>(); 
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 3f;
            Item.mana = 10;

            Item.width = 42;
            Item.height = 42;
            Item.value = Item.sellPrice(0, 0, 40, 10);
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
                .AddIngredient(ItemID.ClayBlock, 20)
                .AddIngredient(ItemID.RichMahogany, 12)
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }*/
    }
}
