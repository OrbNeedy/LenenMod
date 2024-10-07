using lenen.Common.Players.Barriers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class PlayerBarrier : ModPlayer
    {
        /*Predicate<Barrier> deleteConditions;

        public override void Initialize()
        {
            deleteConditions = DeleteConditions;
        }*/

        public override void Initialize()
        {
        }

        public override void ResetEffects()
        {
            foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
            {
                barrier.State = 0;
            }
        }

        public override void PostUpdateEquips()
        {
            foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
            {
                // If the barrier is inactive, skip
                if (barrier.State == 0) continue;
                /*Main.NewText("Barrier " + barrier + " stats");
                Main.NewText("State: " + barrier.State);
                Main.NewText("Life: " + barrier.Life);
                Main.NewText("Cooldown: " + barrier.Cooldown);
                Main.NewText("Recovery: " + barrier.Recovery);*/

                // If the barrier still has life, do the passive effects

                if (barrier.Life <= 0)
                {
                    if (barrier.Cooldown <= 0)
                    {
                        barrier.OnRebuild(Player);
                    } else
                    {
                        barrier.Cooldown--;
                        BuffPlayer buffs = Player.GetModPlayer<BuffPlayer>();
                        switch (buffs.barrierBuff)
                        {
                            case 1:
                                if (Player.GetModPlayer<OptionsDrawingPlayer>().UpdateCount % 2 == 0)
                                {
                                    barrier.Cooldown--;
                                }
                                break;
                            case 2:
                                barrier.Cooldown--;
                                break;
                            case 3:
                                barrier.Cooldown--;
                                if (Player.GetModPlayer<OptionsDrawingPlayer>().UpdateCount % 2 == 0)
                                {
                                    barrier.Cooldown--;
                                }
                                break;
                        }
                    }
                } else
                {
                    barrier.PassiveEffects(Player);
                    if (barrier.Life < barrier.MaxLife)
                    {
                        if (barrier.Recovery <= 0)
                        {
                            barrier.OnRecovery(Player);
                        } else
                        {
                            barrier.Recovery--;
                            BuffPlayer buffs = Player.GetModPlayer<BuffPlayer>();
                            switch (buffs.barrierBuff)
                            {
                                case 1:
                                    if (Player.GetModPlayer<OptionsDrawingPlayer>().UpdateCount % 2 == 0)
                                    {
                                        barrier.Recovery--;
                                    }
                                    break;
                                case 2:
                                    barrier.Recovery--;
                                    break;
                                case 3:
                                    barrier.Recovery--;
                                    if (Player.GetModPlayer<OptionsDrawingPlayer>().UpdateCount % 2 == 0)
                                    {
                                        barrier.Recovery--;
                                    }
                                    break;
                            }
                        }
                    }
                }

                /*
                if (barrier.IsAvailable()) barrier.PassiveEffects();

                // If the barrier was broken, subtract from the cooldown, also adjust for the barrier buff
                if (barrier.Cooldown > 0)
                {
                    barrier.Cooldown -= 1;
                    switch (Player.GetModPlayer<BuffPlayer>().barrierBuff)
                    {
                        case 1:
                            if (Player.GetModPlayer<OptionsDrawingPlayer>().UpdateCount % 2 == 0)
                            {
                                barrier.Cooldown -= 1;
                            }
                            break;
                        case 2:
                            barrier.Cooldown -= 1;
                            break;
                        case 3:
                            barrier.Cooldown -= 2;
                            break;
                    }
                    // If the new cooldown is 0, activate the barrier and it's initial effects
                    if (barrier.Cooldown <= 0)
                    {
                        barrier.OnRebuild();
                    }
                }

                // If the barrier is not broken but just damaged, subtract from the recovery cooldown
                if (barrier.Recovery > 0)
                {
                    barrier.Recovery -= 1;
                    switch (Player.GetModPlayer<BuffPlayer>().barrierBuff)
                    {
                        case 1:
                            if (Player.GetModPlayer<OptionsDrawingPlayer>().UpdateCount % 2 == 0)
                            {
                                barrier.Cooldown -= 1;
                            }
                            break;
                        case 2:
                            barrier.Cooldown -= 1;
                            break;
                        case 3:
                            barrier.Cooldown -= 2;
                            break;
                    }
                    // If the new recovery cooldown is 0, partially activate the barrier
                    if (barrier.Recovery <= 0)
                    {
                        barrier.OnRecovery();
                    }
                }

                // If the barrier is active, run it's passive effects
                if (barrier.IsAvailable())
                {
                    barrier.PassiveEffects();
                }*/
            }
            base.PostUpdateEquips();
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
            {
                if (barrier.IsAvailable())
                {
                    if (!barrier.GeneralOnHitLogic(ref modifiers, Player, proj)) return;
                    barrier.OnHitByProjectile(proj, ref modifiers, Player);
                }
            }
            base.ModifyHitByProjectile(proj, ref modifiers);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
            {
                if (barrier.IsAvailable())
                {
                    if (!barrier.GeneralOnHitLogic(ref modifiers, Player, npc:npc)) return;
                    barrier.OnHitByNPC(npc, ref modifiers, Player);
                }
            }
            base.ModifyHitByNPC(npc, ref modifiers);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
            {
                if (barrier.IsAvailable())
                {
                    if (!barrier.GeneralOnHitLogic(ref modifiers, Player)) return;
                    barrier.OnHitByMisc(ref modifiers, Player);
                }
            }
            base.ModifyHurt(ref modifiers);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
            {
                if (barrier.State != 0 && barrier.Cooldown >= (barrier.MaxCooldown - 5))
                {
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_end"),
                        Player.Center);
                    return;
                }
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.drawPlayer != Main.LocalPlayer) return;
            if (drawInfo.shadow == 0f)
            {
                foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
                {
                    if (barrier.IsAvailable())
                    {
                        barrier.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
                    }
                }
            }
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }

        public bool HasAnyBarrier()
        {
            foreach (var barrier in BarrierLookups.BarrierDictionary.Values)
            {
                if (barrier.State != 0) return true;
            }
            return false;
        }
        /*
#nullable enable
        public Barrier? GetBarrier(Type type)
        {
            foreach (Barrier barrier in barriers)
            {
                if (barrier.GetType() == type)
                {
                    return barrier;
                }
            }
            return null;
        }*/
    }
}
