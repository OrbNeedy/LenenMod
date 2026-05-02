using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace lenen.Common.Players.Barriers
{
    public class SkullBarrier : Barrier
    {
        public override int MaxCooldown { get; set; } = 600;
        public override int MaxLife { get; set; } = 2;
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

        public override void PreHit(Player player, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage.Flat -= 10;
            base.PreHit(player, ref modifiers);
        }

        public override void PreHitByProjectile(Player player, Projectile proj, ref Player.HurtModifiers modifiers)
        {
            proj.friendly = true;
            proj.velocity *= -1;
            modifiers.FinalDamage *= 0.5f;
        }

        public override void OnBreak(Player player, Player.HurtInfo info)
        {
            Entity entity = null;
            bool success = info.DamageSource.TryGetCausingEntity(out entity);

            if (success && entity is Projectile projectile)
            {
                Cooldown = MaxCooldown * 2;
            }
        }
    }
}
