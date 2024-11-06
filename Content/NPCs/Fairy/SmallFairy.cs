using lenen.Content.EmoteBubbles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace lenen.Content.NPCs.Fairy
{
    public class SmallFairy : ModNPC
    {
        int powerLevel = 0;

        string fairyType = "Mono";
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
        }

        public override string Texture => "lenen/Assets/Textures/Empty";

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 60;
            NPC.aiStyle = -1;
            NPC.damage = 10;
            NPC.defense = 0;
            NPC.knockBackResist = 0.2f;
            
            NPC.lifeMax = 20;
            NPC.HitSound = SoundID.Pixie;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 50f;

            NPC.GravityIgnoresType = true;
            NPC.GravityIgnoresLiquid = true;
            NPC.GravityIgnoresSpace = true;
            NPC.shimmering = false;
            NPC.noGravity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Select type at random (Mono, Magic, Gun, Sword)
            switch(Main.rand.Next(4))
            {
                case 0:
                    fairyType = "Magic";
                    break;
                case 1:
                    fairyType = "Slash";
                    break;
                case 2:
                    fairyType = "Shot";
                    break;
                default:
                    fairyType = "Mono";
                    break;
            }
            NPC.setNPCName(fairyType, Type);
            base.OnSpawn(source);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Overworld.Chance * 0.1f;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            ControlEyes();
            ControlBody();
            ControlTurning();
            ControlWings();
            base.AI();
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

        private void ControlTurning()
        {

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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D backWings = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{fairyType}FairyWingsBack").Value;
            Texture2D turningWings = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{fairyType}FairyWingsTurning").Value;
            Texture2D body = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{fairyType}Fairy").Value;
            Texture2D eyes = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{fairyType}FairyEye").Value;
            Texture2D frontWings = ModContent.Request<Texture2D>(
                $"lenen/Content/NPCs/Fairy/{fairyType}FairyWings").Value;

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
