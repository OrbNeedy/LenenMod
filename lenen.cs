using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using lenen.Common.Players;

namespace lenen
{
	public class lenen : Mod
    {
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
                    OptionsDrawingPlayer drawPlayer = Main.player[playerNumber].GetModPlayer<OptionsDrawingPlayer>();
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
