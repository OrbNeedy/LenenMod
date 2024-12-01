using lenen.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class Retrovirus : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(250);
            Projectile.light = 1f;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.damage = 1;
            Projectile.knockBack = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.ownerHitCheck = false;
            Projectile.alpha = 127;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.3f;
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bom_00") with 
            { 
                Volume = 0.65f, 
                PitchVariance = 0.1f 
            }, Projectile.Center);
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 210)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bom_05") with
                {
                    Volume = 0.65f,
                    PitchVariance = 0.1f
                }, Projectile.Center);
            }
            Projectile.rotation += MathHelper.TwoPi/45;
            if (Projectile.scale < 2)
            {
                Projectile.scale += 0.007083f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Kurovirus>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Kurovirus>(), 300);
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> virusTexture = ModContent.Request<Texture2D>("lenen/Content/Projectiles/Retrovirus");

            Main.EntitySpriteDraw(virusTexture.Value,
                Projectile.Center - Main.screenPosition,
                virusTexture.Value.Bounds,
                Color.White*0.5f,
                -Projectile.rotation,
                virusTexture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Color[] colors = { Color.Red, Color.Green, Color.Blue };
            foreach (Color color in colors)
            {
                Vector2 direction = new Vector2(1, 0).RotatedByRandom(MathHelper.TwoPi);
                Rectangle bounds = new Rectangle((int)(Projectile.Center.X - Main.screenPosition.X),
                    (int)(Projectile.Center.Y - Main.screenPosition.Y), (int)(30 * Projectile.scale), 4);
                Vector2 offset = Vector2.Zero;

                for (int i = 0; i < 3; i++)
                {
                    Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value,
                        Projectile.Center - Main.screenPosition + offset,
                        bounds,
                        color,
                        direction.ToRotation(),
                        new Vector2(0, 0),
                        1f,
                        SpriteEffects.None
                    );
                    offset += direction * bounds.Width;
                    direction = direction.RotatedByRandom(MathHelper.Pi/1.5);
                }
            }
        }

        public override void CutTiles()
        {
            Vector2 starting = (Projectile.Center + new Vector2(0, 125 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 ending = (Projectile.Center + new Vector2(0, -125 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);
            float width = 10f * Projectile.scale;

            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);

            Utils.PlotTileLine(starting, ending, width * Projectile.scale, cut);
            Utils.PlotTileLine(starting.RotatedBy(MathHelper.PiOver2), 
                ending.RotatedBy(MathHelper.PiOver2), width * Projectile.scale, cut);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 ellipsePosition = new Vector2(projHitbox.Left, projHitbox.Top);
            Vector2 ellipseDimentions = new Vector2(projHitbox.Width*1.2f, projHitbox.Height*1.2f);
            Vector2 ellipseCenter = ellipsePosition + 0.5f * ellipseDimentions;
            ellipseDimentions *= Projectile.scale;
            float x = 0f;
            float y = 0f;
            if (targetHitbox.Left > ellipseCenter.X)
            {
                x = targetHitbox.Left - ellipseCenter.X;
            }
            else if (targetHitbox.Left + targetHitbox.Width < ellipseCenter.X)
            {
                x = targetHitbox.Left + targetHitbox.Width - ellipseCenter.X;
            }
            if (targetHitbox.Top > ellipseCenter.Y)
            {
                y = targetHitbox.Top - ellipseCenter.Y;
            }
            else if (targetHitbox.Top + targetHitbox.Height < ellipseCenter.Y)
            {
                y = targetHitbox.Top + targetHitbox.Height - ellipseCenter.Y;
            }
            float a = ellipseDimentions.X / 2f;
            float b = ellipseDimentions.Y / 2f;
            return (x * x) / (a * a) + (y * y) / (b * b) <= 1;
        }
    }
}
