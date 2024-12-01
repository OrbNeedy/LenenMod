using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;

namespace lenen.Common.Players.Barriers
{
    public class BetterSkullBarrier : Barrier
    {
        public BetterSkullBarrier() : base()
        {
            MaxCooldown = 1200;
            Cooldown = 0;
            MaxLife = 5;
            Life = MaxLife;
            MaxRecovery = 900;
            Recovery = 0;
            Colors = [new Color(184, 184, 184), new Color(128, 143, 129), new Color(74, 101, 77)];
        }

        public override string IconPath()
        {
            return "lenen/Assets/Icons/SkullIcon";
        }

        public override void PassiveEffects(Player player)
        {
            player.noKnockback = true;
        }

        public override bool GeneralOnHitLogic(ref Player.HurtModifiers modifiers, Player player, Projectile proj = null, NPC npc = null)
        {
            int flatReduction = 15;
            if (State == 2)
            {
                flatReduction = 5;
            }
            modifiers.FinalDamage.Flat -= flatReduction;
            return base.GeneralOnHitLogic(ref modifiers, player, proj, npc);
        }

        public override bool OnHitByProjectileLogic(Projectile proj, ref Player.HurtModifiers modifiers, Player player)
        {
            proj.friendly = true;
            proj.velocity *= -1;
            modifiers.FinalDamage *= 0.5f;
            return true;
        }

        public override bool OnBreakLogic(int source, Player player)
        {
            bool returnCode = true;
            if (source == 0)
            {
                Recovery = 0;
                Cooldown = MaxCooldown;
                OnBreakEffects(source, player);
                returnCode = false;
            }
            return returnCode;
        }

        public override void OnBreakEffects(int source, Player player)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.Stone,
                    newColor: Color.White);
            }
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier"), player.Center);
        }
    }
}
