using lenen.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using lenen.Common.Players.Barriers;

namespace lenen.Common.UI
{
    public class BarrierBar : UIState
    {
        private UIElement area;
        private List<UIImage> barFrames = new List<UIImage>();
        private List<UIImage> barIcons = new List<UIImage>();

        public override void OnInitialize()
        {
            if (Main.dedServ) return;

            area = new UIElement();
            area.Left.Set(10, 0f);
            area.Top.Set(-140, 1f);
            area.Width.Set(Main.ScreenSize.X, 0f);
            area.Height.Set(130, 0f);

            int index = 0;
            barFrames.Clear();
            barIcons.Clear();
            foreach (Barrier barrier in PlayerBarrier.BarrierTemplates)
            {
                barFrames.Add(new UIImage(ModContent.Request<Texture2D>("lenen/Common/UI/BarrierBarFrame")));
                barFrames[index].Top.Set(0, 0f);
                barFrames[index].Width.Set(26, 0f);
                barFrames[index].Height.Set(130, 0f);

                barIcons.Add(new UIImage(ModContent.Request<Texture2D>("lenen/Assets/Icons/BarrierIcon")));
                barIcons[index].Left.Set(4, 0f);
                barIcons[index].Top.Set(114, 0f);
                barIcons[index].Width.Set(12, 0f);
                barIcons[index].Height.Set(12, 0f);

                index++;
            }

            Append(area);
        }

        public override void OnActivate()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.dedServ) return;

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.dedServ) return;

            PlayerBarrier manager = Main.LocalPlayer.GetModPlayer<PlayerBarrier>();

            int index = 0;
            int barrierCount = 0;
            foreach (Barrier barrier in manager.barriers.Values)
            {
                UIImage bar = barFrames[index];
                UIImage icon = barIcons[index];

                if (!barrier.Active)
                {
                    bar.Remove();
                    icon.Remove();
                    index++;
                    continue;
                }

                bar.Left.Set(30 * barrierCount, 0);
                icon.Left.Set(4 + (30 * barrierCount), 0);

                area.Append(bar);
                area.Append(icon);

                icon.SetImage(PlayerBarrier.barrierIcons[barrier.IconTextureIndex]);

                Rectangle hitbox = bar.GetInnerDimensions().ToRectangle();
                hitbox.X += 6;
                hitbox.Width = 8;
                hitbox.Y += 6;
                hitbox.Height = 100;

                float percent = (float)barrier.Life / (float)barrier.MaxLife;
                percent = float.Clamp(percent, 0f, 1f);

                Rectangle hitbox2 = bar.GetInnerDimensions().ToRectangle();
                hitbox2.X += 20;
                hitbox2.Width = 2;
                hitbox2.Y += 12;
                hitbox2.Height = 88;

                float percent2 = (float)(barrier.MaxRecovery - barrier.Recovery) / (float)barrier.MaxRecovery;
                Color gradiantA;
                Color gradiantB;
                if (barrier.Broken)
                {
                    gradiantA = new Color(222, 3, 6);
                    gradiantB = new Color(167, 1, 5);
                    percent2 = (float)(barrier.MaxFullRecovery - barrier.Recovery) / (float)barrier.MaxFullRecovery;
                } else
                {
                    gradiantA = new Color(3, 217, 213);
                    gradiantB = new Color(3, 217, 213);
                }
                percent2 = float.Clamp(percent2, 0f, 1f);
                if (barrier.Life >= barrier.MaxLife) percent2 = 0;

                int bottom = hitbox.Bottom;
                int top = hitbox.Top;
                int steps = (int)((bottom - top) *percent);

                for (int i = 0; i < steps; i++)
                {
                    float gradient = (float)i / (bottom - top);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                        new Rectangle(hitbox.X, bottom - i, hitbox.Width, 1),
                        Color.Lerp(barrier.BottomColor, barrier.TopColor, gradient));
                }

                bottom = hitbox2.Bottom;
                top = hitbox2.Top;
                steps = (int)((bottom - top) * percent2);

                for (int i = 0; i < steps; i++)
                {
                    float gradient = (float)i / (bottom - top);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                        new Rectangle(hitbox2.X, bottom - i, hitbox2.Width, 1),
                        Color.Lerp(gradiantA, gradiantB, gradient));
                }
                index++;
                barrierCount++;
            }

            if (barrierCount <= 0) return;

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}
