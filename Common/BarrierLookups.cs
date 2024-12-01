using lenen.Common.Players.Barriers;
using System.Collections.Generic;

namespace lenen.Common
{
    public static class BarrierLookups
    {
        public enum Barriers
        {
            SkullBarrier,
            BetterSkullBarrier,
            HarujionBarrier,
            DesperateBarrier,
            GravityBarrier
        }

        public static Dictionary<Barriers, Barrier> BarrierDictionary = new Dictionary<Barriers, Barrier>()
        {
            [Barriers.SkullBarrier] = new SkullBarrier(),
            [Barriers.BetterSkullBarrier] = new BetterSkullBarrier(),
            [Barriers.HarujionBarrier] = new ResurrectionBarrier(),
            [Barriers.DesperateBarrier] = new DesperateBarrier(),
            [Barriers.GravityBarrier] = new GravityBarrier(),
        };

        public static void ResetBarriers()
        {
            BarrierDictionary.Clear();
            BarrierDictionary[Barriers.SkullBarrier] = new SkullBarrier();
            BarrierDictionary[Barriers.BetterSkullBarrier] = new BetterSkullBarrier();
            BarrierDictionary[Barriers.HarujionBarrier] = new ResurrectionBarrier();
            BarrierDictionary[Barriers.DesperateBarrier] = new DesperateBarrier();
            BarrierDictionary[Barriers.GravityBarrier] = new GravityBarrier();
        }
    }
}
