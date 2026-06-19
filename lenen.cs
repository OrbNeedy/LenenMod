using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using lenen.Common.Players;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using lenen.Common.Utils;

namespace lenen
{
	public partial class lenen : Mod
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> curtainShader = this.Assets.Request<Effect>("Effects/RiftEffect");
                Asset<Effect> textureShader = this.Assets.Request<Effect>("Effects/TextureEffect");
                Asset<Effect> haniwaShader = this.Assets.Request<Effect>("Effects/HaniwaTexture");
                Asset<Effect> silouetteShader = this.Assets.Request<Effect>("Effects/SilouetteEffect");
                Asset<Effect> solidShader = this.Assets.Request<Effect>("Effects/SolidColor");
                Asset<Effect> heavyNoiseShader = this.Assets.Request<Effect>("Effects/HeavyNoiseEffect");
                Asset<Effect> rectangleWaveShader = this.Assets.Request<Effect>("Effects/RectangleWaveEffect");
                Asset<Effect> screenInvertedShader = this.Assets.Request<Effect>("Effects/InvertedEffect");
                Asset<Effect> gradientShader = this.Assets.Request<Effect>("Effects/SpellcardGradient");
                Asset<Effect> gradientShader2 = this.Assets.Request<Effect>("Effects/SpellcardGradient");

                GameShaders.Misc["Rift"] = new MiscShaderData(curtainShader, "RiftShader");
                GameShaders.Misc["Harujion"] = new MiscShaderData(textureShader, "TextureShader").
                    UseImage1(Assets.Request<Texture2D>("Assets/Textures/FlowerPattern"));
                GameShaders.Misc["Haniwa"] = new MiscShaderData(haniwaShader, "Texture").
                    UseImage1(Assets.Request<Texture2D>("Assets/Textures/HaniwaTexture"));
                GameShaders.Misc["Silouette"] = new MiscShaderData(silouetteShader, "SilouetteShader").
                    UseColor(0.075f, 0.075f, 0.075f).UseSecondaryColor(0.7f, 0.7f, 0.7f);
                GameShaders.Misc["SolidColor"] = new MiscShaderData(solidShader, "Solid").
                    UseColor(1f, 0.02f, 0f);
                GameShaders.Misc["HeavyNoise"] = new MiscShaderData(heavyNoiseShader, "NoiseShader");
                GameShaders.Misc["RectangleWave"] = new MiscShaderData(rectangleWaveShader, "SquareWaveShader");
                GameShaders.Misc["Inverted"] = new MiscShaderData(screenInvertedShader, "DimensionalDeletion");
                GameShaders.Misc["Gradient"] = new MiscShaderData(gradientShader, "Gradient");
                GameShaders.Misc["Gradient2"] = new MiscShaderData(gradientShader2, "Gradient");

                Filters.Scene["DimensionalDeletion"] = new Filter(
                    new ScreenShaderData(screenInvertedShader, "DimensionalDeletion"), EffectPriority.Medium);
                Filters.Scene["DimensionalDeletion"].Load();
                //Terraria.Graphics.Effects.Filters.Scene["Name"].GetShader().Shader.Parameters[""].SetValue(2);
            }
        }

        public override void PostSetupContent()
        {
            BulletUtils.LoadTexture();
        }

        internal enum MessageType : byte
        {
            PlayerUpdateCount
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                // This message syncs ExampleStatIncreasePlayer.exampleLifeFruits and ExampleStatIncreasePlayer.exampleManaCrystals
                case MessageType.PlayerUpdateCount:
                    byte playerNumber = reader.ReadByte();
                    OptionsManagingPlayer drawPlayer = Main.player[playerNumber].GetModPlayer<OptionsManagingPlayer>();
                    drawPlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        drawPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
            }
        }
    }
}
