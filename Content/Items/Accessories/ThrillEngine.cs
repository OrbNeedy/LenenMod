using lenen.Common.Graphics;
using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace lenen.Content.Items.Accessories
{
    public class ThrillEngine : ModItem
    {
        public float glowTimer = 0;
        public Color glowColor = Color.White;
        public Color glassColor = Color.White;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 40;
            Item.accessory = true;

            Item.rare = ItemRarityID.Master;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ThrillPlayer>().gainThrill = true;
            if (player.GetModPlayer<ThrillPlayer>().percent >= 1)
            {
                glowTimer += 0.04f;
                float sine = (float)(Math.Sin(glowTimer) + 1) * 0.5f;
                glowColor = new Color(1f, sine, sine);
                glassColor = new Color(1f, 1f, 1f, 0.5f);
            }
            else
            {
                glowColor = new Color(1f, 1f, 1f, 0f);
                glassColor = new Color(1f, 1f, 1f, 0f);
            }
        }

        public override void PostUpdate()
        {
            glowColor = new Color(1f, 1f, 1f, 0f);
        }

        public override void UpdateInventory(Player player)
        {
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D engine = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngine").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngineGlowmask").Value;
            Texture2D bigGlow = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngineGlowmask2").Value;
            Texture2D glass = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngineGlowmask2Glass").Value;

            // Engine
            spriteBatch.Draw(
                engine,
                position,
                frame,
                drawColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
            // Glowmask
            spriteBatch.Draw(
                glowmask,
                position,
                frame,
                drawColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
            // Primary glow
            spriteBatch.Draw(
                bigGlow,
                position,
                frame,
                glowColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
            // Glass
            spriteBatch.Draw(
                glass,
                position,
                frame,
                glassColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D engine = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngine").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngineGlowmask").Value;
            Texture2D bigGlow = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngineGlowmask2").Value;
            Texture2D glass = ModContent.Request<Texture2D>("lenen/Content/Items/Accessories/ThrillEngineGlowmask2Glass").Value;
            
            // Engine
            spriteBatch.Draw(
                engine,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - engine.Height * 0.5f
                ),
                new Rectangle(0, 0, engine.Width, engine.Height),
                Lighting.GetColor(Item.Center.ToTileCoordinates(), Color.White),
                rotation,
                engine.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
            // Glowmask
            spriteBatch.Draw(
                glowmask,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - glowmask.Height * 0.5f
                ),
                new Rectangle(0, 0, glowmask.Width, glowmask.Height),
                Lighting.GetColor(Item.Center.ToTileCoordinates(), Color.White),
                rotation,
                glowmask.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
