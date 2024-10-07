using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using lenen.Common.Players;

namespace lenen.Common.UI
{
    public class SpellCardBar : UIState
    {
        private UIElement area;
        private UIImage barFrame;
        private Color gradientA;
        private Color gradientB;

        public override void OnInitialize()
        {
            area = new UIElement();
            area.Width.Set(60, 0f);
            area.Height.Set(18, 0f);
            area.Left.Set(-(Main.ScreenSize.X / 2) - (area.Width.Pixels / 2), 1f);
            area.Top.Set((Main.ScreenSize.Y / 2) + 34, 0f);

            barFrame = new UIImage(ModContent.Request<Texture2D>("lenen/Common/UI/SpellCardFrame")); // Frame of our resource bar
            barFrame.Left.Set(0, 0f);
            barFrame.Top.Set(0, 0f);
            barFrame.Width.Set(60, 0f);
            barFrame.Height.Set(18, 0f);

            gradientA = new Color(180, 80, 0);
            gradientB = new Color(230, 170, 0);

            area.Append(barFrame);
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            //Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.GetModPlayer<SpellCardManagement>().spellCardTimer <= 0)
                return;

            SpellCardManagement manager = Main.LocalPlayer.GetModPlayer<SpellCardManagement>();
            float percent = (manager.spellCardTimer + 0.0001f) / manager.maxSinceZero;
            percent = Utils.Clamp(percent, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.

            // Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
            Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
            hitbox.X += 6;
            hitbox.Width -= 12;
            hitbox.Y += 6;
            hitbox.Height -= 12;

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * percent);
            for (int i = 0; i < steps; i += 1)
            {
                // float percent = (float)i / steps; // Alternate Gradient Approach
                float gradient = (float)i / (right - left);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, 
                    new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), 
                    Color.Lerp(gradientA, gradientB, gradient));
            }

            base.Draw(spriteBatch);
        }

        // Here we draw our UI
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}
