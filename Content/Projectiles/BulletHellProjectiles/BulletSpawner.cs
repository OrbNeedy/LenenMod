using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.BulletHellProjectiles
{
    class BulletSpawner : ModProjectile
    {
        public int BulletColor { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int BulletSprite { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Behavior { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        Emitter[] emitters = new[] { new Emitter(), new Emitter(), new Emitter() };

        public override void SetDefaults()
        {
            Projectile.Size = Vector2.Zero;
            Projectile.scale = 1f;
            Projectile.light = 0;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void OnSpawn(IEntitySource source)
        {
            foreach (Emitter emitter in emitters)
            {
                emitter.position = Projectile.Center;
                emitter.aimAngle = new Vector2(0, -1).ToRotation();
                emitter.color = (SheetFrame)BulletColor;
                emitter.sprite = (Sheet)BulletSprite;
                emitter.behavior = (BulletBehavior)Main._rand.Next(
                    new int[] { (int)BulletBehavior.Serpentine,
                    (int)BulletBehavior.NegativeSerpentine });
                // Main.NewText("One set to " + emitter.behavior);
                emitter.owner = Projectile.owner;
                emitter.minSpeed = emitter.maxSpeed = 8;
                emitter.maxTimer = 10;
                //emitter.initialTimer = Main._rand.Next(0, 180);
            }
        }

        public override void AI()
        {
            foreach (Emitter emitter in emitters)
            {
                float thisX = emitter.behavior == BulletBehavior.Serpentine ? -80 : 80;
                emitter.position = Projectile.Center + new Vector2(Main._rand.NextFloat(-80, 80) + thisX, 0);

                emitter.Update(Projectile.GetSource_FromThis(), Projectile.damage, 
                    Projectile.knockBack);

                emitter.FinalUpdate();
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
