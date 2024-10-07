using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.GlobalPlayers
{
    public class BackRibsDraw : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.JimsCloak, PlayerDrawLayers.MountBack);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                Player player = drawInfo.drawPlayer;
                if (!player.GetModPlayer<SoulAbsorptionPlayer>().revivedState) return;
                Asset<Texture2D> sprite = ModContent.Request<Texture2D>("lenen/Assets/Textures/GashadokuroRibs");

                Texture2D texture = sprite.Value;
                Vector2 offset = new Vector2(-30, -34);

                SpriteEffects effects = SpriteEffects.None;
                if (drawInfo.drawPlayer.direction == -1) effects = SpriteEffects.FlipHorizontally;

                drawInfo.DrawDataCache.Add(new DrawData(
                    texture,
                    drawInfo.drawPlayer.MountedCenter + offset - Main.screenPosition,
                    texture.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    effects
                ));
            }
        }
    }
}
