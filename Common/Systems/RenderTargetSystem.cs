﻿using FullSerializer;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using lenen.Common.Graphics;
using lenen.Content.Projectiles;
using lenen.Common.Players;

namespace lenen.Common.Systems
{
    // From https://github.com/ZenTheMod/WizenkleBoss/blob/main/Common/Ink/InkSystem.cs#L33
    public class RenderTargetSystem : ModSystem
    {
        // This shader doesn't have a color, so this will be tentatively commented 
        /*public static Color OutlineColor { get; private set; } = Main.OurFavoriteColor; // just so i can change it later

        public static bool PrideMonth = false;

        public static Color InkColor => PrideMonth ? Main.DiscoColor : new(85, 25, 255, 255);*/

        // Lame rt setup
        /*#region RenderTargetSetup

        public class InkTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);

                DrawInk();

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }

        public static InkTargetContent inkTargetByRequest;

        public class InsideInkTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);*/

                /*InkRippleSystem.requestedThisFrame = true;
                if (InkRippleSystem.isReady)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);
                    var Ink = Helper.WaterInkColorizer;

                    Ink.Value.Parameters["InkColor"]?.SetValue(InkColor.ToVector4());
                    Ink.Value.Parameters["RippleStrength"]?.SetValue(5f * Utils.Remap(ModContent.GetInstance<VFXConfig>().InkContrast / 100f, 0f, 1f, 1f, 4f));

                    // Nerd Shit.
                    if (!ModContent.GetInstance<DebugConfig>().DebugColoredRipples)
                        Ink.Value.CurrentTechnique.Passes[1].Apply();

                    spriteBatch.Draw(InkRippleSystem.rippleTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                    spriteBatch.End();
                }*/
                /*spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);

                DrawInInk();

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
                InsideInkTargetDrawnToThisFrame = true;
            }
        }
        public static InsideInkTargetContent insideInkTargetByRequest;

        public static bool InsideInkTargetDrawnToThisFrame = false;

        public override void Load()
        {
            inkTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(inkTargetByRequest);

            insideInkTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(insideInkTargetByRequest);

            On_Main.CheckMonoliths += DrawInsideInkTarget;
        }

        public override void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(inkTargetByRequest);

            Main.ContentThatNeedsRenderTargets.Remove(insideInkTargetByRequest);

            On_Main.CheckMonoliths -= DrawInsideInkTarget;
        }
        // This makes sure that the target gets drawn to BEFORE the player normally draws.
        private void DrawInsideInkTarget(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (Main.gameMenu)
                return;
            if (AnyActiveInk)
                insideInkTargetByRequest.Request();
        }

        #endregion
        // This is so that the effect actually ends when youre not drugged asf
        public override void PostUpdateEverything()
        {
            AnyActiveInk = _AnyActiveInk();

            InvertedShaderData.ToggleActivityIfNecessary();
        }

        public static bool AnyActiveInk;

        private static bool _AnyActiveInk() =>
            Main.projectile.Where(p => p.active && p.ModProjectile is FlashbombProjectile && 
            p.ai[0] == (int)Flashbomb.DimensionalDeletion ).Any();

        public static void DrawInk()
        {
            foreach (var p in Main.ActiveProjectiles)
            {
                if (!p.active || (p.ModProjectile is not FlashbombProjectile drawer && 
                    p.ai[0] != (int)Flashbomb.DimensionalDeletion))
                    continue;
                // Tentatively deleting this 
                //drawer.Shape();
            }*/
            /*foreach (var npc in Main.ActiveNPCs)
            {
                if (!npc.active || npc.ModNPC is not IDrawInk drawer)
                    continue;
                drawer.Shape();
            }*/
        /*}

        public static void DrawInInk()
        {
            // jank prediction
            Vector2 screenPosition = Main.screenPosition - (Main.screenLastPosition - Main.screenPosition);
            foreach (var p in Main.ActiveProjectiles)
            {
                if (!p.active || (p.ModProjectile is not FlashbombProjectile drawer &&
                    p.ai[0] != (int)Flashbomb.DimensionalDeletion))
                    continue;
                //drawer.Shape(screenPosition);
            }*/
            /*foreach (var npc in Main.ActiveNPCs)
            {
                if (!npc.active || npc.ModNPC is not IDrawInInk drawer)
                    continue;
                drawer.Shape(screenPosition);
            }*/

            /*DrawVanishedPlayers();

            InkCreatureHelper bh = ModContent.GetInstance<InkCreatureHelper>();
            if (bh is IDrawInInk bhdrawer)
                bhdrawer.Shape(screenPosition);*/
        /*}

        public static void DrawVanishedPlayers()
        {*/
            /*foreach (var player in Main.player.Where(p => p.active && p.GetModPlayer<InkPlayer>().InkBuffActive && p.dye.Length > 0))
            {
                player.heldProj = -1;
                if (player.GetModPlayer<InkPlayer>().InGhostInk && player.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
                {
                    float rot = player.GetModPlayer<InkPlayer>().DashVelocity.ToRotation() + MathHelper.PiOver2;

                    rot = MathF.Round(rot / MathHelper.PiOver4) * MathHelper.PiOver4;

                    Vector2 offset = Main.screenPosition - (Main.screenLastPosition - Main.screenPosition);
                    if (player.GetModPlayer<InkPlayer>().InTile)
                    {
                        int count = (int)(Main.GlobalTimeWrappedHourly * 60 % 60 / 4);
                        Rectangle frame = TextureRegistry.InkDash.Value.Frame(1, 15, 0, count, 0, 0);
                        Main.spriteBatch.Draw(TextureRegistry.InkDash.Value, player.Center - offset, frame, Color.White, rot, new Vector2(26), 1f, SpriteEffects.None, 0f);

                        count = (int)((Main.GlobalTimeWrappedHourly + 20) * 60 % 60 / 4);
                        frame = TextureRegistry.InkDash.Value.Frame(1, 15, 0, count, 0, 0);
                        Main.spriteBatch.Draw(TextureRegistry.InkDash.Value, player.Center - offset, frame, Color.White * 0.5f, rot, new Vector2(26), 2f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        Main.spriteBatch.Draw(TextureRegistry.Star.Value, player.Center - offset, null, Color.White with { A = 0 }, rot, TextureRegistry.Star.Size() / 2f, 0.8f * MathF.Sin(Main.GlobalTimeWrappedHourly * 10), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(TextureRegistry.Star.Value, player.Center - offset, null, Color.White with { A = 0 }, rot + MathHelper.PiOver4, TextureRegistry.Star.Size() / 2f, 1.3f * MathF.Sin(Main.GlobalTimeWrappedHourly * 14), SpriteEffects.None, 0f);
                        for (int k = player.GetModPlayer<InkPlayer>().dashOldPos.Length - 1; k > 0; k--)
                        {
                            float interpolator = (player.GetModPlayer<InkPlayer>().dashOldPos.Length - k) / (float)player.GetModPlayer<InkPlayer>().dashOldPos.Length;
                            Main.spriteBatch.Draw(TextureRegistry.Star.Value, player.GetModPlayer<InkPlayer>().dashOldPos[k] - offset, null, (Color.White * interpolator) with { A = 0 }, rot + MathHelper.PiOver4 * k, TextureRegistry.Star.Size() / 2f, 0.7f * MathF.Sin(Main.GlobalTimeWrappedHourly * interpolator * 5) * interpolator, SpriteEffects.None, 0f);
                        }
                    }

                }
                else
                {
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position - (Main.screenPosition - Main.screenLastPosition), player.fullRotation, player.fullRotationOrigin, 0);
                }
            }*/
        //}
    }
}
