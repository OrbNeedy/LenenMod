using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace lenen.Common.Players.Barriers
{
    public abstract class Barrier
    {
        public virtual string IconPath { get; } = "lenen/Assets/Icons/BarrierIcon";
        public Color[] Colors { get; set; } = new Color[3];
        public int State { get; set; } = 0;
        public int Cooldown { get; set; } = 1;
        public int MaxCooldown { get; set; } = 900;
        public int Life { get; set; } = 1;
        public int MaxLife { get; set; } = 1;
        public int Recovery { get; set; } = 0;
        public int MaxRecovery { get; set; } = 600;

        public Barrier()
        {
        }

        public void Reset()
        {
            State = 0; 
            Cooldown = 0; 
            Life = MaxLife; 
            Recovery = 0;
        }

        public virtual void PassiveEffects(Player player)
        {

        }

        public virtual void OnRebuild(Player player)
        {
            //Main.NewText("Rebuilding " + GetType());
            if (!OnRebuildLogic(player)) return;
            //Main.NewText("Main logic not overriden");
            Cooldown = 0;
            Recovery = 0;
            Life = MaxLife;
            OnRebuildEffects(player);
        }

        public virtual void OnRebuildEffects(Player player)
        {
            //Main.NewText("Playing rebuild effects");
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier_on"), player.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
            }
        }

        public virtual bool OnRebuildLogic(Player player)
        {
            return true;
        }

        public virtual void OnHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers, Player player)
        {
            if (!OnHitByProjectileLogic(proj, ref modifiers, player)) return;
            Life -= 1;

            if (Life <= 0)
            {
                OnBreak(0, player);
            } else
            {
                Recovery = MaxRecovery;
                OnHitByProjectileEffects(proj, ref modifiers, player);
            }
        }

        public virtual void OnHitByProjectileEffects(Projectile proj, ref Player.HurtModifiers modifiers, Player player)
        {
            //Main.NewText("Playing hit effects");
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_hit"), player.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
            }
        }

        public virtual bool OnHitByProjectileLogic(Projectile proj, ref Player.HurtModifiers modifiers, Player player)
        {
            return true;
        }

        public virtual void OnHitByNPC(NPC npc, ref Player.HurtModifiers modifiers, Player player)
        {
            if (!OnHitByNPCLogic(npc, ref modifiers, player)) return;
            Life -= 1;

            if (Life <= 0)
            {
                OnBreak(1, player);
            }
            else
            {
                Recovery = MaxRecovery;
                OnHitByNPCEffects(npc, ref modifiers, player);
            }
        }

        public virtual void OnHitByNPCEffects(NPC npc, ref Player.HurtModifiers modifiers, Player player)
        {
            //Main.NewText("Playing hit effects");
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_hit"), player.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
            }
        }

        public virtual bool OnHitByNPCLogic(NPC npc, ref Player.HurtModifiers modifiers, Player player)
        {
            return true;
        }

        public virtual void OnHitByMisc(ref Player.HurtModifiers modifiers, Player player)
        {
            if (modifiers.DamageSource.SourceOtherIndex == -1) return;
            if (!OnHitByMiscLogic(ref modifiers, player)) return;
            Life -= 1;

            if (Life <= 0)
            {
                OnBreak(2, player);
            }
            else
            {
                Recovery = MaxRecovery;
                OnHitByMiscEffects(ref modifiers, player);
            }
        }

        public virtual void OnHitByMiscEffects(ref Player.HurtModifiers modifiers, Player player)
        {
            /*Main.NewText("Playing hit effects at " + player.Center);
            Main.NewText("Player health: " + player.statLife);
            Main.NewText("Player's id: " + player.whoAmI);*/
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_hit"), player.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
            }
        }

        public virtual bool OnHitByMiscLogic(ref Player.HurtModifiers modifiers, Player player)
        {
            return true;
        }

#nullable enable
        public virtual bool GeneralOnHitLogic(ref Player.HurtModifiers modifiers, Player player, Projectile? proj = null, 
            NPC? npc = null)
        {
            return true;
        }

        public virtual void OnBreak(int source, Player player)
        {
            if (!OnBreakLogic(source, player)) return;
            Recovery = 0;
            Cooldown = MaxCooldown;
            Life = 0;
            OnBreakEffects(source, player);
        }

        public virtual void OnBreakEffects(int source, Player player)
        {
            //Main.NewText("Playing break effects");
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier"), player.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
            }
        }

        public virtual bool OnBreakLogic(int source, Player player)
        {
            return true;
        }

        public virtual void OnRecovery(Player player)
        {
            //Main.NewText("Recovering " + GetType() + " Barrier");
            if (!OnRecoveryLogic(player)) return;
            //Main.NewText("Main recovery logic not overriden");
            Life++;
            if (Life >=  MaxLife)
            {
                // Just in case
                Life = MaxLife;
                OnRebuild(player);
            } else
            {
                Recovery = MaxRecovery;
                OnRecoveryEffects(player);
            }
        }

        public virtual void OnRecoveryEffects(Player player)
        {
            //Main.NewText("Playing recovery effects");
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier_on") with
            {
                Volume = 0.5f
            }, player.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(player.Center - new Vector2(16, 24f), 32, 48, DustID.ManaRegeneration);
            }
        }

        public virtual bool OnRecoveryLogic(Player player)
        {
            return true;
        }

        public virtual void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
        }

        public bool IsAvailable()
        {
            return Life > 0 && State != 0;
        }

        public bool IsFull()
        {
            return Life >= MaxLife && State != 0;
        }
    }
}
