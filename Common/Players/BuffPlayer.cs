using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class BuffPlayer : ModPlayer
    {
        public bool virusDebuff { get; set; }
        public int barrierBuff { get; set; }

        public override void ResetEffects()
        {
            virusDebuff = false;
            barrierBuff = 0;
        }

        public override void PostUpdate()
        {
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
