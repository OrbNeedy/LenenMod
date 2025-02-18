using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using lenen.Common.Players;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;

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
            Item.width = 0;
            Item.height = 0;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 60, 0);

            ItemID.Sets.OverflowProtectionTimeOffset[Item.type] = 1;
            ItemID.Sets.ItemSpawnDecaySpeed[Item.type] = 5;
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
                if (Collision.CheckAABBvAABBCollision(Item.position - new Vector2(25), new Vector2(50), 
                    player.position, player.Size))
                {
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/item_00") with
                    {
                        Volume = 1f,
                        PitchVariance = 0.25f
                    }, player.Center);
                    player.GetModPlayer<SoulAbsorptionPlayer>().AddSouls(Item.stack);
                    Item.active = false;
                }
            }
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Beige.ToVector3() * 0.8f * Main.essScale);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D spirit = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(
                spirit,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X,
                    Item.position.Y - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, spirit.Width, spirit.Height),
                Lighting.GetColor(Item.Center.ToTileCoordinates(), Color.White),
                rotation,
                spirit.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
            return false;
        }

        public override bool CanPickup(Player player)
        {
            return player.GetModPlayer<DeathManagingPlayer>().harujionRevival || 
                player.Distance(Item.Center) <= 120;
        }

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/item_00") with
            {
                Volume = 1f,
                PitchVariance = 0.25f
            }, player.Center);
            player.GetModPlayer<SoulAbsorptionPlayer>().AddSouls(Item.stack);
            return false;
        }

        public override bool CanStackInWorld(Item source)
        {
            return false;
        }
    }
}
