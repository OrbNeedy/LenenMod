using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using lenen.Common.Players;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace lenen
{
	public class lenen : Mod
    {
        /*public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> dyeShader = this.Assets.Request<Effect>("Effects/RiftEffect");
            }
        }*/

        internal enum MessageType : byte
        {
            PlayerUpdateCount
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                // This message syncs ExampleStatIncreasePlayer.exampleLifeFruits and ExampleStatIncreasePlayer.exampleManaCrystals
                case MessageType.PlayerUpdateCount:
                    byte playerNumber = reader.ReadByte();
                    OptionsManagingPlayer drawPlayer = Main.player[playerNumber].GetModPlayer<OptionsManagingPlayer>();
                    drawPlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        drawPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
            }
        }
    }
}
