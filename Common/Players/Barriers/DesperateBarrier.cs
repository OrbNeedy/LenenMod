using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace lenen.Common.Players.Barriers
{
    public class DesperateBarrier : Barrier
    {
        public override int MaxCooldown { get; set; } = 600;
        public override int MaxLife { get; set; } = 10;
        public override int MaxRecovery { get; set; } = 300;
        public override int MaxFullRecovery { get; set; } = 600;
        public override Color TopColor { get; set; } = new Color(236, 188, 133);
        public override Color MidColor { get; set; } = new Color(194, 182, 134);
        public override Color BottomColor { get; set; } = new Color(141, 168, 132);
        public override string IconTextureIndex { get; set; } = "DesperateIcon";

        public override List<string> InitializeTextures()
        {
            return [IconTextureIndex];
        }

        public override void PassiveEffects(Player player)
        {
            if (player.statLife <= player.statLifeMax2 / 2f)
            {
                player.GetModPlayer<SpellCardManagement>().desperateBomb = true;
            }
            base.PassiveEffects(player);
        }

        public override void PreHit(Player player, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage.Flat -= 10;
            if (player.statLife <= player.statLifeMax2 / 2f)
            {
                modifiers.FinalDamage.Base -= 10;
                modifiers.FinalDamage *= 0.9f;
            }
            base.PreHit(player, ref modifiers);
        }
    }
}
