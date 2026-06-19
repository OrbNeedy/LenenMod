using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace lenen.Common.Players.Barriers
{
    public abstract class Barrier
    {
        // Visuals
        public virtual Color TopColor { get; set; } = Color.White;
        public virtual Color MidColor { get; set; } = Color.White;
        public virtual Color BottomColor { get; set; } = Color.White;
        public virtual string IconTextureIndex { get; set; } = "BarrierIcon";
        // Current State
        public bool Active { get; set; } = false;
        public bool Broken { get; set; } = false;
        public int Variation { get; set; } = 0; // Accessories that change this also change the behavior of the barrier
        // Stats
        public int Cooldown { get; set; } = 0; // Time before recovery can begin, will play a sound upon reaching 0
        public virtual int MaxCooldown { get; set; } = 0;
        public int Life { get; set; } = 0;
        public virtual int MaxLife { get; set; } = 0;
        public int Recovery { get; set; } = 0; // Timer until one life is recovered or the entire barrier is recovered
        public virtual int MaxRecovery { get; set; } = 0;
        public virtual int MaxFullRecovery { get; set; } = 0;

        public virtual List<string> InitializeTextures()
        {
            return [IconTextureIndex];
        }

        public virtual void Initialize(Player player)
        {
            Life = MaxLife;
        }

        /// <summary>
        /// Called when hit by an NPC.
        /// If the hit is cancelled, call <see cref="ReduceHealth(Player)"/> manually.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="modifiers"></param>
        public virtual void PreHitByNPC(Player player, NPC npc, ref Player.HurtModifiers modifiers)
        {

        }

        /// <summary>
        /// Called when hit by a projectile.
        /// If the hit is cancelled, call <see cref="ReduceHealth(Player)"/> manually.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="modifiers"></param>
        public virtual void PreHitByProjectile(Player player, Projectile proj, ref Player.HurtModifiers modifiers)
        {

        }

        /// <summary>
        /// Called when hit by neither a projectile or an NPC, for example, spikes.
        /// If the hit is cancelled, call <see cref="ReduceHealth(Player)"/> manually.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="modifiers"></param>
        public virtual void PreHitByMisc(Player player, ref Player.HurtModifiers modifiers)
        {

        }

        /// <summary>
        /// Called regardless of hit source.
        /// If the hit is cancelled, call <see cref="ReduceHealth(Player)"/> manually.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="modifiers"></param>
        public virtual void PreHit(Player player, ref Player.HurtModifiers modifiers)
        {

        }

        public virtual void PostHitByNPC(Player player, NPC npc, Player.HurtInfo info)
        {

        }

        public virtual void PostHitByProjectile(Player player, Projectile proj, Player.HurtInfo info)
        {

        }

        public virtual void PostHitByMisc(Player player, Player.HurtInfo info)
        {

        }

        public virtual void PostHit(Player player, Player.HurtInfo info)
        {

        }

        public virtual void AlwaysOnEffects(Player player)
        {

        }

        public virtual void PassiveEffects(Player player)
        {

        }

        public virtual void OnBarrierHit(Player player, Player.HurtInfo info)
        {

        }

        public virtual void OnBreak(Player player, Player.HurtInfo info)
        {

        }

        public virtual void OnRecovery(Player player)
        {

        }

        public virtual void OnRebuild(Player player)
        {

        }

        public void ReduceHealth(Player player, Player.HurtInfo info)
        {
            Life--;
            Cooldown = MaxCooldown;
            if (Life <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier"), player.Center);
                OnBreak(player, info);
                Broken = true;
                Recovery = MaxFullRecovery;
            } else
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_hit"), player.Center);
                OnBarrierHit(player, info);
                Recovery = MaxRecovery;
            }
        }

        public virtual void UniqueResetEffects(Player player)
        {

        }

        public void ResetEffects(Player player)
        {
            UniqueResetEffects(player);
            int oldCooldown = Cooldown;

            if (Cooldown > 0) Cooldown--;

            if (Cooldown <= 0 && oldCooldown > 0 && Active)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/charge_0") with
                {
                    Volume = 0.5f
                }, player.Center);
            }

            if (Cooldown <= 0 && Life < MaxLife)
            {
                if (Recovery > 0)
                {
                    Recovery--;

                    BuffPlayer buffs = player.GetModPlayer<BuffPlayer>();
                    switch (buffs.BarrierBuff)
                    {
                        case 1:
                            if (player.GetModPlayer<OptionsManagingPlayer>().UpdateCount % 2 == 0)
                            {
                                Recovery--;
                            }
                            break;
                        case 2:
                            Recovery--;
                            break;
                        case 3:
                            Recovery--;
                            if (player.GetModPlayer<OptionsManagingPlayer>().UpdateCount % 2 == 0)
                            {
                                Recovery--;
                            }
                            break;
                    }
                }
                
                if (Recovery <= 0)
                {
                    int oldLife = Life;
                    
                    if (Broken)
                    {
                        Life = MaxLife;
                        Broken = false;
                        if (Active) OnRebuild(player);
                    } else
                    {
                        Life++;
                        if (Active) OnRecovery(player);
                    }

                    Recovery = MaxRecovery;

                    // This sound would be very annoying if it played all the time even without barriers, so it only
                    // plays when the barrier is currently active
                    if (Active)
                    {
                        SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_barrier_on") with
                        {
                            Volume = Broken ? 1f : 0.5f
                        }, player.Center);
                    }
                }
            }

            Active = false;
            Variation = 0;
        }
    }
}
