using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.Graphics
{
    public struct PlayerPosition(Vector2 playerPos, Vector2 playerCenter,
        Vector2 mountCenter, Vector2 screenPos, Vector2 itemPos, int heldProj, 
        Vector2 offset)
    {
        public Vector2 playerPos = playerPos;
        public Vector2 playerCenter = playerCenter;
        public Vector2 mountCenter = mountCenter;
        public Vector2 screenPos = screenPos;
        public Vector2 itemPos = itemPos;
        public int heldProj = heldProj;
        public Vector2 offset = offset;
    }

    public class RenderTarget : ModSystem
    {
        public static RenderTarget2D shadowsRenderTarget;
        public static RenderTarget2D haniwaRenderTarget;

        public static bool canUseShadow = false;
        public static bool canUseHaniwa = false;

        public static int sheetSquareX = 200;
        public static int sheetSquareY = 300;

        public static Dictionary<int, int> PlayerIndexLookup = new();

        public static int prevShadowCount;
        public static int prevHaniwaCount;

        PlayerPosition oldPos;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            sheetSquareX = 200;
            sheetSquareY = 300;

            PlayerIndexLookup = new();
            prevShadowCount = -1;
            prevHaniwaCount = -1;

            Main.QueueMainThreadAction(() => {
                shadowsRenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth,
                    Main.screenHeight);
                haniwaRenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth,
                    Main.screenHeight);
            });

            // Used to setup
            On_Main.CheckMonoliths += RenderTargetSetup;
            On_Lighting.GetColor_int_int += GetColorOverrideIntInt;
            On_Lighting.GetColor_int_int += GetColorOverrideIntIntHaniwa;
            On_Lighting.GetColor_Point += GetColorOverrideIntInt;
            On_Lighting.GetColor_Point += GetColorOverrideIntIntHaniwa;
            On_Lighting.GetColor_int_int_Color += GetColorOverrideIntInt;
            On_Lighting.GetColor_int_int_Color += GetColorOverrideIntIntHaniwa;
            On_Lighting.GetColor_Point_Color += GetColorOverride;
            On_Lighting.GetColor_Point_Color += GetColorOverrideHaniwa;
            On_Lighting.GetColorClamped += GetColorOverride;
            On_Lighting.GetColorClamped += GetColorOverrideHaniwa;
        }

        private void RenderTargetSetup(On_Main.orig_CheckMonoliths orig)
        {
            orig();

            // Do not run in the menu, or when there are no active projectiles
            if (Main.gameMenu) return;

            if (Main.projectile.Any(n => n.active) && Main.projectile.Any(n => n.active))
            {
                int activePlayerCount = Main.player.Count(n => n.active);

                if (activePlayerCount != prevHaniwaCount)
                {
                    prevHaniwaCount = activePlayerCount;

                    haniwaRenderTarget.Dispose();
                    haniwaRenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice,
                        sheetSquareY * activePlayerCount, sheetSquareY);

                    int activeCount = 0;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active)
                        {
                            PlayerIndexLookup[i] = activeCount;
                            activeCount++;
                        }
                        else
                        {
                            PlayerIndexLookup[i] = -1;
                        }
                    }
                }

                SetShadow();

                SetHaniwa();
                // Set haniwa too 
            }

            if (Main.instance.tileTarget.IsDisposed) return;
        }

        private Vector2 GetHaniwaOffset(int index)
        {
            if (PlayerIndexLookup.ContainsKey(index))
            {
                return new Vector2((sheetSquareX * PlayerIndexLookup[index]) + (sheetSquareX / 2f),
                    sheetSquareY / 2f);
            }

            return Vector2.Zero;
        }

        public static Rectangle GetHaniwaRect(int index)
        {
            if (PlayerIndexLookup.ContainsKey(index))
            {
                return new Rectangle(sheetSquareX * PlayerIndexLookup[index], 0,
                    sheetSquareX, sheetSquareY);
            }

            return Rectangle.Empty;
        }

        private void SetHaniwa()
        {
            int type = ModContent.ProjectileType<HaniwaClone>();

            RenderTargetBinding[] oldtargets = Main.graphics.GraphicsDevice.GetRenderTargets();
            // Don't use the render target while it's drawing to it 
            canUseHaniwa = false;

            Main.graphics.GraphicsDevice.SetRenderTarget(haniwaRenderTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null,
                Main.GameViewMatrix.EffectMatrix);

            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active && proj.type == type)
                {
                    HaniwaClone modProj = (HaniwaClone)proj.ModProjectile;
                    Player clone = modProj.clonedPlayer;

                    oldPos = GetPositionStats(clone, Main.screenPosition, GetHaniwaOffset(proj.owner));

                    // Temp change Player's actual position to lock into their frame
                    Main.screenPosition = Vector2.Zero;
                    clone.position = oldPos.offset;
                    clone.Center = oldPos.playerCenter - oldPos.playerPos + oldPos.offset;
                    clone.itemLocation = oldPos.itemPos - oldPos.playerPos + oldPos.offset;
                    clone.MountedCenter = oldPos.mountCenter - oldPos.playerPos + oldPos.offset;
                    clone.heldProj = -1;
                    Main.screenPosition = Vector2.Zero;

                    Main.PlayerRenderer.DrawPlayer(Main.Camera, modProj.clonedPlayer,
                        oldPos.offset, modProj.clonedPlayer.fullRotation,
                        modProj.clonedPlayer.fullRotationOrigin, 0f);

                    clone.position = oldPos.playerPos;
                    clone.Center = oldPos.playerCenter;
                    clone.itemLocation = oldPos.itemPos;
                    clone.MountedCenter = oldPos.mountCenter;
                    clone.heldProj = oldPos.heldProj;
                    Main.screenPosition = oldPos.screenPos;

                    count++;
                }
            }

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets);
            canUseHaniwa = true;
        }

        private Vector2 GetShadowOffset(int index, int shadow)
        {
            if (PlayerIndexLookup.ContainsKey(index))
            {
                int playerSeparation = PlayerIndexLookup[index] * 7;
                return new Vector2((sheetSquareX * (playerSeparation + shadow)) + (sheetSquareX / 2f),
                    sheetSquareY / 2f);
            }

            return Vector2.Zero;
        }

        public static Rectangle GetShadowRect(int index, int shadow)
        {
            if (PlayerIndexLookup.ContainsKey(index))
            {
                int playerSeparation = PlayerIndexLookup[index] * 7;
                return new Rectangle(sheetSquareX * (playerSeparation + shadow), 0,
                    sheetSquareX, sheetSquareY);
            }

            return Rectangle.Empty;
        }

        private void SetShadow()
        {
            int type = ModContent.ProjectileType<BodyDouble>();

            RenderTargetBinding[] oldtargets = Main.graphics.GraphicsDevice.GetRenderTargets();
            // Don't use the render target while it's drawing to it 
            canUseShadow = false;

            Main.graphics.GraphicsDevice.SetRenderTarget(shadowsRenderTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, 
                Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, 
                Main.GameViewMatrix.EffectMatrix);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active && proj.type == type)
                {
                    BodyDouble modProj = (BodyDouble)proj.ModProjectile;
                    Player clone = modProj.clonedPlayer;

                    oldPos = GetPositionStats(clone, Main.screenPosition, GetShadowOffset(proj.owner, (int)proj.ai[0]));

                    //temp change Player's actual position to lock into their frame
                    Main.screenPosition = Vector2.Zero;
                    clone.position = oldPos.offset;
                    clone.Center = oldPos.playerCenter - oldPos.playerPos + oldPos.offset;
                    clone.itemLocation = oldPos.itemPos - oldPos.playerPos + oldPos.offset;
                    clone.MountedCenter = oldPos.mountCenter - oldPos.playerPos + oldPos.offset;
                    clone.heldProj = -1;
                    Main.screenPosition = Vector2.Zero;

                    Main.PlayerRenderer.DrawPlayer(Main.Camera, modProj.clonedPlayer,
                        oldPos.offset, modProj.clonedPlayer.fullRotation,
                        modProj.clonedPlayer.fullRotationOrigin, 0f);

                    clone.position = oldPos.playerPos;
                    clone.Center = oldPos.playerCenter;
                    clone.itemLocation = oldPos.itemPos;
                    clone.MountedCenter = oldPos.mountCenter;
                    clone.heldProj = oldPos.heldProj;
                    Main.screenPosition = oldPos.screenPos;
                }
            }

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets);
            canUseShadow = true;
        }

        public static PlayerPosition GetPositionStats(Player player, Vector2 screenPosition, Vector2 offset)
        {
            return new PlayerPosition(player.position, player.Center, player.MountedCenter, 
                screenPosition, player.itemLocation, player.heldProj, offset);
        }

        private Color GetColorOverride(On_Lighting.orig_GetColorClamped orig, int x, int y, Color oldColor)
        {
            if (canUseShadow)
                return orig.Invoke(x, y, oldColor);

            return orig.Invoke(x + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16), oldColor);
        }

        private Color GetColorOverride(On_Lighting.orig_GetColor_Point_Color orig, Point point, Color originalColor)
        {
            if (canUseShadow)
                return orig.Invoke(point, originalColor);

            return orig.Invoke(new Point(point.X + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                point.Y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16)), originalColor);
        }

        public Color GetColorOverrideIntInt(On_Lighting.orig_GetColor_Point orig, Point point)
        {
            if (canUseShadow)
                return orig.Invoke(point);

            return orig.Invoke(new Point(point.X + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                point.Y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16)));
        }

        public Color GetColorOverrideIntInt(On_Lighting.orig_GetColor_int_int orig, int x, int y)
        {
            if (canUseShadow)
                return orig.Invoke(x, y);

            return orig.Invoke(x + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16));
        }

        public Color GetColorOverrideIntInt(On_Lighting.orig_GetColor_int_int_Color orig, int x, int y, Color c)
        {
            if (canUseShadow)
                return orig.Invoke(x, y, c);

            return orig.Invoke(x + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16), c);
        }



        private Color GetColorOverrideHaniwa(On_Lighting.orig_GetColorClamped orig, int x, int y, Color oldColor)
        {
            if (canUseHaniwa)
                return orig.Invoke(x, y, oldColor);

            return orig.Invoke(x + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16), oldColor);
        }

        private Color GetColorOverrideHaniwa(On_Lighting.orig_GetColor_Point_Color orig, Point point, Color originalColor)
        {
            if (canUseHaniwa)
                return orig.Invoke(point, originalColor);

            return orig.Invoke(new Point(point.X + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                point.Y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16)), originalColor);
        }

        public Color GetColorOverrideIntIntHaniwa(On_Lighting.orig_GetColor_Point orig, Point point)
        {
            if (canUseHaniwa)
                return orig.Invoke(point);

            return orig.Invoke(new Point(point.X + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                point.Y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16)));
        }

        public Color GetColorOverrideIntIntHaniwa(On_Lighting.orig_GetColor_int_int orig, int x, int y)
        {
            if (canUseHaniwa)
                return orig.Invoke(x, y);

            return orig.Invoke(x + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16));
        }

        public Color GetColorOverrideIntIntHaniwa(On_Lighting.orig_GetColor_int_int_Color orig, int x, int y, Color c)
        {
            if (canUseHaniwa)
                return orig.Invoke(x, y, c);

            return orig.Invoke(x + (int)((oldPos.playerPos.X - oldPos.offset.X) / 16),
                y + (int)((oldPos.playerPos.Y - oldPos.offset.Y) / 16), c);
        }
    }
}
