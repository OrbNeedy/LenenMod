using lenen.Common.Graphics;
using lenen.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace lenen.Common.Utils
{
    public static class FlashbombStats
    {
        public static bool Circle(Projectile self, Projectile target, float size = 60)
        {
            return (self.Center.Distance(target.Center) <= size + target.Size.X);
        }

        public static bool Square(Projectile self, Projectile target, Vector2 dimensions)
        {
            return Collision.CheckAABBvAABBCollision(self.Center - dimensions / 2f, dimensions,
                target.position, target.Size);
        }

        public static bool Line(Projectile self, Projectile target, Vector2 start, Vector2 end, 
            float width, out float collisionPoint)
        {
            collisionPoint = 0;
            return Collision.CheckAABBvLineCollision(target.Center, target.Size,
                self.Center + start, self.Center + end, width, ref collisionPoint);
        }

        public static void RememberedRemnantsDraw(Projectile projectile)
        {
            Rectangle sourceRectangle;
            if (PlayerRenderTarget.canUseTarget)
            {
                SpriteBatchState tempState = SpriteBatchExt.GetState(Main.spriteBatch);

                SpriteBatchExt.Restart(Main.spriteBatch, tempState, SpriteSortMode.Immediate);

                Rectangle playerRect = PlayerRenderTarget.
                    getPlayerTargetSourceRectangle(projectile.owner);
                sourceRectangle = new Rectangle(projectile.owner * playerRect.Width, 0,
                    playerRect.Width, playerRect.Height);

                GameShaders.Misc["Silouette"].UseColor(0.075f, 0.075f, 0.075f).
                    UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(projectile.Opacity).Apply();
                Main.spriteBatch.Draw(PlayerRenderTarget.Target, projectile.Center - Main.screenPosition -
                    playerRect.Size() / 2 - new Vector2(10, 21), sourceRectangle, Color.White);
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                SpriteBatchExt.Restart(Main.spriteBatch, tempState);
            }
        }

        public static void WormholeDraw(Projectile projectile)
        {
            Rectangle sourceRectangle;
            Vector2 origin;
            // Why did I give it such a long name?
            Texture2D flashbomb = ModContent.Request<Texture2D>("lenen/Content/Projectiles/GravityPullBulletWithAura").Value;

            sourceRectangle = new Rectangle(0, 0, flashbomb.Width, flashbomb.Height);
            origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(
                flashbomb,
                projectile.Center - Main.screenPosition,
                sourceRectangle,
                Color.White * projectile.Opacity,
                projectile.rotation,
                origin,
                new Vector2(1, 1),
                SpriteEffects.None);
        }

        public static void BlackRopesDraw(Projectile projectile)
        {
            Rectangle sourceRectangle;
            Vector2 origin;
            Asset<Texture2D> flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/BlackRopes");

            Vector2 startOffset = new Vector2(-800);
            sourceRectangle = flashbomb.Frame();
            origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(
                flashbomb.Value,
                projectile.Center - Main.screenPosition,
                sourceRectangle,
                Color.White * projectile.Opacity,
                -MathHelper.PiOver4 - MathHelper.PiOver2,
                origin,
                new Vector2(10, 1),
                SpriteEffects.None);

            startOffset.X *= -1;

            Main.EntitySpriteDraw(
                flashbomb.Value,
                projectile.Center - Main.screenPosition,
                sourceRectangle,
                Color.White * projectile.Opacity,
                -MathHelper.PiOver4,
                origin,
                new Vector2(10, 1),
                SpriteEffects.None);
        }

        public static void VertexEmitDraw(Projectile projectile)
        {
            Asset<Texture2D> flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/VertexEmit");
            Rectangle sourceRect = flashbomb.Frame();
            SpriteBatchState tempState = SpriteBatchExt.GetState(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState, SpriteSortMode.Immediate);

            // Silouette
            MiscShaderData shader = GameShaders.Misc["Silouette"].UseColor(0.075f, 0.075f, 0.075f).
                UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(projectile.Opacity);

            /*shader.Shader.Parameters["leftColor"].
                SetValue(new Vector3(255, 0, 0));
            shader.Shader.Parameters["middleColor"].
                SetValue(new Vector3(0, 0, 255));
            shader.Shader.Parameters["rightColor"].
                SetValue(new Vector3(0, 255, 0));*/

            DrawData data = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition,
                sourceRect,
                Color.White * projectile.Opacity,
                MathHelper.PiOver2,
                sourceRect.Size() / 2,
                new Vector2(2400, 1),
                SpriteEffects.None
            );

            DrawData data2 = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition,
                sourceRect,
                Color.White * projectile.Opacity,
                0,
                sourceRect.Size() / 2,
                new Vector2(2400, 1),
                SpriteEffects.None
            );

            shader.Apply(data);
            shader.Apply(data2);

            data.Draw(Main.spriteBatch);
            data2.Draw(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState);
        }

        public static void MonochromeFlashDraw(Projectile projectile, float progress)
        {
            Asset<Texture2D> flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/MonochromeFlash");
            Rectangle bottom = flashbomb.Frame(1, 3, 0, 0);
            Rectangle sourceRect = flashbomb.Frame(1, 3, 0, 1);
            Rectangle top = flashbomb.Frame(1, 3, 0, 0);
            Vector2 origin = new(sourceRect.Width / 2f, sourceRect.Height);
            SpriteBatchState tempState = SpriteBatchExt.GetState(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState, SpriteSortMode.Immediate);
            
            MiscShaderData shader = GameShaders.Misc["Silouette"].UseColor(0.075f, 0.075f, 0.075f).
                UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(projectile.Opacity * 0.5f);

            /*shader.Shader.Parameters["leftColor"].
                SetValue(new Vector3(255, 0, 255));
            shader.Shader.Parameters["middleColor"].
                SetValue(new Vector3(0, 0, 255));
            shader.Shader.Parameters["rightColor"].
                SetValue(new Vector3(0, 255, 255));*/

            float addedHeight = (1 - progress) * 2880f / 124f;

            DrawData bigBottom = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition,
                bottom,
                Color.White * projectile.Opacity,
                projectile.rotation,
                origin,
                new Vector2(1 * progress, 1),
                SpriteEffects.FlipVertically
            );

            Vector2 bottomEnd = new Vector2(0, bottom.Height).
                RotatedBy(projectile.rotation);
            DrawData bigMiddle = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition - bottomEnd,
                sourceRect,
                Color.White * projectile.Opacity,
                projectile.rotation,
                origin,
                new Vector2(1 * progress, 1 + addedHeight),
                SpriteEffects.None
            );

            Vector2 middleEnd = new Vector2(0, sourceRect.Height * (1 + addedHeight)).
                RotatedBy(projectile.rotation);
            DrawData bigTop = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition - bottomEnd - middleEnd,
                top,
                Color.White * projectile.Opacity,
                projectile.rotation,
                origin,
                new Vector2(1 * progress, 1),
                SpriteEffects.None
            );

            DrawData smallBottom = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition,
                bottom,
                Color.White * projectile.Opacity,
                projectile.rotation,
                origin,
                new Vector2(0.4f * progress, 1),
                SpriteEffects.FlipVertically
            );

            DrawData smallMiddle = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition - bottomEnd,
                sourceRect,
                Color.White * projectile.Opacity,
                projectile.rotation,
                origin,
                new Vector2(0.4f * progress, 1 + addedHeight),
                SpriteEffects.None
            );

            DrawData smallTop = new(
                flashbomb.Value,
                projectile.Center - Main.screenPosition - bottomEnd - middleEnd,
                top,
                Color.White * projectile.Opacity,
                projectile.rotation,
                origin,
                new Vector2(0.4f * progress, 1),
                SpriteEffects.None
            );

            shader.Apply();
            /*shader.Apply(bigBottom);
            shader.Apply(bigMiddle);
            shader.Apply(bigTop);
            shader.Apply(bigBottom);
            shader.Apply(bigMiddle);
            shader.Apply(bigTop);*/

            bigBottom.Draw(Main.spriteBatch);
            bigMiddle.Draw(Main.spriteBatch);
            bigTop.Draw(Main.spriteBatch);

            smallBottom.Draw(Main.spriteBatch);
            smallMiddle.Draw(Main.spriteBatch);
            smallTop.Draw(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState);
        }
    }
}
