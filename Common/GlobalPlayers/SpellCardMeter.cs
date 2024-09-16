using lenen.Common.Players;
using Microsoft.Xna.Framework;
using SteelSeries.GameSense;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace lenen.Common.GlobalPlayers
{
    public class SpellCardMeter : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                Player player = drawInfo.drawPlayer;
                SpellCardManagement manager = drawInfo.drawPlayer.GetModPlayer<SpellCardManagement>();
                float percent = (manager.spellCardTimer + 0.0001f) / manager.maxSinceZero;
                Rectangle bounds = new Rectangle((int)(player.Center.X - Main.screenPosition.X - 14),
                    (int)(30 + player.Center.Y - Main.screenPosition.Y),
                    (int)(28 * percent),
                    7);

                drawInfo.DrawDataCache.Add(new DrawData(
                    TextureAssets.MagicPixel.Value,
                    bounds,
                    Color.Lerp(new Color(159, 110, 50), new Color(125, 67, 200), percent)
                ));
            }
        }
    }
}
