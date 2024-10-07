using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using lenen.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using lenen.Common.Players;

namespace lenen.Common.GlobalPlayers
{
    public class FrontRibsDraw : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.IceBarrier, PlayerDrawLayers.CaptureTheGem);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                Player player = drawInfo.drawPlayer;
                if (!player.GetModPlayer<SoulAbsorptionPlayer>().revivedState) return;
                Asset<Texture2D> sprite = ModContent.Request<Texture2D>("lenen/Assets/Textures/GashadokuroRibsFront");

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
