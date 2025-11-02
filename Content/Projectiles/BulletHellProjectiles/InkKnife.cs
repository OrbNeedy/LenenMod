using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class InkKnife : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int Timer { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Stage { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        // Position, Scale
        public List<(Vector2, float)> afterimages = new();
        Vector2 initialPosition = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 1f;
            Projectile.light = 0.375f;

            Projectile.DamageType = ModContent.GetInstance<UniversalHybrid>();
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 100;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            initialPosition = Projectile.Center;
        }

        public override void AI()
        {
            switch (Stage)
            {
                case 0:
                    (Vector2, float) current = (Projectile.Center, 1);
                    afterimages.Add(current);
                    if (Timer >= 10)
                    {
                        Projectile.velocity *= 0.9f;
                    }
                    if (Timer >= 15)
                    {
                        Projectile.velocity = Vector2.Zero;
                        Stage++;
                        Timer = 0;
                    }
                    break;
                case 1:
                    if (Timer > 10 && Projectile.scale < 2)
                    {
                        Projectile.scale += 0.1f;
                    }
                    if (Timer >= 20)
                    {
                        Projectile.scale = 1;
                        Stage++;
                        Timer = 0;

                        Vector2 targetPosition = Main.MouseWorld;
                        Projectile.velocity = initialPosition.DirectionTo(targetPosition) * 0.01f;
                    }
                    break;
                case 2:
                    if (Main.myPlayer != Projectile.owner) break;

                    if (Timer >= 15)
                    {
                        Projectile.velocity = initialPosition.DirectionTo(
                            initialPosition + Projectile.velocity) * 28;

                        Stage++;
                        Timer = 0;
                    }

                    Projectile.netUpdate = true;
                    break;
            }

            if (Stage >= 2 && Projectile.velocity.Length() > 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            for (int i = 0; i < afterimages.Count; i++)
            {
                afterimages[i] = (afterimages[i].Item1, afterimages[i].Item2 - 0.05f);
            }

            afterimages.RemoveAll((x) => x.Item2 <= 0);

            Timer++;
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> body = Terraria.GameContent.TextureAssets.Projectile[Type];

            for (int i = 0; i < afterimages.Count; i++)
            {
                Main.EntitySpriteDraw(new DrawData(
                    body.Value,
                    afterimages[i].Item1 - Main.screenPosition,
                    body.Value.Bounds,
                    Color.White * Projectile.Opacity,
                    0,
                    body.Value.Size() * 0.5f,
                    afterimages[i].Item2,
                    SpriteEffects.None)
                );
            }

            return base.PreDrawExtras();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> body = Terraria.GameContent.TextureAssets.Projectile[Type];
            Rectangle bounds = body.Value.Bounds;

            if (Stage >= 2)
            {
                body = ModContent.Request<Texture2D>(BasicBullet.route + "InkKnife_Knives");
                bounds = body.Value.Bounds;
                bounds.Width /= 2;
                bounds.X = BulletColor == 0 ? 0 : bounds.Width;
            }

            Main.EntitySpriteDraw(new DrawData(
                body.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                body.Value.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None)
            );

            return false;
        }
    }
}
