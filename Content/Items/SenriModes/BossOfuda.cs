using lenen.Common;
using lenen.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace lenen.Content.Items.SenriModes
{
    class BossOfuda : ModItem
    {
        public OfudaModes mode = OfudaModes.Default;
        Dictionary<int, OfudaModes> npcTypeKey = new();

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Master;
            Item.maxStack = Item.CommonMaxStack;

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;

            npcTypeKey = new() {
                [NPCID.KingSlime] = OfudaModes.KingSlime,
                [NPCID.EyeofCthulhu] = OfudaModes.EyeOfCthulhu,
                [NPCID.EaterofWorldsHead] = OfudaModes.EaterOfWorlds,
                [NPCID.EaterofWorldsBody] = OfudaModes.EaterOfWorlds,
                [NPCID.EaterofWorldsTail] = OfudaModes.EaterOfWorlds,
                [NPCID.BrainofCthulhu] = OfudaModes.BrainOfCthulhu,
                [NPCID.QueenBee] = OfudaModes.QueenBee,
                [NPCID.SkeletronHead] = OfudaModes.Skeletron,
                [NPCID.DungeonGuardian] = OfudaModes.Skeletron,
                [NPCID.Deerclops] = OfudaModes.Deerclops,
                [NPCID.WallofFlesh] = OfudaModes.WallOfFlesh,
                [NPCID.WallofFleshEye] = OfudaModes.WallOfFlesh,
                [NPCID.QueenSlimeBoss] = OfudaModes.QueenSlime,
                [NPCID.Retinazer] = OfudaModes.Retinazer,
                [NPCID.Spazmatism] = OfudaModes.Spazmatism,
                [NPCID.TheDestroyer] = OfudaModes.TheDestroyer,
                [NPCID.TheDestroyerBody] = OfudaModes.TheDestroyer,
                [NPCID.TheDestroyerTail] = OfudaModes.TheDestroyer,
                [NPCID.SkeletronPrime] = OfudaModes.SkeletronPrime,
                [NPCID.Plantera] = OfudaModes.Plantera,
                [NPCID.Golem] = OfudaModes.Golem,
                [NPCID.DukeFishron] = OfudaModes.DukeFishron,
                [NPCID.HallowBoss] = OfudaModes.EmpressOfLight,
                [NPCID.CultistBoss] = OfudaModes.LunaticCultist,
                [NPCID.MoonLordCore] = OfudaModes.MoonlordsCore
            };
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Mode"] = (int)mode;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Mode"))
            {
                mode = (OfudaModes)tag.GetInt("Mode");
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)mode);
        }

        public override void NetReceive(BinaryReader reader)
        {
            mode = (OfudaModes)reader.Read();
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Main.NewText("Source: " + source);
            //Main.NewText("Context: " + source.Context);

            try
            {
                if (source is EntitySource_Loot lootSource)
                {
                    //Main.NewText("Thing: " + lootSource.Entity);
                    if (lootSource.Entity is NPC npc && npc.boss)
                    {
                        // TODO: Replace name for NPC type
                        if (npcTypeKey.ContainsKey(npc.type))
                        {
                            mode = npcTypeKey[npc.type];
                        } else
                        {
                            mode = OfudaModes.Default;
                        }
                        /*string newName = npc.TypeName.ToLower().Replace(" ", "").Replace("'", "");
                        //Main.NewText("Boss: " + npc.TypeName);
                        //Main.NewText("Modified Name: " + newName);
                        int index = -1;

                        for (int i = 0; i < SenriPriestHeadpiece._availableModes.Length; i++)
                        {
                            OfudaModes md = SenriPriestHeadpiece._availableModes[i];
                            if (md.ToString().ToLower() == newName)
                            {
                                index = i;
                                break;
                            }
                        }

                        if (index != -1)
                        {
                            mode = SenriPriestHeadpiece._availableModes[index];
                        } else
                        {
                            mode = OfudaModes.Default;
                        }*/
                    }
                }
            } catch
            {
                mode = OfudaModes.Default;
            }
        }

        public override bool? UseItem(Player player)
        {
            //Main.NewText("Mode: " + mode);
            player.GetModPlayer<SenriPlayer>().currentMode = mode;
            return base.UseItem(player);
        }

        public override bool CanStack(Item source)
        {
            if (source.ModItem is BossOfuda ofuda)
            {
                return ofuda.mode == mode;
            }
            return base.CanStack(source);
        }

        public override bool CanStackInWorld(Item source)
        {
            if (source.ModItem is BossOfuda ofuda)
            {
                return ofuda.mode == mode;
            }
            return base.CanStackInWorld(source);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string description = Language.GetTextValue($"Mods.lenen.SenriCopy.{mode.ToString()}.Description");
            string name = Language.GetTextValue($"Mods.lenen.SenriCopy.{mode.ToString()}.OfudaName");

            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "StateDescriptor", description));
            }

            index = tooltips.FindIndex((x) => x.Name.StartsWith("ItemName") && x.Mod == "Terraria");

            if (index != -1)
            {
                tooltips[index].Text = name;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D ofuda = ModContent.Request<Texture2D>($"lenen/Content/Items/SenriModes/Default").Value;
            try
            {
                ofuda = ModContent.Request<Texture2D>($"lenen/Content/Items/SenriModes/{mode.ToString()}").Value;
            } catch
            {
                ofuda = ModContent.Request<Texture2D>($"lenen/Content/Items/SenriModes/Default").Value;
            }

            spriteBatch.Draw(
                ofuda,
                position,
                frame,
                drawColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D ofuda = ModContent.Request<Texture2D>($"lenen/Content/Items/SenriModes/Default").Value;
            try
            {
                ofuda = ModContent.Request<Texture2D>($"lenen/Content/Items/SenriModes/{mode.ToString()}").Value;
            }
            catch
            {
                ofuda = ModContent.Request<Texture2D>($"lenen/Content/Items/SenriModes/Default").Value;
            }

            spriteBatch.Draw(
                ofuda,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - ofuda.Height * 0.5f
                ),
                new Rectangle(0, 0, ofuda.Width, ofuda.Height),
                Lighting.GetColor(Item.Center.ToTileCoordinates(), Color.White),
                rotation,
                ofuda.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddCondition(LenenConditions.HasSenri)
                .Register();
        }
    }
}
