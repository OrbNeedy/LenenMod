using lenen.Common.Players;
using lenen.Content.Items.Weapons.Yabusame;
using lenen.Content.NPCs.TasoukenBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.GlobalPlayers
{
    public class OptionsDrawingFront : PlayerDrawLayer
    {
        private int spriteIndex = 0;
        private int spriteTimer = 0;

        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.DeadOrGhost) return;
            if (drawInfo.shadow != 0f) return;

            if (player.GetModPlayer<BuffPlayer>().CanCut)
            {
                drawInfo.DrawDataCache.Add(
                    new(
                        TasoukenBoss.BossBorder.Value,
                        player.Center - Main.screenPosition,
                        TasoukenBoss.BossBorder.Frame(),
                        Color.LightGoldenrodYellow * 0.3f,
                        0,
                        TasoukenBoss.BossBorder.Size() / 2f,
                        64f / 600f,
                        SpriteEffects.None,
                        1
                    ));
            }

            int item = player.HeldItem.type;

            if (item == ModContent.ItemType<DimensionalOrbs>())
            {
                Vector2 orbit = new Vector2(35f, 0).RotatedBy(Main.GameUpdateCount * MathHelper.Pi / 20);
                Vector2 offset = orbit * new Vector2(1.2f, 0.4f);

                if (offset.Y <= 0)
                {
                    offset *= -1;
                }
                orbit.Normalize();
                float darkening = 0.4f + Math.Abs(orbit.Y) * 0.3f;

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

                Texture2D texture = OptionsDrawingBack.DimensionalOrbsOptions.Value;
                Rectangle bounds = texture.Frame(1, 3, 0, spriteIndex);

                drawInfo.DrawDataCache.Add(new DrawData(
                    texture,
                    drawInfo.Position + offset - Main.screenPosition,
                    bounds,
                    new Color(darkening, darkening, darkening),
                    Main.GameUpdateCount * 0.5f,
                    bounds.Size() / 2f,
                    1f,
                    SpriteEffects.None
                ));
            }

            if (player.GetModPlayer<OptionsManagingPlayer>().GravityAnomaly)
            {
                Asset<Texture2D> well = ModContent.Request<Texture2D>("lenen/Content/Projectiles/GravityPullBulletWithAura");

                drawInfo.DrawDataCache.Add(new DrawData(
                    well.Value,
                    Main.MouseWorld - Main.screenPosition,
                    well.Value.Bounds,
                    Color.White * 0.7f,
                    Main.GameUpdateCount * 0.5f,
                    well.Value.Bounds.Size() * 0.5f,
                    1f,
                    SpriteEffects.None
                ));
            }
        }
    }
}
