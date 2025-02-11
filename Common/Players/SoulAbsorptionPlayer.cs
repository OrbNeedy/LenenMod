using lenen.Common.Systems;
using lenen.Content.Buffs;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace lenen.Common.Players
{
    public class SoulAbsorptionPlayer : ModPlayer
    {
        public int soulsCollected = 0;
        public bool activateRevival = false;
        public bool revivedState = false;
        public bool seeSpirits = false;
        public float harujionPotency = 0f;
        public int queuedReduction = -1;

        public override void Initialize()
        {
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.DeadOrGhost)
            {
                return;
            }
            if (KeybindSystem.revivalStarter.JustPressed)
            {
                //Main.NewText("Trying to activate revival");
                activateRevival = true;
            }
            if (KeybindSystem.revivalStarter.JustReleased)
            {
                activateRevival = false;
            }
        }

        public override void ResetEffects()
        {
            revivedState = false;
            harujionPotency = 0f;
            seeSpirits = false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Souls"] = soulsCollected;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Souls"))
            {
                soulsCollected = tag.GetInt("Souls");
            }
        }

        public override void UpdateBadLifeRegen()
        {
            base.UpdateBadLifeRegen();
        }

        public override void PostUpdate()
        {
            // Purely recreational, never add
            /*if (Main.rand.NextBool(48))
            {
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + 
                        new Vector2(Main.rand.Next(-1000, 1000), Main.rand.Next(-850, -750)),
                        new Vector2(Main.rand.Next(2, 11), 0).RotatedByRandom(MathHelper.TwoPi),
                        ModContent.ProjectileType<SpiritFlame>(), 2, 6, Player.whoAmI, 1);
                }
            }*/

            if (queuedReduction != -1)
            {
                soulsCollected -= queuedReduction;
                queuedReduction = -1;
            }
        }

        public void AddSouls(int souls)
        {
            soulsCollected += souls;
        }

        public void QueueDeduction(int souls)
        {
            if (souls > queuedReduction)
            {
                queuedReduction = souls;
            }
        }
    }
}
