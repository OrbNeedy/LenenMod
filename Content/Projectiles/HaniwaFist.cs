using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public class HaniwaFist : ModProjectile
    {
        private float FistIndex
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public bool shooting = false;
        public bool staying = true;
        private int safetyTimer = 0;
        public int safeReturnTimer = 0;
        private Vector2 wantedPosition;

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.scale = 1f;
            Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;

            DrawOffsetX = -12;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] == 1)
            {
                shooting = true;
            }
            base.OnSpawn(source);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            if (FistIndex == 1)
            {
                
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }

            UpdatePosition(player);

            UpdateAim(player.Center, player.HeldItem.shootSpeed);
        }

        private void UpdatePosition(Player player)
        {
            float speed = 1f;
            if (shooting)
            {
                staying = false;
                safeReturnTimer = 0;
                Projectile.damage = (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(player.HeldItem.damage);
                Projectile.knockBack = player.GetTotalKnockback(DamageClass.Summon).ApplyTo(player.HeldItem.knockBack);
                safetyTimer++;
                if (Projectile.position.DistanceSQ(Main.MouseWorld) <= 400 || safetyTimer >= 300)
                {
                    shooting = false;
                    safetyTimer = 0;
                }
                wantedPosition = Main.MouseWorld;

                player.heldProj = Projectile.whoAmI;
                player.itemTime = 2;
                player.itemAnimation = 2;

            } else
            {
                Projectile.damage = 0;
                float additionalOffsetRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
                Vector2 beamIdOffset = new Vector2(0, -50).RotatedBy((FistIndex * MathHelper.Pi) +
                    additionalOffsetRotation + MathHelper.Pi);
                Vector2 optionOffset = new Vector2(0, -10);
                wantedPosition = player.Center + beamIdOffset + optionOffset;
                safeReturnTimer++;
                speed += 1 + (safeReturnTimer/120);
                if (Projectile.position.DistanceSQ(wantedPosition) <= (350 * speed) || safeReturnTimer >= 180)
                {
                    safeReturnTimer = 0;
                    staying = true;
                }
            }

            if (staying)
            {
                Projectile.Center = wantedPosition;
            } else
            {
                if (Projectile.ai[2] == 2)
                {
                    GrabNPCs();
                } else
                {
                    GrabItems();
                }
                Projectile.velocity = Projectile.position.DirectionTo(wantedPosition) * 
                    player.HeldItem.shootSpeed * speed;
            }
            //Projectile.Center = Vector2.Lerp(Projectile.Center, wantedPosition, speed);
        }

        private void UpdateAim(Vector2 source, float speed)
        {
            float offsetRotation = FistIndex <= 1 ? -MathHelper.PiOver4 : MathHelper.PiOver4;

            if (shooting)
            {
                Projectile.rotation = (Projectile.Center - Main.MouseWorld).ToRotation() -
                MathHelper.PiOver2;
            } else
            {
                Projectile.rotation = (source - Main.MouseWorld).ToRotation() -
                MathHelper.PiOver2;
            }
        }

        public override void OnKill(int timeLeft)
        {
            //Main.NewText("Killing fist number " + FistIndex);
            base.OnKill(timeLeft);
        }

        public void GrabItems()
        {
            foreach (var item in Main.ActiveItems)
            {
                if (Collision.CheckAABBvAABBCollision(Projectile.position, Projectile.Size, 
                    item.position, item.Size) && !item.beingGrabbed)
                {
                    item.Center = Projectile.Center;
                }
            }
        }

        public void GrabNPCs()
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (Collision.CheckAABBvAABBCollision(Projectile.position, Projectile.Size,
                    npc.position, npc.Size) && npc.friendly && shooting)
                {
                    npc.Center = Projectile.Center;
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return !staying;
        }

        public override bool? CanCutTiles()
        {
            return !staying;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D fist = ModContent.Request<Texture2D>("lenen/Content/Projectiles/HaniwaFist").Value;
            
            int frameHeight = fist.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;
            
            Rectangle sourceRectangle = new Rectangle(0, startY, fist.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;

            SpriteEffects effects = SpriteEffects.None;
            if (FistIndex != 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            float offsetY = 14f;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

            Color color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(
                fist,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle,
                color,
                Projectile.rotation,
                origin,
                1f,
                effects
            );
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }
    }
}
