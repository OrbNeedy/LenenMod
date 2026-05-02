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
        public static Asset<Texture2D> spriteSheet;
        public static Dictionary<Sheet, string> texturePaths = new () {
            [Sheet.Default] = "lenen/Content/Projectiles/BulletHellProjectiles/BasicBullet", 
            [Sheet.Big] = "lenen/Content/Projectiles/BulletHellProjectiles/BigBullet", 
            [Sheet.Double] = "lenen/Content/Projectiles/BulletHellProjectiles/DoubleBullet", 
            [Sheet.Pellet] = "lenen/Content/Projectiles/BulletHellProjectiles/PelletBullet", 
            [Sheet.Small] = "lenen/Content/Projectiles/BulletHellProjectiles/SmallBullet", 
            [Sheet.Ofuda] = "lenen/Content/Projectiles/BulletHellProjectiles/OfudaBullet", 
            [Sheet.Bullet] = "lenen/Content/Projectiles/BulletHellProjectiles/BulletBullet"
        };

        public static void LoadTexture()
        {
            spriteSheet = ModContent.Request<Texture2D>(
                "lenen/Content/Projectiles/BulletHellProjectiles/Bullets");
        }

        public static DrawData GetTexture(int color, int type)
        {
            if (!spriteSheet.IsLoaded)
            {
                Main.NewText("Forced to load after setup");
                LoadTexture();
            }

            Rectangle source = spriteSheet.Frame(8, 7, color, type);

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
    }
}
