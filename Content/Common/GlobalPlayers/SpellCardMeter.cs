using lenen.Content.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace lenen.Content.Common.GlobalPlayers
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
                float percent = (manager.spellCardTimer+0.0001f) /manager.maxSinceZero;
                Rectangle bounds = new Rectangle((int)(player.Center.X - Main.screenPosition.X - 14),
                    (int)(25 + player.Center.Y - Main.screenPosition.Y), 
                    (int)(28*percent), 
                    7);

                drawInfo.DrawDataCache.Add(new DrawData(
                    TextureAssets.MagicPixel.Value,
                    bounds, 
                    new Color(183, 113, 34)
                ));
            }
        }
    }
}
