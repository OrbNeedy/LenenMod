using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace lenen.Content.Projectiles
{
    public class DarkKnife : ModProjectile
    {
        float speed = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.damage = 20;
            Projectile.knockBack = 5;
            Projectile.penetrate = 1;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 480;
            Projectile.ownerHitCheck = false;
            Projectile.tileCollide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft <= 420)
            {
                Vector2 down = new Vector2(0, 16);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, down, 0.015f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                target.AddBuff(BuffID.Confused, 600);
                target.AddBuff(BuffID.Darkness, 600);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool())
            {
                target.AddBuff(BuffID.Confused, 600);
                target.AddBuff(BuffID.Darkness, 600);
            }
        }
    }
}
