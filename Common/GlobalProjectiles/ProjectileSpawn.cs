using lenen.Common.Players;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.GlobalProjectiles
{
    public class ProjectileSpawn : GlobalProjectile
    {
        bool flameUpgrade = false;
        bool spawnFlames = false;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            bool modProjectileStatement = false;
            if (entity.ModProjectile != null)
            {
                modProjectileStatement = entity.ModProjectile is SpiritFlame;
            }
            return !(modProjectileStatement || entity.minion || entity.sentry);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // This took unreasonably long to test
            /*Main.NewText("Spawned a " + projectile.Name);
            Main.NewText("Stats:");
            Main.NewText($"Is spirit flame: {projectile.ModProjectile is SpiritFlame}");
            if (projectile.ModProjectile != null)
            {
                Main.NewText($"Mod Projectile: {projectile.ModProjectile.Name}");
            } else
            {
                Main.NewText($"Mod Projectile is null");
            }
            Main.NewText($"Damage: {projectile.damage}");
            Main.NewText($"Minion: {projectile.minion}");
            Main.NewText($"Hostile: {projectile.hostile}");
            Main.NewText($"NPCProj: {projectile.npcProj}");
            Main.NewText($"Owner: {projectile.owner}");
            Main.NewText($"Source: {source.GetType().Name}");
            Main.NewText($"Source context: {source.Context}");
            Main.NewText($"Damage type: {projectile.DamageType}");

            bool modProjectileStatement = false;
            if (projectile.ModProjectile != null)
            {
                modProjectileStatement = projectile.ModProjectile is SpiritFlame;
            }
            Main.NewText($"Mod flag: {!modProjectileStatement}");

            Main.NewText($"Complete check: {!(modProjectileStatement || projectile.damage <= 0 || 
                projectile.minion || projectile.hostile || projectile.npcProj)}");

            if (modProjectileStatement || projectile.damage <= 0 ||
                projectile.minion || projectile.hostile || projectile.npcProj)
            {
                return;
            }*/

            if (Main.dedServ || projectile.owner < 0 || projectile.owner >= Main.player.Length || 
                (source is not IEntitySource_WithStatsFromItem && source is not EntitySource_ItemUse_WithAmmo && 
                source is not EntitySource_ItemUse) || projectile.hostile || projectile.npcProj || 
                projectile.minion || projectile.damage <= 0) return;

            Player owner = Main.player[projectile.owner];
            SpiritFlamesPlayer SFP = owner.GetModPlayer<SpiritFlamesPlayer>();
            SoulAbsorptionPlayer SAP = owner.GetModPlayer<SoulAbsorptionPlayer>();
            if (SFP.spiritFires && SFP.flamesCooldown <= 0 && SFP.singleQueueRequests <= 5 && 
                SAP.soulsCollected >= 18)
            {
                //Main.NewText("Projectile affected: " + projectile.Name);
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bullet_del"), projectile.Center);
                SFP.QueueCooldown(360);
                SAP.QueueDeduction(18);
                spawnFlames = true;
                flameUpgrade = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (flameUpgrade)
            {
                //Main.NewText("Applying extra damage from " + projectile.Name);
                modifiers.FinalDamage += 0.25f;
                modifiers.CritDamage += 1f;
                modifiers.SetCrit();
            }
            base.ModifyHitNPC(projectile, target, ref modifiers);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (flameUpgrade)
            {
                target.AddBuff(BuffID.ShadowFlame, 90);
            }
            if (spawnFlames && Main.myPlayer == projectile.owner)
            {
                Player owner = Main.player[projectile.owner];
                //Main.NewText("Spawning flames from " + projectile.Name);
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center,
                    new Vector2(Main.rand.Next(2, 11), 0).RotatedByRandom(MathHelper.TwoPi),
                    ModContent.ProjectileType<SpiritFlame>(),
                    (int)owner.GetTotalDamage(owner.HeldItem.DamageType).
                    ApplyTo(25 + (owner.HeldItem.damage * 0.5f)), 2, projectile.owner, ai0: 1);
                }
                spawnFlames = false;
            }
            base.OnHitNPC(projectile, target, hit, damageDone);
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (flameUpgrade)
            {
                //Main.NewText("Applying extra damage from " + projectile.Name);
                modifiers.FinalDamage += 0.25f;
            }
            base.ModifyHitPlayer(projectile, target, ref modifiers);
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (flameUpgrade)
            {
                target.AddBuff(BuffID.ShadowFlame, 90);
            }
            if (spawnFlames && Main.myPlayer == projectile.owner)
            {
                Player owner = Main.player[projectile.owner];
                //Main.NewText("Spawning flames from " + projectile.Name);
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center,
                    new Vector2(Main.rand.Next(2, 11), 0).RotatedByRandom(MathHelper.TwoPi),
                    ModContent.ProjectileType<SpiritFlame>(),
                    (int)owner.GetTotalDamage(owner.HeldItem.DamageType).
                    ApplyTo(25 + (owner.HeldItem.damage * 0.5f)), 2,
                    projectile.owner, ai0: 1);
                }
                spawnFlames = false;
            }
            base.OnHitPlayer(projectile, target, info);
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            //Main.NewText($"Velocity: {projectile.velocity}");
            //Main.NewText($"Old velocity: {oldVelocity}");
            if (spawnFlames && Main.myPlayer == projectile.owner)
            {
                Player owner = Main.player[projectile.owner];
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center,
                    new Vector2(Main.rand.Next(2, 11), 0).RotatedByRandom(MathHelper.TwoPi),
                    ModContent.ProjectileType<SpiritFlame>(),
                    (int)owner.GetTotalDamage(owner.HeldItem.DamageType).
                    ApplyTo(25 + (owner.HeldItem.damage * 0.5f)), 2,
                    projectile.owner, ai0: 1);
                }
                spawnFlames = false;
            }
            return base.OnTileCollide(projectile, oldVelocity);
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            base.OnKill(projectile, timeLeft);
        }
    }
}
