using lenen.Content.Items.Accessories;
using lenen.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace lenen.Common.GlobalNPCs
{
    public class ItemDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropGravity(), ModContent.ItemType<GravitationalAnomaly>(), 
                30));
            npcLoot.Add(ItemDropRule.ByCondition(new DropGravity2(), ModContent.ItemType<GravitationalAnomaly>(),
                100));
            /*ItemDropRule.ByCondition(new SimpleItemDropRuleCondition(
            Language.GetText("Condition.Localization.Here"), () => Main.LocalPlayer.zone,
            ShowItemDropInUI.WhenConditionSatisfied),
            ModContent.ItemType<GravitationalAnomaly>()));*/

            // Skeleton clause
            if (NPCID.Sets.Skeletons[npc.type])
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustedKnife>(), 40));
            }

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
                case NPCID.MoonLordCore:
                    npcLoot.Add(ItemDropRule.ByCondition(new DropInNormal(),
                        ModContent.ItemType<AntiGravityCape>(), 4));
                    break;

                // Normal Enemies
                case NPCID.ChaosElemental:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DimensionalFragment>(), 8));
                    break;
                case NPCID.LunarTowerVortex:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GravitationalAnomaly>(), 2));
                    break;

                // Friendly NPCs
                case NPCID.Clothier:
                case NPCID.OldMan:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AssassinKnife>(), 2));
                    
                    /*npcLoot.Add(ItemDropRule.ByCondition(new SimpleItemDropRuleCondition(
                        Language.GetText("Condition.Localization.Here"), () => !Main.expertMode, 
                        ShowItemDropInUI.WhenConditionSatisfied), 
                        ModContent.ItemType<YourItem>()));*/
                    break;
            }
        }
    }
}
