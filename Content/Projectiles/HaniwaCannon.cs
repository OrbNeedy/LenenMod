using lenen.Common.Players;
using lenen.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    public enum HaniwaMaterial
    {
        Clay, 
        Stone, 
        Ice, 
        Sandstone, 
        Crimstone, 
        Ebonstone, 
        Pearlstone
    }

    public class HaniwaCannon : ModProjectile
    {
        public bool FromCreationTools { get => Projectile.ai[0] != 0; }

        Asset<Texture2D> body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
        HaniwaMaterial material = HaniwaMaterial.Clay;

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
            string? sourceString = null;
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/haniwa_00") with
            {
                Volume = 0.35f
            }, Projectile.Center);

            if (player.ZoneSnow)
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/IceHaniwaFrame");
                material = HaniwaMaterial.Ice;
                sourceString = "IceHaniwa";
            }
            else if (player.ZoneRockLayerHeight)
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/StoneHaniwaFrame");
                material = HaniwaMaterial.Stone;
                sourceString = "StoneHaniwa";
            }
            else
            {
                body = ModContent.Request<Texture2D>("lenen/Assets/Textures/ClayHaniwaFrame");
                material = HaniwaMaterial.Clay;
                sourceString = "ClayHaniwa";
            }

            if (OwnerActive(Projectile.owner))
            {
                Player owner = Main.player[Projectile.owner];
                BuffPlayer buff = owner.GetModPlayer<BuffPlayer>();

                if (FromCreationTools)
                {
                    Projectile.minionSlots = buff.prevSummonSlots * 0.3f;
                }
                else
                {
                    Projectile.minionSlots = buff.prevSummonSlots;
                }
            }

            if (Main.myPlayer != Projectile.owner) return;

            int anchor = 1;

            if (FromCreationTools) anchor = (int)Projectile.ai[0];

            for (int i = -1; i < 2; i++)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(sourceString), Projectile.Center,
                    Vector2.Zero, ModContent.ProjectileType<Cannon>(), Projectile.damage,
                    Projectile.knockBack, Projectile.owner, Projectile.whoAmI, i * anchor, i + 1);
            }
        }

        /*The moon above is shining 
        Bathing everything in white
        It's there for you, it's there for me
        It's there for everyone in need
        
        When life beats you down
        Perhaps others left you behind
        The winter is too cold, or your mind way too loud
        Whatever you may need, it's already lost 
        
        Nothing will matter 
        Nothing ever has
        Just look up to the sky
        What a beautiful moon today*/

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public bool OwnerActive(Player owner)
        {
            //Main.NewText("Owner check");
            bool cannonBuff = owner.HasBuff(ModContent.BuffType<HaniwaCannonBuff>());
            bool commanderBuff = owner.HasBuff(ModContent.BuffType<HaniwaCommander>());
            bool buffCheck = (!FromCreationTools && cannonBuff) || (FromCreationTools && commanderBuff);

            return owner.active && owner.statLife > 0 && !owner.DeadOrGhost &&
                buffCheck;
        }

        public bool OwnerActive(int who)
        {
            //Main.NewText("Who: " + who);
            if (who == -1) return false;

            Player owner = Main.player[who];

            //Main.NewText("Check: " + (owner.active && owner.statLife > 0 && !owner.DeadOrGhost));
            return OwnerActive(owner);
        }

        public override void AI()
        {
            if (!OwnerActive(Projectile.owner))
            {
                Projectile.timeLeft = 0;
                return;
            }

            Projectile.timeLeft = 2;

            Player owner = Main.player[Projectile.owner];
            BuffPlayer buff = owner.GetModPlayer<BuffPlayer>();
            Vector2 offset = new Vector2(288 * Projectile.ai[0], -360);

            Projectile.Center = owner.Center + offset;

            if (FromCreationTools)
            {
                Projectile.minionSlots = buff.prevSummonSlots * 0.3f;
            } else
            {
                Projectile.minionSlots = buff.prevSummonSlots;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float increase = 10;

            if (FromCreationTools)
            {
                increase = 45;
            }

            modifiers.SourceDamage.Base += Projectile.minionSlots * increase;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            float increase = 10;

            if (FromCreationTools)
            {
                increase = 45;
            }

            modifiers.SourceDamage.Base += Projectile.minionSlots * increase;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(body.Value,
                Projectile.Center - Main.screenPosition,
                body.Value.Bounds,
                Color.White,
                Projectile.rotation,
                body.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );

            return base.PreDraw(ref lightColor);
        }
    }
}
