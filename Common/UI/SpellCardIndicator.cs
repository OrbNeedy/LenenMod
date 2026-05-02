using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace lenen.Common.UI
{
    class SpellCardIndicator : UIState
    {
        UIElement area;
        UIShaderImage spellCardBackground;
        UIText spellCardName;
        UIShaderImage flashbombBackground;
        UIText flashbombName;

        float flashbombAlpha = 0;
        float spellcardAlpha = 0;

        public override void OnInitialize()
        {
            area = new();
            area.Width.Set(0, 0.5f);
            area.Height.Set(54, 0f);
            area.Left.Set(0, 0.5f);
            area.Top.Set(-110, 1f);

            spellCardBackground = new(ModContent.Request<Texture2D>(
                "lenen/Common/UI/SpellCardBackground"));
            spellCardBackground.Width.Set(288, 0);
            spellCardBackground.Height.Set(48, 1f);
            spellCardBackground.Left.Set(224, 0f);
            spellCardBackground.Top.Set(54, 0f);

            spellCardName = new("");
            spellCardName.HAlign = 0.5f;
            spellCardName.Top.Set(24, 0f);
            spellCardBackground.Append(spellCardName);

            flashbombBackground = new(ModContent.Request<Texture2D>(
                "lenen/Common/UI/FlashbombBackground"));
            flashbombBackground.Width.Set(320, 0);
            flashbombBackground.Height.Set(32, 1f);
            flashbombBackground.Left.Set(-160, 0f);
            flashbombBackground.Top.Set(54, 0f);

            flashbombName = new("");
            flashbombName.HAlign = 0.5f;
            flashbombName.Top.Set(8, 0f);
            flashbombBackground.Append(flashbombName);

            area.Append(spellCardBackground);
            area.Append(flashbombBackground);
            Append(area);
        }

        public void SetShaders()
        {
            spellCardBackground.SetShader(GameShaders.Misc["Gradient"]);
            flashbombBackground.SetShader(GameShaders.Misc["Gradient2"]);
        }

        public override void Update(GameTime gameTime)
        {
            //Main.NewText("Updating UI");
            ThrillPlayer flashbomb = Main.LocalPlayer.GetModPlayer<ThrillPlayer>();

            if (ThrillPlayer.uiData.ContainsKey(flashbomb.lastFlashbombUse))
            {
                //Main.NewText("Flashbomb detected, time: " + alpha);
                FlashbombInterfaceNames data = ThrillPlayer.uiData[flashbomb.lastFlashbombUse];
                flashbombAlpha = float.Clamp((75f - flashbomb.timeSinceFlashbomb) / 45f, 0, 1);
                
                if (flashbomb.timeSinceFlashbomb <= 0)
                {
                    flashbombName.SetText(data.name);
                    flashbombBackground.SetShaderParams(data.outerColor, data.midColor, data.outerColor);
                }

                Recalculate();
            } else
            {
                flashbombAlpha = 0;
            }

            SpellCardManagement spellcard = Main.LocalPlayer.GetModPlayer<SpellCardManagement>();

            if (SpellCardManagement.spellcardData.ContainsKey(spellcard.lastSpellCard))
            {
                SpellCardData data = SpellCardManagement.spellcardData[spellcard.lastSpellCard];
                spellcardAlpha = float.Clamp((150f - spellcard.timeSinceSpellcard) / 60f, 0, 1);

                if (spellcard.timeSinceSpellcard <= 1)
                {
                    spellCardName.SetText(data.name);
                    if (spellcard.lastDesperate)
                    {
                        spellCardName.SetText(data.desperateName);
                    }
                    spellCardBackground.SetShaderParams(data.leftColor, data.midColor, data.rightColor);
                }

                Recalculate();
            } else
            {
                spellcardAlpha = 0;
            }

            flashbombBackground.Color = Color.White * flashbombAlpha;
            flashbombName.TextColor = Color.White * flashbombAlpha;
            flashbombName.ShadowColor = Color.Black * flashbombAlpha;

            spellCardBackground.Color = Color.White * spellcardAlpha;
            spellCardName.TextColor = Color.White * spellcardAlpha;
            spellCardName.ShadowColor = Color.Black * spellcardAlpha;
        }
    }
}
