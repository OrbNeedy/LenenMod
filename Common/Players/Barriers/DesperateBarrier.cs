using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;
using Terraria.ID;
using Terraria.Audio;

namespace lenen.Common.Players.Barriers
{
    public class DesperateBarrier : Barrier
    {
        public DesperateBarrier() : base()
        {
            MaxCooldown = 1200;
            Cooldown = 0;
            MaxLife = 10;
            Life = MaxLife;
            MaxRecovery = 900;
            Recovery = 0;
            Colors = [new Color(141, 168, 132), new Color(194, 182, 134), new Color(236, 188, 133)];
        }

        public override string IconPath { get => "lenen/Assets/Icons/DesperateIcon"; }

        public override bool GeneralOnHitLogic(ref Player.HurtModifiers modifiers, Player player, Projectile proj = null, NPC npc = null)
        {
            modifiers.FinalDamage.Flat -= 10;
            if (State == 2)
            {
                modifiers.FinalDamage.Base -= 10;
                modifiers.FinalDamage *= 0.9f;
            }
            return true;
        }

        public override void OnRebuildEffects(Player player)
        {
            if (State == 1)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
                }
            }
            if (State == 2)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration,
                        newColor: Color.DarkRed);
                }
            }
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier_on"), player.Center);
        }

        public override void OnBreakEffects(int source, Player player)
        {
            if (State == 1)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
                }
            }
            if (State == 2)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration,
                        newColor: Color.DarkRed);
                }
            }
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier"), player.Center);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (State == 2)
            {
                float value = 1f;
                float value2 = 0.6f + ((float)Math.Sin(Main.GameUpdateCount * 0.006) * 0.2f);
                float value3 = 0.5f;
                r = value;
                g = value2;
                b = value3;
            }
        }
    }
}
