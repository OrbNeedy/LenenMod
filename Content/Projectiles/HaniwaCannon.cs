using lenen.Common.Players;
using lenen.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    // Left: 
    // Left cannon: Stack
    // Center cannon: Laser
    // Right cannon: Semi circle
    // Right: Switch left cannon and right cannon
    public class HaniwaCannon : ModProjectile
    {
        Asset<Texture2D> body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
        Projectile[] cannons = new Projectile[3];
        int baseDamage = 0;
        int material = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 190;
            Projectile.height = 100;
            Projectile.scale = 1f;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0.5f;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            baseDamage = Projectile.damage;
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/haniwa_00"), Projectile.Center);

            int[] cannonAIs = new int[] { 0, -1, -2 };
            if (Projectile.ai[0] == 1)
            {
                cannonAIs[1] = 1;
                cannonAIs[2] = 2;
            }

            int index = 0;
            foreach (int ai in cannonAIs)
            {
                cannons[index] = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, 
                    Vector2.Zero, ModContent.ProjectileType<Cannon>(), Projectile.damage, 
                    Projectile.knockBack, Projectile.owner, Projectile.whoAmI, ai);
                index++;
            }

            if (Projectile.ai[0] == 0)
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
                return;
            }
            if (player.ZoneSnow)
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/IceHaniwaFrame");
                material = 1;
                return;
            }
            else if (player.ZoneRockLayerHeight)
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/StoneHaniwaFrame");
                return;
            }
            else
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
                return;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!CheckActive(player))
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }

            int increase = 10;
            if (Projectile.ai[0] == 0)
            {
                Projectile.minionSlots = player.maxMinions;
            } else
            {
                increase = 20;
                Projectile.minionSlots = player.maxMinions/2;
            }

            Vector2 wantedPosition = player.Center + new Vector2((0 * Projectile.ai[0]) - Projectile.width/2, -360);
            float distanceToPosition = Projectile.Center.Distance(wantedPosition);
            Projectile.position = wantedPosition;

            Projectile.damage = (int)(Projectile.damage + (increase * Projectile.minionSlots));
        }

        public override void OnKill(int timeLeft)
        {
            foreach (Projectile projectile in cannons)
            {
                projectile.Kill();
            }
            base.OnKill(timeLeft);
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<HaniwaCannonBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<HaniwaCannonBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.PlayerRenderer.DrawPlayer(Main.Camera, Main.player[Projectile.owner], 
            //    Projectile.Center + new Vector2(0, 200), 0, Vector2.Zero);

            float alpha = 1;

            Main.EntitySpriteDraw(body.Value,
                Projectile.Center - Main.screenPosition,
                body.Value.Bounds,
                Color.White * alpha,
                Projectile.rotation,
                body.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );

            return base.PreDraw(ref lightColor);
        }

        /*
        private void CheckTarget()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                targetPosition = Main.MouseWorld;
                hasTarget = false;
                NPC target = Main.LocalPlayer.GetModPlayer<TargetPlayer>().target;
                if (target != null)
                {
                    if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height,
                        target.position, target.width, target.height))
                    {
                        targetPosition = target.Center;
                        hasTarget = true;
                    }
                }
            }
        }*/
    }
}
