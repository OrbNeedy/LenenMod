using lenen.Content.Projectiles;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace lenen.Common.Players
{
    public enum OfudaModes
    {
        Default, // Sprite, Attack
        KingSlime, // Sprite, Attack
        EyeOfCthulhu, // Sprite, Attack
        EaterOfWorlds, // Sprite, Attack
        BrainOfCthulhu, // Sprite, Attack
        QueenBee, // Sprite, Attack
        Skeletron, // Sprite, Attack
        Deerclops, // Sprite, Attack
        WallOfFlesh, // Sprite, Attack
        QueenSlime, // Sprite, Attack
        Retinazer, // Sprite, Attack
        Spazmatism, // Sprite, Attack
        Twins,
        TheDestroyer, // Sprite, Attack
        SkeletronPrime, // Sprite, Attack
        Plantera, // Sprite, Attack
        Golem, // Sprite, Attack
        DukeFishron, // Sprite
        EmpressOfLight, // Sprite, Attack
        LunaticCultist, // Sprite, Attack
        LunarEvent,
        MoonlordsCore // Sprite, Attack
    }

    class SenriPlayer : ModPlayer
    {
        public OfudaModes currentMode;
        public bool senriActive = false;
        public bool canDropSenri = false;
        public int attackCooldown = 0;
        public int attackCount = 0;
        public bool evenNumber = true;

        public override void SaveData(TagCompound tag)
        {
            tag["LastMode"] = (int)currentMode;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("LastMode"))
            {
                currentMode = (OfudaModes)tag.GetInt("LastMode");
            }
        }

        public override void PreUpdate()
        {
            if (Main.myPlayer != Player.whoAmI) return;

            if (senriActive && Player.ItemAnimationActive && attackCooldown <= 0)
            {
                Item item = Player.HeldItem;
                if (item.axe <= 0 && item.pick <= 0 && item.hammer <= 0 && item.createTile == -1 &&
                    item.createWall == -1 && item.damage > 0 && !item.accessory && item.defense <= 0 &&
                    !item.vanity)
                {
                    Vector2 direction = Player.Center.DirectionTo(Main.MouseWorld);
                    Vector2 position = Main.MouseWorld;
                    TargetPlayer target = Player.GetModPlayer<TargetPlayer>();
                    Vector2 offset = Vector2.Zero;
                    int color = GetRandBlackWhite();
                    int oppositeColor = color == (int)SheetFrame.Black ? (int)SheetFrame.White : (int)SheetFrame.Black;
                    switch (currentMode)
                    {
                        default:
                        case OfudaModes.Default:
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                direction.RotatedByRandom(MathHelper.PiOver4 / 3f) * 40,
                                ModContent.ProjectileType<InkBullet>(), (int)(item.damage),
                                1, Player.whoAmI, GetRandBlackWhite());
                            attackCooldown = 90;
                            break;
                        case OfudaModes.KingSlime:
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                direction.RotatedByRandom(MathHelper.PiOver4 / 2f) * 22,
                                ModContent.ProjectileType<SenriSlimeProjectile>(), (int)(item.damage * 1.5f),
                                2, Player.whoAmI, GetRandBlackWhite(), (int)Sheet.Big);
                            attackCooldown = 45;
                            break;
                        case OfudaModes.EyeOfCthulhu:
                            for (int i = -1; i < 2; i += 2)
                            {
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                    direction.RotatedByRandom(MathHelper.PiOver2) * 20,
                                    ModContent.ProjectileType<CthulhuBullet>(), (int)(item.damage * 0.8f),
                                    3, Player.whoAmI, GetRandBlackWhite(), 10);
                            }
                            attackCooldown = 90;
                            break;
                        case OfudaModes.EaterOfWorlds:
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                direction * 12, ModContent.ProjectileType<WriglingProjectile>(), 
                                (int)(item.damage * 0.35f), 3, Player.whoAmI, GetRandBlackWhite(), -2, 0);
                            attackCooldown = 190;
                            break;
                        case OfudaModes.BrainOfCthulhu:
                            if (target.target != null) position = target.target.Center;

                            Vector2 basePosition = new Vector2(240, 0).RotatedByRandom(MathHelper.TwoPi);
                            for (int i = 0; i < 12; i++)
                            {
                                Vector2 currentPosition = position + basePosition.RotatedBy(MathHelper.TwoPi / 12 * i);
                                Projectile.NewProjectile(Player.GetSource_FromThis(), currentPosition,
                                    currentPosition.DirectionTo(position) * 2, ModContent.ProjectileType<CthulhuBullet>(), 
                                    (int)(item.damage * 0.15f), 1, Player.whoAmI, GetRandBlackWhite(), 1);
                            }
                            attackCooldown = 120;
                            break;
                        case OfudaModes.QueenBee:
                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                    direction.RotatedByRandom(MathHelper.PiOver4 / 2) * Main._rand.Next(6, 15),
                                    ModContent.ProjectileType<BasicBullet>(), (int)(item.damage * 0.35f),
                                    3, Player.whoAmI, GetRandBlackWhite(), (int)Sheet.Small,
                                    (int)BulletBehavior.Homing);
                            }
                            attackCooldown = 30;
                            break;
                        case OfudaModes.Skeletron:
                            for (int i = -7; i < 7; i++)
                            {
                                offset = (direction * i * 28).RotatedBy(MathHelper.PiOver2);
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + offset,
                                    direction * 18, ModContent.ProjectileType<BasicBullet>(), 
                                    (int)(item.damage * 0.75f), 4.5f, Player.whoAmI, color, 
                                    (int)Sheet.Default, (int)BulletBehavior.Penetrate);
                            }
                            attackCooldown = 60;
                            break;
                        case OfudaModes.Deerclops:
                            if (target.target != null)
                            {
                                position = target.target.Center;
                                offset = target.target.velocity * 20;
                            } else
                            {
                                offset = new Vector2(80, 0).RotatedByRandom(MathHelper.TwoPi);
                            }

                            direction = (position + offset).DirectionTo(position);

                            Projectile.NewProjectile(
                                Player.GetSource_FromThis(), position + offset, 
                                direction, ModContent.ProjectileType<DisruptionBullet>(), 
                                (int)(item.damage * 0.775f), 1.5f, Player.whoAmI, color, 
                                (int)Sheet.Default
                            );
                            attackCooldown = 90;
                            break;
                        case OfudaModes.WallOfFlesh:
                            for (int i = 0; i < 16; i++)
                            {
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                    direction.RotatedByRandom(MathHelper.TwoPi) * Main._rand.Next(6, 15),
                                    ModContent.ProjectileType<InkKnife>(), (int)(item.damage * 0.7f),
                                    3, Player.whoAmI, GetRandBlackWhite());
                            }
                            attackCooldown = 60;
                            break;
                        case OfudaModes.QueenSlime:
                            if (target.target != null) position = target.target.Center;

                            Projectile.NewProjectile(Player.GetSource_FromThis(), position - new Vector2(0, 900),
                                new Vector2(0, 28).RotatedByRandom(MathHelper.PiOver4 / 4f),
                                ModContent.ProjectileType<SenriSlimeProjectile>(), (int)(item.damage * 1.5f),
                                2, Player.whoAmI, GetRandBlackWhite(), (int)Sheet.Big);
                            attackCooldown = 45;
                            break;
                        case OfudaModes.Spazmatism:
                            for (int i = -1; i < 2; i++)
                            {
                                Projectile.NewProjectile(Player.GetSource_FromThis("SenriSpazmatism"), Player.Center,
                                    direction.RotatedBy(MathHelper.PiOver4 / 8f * i) * 14,
                                    ModContent.ProjectileType<BasicBullet>(), (int)(item.damage * 0.8f),
                                    2, Player.whoAmI, color, (int)Sheet.Pellet, (int)BulletBehavior.Penetrate);
                            }

                            attackCooldown = 50;
                            break;
                        case OfudaModes.Retinazer:
                            for (int k = 7; k < 16; k += 3)
                            {
                                for (int i = -18; i <= 18; i++)
                                {
                                    offset = new Vector2(60 * i, -900);

                                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + offset,
                                        new Vector2(0, k), ModContent.ProjectileType<BasicBullet>(),
                                        (int)(item.damage), 2, Player.whoAmI, color, (int)Sheet.Pellet,
                                        (int)BulletBehavior.Penetrate);
                                }
                            }

                            attackCooldown = 180;
                            break;
                        case OfudaModes.TheDestroyer:
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                direction * 6, ModContent.ProjectileType<WriglingProjectile>(), 
                                (int)(item.damage * 0.85f), 3, Player.whoAmI, GetRandBlackWhite(), -1, 0);
                            attackCooldown = 120;
                            break;
                        case OfudaModes.SkeletronPrime:
                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                    direction * 6, ModContent.ProjectileType<SkeletronCopyBullet>(),
                                    (int)(item.damage * 0.725f), 3, Player.whoAmI, color, 0, i);
                            }

                            attackCooldown = 150;
                            break;
                        case OfudaModes.Plantera:
                            offset = new Vector2(Main._rand.Next(0, 60), 0)
                                    .RotatedByRandom(MathHelper.TwoPi);

                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + offset,
                                new Vector2(Main._rand.NextFloat(0.001f, 0.08f)).RotatedByRandom(MathHelper.TwoPi),
                                ModContent.ProjectileType<BasicBullet>(), (int)(item.damage * 0.5f), 3,
                                Player.whoAmI, GetRandBlackWhite(), (int)Sheet.Big, (int)BulletBehavior.Penetrate);

                            attackCooldown = 20;
                            break;
                        case OfudaModes.Golem:
                            for (int i = 0; i < 22; i++)
                            {
                                float speed = Main._rand.NextBool() ? 8 : 12;
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                    new Vector2(speed, 0).RotatedByRandom(MathHelper.TwoPi),
                                    ModContent.ProjectileType<BasicBullet>(), (int)(item.damage * 0.75f), 1.5f,
                                    Player.whoAmI, GetRandBlackWhite(), (int)Sheet.Small, (int)BulletBehavior.Bounce);
                            }

                            attackCooldown = 180;
                            break;
                        case OfudaModes.DukeFishron:
                            offset = new Vector2(0, 500);
                            Projectile.NewProjectile(Player.GetSource_FromThis(), position + offset, 
                                Vector2.Zero, ModContent.ProjectileType<BulletSpawner>(), 
                                (int)(item.damage * 0.5f), 3f, Player.whoAmI, color, (int)Sheet.Pellet);

                            Projectile.NewProjectile(Player.GetSource_FromThis(), position + offset,
                                Vector2.Zero, ModContent.ProjectileType<BulletSpawner>(),
                                (int)(item.damage * 0.5f), 3f, Player.whoAmI, oppositeColor, 
                                (int)Sheet.Pellet);

                            attackCooldown = 120;
                            break;
                        case OfudaModes.EmpressOfLight:
                            for (int i = 0; i < 8; i++)
                            {
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                    direction * 10, ModContent.ProjectileType<EmpressCopyBullet>(),
                                    (int)(item.damage * 0.75), 5f, Player.whoAmI, color,
                                    evenNumber ? 1 : -1);
                                direction = direction.RotatedBy(MathHelper.TwoPi / 8);
                            }

                            attackCooldown = 210;
                            break;
                        case OfudaModes.LunaticCultist:
                            offset = direction * 50;

                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + offset,
                                Vector2.Zero, ModContent.ProjectileType<CultistLightningCopy>(), 
                                (int)(item.damage * 0.75f), 1.5f, Player.whoAmI, GetRandBlackWhite(), 
                                1, 0);

                            attackCooldown = 210;
                            break;
                        case OfudaModes.MoonlordsCore:
                            if (target.target != null)
                            {
                                direction = Player.Center.DirectionTo(target.target.Center);
                            }

                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                                direction, ModContent.ProjectileType<SenriCopyLaserHoldout>(), (int)(item.damage * 1.5f), 
                                2.5f, Player.whoAmI, color, evenNumber ? 1 : -1);

                            attackCooldown = 900;
                            break;
                    }
                    attackCount++;
                    evenNumber = !evenNumber;
                }
            }
        }

        public static int GetRandBlackWhite()
        {
            return Main._rand.Next(new int[] { (int)SheetFrame.Black,
                (int)SheetFrame.White });
        }

        public static int GetRandColor()
        {
            return (int)Main._rand.Next(BasicBullet._AvailableColors);
        }

        public static int GetRandColor(SheetFrame[] excludedColors)
        {
            SheetFrame[] possibleColors = (SheetFrame[])BasicBullet._AvailableColors.Where((x) => !excludedColors.Contains(x));
            return (int)Main._rand.Next(possibleColors);
        }

        public static int GetRandColorFromThis(SheetFrame[] possibleColors)
        {
            return (int)Main._rand.Next(possibleColors);
        }

        public override void ResetEffects()
        {
            senriActive = false; 
            canDropSenri = false;
            if (attackCooldown > 0) attackCooldown--;
        }
    }
}
