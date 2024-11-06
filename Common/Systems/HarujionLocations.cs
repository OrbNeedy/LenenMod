using lenen.Content.Buffs;
using lenen.Content.Items;
using lenen.Content.Tiles.Plants;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace lenen.Common.Systems
{
    public class HarujionLocations : ModSystem
    {
        public Point16 harujionLocation = new(0, 0);
        public int soulsAbsorbed = 0;
        public int[] validTiles = new[] { (int)TileID.Dirt, (int)TileID.Grass,
                (int)TileID.CorruptGrass, (int)TileID.CorruptJungleGrass, (int)TileID.CrimsonGrass,
                (int)TileID.CrimsonJungleGrass, (int)TileID.JungleGrass, (int)TileID.MushroomGrass,
                (int)TileID.Mud, (int)TileID.Sand, (int)TileID.AshGrass, (int)TileID.Ash, (int)TileID.GolfGrass,
            (int)TileID.GolfGrassHallowed, (int)TileID.HallowedGrass, (int)TileID.SnowBlock,
                (int)TileID.DirtiestBlock};

        public override void SaveWorldData(TagCompound tag)
        {
            Point16 point = harujionLocation;
            tag["HarujionXPosition"] = point.X;
            tag["HarujionYPosition"] = point.Y;
            tag["HarujionSouls"] = soulsAbsorbed;
            base.SaveWorldData(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("HarujionXPosition") && tag.ContainsKey("HarujionYPosition"))
            {
                harujionLocation = new Point16(tag.GetShort("HarujionXPosition"), tag.GetShort("HarujionYPosition"));
            }

            if (tag.ContainsKey("HarujionSouls"))
            {
                soulsAbsorbed = tag.GetInt("HarujionSouls");
            }
        }

        public override void OnWorldLoad()
        {
            if (harujionLocation == Point16.Zero)
            {
                //Main.NewText("Reafirming Harujion's existence");
                ReafirmHarujion();
            }
            if (TileEntity.ByPosition.TryGetValue(harujionLocation, out TileEntity tile))
            {
                if (TileLoader.GetTile(Main.tile[harujionLocation].TileType) is not HarujionSapling harujion)
                {
                    ReafirmHarujion();
                }
            }
            else
            {
                ReafirmHarujion();
            }
        }

        public override void PreUpdateWorld()
        {
            if (Main.rand.NextBool(7200))
            {
                ReafirmHarujion();
            }

            if (Main.rand.NextFloat() < 0.06f)
            {
                Tile tile = Framing.GetTileSafely(harujionLocation.X, harujionLocation.Y);
                if (tile.TileType == ModContent.TileType<HarujionSapling>())
                {
                    DeleteLife();
                }
            }

            //Main.NewText("Harujion position: " + harujionLocation);
            //Main.NewText("Harujion position (World): " + harujionLocation.ToWorldCoordinates());
            //Main.NewText("Harujion souls: " + soulsAbsorbed);

            if (harujionLocation != Point16.Zero)
            {
                //Main.NewText("Harujion does exist");
                Point16 point = harujionLocation;
                Vector2 location = point.ToWorldCoordinates();
                float radius = GetRadius() * 5;

                foreach (Item item in Main.ActiveItems)
                {
                    if (item.ModItem is SoulItem && item.DistanceSQ(location) <= radius)
                    {
                        if (Collision.CheckAABBvAABBCollision(location, new Vector2(16, 18),
                            item.position, item.Size))
                        {
                            soulsAbsorbed += item.stack;
                            item.active = false;
                        }

                        item.Center += item.DirectionTo(location) * 4;
                    }
                }
            }

            base.PreUpdateWorld();
        }

        public override void PreUpdatePlayers()
        {
            if (harujionLocation == Point16.Zero) return;
            Point16 point = harujionLocation;
            Vector2 location = point.ToWorldCoordinates();
            float radius = GetRadius();
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.DistanceSQ(location) <= radius)
                {
                    player.AddBuff(ModContent.BuffType<HarujionAbsorb>(), 2);
                }
            }
            base.PreUpdatePlayers();
        }

        /*public override void PostUpdatePlayers()
        {
            Main.NewText("Post Update Player");
            Point16 point = harujionLocation;
            Vector2 location = point.ToWorldCoordinates();
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.DistanceSQ(location) <= 30000)
                {
                    Main.NewText($"Player {player.name} has their soul absorbed");
                    player.AddBuff(ModContent.BuffType<HarujionAbsorb>(), 2);
                    player.GetModPlayer<SoulAbsorptionPlayer>().harujionPotency = 10;
                }
            }
            base.PostUpdatePlayers();
        }*/

        public override void PreUpdateNPCs()
        {
            if (harujionLocation == Point16.Zero) return;
            Point16 point = harujionLocation;
            Vector2 location = point.ToWorldCoordinates();
            float radius = GetRadius();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCID.TargetDummy) return;
                if (npc.DistanceSQ(location) <= radius)
                {
                    npc.AddBuff(ModContent.BuffType<HarujionAbsorb>(), 2);
                }
            }
            base.PreUpdateNPCs();
        }

        /*public override void PostUpdateNPCs()
        {
            Main.NewText("Post Update NPC");
            Point16 point = harujionLocation;
            Vector2 location = point.ToWorldCoordinates();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.DistanceSQ(location) <= 30000)
                {
                    Main.NewText($"NPC {npc.FullName} has their soul absorbed");
                    npc.GetGlobalNPC<SoulDrops>().harujionPotency = 10;
                }
            }
            base.PostUpdateNPCs();
        }*/

        public float GetRadius()
        {
            float radius = 60000f;
            radius *= (80f + soulsAbsorbed) / 80f;
            return radius;
        }

        public float GetGrowth()
        {
            float growth = 1f;
            growth += soulsAbsorbed / 2000f;
            return growth;
        }

        public Point16? WorldHasHarujion()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.TileType == ModContent.TileType<HarujionSapling>())
                    {
                        //Main.NewText($"Found Harujion at X: {i} and Y: {j}");
                        return new Point16(i, j);
                    }
                }
            }
            //Main.NewText("No Harujion found");
            return null;
        }

        public void UpdateHarujion()
        {
            //Main.NewText("Updating Harujion");
            //Main.NewText($"Saved location: {harujionLocation}");
            if (harujionLocation == Point16.Zero)
            {
                //Main.NewText("It is supposed to not exist");
                var response = WorldHasHarujion();
                if (response != null)
                {
                    //Main.NewText("It actually exists");
                    //Main.NewText($"Exists at: {response}");
                    if (harujionLocation != (Point16)response)
                    {
                        harujionLocation = (Point16)response;
                        soulsAbsorbed = 0;
                    }
                }
                else
                {
                    //Main.NewText("It does not exist");
                    harujionLocation = Point16.Zero;
                    soulsAbsorbed = 0;
                }
            }
            else
            {
                //Main.NewText("It is supposed to exist");
                Tile tile = Framing.GetTileSafely(harujionLocation.X, harujionLocation.Y);
                if (tile.TileType != ModContent.TileType<HarujionSapling>())
                {
                    var response = WorldHasHarujion();
                    if (response != null)
                    {
                        //Main.NewText("It exists, at a different location");
                        //Main.NewText($"Supposed location: {harujionLocation}");
                        //Main.NewText($"Real location: {response}");
                        if (harujionLocation != (Point16)response)
                        {
                            harujionLocation = (Point16)response;
                            soulsAbsorbed = 0;
                        }
                    }
                    else
                    {
                        //Main.NewText("It does not exist");
                        harujionLocation = Point16.Zero;
                        soulsAbsorbed = 0;
                    }
                }
                else
                {
                    //Main.NewText("It exists");
                    //Main.NewText($"Harujion location: {harujionLocation}");
                }
                //Main.NewText($"Final location: {harujionLocation}");
            }
        }

        public void ReafirmHarujion()
        {
            var response = WorldHasHarujion();
            if (response == null)
            {
                AddHarujion();
            }
            else
            {
                if (harujionLocation != (Point16)response)
                {
                    harujionLocation = (Point16)response;
                    soulsAbsorbed = 0;
                }
            }
        }

        private void AddHarujion()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            bool success = false;
            int attempts = 0;

            while (!success)
            {
                attempts++;
                if (attempts > 1000)
                {
                    ModContent.GetInstance<lenen>().Logger.Info($"Failed adding Harujion after {attempts} attempts");
                    break;
                }

                int x = Main.rand.Next(0, Main.maxTilesX);
                int y = Main.rand.Next((int)(Main.maxTilesY * 0.15), (int)Main.worldSurface);

                Tile tile = Framing.GetTileSafely(x, y);

                WorldGen.PlaceTile(x, y, ModContent.TileType<HarujionSapling>(), true);

                success = Main.tile[x, y].TileType == ModContent.TileType<HarujionSapling>();
                if (success)
                {
                    harujionLocation = new Point16(x, y);
                    soulsAbsorbed = 0;
                    ModContent.GetInstance<lenen>().Logger.Info($"Added Harujion at {attempts} attempts");
                    ModContent.GetInstance<lenen>().Logger.Info($"Harujion at tile coordinates " +
                        $"X:{x} and Y: {y}");
                }
            }
        }

        private void DeleteLife()
        {
            Point16 range = new Point16((int)Math.Sqrt(200f * (80f + soulsAbsorbed) / 80f));
            float rangeSquared = (float)range.X;
            Point16 searchStart = harujionLocation - range;
            Point16 searchEnd = harujionLocation + range;
            Vector2 harujion = harujionLocation.ToVector2();
            //Main.NewText($"Referenced Harujion position: {harujion}");
            //Main.NewText($"Range of search: {rangeSquared}");
            //Main.NewText($"That means from {searchStart} to {searchEnd}");

            for (int i = 0; i < 20; i++)
            {
                Point16 randTileSelect = new Point16(
                    Main.rand.Next(searchStart.X, searchEnd.X), Main.rand.Next(searchStart.Y, searchEnd.Y));
                if (randTileSelect == harujionLocation)
                {
                    //Main.NewText("Location was the same as Harujion");
                    continue;
                }

                float circlePoint = CheckCircle(harujion, randTileSelect.ToVector2());
                if (circlePoint < rangeSquared)
                {
                    Tile tile = Framing.GetTileSafely(randTileSelect.X, randTileSelect.Y);
                    if (tile.HasTile)
                    {
                        //Main.NewText($"Trying to eliminate tile {tile.TileType}");
                        float chance = (circlePoint) / rangeSquared;
                        if (Main.rand.NextFloat() < chance && Main.rand.NextFloat() < 0.8f)
                        {
                            bool success = ReplaceByType(tile) || 
                                KillByType(tile, randTileSelect.X, randTileSelect.Y);
                            WorldGen.SquareTileFrame(randTileSelect.X, randTileSelect.Y, true);
                            WorldGen.SquareWallFrame(randTileSelect.X, randTileSelect.Y, true);
                            if (success)
                            {
                                //Main.NewText($"Success");
                            }
                            else
                            {
                                //Main.NewText($"Did not work");
                            }
                            if (success && Main.rand.NextBool())
                            {
                                soulsAbsorbed += 1;
                            }
                        }
                    } else
                    {
                        //Main.NewText($"Location did not have a tile");
                    }
                } else
                {
                    //Main.NewText($"Location was outside of the range");
                    //Main.NewText($"Location: {randTileSelect}");
                }

            }
            
            /*for (int i = searchStart.X; i < searchEnd.X; i++)
            {
                for (int k = searchStart.Y; k < searchEnd.Y; k++)
                {
                    if (new Point16(i, k) == harujionLocation) continue;
                    float circlePoint = CheckCircle(harujion, new Vector2(i, k));
                    if (circlePoint < rangeSquared)
                    {
                        Tile tile = Framing.GetTileSafely(i, k);
                        if (tile.HasTile)
                        {
                            Main.NewText($"Trying to eliminate tile {tile.TileType}");
                            float chance = (circlePoint) / rangeSquared;
                            if (Main.rand.NextFloat() < chance && Main.rand.NextFloat() < 0.25f)
                            {
                                bool success = ReplaceByType(tile) || KillByType(tile, i, k);
                                WorldGen.SquareTileFrame(i, k, true);
                                //WorldGen.SquareWallFrame(i, k, true);
                                if (success)
                                {
                                    Main.NewText($"Success");
                                } else
                                {
                                    Main.NewText($"Did not work");
                                }
                                if (success && Main.rand.NextBool())
                                {
                                    soulsAbsorbed += 1;
                                }
                            }
                        }
                    }
                }
            }*/
        }

        private bool ReplaceByType(Tile tile)
        {
            if (TileID.Sets.Conversion.Sand[tile.TileType])
            {
                tile.TileType = TileID.Sand;
                return true;
            }

            if (TileID.Sets.Conversion.Ice[tile.TileType])
            {
                tile.TileType = TileID.IceBlock;
                return true;
            }

            if (TileID.Sets.Conversion.Dirt[tile.TileType])
            {
                tile.TileType = TileID.Dirt;
                return true;
            }

            if (TileID.Sets.Conversion.Snow[tile.TileType])
            {
                tile.TileType = TileID.SnowBlock;
                return true;
            }

            if (TileID.Sets.Conversion.JungleGrass[tile.TileType])
            {
                tile.TileType = TileID.Mud;
                return true;
            }

            if (TileID.Sets.Conversion.MushroomGrass[tile.TileType])
            {
                tile.TileType = TileID.Mud;
                return true;
            }

            if (TileID.Sets.Conversion.GolfGrass[tile.TileType])
            {
                tile.TileType = TileID.Dirt;
                return true;
            }

            if (TileID.Sets.Conversion.Moss[tile.TileType])
            {
                tile.TileType = TileID.Stone;
                return true;
            }

            if (tile.TileType == TileID.JungleGrass)
            {
                tile.TileType = TileID.Mud;
                return true;
            }

            switch(tile.TileType)
            {
                case TileID.Grass:
                case TileID.HallowedGrass:
                case TileID.CorruptGrass:
                case TileID.CrimsonGrass:
                    tile.TileType = TileID.Dirt;
                    return true;
                case TileID.JungleGrass:
                case TileID.MushroomGrass:
                case TileID.CorruptJungleGrass:
                case TileID.CrimsonJungleGrass:
                    tile.TileType = TileID.Mud;
                    return true;
                case TileID.Crimsand:
                case TileID.Ebonsand:
                case TileID.Pearlsand:
                    tile.TileType = TileID.Sand;
                    return true;
                case TileID.Ebonstone:
                case TileID.Crimstone:
                case TileID.Pearlstone:
                case TileID.ArgonMoss:
                case TileID.BlueMoss:
                case TileID.BrownMoss:
                case TileID.GreenMoss:
                case TileID.KryptonMoss:
                case TileID.LavaMoss:
                case TileID.LongMoss:
                case TileID.PurpleMoss:
                case TileID.RainbowMoss:
                case TileID.RedMoss:
                case TileID.VioletMoss:
                case TileID.XenonMoss:
                    tile.TileType = TileID.Stone;
                    return true;
            }

            if (tile.WallType == 0) return false;

            if (WallID.Sets.Conversion.Sandstone[tile.WallType])
            {
                tile.WallType = WallID.Sandstone;
                return true;
            }

            if (WallID.Sets.Conversion.Ice[tile.WallType])
            {
                tile.WallType = TileID.IceBlock;
                return true;
            }

            if (WallID.Sets.Conversion.Dirt[tile.WallType])
            {
                tile.WallType = WallID.Dirt;
                return true;
            }

            if (WallID.Sets.Conversion.Snow[tile.WallType])
            {
                tile.WallType = WallID.SnowBrick;
                return true;
            }

            if (WallID.Sets.Conversion.Grass[tile.WallType])
            {
                tile.WallType = WallID.Dirt;
                return true;
            }

            if (WallID.Sets.Conversion.Stone[tile.WallType])
            {
                tile.WallType = WallID.Stone;
                return true;
            }

            if (WallID.Sets.Conversion.HardenedSand[tile.WallType])
            {
                tile.WallType = WallID.HardenedSand;
                return true;
            }

            if (WallID.Sets.Conversion.PureSand[tile.WallType])
            {
                tile.WallType = WallID.Sandstone;
                return true;
            }

            switch (tile.WallType)
            {
                case WallID.Grass:
                    tile.WallType = WallID.Dirt;
                    return true;
                case WallID.CorruptGrassEcho:
                    tile.WallType = WallID.Dirt;
                    return true;
                case WallID.CrimsonGrassEcho:
                    tile.WallType = WallID.Dirt;
                    return true;
            }
            return false;
        }

        private bool KillByType(Tile tile, int i, int k)
        {
            if (TileID.Sets.Conversion.Thorn[tile.TileType])
            {
                WorldGen.KillTile(i, k, noItem: true);
                return true;
            }

            switch (tile.TileType)
            {
                case TileID.LivingWood:
                case TileID.LivingMahoganyLeaves:
                case TileID.LivingMahogany:
                case TileID.PalmTree:
                case TileID.TreeAmber:
                case TileID.TreeAmethyst:
                case TileID.TreeAsh:
                case TileID.TreeDiamond:
                case TileID.TreeEmerald:
                case TileID.TreeRuby:
                case TileID.Trees:
                case TileID.TreeSapphire:
                case TileID.TreeTopaz:
                case TileID.ChristmasTree:
                case TileID.MushroomTrees:
                case TileID.PineTree:
                case TileID.VanityTreeSakura:
                case TileID.VanityTreeSakuraSaplings:
                case TileID.VanityTreeWillowSaplings:
                case TileID.VanityTreeYellowWillow:
                case TileID.BloomingHerbs:
                case TileID.ImmatureHerbs:
                case TileID.MatureHerbs:
                case TileID.Bamboo:
                case TileID.LeafBlock:
                case TileID.AbigailsFlower:
                case TileID.PlantDetritus:
                case TileID.PlantDetritus2x2Echo:
                case TileID.PlantDetritus3x2Echo:
                case TileID.PlanteraBulb:
                case TileID.PlanteraThorns:
                case TileID.PlanterBox:
                case TileID.Plants:
                case TileID.Plants2:
                case TileID.AshPlants:
                case TileID.CorruptPlants:
                case TileID.CrimsonPlants:
                case TileID.DyePlants:
                case TileID.HallowedPlants:
                case TileID.HallowedPlants2:
                case TileID.JunglePlants:
                case TileID.JunglePlants2:
                case TileID.MushroomPlants:
                case TileID.OasisPlants:
                case TileID.PottedCrystalPlants:
                case TileID.PottedLavaPlants:
                case TileID.PottedLavaPlantTendrils:
                case TileID.PottedPlants1:
                case TileID.PottedPlants2:
                case TileID.SeaweedPlanter:
                case TileID.LifeFruit:
                    WorldGen.KillTile(i, k, noItem: true);
                    return true;
            }

            if (tile.WallType == 0) return false;
            switch(tile.WallType)
            {
                case WallID.LivingLeaf:
                case WallID.LivingWood:
                case WallID.LivingWoodUnsafe:
                case WallID.BambooBlockWall:
                case WallID.BambooFence:
                case WallID.LargeBambooBlockWall:
                    WorldGen.KillWall(i, k);
                    return true;
            }
            return false;
        }

        private float CheckCircle(Vector2 origin, Vector2 target)
        {
            float deltaX = (float)Math.Pow(target.X - origin.X, 2);
            float deltaY = (float)Math.Pow(target.Y - origin.Y, 2);
            return (float)Math.Sqrt(deltaX + deltaY);
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int PilesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Water Plants"));

            if (PilesIndex != -1)
            {
                tasks.Insert(PilesIndex + 1, new HarujionGenPass("Harujion Flower Generation", 100f));
            }
        }
    }

    public class HarujionGenPass : GenPass
    {
        public HarujionGenPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Cursing A Daisy";

            bool success = false;
            int attempts = 0;

            while (!success)
            {
                attempts++;
                if (attempts > 1000)
                {
                    ModContent.GetInstance<lenen>().Logger.Info($"Failed adding Harujion after {attempts} attempts");
                    break;
                }

                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh);

                Tile tile = Framing.GetTileSafely(x, y);

                WorldGen.PlaceTile(x, y, ModContent.TileType<HarujionSapling>(), true);

                success = Main.tile[x, y].TileType == ModContent.TileType<HarujionSapling>();
                if (success)
                {
                    ModContent.GetInstance<lenen>().Logger.Info($"Added Harujion at {attempts} attempts");
                    ModContent.GetInstance<lenen>().Logger.Info($"Harujion at tile coordinates " +
                        $"X:{x} and Y: {y}");
                }
            }
        }
    }
}
