using lenen.Common.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace lenen.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class UISystem : ModSystem
    {
        private UserInterface SpellCardBarUserInterface;

        private UserInterface BarrierBarUserInterface;

        internal SpellCardBar SpellCardBar;

        internal BarrierBar BarrierBar;

        public override void Load()
        {
            SpellCardBar = new();
            SpellCardBarUserInterface = new();
            SpellCardBarUserInterface.SetState(SpellCardBar);

            BarrierBar = new();
            BarrierBarUserInterface = new();
            BarrierBarUserInterface.SetState(BarrierBar);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            SpellCardBarUserInterface?.Update(gameTime);
            BarrierBarUserInterface?.Update(gameTime);
        }

        public override void OnWorldLoad()
        {
            //BarrierBar.InitializeBarrierAssets();
            base.OnWorldLoad();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Len'en Mod: Spell Card Bar",
                    delegate {
                        SpellCardBarUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Len'en Mod: Barrier Bar",
                    delegate {
                        BarrierBarUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
