using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;

namespace lenen.Content.Projectiles
{
    public class InfiniteLaser : ModProjectile
    {
        private Vector2 origin = Vector2.Zero;
        private Vector2 offset = Vector2.Zero;
        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
            Projectile.width = 70;
            Projectile.height = 234;

            Projectile.damage = 30;
            Projectile.knockBack = 12;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 110;
            Projectile.light = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 50;

            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = Projectile.ai[2];

            //Projectile.Center += Projectile.Size * Projectile.scale;
            origin = Projectile.Center + new Vector2(0, -100 * Projectile.scale).RotatedBy(Projectile.velocity.ToRotation());
            Projectile.rotation = Projectile.velocity.ToRotation();
            offset = Projectile.Center - origin;
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/laser_3"), Projectile.Center);
            Projectile.velocity = Vector2.Zero;
        }

        public override void AI()
        {
            Projectile.Center = origin + offset;
            Projectile.rotation = origin.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2;
            // Projectile.rotation = origin.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2;
            if (Projectile.timeLeft == 105 && Projectile.ai[0] > 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(-1, 0),
                    ModContent.ProjectileType<InfiniteLaser>(), Projectile.damage, Projectile.knockBack, 
                    Projectile.owner, Projectile.ai[0]-1, Projectile.ai[1], Projectile.ai[2]);
            }
            if (Projectile.timeLeft > 20 && Projectile.timeLeft <= 90)
            {
                /*Projectile.velocity = new Vector2(0, Projectile.ai[1] * Projectile.scale)
                    .RotatedBy(Projectile.Center.DirectionFrom(origin).ToRotation());
                if (Projectile.Center.Distance(origin) > 100 * Projectile.scale)
                {
                    Main.NewText("Too far");
                }*/
                offset = offset.RotatedBy(Projectile.ai[1]);
            } else
            {
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float widthMultiplier = 70f;
            float collisionPoint = 1f;

            Vector2 tip = (Projectile.Center + new Vector2(0, 117 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 bottom = (Projectile.Center + new Vector2(0, -117 * Projectile.scale)).RotatedBy(Projectile.rotation, Projectile.Center);

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip,
                bottom, widthMultiplier * Projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }
    }
}
