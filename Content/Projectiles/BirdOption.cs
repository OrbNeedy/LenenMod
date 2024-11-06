using lenen.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class BirdOption : ModProjectile
    {
        private float DroneIndex
        {
            get => Projectile.ai[0]; 
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 20;
            Projectile.scale = 1f;
            Projectile.light = 0.25f;
            Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            if (++Projectile.frameCounter >= 4)
            {
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }

            //Main.NewText("Drone " + DroneIndex + " statistics");

            //Main.NewText("Drone position before UpdatePlayerVisuals " + Projectile.Center);

            UpdatePlayerVisuals(player);

            //Main.NewText("Drone position after UpdatePlayerVisuals " + Projectile.Center);

            UpdateAim(player.Center, player.HeldItem.shootSpeed);

            //Main.NewText("Drone position after UpdateAim " + Projectile.Center);
        }

        private void UpdatePlayerVisuals(Player player)
        {
            float additionalOffsetRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
            Vector2 beamIdOffset = new Vector2(0, -50).RotatedBy((DroneIndex * MathHelper.PiOver4) +
                additionalOffsetRotation + MathHelper.PiOver4 / 2);
            Vector2 optionOffset = new Vector2(0, -10);//player.Center.DirectionTo(Main.MouseWorld)*10;

            Projectile.Center = player.Center + beamIdOffset + optionOffset;
        }

        private void UpdateAim(Vector2 source, float speed)
        {
            float offsetRotation = DroneIndex <= 1 ? -MathHelper.PiOver4 : MathHelper.PiOver4;
            int updateCount = Main.player[Projectile.owner].GetModPlayer<OptionsManagingPlayer>().UpdateCount;
            Vector2 beamRotation = new Vector2(1, 0).RotatedBy(
                Math.Sin((updateCount + (230 * DroneIndex)) * 0.008) + offsetRotation) * 6f;

            Projectile.rotation = beamRotation.ToRotation() + (source - Main.MouseWorld).ToRotation() - 
                MathHelper.PiOver2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
