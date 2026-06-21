using lenen.Content.Items.Weapons.Yabusame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.GlobalPlayers
{
    public class OptionsDrawingBack : PlayerDrawLayer
    {
        private int spriteIndex = 0;
        private int spriteTimer = 0;
        public static Asset<Texture2D> DimensionalOrbsOptions;

        public override void Load()
        {
            DimensionalOrbsOptions = ModContent.Request<Texture2D>("lenen/Assets/Textures/DimensionalOption");
        }

        public override void Unload()
        {
            DimensionalOrbsOptions = null;
        }

        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.JimsCloak, PlayerDrawLayers.MountBack);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.DeadOrGhost) return;
            if (drawInfo.shadow == 0f)
            {
                int item = player.HeldItem.type;

                if (item == ModContent.ItemType<DimensionalOrbs>())
                {
                    Vector2 orbit = new Vector2(35f, 0).RotatedBy(Math.PI + Main.GameUpdateCount * MathHelper.Pi / 20);
                    Vector2 offset = orbit * new Vector2(1.2f, 0.4f);

                    if (offset.Y > 0)
                    {
                        offset *= -1;
                    }
                    orbit.Normalize();
                    float darkening = 0.4f - Math.Abs(orbit.Y) * 0.3f;

                    offset = offset.RotatedBy(Math.Cos(Main.GameUpdateCount * MathHelper.Pi / 45) * 0.6);

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

                    Texture2D texture = DimensionalOrbsOptions.Value;
                    Rectangle bounds = texture.Frame(1, 3, 0, spriteIndex);

                    drawInfo.DrawDataCache.Add(new DrawData(
                            texture,
                            drawInfo.Position + offset - Main.screenPosition,
                            bounds,
                            new Color(darkening, darkening, darkening),
                            0f,//Main.GameUpdateCount * 0.5f,
                            bounds.Size() / 2f,
                            1f,
                            SpriteEffects.None
                        ));
                }
            }
        }
    }
}
