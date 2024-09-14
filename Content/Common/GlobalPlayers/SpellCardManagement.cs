using Terraria.ModLoader;

namespace lenen.Content.Common.GlobalPlayers
{
    public class SpellCardManagement : ModPlayer
    {
        public int spellCardTimer = 0;
        public int maxSinceZero = 0;

        public override void PreUpdate()
        {
            spellCardTimer -= spellCardTimer > 0 ? 1 : 0;
            if (spellCardTimer <= 0)
            {
                maxSinceZero = 0;
            } else if (spellCardTimer > maxSinceZero)
            {
                maxSinceZero = spellCardTimer;
            }
        }
    }
}
