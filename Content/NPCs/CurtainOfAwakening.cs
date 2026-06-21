using lenen.Common;
using lenen.Common.Graphics;
using lenen.Common.Players;
using lenen.Content.EmoteBubbles;
using lenen.Content.Items.Accessories;
using lenen.Content.Items.Accessories.Hooaka;
using lenen.Content.Items.Accessories.Tsurubami;
using lenen.Content.Items.Misc;
using lenen.Content.Items.Vanity.Dyes;
using lenen.Content.Items.Weapons.Clause;
using lenen.Content.Items.Weapons.Kurohebi;
using lenen.Content.Items.Weapons.Kuroji;
using lenen.Content.Items.Weapons.Suzumi;
using lenen.Content.Items.Weapons.Yabusame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace lenen.Content.NPCs
{
    public class CurtainOfAwakening : ModNPC
    {
        private List<AwakeningData> awakeningData = new();
        private static Profiles.StackedNPCProfile NPCProfile;
        private int timeSpent = 0;

        private int AnimationFrame = 0;
        private int AnimationTime = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.ShimmerTownTransform[NPC.type] = false;
            NPCID.Sets.CanHitPastShimmer[NPC.type] = true;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = -1;
            NPCID.Sets.ShimmerTransformToItem[NPC.type] = -1;

            NPCID.Sets.ImmuneToAllBuffs[NPC.type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.SpawnsWithCustomName[Type] = false;
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);

            NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<NPCEmotes>();

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 0f, 
                Direction = 1
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            /*
                [ModContent.ItemType<DimensionalFragment>()] = (new[] { Condition.DownedGolem }, 
                ModContent.ItemType<DimensionalOrbs>()),
                [ModContent.ItemType<AssassinKnife>()] = (new[] { Condition.DownedSkeletron,  },
                ModContent.ItemType<ImprovedKnife>())*/
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Main.NewText("Data: " + awakeningData.Count);
            //Main.NewText("Making data");

            awakeningData = new() {
                new AwakeningData(
                    new() { ModContent.ItemType<DimensionalFragment>() }, new() { Condition.DownedGolem },
                    null, new() { ModContent.ItemType<DimensionalOrbs>() }),
                new AwakeningData(
                    new() { ModContent.ItemType<AssassinKnife>() }, new() { Condition.DownedSkeletron },
                    AssassinKnifeCreate, new() { ModContent.ItemType<ImprovedKnife>() }),
                new AwakeningData(
                    new() { ModContent.ItemType<HooWing>(), ModContent.ItemType<AkaWing>() },
                    new() { Condition.DownedPlantera }, null, new() { ModContent.ItemType<HooakaWings>() }),
                new AwakeningData(
                    new() { ModContent.ItemType<FunctionalBirdDrone>() }, new() { Condition.DownedGolem },
                    null, new() { ModContent.ItemType<TrueBirdDrone>() }
                    ),
                new AwakeningData(
                    new() { ModContent.ItemType<GravitationalAnomaly>() }, new() { LenenConditions.DownedAnyTower },
                    GravityGlobeCreate, new() { ModContent.ItemType<GravityGlobe>() }
                    )
            };

            //Main.NewText("Data: " + awakeningData.Count);
        }

        public void AssassinKnifeCreate(ref List<int> result)
        {
            if (NPC.downedMechBossAny && Main.LocalPlayer.HasItem(ModContent.ItemType<UnstableRock>()))
            {
                int duoPosition = Main.LocalPlayer.FindItem(ModContent.ItemType<UnstableRock>());
                if (Main.LocalPlayer.inventory[duoPosition].stack == 1)
                {
                    Main.LocalPlayer.inventory[duoPosition].TurnToAir();
                }
                else
                {
                    Main.LocalPlayer.inventory[duoPosition].stack -= 1;
                }
                result.Add(ModContent.ItemType<MemoryKnife>());
            }
        }

        public void GravityGlobeCreate(ref List<int> result)
        {
            int duoPosition = -1;
            duoPosition = Main.LocalPlayer.FindItem([ItemID.Present, ItemID.YellowPresent, ItemID.BluePresent,
                ItemID.GreenPresent]);

            if (duoPosition != -1)
            {
                if (Main.LocalPlayer.inventory[duoPosition].stack == 1)
                {
                    Main.LocalPlayer.inventory[duoPosition].TurnToAir();
                }
                else
                {
                    Main.LocalPlayer.inventory[duoPosition].stack -= 1;
                }
                result = new() { ModContent.ItemType<ChristmasGlobe>() };
            }
        }

        public override void SetDefaults()
        {
            NPC.friendly = true; 
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0f;
            NPC.GravityIgnoresType = true;
            NPC.GravityIgnoresLiquid = true;
            NPC.GravityIgnoresSpace = true;
            NPC.shimmering = false;
            NPC.noGravity = true;
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return false;
        }

        public override bool CanChat()
        {
            return true;
        }

        public override bool NeedSaving()
        {
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.lenen.Bestiary.CurtainOfAwakening"))
            });
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false;
        }

        public override bool? CanBeCaughtBy(Item item, Player player)
        {
            return false;
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            return false;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            chat.Add(Language.GetTextValue("Mods.lenen.Dialogue.CurtainOfAwakening.Chat"));
            return chat; 
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { 
            button = Language.GetTextValue("Mods.lenen.Dialogue.CurtainOfAwakening.Awaken");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                //Main.NewText("Data length: " + awakeningData.Count);
                foreach (AwakeningData data in awakeningData)
                {
                    //Main.NewText("Checking one data");
                    bool success = false;
                    List<int> itemIndex = new();

                    // Check all conditions
                    foreach (Condition condition in data.conditions)
                    {
                        //Main.NewText("Condition: " + condition.Description);
                        if (condition.IsMet())
                        {
                            //Main.NewText("Condition met");
                            success = true;
                            break;
                        }
                    }

                    if (!success) continue;

                    // Check all ingredient indexes
                    foreach (int itemID in data.ingredients)
                    {
                        int some = Item.NewItem(NPC.GetSource_FromAI(), Vector2.Zero, itemID);
                        //Main.NewText("Ingredient: " + Main.item[some].Name);
                        bool voidBag = false;
                        int currentIndex = Main.LocalPlayer.FindItemInInventoryOrOpenVoidBag(itemID, out voidBag);
                        if (currentIndex == -1)
                        {
                            // Not all items found
                            //Main.NewText("Not found");
                            success = false;
                            break;
                        } else
                        {
                            itemIndex.Add(currentIndex);
                        }
                    }

                    if (!success) continue;

                    // Delete necesary items
                    foreach (int index in itemIndex)
                    {
                        Item item = Main.LocalPlayer.inventory[index];
                        //Main.NewText("Registered ingredient: " + item.Name);
                        if (item.stack > 1)
                        {
                            // Reduce item stack
                            item.stack -= 1;
                        }
                        else
                        {
                            // Delete item
                            item.TurnToAir();
                        }
                    }

                    // Upgrade callback
                    List<int> finalResult = data.result.ToList();
                    if (data.OnUpgrade != null)
                    {
                        data.OnUpgrade.Invoke(ref finalResult);
                    }

                    // Spawn all items
                    foreach (int result in finalResult)
                    {
                        int some = Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), result);
                        //Main.NewText("Result: " + Main.item[some].Name);
                    }

                    // Try to spawn the dye
                    if (Main.rand.NextBool(4))
                    {
                        int stack = Main.rand.Next(1, 4);
                        Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(),
                            ModContent.ItemType<RiftDye>(), stack);
                    }

                    // Try to spawn the senri priest headpiece
                    if (Main.LocalPlayer.GetModPlayer<SenriPlayer>().canDropSenri)
                    {
                        Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(),
                            ModContent.ItemType<SenriPriestHeadpiece>());
                    }

                    // NPC dies immediatly after success
                    NPC.active = false;
                    // NPC.netSkip = -1;
                    NPC.life = 0;
                    for (int k = 0; k < 15; k++)
                    {
                        Dust.NewDust(NPC.position, 30, 52, DustID.ShimmerSpark, newColor: new(10, 255, 60));
                    }
                    return;
                }

                // If not success, display fail message
                Main.npcChatText = Language.GetTextValue("Mods.lenen.Dialogue.CurtainOfAwakening.Failed");
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(NPC.position, 30, 52, DustID.Smoke);
                }
            }
        }

        public override void AI()
        {
            if (timeSpent >= 18000)
            {
                if (Main._rand.NextBool(120))
                {
                    if (Main._rand.NextBool())
                    {
                        Item.NewItem(NPC.GetSource_Death(), NPC.Center,
                            ModContent.ItemType<SenriPriestHeadpiece>());
                    }
                    NPC.active = false;
                    NPC.life = 0;
                }
            }
            if (Main._rand.NextBool(60))
            {
                Dust.NewDust(NPC.position, 30, 52, DustID.ShimmerSpark, newColor: new(10, 255, 60));
            }
            if (NPC.shimmerWet)
            {
                NPC.velocity = new Vector2(0, -2.5f);
            } else
            {
                NPC.velocity *= 0.95f;
            }
            NPC.homeless = true;
            timeSpent++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (++AnimationTime > 6)
            {
                AnimationTime = 0;
                if (++AnimationFrame >= Main.npcFrameCount[Type])
                {
                    AnimationFrame = 3;
                }
            }
            Texture2D texture = ModContent.Request<Texture2D>("lenen/Content/NPCs/CurtainOfAwakening").Value;
            Texture2D overlay = ModContent.Request<Texture2D>("lenen/Content/NPCs/ShaderOfAwakening").Value;
            int height = texture.Bounds.Height / Main.npcFrameCount[Type];
            SpriteEffects flip = SpriteEffects.None;
            if (NPC.direction == -1) flip = SpriteEffects.FlipHorizontally;
            Rectangle bounds = new Rectangle(0, AnimationFrame*height, 
                texture.Bounds.Width, height);
            
            DrawData data = new DrawData(texture,
                    NPC.position - Main.screenPosition - new Vector2(2, 2),
                    bounds,
                    Color.White * 0f,
                    0f,
                    Vector2.Zero,
                    new Vector2(1f, 1f),
                    flip);

            // Credits to Tyfyter for FastFieldInfo and SpriteBatchMethods
            SpriteBatchState state = SpriteBatchExt.GetState(spriteBatch);

            data.Draw(spriteBatch);

            SpriteBatchExt.Restart(spriteBatch, state, SpriteSortMode.Immediate);

            // Rift Gradient
            MiscShaderData shader = GameShaders.Misc["Rift"].UseColor(0, 1, 0);
            /*shader.Shader.Parameters["leftColor"].
                SetValue(new Vector3(0f, 0f, 0f));
            shader.Shader.Parameters["middleColor"].
                SetValue(new Vector3(0f, 0f, 0f));
            shader.Shader.Parameters["rightColor"].
                SetValue(new Vector3(0f, 0f, 0f));
            Texture2D test = ModContent.Request<Texture2D>("lenen/Content/Projectiles/BigBullet").Value;*/

            DrawData data2 = new DrawData(
                overlay,
                NPC.position - Main.screenPosition - new Vector2(2, 2),
                bounds,
                Color.White * 1f,
                0f,
                Vector2.Zero,
                new Vector2(1f, 1f),
                flip);

            shader.Apply(data2);

            data2.Draw(spriteBatch);

            SpriteBatchExt.Restart(spriteBatch, state);
            return false;
        }
    }
}
