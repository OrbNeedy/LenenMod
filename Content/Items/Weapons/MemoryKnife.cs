using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.Localization;

namespace lenen.Content.Items.Weapons
{
    public class MemoryKnife : ModItem
    {
        public int state = 0;
        public string stateDescription = "";

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.shoot = ModContent.ProjectileType<KnifeProjectile>();
            Item.shootSpeed = 8f;
            Item.knockBack = 4f;
            Item.DamageType = ModContent.GetInstance<MeleeRangedHybrid>();

            Item.width = 30;
            Item.height = 50;
            Item.value = Item.sellPrice(0, 0, 0, 40);
            Item.rare = ItemRarityID.LightPurple;

            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override string Texture => $"lenen/Assets/Textures/MemoryKnife_{state}";

        public override bool AllowPrefix(int pre)
        {
            return base.AllowPrefix(pre);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            switch (state)
            {
                case 0:
                    stateDescription = Language.GetTextValue("Mods.lenen.MemoryDescriptions.Trace");
                    break;
                case 1:
                    stateDescription = Language.GetTextValue("Mods.lenen.MemoryDescriptions.Reproduction");
                    break;
                case 2:
                    stateDescription = Language.GetTextValue("Mods.lenen.MemoryDescriptions.Override");
                    break;
            }
            int index = -1;
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Name == "Tooltip1")
                {
                    index = i;
                    break;
                }
            }
            tooltips.Insert(index + 1, new TooltipLine(Mod, "MemoryDescriptions", stateDescription));
            base.ModifyTooltips(tooltips);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            switch (state)
            {
                case 0:
                    damage.Base += 15;
                    break;
                case 1:
                    //damage.Base = 55;
                    break;
                case 2:
                    damage.Base -= 10;
                    break;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }
            Vector2 offset = new Vector2(0).RotatedBy(player.MountedCenter
                .DirectionTo(Main.MouseWorld).ToRotation());
            for (int i = -1; i < 2; i++)
            {
                Vector2 vel = velocity.RotatedBy(MathHelper.PiOver2*i/5);
                Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter + offset, vel, type,
                damage, knockback, player.whoAmI, state);
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            state++;
            if (state >= 3)
            {
                state = 0;
            }
            SoundEngine.PlaySound(new SoundStyle("lenen/Assets/Sounds/warning"), player.Center);
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D knife = ModContent.Request<Texture2D>("lenen/Assets/Textures/MemoryKnife_0").Value;
            if (state == 0 || state == 1 || state == 2)
            {
                knife = ModContent.Request<Texture2D>("lenen/Assets/Textures/MemoryKnife_" + state).Value;
            }

            spriteBatch.Draw(
                knife,
                position,
                frame,
                drawColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D knife = ModContent.Request<Texture2D>("lenen/Assets/Textures/MemoryKnife_" + state).Value;
            spriteBatch.Draw(
                knife,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - knife.Height * 0.5f
                ),
                new Rectangle(0, 0, knife.Width, knife.Height),
                Lighting.GetColor(Item.Center.ToTileCoordinates(), Color.White),
                rotation,
                knife.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
