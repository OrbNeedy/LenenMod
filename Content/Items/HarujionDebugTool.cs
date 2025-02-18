using lenen.Common.Systems;
using lenen.Content.Projectiles;
using lenen.Content.Tiles.Plants;
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

namespace lenen.Content.Items
{
    class HarujionDebugTool : ModItem
    {
        enum ToolMode
        {
            Create, 
            Grow, 
            Destroy, 
            ForceUpdate
        }

        private ToolMode mode = ToolMode.Create;
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.shoot = ModContent.ProjectileType<HaniwaFist>();
            Item.shootSpeed = 8.5f;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 3.5f;

            Item.width = 54;
            Item.height = 54;
            Item.value = Item.sellPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.LightPurple;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.staff[Type] = false;
            Item.channel = false;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.shootsEveryUse = false;
            Item.useTurn = true;
        }

        public override string Texture => "lenen/Content/Items/LumenDiscFragment_2";

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                switch (mode)
                {
                    case ToolMode.Create:
                        mode = ToolMode.Grow;
                        Main.NewText("Mode changed to Grow.");
                        break;
                    case ToolMode.Grow:
                        mode = ToolMode.Destroy;
                        Main.NewText("Mode changed to Destroy.");
                        break;
                    case ToolMode.Destroy:
                        mode = ToolMode.ForceUpdate;
                        Main.NewText("Mode changed to Update.");
                        break;
                    case ToolMode.ForceUpdate:
                        mode = ToolMode.Create;
                        Main.NewText("Mode changed to Create.");
                        break;
                    default:
                        mode = ToolMode.Create;
                        Main.NewText("Mode changed to Create.");
                        break;
                }
            }
            else
            {
                Point16 pos = Main.MouseWorld.ToTileCoordinates16();
                Tile tile = Framing.GetTileSafely(pos);
                switch (mode)
                {
                    case ToolMode.Create:
                        WorldGen.PlaceTile(pos.X, pos.Y,
                            ModContent.TileType<HarujionSapling>(), false);
                        bool success = Main.tile[pos.X, pos.Y].TileType == ModContent.TileType<HarujionSapling>();
                        ModContent.GetInstance<HarujionLocations>().UpdateHarujion();
                        Main.NewText("Harujion's final location: " + ModContent.GetInstance<HarujionLocations>().harujionLocation);
                        Main.NewText("Current player location: " + player.Center.ToTileCoordinates16());
                        Main.NewText("Was the placing successful? " + success);
                        break;
                    case ToolMode.Grow:
                        if (IsValidHarujion(pos))
                        {
                            switch (HarujionSapling.GetStage(pos.X, pos.Y))
                            {
                                case PlantStage.Planted:
                                    tile.TileFrameX = 18;
                                    break;
                                case PlantStage.Growing:
                                    tile.TileFrameX = 18 * 2;
                                    break;
                                case PlantStage.Grown:
                                    ModContent.GetInstance<HarujionLocations>().GrowTree();
                                    break;
                            }
                        }
                        break;
                    case ToolMode.Destroy:
                        for (int x = pos.X; x < pos.X + 5; x++)
                        {
                            for (int y = pos.Y; y < pos.Y + 11; y++)
                            {
                                if (Framing.GetTileSafely(x, y).HasTile)
                                {
                                    WorldGen.KillTile(x, y, false, false, true);
                                }
                            }
                        }
                        break;
                    case ToolMode.ForceUpdate:
                        //ModContent.GetInstance<HarujionLocations>().UpdateHarujion();

                        WorldGen.PlaceTile(pos.X, pos.Y, ModContent.TileType<HarujionMultitile>(), true);
                        if (Main.tile[pos.X, pos.Y].TileType == ModContent.TileType<HarujionMultitile>())
                        {
                            Main.NewText("Sucess");
                        } else
                        {
                            Main.NewText("Failed");
                        }
                         break;
                }
            }
            return false;
        }

        private bool IsValidHarujion(Point16 pos)
        {
            if (ModContent.GetInstance<HarujionLocations>().harujionLocation == new Point16(0, 0))
            {
                Main.NewText("Harujion doesn't exist, use the Update mode to check again or use the create mode to make a new one.");
                return false;
            }
            if (Framing.GetTileSafely(pos).TileType != ModContent.TileType<HarujionSapling>() ||
                !Framing.GetTileSafely(pos).HasTile)
            {
                Main.NewText("No valid tile in the mouse position.");
                return false;
            }
            if (ModContent.GetInstance<HarujionLocations>().harujionLocation != pos)
            {
                Main.NewText("Harujion is not registered as the real Harujion, delete this tile or use Update mode.");
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
