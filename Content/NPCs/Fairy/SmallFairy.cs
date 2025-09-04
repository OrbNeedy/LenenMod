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
        int counter = 0;

        int powerLevel = 0;
        int attackTimer = 0;
        int maxAttackTimer = 0;
        Pattern attackPattern;
        public bool distracted = false;
        public Vector2 distractionPosition = new Vector2(-1);

        public FairyType fairyType = FairyType.Mono;
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

        Vector2 itemPosition = new Vector2();
        float itemRotation = 0;
        int itemUtilityTimer = 0;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ShimmerTownTransform[NPC.type] = false;
            NPCID.Sets.CanHitPastShimmer[NPC.type] = false;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = -1;
            NPCID.Sets.ShimmerTransformToItem[NPC.type] = -1;

            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.SpawnsWithCustomName[Type] = false;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { 
                CustomTexturePath = "lenen/Content/NPCs/Fairy/SmallFairy", 
                Position = new Vector2(0f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

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

            itemUtilityTimer = 0;
            attackTimer = 300;
            maxAttackTimer = 300;
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
                return SpawnCondition.Sky.Chance * 0.275f;
            }
            if (spawnInfo.Player.ZoneOverworldHeight)
            {
                return SpawnCondition.Overworld.Chance * 0.15f;
            }
            return 0f;
        }

        public override void ResetEffects()
        {
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.lenen.Bestiary.SmallFairy")),

            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Sake, 60, 1, 5));
            //
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)fairyType);
            writer.Write7BitEncodedInt(powerLevel);
            writer.Write(distracted);
            writer.WriteVector2(distractionPosition);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            fairyType = (FairyType)reader.Read7BitEncodedInt();
            powerLevel = reader.Read7BitEncodedInt();
            distracted = reader.ReadBoolean();
            distractionPosition = reader.ReadVector2();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            if (CheckTarget())
            {
                MoveToTarget();
                
                if (attackTimer <= 0)
                {
                    maxAttackTimer = attackTimer = attackPattern.Shoot(0, powerLevel, NPC, fairyType, 
                        distractionPosition, distracted);
                }
            } else
            {
                MoveAimlessly();
            }
            
            /*Main.NewText($"Target position: {targetPosition}");
            Main.NewText($"Distracted: {distracted}");
            Main.NewText($"Distracted position: {distractionPosition}");*/
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetPosition) * speed, accuracy);

            // From Calamity's public github
            float recoilSpeed = 0.7f;
            if (NPC.collideX)
            {
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
            ControlItem();
            if (attackPattern.CanShoot(0, powerLevel, NPC, fairyType, distractionPosition, distracted)) attackTimer--;
            counter++;
            distracted = false;

            NPC.netUpdate = true;
            base.AI();
        }

        private bool CheckTarget()
        {
            NPC.TargetClosest(false);
            if (NPC.HasValidTarget)
            {
                if (NPC.HasPlayerTarget)
                {
                    Player target = Main.player[NPC.target];
                    if (distracted)
                    {
                        return ((distractionPosition.Distance(NPC.Center) - target.aggro) <= 700);
                    } else
                    {
                        return ((target.Distance(NPC.Center) - target.aggro) <= 700);
                    }
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
            if (distracted)
            {
                NPC.direction = NPC.position.X >= distractionPosition.X ? -1 : 1;
            } else
            {
                NPC.FaceTarget();
            }
            // Slash type fairies like to get in close
            if (fairyType == FairyType.Slash)
            {
                if (NPC.HasPlayerTarget)
                {
                    if (distracted)
                    {
                        targetPosition = distractionPosition;
                    } else
                    {
                        Player player = Main.player[NPC.target];
                        targetPosition = player.Center;
                    }
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
                    if (distracted)
                    {
                        if (distractionPosition.Distance(NPC.Center) > preferedDistance)
                        {
                            targetPosition = distractionPosition;
                        } else
                        {
                            targetPosition = NPC.Center + NPC.Center.DirectionFrom(distractionPosition);
                        }
                    } else
                    {
                        Player player = Main.player[NPC.target];
                        if (player.Distance(NPC.Center) > preferedDistance)
                        {
                            targetPosition = player.Center;
                        } else
                        {
                            targetPosition = NPC.Center + NPC.Center.DirectionFrom(player.Center);
                        }
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

        private void ControlItem()
        {
            Vector2 targetPosition = Vector2.Zero;
            if (NPC.HasValidTarget)
            {
                if (distracted)
                {
                    targetPosition = distractionPosition;
                }
                else
                {
                    targetPosition = Main.player[NPC.target].Center + (Main.player[NPC.target].velocity * 30);
                }
            }

            Vector2 preferredPositionOffset = itemPosition;
            float preferredRotation = itemRotation;

            float rotationPotency = 0.05f;
            float movementPotency = 0.0125f;

            switch (fairyType)
            {
                case FairyType.Slash:
                    movementPotency = 0.005f;
                    if (turning)
                    {
                        movementPotency = 0.05f;
                    }
                    if (NPC.direction == -1)
                    {
                        if (attackTimer <= 10 || attackTimer >= maxAttackTimer-20)
                        {
                            rotationPotency = 0.5f;
                            movementPotency = 0.25f;
                            preferredPositionOffset = new Vector2(2, -48);
                            preferredRotation = -MathHelper.PiOver2 - MathHelper.PiOver4 + 0.01f;
                        } else if (attackTimer <= 120)
                        {
                            movementPotency = 0.025f;
                            preferredPositionOffset = new Vector2(-14, 6);
                            preferredRotation = MathHelper.PiOver4 - 0.01f;
                        } else
                        {
                            preferredPositionOffset = new Vector2(-32, -8 + (float)(Math.Sin(counter*0.01f) * 6f));
                            preferredRotation = MathHelper.Pi + 0.2f;
                        }
                    }
                    else
                    {
                        if (attackTimer <= 10 || attackTimer >= maxAttackTimer - 20)
                        {
                            rotationPotency = 0.5f;
                            movementPotency = 0.25f;
                            preferredPositionOffset = new Vector2(-32, -48);
                            preferredRotation = MathHelper.PiOver2 + MathHelper.PiOver4 - 0.01f;
                        } else if (attackTimer <= 120)
                        {
                            movementPotency = 0.025f;
                            preferredPositionOffset = new Vector2(-16, 6);
                            preferredRotation = -MathHelper.PiOver4 + 0.01f;
                        } else
                        {
                            preferredPositionOffset = new Vector2(2, -8 + (float)(Math.Sin(counter * 0.01f) * 6f));
                            preferredRotation = MathHelper.Pi - 0.2f;
                        }
                    }
                    break;
                case FairyType.Magic:
                    if (NPC.direction == -1)
                    {
                        if (attackTimer <= 120)
                        {
                            preferredPositionOffset = new Vector2(23 + ((float)Math.Cos(counter * 0.01f) * 2),
                                -33 + (float)Math.Sin(counter * 0.02f) * 4);
                            preferredRotation = 0;
                            if (Main.rand.NextBool(3))
                            {
                                for (int i = 0; i < Main.rand.Next(0, 8); i++)
                                {
                                    Dust.NewDust(NPC.position - itemPosition - 
                                        new Vector2(21 * NPC.scale, 21 * NPC.scale).RotatedBy(itemRotation), 
                                        (int)(16*NPC.scale), (int)(16 * NPC.scale), DustID.Enchanted_Pink);
                                }
                            }
                        } else
                        {
                            preferredPositionOffset = new Vector2(-49 + ((float)Math.Cos(counter * 0.01f) * 8), 
                                -25 + (float)Math.Sin(counter * 0.02f) * 10);
                            preferredRotation = MathHelper.PiOver2; 
                        }
                    }
                    else
                    {
                        if (attackTimer <= 120)
                        {
                            preferredPositionOffset = new Vector2(-53 + ((float)Math.Cos(counter * 0.01f) * 2),
                                -33 + (float)Math.Sin(counter * 0.02f) * 4);
                            preferredRotation = 0;
                            if (Main.rand.NextBool(3))
                            {
                                for (int i = 0; i < Main.rand.Next(0, 8); i++)
                                {
                                    Dust.NewDust(NPC.position - itemPosition -
                                        new Vector2(-4 * NPC.scale, 20 * NPC.scale).RotatedBy(itemRotation), 
                                        (int)(16 * NPC.scale), (int)(16 * NPC.scale), DustID.Enchanted_Pink);
                                }
                            }
                        }
                        else
                        {
                            preferredPositionOffset = new Vector2(19 + ((float)Math.Cos(counter * 0.01f) * 8),
                                -25 + (float)Math.Sin(counter * 0.02f) * 10);
                            preferredRotation = -MathHelper.PiOver2;
                        }
                    }
                    break;
                case FairyType.Shot:
                    if (NPC.direction == -1)
                    {
                        if (attackTimer <= 60)
                        {
                            preferredPositionOffset = new Vector2(9, -39);
                            preferredRotation = (NPC.Center - itemPosition).DirectionTo(targetPosition).
                                ToRotation() + MathHelper.Pi;
                            movementPotency = 0.025f;
                        }
                        else
                        {
                            preferredPositionOffset = new Vector2(-45, -21);
                            preferredRotation = MathHelper.PiOver2;
                        }
                    }
                    else
                    {
                        if (attackTimer <= 60)
                        {
                            preferredPositionOffset = new Vector2(-39, -39);
                            preferredRotation = (NPC.Center - itemPosition).DirectionTo(targetPosition).
                                ToRotation();
                            movementPotency = 0.5f;
                        }
                        else
                        {
                            preferredPositionOffset = new Vector2(15, -21);
                            preferredRotation = -MathHelper.PiOver2;
                        }

                    }
                    break;
            }

            // Parts from https://gist.github.com/RonenNess/8b04d8d99ab5d18a24a26d9244b1615b
            while (preferredRotation < 0) { preferredRotation += MathHelper.TwoPi; }
            while (preferredRotation > MathHelper.TwoPi) { preferredRotation -= MathHelper.TwoPi; }

            // Guarantees it takes the shortest possible rotation to the target, only works because it's wraped 
            // around 0 to 2Pi
            if (preferredRotation > MathHelper.Pi)
            {
                preferredRotation -= MathHelper.TwoPi;
            }

            if (preferredPositionOffset.Distance(itemPosition) <= 0.1f)
            {
                itemPosition = preferredPositionOffset;
            }
            else
            {
                itemPosition = Vector2.Lerp(itemPosition, preferredPositionOffset, movementPotency);
            }

            if (Math.Abs(preferredRotation - itemRotation) <= 0.02f)
            {
                itemRotation = preferredRotation;
            }
            else
            {
                itemRotation = float.Lerp(itemRotation, preferredRotation, rotationPotency);
            }
        }

        public override void DrawEffects(ref Color drawColor)
        {
            base.DrawEffects(ref drawColor);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
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
            Texture2D item = null;
            if (fairyType != FairyType.Mono)
            {
                item = ModContent.Request<Texture2D>($"lenen/Content/NPCs/Fairy/{typeString}FairyItem")
                .Value;
            }

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
                        NPC.position - screenPos - (backWingOffset * NPC.scale),
                        backWingBounds,
                        NPC.GetAlpha(drawColor),
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
                        NPC.position - screenPos - (turningWingOffset * NPC.scale),
                        turningWingBounds,
                        NPC.GetAlpha(drawColor),
                        0f,
                        Vector2.Zero,
                        NPC.scale,
                        effects)
                    );
            }

            // Draw main body
            Rectangle bodyBounds = new Rectangle(2 + (34 * bodyXFrame), 2 + (64 * bodyYFrame), 30, 60);
            Main.EntitySpriteDraw(new DrawData(body,
                    NPC.position - screenPos,
                    bodyBounds,
                    NPC.GetAlpha(drawColor),
                    0f,
                    Vector2.Zero,
                    NPC.scale,
                    effects)
                );

            // Draw eyes
            Rectangle eyeBounds = new Rectangle(2 + (34 * bodyXFrame), 2 + (64 * eyeYFrame), 30, 60);
            Main.EntitySpriteDraw(new DrawData(eyes,
                    NPC.position - screenPos,
                    eyeBounds,
                    NPC.GetAlpha(drawColor),
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
                        NPC.position - screenPos - (frontWingOffset * NPC.scale),
                        frontWingBounds,
                        NPC.GetAlpha(drawColor),
                        0f,
                        Vector2.Zero,
                        NPC.scale,
                        effects)
                    );
            }
            
            // Draw the item if possible (a.k.a not a Mono Fairy)
            if (item != null)
            {
                Vector2 origin = Vector2.Zero;
                switch (fairyType)
                {
                    case FairyType.Slash:
                        origin = new Vector2(0, item.Height);
                        if (NPC.direction == -1) origin.X = item.Width;
                        break;
                    default:
                        origin = item.Size() / 2;
                        break;
                }

                Main.EntitySpriteDraw(new DrawData(item,
                    NPC.position - screenPos - (itemPosition * NPC.scale),
                    item.Bounds,
                    Color.White,
                    itemRotation,
                    origin,
                    NPC.scale,
                    effects)
                );
            }
        }
    }
}
