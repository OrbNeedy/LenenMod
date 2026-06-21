using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using lenen.Common.Players;

namespace lenen.Common.GlobalPlayers
{
    public class FrontRibsDraw : PlayerDrawLayer
    {
        public static Asset<Texture2D> FrontRibs;

        public override void Unload()
        {
            if (FrontRibs.IsLoaded)
            {
                FrontRibs = null;
            }
        }

        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.IceBarrier, PlayerDrawLayers.CaptureTheGem);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.DeadOrGhost) return;
            if (drawInfo.shadow == 0f)
            {
                if (!player.GetModPlayer<SoulAbsorptionPlayer>().revivedState) return;

                if (!FrontRibs.IsLoaded)
                {
                    FrontRibs = ModContent.Request<Texture2D>("lenen/Assets/Textures/GashadokuroRibsFront");
                }

                Texture2D texture = FrontRibs.Value;

                SpriteEffects effects = SpriteEffects.None;
                if (drawInfo.drawPlayer.direction == -1) effects = SpriteEffects.FlipHorizontally;

                drawInfo.DrawDataCache.Add(new DrawData(
                    texture,
                    drawInfo.drawPlayer.MountedCenter - Main.screenPosition,
                    texture.Bounds,
                    Color.White,
                    0f,
                    texture.Size() / 2f,
                    1f,
                    effects
                ));
            }
        }
    }
}
