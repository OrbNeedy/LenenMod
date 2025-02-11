using Microsoft.Xna.Framework;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System;
using lenen.Common.Players;

namespace lenen.Content.Projectiles
{
    public class LumenLaser : ModProjectile
    {
        private const float MaxBeamLength = 2000f;
        private const int NumSamplePoints = 3;
        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 8f;
        private const float BeamLengthChangeFactor = 0.75f;
        private const float BeamLightBrightness = 1f;
        private float HostPrismIndex
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private float BeamLength
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.alpha = 255;

            Projectile.damage = 30;
            Projectile.knockBack = 3;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 70;
            Projectile.light = 1f;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 12;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BeamLength);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BeamLength = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile hostPrism = Main.projectile[(int)HostPrismIndex];
            if (Projectile.type != ModContent.ProjectileType<LumenLaser>() || !hostPrism.active ||
                hostPrism.type != ModContent.ProjectileType<LumenBall>())
            {
                Main.NewText("Beam dying because of incorrect host or type");
                Projectile.Kill();
                return;
            }

            Vector2 hostPrismDir = Vector2.Normalize(hostPrism.velocity);

            Projectile.Opacity = 1f;

            Projectile.Center = hostPrism.Center + (hostPrismDir * 20);

            Projectile.velocity = hostPrismDir;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Update the beam's length by performing a hitscan collision check.
            float hitscanBeamLength = PerformBeamHitscan(hostPrism);
            BeamLength = MathHelper.Lerp(BeamLength, hitscanBeamLength, BeamLengthChangeFactor);

            // This Vector2 stores the beam's hitbox statistics. X = beam length. Y = beam width.
            Vector2 beamDims = new Vector2(Projectile.velocity.Length() * BeamLength, Projectile.width * Projectile.scale);

            // Only produce dust and cause water ripples if the beam is above a certain charge level.
            // Always produce dust and cause water ripples because the beam has no charge mechanic
            Color beamColor = Color.White;

            ProduceBeamDust(beamColor);

            // If the game is rendering (i.e. isn't a dedicated server), make the beam disturb water.
            if (Main.netMode != NetmodeID.Server)
            {
                ProduceWaterRipples(beamDims);
            }

            // Make the beam cast light along its length. The brightness of the light scales with the charge.
            // The brightness is always the same because it has no charge mechanic
            // v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight.
            DelegateMethods.v3_1 = beamColor.ToVector3() * BeamLightBrightness;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength,
                beamDims.Y, new Utils.TileActionAttempt(DelegateMethods.CastLight));
        }

        private float PerformBeamHitscan(Projectile prism)
        {
            Vector2 samplingPoint = Projectile.Center;

            float[] laserScanResults = new float[NumSamplePoints];
            Collision.LaserScan(samplingPoint, Projectile.velocity, 0 * Projectile.scale, MaxBeamLength,
                laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;

            return averageLengthSample;
        }

        // Determines whether the specified target hitbox is intersecting with the beam.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // If the beam doesn't have a defined direction, don't draw anything.
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            // Why is it 10.5? Questions for later
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * 10.5f;
            Vector2 drawScale = new Vector2(Projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            float visualBeamLength = BeamLength - 14.5f * Projectile.scale * Projectile.scale;

            DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            Vector2 startPosition = centerFloored - Main.screenPosition;
            Vector2 endPosition = startPosition + Projectile.velocity * visualBeamLength;

            // Draw the outer beam.
            DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale,
                Color.White * Projectile.Opacity);

            // Returning false prevents Terraria from trying to draw the Projectile itself.
            return false;
        }

        private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor)
        {
            Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(LumenLaserDraw);

            DelegateMethods.c_1 = beamColor;
            Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
        }

        private static void LumenLaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
        {
            color = Color.White;
            switch (stage)
            {
                case 0:
                    distCovered = 33f;
                    frame = new Rectangle(0, 0, 16, 22);
                    origin = frame.Size() / 2f;
                    break;
                case 1:
                    frame = new Rectangle(0, 25, 16, 28);
                    distCovered = frame.Height;
                    origin = new Vector2(frame.Width / 2, 0f);
                    break;
                case 2:
                    distCovered = 22f;
                    frame = new Rectangle(0, 56, 16, 22);
                    origin = new Vector2(frame.Width / 2, 1f);
                    break;
                default:
                    distCovered = 9999f;
                    frame = Rectangle.Empty;
                    origin = Vector2.Zero;
                    color = Color.Transparent;
                    break;
            }
        }

        // No changes needed for visual effects like these
        private void ProduceBeamDust(Color beamColor)
        {
            const int type = 15;
            Vector2 endPosition = Projectile.Center + Projectile.velocity * (BeamLength - 14.5f * Projectile.scale);

            float angle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
            float startDistance = Main.rand.NextFloat(1f, 1.8f);
            float scale = Main.rand.NextFloat(0.7f, 1.1f);
            Vector2 velocity = angle.ToRotationVector2() * startDistance;
            Dust dust = Dust.NewDustDirect(endPosition, 0, 0, type, velocity.X, velocity.Y, 0, beamColor, scale);
            dust.color = beamColor;
            dust.noGravity = true;

            dust.velocity *= Projectile.scale;
            dust.scale *= Projectile.scale;
        }

        private void ProduceWaterRipples(Vector2 beamDims)
        {
            WaterShaderData shaderData = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

            float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
            Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);

            Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
            shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);
            Vector2 beamStartPos = Projectile.Center;
            Vector2 beamEndPos = beamStartPos + Projectile.velocity * BeamLength;

            Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
        }
    }
}
