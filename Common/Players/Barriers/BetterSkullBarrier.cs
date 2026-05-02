using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace lenen.Common.Players.Barriers
{
    public class BetterSkullBarrier : Barrier
    {
        public override int MaxCooldown { get; set; } = 600;
        public override int MaxLife { get; set; } = 5;
        public override int MaxRecovery { get; set; } = 300;
        public override int MaxFullRecovery { get; set; } = 600;
        public override Color TopColor { get; set; } = new Color(74, 101, 77);
        public override Color MidColor { get; set; } = new Color(128, 143, 129);
        public override Color BottomColor { get; set; } = new Color(184, 184, 184);
        public override string IconTextureIndex { get; set; } = "SkullIcon";

        public override List<string> InitializeTextures()
        {
            return [IconTextureIndex];
        }

        public override void PassiveEffects(Player player)
        {
            player.noKnockback = true;
        }

        public override void PreHitByProjectile(Player player, Projectile proj, ref Player.HurtModifiers modifiers)
        {
            proj.friendly = true;
            proj.velocity *= -1;
            modifiers.FinalDamage *= 0.5f;
        }

        public override void PreHit(Player player, ref Player.HurtModifiers modifiers)
        {
            int flatReduction = 15;
            if (Variation == 1)
            {
                flatReduction = 5;
            }
            modifiers.FinalDamage.Flat -= flatReduction;
        }
    }
}
