using lenen.Common.Utils;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles.TasoukenProjectiles
{
    class RingCircle : ModProjectile
    {
        public int TasoukenIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int AddedTime { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int TargetPhase { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int ownerType = -1;
        public Vector2 initialVel = Vector2.Zero;
        public Vector2 axis = Vector2.Zero;
        public Vector2 offset = Vector2.Zero;

        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;

            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 480;
        }

        public override string Texture => BulletUtils.texturePaths[Sheet.ReverseDefault];

        public override void OnSpawn(IEntitySource source)
        {
            if (AddedTime != 0)
            {
                initialVel = Projectile.velocity;

                axis = Projectile.Center;
                Projectile.Center = axis + new Vector2(200, 0).
                    RotatedBy(AddedTime * MathHelper.TwoPi / 30f);

                offset = axis - Projectile.Center;
            }
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);

            if (CheckBoss(TasoukenIndex))
            {
                ownerType = Main.npc[TasoukenIndex].type;
            }
            // Main.NewText("Damage: " + Projectile.damage);
        }

        public static bool CheckBoss(int index)
        {
            if (index <= -1) return false;

            NPC tasouken = Main.npc[index];

            return tasouken.active && tasouken.life > 1;
        }

        public static bool CheckBoss(int index, int type)
        {
            if (!CheckBoss(index)) return false;

            NPC tasouken = Main.npc[index];

            return tasouken.type == type;
        }

        public static bool ValidatePhase(int index, int type, int phase)
        {
            if (CheckBoss(index, type))
            {
                NPC tasouken = Main.npc[index];

                return tasouken.ai[1] == phase;
            }

            return false;
        }

        public override void AI()
        {
            if (!ValidatePhase(TasoukenIndex, ownerType, TargetPhase))
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bullet_del"), Projectile.Center);
                Projectile.timeLeft = 0;
                return;
            }

            if (AddedTime == 0)
            {
                Projectile.velocity *= 0.99f;
                Projectile.velocity.Y += 0.1f;
                return;
            }

            //axis += initialVel;
            if (AddedTime < 0)
            {
                //Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.TwoPi / -3000f);
                offset = offset.RotatedBy(0.0255f);
            } else
            {
                //Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.TwoPi / 3000f);
                offset = offset.RotatedBy(-0.0255f);
            }

            axis += Projectile.velocity;

            Projectile.Center = axis + offset;

            Projectile.netUpdate = true;

            int slashType = ModContent.ProjectileType<BulletCut>();

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == slashType)
                {
                    if (projectile.Colliding(projectile.Hitbox, Projectile.Hitbox))
                    {
                        Projectile.timeLeft = 0;
                        SpawnDebree(4);
                        return;
                    }
                }
            }
        }

        public void SpawnDebree(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                float speed = Main._rand.NextFloat(-8, -2);
                Vector2 dir = new Vector2(0, speed).RotatedByRandom(MathHelper.PiOver2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                    dir, Type, (int)(Projectile.damage * 0.5f), 3f, ai0: TasoukenIndex, ai1: 
                    0, ai2: TargetPhase);
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }

        public override bool ShouldUpdatePosition()
        {
            return AddedTime == 0;
            // return true; 
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int color = (int)SheetFrame.Yellow;
            int shape = (int)Sheet.ReverseDefault;

            if (AddedTime == 0)
            {
                color = (int)SheetFrame.White;
                shape = (int)Sheet.Small;
            }

            Main.EntitySpriteDraw(
                BulletUtils.GetTexture(color, shape) with
                {
                    position = Projectile.Center - Main.screenPosition
                });

            return false;
        }
    }
}
