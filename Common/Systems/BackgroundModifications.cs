using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.Systems
{
    public class BackgroundModifications : ModSystem
    {
        public static RenderTarget2D backgroundRT;
        public static bool canUseBGRT = false;
        public static bool drawOverBG = false;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() => {
                backgroundRT = new RenderTarget2D(Main.graphics.GraphicsDevice,
                    Main.screenWidth, Main.screenHeight);
            });

            // On_Main.RenderBackground += On_Main_RenderBackground;
            // On_Main.DrawBackground += On_Main_DrawBackground;
            On_Main.GetBackgroundRect += On_Main_GetBackgroundRect;
            On_Main.DrawBG += On_Main_DrawBG;
            Main.OnResolutionChanged += Main_OnResolutionChanged;

            /*MethodInfo backgroundDrawMethod = Main.instance.GetType().GetMethod("DrawBackground",
                BindingFlags.NonPublic | BindingFlags.Instance);
            // DrawBackground() has no parameters
            backgroundDrawMethod.Invoke(Main.instance, []);*/
        }

        private void Main_OnResolutionChanged(Vector2 obj)
        {
            // If resized, create new RT with new resolution
            backgroundRT.Dispose();
            backgroundRT = new RenderTarget2D(Main.graphics.GraphicsDevice,
                    Main.screenWidth, Main.screenHeight);

            // Redraw because it was disposed
            RenderTargetBinding[] oldtargets = Main.graphics.GraphicsDevice.GetRenderTargets();
            // Don't use the render target while it's drawing to it 
            canUseBGRT = false;

            Main.graphics.GraphicsDevice.SetRenderTarget(backgroundRT);
            Main.graphics.GraphicsDevice.Clear(Color.Black);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null,
                Main.GameViewMatrix.EffectMatrix);

            MethodInfo backgroundDrawMethod = Main.instance.GetType().GetMethod("DrawBG",
                BindingFlags.NonPublic | BindingFlags.Instance);
            // DrawBackground() has no parameters
            backgroundDrawMethod.Invoke(Main.instance, []);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets);
            canUseBGRT = true;
        }

        private void On_Main_DrawBG(On_Main.orig_DrawBG orig, Main self)
        {
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;
            /*RenderTargetBinding[] oldtargets = gd.GetRenderTargets();
            // Don't use the render target while it's drawing to it 
            canUseBGRT = false;

            gd.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            SpriteBatchState tempState = SpriteBatchExt.GetState(sb);

            sb.End();

            gd.SetRenderTarget(backgroundRT);
            //gd.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null,
                Main.GameViewMatrix.EffectMatrix);

            orig(self);

            sb.End();

            foreach (RenderTargetBinding rt in oldtargets)
            {
                if (rt.RenderTarget is RenderTarget2D target)
                {
                    target.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                }
            }

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets);

            SpriteBatchExt.Begin(sb, tempState);

            canUseBGRT = true;*/

            //gd.Clear(Color.Transparent);

            if (drawOverBG && canUseBGRT)
            {
                Vector2 offset = new Vector2(0, 200);
                sb.Draw(
                    backgroundRT, 
                    Main.LocalPlayer.Center - Main.screenPosition + offset - new Vector2(backgroundRT.Width / 2, 0), 
                    new Rectangle(0, 0, backgroundRT.Width / 2, backgroundRT.Height),
                    Color.White);

                sb.Draw(
                    backgroundRT,
                    Main.LocalPlayer.Center - Main.screenPosition - offset,
                    new Rectangle(backgroundRT.Width / 2, 0, backgroundRT.Width / 2, backgroundRT.Height),
                    Color.White);

                drawOverBG = false;
            } else
            {
                orig(self);
            }
        }

        public override void PreUpdateWorld()
        {
            RenderTargetBinding[] oldtargets = Main.graphics.GraphicsDevice.GetRenderTargets();
            // Don't use the render target while it's drawing to it 
            canUseBGRT = false;

            Main.graphics.GraphicsDevice.SetRenderTarget(backgroundRT);
            Main.graphics.GraphicsDevice.Clear(Color.Black);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null,
                Main.GameViewMatrix.EffectMatrix);

            MethodInfo backgroundDrawMethod = Main.instance.GetType().GetMethod("DrawBG",
                BindingFlags.NonPublic | BindingFlags.Instance);
            // DrawBackground() has no parameters
            backgroundDrawMethod.Invoke(Main.instance, []);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets);
            canUseBGRT = true;
        }

        private Rectangle? On_Main_GetBackgroundRect(On_Main.orig_GetBackgroundRect orig, Main self, int backgroundTextureIndex)
        {
            Rectangle? backgroundRect = orig(self, backgroundTextureIndex);

            int sum = Main.screenWidth;
            int sum2 = Main.screenHeight;

            return backgroundRect;
        }
    }
}
