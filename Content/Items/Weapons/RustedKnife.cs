using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Weapons
{
    public class RustedKnife : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.shoot = ModContent.ProjectileType<RustyKnife>();
            Item.shootSpeed = 2f;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.MeleeNoSpeed;

            Item.width = 30;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.White;


            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shootsEveryUse = true;
            Item.noUseGraphic = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.LocalPlayer.HasItem(Item.type))
            {
                int itemIndex = Main.LocalPlayer.FindItem(Item.type);
                player.inventory[itemIndex].TurnToAir();
            }
        }
    }
}
