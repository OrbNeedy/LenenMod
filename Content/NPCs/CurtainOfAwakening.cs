using lenen.Content.EmoteBubbles;
using lenen.Content.Items;
using lenen.Content.Items.Accessories;
using lenen.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace lenen.Content.NPCs
{
    public class CurtainOfAwakening : ModNPC
    {
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
                // It needs to be during runtime because on spawn, lists haven't been loaded
                // Probably
                Dictionary<int, int> items = GetItems();
                for (int i = 0; i < Main.LocalPlayer.inventory.Length; i++)
                {
                    if (items.Keys.Contains(Main.LocalPlayer.inventory[i].type))
                    {
                        int itemType = Main.LocalPlayer.inventory[i].type;
                        if (CanItemSpawn(itemType))
                        {
                            Main.LocalPlayer.inventory[i].TurnToAir();
                            Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), items[itemType]);
                            if (Main.rand.NextBool())
                            {
                                int stack = Main.rand.Next(1, 4);
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(),
                                ModContent.ItemType<RiftDye>(), stack);
                            }

                            NPC.active = false;
                            // NPC.netSkip = -1;
                            NPC.life = 0;
                            for (int k = 0; k < 15; k++)
                            {
                                Dust.NewDust(NPC.position, 30, 52, DustID.ShimmerSpark, newColor: new(10, 255, 60));
                            }
                            return;
                        }
                    }
                }
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
                if (Main.rand.NextBool(120))
                {
                    NPC.active = false;
                    NPC.life = 0;
                }
            }
            if (Main.rand.NextBool(60))
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

        private Dictionary<int, int> GetItems()
        {
            Dictionary<int, int> items = new Dictionary<int, int>
            {
                [ModContent.ItemType<DimensionalFragment>()] = ModContent.ItemType<DimensionalOrbs>(),
                [ModContent.ItemType<AssassinKnife>()] = ModContent.ItemType<ImprovedKnife>(),
                [ModContent.ItemType<HooWing>()] = ModContent.ItemType<HooakaWings>(),
                [ModContent.ItemType<AkaWing>()] = ModContent.ItemType<HooakaWings>(),
                [ModContent.ItemType<FunctionalBirdDrone>()] = ModContent.ItemType<TrueBirdDrone>()
            };
            return items;
        }

        private bool CanItemSpawn(int item)
        {
            if (item == ModContent.ItemType<DimensionalFragment>())
            {
                return NPC.downedGolemBoss;
            }
            if (item == ModContent.ItemType<AssassinKnife>())
            {
                if (NPC.downedMechBossAny)
                {
                    if (Main.LocalPlayer.HasItem(ModContent.ItemType<UnstableRock>()))
                    {
                        int duoPosition = Main.LocalPlayer.FindItem(ModContent.ItemType<UnstableRock>());
                        Main.LocalPlayer.inventory[duoPosition].TurnToAir();
                        Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(),
                            ModContent.ItemType<MemoryKnife>(), 1);
                    }
                }
                return NPC.downedBoss3;
            }
            if (item == ModContent.ItemType<HooWing>())
            {
                if (Main.LocalPlayer.HasItem(ModContent.ItemType<AkaWing>()))
                {
                    if (NPC.downedPlantBoss)
                    {
                        int duoPosition = Main.LocalPlayer.FindItem(ModContent.ItemType<AkaWing>());
                        Main.LocalPlayer.inventory[duoPosition].TurnToAir();
                        return true;
                    }
                }
                else return false;
            }
            if (item == ModContent.ItemType<AkaWing>())
            {
                if (Main.LocalPlayer.HasItem(ModContent.ItemType<HooWing>()))
                {
                    if (NPC.downedPlantBoss)
                    {
                        int duoPosition = Main.LocalPlayer.FindItem(ModContent.ItemType<HooWing>());
                        Main.LocalPlayer.inventory[duoPosition].TurnToAir();
                        return true;
                    }
                }
                else return false;
            }
            if (item == ModContent.ItemType<FunctionalBirdDrone>())
            {
                return NPC.downedGolemBoss;
            }
            return false;
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
            //Texture2D overlay = ModContent.Request<Texture2D>("lenen/Content/NPCs/ShaderOfAwakening").Value;
            int height = texture.Bounds.Height / Main.npcFrameCount[Type];
            Rectangle bounds = new Rectangle(0, AnimationFrame*height, 
                texture.Bounds.Width, height);
            //ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<RiftDye>());
            
            DrawData data = new DrawData(texture,
                    NPC.position - Main.screenPosition - new Vector2(2, 2),
                    bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(1f, 1f),
                    SpriteEffects.None);

            /*DrawData data2 = new DrawData(overlay,
                    NPC.position - Main.screenPosition - new Vector2(2, 2),
                    bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(1f, 1f),
                    SpriteEffects.None);*/

            //shader.Apply(Entity, data2);

            Main.EntitySpriteDraw(data);
            //Main.EntitySpriteDraw(data2);
            return false;
        }
    }
}
