using Terraria.GameContent.ItemDropRules;
using Terraria;

namespace lenen.Common
{
    public class DropSouls : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "";
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
