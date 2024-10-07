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
        private int animationFrame = 0;
        private int animationTimer = 0;
        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                PlayerBarrier player = drawInfo.drawPlayer.GetModPlayer<PlayerBarrier>();
                if (BarrierLookups.BarrierDictionary[BarrierLookups.Barriers.SkullBarrier].IsAvailable() ||
                    BarrierLookups.BarrierDictionary[BarrierLookups.Barriers.BetterSkullBarrier].IsAvailable())//player.GetBarrier(typeof(SkullBarrier)) != null)
                {
                    Asset<Texture2D> skullTexture = ModContent.
                        Request<Texture2D>("lenen/Content/Items/Accessories/SkullBarrier");

                    if (animationFrame <= -1)
                    {
                        animationFrame = 2;
                    }
                    if (animationTimer >= 30)
                    {
                        animationFrame -= 1;
                        animationTimer = 0;
                    }

                    Rectangle bounds = skullTexture.Value.Bounds;
                    bounds.Height /= 3;

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

                        bounds.Y = bounds.Height * animationOffset;
                        Vector2 offset = new Vector2(70 - (animationOffset * 8), 0).
                            RotatedBy((Main.GameUpdateCount * 0.08) + (i * MathHelper.TwoPi / 5));
                        Vector2 additionalOffset = new Vector2(0, 25);
                        SpriteEffects effects = SpriteEffects.None;
                        if (drawInfo.drawPlayer.direction == -1) effects = SpriteEffects.FlipHorizontally;

                        Main.EntitySpriteDraw(skullTexture.Value,
                            drawInfo.drawPlayer.MountedCenter - Main.screenPosition + offset + additionalOffset,
                            bounds,
                            Color.White,
                            0f,
                            skullTexture.Size()/2,
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
