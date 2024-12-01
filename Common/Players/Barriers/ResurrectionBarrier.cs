using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;
using Terraria.ID;
using Terraria.Audio;

namespace lenen.Common.Players.Barriers
{
    public class ResurrectionBarrier : Barrier
    {
        public ResurrectionBarrier() : base()
        {
            MaxCooldown = 600;
            Cooldown = 0;
            MaxLife = 2;
            Life = MaxLife;
            MaxRecovery = 3600;
            Recovery = 0;
            Colors = [new Color(172, 120, 248), new Color(213, 163, 242), new Color(239, 190, 227)];
        }

        public override string IconPath()
        {
            return "lenen/Assets/Icons/HarujionIcon";
        }

        public override void PassiveEffects(Player player)
        {
            player.noKnockback = true;
            base.PassiveEffects(player);
        }

        public override bool GeneralOnHitLogic(ref Player.HurtModifiers modifiers, Player player, Projectile proj = null, NPC npc = null)
        {
            modifiers.FinalDamage *= 0.9f;
            return base.GeneralOnHitLogic(ref modifiers, player, proj, npc);
        }
    }
}
