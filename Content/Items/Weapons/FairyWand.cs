using lenen.Content.Projectiles.BulletHellProjectiles;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace lenen.Content.Items.Weapons
{
    public class FairyWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.shoot = ModContent.ProjectileType<BasicBullet>();
            Item.shootSpeed = 7.5f;
            Item.knockBack = 1.5f;
            Item.mana = 6;
            Item.DamageType = DamageClass.Magic;

            Item.value = Item.sellPrice(0, 0, 4, 25);
            Item.rare = ItemRarityID.Green;

            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int color = (int)Main.rand.NextFromList(SheetFrame.Blue, SheetFrame.Cyan, SheetFrame.Green);
            for (int i = -1; i < 2; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4 * i), 
                    type, damage, knockback, player.whoAmI, color, (int)Sheet.Default);
            }
            return false;
        }

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            return null;
        }
    }
}
