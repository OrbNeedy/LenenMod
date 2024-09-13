using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace lenen.Content.EmoteBubbles
{
    public class NPCEmotes : ModEmoteBubble
    {
        public override string Texture => "lenen/Content/EmoteBubbles/CurtainIcon";

        public override void SetStaticDefaults()
        {
            AddToCategory(EmoteID.Category.Dangers);
        }

        public override Rectangle? GetFrame()
        {
            return new Rectangle(EmoteBubble.frame * 22, 0, 22, 26);
        }

        public override Rectangle? GetFrameInEmoteMenu(int frame, int frameCounter)
        {
            return new Rectangle(frame * 22, 0, 22, 26);
        }
    }
}
