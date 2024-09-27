using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class DebuffPlayer : ModPlayer
    {
        public bool virusDebuff { get; set; }

        public override void ResetEffects()
        {
            virusDebuff = false;
        }

        public override void UpdateBadLifeRegen()
        {
            if (virusDebuff)
            {
                if (Player.lifeRegen > 0) Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 20;
            }
        }
    }
}
