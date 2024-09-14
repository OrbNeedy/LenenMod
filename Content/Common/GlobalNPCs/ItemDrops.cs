﻿using lenen.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Common.GlobalNPCs
{
    public class ItemDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DropInNormal(), ModContent.ItemType<DimensionalFragment>(), 6));
            }
        }
    }

    public class DropInNormal : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => !Main.expertMode;
        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription()
        {
            return "";
        }
    }
}