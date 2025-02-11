using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using lenen.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.Audio;
using System.Collections.Generic;

namespace lenen.Common.GlobalItems
{
    public class SpiritFireModification : GlobalItem
    {
        public bool upgradedHit;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return !(entity.axe > 0 || entity.pick > 0 || entity.hammer > 0 || entity.damage <= 0 ||
                entity.accessory || entity.defense > 0 || entity.ammo != AmmoID.None || entity.vanity ||
                entity.createTile != -1);
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (item.axe > 0 || item.pick > 0 || item.hammer > 0) return base.UseItem(item, player);

            SpiritFlamesPlayer SFP = player.GetModPlayer<SpiritFlamesPlayer>();
            SoulAbsorptionPlayer SAP = player.GetModPlayer<SoulAbsorptionPlayer>();
            if (SFP.spiritFires && SFP.flamesCooldown <= 0 && SAP.soulsCollected >= 18)
            {
                if (item.shoot == ProjectileID.None)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center,
                            new Vector2(Main.rand.Next(2, 11), 0).RotatedByRandom(MathHelper.TwoPi),
                            ModContent.ProjectileType<SpiritFlame>(), 
                            (int)player.GetTotalDamage(item.DamageType).ApplyTo(25 + item.damage), 4, 
                            player.whoAmI);
                    }
                    SFP.QueueCooldown(180);
                    SAP.QueueDeduction(18);
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bullet_del"), player.Center);
                }
            }
            return base.UseItem(item, player);
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(item, player, target, ref modifiers);
        }

        public override void ModifyHitPvp(Item item, Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            if (item.axe > 0 || item.pick > 0 || item.hammer > 0) return;

            SpiritFlamesPlayer SFP = player.GetModPlayer<SpiritFlamesPlayer>();
            SoulAbsorptionPlayer SAP = player.GetModPlayer<SoulAbsorptionPlayer>();
            if (SFP.spiritFires && SFP.flamesCooldown <= 0 && SAP.soulsCollected >= 18)
            {
                target.AddBuff(BuffID.ShadowFlame, 90);
                //Main.NewText("Applying extra damage from " + projectile.Name);
                modifiers.FinalDamage += 0.25f;
            }
            base.ModifyHitPvp(item, player, target, ref modifiers);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
}
