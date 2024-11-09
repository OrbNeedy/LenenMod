using lenen.Common.Players;
using Terraria;

namespace lenen.Common
{
    public static class SoulsCondition
    {
        public static Condition HasEnoughSouls = new Condition("Mods.lenen.Conditions.HasEnoughSouls",
            () => Main.LocalPlayer.GetModPlayer<SoulAbsorptionPlayer>().soulsCollected >= 1500);
    }
}
