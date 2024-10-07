using lenen.Common.Players.Barriers;
using System.Collections.Generic;
using Terraria;

namespace lenen.Common
{
    public static class BarrierLookups
    {
        public enum Barriers
        {
            SkullBarrier,
            BetterSkullBarrier,
            DesperateBarrier
        }

        public static Dictionary<Barriers, Barrier> BarrierDictionary = new Dictionary<Barriers, Barrier>()
        {
            [Barriers.SkullBarrier] = new SkullBarrier(),
            [Barriers.BetterSkullBarrier] = new BetterSkullBarrier(),
            [Barriers.DesperateBarrier] = new DesperateBarrier(),
        };

        public static void ResetBarriers()
        {
            BarrierDictionary.Clear();
            BarrierDictionary[Barriers.SkullBarrier] = new SkullBarrier();
            BarrierDictionary[Barriers.BetterSkullBarrier] = new BetterSkullBarrier();
            BarrierDictionary[Barriers.DesperateBarrier] = new DesperateBarrier();
        }
    }
}
