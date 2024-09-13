using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using lenen.Content.NPCs;
using Terraria.Utilities;
using System;

namespace lenen.Content.Items
{
    public class DimensionalFracture : ModItem
    {
        private int timeSinceSpawn;
        
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = 0;
            Item.rare = ItemRarityID.Master;

            Item.makeNPC = ModContent.NPCType<CurtainOfAwakening>(); 
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            timeSinceSpawn = 0;
            base.OnSpawn(source);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = Vector2.One.RotateRandom(MathHelper.TwoPi) * new Random().Next(-2, 2);
                Dust.NewDust(Item.position, 32, 30, DustID.ShimmerSpark, vel.X, 
                    vel.Y, newColor: new(10, 255, 60));
            }
            if (timeSinceSpawn >= 120)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 vel = Vector2.One.RotateRandom(MathHelper.TwoPi) * new Random().Next(-5, 5);
                    Dust.NewDust(Item.position, 32, 30, DustID.ShimmerSpark, vel.X,
                        vel.Y, newColor: new(10, 255, 60));
                }
                NPC.NewNPC(Item.GetSource_FromThis(), (int)Item.position.X + 9, 
                    (int)Item.position.Y + 20, ModContent.NPCType<CurtainOfAwakening>());
                Item.active = false;
            }
            timeSinceSpawn += 1;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.8f * Main.essScale);
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override bool CanPickup(Player player)
        {
            return false;
        }
    }
}
