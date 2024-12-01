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
using System;

namespace lenen.Common.UI
{
    public class BarrierBar : UIState
    {
        private UIElement area;
        private List<UIImage> barFrames = new List<UIImage>();
        private List<UIImage> barIcons = new List<UIImage>();

        public override void OnInitialize()
        {
            area = new UIElement();
            area.Left.Set(-Main.ScreenSize.X + 10, 1f);
            area.Top.Set(Main.ScreenSize.Y - 140, 0f);
            area.Width.Set(Main.ScreenSize.X, 0f);
            area.Height.Set(130, 0f);

            int index = 0;
            barFrames.Clear();
            barIcons.Clear();
            foreach (BarrierLookups.Barriers barrierKey in Enum.GetValues(typeof(BarrierLookups.Barriers)))
            {
                Barrier barrier = BarrierLookups.BarrierDictionary[barrierKey];
                barFrames.Add(new UIImage(ModContent.Request<Texture2D>("lenen/Common/UI/BarrierBarFrame")));
                barFrames[index].Top.Set(0, 0f);
                barFrames[index].Width.Set(26, 0f);
                barFrames[index].Height.Set(130, 0f);

                barIcons.Add(new UIImage(ModContent.Request<Texture2D>(barrier.IconPath())));
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
            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            PlayerBarrier manager = Main.LocalPlayer.GetModPlayer<PlayerBarrier>();

            int index = 0;
            int barrierCount = 0;
            foreach (BarrierLookups.Barriers barrierKey in Enum.GetValues(typeof(BarrierLookups.Barriers)))
            {

                UIImage bar = barFrames[index];
                UIImage icon = barIcons[index];
                //Main.NewText("Bar " + index + " Stats:");
                //Main.NewText("Position X: " + bar.Left.Pixels);
                //Main.NewText("Position Y: " + bar.Top.Pixels);
                /*
                if (index >= manager.barriers.Count)
                {
                    Main.NewText("Returning because fuck you that's why");
                    bar.Remove();
                    icon.Remove();
                    return;
                }*/
                Barrier barrier = BarrierLookups.BarrierDictionary[barrierKey];
                // If the player doesn't have this barrier, increase index, remove the bar and icon, and skip
                if (barrier.State == 0)
                {
                    //Main.NewText("Barrier not available");
                    //Main.NewText("Player doesn't have barrier " + barrier);
                    bar.Remove();
                    icon.Remove();
                    index++;
                    continue;
                }
                //Main.NewText("Stats for bar witn index " + index);
                //Main.NewText("Barrier " + barrier.GetType() + " stats");

                bar.Left.Set(30 * barrierCount, 0);
                icon.Left.Set(4 + (30 * barrierCount), 0);

                area.Append(bar);
                area.Append(icon);
                //Main.NewText("Bar added for barrier " + barrier);
                //Main.NewText("Icon added: " + IconPath[barrier]);

                // Barrier bar hitbox
                Rectangle hitbox = bar.GetInnerDimensions().ToRectangle();
                hitbox.X += 6;
                hitbox.Width = 8;
                hitbox.Y += 6;
                hitbox.Height = 100;

                float percent = (float)barrier.Life / (float)barrier.MaxLife;
                percent = Utils.Clamp(percent, 0f, 1f);
                /*Main.NewText("Life: " + barrier.Life);
                Main.NewText("Max life: " + barrier.MaxLife);
                Main.NewText("Life percent: " + percent);*/

                // Recovery hitbox
                Rectangle hitbox2 = bar.GetInnerDimensions().ToRectangle();
                hitbox2.X += 20;
                hitbox2.Width = 2;
                hitbox2.Y += 12;
                hitbox2.Height = 88;

                float percent2;
                Color gradiantA;
                Color gradiantB;
                if (barrier.Cooldown > 0)
                {
                    gradiantA = new Color(222, 3, 6);
                    gradiantB = new Color(167, 1, 5);
                    percent2 = (float)(barrier.MaxCooldown - barrier.Cooldown) / (float)barrier.MaxCooldown;
                } else
                {
                    gradiantA = new Color(3, 217, 213);
                    gradiantB = new Color(3, 217, 213);
                    percent2 = (float)(barrier.MaxRecovery - barrier.Recovery) / (float)barrier.MaxRecovery;
                }
                percent2 = Utils.Clamp(percent2, 0f, 1f);
                if (barrier.Life >= barrier.MaxLife) percent2 = 0;
                /*Main.NewText("Cooldown: " + barrier.Cooldown);
                Main.NewText("Recovery: " + barrier.Recovery;
                Main.NewText("Max cooldown: " + barrier.MaxCooldown);
                Main.NewText("Max Recovery: " + barrier.MaxRecovery);
                Main.NewText("Current recovery percent: " + percent2);
                Main.NewText("Max recovery - recovery: " + (barrier.MaxRecovery - barrier.Recovery));*/

                int bottom = hitbox.Bottom;
                int top = hitbox.Top;
                int steps = (int)((bottom - top) *percent);

                for (int i = 0; i < steps; i++)
                {
                    float gradient = (float)i / (bottom - top);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                        new Rectangle(hitbox.X, bottom - i, hitbox.Width, 1),
                        Color.Lerp(barrier.Colors[0], barrier.Colors[2], gradient));
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

        // Here we draw our UI
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }

        /*public void InitializeBarrierAssets()
        {
            PlayerBarrier player = Main.LocalPlayer.GetModPlayer<PlayerBarrier>();
            int index = 0;
            barFrames.Clear();
            barIcons.Clear();
            foreach (var barrier in player.barriers)
            {
                barFrames.Add(new UIImage(ModContent.Request<Texture2D>("lenen/Common/UI/BarrierBarFrame")));
                barFrames[index].Top.Set(0, 0f);
                barFrames[index].Width.Set(26, 0f);
                barFrames[index].Height.Set(130, 0f);

                barIcons.Add(new UIImage(ModContent.Request<Texture2D>(barrier.IconPath)));
                barIcons[index].Left.Set(4, 0f);
                barIcons[index].Top.Set(114, 0f);
                barIcons[index].Width.Set(12, 0f);
                barIcons[index].Height.Set(12, 0f);

                index++;
            }
        }*/
    }
}
