using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using lenen.Common.Players;
using Terraria.GameContent;
using System.Linq;
using lenen.Content.NPCs.Fairy;
using Terraria.Localization;

namespace lenen.Common.UI
{

    public class SpiritsJar : UIState
    {
        private UIElement area;
        private UIImage jarFrame;
        private UIText spiritsText;
        private Color[] colors = new Color[] { new Color(0.8f, 0.05f, 0.1f), new Color(0.15f, 0.8f, 0.1f),
            new Color(0.05f, 0.0f, 0.7f), new Color(0.85f, 0.05f, 0.9f), new Color(1.0f, 1.0f, 0.0f),
            new Color(1.0f, 0.75f, 0.0f), new Color(0.1f, 0.625f, 1.0f), new Color(0.65f, 0.0f, 0.65f),
            new Color(0.65f, 0.8f, 0.65f), new Color(0.875f, 0.3f, 0.38f), new Color(0.22f, 0.3f, 0.38f),
            new Color(0.25f, 0.55f, 0.45f), new Color(0.8f, 0.55f, 0.45f)
        };
        public bool hidden = false;

        public override void OnInitialize()
        {
            area = new UIElement();
            area.Width.Set(100, 0f);
            area.Height.Set(100, 0f);
            area.Left.Set(-180, 1f);
            area.Top.Set(Main.ScreenSize.Y - 120, 0f);

            jarFrame = new UIImage(ModContent.Request<Texture2D>("lenen/Common/UI/SpiritsJar"));
            jarFrame.Left.Set(0, 0f);
            jarFrame.Top.Set(0, 0f);
            jarFrame.Width.Set(100, 0f);
            jarFrame.Height.Set(100, 0f);

            spiritsText = new UIText("Spirits: 0", 0.8f);
            spiritsText.Left.Set(80, 0f);
            spiritsText.Top.Set(100, 0f);
            spiritsText.Width.Set(16, 0f);
            spiritsText.Height.Set(28, 0f);

            colors = colors.OrderBy(x => Random.Shared.Next()).ToArray();

            area.Append(jarFrame);
            area.Append(spiritsText);
            Append(area);
        }

        public override void OnDeactivate()
        {
            hidden = true;
            jarFrame.Remove();
            spiritsText.Remove();
        }

        public override void OnActivate()
        {
            hidden = false;
            area.Append(jarFrame);
            area.Append(spiritsText);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hidden) return;
            SoulAbsorptionPlayer manager = Main.LocalPlayer.GetModPlayer<SoulAbsorptionPlayer>();
            int soulsUsed = manager.soulsCollected;
            spiritsText.SetText(Language.GetTextValue($"Mods.lenen.UI.Spirits", soulsUsed));
            int divisions = (int)(soulsUsed / 1000);
            int divisionsOfDivisions = (int)(divisions / colors.Length);
            soulsUsed -= (int)(divisions*1000);
            float percent = (float)(soulsUsed) / 1000f;
            percent = Utils.Clamp(percent, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.
            int currentColor = divisions - (divisionsOfDivisions*colors.Length);

            // Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
            Rectangle hitbox = jarFrame.GetInnerDimensions().ToRectangle();
            hitbox.X -= 0;
            hitbox.Width -= 4;
            hitbox.Y -= 0;
            hitbox.Height -= 4;

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int bottom = hitbox.Bottom;
            int top = hitbox.Top;
            int left = hitbox.Center.X;
            int width = 0;
            int steps = (int)(Math.Abs(top - bottom)*percent);
            int reverseSteps = (int)(Math.Abs(top - bottom) * (1-percent));
            double powOf05 = Math.Pow(0.5, 2);
            int offset = left - 44;

            if (divisions > 0)
            {
                int previousColor = currentColor - 1;
                if (previousColor < 0) previousColor = colors.Length - 1;
                for (int i = 0; i < reverseSteps; i++)
                {
                    float x = (float)(i + 1) / (float)reverseSteps * (1 - percent);
                    width = (int)(2 * hitbox.Width * Math.Sqrt(-Math.Pow(x - 0.5, 2) + powOf05));//(hitbox.Width * (Math.Sin((float)i / (float)steps * (MathHelper.Pi) * percent * 0.5)+1));
                    int displacement = (hitbox.Width - width) / 2;
                    float gradient = (float)i / (float)(reverseSteps);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                        new Rectangle(offset + displacement, top + i + 2, width, 1),
                        colors[previousColor] * 0.35f);
                }
            }

            for (int i = 0; i < steps; i++)
            {
                float x = (float)(i+1) / (float)steps * percent;
                width = (int)(2 * hitbox.Width*Math.Sqrt(-Math.Pow(x - 0.5, 2) + powOf05));//(hitbox.Width * (Math.Sin((float)i / (float)steps * (MathHelper.Pi) * percent * 0.5)+1));
                int displacement = (hitbox.Width - width)/2;
                float gradient = (float)i / (float)(steps);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                    new Rectangle(offset + displacement, bottom - i, width, 1),
                    colors[currentColor] * 0.675f);
            }
            
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}
