using System.Collections.Generic;
using Terraria;

namespace lenen.Common
{
    public delegate void OnUpgradeCallback(ref List<int> result);
    class AwakeningData
    {
        public List<int> ingredients = new();
        public List<Condition> conditions = new();
        public OnUpgradeCallback OnUpgrade = null;
        public List<int> result = new();

        public AwakeningData(List<int> ingredients, List<Condition> conditions, 
            OnUpgradeCallback upgradeCallback, List<int> result)
        {
            this.ingredients = ingredients;
            this.conditions = conditions;
            OnUpgrade = upgradeCallback;
            this.result = result;
        }
    }
}
