using lenen.Common.GlobalProjectiles;
using lenen.Common.Systems;
using lenen.Content.Items.Weapons;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public enum Flashbomb
    {
        MaidenPit,
        VertexEmit,
        MonochromeFlash,
        RememberedRemnants,
        CurrentBlast,
        Whiteout,
        NegativeAndPositive,
        BipolarWings,
        Wormhole,
        LostTorus,
        HaniwaFist,
        BlackRopes,
        Ladder,
        CorpseCard,
        CheapImpact,
        ArmorMode,
        KampakuCannon,
        FeralGhost,
        DimensionalDeletion,
        Suzumi
    }

    public class ThrillPlayer : ModPlayer
    {
        public bool gainThrill = false;
        public float oldPercent = 0;
        public float percent = 0;
        public float fillRate = 0.025f;
        public int permanenceTime = 0;

        public bool flashbombActive = false;
        public bool flashbombVariation = false;
        public int flashbombDuration = 0;
        public int lastFlashbombUse = -1;

        public override void ResetEffects()
        {
            gainThrill = false;
            //percent = 1;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // To activate it, the player must not be stone or dead
            if (Player.DeadOrGhost || Player.stoned)
            {
                return;
            }
            if (KeybindSystem.flashbomb.Current)
            {
                if (percent >= 1 && !flashbombActive)
                {
                    flashbombActive = true;
                    lastFlashbombUse = GetPlayerFlashbombItem(Player, out flashbombVariation);
                    CheckWormhole(true);
                    flashbombDuration = FlashbombBegin(lastFlashbombUse);
                    percent -= FlashbombPercentUsage(lastFlashbombUse);
                }
            }
        }

        public override void PreUpdate()
        {
            // Update oldPercent before thrill gain
            oldPercent = percent;

            if (flashbombActive)
            {
                if (flashbombDuration > 0)
                {
                    FlashbombUpdate(lastFlashbombUse, flashbombDuration);
                    flashbombDuration--;
                } else
                {
                    flashbombActive = false;
                }

            }

            // Gain thrill if possible
            if (gainThrill && !flashbombActive && !Player.immune)
            {
                foreach (var projectile in Main.ActiveProjectiles)
                {
                    ProjectileGrazeLimit grazeProjectile = projectile.GetGlobalProjectile<ProjectileGrazeLimit>();
                    
                    // Don't gain thrill from recently grazed projectiles, projectiles with no more graze available,
                    // friendly projectiles, minions, sentries, or projectiles of 0 damage
                    if (grazeProjectile.grazeCooldown > 0 || grazeProjectile.grazeAvailable <= 0 || 
                        projectile.friendly || projectile.damage <= 0|| projectile.sentry || 
                        projectile.minion) continue;

                    // If it is a modded projectile, take into account it's special collisions
                    bool? moddedCollision = projectile.ModProjectile?.Colliding(projectile.Hitbox, 
                        new Rectangle((int)Player.Center.X-60, (int)Player.Center.Y-60, 120, 120));
                    bool finalModdedCollision = false;
                    if (moddedCollision != null) finalModdedCollision = (bool)moddedCollision;

                    // Standard box collision followed by a radius collision
                    if ((Collision.CheckAABBvAABBCollision(Player.Center, new Vector2(120, 120), projectile.Center,
                        projectile.Size * 1.5f) && projectile.Center.Distance(Player.Center) <=
                        50 + (projectile.Size.X / 2)) || finalModdedCollision)
                    {
                        // Increase percent, reset permanence timer to 30 seconds, give thrill cooldown to the
                        // projectile, and reduce the graze available
                        percent += fillRate;
                        permanenceTime = 1800;
                        grazeProjectile.grazeCooldown = 15;
                        grazeProjectile.grazeAvailable--;
                        SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/thrill"), Player.Center);
                    }
                }
            }

            // If permanence is at <= 0, reduce the percent every 5 frames
            // Otherwise, reduce the timer
            if (permanenceTime <= 0)
            {
                percent -= fillRate/2;
                permanenceTime = 5;
            } else
            {
                permanenceTime--;
            }

            // Clamp the percent between 0 and 1
            percent = MathHelper.Clamp(percent, 0, 1);

            // Sound effect when the percent has just reached 1
            if (oldPercent < 1 && percent >= 1)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/charge_up"), Player.Center);
            }
        }

        public override void PostUpdateBuffs()
        {
            if (flashbombActive)
            {
                FlashbombModifyStats(lastFlashbombUse, flashbombDuration);
            }
        }

        private void FlashbombUpdate(int type, int duration)
        {
            switch (type)
            {
                case (int)Flashbomb.Suzumi:
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 1);
                    if (KeybindSystem.flashbomb.Current && percent > 0)
                    {
                        flashbombDuration = 2;
                        percent -= fillRate / 3;
                    }
                    break;
                default:
                    break;
            }
        }

        private void FlashbombModifyStats(int type, int duration)
        {
            switch (type)
            {
                case (int)Flashbomb.Suzumi:
                    Player.aggro -= 1000;
                    break;
                default:
                    break;
            }
        }

        private float FlashbombPercentUsage(int type)
        {
            if (type == (int)Flashbomb.Suzumi) return fillRate*2;
            return 1;
        }

        private int FlashbombBegin(int type)
        {
            Vector2 direction;
            switch (type)
            {
                case (int)Flashbomb.MaidenPit:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, 
                        ai0: type);
                    return 45;
                case (int)Flashbomb.RememberedRemnants:
                    // Default flashbomb, but include a scapegoat
                    direction = Player.Center.DirectionTo(Main.MouseWorld);
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 30);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + (direction * 60),
                        direction, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: -1);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, 
                        ai0: type);
                    return 30;
                case (int)Flashbomb.NegativeAndPositive:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: type);
                    return 15;
                case (int)Flashbomb.Wormhole:
                    // Default flashbomb, but include a wormhole
                    direction = Player.Center.DirectionTo(Main.MouseWorld);
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 30);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + (direction * 60),
                        direction, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: -1);
                    // Only make a new wormhole if there is no wormhole active.
                    // This will inevitably activate after the player uses the wormhole as well
                    // Note: Apparently, it's not activated after the player uses a wormhole
                    if (!CheckWormhole())
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: type);
                    }
                    return 30;
                case (int)Flashbomb.LostTorus:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, 
                        ai0: type);
                    return 60;
                case (int)Flashbomb.BlackRopes:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Main.MouseWorld,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: type);
                    return 30;
                case (int)Flashbomb.DimensionalDeletion:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Player.Center.DirectionTo(Main.MouseWorld) * 8, 
                        ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: type);
                    return 15;
                case (int)Flashbomb.Suzumi:
                    percent = 1;
                    return 1;
                default:
                    direction = Player.Center.DirectionTo(Main.MouseWorld);
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 30);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + (direction * 60), 
                        direction, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: type);
                    return 30;
            }
        }

        private bool CheckWormhole(bool teleport = false)
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is FlashbombProjectile flashbomb && 
                    projectile.ai[0] == (int)Flashbomb.Wormhole && projectile.owner == Player.whoAmI)
                {
                    if (teleport)
                    {
                        Player.Center = projectile.Center;
                        projectile.timeLeft = 0;
                        Player.immune = true;
                        Player.AddImmuneTime(ImmunityCooldownID.General, 5);
                    }
                    return true;
                }
            }
            return false;
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (!PlayerRenderTarget.canUseTarget || flashbombDuration <= 0 || 
                lastFlashbombUse != (int)Flashbomb.Suzumi)
            {
                return;
            }

            foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers)
            {
                layer.Hide();
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (flashbombDuration > 0 && PlayerRenderTarget.canUseTarget)
            {
                switch (lastFlashbombUse)
                {
                    case (int)Flashbomb.Suzumi:
                        Vector2 position = PlayerRenderTarget.getPlayerTargetPosition(drawInfo.drawPlayer.whoAmI);
                        Rectangle sourceRect = PlayerRenderTarget.
                            getPlayerTargetSourceRectangle(drawInfo.drawPlayer.whoAmI);

                        GameShaders.Misc["HeavyNoise"].UseOpacity(1).Apply(); 
                        Main.spriteBatch.Draw(PlayerRenderTarget.Target, position, sourceRect, Color.White);
                        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                        break;
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
        }

        public static int GetPlayerFlashbombItem(Player player, out bool variation)
        {
            variation = false;
            if (player.HeldItem.ModItem is DimensionalFragment || player.HeldItem.ModItem is DimensionalOrbs)
            {
                return (int)Flashbomb.MaidenPit;
            }
            if (player.HeldItem.ModItem is ImprovedKnife)
            {
                return (int)Flashbomb.RememberedRemnants;
            }
            if (player.HeldItem.ModItem is TrueBirdDrone)
            {
                return (int)Flashbomb.NegativeAndPositive;
            }
            if (player.HeldItem.ModItem is GravitationalAnomaly || player.HeldItem.ModItem is GravityGlobe ||
                player.HeldItem.ModItem is ChristmasGlobe)
            {
                variation = player.HeldItem.ModItem is ChristmasGlobe;
                return (int)Flashbomb.Wormhole;
            }
            if (player.HeldItem.ModItem is HaniwaMaker || player.HeldItem.ModItem is ExtendedGrab)
            {
                // Variation soon
                return (int)Flashbomb.LostTorus;
            }
            if (player.HeldItem.ModItem is Tasouken)
            {
                return (int)Flashbomb.BlackRopes;
            }
            if (player.HeldItem.ModItem is BarrierWeapon)
            {
                // Will be implemented some other time
                return -1;
            }
            if (player.HeldItem.ModItem is MemoryKnife)
            {
                return (int)Flashbomb.Suzumi;
            }
            // etc...
            return -1;
        }
    }
}
