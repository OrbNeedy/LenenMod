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
        public static Asset<Texture2D> BackRibs;

        public override void Load()
        {
            BackRibs = ModContent.Request<Texture2D>("lenen/Assets/Textures/GashadokuroRibs");
        }

        public override void Unload()
        {
            BackRibs = null;
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
                if (!player.GetModPlayer<SoulAbsorptionPlayer>().revivedState) return;

                Texture2D texture = BackRibs.Value;

                SpriteEffects effects = SpriteEffects.None;
                if (drawInfo.drawPlayer.direction == -1) effects = SpriteEffects.FlipHorizontally;
                
                drawInfo.DrawDataCache.Add(new DrawData(
                    texture,
                    drawInfo.drawPlayer.MountedCenter - Main.screenPosition,
                    texture.Bounds,
                    Color.White,
                    0f,
                    texture.Size(),
                    1f,
                    effects
                ));
            }
        }
    }
}
