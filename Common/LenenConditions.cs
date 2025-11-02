using lenen.Common.Players;
using Terraria;
using Terraria.Localization;

namespace lenen.Common
{
    public static class LenenConditions
    {
        public static int GashadokuroUpgradeSpirits = 1500;

        public static Condition HasEnoughSouls = new Condition(Language.GetTextValue("Mods.lenen.Conditions.HasEnoughSouls", 
            GashadokuroUpgradeSpirits),
            () => Main.LocalPlayer.GetModPlayer<SoulAbsorptionPlayer>().soulsCollected >= GashadokuroUpgradeSpirits);

        public static Condition DownedAnyTower = new Condition(Language.GetTextValue("Mods.lenen.Conditions.AnyTower"),
            () => Condition.DownedSolarPillar.IsMet() || Condition.DownedStardustPillar.IsMet() ||
            Condition.DownedVortexPillar.IsMet() || Condition.DownedNebulaPillar.IsMet());

        public static Condition HasSenri = new Condition(Language.GetTextValue("Mods.lenen.Conditions.IsSenri"),
            () => Main.LocalPlayer.GetModPlayer<SenriPlayer>().senriActive);
    }
}
