using Terraria.ModLoader;

namespace lenen.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind revivalStarter { get; private set; }
        public static ModKeybind hideBars { get; private set; }
        public static ModKeybind gravityAdjust { get; private set; }
        public static ModKeybind flashbomb { get; private set; }

        public override void Load()
        {
            revivalStarter = KeybindLoader.RegisterKeybind(Mod, "Revival", "Q");
            hideBars = KeybindLoader.RegisterKeybind(Mod, "HideJar", "P");
            gravityAdjust = KeybindLoader.RegisterKeybind(Mod, "Gravity", "X");
            flashbomb = KeybindLoader.RegisterKeybind(Mod, "Flashbomb", "F");
        }

        public override void Unload()
        {
            revivalStarter = null;
            hideBars = null;
            gravityAdjust = null;
            flashbomb = null;
        }
    }
}
