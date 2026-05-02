using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace lenen.Common.Players.Barriers
{
    public class BeginerBarrier : Barrier
    {
        public override int MaxCooldown { get; set; } = 210;
        public override int MaxLife { get; set; } = 4;
        public override int MaxRecovery { get; set; } = 180;
        public override int MaxFullRecovery { get; set; } = 900;
        public override Color TopColor { get; set; } = new Color(254, 254, 254);
        public override Color MidColor { get; set; } = new Color(185, 218, 232);
        public override Color BottomColor { get; set; } = new Color(116, 181, 209);
        public override string IconTextureIndex { get; set; } = "BarrierIcon";

        public override List<string> InitializeTextures()
        {
            return [IconTextureIndex];
        }

        public override void PassiveEffects(Player player)
        {
            player.statDefense += 8;
            base.PassiveEffects(player);
        }
    }
}
