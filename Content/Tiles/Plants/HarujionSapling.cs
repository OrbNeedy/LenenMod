using lenen.Common.Systems;
using lenen.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace lenen.Content.Tiles.Plants
{
    public enum PlantStage : byte
    {
        Planted,
        Growing,
        Grown
    }

    public class HarujionSapling : ModTile, ITileData
    {
        private const int FrameWidth = 18; 
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileCut[Type] = false;
            Main.tileNoFail[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = false;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = false;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(64, 19, 110), name);

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = Point16.Zero;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                20
            };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = -1;
            TileObjectData.newTile.StyleHorizontal = true;
            /*TileObjectData.newTile.AnchorAlternateTiles = new int[] { (int)TileID.Dirt, (int)TileID.Grass,
                (int)TileID.CorruptGrass, (int)TileID.CorruptJungleGrass, (int)TileID.CrimsonGrass,
                (int)TileID.CrimsonJungleGrass, (int)TileID.JungleGrass, (int)TileID.MushroomGrass,
                (int)TileID.Mud, (int)TileID.Sand, (int)TileID.AshGrass, (int)TileID.Ash, (int)TileID.GolfGrass,
                (int)TileID.GolfGrassHallowed, (int)TileID.HallowedGrass, (int)TileID.SnowBlock,
                (int)TileID.DirtiestBlock };*/
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 
                TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.FlattenAnchors = true;

            TileObjectData.newTile.AnchorValidTiles = new int[] { (int)TileID.Dirt, (int)TileID.Grass,
                (int)TileID.CorruptGrass, (int)TileID.CorruptJungleGrass, (int)TileID.CrimsonGrass,
                (int)TileID.CrimsonJungleGrass, (int)TileID.JungleGrass, (int)TileID.MushroomGrass,
                (int)TileID.Mud, (int)TileID.Sand, (int)TileID.Ebonsand, (int)TileID.Ebonstone, 
                (int)TileID.Crimsand, (int)TileID.Crimstone, (int)TileID.Stone, (int)TileID.AshGrass, 
                (int)TileID.Ash, (int)TileID.GolfGrass, (int)TileID.GolfGrassHallowed, (int)TileID.HallowedGrass, 
                (int)TileID.SnowBlock, (int)TileID.DirtiestBlock 
            };
            TileObjectData.addTile(Type);

            HitSound = SoundID.Grass;
            DustType = DustID.CorruptPlants;

            MinPick = 65;

            TileID.Sets.SwaysInWindBasic[Type] = true;
            AdjTiles = new int[] { TileID.Plants };

            glowTexture = ModContent.Request<Texture2D>(Texture + "_Glowmask");
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            HarujionLocations.instance.harujionLocation = new Point16(i, j);
            HarujionLocations.instance.soulsAbsorbed = 0;
            HarujionLocations.instance.UpdateHarujion();
            base.PlaceInWorld(i, j, item);
        }

        public override bool IsTileDangerous(int i, int j, Player player)
        {
            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.4078f;
            g = 0.2509f;
            b = 0.5019f;
            base.ModifyLight(i, j, ref r, ref g, ref b);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                HarujionLocations.instance.harujionLocation = new Point16(0, 0);
                HarujionLocations.instance.soulsAbsorbed = 0;
            } else
            {
            }
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override bool CanDrop(int i, int j)
        {
            PlantStage stage = GetStage(i, j);
            return stage == PlantStage.Grown;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<Harujion>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 5 : 15;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            PlantStage stage = GetStage(i, j);

            if (HarujionLocations.instance.harujionLocation != new Point16(i, j)) return;

            if (stage == PlantStage.Planted && HarujionLocations.instance.soulsAbsorbed >= 1500)
            {
                tile.TileFrameX = FrameWidth; 
                MinPick = 150;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, 1);
                }
            }
            else if (stage == PlantStage.Growing && HarujionLocations.instance.soulsAbsorbed >= 3000)
            {
                tile.TileFrameX = FrameWidth * 2; 
                MinPick = 200;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, 1);
                }
            } else if (stage == PlantStage.Growing && HarujionLocations.instance.soulsAbsorbed >= 6000)
            {
                // Transform the Harujion into a tree
            }

        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -2; 
        }

        public override bool IsTileSpelunkable(int i, int j)
        {
            return true;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y);
            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                position + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 20),
                Lighting.GetColor(i, j), 
                0f, 
                Vector2.Zero, 
                1f, 
                SpriteEffects.None, 
                0f);
            
            spriteBatch.Draw(
                glowTexture.Value,
                position + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 20),
                Color.White, 
                0f, 
                Vector2.Zero, 
                1f, 
                SpriteEffects.None, 
                0f);
            return false;
        }

        private static PlantStage GetStage(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return (PlantStage)(tile.TileFrameX / FrameWidth);
        }
    }
}
