using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace lenen.Content.Projectiles
{
    public class KuroLaserHoldout : ModProjectile
    {

        public override string Texture => "lenen/Assets/Textures/Empty";
        
        private const float AimResponsiveness = 0.4f;

        private float FrameCounter
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.LastPrism);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            UpdatePlayerVisuals(player);

            if (Projectile.owner == Main.myPlayer)
            {
                UpdateAim(player.MountedCenter, player.HeldItem.shootSpeed);
                FrameCounter++;
                if (FrameCounter >= 109)
                {
                    FrameCounter = 0;
                }

                bool stillInUse = player.channel && !player.noItems && !player.CCed;

                if (stillInUse && FrameCounter == 1f)
                {
                    FireBeams();
                } else if (!stillInUse)
                {
                    Projectile.Kill();
                }
            }

            Projectile.timeLeft = 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void UpdatePlayerVisuals(Player player)
        {
            Projectile.Center = player.MountedCenter;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        private void UpdateAim(Vector2 source, float speed)
        {
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
            Projectile.velocity = aim;
        }

        private void FireBeams()
        {
            // If for some reason the beam velocity can't be correctly normalized, set it to a default value.
            Vector2 beamVelocity = Vector2.Normalize(Projectile.velocity);
            if (beamVelocity.HasNaNs())
            {
                beamVelocity = -Vector2.UnitY;
            }

            // This UUID will be the same between all players in multiplayer, ensuring that the beams are properly anchored on the Prism on everyone's screen.
            int uuid = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);

            int damage = Projectile.damage;
            float knockback = Projectile.knockBack;
            for (int b = 0; b < 4; ++b)
            {
                // Position doesn't matter as the beam itself should correct it's position on it's own
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, 
                    ModContent.ProjectileType<KuroLaser>(), damage, knockback, Projectile.owner, 
                    b, uuid);
            }

            Projectile.netUpdate = true;
        }

        // This is invisible because there is code elsewhere that handles the options that should appear when the player holds an item like this
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
