using lenen.Common.GlobalProjectiles;
using lenen.Common.Systems;
using lenen.Content.Buffs;
using lenen.Content.Items.Weapons.Clause;
using lenen.Content.Items.Weapons.Fumikado;
using lenen.Content.Items.Weapons.Kurohebi;
using lenen.Content.Items.Weapons.Kuroji;
using lenen.Content.Items.Weapons.Sukune;
using lenen.Content.Items.Weapons.Suzumi;
using lenen.Content.Items.Weapons.Tsurubami;
using lenen.Content.Items.Weapons.Yabusame;
using lenen.Content.Items.Weapons.Yaorochi;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
        None,
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

    public struct ItemFlashbombStats(Flashbomb flashbombType, bool variant = false)
    {
        public Flashbomb flashbombType = flashbombType;
        public bool variant = variant;
    }

    public record FlashbombInterfaceNames(string name, Color midColor, Color outerColor)
    {
        public string name = name;
        public Color midColor = midColor;
        public Color outerColor = outerColor;
    }

    public class ThrillPlayer : ModPlayer
    {
        public bool gainThrill = false;
        public float oldPercent = 0;
        public float percent = 0;
        public float fillRate = 0.03f;
        public int permanenceTime = 0;

        public int armorModeTimer = 0;

        public bool flashbombActive = false;
        public bool flashbombVariation = false;
        public int flashbombDuration = 0;
        public Flashbomb lastFlashbombUse = Flashbomb.None;
        public int timeSinceFlashbomb = 120;
        public Dictionary<int, int> outerItemDict = new();
        public Dictionary<int, ItemFlashbombStats> innerItemDict = new();

        public static int grazeDistance = 240;
        public static Dictionary<Flashbomb, FlashbombInterfaceNames> uiData = new()
        {
            [Flashbomb.None] = new("Earthern Spotlight", new Color(21, 163, 70), new Color(186, 158, 154)),
            [Flashbomb.MaidenPit] = new("Maiden Pit", new Color(188, 177, 146), new Color(197, 197, 197)),
            [Flashbomb.RememberedRemnants] = new("Earthern Spotlight", new Color(21, 163, 70), new Color(186, 158, 154)),
            [Flashbomb.NegativeAndPositive] = new("Negative and Positive", new Color(108, 86, 132), new Color(72, 72, 72)),
            [Flashbomb.Wormhole] = new("Earthern Spotlight", new Color(21, 163, 70), new Color(186, 158, 154)),
            [Flashbomb.LostTorus] = new("Lost Torus", new Color(140, 109, 82), new Color(151, 79, 66)),
            [Flashbomb.BlackRopes] = new("Black Ropes", new Color(0, 0, 0), new Color(58, 98, 32)),
            [Flashbomb.VertexEmit] = new("Vertex Emit", new Color(227, 227, 227), new Color(0, 0, 0)),
            [Flashbomb.MonochromeFlash] = new("Monochrome Ray -Flash-", new Color(227, 227, 227), new Color(0, 0, 0)),
            [Flashbomb.ArmorMode] = new("Armor Mode", new Color(224, 2, 7), new Color(131, 156, 119))
        };

        public override void ResetEffects()
        {
            gainThrill = false;
            //percent = 1;
        }

        public override void Initialize()
        {
            innerItemDict = new() {
                [-1] = new(Flashbomb.None),
                [0] = new(Flashbomb.MaidenPit),
                [1] = new(Flashbomb.RememberedRemnants),
                [2] = new(Flashbomb.NegativeAndPositive),
                [3] = new(Flashbomb.Wormhole),
                [4] = new(Flashbomb.Wormhole, true),
                [5] = new(Flashbomb.LostTorus),
                [6] = new(Flashbomb.BlackRopes),
                [7] = new(Flashbomb.Suzumi),
                [8] = new(Flashbomb.VertexEmit),
                [9] = new(Flashbomb.MonochromeFlash),
                [10] = new(Flashbomb.ArmorMode)
            };

            outerItemDict = new() {
                [ModContent.ItemType<DimensionalFragment>()] = 0,
                [ModContent.ItemType<DimensionalOrbs>()] = 0,
                [ModContent.ItemType<ImprovedKnife>()] = 1,
                [ModContent.ItemType<TrueBirdDrone>()] = 2,
                [ModContent.ItemType<GravitationalAnomaly>()] = 3,
                [ModContent.ItemType<GravityGlobe>()] = 3,
                [ModContent.ItemType<ChristmasGlobe>()] = 4,
                [ModContent.ItemType<HaniwaMaker>()] = 5,
                [ModContent.ItemType<ExtendedGrab>()] = 5,
                [ModContent.ItemType<Tasouken>()] = 6,
                [ModContent.ItemType<MemoryKnife>()] = 7,
                [ModContent.ItemType<PowerRodFan>()] = 8,
                [ModContent.ItemType<HomunculusWeapon>()] = 10
            };
            
            // Thank you for the Gensokyo mod, Eidolon
            if (ModContent.TryFind("Gensokyo", "NanotechInk", out ModItem nanotechInk))
            {
                outerItemDict.Add(nanotechInk.Type, 9);
            }
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
                    lastFlashbombUse = Flashbomb.None;
                    if (outerItemDict.ContainsKey(Player.HeldItem.type))
                    {
                        lastFlashbombUse = innerItemDict[outerItemDict[Player.HeldItem.ModItem.Type]].flashbombType;
                    }
                    CheckWormhole(true);
                    timeSinceFlashbomb = 0;

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

            if (timeSinceFlashbomb < 120) timeSinceFlashbomb++;

            // Gain thrill if possible
            if (gainThrill && !flashbombActive && !Player.immune)
            {
                foreach (var projectile in Main.ActiveProjectiles)
                {
                    ProjectileGrazeLimit grazeProjectile = projectile.GetGlobalProjectile<ProjectileGrazeLimit>();
                    
                    // Don't gain thrill from recently grazed projectiles, projectiles with no more graze available,
                    // friendly projectiles, minions, sentries, or projectiles of 0 damage
                    if (grazeProjectile.grazeCooldown > 0 || grazeProjectile.grazeAvailable <= 0 || 
                        !projectile.hostile || projectile.damage <= 0|| projectile.sentry || 
                        projectile.minion) continue;

                    bool finalGraze = false;

                    //int grazeDistance = 200;
                    // If it is a modded projectile, take into account it's special collisions
                    Vector2 pos = Player.Center - new Vector2(grazeDistance / 2f);
                    bool? moddedCollision = projectile.ModProjectile?.Colliding(projectile.Hitbox, 
                        new Rectangle((int)pos.X, (int)pos.Y, grazeDistance / 2, grazeDistance / 2));

                    if (moddedCollision != null)
                    {
                        finalGraze = (bool)moddedCollision;
                    } else
                    {
                        // Standard box collision followed by a radius collision
                        if (Collision.CheckAABBvAABBCollision(pos, new Vector2(grazeDistance, grazeDistance),
                        projectile.position, projectile.Size))
                        {
                            finalGraze = projectile.Center.Distance(Player.Center) <=
                                (grazeDistance / 2f) + (projectile.Size.X / 2f);
                        }
                    }

                    if (finalGraze)
                    {
                        // Increase percent, reset permanence timer to 30 seconds, give thrill cooldown to the
                        // projectile, and reduce the graze available
                        percent += fillRate;
                        permanenceTime = 1800;
                        grazeProjectile.grazeCooldown = 10;
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

        private void FlashbombUpdate(Flashbomb type, int duration)
        {
            switch (type)
            {
                case Flashbomb.Suzumi:
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 1);
                    if (KeybindSystem.flashbomb.Current && percent > 0)
                    {
                        flashbombDuration = 2;
                        percent -= fillRate / 3;
                    }
                    break;
                case Flashbomb.ArmorMode:
                    if (KeybindSystem.flashbomb.Current)
                    {
                        float colPer = armorModeTimer / 120f;
                        Color finalColor = Color.Lerp(Color.Blue, Color.DarkRed, colPer);
                        for (int i = 0; i < 2 + (colPer * 6); i++)
                        {
                            Vector2 dir = new Vector2(3, 0).
                                RotatedByRandom(MathHelper.TwoPi) + Player.velocity;
                            int index = Dust.NewDust(Player.Center, 0, 0, DustID.TintableDustLighted, 
                                dir.X, dir.Y, 0, finalColor);
                        }

                        if (armorModeTimer < 120)
                        {
                            flashbombDuration = 2;
                            armorModeTimer++;
                        } else
                        {
                            Player.AddBuff(ModContent.BuffType<ArmorMode>(), 300);
                            percent = 0;
                            armorModeTimer = 0;

                            for (int i = 0; i < 22; i++)
                            {
                                Vector2 dir = new Vector2(8, 0).
                                    RotatedByRandom(MathHelper.TwoPi) + Player.velocity;
                                int index = Dust.NewDust(Player.Center, 0, 0, DustID.TintableDustLighted,
                                    dir.X, dir.Y, 0, Color.LightPink);
                            }
                        }
                    }
                    break;
            }
        }

        private void FlashbombModifyStats(Flashbomb type, int duration)
        {
            switch (type)
            {
                case Flashbomb.Suzumi:
                    Player.aggro -= 10000;
                    break;
                default:
                    break;
            }
        }

        private float FlashbombPercentUsage(Flashbomb type)
        {
            switch (type)
            {
                case Flashbomb.Suzumi:
                    return fillRate * 2;
                case Flashbomb.ArmorMode:
                    return 0;
            }
            return 1;
        }

        private int FlashbombBegin(Flashbomb type)
        {
            Vector2 direction;
            direction = Player.Center.DirectionTo(Main.MouseWorld);
            int typeAsInt = (int)type;
            switch (type)
            {
                case Flashbomb.MaidenPit:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, 
                        ai0: typeAsInt);
                    return 45;
                case Flashbomb.RememberedRemnants:
                    // Default flashbomb, but include a scapegoat
                    direction = Player.Center.DirectionTo(Main.MouseWorld);
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Bosses, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.DD2OgreKnockback, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.TileContactDamage, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Lava, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.WrongBugNet, 30);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + (direction * 60),
                        direction, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: -1);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, 
                        ai0: typeAsInt);
                    return 30;
                case Flashbomb.NegativeAndPositive:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: typeAsInt);
                    return 15;
                case Flashbomb.Wormhole:
                    // Default flashbomb, but include a wormhole
                    direction = Player.Center.DirectionTo(Main.MouseWorld);
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Bosses, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.DD2OgreKnockback, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.TileContactDamage, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Lava, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.WrongBugNet, 30);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + (direction * 60),
                        direction, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: -1);
                    // Only make a new wormhole if there is no wormhole active.
                    // This will inevitably activate after the player uses the wormhole as well
                    // Note: Apparently, it's not activated after the player uses a wormhole
                    if (!CheckWormhole())
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: typeAsInt);
                    }
                    return 30;
                case Flashbomb.LostTorus:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, 
                        ai0: typeAsInt);
                    return 60;
                case Flashbomb.BlackRopes:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Main.MouseWorld,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: typeAsInt);
                    return 30;
                case Flashbomb.DimensionalDeletion:
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Player.Center.DirectionTo(Main.MouseWorld) * 8, 
                        ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: typeAsInt);
                    return 15;
                case Flashbomb.Suzumi:
                    percent = 1;
                    return 1;
                case Flashbomb.VertexEmit:
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Bosses, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.DD2OgreKnockback, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.TileContactDamage, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Lava, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.WrongBugNet, 30);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        Vector2.Zero, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: typeAsInt);
                    return 30;
                case Flashbomb.MonochromeFlash:
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 40);
                    Player.AddImmuneTime(ImmunityCooldownID.Bosses, 40);
                    Player.AddImmuneTime(ImmunityCooldownID.DD2OgreKnockback, 40);
                    Player.AddImmuneTime(ImmunityCooldownID.TileContactDamage, 40);
                    Player.AddImmuneTime(ImmunityCooldownID.Lava, 40);
                    Player.AddImmuneTime(ImmunityCooldownID.WrongBugNet, 40);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                        direction, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI,
                        ai0: typeAsInt);
                    return 40;
                case Flashbomb.ArmorMode:
                    percent = 1;
                    armorModeTimer = 0;
                    return 1;
                default:
                    Player.immune = true;
                    Player.AddImmuneTime(ImmunityCooldownID.General, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Bosses, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.DD2OgreKnockback, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.TileContactDamage, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.Lava, 30);
                    Player.AddImmuneTime(ImmunityCooldownID.WrongBugNet, 30);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + (direction * 60), 
                        direction, ModContent.ProjectileType<FlashbombProjectile>(), 0, 0, Player.whoAmI, ai0: 
                        typeAsInt);
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
                lastFlashbombUse != Flashbomb.Suzumi)
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
                    case Flashbomb.Suzumi:
                        Vector2 position = PlayerRenderTarget.getPlayerTargetPosition(drawInfo.drawPlayer.whoAmI);
                        Rectangle sourceRect = PlayerRenderTarget.
                            getPlayerTargetSourceRectangle(drawInfo.drawPlayer.whoAmI);

                        GameShaders.Misc["HeavyNoise"].UseOpacity(1).UseColor(1f, 0f, 0f).Apply(); 
                        Main.spriteBatch.Draw(PlayerRenderTarget.Target, position, sourceRect, Color.White);
                        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                        break;
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
        }
    }
}
