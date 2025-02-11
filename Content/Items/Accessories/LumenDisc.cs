using lenen.Common;
using lenen.Common.Players;
using lenen.Content.Buffs;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class LumenDisc : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.accessory = true;
            Item.defense = 7;

            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BuffPlayer>().lumenBuff = true;
            player.AddBuff(ModContent.BuffType<LumenBallBuff>(), 2);
            base.UpdateAccessory(player, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LumenDiscFragment>(6)
                .AddTile(TileID.DemonAltar)
                .AddCondition(Condition.TimeDay)
                .Register();

            CreateRecipe()
                .AddIngredient<LumenDiscFragment>(6)
                .AddIngredient(ItemID.WhiteTorch)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
