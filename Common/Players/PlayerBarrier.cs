using lenen.Common.Players.Barriers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public enum BarrierTypes
    {
        SkullBarrier, 
        SkullBarrier2,
        DesperateBarrier, 
        ResurrectionBarrier, 
        GravityBarrier, 
        Beginner
    }

    public class PlayerBarrier : ModPlayer
    {
        public Dictionary<BarrierTypes, Barrier> barriers = new() { [BarrierTypes.SkullBarrier] = new SkullBarrier(),
            [BarrierTypes.SkullBarrier2] = new BetterSkullBarrier(),
            [BarrierTypes.DesperateBarrier] = new DesperateBarrier(),
            [BarrierTypes.ResurrectionBarrier] = new ResurrectionBarrier(),
            [BarrierTypes.GravityBarrier] = new GravityBarrier(),
            [BarrierTypes.Beginner] = new BeginerBarrier()
        };
        public static Dictionary<string, Asset<Texture2D>> barrierIcons = new();

        public static Barrier[] BarrierTemplates { get; set; } = { new SkullBarrier(), new BetterSkullBarrier(),
            new DesperateBarrier(), new ResurrectionBarrier(), new GravityBarrier(), new BeginerBarrier() };

        public override void Load()
        {
            barrierIcons.Add("Default", ModContent.Request<Texture2D>($"lenen/Assets/Icons/BarrierIcon"));
            foreach (Barrier barrier in barriers.Values)
            {
                List<string> keys = barrier.InitializeTextures();
                foreach (string name in keys)
                {
                    if (barrierIcons.ContainsKey(name)) continue;

                    barrierIcons.Add(name, ModContent.Request<Texture2D>($"lenen/Assets/Icons/{name}"));
                }
            }
        }

        public override void Initialize()
        {
            foreach (Barrier barrier in barriers.Values)
            {
                barrier.Initialize(Player);
                /*List<string> keys = barrier.InitializeTextures();
                foreach (string name in keys)
                {
                    barrierIcons.Add(name, ModContent.Request<Texture2D>($"lenen/Assets/Icons/{name}"));
                }*/
            }
        }

        public override void ResetEffects()
        {
            foreach (Barrier barrier in barriers.Values)
            {
                barrier.ResetEffects(Player);
            }
        }

        public override void PostUpdateEquips()
        {
            foreach (Barrier barrier in barriers.Values)
            {
                if (barrier.Active)
                {
                    if (barrier.Life > 0) barrier.PassiveEffects(Player);

                    barrier.AlwaysOnEffects(Player);
                }
            }
            base.PostUpdateEquips();
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            foreach (Barrier barrier in barriers.Values)
            {
                if (!barrier.Active || barrier.Life <= 0) continue;

                Entity damageCausingEntity = null;

                bool success = modifiers.DamageSource.TryGetCausingEntity(out damageCausingEntity);

                // Note: Spikes are 3, anything else is -1
                // Main.NewText("Sources: " + modifiers.DamageSource.SourceOtherIndex);

                if (!success || damageCausingEntity == null)
                {
                    barrier.PreHitByMisc(Player, ref modifiers);
                } else
                {
                    if (damageCausingEntity is Projectile proj)
                    {
                        barrier.PreHitByProjectile(Player, proj, ref modifiers);
                    }
                    if (damageCausingEntity is NPC npc)
                    {
                        barrier.PreHitByNPC(Player, npc, ref modifiers);
                    }
                }

                barrier.PreHit(Player, ref modifiers);
            }
            base.ModifyHurt(ref modifiers);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            foreach (Barrier barrier in barriers.Values)
            {
                if (!barrier.Active || barrier.Life <= 0) continue;

                Entity damageCausingEntity = null;

                bool success = info.DamageSource.TryGetCausingEntity(out damageCausingEntity);

                //Main.NewText("Sources: " + info.DamageSource.SourceOtherIndex);

                if (!success || damageCausingEntity == null)
                {
                    barrier.PostHitByMisc(Player, info);
                }
                else
                {
                    if (damageCausingEntity is Projectile proj)
                    {
                        barrier.PostHitByProjectile(Player, proj, info);
                    }
                    if (damageCausingEntity is NPC npc)
                    {
                        barrier.PostHitByNPC(Player, npc, info);
                    }
                }

                barrier.PostHit(Player, info);

                if (!info.Cancelled)
                {
                    barrier.ReduceHealth(Player, info);
                }
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            foreach (Barrier barrier in barriers.Values)
            {
                if (barrier.Active)
                {
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/j_end"),
                        Player.Center);
                    return;
                }
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }

        public bool HasAnyBarrier()
        {
            foreach (Barrier barrier in barriers.Values)
            {
                if (barrier.Active) return true;
            }
            return false;
        }
    }
}
