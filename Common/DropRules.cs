using Terraria.GameContent.ItemDropRules;
using Terraria;

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
}
