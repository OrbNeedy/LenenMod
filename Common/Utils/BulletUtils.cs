using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.Utils
{
    public static class BulletUtils
    {
        public static int hFrames = 8;
        public static int vFrames = 10;
        public static Asset<Texture2D> spriteSheet;
        public static Asset<Texture2D> laserSheet;
        public static Dictionary<Sheet, string> texturePaths = new () {
            [Sheet.Default] = "lenen/Content/Projectiles/BulletHellProjectiles/BasicBullet", 
            [Sheet.Big] = "lenen/Content/Projectiles/BulletHellProjectiles/BigBullet", 
            [Sheet.Double] = "lenen/Content/Projectiles/BulletHellProjectiles/DoubleBullet", 
            [Sheet.Pellet] = "lenen/Content/Projectiles/BulletHellProjectiles/PelletBullet", 
            [Sheet.Small] = "lenen/Content/Projectiles/BulletHellProjectiles/SmallBullet", 
            [Sheet.Ofuda] = "lenen/Content/Projectiles/BulletHellProjectiles/OfudaBullet", 
            [Sheet.Bullet] = "lenen/Content/Projectiles/BulletHellProjectiles/BulletBullet",
            [Sheet.Knife] = "lenen/Content/Projectiles/BulletHellProjectiles/KnifeBullet",
            [Sheet.ReverseDefault] = "lenen/Content/Projectiles/BulletHellProjectiles/ReverseBullet",
            [Sheet.ReverseBig] = "lenen/Content/Projectiles/BulletHellProjectiles/BigReverse"
        };
        public static string laserTexturePath = "lenen/Content/Projectiles/BulletHellProjectiles/Laser";

        public static void LoadTexture()
        {
            spriteSheet = ModContent.Request<Texture2D>(
                "lenen/Content/Projectiles/BulletHellProjectiles/Bullets");
            laserSheet = ModContent.Request<Texture2D>(
                "lenen/Content/Projectiles/BulletHellProjectiles/BigLaser");
        }

        public static DrawData GetTexture(int color, int type)
        {
            if (!spriteSheet.IsLoaded)
            {
                // Main.NewText("Forced to load after setup");
                LoadTexture();
            }

            Rectangle source = spriteSheet.Frame(hFrames, vFrames, color, type);

            DrawData data = new(
                spriteSheet.Value, 
                Vector2.Zero,
                source, 
                Color.White, 
                0,
                source.Size() / 2f, 
                Vector2.One, 
                SpriteEffects.None
                );

            // Main.NewText($"Size: {source}");

            return data;
        }

        public static Vector2 GetSize()
        {
            return spriteSheet.Frame(hFrames, vFrames, 0, 0).Size();
        }

        public static DrawData GetLaserTexture(int color)
        {
            if (!laserSheet.IsLoaded)
            {
                // Main.NewText("Forced to load after setup");
                LoadTexture();
            }

            Rectangle source = laserSheet.Frame(8, 1, color, 0);

            DrawData data = new(
                laserSheet.Value,
                Vector2.Zero,
                source,
                Color.White,
                0,
                new(source.Width / 2f, 50),
                Vector2.One,
                SpriteEffects.None
                );

            return data;
        }

        public static int GetRandomColor(SheetFrame[] colors)
        {
            return (int)Main._rand.NextFromList(colors);
        }

        public static int GetRandomColor(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return (int)Main._rand.NextFromList(SheetFrame.Blue, SheetFrame.Cyan);
                default:
                case 1:
                    return (int)Main.rand.NextFromList(SheetFrame.Blue, SheetFrame.Cyan, SheetFrame.Green);
                case 2:
                    return (int)Main.rand.NextFromList(SheetFrame.Green, SheetFrame.Yellow, SheetFrame.Red);
                case 3:
                    return (int)Main.rand.NextFromList(SheetFrame.Pink, SheetFrame.Red);
            }
        }

        public static bool OwnerCheck(int ownerWhoamI)
        {
            if (ownerWhoamI == -1) return false;

            Player owner = Main.player[ownerWhoamI];

            return owner.active && !owner.DeadOrGhost;
        }

        public static Vector2 WarpAroundVector2(Vector2 point, Vector2 min, Vector2 max)
        {
            // IOU a warp around function
            return point;
        }

        public static Rectangle GetHitboxFromLine(Vector2 point1, Vector2 point2, int fluff = 8)
        {
            int upperBound = (int)float.Min(point1.Y, point2.Y) - fluff;
            int leftmostBound = (int)float.Min(point1.X, point2.X) - fluff;
            int lowerBound = (int)float.Max(point1.Y, point2.Y) + fluff;
            int rightmostBound = (int)float.Max(point1.X, point2.X) + fluff;

            return new Rectangle(leftmostBound, upperBound, rightmostBound, lowerBound);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetWhoamI"></param>
        /// <param name="currentWhoamI"></param>
        /// <returns>True to cancel hit.</returns>
        public static bool DisableInitialHit(int targetWhoamI, int currentWhoamI)
        {
            return targetWhoamI == currentWhoamI;
        }

        public static bool LineCollision(Rectangle target, Projectile self, 
            Vector2 start, Vector2 end, float width, out float collisionPoint)
        {
            collisionPoint = 0;
            return Collision.CheckAABBvLineCollision(target.TopLeft(), target.Size(),
                self.Center + start, self.Center + end, width, ref collisionPoint);
        }
    }
}
