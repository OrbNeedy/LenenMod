using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class DimensionalFragment : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 111;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 6;
            Item.shootSpeed = 32f;
            Item.shoot = ModContent.ProjectileType<DimensionalFragmentProjectile>();
            Item.mana = 8;

            Item.width = 14;
            Item.height = 30;
            Item.value = 0;
            Item.rare = ItemRarityID.LightRed;

            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item143 with { Volume = 0.5f, PitchVariance = 0.1f });
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
