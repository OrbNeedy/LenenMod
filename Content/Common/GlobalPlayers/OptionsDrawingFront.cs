using lenen.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Common.GlobalPlayers
{
    public class OptionsDrawingFront : PlayerDrawLayer
    {
        private bool reverse = false;
        private int spriteIndex = 0;
        private int spriteTimer = 0;

        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                Player player = drawInfo.drawPlayer;
                int item = player.HeldItem.type;

                if (item == ModContent.ItemType<DimensionalOrbs>())
                {
                    Vector2 orbit = new Vector2(35f, 0).RotatedBy((Main.GameUpdateCount*MathHelper.Pi) / 20);
                    Vector2 offset = orbit * new Vector2(1f, 0.4f);

                    if (offset.Y <= 0)
                    {
                        offset *= -1;
                    }
                    orbit.Normalize();
                    float darkening = 0.4f + (Math.Abs(orbit.Y) * 0.3f);
                    offset.Y += 5 + drawInfo.mountOffSet;
                    offset.X -= 2;

                    int spriteCount = 3;
                    if (++spriteTimer > 3)
                    {
                        spriteTimer = 0;
                        if (++spriteIndex >= spriteCount)
                        {
                            spriteIndex = 0;
                        }
                    }
                    Asset<Texture2D> sprite = ModContent.Request<Texture2D>("lenen/Content/Items/Weapons/DimensionalOption");

                    Texture2D texture = sprite.Value;
                    int height = texture.Bounds.Height / spriteCount;
                    Rectangle bounds = new Rectangle(0, spriteIndex * height, texture.Bounds.Width, height);

                    drawInfo.DrawDataCache.Add(new DrawData(
                            texture,
                            drawInfo.Position + offset - Main.screenPosition,
                            bounds,
                            new Color(darkening, darkening, darkening),
                            0f,//Main.GameUpdateCount * 0.5f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None
                        ));
                }
            }
        }
    }
}
