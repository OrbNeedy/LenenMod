using lenen.Common.Players;
using lenen.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.GlobalPlayers
{
    public class TrackingLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                if (!drawInfo.drawPlayer.GetModPlayer<TrackingPlayer>().trackingHarujion) return;
                Texture2D texture = ModContent.Request<Texture2D>("lenen/Assets/Icons/NoHarujionIcon").Value;

                Vector2 offset = new Vector2(0, -40);
                /*float distanceToHarujion = ModContent.GetInstance<HarujionLocations>().
                    harujionLocation.ToWorldCoordinates().Distance(drawInfo.Center) / 
                    ModContent.GetInstance<HarujionLocations>().GetRadius();*/
                if (ModContent.GetInstance<HarujionLocations>().harujionLocation == Point16.Zero)
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                        texture,
                        drawInfo.Position + offset - Main.screenPosition,
                        null,
                        new Color(1f, 1f, 1f),
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None
                    ));
                    return;
                }

                texture = ModContent.Request<Texture2D>("lenen/Assets/Icons/PointToHarujion").Value;
                float rotationToHarujion = drawInfo.Center.AngleTo(ModContent.
                    GetInstance<HarujionLocations>().harujionLocation.ToWorldCoordinates());

                drawInfo.DrawDataCache.Add(new DrawData(
                    texture,
                    drawInfo.Center + offset - Main.screenPosition,
                    null,
                    new Color(1f, 1f, 1f),
                    rotationToHarujion,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None
                ));
            }
        }
    }
}
