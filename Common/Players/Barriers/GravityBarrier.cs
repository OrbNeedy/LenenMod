using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace lenen.Common.Players.Barriers
{
    public class GravityBarrier : Barrier
    {
        public GravityBarrier() : base()
        {
            MaxCooldown = 1560;
            Cooldown = 0;
            MaxLife = 5;
            Life = MaxLife;
            MaxRecovery = 1200;
            Recovery = 0;
            Colors = [new Color(22, 0, 61), new Color(45, 1, 61), new Color(86, 1, 76)];
        }

        public override string IconPath()
        {
            string path = "lenen/Assets/Icons/GravityPlusIcon";
            if (State == 2)
            {
                path = "lenen/Assets/Icons/CheerIcon";
            }
            return path;
        }

        public override void PassiveEffects(Player player)
        {
            player.GetModPlayer<GravityPlayer>().antiGravity = true;
            player.GetModPlayer<GravityPlayer>().levitation = true;
            player.GetModPlayer<GravityPlayer>().gravityResist = true;
            if (State != 2)
            {
                Colors = [new Color(22, 0, 61), new Color(45, 1, 61), new Color(86, 1, 76)];
            } else
            {
                Colors = [new Color(204, 14, 0), new Color(207, 103, 99), new Color(209, 202, 202)];
            }
        }

        public override bool GeneralOnHitLogic(ref Player.HurtModifiers modifiers, Player player, Projectile proj = null, NPC npc = null)
        {
            modifiers.FinalDamage.Flat -= 4;
            return true;
        }

        public override bool OnHitByNPCLogic(NPC npc, ref Player.HurtModifiers modifiers, Player player)
        {
            foreach (NPC attacker in Main.ActiveNPCs)
            {
                if (attacker.friendly) continue;
                float distance = player.Distance(attacker.Center);
                float knockback = 30f * attacker.knockBackResist / (1f + (distance / 500f));
                knockback -= 0.01f;
                if (knockback <= 0.01f) continue;

                NPC.HitInfo hitInfo = new NPC.HitInfo();
                hitInfo.Damage = 1;
                hitInfo.Knockback = knockback;
                hitInfo.HitDirection = attacker.Center.X > player.Center.X ? 1 : -1;
                attacker.StrikeNPC(hitInfo);
                //NetMessage.SendStrikeNPC(attacker, hitInfo);
            }
            return base.OnHitByNPCLogic(npc, ref modifiers, player);
        }

        public override bool OnHitByProjectileLogic(Projectile proj, ref Player.HurtModifiers modifiers, Player player)
        {
            foreach (NPC attacker in Main.ActiveNPCs)
            {
                if (attacker.friendly || attacker.CountsAsACritter) continue;
                float distance = player.Distance(attacker.Center);
                float knockback = 30f * attacker.knockBackResist / (1f + (distance / 400f));
                knockback -= 0.01f;
                if (knockback <= 0.01f) continue;

                NPC.HitInfo hitInfo = new NPC.HitInfo();
                hitInfo.Damage = 1;
                hitInfo.Knockback = knockback;
                hitInfo.HitDirection = attacker.Center.X > player.Center.X ? 1 : -1;
                attacker.StrikeNPC(hitInfo);
                //NetMessage.SendStrikeNPC(attacker, hitInfo);
            }
            return base.OnHitByProjectileLogic(proj, ref modifiers, player);
        }
    }
}
