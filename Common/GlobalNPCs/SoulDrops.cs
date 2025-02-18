using lenen.Common.Players;
using lenen.Common.Systems;
using lenen.Content.Buffs;
using lenen.Content.Items;
using lenen.Content.NPCs.Fairy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace lenen.Common.GlobalNPCs
{
    public class SoulDrops : GlobalNPC
    {
        public float harujionPotency = 0f;
        public int aproxSpirits = 0;
        public int volumeCalculated = 1;
        public int soulsCalculated = 1;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            harujionPotency = 0f;
        }

        public override void SaveData(NPC npc, TagCompound tag)
        {
            SoulDrops souls = npc.GetGlobalNPC<SoulDrops>();
            tag["SpiritAprox"] = souls.aproxSpirits;
            tag["Volume"] = souls.volumeCalculated;
            tag["Spirits"] = souls.soulsCalculated;
        }

        public override void LoadData(NPC npc, TagCompound tag)
        {
            SoulDrops souls = npc.GetGlobalNPC<SoulDrops>();
            if (tag.ContainsKey("SpiritAprox"))
            {
                souls.aproxSpirits = tag.GetInt("SpiritAprox");
            }

            if (tag.ContainsKey("Volume"))
            {
                souls.volumeCalculated = tag.GetInt("Volume");
            }

            if (tag.ContainsKey("Spirits"))
            {
                souls.soulsCalculated = tag.GetInt("Spirits");
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            CalculateSpirits(npc);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (harujionPotency > 0f)
            {
                npc.lifeRegen -= (int)(30*harujionPotency);
            }
        }

        public override void AI(NPC npc)
        {
            if (harujionPotency > 0) npc.AddBuff(ModContent.BuffType<HarujionAbsorb>(), 2);
        }

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (aproxSpirits <= 0 || !npc.active || 
                    SoulExceptions.instance.soulessNPCs.Contains(npc.type) ||
                    SoulExceptions.instance.undetectableNPCs.Contains(npc.type)) return;
                SoulAbsorptionPlayer manager = Main.LocalPlayer.GetModPlayer<SoulAbsorptionPlayer>();
                if (manager.seeSpirits)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(
                        "lenen/Assets/Textures/SpiritLock").Value;
                    Rectangle rect = new Rectangle((int)(npc.Center.X - texture.Width/2 - screenPos.X),
                        (int)(npc.Center.Y - texture.Height / 2 - screenPos.Y), texture.Width, texture.Height);
                    spriteBatch.Draw(
                        texture, 
                        rect,
                        Color.White * 0.588235f
                    );

                    spriteBatch.DrawString(FontAssets.MouseText.Value, 
                        Language.GetTextValue("Mods.lenen.UI.Spirits", aproxSpirits), 
                        npc.Center + new Vector2(0, -texture.Height * 0.7f) - screenPos, 
                        new Color(202, 159, 224));
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            if (npc.SpawnedFromStatue || npc.lifeMax <= 20 || 
                SoulExceptions.instance.soulessNPCs.Contains(npc.type)) return;

            Vector2 position = npc.position;
            for (int i = 0; i < soulsCalculated; i++)
            {
                Item.NewItem(npc.GetSource_DropAsItem(), new Rectangle((int)position.X, (int)position.Y,
                npc.width, npc.height), ModContent.ItemType<SoulItem>(), Main.rand.Next(1, 1 + volumeCalculated));
            }
        }

        private void CalculateSpirits(NPC npc)
        {
            aproxSpirits = 0;
            if (npc.SpawnedFromStatue) return;
            if (npc.lifeMax <= 20) return;

            int souls = 1;
            int volume = 1;
            souls += (5 * npc.rarity);
            volume *= (int)(1 + (npc.rarity * 0.475f));

            volume += npc.life / 800;

            souls += npc.defense / 15;

            if (Main.hardMode) volume *= 2;

            if (npc.boss) souls += Main.rand.Next(5, 16);

            if (NPC.downedPlantBoss) souls += (int)(2 * Main.rand.NextFloat(1f, 2f));

            if (npc.type == ModContent.NPCType<SmallFairy>())
            {
                souls += Main.rand.Next(5);
                volume += Main.rand.Next(1, 3);
            }

            volumeCalculated = volume;
            soulsCalculated = souls;

            for (int i = 0; i < souls; i++)
            {
                aproxSpirits += 1 + volume / 2;
            }
        }

        private void RecalculateSpirits(NPC npc)
        {
            aproxSpirits = 0;
            if (npc.SpawnedFromStatue) return;
            if (npc.lifeMax <= 20) return;

            int souls = soulsCalculated;
            int volume = volumeCalculated;

            for (int i = 0; i < souls; i++)
            {
                //npc.width, npc.height), ModContent.ItemType<SoulItem>(), Main.rand.Next(1, 1 + volume));
                aproxSpirits += 1 + volume / 2;
            }
        }
    }
}
