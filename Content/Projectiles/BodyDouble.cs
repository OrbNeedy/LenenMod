using lenen.Common.Graphics;
using lenen.Common.Players;
using lenen.Common.Systems;
using lenen.Content.Items.Vanity.Dyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace lenen.Content.Projectiles
{
    class BodyDouble : ModProjectile
    {
        public int ShadowNumber { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public Player clonedPlayer = new();

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 56;
            Projectile.scale = 1f;
            Projectile.light = 0.5f;
            Projectile.Opacity = 0.5f;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 0;
            Projectile.knockBack = 0;
            Projectile.penetrate = 6;
            Projectile.ArmorPenetration = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
        }

        public override string Texture => "lenen/Assets/Textures/Empty";

        public bool OwnerActive(Player owner)
        {
            return owner.active && owner.statLife > 0 && !owner.DeadOrGhost;
        }

        public bool OwnerActive(int who)
        {
            //Main.NewText("Who: " + who);
            if (who == -1) return false;

            Player owner = Main.player[who];

            //Main.NewText("Check: " + (owner.active && owner.statLife > 0 && !owner.DeadOrGhost));
            return owner.active && owner.statLife > 0 && !owner.DeadOrGhost;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (OwnerActive(Projectile.owner))
            {
                //clonedPlayer = (Player)Main.player[Projectile.owner].Clone();
                clonedPlayer.CopyVisuals(Main.player[Projectile.owner]);
                int dyeType = ModContent.ItemType<BodyDoubleDye>();
                for (int i = 0; i < clonedPlayer.dye.Length; i++)
                {
                    Item dye = new Item(dyeType);
                    clonedPlayer.dye[i] = dye;
                }
                clonedPlayer.UpdateDyes();
            }
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (OwnerActive(Projectile.owner))
                {
                    Player owner = Main.player[Main.myPlayer];
                    Projectile.Center = owner.GetModPlayer<AfterimageEffectPlayer>().
                        GetPosition(ShadowNumber);
                    Projectile.velocity = owner.GetModPlayer<AfterimageEffectPlayer>().
                        GetVelocity(ShadowNumber);

                    // Set to player direction if horizontal speed is 0 so standing players don't always 
                    // cast right-facing shadows
                    if (Projectile.Center.Distance(owner.Center) <= 1)
                    {
                        Projectile.direction = owner.direction;
                    }

                    Projectile.netUpdate = true;
                }
            }
            if (OwnerActive(Projectile.owner))
            {
                //clonedPlayer = (Player)Main.player[Projectile.owner].Clone();

                clonedPlayer.itemAnimation = Main.player[Projectile.owner].itemAnimation;
                clonedPlayer.itemAnimationMax = Main.player[Projectile.owner].itemAnimationMax;
            }

            clonedPlayer.Center = Projectile.Center;
            clonedPlayer.velocity = Projectile.velocity;
            clonedPlayer.direction = Projectile.direction;

            clonedPlayer.PlayerFrame();
            clonedPlayer.WingFrame(clonedPlayer.velocity.Y != 0);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!RenderTarget.canUseShadow) return false;

            SpriteBatchState tempState = SpriteBatchExt.GetState(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState, SpriteSortMode.Immediate);

            Rectangle playerRect = RenderTarget.
                GetShadowRect(Projectile.owner, ShadowNumber);
            /*
                int playerSeparation = HaniwaIndexLookup[index] * 7;
                return new Rectangle(sheetSquareX * (playerSeparation + shadow), 0,
                    sheetSquareX, sheetSquareY);*/
            int playerSeparation = Projectile.owner * 7;

            Rectangle sourceRectangle = new Rectangle((ShadowNumber + playerSeparation) * 
                playerRect.Width, 0, playerRect.Width, playerRect.Height);

            MiscShaderData shader = GameShaders.Misc["SolidColor"];
            
            Vector2 position = Projectile.Center - Main.screenPosition - playerRect.Size() / 2f;

            DrawData data = new(
                RenderTarget.shadowsRenderTarget,
                position - (clonedPlayer.Size / 2f),
                playerRect,
                Color.White * 0.5f);

            shader.Apply(data);

            data.Draw(Main.spriteBatch);

            SpriteBatchExt.Restart(Main.spriteBatch, tempState);

            return false;
        }
    }
}
