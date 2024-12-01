using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using lenen.Content.NPCs.Fairy.Patterns;
using Terraria.GameContent.ItemDropRules;
using System.IO;
using System;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;

namespace lenen.Content.NPCs.Fairy
{
    // TODO: Apply the hunter effect manually to the sprite
    public enum FairyType
    {
        Mono, 
        Slash, 
        Magic, 
        Shot
    }

    public class SmallFairy : ModNPC
    {
        private static readonly FairyType[] _fairyTypeValues = Enum.GetValues<FairyType>();

        Vector2 targetPosition = Vector2.Zero;
        float speed = 2.25f;
        float accuracy = 0.06f;

        int powerLevel = 0;
        int attackTimer = 0;
        Pattern attackPattern;

        FairyType fairyType = FairyType.Mono;
        bool closing = false;
        int blinking = 0;
        int blinkTimer = 0;

        bool growing = false;
        int wingTimer = 0;

        bool flowingLeft = false;
        bool turning = false;
        int bodyTimer = 0;
        int previousDirection = -1;
        int turningTimer = 0;

        int wingFrame = 0;
        int bodyXFrame = 0;
        int bodyYFrame = 0;
        int eyeYFrame = 0;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ShimmerTownTransform[NPC.type] = false;
            NPCID.Sets.CanHitPastShimmer[NPC.type] = false;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = -1;
            NPCID.Sets.ShimmerTransformToItem[NPC.type] = -1;

            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.SpawnsWithCustomName[Type] = false;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "lenen/Content/NPCs/Fairy/SmallFairy", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = new Vector2(0f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        //public override string Texture => "lenen/Assets/Textures/Empty";

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 60;
            NPC.aiStyle = -1;
            NPC.damage = 15;
            NPC.defense = 8;
            NPC.knockBackResist = 0.2f;
            
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCHit54;
            NPC.value = 50f;
            NPC.rarity = 0;
            NPC.townNPC = false;

            NPC.GravityIgnoresType = true;
            NPC.GravityIgnoresLiquid = true;
            NPC.GravityIgnoresSpace = true;
            NPC.shimmering = false;
            NPC.noGravity = true;
            attackTimer = 300;
        }

        public override void OnSpawn(IEntitySource source)
        {
            attackPattern = Main.rand.NextFromList(new Pattern());
            if (Main.hardMode)
            {
                attackPattern = Main.rand.NextFromList(new Pattern(), new Shapes());
            }
            fairyType = Main.rand.Next(_fairyTypeValues);
            int min = GetMinLevel();
            int max = GetMaxLevel();
            if (min >= max)
            {
                min = max - 1;
            }
            powerLevel = (int)MathHelper.Clamp(Main.rand.Next(min, max), 0, 19);
            attackPattern.SetDefaults(0, powerLevel, NPC, fairyType);
            base.OnSpawn(source);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneNormalSpace)
            {
                return SpawnCondition.Sky.Chance * 0.25f;
            }
            if (spawnInfo.Player.ZoneOverworldHeight)
            {
                return SpawnCondition.Overworld.Chance * 0.125f;
            }
            return 0f;
        }

        public override void ModifyTypeName(ref string typeName)
        {
            typeName = Language.GetTextValue($"Mods.lenen.Fairy.Small.{fairyType}");
            base.ModifyTypeName(ref typeName);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (fairyType == FairyType.Slash)
            {
                modifiers.FinalDamage *= 2;
            }
            base.ModifyHitPlayer(target, ref modifiers);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (fairyType == FairyType.Slash)
            {
                modifiers.FinalDamage *= 2;
            }
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Sake, 60, 1, 5));
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)fairyType);
            writer.Write7BitEncodedInt(powerLevel);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            fairyType = (FairyType)reader.Read7BitEncodedInt();
            powerLevel = reader.Read7BitEncodedInt();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            if (CheckTarget())
            {
                MoveToTarget();
                
                if (attackTimer <= 0)
                {
                    attackTimer = attackPattern.Shoot(0, powerLevel, NPC, fairyType);
                }
            } else
            {
                MoveAimlessly();
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetPosition) * speed, accuracy);

            // From Calamity's public github
            float recoilSpeed = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.velocity.X * -recoilSpeed;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -recoilSpeed;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1.5f)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1.5f)
                {
                    NPC.velocity.Y = -2f;
                }
            }

            ControlEyes();
            ControlBody();
            ControlWings();
            attackTimer--;
            NPC.netUpdate = true;
            base.AI();
        }

        private bool CheckTarget()
        {
            NPC.TargetClosest(true);
            if (NPC.HasValidTarget)
            {
                if (NPC.HasPlayerTarget)
                {
                    Player target = Main.player[NPC.target];
                    return ((target.Distance(NPC.Center)-target.aggro) <= 700);
                } else
                {
                    return false;
                }
            } else
            {
                return false;
            }
        }

        private void MoveAimlessly()
        {
            targetPosition = NPC.Center + new Vector2(NPC.direction, 0);
            speed = 2.25f;
            accuracy = 0.08f;
        }

        private void MoveToTarget()
        {
            NPC.FaceTarget();
            // Slash type fairies like to get in close
            if (fairyType == FairyType.Slash)
            {
                if (NPC.HasPlayerTarget)
                {
                    Player player = Main.player[NPC.target];
                    targetPosition = player.Center;
                    speed = 3;
                }
                if (NPC.HasNPCTarget)
                {
                    NPC target = Main.npc[NPC.target];
                    targetPosition = target.Center;
                    speed = 3;
                }
            } else
            {
                // Other fairies prefer to stay at a distance
                float preferedDistance = 325;
                // Shot fairies specifically like to be farther away from the player
                if (fairyType == FairyType.Shot) preferedDistance = 400;
                if (NPC.HasPlayerTarget)
                {
                    Player player = Main.player[NPC.target];
                    if (player.Distance(NPC.Center) > preferedDistance)
                    {
                        targetPosition = player.Center;
                    } else
                    {
                        targetPosition = NPC.Center + NPC.Center.DirectionFrom(player.Center);
                    }
                    speed = 3;
                }
                if (NPC.HasNPCTarget)
                {
                    NPC target = Main.npc[NPC.target];
                    if (target.Distance(NPC.Center) > preferedDistance)
                    {
                        targetPosition = target.Center;
                    }
                    else
                    {
                        targetPosition = NPC.Center + NPC.Center.DirectionFrom(target.Center);
                    }
                    speed = 3;
                }
            }
            speed = 3f;
            accuracy = 0.04f;
        }

        private Vector2 CheckTerrain()
        {
            // Preventive measures
            Vector2 nextTarget = new Vector2(NPC.direction, 0);

            //Main.NewText("Next target: " + nextTarget);
            return nextTarget;
        }

        private void ControlEyes()
        {
            blinkTimer++;
            if (blinkTimer >= 300)
            {
                blinkTimer = 0;
                closing = true;
            }

            if (closing)
            {
                if (eyeYFrame < 2)
                {
                    blinking++;
                    if (blinking >= 6)
                    {
                        eyeYFrame++;
                        blinking = 0;
                    }
                } else
                {
                    blinking = 0;
                    closing = false;
                }
            } else
            {
                if (eyeYFrame > 0)
                {
                    blinking++;
                    if (blinking >= 6)
                    {
                        eyeYFrame--;
                        blinking = 0;
                    }
                }
            }
        }

        private void ControlBody()
        {
            if (previousDirection != NPC.direction)
            {
                turning = true;
            }
            if (turning)
            {
                bodyXFrame = 1;
                turningTimer++;
                if (turningTimer >= 20)
                {
                    turningTimer = 0;
                    bodyXFrame = 0;
                    turning = false;
                }
            }
            bodyTimer++;
            if (bodyTimer >= 25)
            {
                bodyTimer = 0;
                if (!flowingLeft)
                {
                    bodyYFrame++;
                    if (bodyYFrame > 2)
                    {
                        bodyYFrame = 1;
                        flowingLeft = true;
                    }
                } else
                {
                    bodyYFrame--;
                    if (bodyYFrame < 0)
                    {
                        bodyYFrame = 1;
                        flowingLeft = false;
                    }
                }
            }
            previousDirection = NPC.direction;
        }

        private void ControlWings()
        {
            wingTimer++;
            if (wingTimer >= 15)
            {
                wingTimer = 0;
                if (!growing)
                {
                    wingFrame++;
                    if (wingFrame > 2)
                    {
                        wingFrame = 1;
                        growing = true;
                    }
                } else
                {
                    wingFrame--;
                    if (wingFrame < 0)
                    {
                        wingFrame = 1;
                        growing = false;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                // No gore because kids don't die in Terraria 
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(NPC.position, 30, 52, DustID.Smoke);
                }
            }
        }

        public int GetMinLevel()
        {
            int finalModifier = 0;
            if (NPC.downedSlimeKing || NPC.downedGoblins) finalModifier++;
            if (NPC.downedDeerclops) finalModifier++;
            if (NPC.downedQueenBee) finalModifier++;
            if (Main.hardMode) finalModifier += 2;
            if (NPC.downedPirates) finalModifier++;
            if (NPC.downedQueenSlime) finalModifier++;
            if (NPC.downedMechBossAny) finalModifier++;
            if (NPC.downedGolemBoss) finalModifier++;
            if (NPC.downedMartians) finalModifier++;
            if (NPC.downedFishron) finalModifier += 2;
            if (NPC.downedEmpressOfLight) finalModifier += 2;
            if (NPC.downedMoonlord) finalModifier += 5;
            return finalModifier;
        }

        public int GetMaxLevel()
        {
            int finalModifier = 2;
            if (NPC.downedSlimeKing || NPC.downedBoss1) finalModifier++;
            if (NPC.downedBoss1) finalModifier++;
            if (NPC.downedBoss2) finalModifier++;
            if (NPC.downedBoss3) finalModifier++;
            if (Main.hardMode) finalModifier += 2;
            if (NPC.downedMechBossAny) finalModifier++;
            if (NPC.downedMechBoss1 && NPC.downedMechBoss1 && NPC.downedMechBoss1) finalModifier++;
            if (NPC.downedPlantBoss) finalModifier += 2;
            if (NPC.downedGolemBoss) finalModifier++;
            if (NPC.downedFishron) finalModifier++;
            if (NPC.downedTowerNebula || NPC.downedTowerSolar ||
                NPC.downedTowerStardust || NPC.downedTowerVortex) finalModifier++;
            if (NPC.downedMoonlord)
            {
                finalModifier += 5;
            }
            return finalModifier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.lenen.Bestiary.SmallFairy")),

				// By default the last added IBestiaryBackgroundImagePathAndColorProvider will be used to show the background image.
				// The ExampleSurfaceBiome ModBiomeBestiaryInfoElement is automatically populated into bestiaryEntry.Info prior to this method being called
				// so we use this line to tell the game to prioritize a specific InfoElement for sourcing the background image.
            });
        }

        public override void DrawEffects(ref Color drawColor)
        {
            base.DrawEffects(ref drawColor);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            string typeString = fairyType.ToString();
            Texture2D backWings = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{typeString}FairyWingsBack").Value;
            Texture2D turningWings = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{typeString}FairyWingsTurning").Value;
            Texture2D body = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{typeString}Fairy").Value;
            Texture2D eyes = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{typeString}FairyEye").Value;
            Texture2D frontWings = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{typeString}FairyWings").Value;

            SpriteEffects effects = SpriteEffects.None;
            if (NPC.direction != 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            if (!turning)
            {
                // Draw back wings
                Rectangle backWingBounds = new Rectangle(2, 2 + (64 * wingFrame), 38, 64);
                Vector2 backWingOffset = new Vector2(24, 4);
                if (NPC.direction == -1)
                {
                    backWingOffset.X = -16;
                }
                Main.EntitySpriteDraw(new DrawData(backWings,
                        NPC.position - Main.screenPosition - (backWingOffset * NPC.scale),
                        backWingBounds,
                        drawColor,
                        0f,
                        Vector2.Zero,
                        NPC.scale,
                        effects)
                    );
            }

            if (turning)
            {
                // Draw forward-facing wings
                Rectangle turningWingBounds = new Rectangle(2, 2 + (70 * wingFrame), 106, 66);
                Vector2 turningWingOffset = new Vector2(38, 4);
                if (NPC.direction == -1)
                {
                    turningWingOffset.X = 38;
                }
                Main.EntitySpriteDraw(new DrawData(turningWings,
                        NPC.position - Main.screenPosition - (turningWingOffset * NPC.scale),
                        turningWingBounds,
                        drawColor,
                        0f,
                        Vector2.Zero,
                        NPC.scale,
                        effects)
                    );
            }

            // Draw main body
            Rectangle bodyBounds = new Rectangle(2 + (34 * bodyXFrame), 2 + (64 * bodyYFrame), 30, 60);
            Main.EntitySpriteDraw(new DrawData(body,
                    NPC.position - Main.screenPosition,
                    bodyBounds,
                    drawColor,
                    0f,
                    Vector2.Zero,
                    NPC.scale,
                    effects)
                );

            // Draw eyes
            Rectangle eyeBounds = new Rectangle(2 + (34 * bodyXFrame), 2 + (64 * eyeYFrame), 30, 60);
            Main.EntitySpriteDraw(new DrawData(eyes,
                    NPC.position - Main.screenPosition,
                    eyeBounds,
                    drawColor,
                    0f,
                    Vector2.Zero,
                    NPC.scale,
                    effects)
                );

            if (!turning)
            {
                // Draw front wings
                Rectangle frontWingBounds = new Rectangle(2, 2 + (74 * wingFrame), 50, 74);
                Vector2 frontWingOffset = new Vector2(44, 12);
                if (NPC.direction == -1)
                {
                    frontWingOffset.X = -24;
                }
                Main.EntitySpriteDraw(new DrawData(frontWings,
                        NPC.position - Main.screenPosition - (frontWingOffset * NPC.scale),
                        frontWingBounds,
                        drawColor,
                        0f,
                        Vector2.Zero,
                        NPC.scale,
                        effects)
                    );
            }
            return false;
        }
    }
}
