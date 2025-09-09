using lenen.Content.Projectiles.BulletHellProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class FairySword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.knockBack = 3.5f;
            Item.DamageType = DamageClass.Melee;

            Item.value = Item.sellPrice(0, 0, 4, 25);
            Item.rare = ItemRarityID.Blue;

            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }
    }
}
