using lenen.Common.Systems;
using lenen.Content.Buffs;
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

        public override void PostUpdateEquips()
        {
            //Main.NewText($"Player location: {Player.Center}");
            //Main.NewText("Current souls: " + soulsCollected);
            base.PostUpdateEquips();
        }

        public void AddSouls(int souls)
        {
            soulsCollected += souls;
            //Main.NewText("Added " + souls + " to the player");
            //Main.NewText("Current souls: " + soulsCollected);
            // Reserved for Harujion
            /*
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                Item item = Player.inventory[i];
                if (item.ModItem != null)
                {
                }
            }*/
        }
    }
}
