using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace lenen.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind revivalStarter { get; private set; }
        //public static ModKeybind flashbomb { get; private set; }

        public override void Load()
        {
            revivalStarter = KeybindLoader.RegisterKeybind(Mod, "Revival", "Q");
            //flashbomb = KeybindLoader.RegisterKeybind(Mod, "Primary septimal ability", "F");
        }

        public override void Unload()
        {
            revivalStarter = null;
        }
    }
}
