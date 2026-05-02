using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using lenen.Common.Graphics;
using lenen.Common.Players;
using Terraria.ID;
using lenen.Content.NPCs.Fairy;
using Terraria.Graphics.Effects;
using lenen.Common.Utils;
using ReLogic.Content;

namespace lenen.Content.Projectiles
{
    public class FlashbombProjectile : ModProjectile
    {
        Vector2 realHitbox = Vector2.Zero;
        public int bombType { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public bool upgraded { get => Projectile.ai[1] != 0; set => Projectile.ai[1] = value ? 1 : 0; }

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.scale = 1f;

            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 800;
            Projectile.DamageType = DamageClass.Default;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
        }

        public override string Texture => "lenen/Assets/Textures/Empty";

        public override void OnSpawn(IEntitySource source)
        {
            string soundPath = "bom_flash_00"; 
            //Main.NewText($"Type: {bombType}");
            switch (bombType)
            {
                case (int)Flashbomb.MaidenPit:
                    Projectile.timeLeft = 45;
                    soundPath = "bom_flash_01";
                    break;
                case (int)Flashbomb.RememberedRemnants:
                    // Special case 1
                    Projectile.timeLeft = 300;
                    return;
                case (int)Flashbomb.NegativeAndPositive:
                    Projectile.timeLeft = 20;
                    soundPath = "bom_flash_01";
                    break;
                case (int)Flashbomb.Wormhole:
                    // Special case 2
                    Projectile.Opacity = 0.5f;
                    Projectile.timeLeft = 10800;
                    return;
                case (int)Flashbomb.LostTorus:
                    Projectile.timeLeft = 60;
                    soundPath = "bom_flash_01";
                    break;
                case (int)Flashbomb.BlackRopes:
                    Projectile.timeLeft = 30;
                    break;
                case (int)Flashbomb.DimensionalDeletion:
                    Projectile.timeLeft = 300;
                    realHitbox = new Vector2(1, 1);
                    break;
                case (int)Flashbomb.VertexEmit:
                    Projectile.timeLeft = 30;
                    soundPath = "bom_flash_01";
                    break;
                case (int)Flashbomb.MonochromeFlash:
                    Player owner = Main.player[Projectile.owner];
                    Projectile.damage = (int)owner.GetTotalDamage(DamageClass.Magic).ApplyTo(80);
                    Projectile.ArmorPenetration = 25;
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.localNPCHitCooldown = 0;
                    break;
                default:
                    Projectile.timeLeft = 30;
                    soundPath = "bom_flash_01";
                    break;
            }
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/" + soundPath) with { 
                Volume = 0.5f
            }, Projectile.Center);
        }

        public override void AI()
        {
            if (Main.dedServ) return;

            if (Projectile.timeLeft <= 10)
            {
                Projectile.Opacity -= 0.1f;
            }

            // General AI, flashbombs that don't delete projectiles end here
            switch ((int)bombType)
            {
                case (int)Flashbomb.MaidenPit:
                    break;
                case (int)Flashbomb.RememberedRemnants:
                    foreach (var npc in Main.ActiveNPCs)
                    {
                        if (npc.ModNPC is SmallFairy fairy && npc.target == Projectile.owner)
                        {
                            fairy.distracted = true;
                            fairy.distractionPosition = Projectile.Center;
                        }
                    }
                    return;
                case (int)Flashbomb.NegativeAndPositive:
                    break;
                case (int)Flashbomb.Wormhole:
                    Projectile.rotation -= 0.6f;
                    return;
                case (int)Flashbomb.LostTorus:
                    break;
                case (int)Flashbomb.BlackRopes:
                    break;
                case (int)Flashbomb.DimensionalDeletion:
                    Projectile.rotation += 0.025f;
                    if (Projectile.timeLeft <= 240)
                    {
                        if (realHitbox.X < 2)
                        {
                            realHitbox.X += 1f/60f;
                        }
                        if (realHitbox.Y < 2)
                        {
                            realHitbox.Y += 1f / 60f;
                        }
                    }
                    break;
                case (int)Flashbomb.MonochromeFlash:
                default:
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    break;

            }

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.friendly || projectile.damage <= 0 || !projectile.hostile) continue;
                float collision = 0f; // Unused
                bool delete = false;

                switch ((int)bombType)
                {
                    case (int)Flashbomb.MaidenPit:
                        // Maiden Pit is a circle
                        if (FlashbombStats.Circle(Projectile, projectile))
                        {
                            delete = true;
                        }
                        break;
                    case (int)Flashbomb.NegativeAndPositive:
                        Vector2 dimensions = new Vector2(200, 300);
                        if (FlashbombStats.Square(Projectile, projectile, dimensions))
                        {
                            projectile.velocity *= -1;
                            projectile.hostile = false;
                            projectile.friendly = true;
                            projectile.netUpdate = true;
                        }
                        break;
                    case (int)Flashbomb.LostTorus:
                        // Lost Torus is a ring (Unless upgraded)
                        bool externalBorder = FlashbombStats.Circle(Projectile, projectile, 100);
                        bool tooBig = projectile.Size.X >= 78;
                        bool internalBorder = !FlashbombStats.Circle(Projectile, projectile, 78);
                        if (externalBorder && (tooBig || internalBorder))
                        {
                            delete = true;
                        }
                        break;
                    case (int)Flashbomb.BlackRopes:
                        // Black ropes is a cross
                        Vector2 startOffset = new Vector2(-800);
                        Vector2 endOffset = new Vector2(800);

                        Vector2 startOffset2 = new Vector2(800, -800);
                        Vector2 endOffset2 = new Vector2(-800, 800);
                        bool line1 = FlashbombStats.Line(Projectile, projectile, startOffset, endOffset, 
                            14, out _);
                        bool line2 = FlashbombStats.Line(Projectile, projectile, startOffset2, endOffset2,
                            14, out _);

                        if (line1 || line2)
                        {
                            delete = true;
                        }
                        break;
                    case (int)Flashbomb.VertexEmit:
                        bool vertex1 = FlashbombStats.Line(Projectile, projectile, new(0, 2400), 
                            new(0, -2400), 10, out _);
                        bool vertex2 = FlashbombStats.Line(Projectile, projectile, new(2400, 0),
                            new(-2400, 0), 10, out _);

                        if (vertex1 || vertex2)
                        {
                            delete = true;
                        }
                        break;
                    case (int)Flashbomb.MonochromeFlash:
                        float progress = Projectile.timeLeft / 40f;
                        Vector2 laserStart = new Vector2(0, 0);
                        Vector2 laserEnd = new Vector2(0, -124 - float.Clamp(2880 * (1 - progress), 0, 1600)).
                            RotatedBy(Projectile.rotation);

                        bool deletion = FlashbombStats.Line(Projectile, projectile, laserStart, laserEnd,
                            (188 * progress), out _);

                        if (deletion)
                        {
                            delete = true;
                        }
                        break;
                    case (int)Flashbomb.DimensionalDeletion:
                        Vector2 TopLeft = new Vector2(80 * realHitbox.X, -210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);
                        Vector2 TopRight = new Vector2(-80 * realHitbox.X, -210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);
                        Vector2 BottomLeft = new Vector2(80 * realHitbox.X, 210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);
                        Vector2 BottomRight = new Vector2(-80 * realHitbox.X, 210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);

                        Vector2 TopLeft2 = new Vector2(60 * realHitbox.X, -140 * realHitbox.Y).
                            RotatedBy(-Projectile.rotation);
                        Vector2 TopRight2 = new Vector2(-80 * realHitbox.X, -140 * realHitbox.Y).
                            RotatedBy(-Projectile.rotation);
                        Vector2 BottomLeft2 = new Vector2(80 * realHitbox.X, 140 * realHitbox.Y).
                            RotatedBy(-Projectile.rotation);
                        Vector2 BottomRight2 = new Vector2(-80 * realHitbox.X, 140 * realHitbox.Y).
                            RotatedBy(-Projectile.rotation);

                        bool hitbox1Collision = Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopLeft, TopRight, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopRight, BottomRight, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            BottomRight, BottomLeft, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            BottomLeft, TopLeft, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopRight, BottomLeft, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopLeft, BottomRight, 10, ref collision);

                        bool hitbox2Collision = Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopLeft2, TopRight2, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopRight2, BottomRight2, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            BottomRight2, BottomLeft2, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            BottomLeft2, TopLeft2, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopRight2, BottomLeft2, 10, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size,
                            TopLeft2, BottomRight2, 10, ref collision);

                        if (hitbox1Collision || hitbox2Collision)
                        {
                            delete = true;
                        }
                        break;
                    default:
                        // Default is a cone-like shape at the front of the player
                        bool coneCollision = false;
                        Vector2 line1Start = -new Vector2(60, 40).RotatedBy(Projectile.rotation);
                        Vector2 line1End = -new Vector2(28, -50).RotatedBy(Projectile.rotation);
                        coneCollision |= FlashbombStats.Line(Projectile, projectile, line1Start, 
                            line1End, 1, out _);

                        Vector2 line2Start = -new Vector2(32, 46).RotatedBy(Projectile.rotation);
                        Vector2 line2End = -new Vector2(14, -48).RotatedBy(Projectile.rotation);
                        coneCollision |= FlashbombStats.Line(Projectile, projectile, line2Start,
                            line2End, 1, out _);

                        Vector2 line3Start = -new Vector2(0, 50).RotatedBy(Projectile.rotation);
                        Vector2 line3End = -new Vector2(0, -50).RotatedBy(Projectile.rotation);
                        coneCollision |= FlashbombStats.Line(Projectile, projectile, line3Start,
                            line3End, 1, out _);

                        Vector2 line4Start = -new Vector2(-32, 46).RotatedBy(Projectile.rotation);
                        Vector2 line4End = -new Vector2(-14, -48).RotatedBy(Projectile.rotation);
                        coneCollision |= FlashbombStats.Line(Projectile, projectile, line4Start,
                            line4End, 1, out _);

                        Vector2 line5Start = -new Vector2(-60, 40).RotatedBy(Projectile.rotation);
                        Vector2 line5End = -new Vector2(-28, -50).RotatedBy(Projectile.rotation);
                        coneCollision |= FlashbombStats.Line(Projectile, projectile, line5Start,
                            line5End, 1, out _);

                        if (coneCollision)
                        {
                            delete = true;
                        }
                        break;
                }

                if (delete)
                {
                    projectile.timeLeft = 0;
                    projectile.netUpdate = true;
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            switch (bombType)
            {
                case (int)Flashbomb.DimensionalDeletion:
                    return true;
                default:
                    return false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.dedServ) return false;
            Asset<Texture2D> flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/DefaultFlashbomb");

            // Special cases
            switch ((int)bombType)
            {
                case (int)Flashbomb.RememberedRemnants:
                    FlashbombStats.RememberedRemnantsDraw(Projectile);
                    return false;
                case (int)Flashbomb.Wormhole:
                    FlashbombStats.WormholeDraw(Projectile);
                    return false;
                case (int)Flashbomb.BlackRopes:
                    FlashbombStats.BlackRopesDraw(Projectile);
                    return false;
                case (int)Flashbomb.VertexEmit:
                    FlashbombStats.VertexEmitDraw(Projectile);
                    return false;
                case (int)Flashbomb.MonochromeFlash:
                    float progress = Projectile.timeLeft / 40f;
                    FlashbombStats.MonochromeFlashDraw(Projectile, progress);
                    return false;
                case (int)Flashbomb.DimensionalDeletion:
                    if (Main.netMode != NetmodeID.Server)
                    {

                        /*if (!Filters.Scene["DimensionalDeletion"].IsInUse())
                        {
                            Filters.Scene.Activate("DimensionalDeletion", Projectile.Center).GetShader().
                                UseTargetPosition(Projectile.Center);
                            Filters.Scene["DimensionalDeletion"].GetShader().Shader.Parameters["uRotation"].
                                SetValue(Projectile.rotation);
                            Filters.Scene["DimensionalDeletion"].GetShader().Shader.Parameters["uRectangleSize"].
                                SetValue(realHitbox * 0.01f);
                        } else
                        {
                            float progress = (300f - Projectile.timeLeft) / 60f;
                            Filters.Scene["DimensionalDeletion"].GetShader().UseProgress(progress);
                            Filters.Scene["DimensionalDeletion"].GetShader().Shader.Parameters["uRotation"].
                                SetValue(Projectile.rotation);
                            Filters.Scene["DimensionalDeletion"].GetShader().Shader.Parameters["uRectangleSize"].
                                SetValue(realHitbox * 0.01f);
                        } 
                        if (!Filters.Scene["DimensionalDeletion2"].IsInUse())
                        {
                            Filters.Scene.Activate("DimensionalDeletion2", Projectile.Center).GetShader().
                                UseTargetPosition(Projectile.Center);
                            Filters.Scene["DimensionalDeletion2"].GetShader().Shader.Parameters["uRotation"].
                                SetValue(-Projectile.rotation);
                            Filters.Scene["DimensionalDeletion2"].GetShader().Shader.Parameters["uRectangleSize"].
                                SetValue(realHitbox * 0.01f);
                        } else
                        {
                            float progress = (300f - Projectile.timeLeft) / 60f;
                            Filters.Scene["DimensionalDeletion2"].GetShader().UseProgress(progress);
                            Filters.Scene["DimensionalDeletion2"].GetShader().Shader.Parameters["uRotation"].
                                SetValue(-Projectile.rotation);
                            Filters.Scene["DimensionalDeletion2"].GetShader().Shader.Parameters["uRectangleSize"].
                                SetValue(realHitbox * 0.01f);
                        }*/
                    }
                    return false;
            }

            MiscShaderData shader = GameShaders.Misc["Silouette"].UseColor(0.075f, 0.075f, 0.075f).
                UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(Projectile.Opacity);

            switch ((int)bombType)
            {
                case (int)Flashbomb.MaidenPit:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/MaidenPit");
                    break;
                case (int)Flashbomb.NegativeAndPositive:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/NegativeAndPositive");
                    shader = GameShaders.Misc["RectangleWave"].UseColor(0.075f, 0.075f, 0.075f).
                        UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(Projectile.Opacity);
                    break;
                case (int)Flashbomb.LostTorus:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/LostTorus");
                    shader = shader.UseOpacity(Projectile.Opacity).UseColor(0.7f, 0.7f, 0.7f);
                    break;
                default:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/DefaultFlashbomb");
                    break;
            }

            Rectangle sourceRectangle = flashbomb.Frame();

            Vector2 origin = sourceRectangle.Size() / 2f;

            SpriteBatchState state = SpriteBatchExt.GetState(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, state, SpriteSortMode.Immediate);

            DrawData data = new DrawData(
                flashbomb.Value,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle,
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                origin,
                1f,
                SpriteEffects.None);

            shader.Apply(data);

            data.Draw(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, state);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            switch(bombType)
            {
                case (int)Flashbomb.DimensionalDeletion:
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (Filters.Scene["DimensionalDeletion"].IsActive())
                        {
                            Filters.Scene["DimensionalDeletion"].Deactivate();
                        }
                        if (Filters.Scene["DimensionalDeletion2"].IsActive())
                        {
                            Filters.Scene["DimensionalDeletion2"].Deactivate();
                        }
                    }
                    break;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (bombType == (int)Flashbomb.MonochromeFlash)
            {
                float progress = Projectile.timeLeft / 40f;
                Vector2 laserStart = new Vector2(0, 0);
                Vector2 laserEnd = new Vector2(0, -124 - (1920 * (1 - progress))).
                    RotatedBy(Projectile.rotation);

                float unused = 0;

                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), 
                    Projectile.Center + laserStart, Projectile.Center + laserEnd, 188 * progress, 
                    ref unused);
            }
            return false;
        }
    }
}
