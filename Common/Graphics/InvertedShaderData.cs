using lenen.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace lenen.Common.Graphics
{
    public class InvertedShaderData //: ScreenShaderData
    {
        /*public const string TenkaiFlashbombKey = "DimensionalDeletion";

        public InvertedShaderData(Asset<Effect> shader, string passName) : base(shader, passName)
        {
        }

        public static void ToggleActivityIfNecessary()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            bool shouldBeActive = RenderTargetSystem.AnyActiveInk;

            if (shouldBeActive && !Filters.Scene[$"{TenkaiFlashbombKey}"].IsActive())
            {
                Filters.Scene.Activate($"{TenkaiFlashbombKey}");
            }

            if (!shouldBeActive && Filters.Scene[$"{TenkaiFlashbombKey}"].IsActive())
            {
                Filters.Scene.Deactivate($"{TenkaiFlashbombKey}");
            }
        }

        public override void Update(GameTime gameTime)
        {
            RenderTargetSystem.inkTargetByRequest.Request();

            if (RenderTargetSystem.inkTargetByRequest.IsReady && RenderTargetSystem.insideInkTargetByRequest.IsReady)
            {
                // Read projectile

                Shader.Parameters["uRotation"].SetValue(0);
                
                Shader.Parameters["uRectangleSize"].SetValue(Vector2.Zero);
            }
        }

        public override void Apply()
        {
            var gd = Main.instance.GraphicsDevice;

            gd.SamplerStates[0] = SamplerState.LinearClamp;

            gd.Textures[1] = RenderTargetSystem.inkTargetByRequest.GetTarget();
            gd.SamplerStates[1] = SamplerState.LinearWrap;

            gd.Textures[2] = RenderTargetSystem.insideInkTargetByRequest.GetTarget();
            gd.SamplerStates[2] = SamplerState.LinearWrap;

            base.Apply();

            RenderTargetSystem.InsideInkTargetDrawnToThisFrame = false;
        }*/
    }
}
