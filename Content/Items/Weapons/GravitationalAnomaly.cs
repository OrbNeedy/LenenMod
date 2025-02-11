using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class GravitationalAnomaly : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.shoot = ModContent.ProjectileType<FriendlyBullet>();
            Item.shootSpeed = 6f;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;

            Item.width = 32;
            Item.height = 36;
            Item.value = Item.sellPrice(0, 0, 2, 20);
            Item.rare = ItemRarityID.Yellow;
            

            Item.useTime = 75;
            Item.useAnimation = 75;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 10; i++)
            {
                int spriteType = Main.rand.Next((int)BulletSprites.Simple, (int)BulletSprites.Pellet + 1);
                int color = Main.rand.Next((int)BulletColors.White, (int)BulletColors.DarkBlue + 1);
                Projectile.NewProjectile(source, position, 
                    velocity.RotatedBy(MathHelper.TwoPi*i / 10), type, damage, knockback, player.whoAmI, 
                    (int)BulletAIs.Simple, color, spriteType);
            }
            return false;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 0.4f * Main.essScale);
        }
    }
}
