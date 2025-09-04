using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using lenen.Common.Systems;
using System;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class SuperNovaBullet : ModProjectile
    {
        private int NumberOfExplosions { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private bool ChristmasEdition { get => Projectile.ai[1] == 0; set => Projectile.ai[1] = value ? 0 : 1; }

        private bool sucking = false;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0.35f;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 185;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (sucking)
            {
                // Attracts bullets and modifies the appearence of it
                if (Projectile.Opacity < 1)
                {
                    Projectile.Opacity += 0.003921568f;
                }
                if (Projectile.scale > 1)
                {
                    Projectile.scale -= 0.375f;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 0.5f;
                    Projectile.netUpdate = true;
                }

                SuckInProjectiles(Projectile.Center, 1200, 14, true);
                SuckInNPCs(Projectile.Center, 1200, 14);
                if (Projectile.timeLeft <= 5)
                {
                    player.GetModPlayer<GravityPlayer>().SpawnExplosion(ChristmasEdition, player.GetSource_FromThis("Spellcard"));
                    SuckInProjectiles(Projectile.Center, 2400, -76);
                    SuckInNPCs(Projectile.Center, 2400, -76);
                    if (NumberOfExplosions > 0)
                    {
                        sucking = false;
                        Projectile.timeLeft = 185;
                        NumberOfExplosions -= 1;
                    } else
                    {
                        Projectile.timeLeft = 0;
                    }
                }
            } else
            {
                if (Projectile.Opacity > 0)
                {
                    Projectile.Opacity -= 0.0156862f;
                }
                if (Projectile.scale < 75)
                {
                    Projectile.scale += 2.25f;
                }
                if (Projectile.timeLeft <= 5 && NumberOfExplosions > 0)
                {
                    sucking = true;
                    Projectile.timeLeft = 305;
                    Projectile.Center = player.Center;
                    // Suck sound
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/charge_3") with
                    {
                        Volume = 0.5f
                    }, Projectile.Center);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = Color.Black;
            Texture2D texture = ModContent.Request<Texture2D>(
                "lenen/Content/Projectiles/BulletHellProjectiles/SuperNovaBullet").Value;

            if (ChristmasEdition)
            {
                texture = ModContent.Request<Texture2D>(
                    "lenen/Content/Projectiles/BulletHellProjectiles/SuperPresent").Value;
                color = Color.White;
            }

            Main.EntitySpriteDraw(new DrawData(texture,
                Projectile.Center - Main.screenPosition,
                texture.Bounds,
                color * Projectile.Opacity,
                Projectile.rotation,
                texture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None)
            );

            return false;
        }

        public static void SuckInProjectiles(Vector2 center, float range, float strength, bool fullTimeExtension = false)
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (!GravityExceptions.Instance.gravityAffectedProjectiles.Contains(projectile.type)) continue;
                
                float distance = projectile.Center.Distance(center);
                if (distance <= range && projectile.friendly)
                {
                    projectile.velocity = Vector2.Lerp(projectile.velocity,
                        projectile.DirectionTo(center) * strength, 0.06f);

                    if (Main.rand.NextBool() || fullTimeExtension) projectile.timeLeft += 1;

                    projectile.netUpdate = true;
                }
            }
        }

        public static void SuckInNPCs(Vector2 center, float range, float strength)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float distance = npc.Distance(center);
                // Not a boss, not friendly, knockback restist over 0, in range, not a gravity resistant NPC
                if (!npc.boss && !npc.friendly && npc.knockBackResist > 0 && distance <= range * 2 &&
                    !GravityExceptions.Instance.gravityResistantNPCs.Contains(npc.type))
                {
                    npc.velocity = Vector2.Lerp(npc.velocity,
                        npc.DirectionTo(center) * strength * npc.knockBackResist, 0.12f);
                    npc.netUpdate = true;
                }
            }
        }
    }
}
