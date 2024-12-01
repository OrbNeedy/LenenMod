using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    public enum BulletColors
    {
        White,
        Red, 
        Magenta, 
        DarkBlue,
        Cyan, 
        Green, 
        Yellow, 
        Black
    }

    public enum BulletSprites
    {
        Simple, 
        Big, 
        Double, 
        Pellet, 
        SuperNova,
        Gravity, 
        AllColored,
        SuperPresent
    }

    public enum BulletAIs
    {
        Simple, 
        Slowing, 
        SuperNova, 
        GravityWheel, 
        DesperateSuperNova, 
        SuperGift, 
        DesperateSuperGift, 
        Penetrating
    }

    public class FriendlyBullet : ModProjectile
    {
        // ai0 = Behavior, ai1 = Colors, ai2 = sprites
        Texture2D coloredTexture = ModContent.Request<Texture2D>(
            "lenen/Content/Projectiles/ColoredEnemyBullet").Value;
        Texture2D colorlessTexture = ModContent.Request<Texture2D>(
            "lenen/Content/Projectiles/EnemyBullet").Value;
        private int utilityTimer = 0;
        private int utilityCounter = 0;
        private bool utilityBool = false;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.light = 0.35f;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override string Texture => "lenen/Content/Projectiles/ColoredEnemyBullet";

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(utilityTimer);
            writer.Write(utilityBool);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            utilityTimer = reader.Read7BitEncodedInt();
            utilityBool = reader.ReadBoolean();
            base.ReceiveExtraAI(reader);
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] != 3 && Projectile.ai[0] != 2)
            {
                SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/e_shot_00"), Projectile.Center);
            }
            switch (Projectile.ai[0])
            {
                case (int)BulletAIs.Slowing:
                    Projectile.penetrate = -1;
                    Projectile.usesIDStaticNPCImmunity = true;
                    Projectile.idStaticNPCHitCooldown = 10;
                    //Projectile.usesLocalNPCImmunity = true;
                    //Projectile.localNPCHitCooldown = 15;
                    break;
                case (int)BulletAIs.SuperNova:
                    Projectile.timeLeft = 185;
                    Projectile.damage = 0;
                    Projectile.penetrate = -1;
                    utilityCounter = 2;
                    break;
                case (int)BulletAIs.DesperateSuperNova:
                    Projectile.timeLeft = 185;
                    Projectile.damage = 0;
                    Projectile.penetrate = -1;
                    utilityCounter = 4;
                    break;
                case (int)BulletAIs.SuperGift:
                    Projectile.timeLeft = 185;
                    Projectile.damage = 0;
                    Projectile.penetrate = -1;
                    utilityCounter = 2;
                    break;
                case (int)BulletAIs.DesperateSuperGift:
                    Projectile.timeLeft = 185;
                    Projectile.damage = 0;
                    Projectile.penetrate = -1;
                    utilityCounter = 4;
                    break;
            }
            switch (Projectile.ai[2])
            {
                case (int)BulletSprites.Big:
                    coloredTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/BigBullet").Value;
                    colorlessTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/BigBulletInside").Value;
                    break;
                case (int)BulletSprites.Double:
                    coloredTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/DoubleBullet").Value;
                    colorlessTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/DoubleBulletInside").Value;
                    break;
                case (int)BulletSprites.Pellet:
                    coloredTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/PelletBullet").Value;
                    colorlessTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/PelletBulletInside").Value;
                    break;
                case (int)BulletSprites.SuperNova:
                    coloredTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/AttractiveBullet").Value;
                    colorlessTexture = ModContent.Request<Texture2D>(
                        "lenen/Assets/Textures/Empty").Value;
                    break;
                case (int)BulletSprites.Gravity:
                    coloredTexture = ModContent.Request<Texture2D>(
                        "lenen/Assets/Textures/Empty").Value;
                    colorlessTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/GravityPullBulletWithAura").Value;
                    break;
                case (int)BulletSprites.AllColored:
                    coloredTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/ColoredEnemyBullet").Value;
                    colorlessTexture = ModContent.Request<Texture2D>(
                        "lenen/Assets/Textures/Empty").Value;
                    break;
                case (int)BulletSprites.SuperPresent:
                    coloredTexture = ModContent.Request<Texture2D>(
                        "lenen/Assets/Textures/Empty").Value;
                    colorlessTexture = ModContent.Request<Texture2D>(
                        "lenen/Content/Projectiles/SuperPresent").Value;
                    break;
            }
            base.OnSpawn(source);
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server) return;
            switch (Projectile.ai[2])
            {
                case 2:
                case 3:
                case 5:
                    if (Projectile.velocity.Length() > 0)
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    }
                    break;
            }
            switch (Projectile.ai[0])
            {
                case (int)BulletAIs.Slowing:
                    if (Projectile.timeLeft > 60)
                    {
                        Projectile.velocity *= 0.985f;
                    }
                    break;
                case (int)BulletAIs.SuperNova:
                    Projectile.netUpdate = true;
                    SuckBullets();
                    break;
                case (int)BulletAIs.SuperGift:
                    Projectile.netUpdate = true;
                    SuckBullets(true);
                    break;
                case (int)BulletAIs.GravityWheel:
                    Projectile.netUpdate = true;
                    GravityField();
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.Center = Main.MouseWorld;
                    }
                    Projectile.rotation -= 0.5f;
                    Projectile.alpha = 120;
                    break;
                case (int)BulletAIs.DesperateSuperNova:
                    Projectile.netUpdate = true;
                    SuckBullets();
                    break;
                case (int)BulletAIs.DesperateSuperGift:
                    Projectile.netUpdate = true;
                    SuckBullets(true);
                    break;
            }
        }

        public void SuckBullets(bool christmas = false)
        {
            Player player = Main.player[Projectile.owner];
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (utilityBool)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha--;
                }
                if (Projectile.scale > 1)
                {
                    Projectile.scale -= 0.25f;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 0.5f;
                }
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.ModProjectile is not FriendlyBullet || projectile == Projectile ||
                        projectile.ai[0] == (int)BulletAIs.GravityWheel) continue;
                    if (projectile.friendly == true &&
                        projectile.Distance(Projectile.Center) <= 2000)
                    {
                        projectile.timeLeft = 420; 
                        projectile.velocity = Vector2.Lerp(projectile.velocity,
                            projectile.DirectionTo(Projectile.Center) * 12, 0.06f);
                        projectile.netUpdate = true;
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.friendly || npc.knockBackResist <= 0) continue;
                    if (npc.Distance(Projectile.Center) <= 1000)
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity,
                            npc.DirectionTo(Projectile.Center) * 10 * npc.knockBackResist, 0.1f);
                    }
                }
                if (Projectile.timeLeft <= 5)
                {
                    foreach (Projectile projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.ModProjectile is not FriendlyBullet || projectile == Projectile ||
                        projectile.ai[0] == (int)BulletAIs.GravityWheel) continue;
                        if (projectile.friendly == true &&
                            projectile.Distance(Projectile.Center) <= 4000)
                        {
                            projectile.timeLeft = 420;
                            projectile.velocity += Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 
                                Main.rand.NextFloat(4, 28);
                            projectile.netUpdate = true;
                        }
                    }
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.knockBackResist <= 0) continue;
                        if (npc.Distance(Projectile.Center) <= 2000)
                        {
                            npc.velocity += Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) *
                                Main.rand.NextFloat(20, 48) * npc.knockBackResist;
                        }
                    }
                    // Expulsion sound
                    player.GetModPlayer<GravityPlayer>().SpawnExplosion(christmas);
                    if (utilityCounter > 0)
                    {
                        utilityBool = false;
                        Projectile.timeLeft = 185;
                        utilityCounter -= 1;
                    }
                }
            }
            else
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 4;
                }
                if (Projectile.scale < 50)
                {
                    Projectile.scale += 1.5f;
                }
                if (Projectile.timeLeft <= 5 && utilityCounter > 0)
                {
                    utilityBool = true;
                    Projectile.timeLeft = 305;
                    utilityCounter -= 1;
                    Projectile.Center = player.Center;
                    // Suck sound
                    SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/charge_3") with
                    {
                        Volume = 0.5f
                    }, Projectile.Center);
                }
            }
        }

        public void GravityField()
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is not FriendlyBullet || projectile == Projectile ||
                        projectile.ai[0] == (int)BulletAIs.SuperNova ||
                        projectile.ai[0] == (int)BulletAIs.DesperateSuperNova ||
                        projectile.ai[0] == (int)BulletAIs.SuperGift ||
                        projectile.ai[0] == (int)BulletAIs.DesperateSuperGift) continue;
                if (projectile.friendly == true &&
                    projectile.Distance(Projectile.Center) <= 200)
                {
                    if (Main.rand.NextBool()) projectile.timeLeft += 1;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, 
                        projectile.DirectionTo(Projectile.Center) * 14, 0.04f);
                    projectile.netUpdate = true;
                }
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.knockBackResist <= 0) continue;
                if (npc.Distance(Projectile.Center) <= 150)
                {
                    npc.velocity = Vector2.Lerp(npc.velocity,
                        npc.DirectionTo(Projectile.Center) * 10 * npc.knockBackResist, 0.05f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = Color.White;
            Color color2 = Color.White;

            switch (Projectile.ai[1])
            {
                case (int)BulletColors.Red:
                    color = Color.Red;
                    break;
                case (int)BulletColors.Magenta:
                    color = Color.Magenta;
                    break;
                case (int)BulletColors.DarkBlue:
                    color = Color.DarkBlue;
                    break;
                case (int)BulletColors.Cyan:
                    color = Color.Cyan;
                    break;
                case (int)BulletColors.Green:
                    color = Color.Green;
                    break;
                case (int)BulletColors.Yellow:
                    color = Color.Yellow;
                    break;
                case (int)BulletColors.Black:
                    color = Color.Black;
                    color2 = Color.Black;
                    break;
            }

            float alpha = 1f - ((float)Projectile.alpha/255f);

            Main.EntitySpriteDraw(new DrawData(coloredTexture,
                Projectile.Center - Main.screenPosition, //- (coloredTexture.Size() * Projectile.scale / 2),
                coloredTexture.Bounds,
                color * alpha,
                Projectile.rotation,
                coloredTexture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None)
            );

            Main.EntitySpriteDraw(new DrawData(colorlessTexture,
                Projectile.Center - Main.screenPosition, //- (colorlessTexture.Size() * Projectile.scale / 2),
                colorlessTexture.Bounds,
                color2 * alpha,
                Projectile.rotation,
                colorlessTexture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None)
            );

            return false;
        }
    }
}
