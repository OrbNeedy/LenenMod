using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.GameInput;
using lenen.Common.Systems;
using System;

namespace lenen.Common.Players
{
    public class GravityPlayer : ModPlayer
    {
        public bool antiGravity = false;
        private bool manualGravity = false;
        public bool levitation = true;
        public bool gravityResist = false;

        public override void ResetEffects()
        {
            antiGravity = false;
            levitation = false;
            gravityResist = false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.gravityAdjust.JustPressed)
            {
                manualGravity = !manualGravity;
            }
        }

        public override void PreUpdate()
        {
            if (Player.CCed) return;
            if (levitation)
            {
                Player.AddBuff(BuffID.Featherfall, 2);
                Player.wingTime = Player.wingTimeMax-1;
                Player.noFallDmg = true;
            }
            if (gravityResist)
            {
                Player.buffImmune[BuffID.VortexDebuff] = true;
            }
            base.PreUpdate();
        }

        public override void PreUpdateMovement()
        {
            if (Player.CCed) return;
            if (antiGravity && !Player.shimmering && manualGravity)
            {
                Player.fallStart = (int)Player.Center.Y;
                int width = (int)(Player.Size.X + 10);
                int height = (int)(Player.Size.Y + 26);
                Vector2 offset = new Vector2((Player.Size.X / -2) - 5,
                    ((Player.Size.Y / 2) - 16));
                if (Player.gravDir == -1) offset.Y = (Player.Size.Y * -2f) + 16;
                //Main.NewText("Offset: " + offset);
                if (Collision.SolidCollision(Player.Center + offset, width, height))
                {
                    if (Math.Abs(Player.velocity.Y * Player.gravDir) < Math.Abs(20 * Player.gravDir))
                    {
                        Player.velocity += new Vector2(0, Player.gravity * -2f);
                    }
                }
            }
            base.PreUpdateMovement();
        }
    }
}
