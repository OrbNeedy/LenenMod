using lenen.Content.Items.Vanity.Dyes;
using lenen.Content.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace lenen.Common.Players
{
    public class AfterimageEffectPlayer : ModPlayer
    {
        public Vector2[] positionStorage = new Vector2[] {
            Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero,
            Vector2.Zero, Vector2.Zero, Vector2.Zero
        };
        public Vector2[] velocityStorage = new Vector2[] {
            Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero,
            Vector2.Zero, Vector2.Zero, Vector2.Zero
        };
        public bool useAfterimages = false;
        public bool oldAfterimageUse = false;
        public float positionAdjustment = 0.08f;
        public int shadowCooldown = 0;
        public int[] shadowIndex = new int[] { 
            -1, -1, -1, -1, 
            -1, -1, -1
        };

        public bool useShader = false;

        public override void SetStaticDefaults()
        {
            positionStorage = new Vector2[] { 
                Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, 
                Vector2.Zero, Vector2.Zero, Vector2.Zero 
            };
            velocityStorage = new Vector2[] {
                Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero,
                Vector2.Zero, Vector2.Zero, Vector2.Zero
            };
        }

        public override void OnEnterWorld()
        {
            positionStorage = new Vector2[] {
                Player.Center, Player.Center, Player.Center, Player.Center,
                Player.Center, Player.Center, Player.Center
            };
            velocityStorage = new Vector2[] {
                Player.velocity, Player.velocity, Player.velocity, Player.velocity,
                Player.velocity, Player.velocity, Player.velocity
            };
        }

        public override void PostUpdate()
        {
            if (useAfterimages)
            {
                IEntitySource source = Player.GetSource_ItemUse(Player.HeldItem);
                int shadowType = ModContent.ProjectileType<BodyDouble>();
                int shadowDamage = (int)(Player.GetWeaponDamage(Player.HeldItem) * 2.5f);
                // Reset positions if previously afterimages weren't used
                if (!oldAfterimageUse)
                {
                    positionStorage = new Vector2[] {
                        Player.Center, Player.Center, Player.Center, Player.Center,
                        Player.Center, Player.Center, Player.Center
                    };
                    velocityStorage = new Vector2[] {
                        Player.velocity, Player.velocity, Player.velocity, Player.velocity,
                        Player.velocity, Player.velocity, Player.velocity
                    };

                    for (int i = 0; i < positionStorage.Length; i++)
                    {
                        shadowIndex[i] = Projectile.NewProjectile(source, positionStorage[i],
                            velocityStorage[i], shadowType, shadowDamage, 0, Player.whoAmI, i
                            );
                    }
                }

                // The first afterimage will always follow the player
                if (positionStorage[0] != Player.Center)
                {
                    velocityStorage[0] = positionStorage[0].DirectionTo(Player.Center);
                    positionStorage[0] = Vector2.Lerp(positionStorage[0], Player.Center, 
                        positionAdjustment);

                    if (positionStorage[0].Distance(Player.Center) <= 1)
                    {
                        positionStorage[0] = Player.Center;
                        velocityStorage[0] = Player.velocity;
                    }
                } 
                else
                {
                    velocityStorage[0] = Player.velocity;
                }

                CheckShadow(0, source, shadowType, shadowDamage);

                // Make every afterimage get closer to the next slowly 
                for (int i = 1; i < positionStorage.Length; i++)
                {
                    if (positionStorage[i] != positionStorage[i - 1])
                    {
                        velocityStorage[i] = positionStorage[i].DirectionTo(positionStorage[i - 1]);
                        positionStorage[i] = Vector2.Lerp(positionStorage[i], positionStorage[i - 1],
                            positionAdjustment);

                        if (positionStorage[i].Distance(Player.Center) <= 1)
                        {
                            positionStorage[i] = Player.Center;
                            velocityStorage[i] = Player.velocity;
                        }
                    }
                    else
                    {
                        velocityStorage[i] = velocityStorage[i - 1];
                    }

                    CheckShadow(i, source, shadowType, shadowDamage);
                }
            } else
            {
                shadowIndex = new int[] {
                    -1, -1, -1, -1,
                    -1, -1, -1
                };
            }
        }

        public void CheckShadow(int i, IEntitySource source, int shadowType, int shadowDamage)
        {
            int maxCooldown = 60;
            if (shadowIndex[i] < 0)
            {
                if (shadowCooldown <= 0)
                {
                    shadowIndex[i] = Projectile.NewProjectile(source, positionStorage[i],
                        velocityStorage[i], shadowType, shadowDamage, 0, Player.whoAmI, i
                        );

                    shadowCooldown = maxCooldown;
                } 
                else
                {
                    shadowIndex[i] = -1;
                }

            } else
            {
                Projectile proj = Main.projectile[shadowIndex[i]];
                if (!proj.active || proj.owner != Player.whoAmI || proj.type != shadowType)
                {
                    if (shadowCooldown <= 0)
                    {
                        shadowIndex[i] = Projectile.NewProjectile(source, positionStorage[i],
                            velocityStorage[i], shadowType, shadowDamage, 0, Player.whoAmI, i
                            );

                        shadowCooldown = maxCooldown;
                    }
                    else
                    {
                        shadowIndex[i] = -1;
                    }

                } else
                {
                    proj.timeLeft = 2;
                    proj.netUpdate = true;
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (useShader)
            {
                // Main.NewText("Modifying draw info");
                int shader = GameShaders.Armor.GetShaderIdFromItemId(
                    ModContent.ItemType<BodyDoubleDye>());
                List<DrawData> newDrawDataCache = new();
                for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
                {
                    DrawData data = drawInfo.DrawDataCache[i];
                    newDrawDataCache.Add(data with { shader = shader });
                }
                drawInfo = drawInfo with { DrawDataCache = newDrawDataCache };

                /*drawInfo.cBody = shader;
                drawInfo.cHead = shader;
                drawInfo.cLegs = shader;
                drawInfo.cWings = shader;*/
            }
        }

        public override void ResetEffects()
        {
            oldAfterimageUse = useAfterimages;
            useAfterimages = false;
             if (shadowCooldown > 0) shadowCooldown--;
        }

        public Vector2 GetPosition(int n)
        {
            if (n < 0 || n >= positionStorage.Length)
            {
                return Player.Center;
            }
            return positionStorage[n];
        }

        public Vector2 GetVelocity(int n)
        {
            if (n < 0 || n >= velocityStorage.Length)
            {
                return Player.velocity;
            }
            return velocityStorage[n];
        }
    }
}
