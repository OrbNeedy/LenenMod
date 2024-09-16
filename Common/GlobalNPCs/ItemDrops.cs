using lenen.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.GlobalNPCs
{
    public class ItemDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                // Bosses
                case NPCID.SkeletronHead:
                    npcLoot.Add(ItemDropRule.ByCondition(new DropInNormal(),
                        ModContent.ItemType<AssassinKnife>(), 3));
                    break;
                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.ByCondition(new DropInNormal(), 
                        ModContent.ItemType<DimensionalFragment>(), 6));
                    break;
                
                // Normal Enemies
                case NPCID.Skeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SkeletonAlien:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SkeletonArcher:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SkeletonAstonaut:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SkeletonCommando:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SkeletonSniper:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.ArmoredSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BoneThrowingSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BoneThrowingSkeleton2:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BoneThrowingSkeleton3:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BoneThrowingSkeleton4:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.GreekSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SkeletonMerchant:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SkeletonTopHat:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.TacticalSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SporeSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SmallSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BigHeadacheSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BigMisassembledSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BigPantlessSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.BigSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.SmallPantlessSkeleton:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
                    break;
                case NPCID.Clothier:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AssassinKnife>(), 2));
                    break;
            }
        }
    }
}
