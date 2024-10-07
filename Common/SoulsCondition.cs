using lenen.Common.Players;
using lenen.Content.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace lenen.Common
{
    public static class SoulsCondition
    {
        public static Condition HasEnoughSouls = new Condition("Mods.lenen.Conditions.HasEnoughSouls",
            () => Main.LocalPlayer.GetModPlayer<SoulAbsorptionPlayer>().soulsCollected >= 1400);
    }
}
