using lenen.Content.Items.Weapons;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class OptionsManagingPlayer : ModPlayer
    {
        public Dictionary<int, int> BirdDrone = new Dictionary<int, int>{
            [0] = -1,
            [1] = -1,
            [2] = -1,
            [3] = -1
        };

        public Dictionary<int, int> HaniwaFists = new Dictionary<int, int>
        {
            [0] = -1,
            [1] = -1
        };
        public bool fistShootState = false;

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
            OptionsManagingPlayer clone = (OptionsManagingPlayer)targetCopy;
            clone.UpdateCount = UpdateCount;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            OptionsManagingPlayer clone = (OptionsManagingPlayer)clientPlayer;
            if (UpdateCount != clone.UpdateCount)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }

        public override void PostUpdate()
        {
            // The following only applies if the player is holding a certain item
            int selectedItem = Player.inventory[Player.selectedItem].type;
            CheckDrone(selectedItem);
            CheckHaniwaFists(selectedItem);
            UpdateCount++;
            if (UpdateCount >= int.MaxValue)
            {
                UpdateCount = 0;
            }
        }

        private void CheckDrone(int selectedItem)
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
                        drone.timeLeft = 2;
                    }
                    // If it doesn't exist, create one and save it to the respective position in the dictionary
                    else
                    {
                        SpawnDrone(index);
                    }
                }
            }
            else
            {
                BirdDrone[0] = -1;
                BirdDrone[1] = -1;
                BirdDrone[2] = -1;
                BirdDrone[3] = -1;
            }
        }

        private void CheckHaniwaFists(int selectedItem)
        {
            if (selectedItem == ModContent.ItemType<ExtendedGrab>())
            {
                foreach (var index in HaniwaFists.Keys)
                {
                    if (HaniwaFists[index] <= -1)
                    {
                        SpawnFist(index);
                        continue;
                    }
                    Projectile fist = Main.projectile[HaniwaFists[index]];
                    bool fistExists = fist.active && fist.owner == Player.whoAmI &&
                        fist.ModProjectile is HaniwaFist;
                    // If there is already a drone, set it's timeLeft to 2 to keep it from despawning
                    if (fistExists)
                    {
                        fist.timeLeft = 3;
                    }
                    // If it doesn't exist, create one and save it to the respective position in the dictionary
                    else
                    {
                        SpawnFist(index);
                    }
                }
            }
            else
            {
                HaniwaFists[0] = -1;
                HaniwaFists[1] = -1;
            }
        }

        private void SpawnDrone(int droneIndex)
        {
            BirdDrone[droneIndex] = Projectile.NewProjectile(Player.GetSource_FromThis(),
                Player.Center, Vector2.Zero, ModContent.ProjectileType<BirdOption>(), 0, 0,
                Player.whoAmI, droneIndex);
        }

        private void SpawnFist(int fistIndex, bool shooting = false)
        {
            if (shooting)
            {
                HaniwaFists[fistIndex] = Projectile.NewProjectile(Player.GetSource_FromThis(),
                    Player.Center, Vector2.Zero, ModContent.ProjectileType<HaniwaFist>(), 0, 10,
                    Player.whoAmI, fistIndex, 1);
            } else
            {
                HaniwaFists[fistIndex] = Projectile.NewProjectile(Player.GetSource_FromThis(),
                    Player.Center, Vector2.Zero, ModContent.ProjectileType<HaniwaFist>(), 0, 10,
                    Player.whoAmI, fistIndex);
            }
        }

        public void ShootFists(int altUse)
        {
            foreach (var index in HaniwaFists.Keys)
            {
                if (HaniwaFists[index] <= -1)
                {
                    SpawnFist(index, true);
                    continue;
                }
                Projectile fist = Main.projectile[HaniwaFists[index]];
                bool fistExists = fist.active && fist.owner == Player.whoAmI;
                if (fistExists && fist.ModProjectile is HaniwaFist verifiedFist)
                {
                    verifiedFist.shooting = true;
                    verifiedFist.safeReturnTimer = 0;
                    fist.ai[2] = altUse;
                } else
                {
                    SpawnFist(index, true);
                }
            }
        }
    }
}
