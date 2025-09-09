using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace lenen.Common.Players.Barriers
{
    public class ResurrectionBarrier : Barrier
    {
        public override int MaxCooldown { get; set; } = 300;
        public override int MaxLife { get; set; } = 2;
        public override int MaxRecovery { get; set; } = 300;
        public override int MaxFullRecovery { get; set; } = 3600;
        public override Color TopColor { get; set; } = new Color(239, 190, 227);
        public override Color MidColor { get; set; } = new Color(213, 163, 242);
        public override Color BottomColor { get; set; } = new Color(172, 120, 248);
        public override string IconTexturePath { get; set; } = "HarujionIcon";

        public override void PassiveEffects(Player player)
        {
            player.noKnockback = true;
            base.PassiveEffects(player);
        }

        public override void PreHit(Player player, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.9f;
            base.PreHit(player, ref modifiers);
        }
    }
}
