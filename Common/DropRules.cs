using Terraria.GameContent.ItemDropRules;
using Terraria;
using lenen.Content.NPCs.Fairy;

namespace lenen.Common
{
    public class DropGravity : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;
            return Main.hardMode
                && NPC.downedPlantBoss
                && npc.lifeMax > 1
                && !npc.SpawnedFromStatue
                && info.player.ZoneSkyHeight;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Be nearby a gravitational anomaly.";
        }
    }
    public class DropGravity2 : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;
            return Main.hardMode
                && NPC.downedPlantBoss
                && npc.lifeMax > 1
                && !npc.SpawnedFromStatue
                && (info.player.ZoneTowerVortex || info.player.ZoneTowerStardust ||
                info.player.ZoneTowerSolar || info.player.ZoneTowerNebula);
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Be nearby a spatial anomaly.";
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

    public class DropInExpert : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => Main.expertMode;
        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription()
        {
            return "";
        }
    }

    public class FairyTypeCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        FairyType type;

        public FairyTypeCondition(FairyType typeToCheck)
        {
            type = typeToCheck;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            // Check other fairy sizes when they get added
            if (info.npc.ModNPC is SmallFairy fairy)
            {
                return fairy.fairyType == type;
            }
            return false;
        }
        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription()
        {
            return $"Droped by a {type.ToString()} fairy.";
        }
    }
}
