using lenen.Common.Players;
using lenen.Common.Utils;
using lenen.Content.NPCs.TasoukenBoss;
using lenen.Content.Projectiles;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace lenen.Content.Items.Weapons
{
    public class Tasouken : SpellCardItem
    {
        public const string InstantPowerName = "Vii_";

        public bool CanSpawnTasouken { get => !powerProven && NPC.downedGolemBoss; }

        protected override SpellCard SpellCardID => powerProven ?
            Common.Players.SpellCard.TwoGlimmers : Common.Players.SpellCard.CloudMowing;
        protected override int SpellCardCooldown => 900;
        protected override int DesperateCooldown => 1500;
        protected override int ManaUse => 0;
        protected override int DesperateManaUse => 0;

        private bool powerProven = false;
        private string owner = "";
        int[] cutsCooldown = { 0, 0, 0 };

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.shoot = ModContent.ProjectileType<Swing>();
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 8;
            Item.ArmorPenetration = 14;

            Item.width = 64;
            Item.height = 66;
            Item.value = Item.sellPrice(0, 1, 10, 50);
            Item.rare = ItemRarityID.Blue;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shootsEveryUse = true;
            Item.useTurn = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void OnSpawn(IEntitySource source)
        {
            if (source.Context != null)
            {
                string[] splitContext = source.Context.Split(":");
                if (splitContext.Length == 2)
                {
                    owner = splitContext[0];
                    powerProven = splitContext[1] == "True";
                }
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("TasoukenOwner"))
            {
                owner = tag.GetString("TasoukenOwner");
            } else
            {
                owner = "";
            }

            if (tag.ContainsKey("PowerProved"))
            {
                powerProven = tag.GetBool("PowerProved");
            } else
            {
                powerProven = false;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["TasoukenOwner"] = owner;
            tag["PowerProved"] = powerProven;
        }

        public override void OnCreated(ItemCreationContext context)
        {
            base.OnCreated(context);
        }

        public override bool CanPickup(Player player)
        {
            if (powerProven) return player.name == owner || player.name == InstantPowerName;

            return base.CanPickup(player);
        }

        public override bool OnPickup(Player player)
        {
            return base.OnPickup(player);
        }

        public override bool? UseItem(Player player)
        {
            return base.UseItem(player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);

            if (!powerProven && NPC.downedGolemBoss)
            {
                int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");
                if (index != -1)
                {
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "TasoukenState",
                        "This weapon awaits a challenger. Use it to summon the Tasouken."));
                }
            }
            if (powerProven)
            {
                int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");
                if (index != -1)
                {
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "TasoukenState",
                        "This weapon's full power has been unleashed."));
                }
            }
        }

        public override void UpdateInventory(Player player)
        {
            for (int i = 0; i < cutsCooldown.Length; i++)
            {
                if (cutsCooldown[i] > 0) cutsCooldown[i]--;
            }

            Item.consumable = CanSpawnTasouken;
        }

        public override void HoldItem(Player player)
        {
            if ((powerProven && player.name == owner) || player.name == InstantPowerName)
            {
                player.GetModPlayer<BuffPlayer>().CanCut = true;
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if ((powerProven && player.name == owner) || player.name == InstantPowerName)
            {
                damage.Base += 52;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (CanSpawnTasouken && player.name != InstantPowerName) return false;

            if (player.altFunctionUse == 2)
            {
                SpellCardManagement scManager = player.GetModPlayer<SpellCardManagement>();

                SetCooldown(player);
                SpellCard(player, scManager.desperateBomb);
                return false;
            }

            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f),
                type, damage, knockback, player.whoAmI, player.direction * player.gravDir,
                player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);

            int cutType = ModContent.ProjectileType<Cut>();
            Vector2 vel = new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi);
            float length = 1f;
            Projectile.NewProjectile(source, Main.MouseWorld, vel, cutType, damage, 
                knockback, player.whoAmI, length, 
                BulletUtils.GetRandomColor([SheetFrame.White, SheetFrame.Pink, SheetFrame.Yellow]));

            if ((powerProven && player.name == owner) || player.name == InstantPowerName)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 secondaryVel = vel.RotatedBy(MathHelper.PiOver2);
                    secondaryVel = secondaryVel.RotatedByRandom(MathHelper.PiOver4);
                    int color = BulletUtils.GetRandomColor([SheetFrame.White, SheetFrame.Pink, SheetFrame.Yellow]);
                    Projectile.NewProjectile(source, Main.MouseWorld, secondaryVel,
                        cutType, damage, knockback, player.whoAmI, 0.4f, color);
                }
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool ConsumeItem(Player player)
        {
            return CanSpawnTasouken && (owner == player.name || owner == "") && 
                !NPC.AnyNPCs(ModContent.NPCType<TasoukenBoss>());
        }

        public override void OnConsumeItem(Player player)
        {
            int type = ModContent.NPCType<TasoukenBoss>();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, type);
            }
            else
            {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            if (owner == player.name && CanSpawnTasouken)
            {
                return false;
            }

            return CanUseSpellCard(player);
        }

        protected override void SpellCard(Player player, bool desperate)
        {
            if (owner == "")
            {
                owner = player.name;
            }

            if ((powerProven && player.name == owner) || player.name == InstantPowerName)
            {
                UpgradedSpellCard(player, desperate);
            } else
            {
                NormalSpellCard(player, desperate);
            }
        }

        public void NormalSpellCard(Player player, bool desperate)
        {
            int dmg = (int)(player.GetWeaponDamage(Item) * 1.6f);
            int difficulty = 1;
            float rotation = 2f * MathHelper.TwoPi / 110f / 3f;
            int amount = 6;

            if (desperate)
            {
                dmg = (int)(player.GetWeaponDamage(Item) * 2.8f);
                difficulty = 3;
                rotation = MathHelper.TwoPi / 110f;
                amount = 10;
            }

            int direction = player.Center.X <= Main.MouseWorld.X ? 1 : -1;
            Vector2 vel = new Vector2(0, -1);
            int type = ModContent.ProjectileType<InfiniteLaser>();

            for (int i = amount; i > 0; i--)
            {
                int color = BulletUtils.GetRandomColor(difficulty);
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item,
                    "Spellcard"), player.Center, vel, type, dmg, Item.knockBack,
                    player.whoAmI, i * 4, rotation * direction, color);
            }
        }

        public void UpgradedSpellCard(Player player, bool desperate)
        {
            SpellCardManagement scManager = player.GetModPlayer<SpellCardManagement>();

            int damage = (int)(player.GetWeaponDamage(Item) * 1.4f);
            int time = 150;

            if (desperate)
            {
                damage = (int)(player.GetWeaponDamage(Item) * 1.7f);
                time += 60;
            }

            scManager.twoGlimmersDamage = damage;
            scManager.twoGlimmersTimer = scManager.twoGlimmersLastMax = time;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.SoulofFlight, 10)
                .AddIngredient<SwordShapedBottleOpener>(1)
                .AddTile(TileID.DemonAltar)
                .AddCondition(Condition.DownedMechBossAny)
                .Register();
        }
    }
}
