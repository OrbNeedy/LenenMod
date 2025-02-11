using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using lenen.Common.Graphics;
using lenen.Common.Players;
using lenen.Common.Systems;
using Terraria.ID;
using lenen.Content.NPCs.Fairy;
using Terraria.Graphics.Effects;

namespace lenen.Content.Projectiles
{
    public class FlashbombProjectile : ModProjectile
    {
        Vector2 realHitbox = Vector2.Zero;
        public float bombType { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
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
            Main.NewText($"Type: {bombType}");
            switch ((int)bombType)
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
                default:
                    Projectile.timeLeft = 30;
                    soundPath = "bom_flash_01";
                    break;
            }
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/" + soundPath), Projectile.Center);
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
                default:
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    break;

            }

            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.friendly || projectile.damage <= 0 || !projectile.hostile) continue;
                float collision = 0f; // Unused

                switch ((int)bombType)
                {
                    case (int)Flashbomb.MaidenPit:
                        // Maiden Pit is a circle
                        if (Projectile.Center.Distance(projectile.Center) <= 60 + projectile.Size.X)
                        {
                            projectile.timeLeft = 0;
                            projectile.netUpdate = true;
                        }
                        break;
                    case (int)Flashbomb.NegativeAndPositive:
                        Vector2 dimensions = new Vector2(200, 300);
                        if (Collision.CheckAABBvAABBCollision(Projectile.Center - dimensions/2, dimensions, 
                            projectile.position, projectile.Size))
                        {
                            projectile.velocity *= -1;
                            projectile.hostile = false;
                            projectile.friendly = true;
                            projectile.netUpdate = true;
                        }
                        break;
                    case (int)Flashbomb.LostTorus:
                        // Lost Torus is a ring (Unless upgraded)
                        if (Projectile.Center.Distance(projectile.Center) <= 100 + projectile.Size.X &&
                            (78 < projectile.Size.X || Projectile.Center.Distance(projectile.Center) > 
                            78 - projectile.Size.X))
                        {
                            projectile.timeLeft = 0;
                            projectile.netUpdate = true;
                        }
                        break;
                    case (int)Flashbomb.BlackRopes:
                        // Black ropes is a cross
                        Vector2 startOffset = Projectile.Center + new Vector2(-800);
                        Vector2 endOffset = Projectile.Center + new Vector2(800);

                        Vector2 startOffset2 = Projectile.Center + new Vector2(800, -800);
                        Vector2 endOffset2 = Projectile.Center + new Vector2(-800, 800);
                        if (Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size, 
                            startOffset, endOffset, 14, ref collision) || 
                            Collision.CheckAABBvLineCollision(projectile.position, projectile.Size, 
                            startOffset2, endOffset2, 14, ref collision))
                        {
                            projectile.timeLeft = 0;
                            projectile.netUpdate = true;
                        }
                        break;
                    case (int)Flashbomb.DimensionalDeletion:
                        Vector2 TopLeft = Projectile.Center + new Vector2(80 * realHitbox.X, -210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);
                        Vector2 TopRight = Projectile.Center + new Vector2(-80 * realHitbox.X, -210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);
                        Vector2 BottomLeft = Projectile.Center + new Vector2(80 * realHitbox.X, 210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);
                        Vector2 BottomRight = Projectile.Center + new Vector2(-80 * realHitbox.X, 210 * realHitbox.Y).
                            RotatedBy(Projectile.rotation);

                        Vector2 TopLeft2 = Projectile.Center + new Vector2(60 * realHitbox.X, -140 * realHitbox.Y).
                            RotatedBy(-Projectile.rotation);
                        Vector2 TopRight2 = Projectile.Center + new Vector2(-80 * realHitbox.X, -140 * realHitbox.Y).
                            RotatedBy(-Projectile.rotation);
                        Vector2 BottomLeft2 = Projectile.Center + new Vector2(80 * realHitbox.X, 140 * realHitbox.Y).
                            RotatedBy(-Projectile.rotation);
                        Vector2 BottomRight2 = Projectile.Center + new Vector2(-80 * realHitbox.X, 140 * realHitbox.Y).
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
                            projectile.timeLeft = 0;
                            projectile.netUpdate = true;
                        }
                        break;
                    default:
                        // Default is a cone-like shape at the front of the player
                        Vector2 line1Start = Projectile.Center - new Vector2(60, 40).RotatedBy(Projectile.rotation);
                        Vector2 line1End = Projectile.Center - new Vector2(28, -50).RotatedBy(Projectile.rotation);

                        Vector2 line2Start = Projectile.Center - new Vector2(32, 46).RotatedBy(Projectile.rotation);
                        Vector2 line2End = Projectile.Center - new Vector2(14, -48).RotatedBy(Projectile.rotation);

                        Vector2 line3Start = Projectile.Center - new Vector2(0, 50).RotatedBy(Projectile.rotation);
                        Vector2 line3End = Projectile.Center - new Vector2(0, -50).RotatedBy(Projectile.rotation);

                        Vector2 line4Start = Projectile.Center - new Vector2(-32, 46).RotatedBy(Projectile.rotation);
                        Vector2 line4End = Projectile.Center - new Vector2(-14, -48).RotatedBy(Projectile.rotation);

                        Vector2 line5Start = Projectile.Center - new Vector2(-60, 40).RotatedBy(Projectile.rotation);
                        Vector2 line5End = Projectile.Center - new Vector2(-28, -50).RotatedBy(Projectile.rotation);
                        if (Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size, line1Start, 
                            line1End, 1f, ref collision) || Collision.CheckAABBvLineCollision(projectile.Center, 
                            projectile.Size, line2Start, line2End, 1f, ref collision) || 
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size, line3Start,
                            line3End, 1f, ref collision) || Collision.CheckAABBvLineCollision(projectile.Center, 
                            projectile.Size, line4Start, line4End, 1f, ref collision) ||
                            Collision.CheckAABBvLineCollision(projectile.Center, projectile.Size, line5Start,
                            line5End, 1f, ref collision))
                        {
                            projectile.timeLeft = 0;
                            projectile.netUpdate = true;
                        }
                        break;
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
            Texture2D flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/DefaultFlashbomb").Value;
            MiscShaderData shader;
            Rectangle sourceRectangle;
            Vector2 origin;

            // Special cases
            switch ((int)bombType)
            {
                case (int)Flashbomb.RememberedRemnants:
                    if (PlayerRenderTarget.canUseTarget)
                    {
                        SpriteBatchState tempState = SpriteBatchExt.GetState(Main.spriteBatch);

                        SpriteBatchExt.Restart(Main.spriteBatch, tempState, SpriteSortMode.Immediate);

                        Rectangle playerRect = PlayerRenderTarget.
                            getPlayerTargetSourceRectangle(Projectile.owner);
                        sourceRectangle = new Rectangle(Projectile.owner * playerRect.Width, 0, 
                            playerRect.Width, playerRect.Height);

                        GameShaders.Misc["Silouette"].UseColor(0.075f, 0.075f, 0.075f).
                            UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(Projectile.Opacity).Apply();
                        Main.spriteBatch.Draw(PlayerRenderTarget.Target, Projectile.Center - Main.screenPosition - 
                            playerRect.Size()/2 - new Vector2(10, 21), sourceRectangle, Color.White);
                        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                        SpriteBatchExt.Restart(Main.spriteBatch, tempState);
                    }
                    return false;
                case (int)Flashbomb.Wormhole:
                    // Why did I give it such a long name?
                    flashbomb = ModContent.Request<Texture2D>("lenen/Content/Projectiles/GravityPullBulletWithAura").Value;
                    
                    sourceRectangle = new Rectangle(0, 0, flashbomb.Width, flashbomb.Height);
                    origin = sourceRectangle.Size() / 2f;
                    Main.EntitySpriteDraw(
                        flashbomb,
                        Projectile.Center - Main.screenPosition,
                        sourceRectangle,
                        Color.White * Projectile.Opacity,
                        Projectile.rotation,
                        origin,
                        new Vector2(1, 1),
                        SpriteEffects.None);
                    return false;
                case (int)Flashbomb.BlackRopes:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/BlackRopes").Value;

                    Vector2 startOffset = new Vector2(-800);
                    sourceRectangle = new Rectangle(0, 0, flashbomb.Width, flashbomb.Height);
                    origin = sourceRectangle.Size() / 2f;

                    Main.EntitySpriteDraw(
                        flashbomb,
                        Projectile.Center - Main.screenPosition,
                        sourceRectangle,
                        Color.White * Projectile.Opacity,
                        -MathHelper.PiOver4 - MathHelper.PiOver2,
                        origin,
                        new Vector2(10, 1),
                        SpriteEffects.None);

                    startOffset.X *= -1;

                    Main.EntitySpriteDraw(
                        flashbomb,
                        Projectile.Center - Main.screenPosition,
                        sourceRectangle,
                        Color.White * Projectile.Opacity,
                        -MathHelper.PiOver4,
                        origin,
                        new Vector2(10, 1),
                        SpriteEffects.None);
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

            shader = GameShaders.Misc["Silouette"].UseColor(0.075f, 0.075f, 0.075f).
                UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(Projectile.Opacity);

            switch ((int)bombType)
            {
                case (int)Flashbomb.MaidenPit:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/MaidenPit").Value;
                    break;
                case (int)Flashbomb.NegativeAndPositive:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/NegativeAndPositive").Value;
                    shader = GameShaders.Misc["RectangleWave"].UseColor(0.075f, 0.075f, 0.075f).
                        UseSecondaryColor(0.7f, 0.7f, 0.7f).UseOpacity(Projectile.Opacity);
                    break;
                case (int)Flashbomb.LostTorus:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/LostTorus").Value;
                    shader = shader.UseOpacity(Projectile.Opacity).UseColor(0.7f, 0.7f, 0.7f);
                    break;
                default:
                    flashbomb = ModContent.Request<Texture2D>("lenen/Assets/Textures/DefaultFlashbomb").Value;
                    break;
            }

            sourceRectangle = new Rectangle(0, 0, flashbomb.Width, flashbomb.Height);

            origin = sourceRectangle.Size() / 2f;

            SpriteBatchState state = SpriteBatchExt.GetState(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, state, SpriteSortMode.Immediate);

            DrawData data = new DrawData(
                flashbomb,
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
            return false;
        }
    }
}
