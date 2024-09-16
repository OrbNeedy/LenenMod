using lenen.Content.EmoteBubbles;
using lenen.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
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

        /*public override bool NeedSaving()
        {
            return true;
        }*/

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
                Main.NewText("List: " + items.ToString());
                Main.NewText("Awaken condition: " + CanItemSpawn(items[ModContent.ItemType<AssassinKnife>()]));
                for (int i = 0; i < Main.LocalPlayer.inventory.Length; i++)
                {
                    if (items.Keys.Contains(Main.LocalPlayer.inventory[i].type))
                    {
                        Main.NewText("Hit");
                        int itemType = Main.LocalPlayer.inventory[i].type;
                        if (CanItemSpawn(itemType))
                        {
                            Main.NewText("Hit 2");
                            Main.LocalPlayer.inventory[i].TurnToAir();
                            Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), items[itemType]);

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
                Main.NewText("Miss");
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
        }

        private Dictionary<int, int> GetItems()
        {
            Dictionary<int, int> items = new Dictionary<int, int>
            {
                [ModContent.ItemType<DimensionalFragment>()] = ModContent.ItemType<DimensionalOrbs>(),
                [ModContent.ItemType<AssassinKnife>()] = ModContent.ItemType<ImprovedKnife>()

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
                return NPC.downedBoss3;
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
            int height = texture.Bounds.Height / Main.npcFrameCount[Type];
            Rectangle bounds = new Rectangle(0, AnimationFrame*height, 
                texture.Bounds.Width, height);
            Main.EntitySpriteDraw(
                    texture,
                    NPC.position - Main.screenPosition - new Vector2(2, 2),
                    bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(1f, 1f),
                    SpriteEffects.None
                );
            return false;
        }
    }
}
