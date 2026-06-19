using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using lenen.Content.Projectiles.BulletHellProjectiles;
using Terraria.DataStructures;
using lenen.Content.Projectiles;
using lenen.Common.Utils;

namespace lenen.Common.Players
{
    public enum SpellCard
    {
        None, 
        LaserRain, 
        CloudMowing, 
        PrimeMinister, 
        SlitSnake, 
        SuperNova, 
        SuperPresent, 
        Retrovirus, 
        MonochromeRay,
        DharmaPower, 
        SuperHaniwa, 
        TwoGlimmers
    }

    public record SpellCardData(string name, string desperateName, Color leftColor,
        Color midColor, Color rightColor)
    {
        public string name = name;
        public string desperateName = desperateName;
        public Color leftColor = leftColor;
        public Color midColor = midColor;
        public Color rightColor = rightColor;
    }

    public class SpellCardManagement : ModPlayer
    {
        public int spellCardTimer = 0;
        public int maxSinceZero = 0;
        public bool desperateBomb = false;
        public SpellCard lastSpellCard = SpellCard.None;
        public bool lastDesperate = false;
        public int timeSinceSpellcard = 210;

        public int swordDropletsLeft = 0;
        public Vector2 storedDirection = Vector2.Zero;
        public int cummulativeDropletTime = 0;
        public int dropletsDamage = 0;
        public int swordDropletsCooldown = 0;

        public int twoGlimmersTimer = 0;
        public int twoGlimmersLastMax = 0;
        public int twoGlimmersDamage = 0;

        public static Dictionary<SpellCard, SpellCardData> spellcardData = new() {
            [SpellCard.LaserRain] = new("Light Card「Laser Rain」", "Light Card「Laser Grid」", 
                new Color(5, 5, 5), new Color(188, 186, 184), new Color(163, 145, 109)),
            [SpellCard.CloudMowing] = new("「Cloud Mowing Sword」", "「Heaven Mowing Sword」",
                new Color(125, 109, 139), new Color(28, 28, 28), new Color(53, 101, 68)),
            [SpellCard.PrimeMinister] = new("「Prime Minister in Black Robes」", 
                "「Prime Minister in Black Robes」", 
                new Color(84, 91, 84), new Color(193, 179, 132), new Color(155, 130, 102)),
            [SpellCard.SlitSnake] = new("Cut Card「Slit Snake」", "Slash Card「Slit Dragon」",
                new Color(181, 152, 29), new Color(92, 67, 54), new Color(111, 166, 121)),
            [SpellCard.SuperNova] = new("New Star「Supernova」", "Super Star「Hypernova」",
                new Color(88, 0, 69), new Color(209, 202, 206), new Color(62, 41, 89)),
            [SpellCard.SuperPresent] = new("Package「Super Present」", "Gift「Special Present」",
                new Color(34, 8, 46), new Color(204, 204, 204), new Color(255, 0, 53)),
            [SpellCard.Retrovirus] = new("RNA「Retrovirus」", "Original Style「Black Wings【Shitodo】」",
                new Color(162, 162, 162), new Color(85, 83, 88), new Color(107, 86, 132)),
            [SpellCard.MonochromeRay] = new("「Monochrome Ray」", "「Monochrome Ray」",
                new Color(0, 0, 0), new Color(227, 227, 227), new Color(0, 0, 0)),
            [SpellCard.DharmaPower] = new("Dharma Power「Myōken of Heaven's Blessings」", 
                "Dharmatic Power「Strange Sword of Myōken's Heavenly Blessing」",
                new Color(131, 154, 114), new Color(150, 75, 64), new Color(0, 0, 0)),
            [SpellCard.SuperHaniwa] = new("Giant Dogū「SUPER HANIWA」", "Fusion Dogū「DELUXE HANIWA」",
                new Color(166, 184, 186), new Color(153, 84, 74), new Color(140, 108, 82)),
            [SpellCard.TwoGlimmers] = new("「Two Glimmers」", "「Two Glimmers」",
                new Color(125, 109, 139), new Color(28, 28, 28), new Color(53, 101, 68))
        };

        public override void PreUpdate()
        {
            spellCardTimer -= spellCardTimer > 0 ? 1 : 0;
            if (spellCardTimer <= 0)
            {
                maxSinceZero = 0;
            }
            else if (spellCardTimer > maxSinceZero)
            {
                maxSinceZero = spellCardTimer;
                timeSinceSpellcard = 0;

                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/bom_00") with
                {
                    Volume = 0.45f,
                    PitchVariance = 0.1f
                }, Player.Center);
            }

            if (swordDropletsLeft > 0 && swordDropletsCooldown <= 0)
            {
                Vector2 dir = storedDirection.RotatedByRandom(MathHelper.PiOver4 / 2f) * Main._rand.NextFloat(20, 40);
                Projectile.NewProjectile(new EntitySource_ItemUse(Player, Player.HeldItem, "Spellcard"),
                    Player.Center, dir, ModContent.ProjectileType<SwordDroplet>(), dropletsDamage, 3f,
                    Player.whoAmI, Main._rand.Next(0, 2), cummulativeDropletTime);

                dir = storedDirection.RotatedByRandom(MathHelper.PiOver4 / 2f) * Main._rand.NextFloat(20, 40);
                Projectile.NewProjectile(new EntitySource_ItemUse(Player, Player.HeldItem, "Spellcard"),
                    Player.Center, dir, ModContent.ProjectileType<SwordDroplet>(), dropletsDamage, 3f,
                    Player.whoAmI, Main._rand.Next(0, 2), cummulativeDropletTime);

                swordDropletsLeft--;
                swordDropletsCooldown = 2;
                cummulativeDropletTime += swordDropletsCooldown;
            }

            if (twoGlimmersTimer > 0)
            {
                if (twoGlimmersTimer % 6 == 0)
                {
                    // Bullets
                    Vector2 dir = new Vector2(0, -6).RotatedBy(0.0255f * -(twoGlimmersLastMax - twoGlimmersTimer));
                    int bulletType = ModContent.ProjectileType<MeleeBasicBullet>();

                    int color = BulletUtils.GetRandomColor(lastDesperate ? 3 : 1);
                    Projectile.NewProjectile(new EntitySource_ItemUse(Player, Player.HeldItem, "Spellcard"),
                        Player.Center, dir, bulletType, twoGlimmersDamage, 3f,
                        Player.whoAmI, color, (int)Sheet.Default);

                    dir = new Vector2(0, -6).RotatedBy(0.0255f * (twoGlimmersLastMax - twoGlimmersTimer));

                    color = BulletUtils.GetRandomColor(lastDesperate ? 3 : 1);
                    Projectile.NewProjectile(new EntitySource_ItemUse(Player, Player.HeldItem, "Spellcard"),
                        Player.Center, dir, bulletType, twoGlimmersDamage, 3f,
                        Player.whoAmI, color, (int)Sheet.Default);
                }

                int timer = lastDesperate ? 4 : 6;
                if (twoGlimmersTimer <= 40 && twoGlimmersTimer % timer == 0)
                {
                    int slashType = ModContent.ProjectileType<JudgementCut>();

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 offset = new Vector2(Main._rand.NextFloat(-750, 750), Main._rand.NextFloat(-750, 750));

                        Projectile.NewProjectile(new EntitySource_ItemUse(Player, Player.HeldItem, "Spellcard"),
                            Player.Center + offset, new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi), slashType,
                            (int)(twoGlimmersDamage * 1.65f), 5f, Player.whoAmI, (int)SheetFrame.White,
                            30 - twoGlimmersTimer);
                    }
                }

                twoGlimmersTimer--;
            }
        }

        public override void ResetEffects()
        {
            desperateBomb = false;
            if (timeSinceSpellcard < 600) timeSinceSpellcard++;
            if (swordDropletsCooldown > 0) swordDropletsCooldown--;
        }
    }
}
