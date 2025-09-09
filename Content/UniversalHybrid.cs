using Terraria.ModLoader;
using Terraria;

namespace lenen.Content
{
    public class UniversalHybrid : DamageClass
    {
        public override void SetDefaultStats(Player player)
        {
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            return StatInheritanceData.Full;
        }
        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return true;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            return true;
        }
    }
}
