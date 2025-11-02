using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace lenen.Common.Config
{
    public class GameplayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Harujion")]
        [DefaultValue(true)]
        public bool HarujionAllowed;

        [Header("HarujionHardMode")]
        [DefaultValue(false)]
        public bool HarujionHardMode;
    }
}
