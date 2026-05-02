using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace lenen.Common.Players.Barriers
{
    public class GravityBarrier : Barrier
    {
        public override int MaxCooldown { get; set; } = 600;
        public override int MaxLife { get; set; } = 5;
        public override int MaxRecovery { get; set; } = 600;
        public override int MaxFullRecovery { get; set; } = 960;
        public override Color TopColor { get; set; } = new Color(86, 1, 76);
        public override Color MidColor { get; set; } = new Color(45, 1, 61);
        public override Color BottomColor { get; set; } = new Color(22, 0, 61);
        public override string IconTextureIndex { get; set; } = "GravityPlusIcon";

        public override List<string> InitializeTextures()
        {
            return [IconTextureIndex, "CheerIcon"];
        }

        public GravityBarrier() : base()
        {
            MaxCooldown = 1560;
            Cooldown = 0;
            MaxLife = 5;
            Life = MaxLife;
            MaxRecovery = 1200;
            Recovery = 0;
        }

        public override void AlwaysOnEffects(Player player)
        {
            if (Variation != 0)
            {
                TopColor = new Color(209, 202, 202);
                MidColor = new Color(207, 103, 99);
                BottomColor = new Color(204, 14, 0);
                IconTextureIndex = "CheerIcon";
            } else
            {
                TopColor = new Color(86, 1, 76);
                MidColor = new Color(45, 1, 61);
                BottomColor = new Color(22, 0, 61);
                IconTextureIndex = "GravityPlusIcon";
            }
        }

        public override void PassiveEffects(Player player)
        {
            GravityPlayer gravityPlayer = player.GetModPlayer<GravityPlayer>();
            gravityPlayer.antiGravity = true;
            gravityPlayer.levitation = true;
            gravityPlayer.gravityResist = true;
        }

        public override void PreHit(Player player, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage.Flat -= 4;

            foreach (NPC attacker in Main.ActiveNPCs)
            {
                if (attacker.friendly) continue;
                float distance = player.Distance(attacker.Center);
                float knockback = 30f * attacker.knockBackResist / (1f + (distance / 500f));
                knockback -= 0.01f;
                if (knockback <= 0.01f) continue;

                player.ApplyDamageToNPC(attacker, 1, knockback, attacker.Center.X > player.Center.X ? 1 : -1);
            }
            base.PreHit(player, ref modifiers);
        }
    }
}
