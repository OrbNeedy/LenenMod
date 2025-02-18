using lenen.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace lenen.Content.Tiles.Plants
{
    public class HarujionMultitile : ModTile
    {
        public static List<Point16> partsWithoutTile = new List<Point16>();

        public override void SetStaticDefaults()
        {
            partsWithoutTile = [new Point16(0, 0), new Point16(4, 0), new Point16(0, 3), new Point16(0, 4),
                new Point16(1, 4), new Point16(4, 4), new Point16(0, 5), new Point16(1, 5), new Point16(4, 5), 
                new Point16(0, 6), new Point16(1, 6), new Point16(4, 6), new Point16(0, 7), new Point16(1, 7), 
                new Point16(4, 7), new Point16(0, 8), new Point16(1, 8), new Point16(4, 8), new Point16(0, 9), 
                new Point16(1, 9), new Point16(4, 9), new Point16(0, 10), new Point16(1, 10), 
                new Point16(4, 10)];

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolid[Type] = false;

            TileID.Sets.ReplaceTileBreakUp[Type] = false;
            TileID.Sets.IgnoredInHouseScore[Type] = false;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = false;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
            TileID.Sets.TouchDamageDestroyTile[Type] = false;
            TileID.Sets.GetsDestroyedForMeteors[Type] = false;
            TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = true; 

            AdjTiles = [TileID.Tombstones]; 
            HitSound = SoundID.Grass;
            DustType = DustID.CorruptPlants;
            MinPick = 210;

            // Names
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(64, 19, 110), name);

            // Placement
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(2, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.AnchorValidTiles = new int[] { (int)TileID.Dirt, (int)TileID.Grass,
                (int)TileID.CorruptGrass, (int)TileID.CorruptJungleGrass, (int)TileID.CrimsonGrass,
                (int)TileID.CrimsonJungleGrass, (int)TileID.JungleGrass, (int)TileID.MushroomGrass,
                (int)TileID.Mud, (int)TileID.Sand, (int)TileID.Ebonsand, (int)TileID.Ebonstone,
                (int)TileID.Crimsand, (int)TileID.Crimstone, (int)TileID.Stone, (int)TileID.AshGrass,
                (int)TileID.Ash, (int)TileID.GolfGrass, (int)TileID.GolfGrassHallowed, (int)TileID.HallowedGrass,
                (int)TileID.SnowBlock, (int)TileID.DirtiestBlock
            };

            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            for (int y = frameY; y > j; y--)
            {
                for (int x = i; x < i+5; x++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.HasTile)
                    {
                        if (tile.TileType == ModContent.TileType<HarujionMultitile>())
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }
                }
            }
            base.KillMultiTile(i, j, frameX, frameY);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Main.NewText("HarujionX: " + ModContent.GetInstance<HarujionLocations>().harujionLocation.X);
            Main.NewText("FrameX: " + tile.TileFrameX);
            Main.NewText("FrameY: " + tile.TileFrameY);
            Main.NewText("PosX: " + i);
            if (partsWithoutTile.Contains(new Point16(tile.TileFrameX/18, tile.TileFrameY/18)))
            {
                return false;
            }
            return true;
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

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            Point16 topLeft = ModContent.GetInstance<HarujionLocations>().harujionLocation;
            if (!partsWithoutTile.Contains(new Point16(i, j) - topLeft)) return;
            num = fail ? 5 : 15;
        }
    }
}
