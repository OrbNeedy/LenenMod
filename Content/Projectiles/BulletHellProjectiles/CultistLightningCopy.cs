using lenen.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using ReLogic.Content;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class CultistLightningCopy : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public bool FinishedTravel { get => (int)Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }
        public bool IsSpawner { get => (int)Projectile.ai[2] == 0; set => Projectile.ai[2] = value ? 0 : 1; }
        Vector2 initialPosition = Vector2.Zero;
        // Position, Scale
        public List<(Vector2, float)> afterimages = new();
        List<(Vector2, int)> zones = new();
        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0.375f;

            Projectile.DamageType = ModContent.GetInstance<UniversalHybrid>();
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 100;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 28;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override string Texture => BasicBullet.route + "InkKnife";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);

            initialPosition = Projectile.Center;

            if (!IsSpawner)
            {
                Projectile.width = 18;
                Projectile.height = 18;
                Projectile.localNPCHitCooldown = 8;
            }

            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server) return;

            if (IsSpawner)
            {
                if (Projectile.timeLeft <= 120 && Projectile.timeLeft%30 == 0 && 
                    Main.myPlayer == Projectile.owner)
                {
                    NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                    Vector2 targetPosition = Main.MouseWorld;

                    if (target != null)
                    {
                        targetPosition = target.Center + (target.velocity * 10);
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                        Projectile.Center.DirectionTo(targetPosition), Projectile.type, 
                        (int)(Projectile.damage * 0.15f), (int)(Projectile.knockBack * 0.25f), 
                        Projectile.owner, BulletColor, 0, 1);
                }
            } else
            {
                if (Main.myPlayer == Projectile.owner && !FinishedTravel)
                {
                    if (Projectile.timeLeft%10 == 0)
                    {
                        zones.Add((Projectile.Center, 0));

                        NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                        Vector2 targetPosition = Main.MouseWorld;

                        if (target != null)
                        {
                            targetPosition = target.Center + (target.velocity * 10);
                        }

                        if (Projectile.Center.Distance(targetPosition) <= 18)
                        {
                            Projectile.Center = targetPosition;
                            FinishedTravel = true;
                        }
                        float maxRotation = Main._rand.NextFloat(0, MathHelper.PiOver2 * 0.75f);
                        Projectile.velocity = Projectile.Center.DirectionTo(targetPosition)
                            .RotatedByRandom(maxRotation) * 18;

                        Projectile.netUpdate = true;
                    }
                    (Vector2, float) current = (Projectile.Center, 1);
                    afterimages.Add(current);

                    if (timer >= 180)
                    {
                        FinishedTravel = true;
                    }
                }

                for (int i = 0; i < afterimages.Count; i++)
                {
                    afterimages[i] = (afterimages[i].Item1, afterimages[i].Item2 - 0.05f);
                }

                afterimages.RemoveAll((x) => x.Item2 <= 0);

                for (int i = 0; i < zones.Count; i++)
                {
                    zones[i] = (zones[i].Item1, zones[i].Item2 + 1);
                }

                zones.RemoveAll((x) => x.Item2 >= 18);

                if (FinishedTravel && zones.Count <= 0)
                {
                    Projectile.Kill();
                    Projectile.timeLeft = 2;
                }
            }

            timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (IsSpawner) return base.OnTileCollide(oldVelocity);
            FinishedTravel = true;
            return zones.Count <= 0 && afterimages.Count <= 0;
        }

        public override bool PreKill(int timeLeft)
        {
            if (IsSpawner) return base.PreKill(timeLeft);
            FinishedTravel = true;
            return zones.Count <= 0 && afterimages.Count <= 0;
        }

        public override bool ShouldUpdatePosition()
        {
            return !FinishedTravel;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CheckAABBvAABBCollision(projHitbox.TopLeft(), projHitbox.Size(),
                targetHitbox.TopLeft(), targetHitbox.Size()))
            {
                return Projectile.Center.Distance(targetHitbox.Center.ToVector2()) <=
                    (Projectile.width / 2) + (targetHitbox.Size().X / 2);
            }

            Vector2 previous = initialPosition;

            float worthless = 0;
            foreach ((Vector2, int) point in zones)
            {
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), 
                    previous, point.Item1, 9, ref worthless))
                {
                    return true;
                }
                previous = point.Item1;
            }

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                previous, Projectile.Center, 9, ref worthless))
            {
                return true;
            }

            return false;
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
            if (IsSpawner)
            {
                Rectangle bounds = BasicBullet.GetBulletBounds(Sheet.Big, (SheetFrame)BulletColor);

                Main.EntitySpriteDraw(new DrawData(
                    BasicBullet.GetBulletTexture(Sheet.Big),
                    Projectile.Center - Main.screenPosition,
                    bounds,
                    Color.White * Projectile.Opacity,
                    Projectile.rotation,
                    bounds.Size() * 0.5f,
                    Projectile.scale,
                    SpriteEffects.None)
                );
            } else
            {
                Asset<Texture2D> body = Terraria.GameContent.TextureAssets.Projectile[Type];

                Main.EntitySpriteDraw(new DrawData(
                    body.Value,
                    Projectile.Center - Main.screenPosition,
                    body.Value.Bounds,
                    Color.White * Projectile.Opacity,
                    Projectile.rotation,
                    body.Value.Size() * 0.5f,
                    Projectile.scale,
                    SpriteEffects.None)
                );
            }
            return false;
        }
    }
}
