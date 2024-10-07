using lenen.Content.Items.Weapons;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class OptionsDrawingPlayer : ModPlayer
    {
        public Dictionary<int, int> BirdDrone = new Dictionary<int, int>{
            [0] = -1,
            [1] = -1,
            [2] = -1,
            [3] = -1
        };

        public int UpdateCount = 0;

        public override void OnEnterWorld()
        {
            UpdateCount = 0;
        }

        public void ReceivePlayerSync(BinaryReader reader)
        {
            UpdateCount = reader.ReadByte();
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write(UpdateCount);
            packet.Send(toWho, fromWho);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            OptionsDrawingPlayer clone = (OptionsDrawingPlayer)targetCopy;
            clone.UpdateCount = UpdateCount;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            OptionsDrawingPlayer clone = (OptionsDrawingPlayer)clientPlayer;
            if (UpdateCount != clone.UpdateCount)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }

        public override void PostUpdate()
        {
            // The following only applies if the player is holding a certain item
            int selectedItem = Player.inventory[Player.selectedItem].type;
            if (Player.HasItem(ModContent.ItemType<TrueBirdDrone>()))
            {
                if (selectedItem == ModContent.ItemType<TrueBirdDrone>())
                {
                    // If said item is of TrueBirdDrone type, check for existing BirdDrone projectiles with the given index
                    foreach (var index in BirdDrone.Keys)
                    {
                        if (BirdDrone[index] <= -1)
                        {
                            SpawnDrone(index);
                            continue;
                        }
                        Projectile drone = Main.projectile[BirdDrone[index]];
                        bool droneExists = drone.active && drone.owner == Player.whoAmI && 
                            drone.ModProjectile is BirdOption;
                        // If there is already a drone, set it's timeLeft to 2 to keep it from despawning
                        if (droneExists)
                        {
                            drone.timeLeft = 3;
                        }
                        // If it doesn't exist, create one and save it to the respective position in the dictionary
                        else
                        {
                            SpawnDrone(index);
                        }
                    }
                } else
                {
                    BirdDrone[0] = -1;
                    BirdDrone[1] = -1;
                    BirdDrone[2] = -1;
                    BirdDrone[3] = -1;
                }
            } else
            {
                BirdDrone[0] = -1;
                BirdDrone[1] = -1;
                BirdDrone[2] = -1;
                BirdDrone[3] = -1;
            }
            UpdateCount++;
            if (UpdateCount >= int.MaxValue)
            {
                UpdateCount = 0;
            }
        }

        private void SpawnDrone(int droneIndex)
        {
            BirdDrone[droneIndex] = Projectile.NewProjectile(Player.GetSource_FromThis(),
                Player.Center, Vector2.Zero, ModContent.ProjectileType<BirdOption>(), 0, 0,
                Player.whoAmI, droneIndex);
        }
    }
}
