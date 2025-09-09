using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class FairyGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.shoot = ModContent.ProjectileType<BasicBullet>();
            Item.shootSpeed = 12f;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = Item.sellPrice(0, 0, 4, 25);
            Item.rare = ItemRarityID.Green;

            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.reuseDelay = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int color = (int)Main.rand.NextFromList(SheetFrame.Blue, SheetFrame.Cyan, SheetFrame.Green);
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, 
                color, (int)Sheet.Small, (int)BulletBehavior.NoGroundPenetrate);
            return false;
        }

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            return null;
        }
    }
}
