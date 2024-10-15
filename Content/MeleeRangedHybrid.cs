using Terraria;
using Terraria.ModLoader;

namespace lenen.Content
{
    public class MeleeRangedHybrid : DamageClass
    {
        public override void SetDefaultStats(Player player)
        {
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic || damageClass == Melee || damageClass == Ranged)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }
        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Ranged)
                return true;

            return false;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Ranged)
                return true;

            return base.GetPrefixInheritance(damageClass);
        }
    }
}
