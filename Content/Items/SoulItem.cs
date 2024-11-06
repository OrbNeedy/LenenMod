using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using lenen.Common.Players;
using lenen.Content.Tiles.Plants;
using Terraria.DataStructures;
using lenen.Common.Systems;

namespace lenen.Content.Items
{
    public class SoulItem : ModItem
    {
        public Tile selectedHarujionTile;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 60, 0);
            Item.rare = ItemRarityID.Master;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Item.velocity = new Vector2(Main.rand.NextFloat(-10, 11), Main.rand.Next(-10, 11));
            base.OnSpawn(source);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.DeadOrGhost) continue;
                if (Collision.CheckAABBvAABBCollision(Item.position, Item.Size, player.position, player.Size))
                {
                    player.GetModPlayer<SoulAbsorptionPlayer>().AddSouls(Item.stack);
                    Item.active = false;
                }
            }
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Beige.ToVector3() * 0.45f * Main.essScale);
        }

        public override bool CanPickup(Player player)
        {
            return player.GetModPlayer<DeathManagingPlayer>().harujionRevival;
        }

        public override bool OnPickup(Player player)
        {
            player.GetModPlayer<SoulAbsorptionPlayer>().AddSouls(Item.stack);
            return false;
        }

        public override bool CanStackInWorld(Item source)
        {
            return false;
        }
    }
}
