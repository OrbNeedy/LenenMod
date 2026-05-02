using lenen.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.UI;

namespace lenen.Common.UI
{
    class UIShaderImage : UIElement
    {
        private Asset<Texture2D> _texture;
        public float ImageScale = 1f;
        public float Rotation;
        public bool ScaleToFit;
        public bool AllowResizingDimensions = true;
        public Color Color = Color.White;
        public Vector2 NormalizedOrigin = Vector2.Zero;
        public bool RemoveFloatingPointsFromDrawPosition;
        private Texture2D _nonReloadingTexture;

        // A copy of UIImage, but includes data for the shader
        Color leftColor = Color.White;
        Color middleColor = Color.White;
        Color rightColor = Color.White;
        MiscShaderData shader = null;

        public UIShaderImage(Asset<Texture2D> texture)
        {
            SetImage(texture);
        }

        public UIShaderImage(Texture2D nonReloadingTexture)
        {
            SetImage(nonReloadingTexture);
        }

        public void SetImage(Asset<Texture2D> texture)
        {
            _texture = texture;
            _nonReloadingTexture = null;
            if (AllowResizingDimensions)
            {
                Width.Set(_texture.Width(), 0f);
                Height.Set(_texture.Height(), 0f);
            }
        }

        public void SetImage(Texture2D nonReloadingTexture)
        {
            _texture = null;
            _nonReloadingTexture = nonReloadingTexture;
            if (AllowResizingDimensions)
            {
                Width.Set(_nonReloadingTexture.Width, 0f);
                Height.Set(_nonReloadingTexture.Height, 0f);
            }
        }

        public void SetShader(MiscShaderData data)
        {
            shader = null;
            shader = data;

            shader.Shader.Parameters["leftColor"].
                SetValue(new Vector3(leftColor.R, leftColor.G, leftColor.B) / 255f);
            shader.Shader.Parameters["middleColor"].
                SetValue(new Vector3(middleColor.R, middleColor.G, middleColor.B) / 255f);
            shader.Shader.Parameters["rightColor"].
                SetValue(new Vector3(rightColor.R, rightColor.G, rightColor.B) / 255f);
        }

        public void SetShaderParams(Color leftColor, Color middleColor, 
            Color rightColor)
        {
            this.leftColor = leftColor;
            this.middleColor = middleColor;
            this.rightColor = rightColor;

            if (shader != null)
            {
                shader.Shader.Parameters["leftColor"].
                    SetValue(new Vector3(this.leftColor.R, this.leftColor.G, this.leftColor.B) / 255f);
                shader.Shader.Parameters["middleColor"].
                    SetValue(new Vector3(this.middleColor.R, this.middleColor.G, this.middleColor.B) / 255f);
                shader.Shader.Parameters["rightColor"].
                    SetValue(new Vector3(this.rightColor.R, this.rightColor.G, this.rightColor.B) / 255f);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Texture2D texture2D = null;
            if (_texture != null)
                texture2D = _texture.Value;

            if (_nonReloadingTexture != null)
                texture2D = _nonReloadingTexture;

            SpriteBatchState tempState = SpriteBatchExt.GetState(spriteBatch);

            DrawData data = new DrawData(
                texture2D,
                dimensions.ToRectangle(),
                texture2D.Frame(),
                Color, 
                0f, 
                Vector2.Zero, 
                SpriteEffects.None
                );

            if (shader != null)
            {
                SpriteBatchExt.Restart(spriteBatch, tempState, SpriteSortMode.Immediate);

                shader.Apply(data);
            }

            if (ScaleToFit)
            {
                data.Draw(spriteBatch);

                if (shader != null)
                {
                    SpriteBatchExt.Restart(Main.spriteBatch, tempState);
                }
                return;
            }

            Vector2 vector = texture2D.Size();
            Vector2 vector2 = dimensions.Position() + vector * (1f - ImageScale) / 2f + vector * NormalizedOrigin;
            if (RemoveFloatingPointsFromDrawPosition)
                vector2 = vector2.Floor();

            data = new DrawData(
                texture2D, 
                vector2, 
                texture2D.Frame(), 
                Color, 
                Rotation, 
                vector * NormalizedOrigin, 
                ImageScale, 
                SpriteEffects.None, 
                0f
            );

            if (shader != null)
            {
                shader.Apply(data);
            }

            data.Draw(spriteBatch);

            if (shader != null)
            {
                SpriteBatchExt.Restart(Main.spriteBatch, tempState);
            }
        }
    }
}
