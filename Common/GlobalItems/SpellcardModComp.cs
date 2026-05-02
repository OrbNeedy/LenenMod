using lenen.Common.Players;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace lenen.Common.GlobalItems
{
    public class SpellcardModComp : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                if (ModContent.TryFind("Gensokyo", "NanotechInk", out ModItem nanotechInk))
                {
                    if (item.type == nanotechInk.Type && player.altFunctionUse == 2)
                    {
                        SpellCardManagement scManager = player.GetModPlayer<SpellCardManagement>();
                        scManager.lastSpellCard = SpellCard.MonochromeRay;
                        scManager.timeSinceSpellcard = 0;
                        
                        SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bom_00") with
                        {
                            Volume = 0.65f,
                            PitchVariance = 0.1f
                        }, player.Center);
                    }
                }
            }
            return base.UseItem(item, player);
        }
    }
}
