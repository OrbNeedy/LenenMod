using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using lenen.Common.Players;

namespace lenen.Common.GlobalPlayers
{
    public class FrontBarrierDrawing : PlayerDrawLayer
    {
        public static Asset<Texture2D> SkullBarrier;
        private int animationFrame = 0;
        private int animationTimer = 0;

        public override void Unload()
        {
            if (SkullBarrier.IsLoaded)
            {
                SkullBarrier = null;
            }
        }

        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            PlayerBarrier playerBarrier = player.GetModPlayer<PlayerBarrier>();
            if (player.DeadOrGhost) return;
            if (drawInfo.shadow == 0f)
            {
                if ((playerBarrier.barriers[BarrierTypes.SkullBarrier].Active && 
                    playerBarrier.barriers[BarrierTypes.SkullBarrier].Life > 0) ||
                    (playerBarrier.barriers[BarrierTypes.SkullBarrier2].Active && 
                    playerBarrier.barriers[BarrierTypes.SkullBarrier2].Life > 0))
                {
                    if (!SkullBarrier.IsLoaded)
                    {
                        SkullBarrier = ModContent.Request<Texture2D>("lenen/Assets/Textures/SkullBarrier");
                    }

                    if (animationFrame <= -1)
                    {
                        animationFrame = 2;
                    }
                    if (animationTimer >= 30)
                    {
                        animationFrame -= 1;
                        animationTimer = 0;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        int animationOffset = animationFrame - i;
                        if (animationOffset <= -1)
                        {
                            animationOffset += 3;
                        }
                        if (animationOffset <= -1)
                        {
                            animationOffset += 3;
                        }

                        Rectangle bounds = SkullBarrier.Frame(1, 3, 0, animationOffset);

                        Vector2 offset = new Vector2(70 - (animationOffset * 8), 0).
                            RotatedBy((Main.GameUpdateCount * 0.08) + (i * MathHelper.TwoPi / 5));
                        Vector2 additionalOffset = new Vector2(0, 25);
                        SpriteEffects effects = SpriteEffects.None;
                        if (player.direction == -1) effects = SpriteEffects.FlipHorizontally;

                        Main.EntitySpriteDraw(
                            SkullBarrier.Value,
                            player.MountedCenter - Main.screenPosition + offset + additionalOffset,
                            bounds,
                            Color.White,
                            0f,
                            bounds.Size() / 2f,
                            1f,
                            effects
                        );
                    }
                    animationTimer += 1;
                }
            }
        }
    }
}
