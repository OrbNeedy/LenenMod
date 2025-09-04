using lenen.Common.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace lenen.Content.Tiles.Plants
{
    public class HarujionMultitile : ModTile
    {
        public static List<Point16> partsWithTile = new List<Point16>();

        public override void SetStaticDefaults()
        {
            partsWithTile = [new Point16(2, 10), new Point16(3, 10)];

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolid[Type] = false;

            TileID.Sets.ReplaceTileBreakUp[Type] = false;
            TileID.Sets.IgnoredInHouseScore[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = false;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
            TileID.Sets.TouchDamageDestroyTile[Type] = false;
            TileID.Sets.GetsDestroyedForMeteors[Type] = false;
            TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = true; 

            AdjTiles = [TileID.Tombstones]; 
            HitSound = SoundID.Dig;
            DustType = DustID.CorruptPlants;
            MinPick = 210;
            MineResist = 4f;

            // Names
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(64, 19, 110), name);

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.GrandfatherClocks, 0));
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(CustomPlaceFunction, -1, 0, true);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(2, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 3);
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
        }

        private int CustomPlaceFunction(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6)
        {
            if (HarujionLocations.instance.harujionLocation != Point16.Zero)
            {
                return -1;
            }
            return 1;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            HarujionLocations.instance.harujionLocation = new Point16(i, j);
            base.PlaceInWorld(i, j, item);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            Point16 topLeft = HarujionLocations.instance.harujionLocation;
            if (partsWithTile.Contains(new Point16(i, j) - topLeft))
            {
                return false;
            }
            return base.KillSound(i, j, fail);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            /*Main.NewText("HarujionX: " + HarujionLocations.instance.harujionLocation.X);
            Main.NewText("FrameX: " + tile.TileFrameX/18);
            Main.NewText("FrameY: " + tile.TileFrameY/18);
            Main.NewText("PosX: " + i);
            Main.NewText("Can be killed: " + !partsWithTile.Contains(new Point16(tile.TileFrameX / 18, tile.TileFrameY / 18)));
            Main.NewText("List: ");
            foreach (var thing in partsWithTile)
            {
                Main.NewText(thing);
            }*/
            if (!partsWithTile.Contains(new Point16(tile.TileFrameX/18, tile.TileFrameY/18)))
            {
                return false;
            }
            return false;
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
            // Will not work until the 
            Point16 topLeft = HarujionLocations.instance.harujionLocation;
            if (partsWithTile.Contains(new Point16(i, j) - topLeft))
            {
                num = 0;
                return;
            }
            num = fail ? 5 : 15;
        }
    }
}
