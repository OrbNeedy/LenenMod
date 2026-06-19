using lenen.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    public class MeleeBasicBullet : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int BulletType { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.Default];

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
        }

        public override void AI()
        {
            if (BulletType != 0)
            {
                Projectile.velocity *= 0.985f;
                Projectile.velocity.Y += 0.15f;

                return;
            }

            int slashType = ModContent.ProjectileType<JudgementCut>();
            int slashType2 = ModContent.ProjectileType<Cut>();
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == slashType || proj.type == slashType2)
                {
                    ModProjectile cut = proj.ModProjectile;
                    if ((bool)cut.Colliding(proj.Hitbox, Projectile.Hitbox))
                    {
                        SpawnDebree(3);
                        Projectile.timeLeft = 0;
                        return;
                    }
                }
            }
        }

        public void SpawnDebree(int amount = 1)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            for (int i = 0; i < amount; i++)
            {
                float speed = Main._rand.NextFloat(-8, -2);
                Vector2 dir = new Vector2(0, speed).RotatedByRandom(MathHelper.PiOver2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                    dir, Type, Projectile.damage, 2f, Projectile.owner, 
                    (int)SheetFrame.White, (int)Sheet.Small, 1);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(BulletColor, BulletSprite) with
                {
                    position = Projectile.Center - Main.screenPosition
                }
                );

            return false;
        }
    }
}
