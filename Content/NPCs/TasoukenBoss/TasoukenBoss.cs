using lenen.Content.BossBars;
using lenen.Content.Items.Weapons.Yaorochi;
using lenen.Content.Projectiles.TasoukenProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.NPCs.TasoukenBoss
{
    public enum TasoukenPhases
    {
        MultipleCuts, 
        CircleCut, 
        BackgroundCut, 
        GridCut, 
        ThrillingCut, 
        SingleGlimmer, 
        None
    }

    [AutoloadBossHead]
    public class TasoukenBoss : ModNPC
    {
        public static Asset<Texture2D> BossBorder;
        public static int maxDistance = 1200;

        public int OwnerIndex { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public TasoukenPhases CurrentPhase { get => (TasoukenPhases)NPC.ai[1]; set => NPC.ai[1] = (int)value; }
        public int PhaseTimer { get => (int)NPC.localAI[0]; set => NPC.localAI[0] = value; }
        public int PhaseCounter = 0;
        public int PhaseState = 0;

        public bool ownerFailed = false;
        public string OwnerName = "";
        public int cutCount = 0;

        public float dangerZone = 0;

        public override void Load()
        {
            BossBorder = ModContent.Request<Texture2D>(
                "lenen/Assets/Textures/BossBorder");
        }

        public override void Unload()
        {
            BossBorder = null;
        }

        public override void SetStaticDefaults()
        {
            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPCID.Sets.TeleportationImmune[Type] = true;
            // This boss also becomes immune to OnFire and all buffs that inherit OnFire immunity during the second half of the fight. See the ApplySecondStageBuffImmunities method.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "lenen/Content/Items/Weapons/Yaorochi/Tasouken",
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
                Rotation = MathHelper.Pi
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 88;

            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 12780;
            NPC.statsAreScaledForThisManyPlayers = 0;

            NPC.DeathSound = SoundID.NPCDeath1; // Add custom sound
            NPC.knockBackResist = 0f;
            
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 20f; 

            NPC.aiStyle = -1;

            NPC.BossBar = ModContent.GetInstance<TasoukenBossBar>();
            NPC.dontTakeDamage = true;

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                // Get custom track
                Music = MusicID.Boss5; // MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ropocalypse2");

                // If you would like to play alternate music when the otherworld soundtrack enabled, use this logic.
                if (!Main.swapMusic == Main.drunkWorld && !Main.remixWorld)
                {
                    Music = MusicID.OtherworldlyBoss2;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            // Only one player can fight against Tasouken at the time, so it will not scale 
            NPC.lifeMax = 12780;

            NPC.damage = 0;
        }

        // "PlayerName has proven their power."
        public override LocalizedText DeathMessage => 
            ownerFailed ? Language.GetText("Mods.lenen.Announcement.TasoukenVictory").WithFormatArgs(OwnerName) : 
            Language.GetText("Mods.lenen.Announcement.TasoukenDefeated").WithFormatArgs(OwnerName);

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_BossSpawn bossSource)
            {
                if (bossSource.Target is Player owner)
                {
                    OwnerIndex = owner.whoAmI;
                    OwnerName = owner.name;
                    NPC.Center = new Vector2(owner.Center.X, owner.Center.Y - 192);
                    return;
                }
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.whoAmI == OwnerIndex)
                {
                    OwnerName = player.name;
                    NPC.Center = new Vector2(player.Center.X, player.Center.Y - 192);
                }
            }
        }

        public bool CheckOwner(int who)
        {
            if (who == -1) return false;

            Player owner = Main.player[who];

            return owner.active && owner.statLife > 0 && !owner.DeadOrGhost;
        }

        /// <summary>
        /// Returns current world difficulty.
        /// </summary>
        /// <returns>0 for normal mode, 1 for expert mode, 2 for master mode, 3 for special seeds.</returns>
        public static int GetDifficulty()
        {
            int difficulty = 0;
            if (Main.expertMode)
            {
                difficulty = 1;
            }

            if (Main.masterMode)
            {
                difficulty = 2;
            }

            if (Main.getGoodWorld || Main.drunkWorld)
            {
                difficulty = 3;
            }

            return difficulty;
        }

        public override void AI()
        {
            // Main.NewText(NPC.whoAmI);
            NPC.damage = 0;
            cutCount++;
            Lighting.AddLight(NPC.Center, new Vector3(1));
            //Main.NewText("ID: " + NPC.whoAmI);

            NPC.DiscourageDespawn(600);

            // Despawn sequence
            if (NPC.life <= 1)
            {
                NPC.Opacity -= 1f / 180f;

                if (NPC.Opacity <= 0)
                {
                    NPC.dontTakeDamage = false;
                    NPC.StrikeNPC(new NPC.HitInfo()
                    {
                        InstantKill = true
                    });
                    NetMessage.SendStrikeNPC(NPC, new NPC.HitInfo()
                    {
                        InstantKill = true
                    });
                } else
                {
                    NPC.life = 1;

                    Color col = Main._rand.Next(
                        [Color.LightGoldenrodYellow, Color.PaleVioletRed, Color.DarkViolet]);
                    int index = Dust.NewDust(NPC.Center, 0, 0, DustID.TintableDustLighted, 
                        Main._rand.NextFloat(-10, 10), Main._rand.NextFloat(-10, 10), newColor: col);

                    Main.dust[index].noGravity = true;
                }
                return;
            }

            // If owner died, despawn and set owner failed to true
            if (!CheckOwner(OwnerIndex))
            {
                ownerFailed = true;
                NPC.life = 1;
                return;
            }

            Player owner = Main.player[OwnerIndex];

            // Limit arena 
            RestrictAccess();

            int phaseLife = 2304;
            TasoukenPhases next = TasoukenPhases.MultipleCuts;

            // Get difficulty and use attack
            int difficulty = GetDifficulty();

            switch (CurrentPhase)
            {
                case TasoukenPhases.MultipleCuts:
                    MultipleCuts(owner, difficulty);
                    next = TasoukenPhases.CircleCut;
                    break;
                case TasoukenPhases.CircleCut:
                    CircleCut(owner, difficulty);
                    next = TasoukenPhases.BackgroundCut;
                    break;
                case TasoukenPhases.BackgroundCut:
                    BackgroundCut(owner, difficulty);
                    next = TasoukenPhases.GridCut;
                    break;
                case TasoukenPhases.GridCut:
                    GridCut(owner, difficulty);
                    next = TasoukenPhases.ThrillingCut;
                    break;
                case TasoukenPhases.ThrillingCut:
                    ThrillingCut(owner, difficulty);
                    next = TasoukenPhases.SingleGlimmer;
                    break;
                case TasoukenPhases.SingleGlimmer:
                    SingleGlimmer(owner, difficulty);
                    next = TasoukenPhases.None;
                    break;
                case TasoukenPhases.None:
                    NPC.life = 1;
                    break;
            }

            if (PhaseTimer > phaseLife)
            {
                PhaseTimer = 0;
                PhaseCounter = -60;
                PhaseState = 0;
                dangerZone = 0;
                CurrentPhase = next;
            }

            PhaseTimer++;

            if (NPC.life > 1) NPC.life--;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false;
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            return false;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }
        public void WeakRestrictAccess(float border, int damage, float knockback)
        {
            if (cutCount % 4 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<PunishmentCut>();
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.whoAmI == OwnerIndex)
                    {
                        float distance = player.Center.Distance(NPC.Center);
                        if (distance < border)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center,
                                player.velocity.RotatedByRandom(MathHelper.PiOver4), type,
                                damage, knockback, ai0: 20);
                        }
                        return;
                    }
                }
            }
        }

        public void RestrictAccess()
        {
            if (cutCount % 4 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<PunishmentCut>();
                foreach (Player player in Main.ActivePlayers)
                {
                    float distance = player.Center.Distance(NPC.Center);
                    bool entry = player.whoAmI != OwnerIndex && distance < maxDistance;
                    bool exit = player.whoAmI == OwnerIndex && distance >= maxDistance;

                    if (entry || exit)
                    {
                        // Restrict entry
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center,
                            player.velocity.RotatedByRandom(MathHelper.PiOver4), type, 
                            600, 0, ai0: 20);
                    }
                }
            }
        }

        public void MultipleCuts(Player target, int difficulty)
        {
            int attackStart = 60;
            int modification = (int)(3f * difficulty);
            if (PhaseCounter >= attackStart && PhaseCounter < attackStart + 60 && 
                PhaseCounter % (10 - modification) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 offset = new Vector2(0, Main._rand.NextFloat(0, 1000)).
                    RotatedByRandom(MathHelper.TwoPi);
                Vector2 pos = NPC.Center + offset;
                Vector2 direction = new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi);

                int dmg = 80;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, direction,
                    ModContent.ProjectileType<LongCut>(), dmg, 1f, ai0: NPC.whoAmI,
                    ai1: PhaseCounter - 60, ai2: (int)CurrentPhase
                    );
            }

            if (PhaseCounter >= attackStart + 80) PhaseCounter = 0;

            PhaseCounter++;
        }

        public void CircleCut(Player target, int difficulty)
        {
            int slashTime = 105;

            if (PhaseCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int bulletType = ModContent.ProjectileType<RingCircle>();
                float speed = 6 + (2f * difficulty);

                Vector2 dir2 = target.Center.DirectionTo(NPC.Center);
                Vector2 center = target.Center + new Vector2(0, 860).RotatedByRandom(MathHelper.TwoPi);
                Vector2 dir = center.DirectionTo(target.Center);

                for (int i = 1; i <= 30; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center, 
                        dir * speed, bulletType, 60, 4f, ai0: NPC.whoAmI, ai1: i, 
                        ai2: (int)CurrentPhase);
                }

                center = target.Center + new Vector2(0, 820).RotatedByRandom(MathHelper.TwoPi);
                dir = center.DirectionTo(target.Center);

                for (int i = -1; i >= -30; i--)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center,
                        dir * speed, bulletType, 60, 4f, ai0: NPC.whoAmI, ai1: i,
                        ai2: (int)CurrentPhase);
                }
            }

            if (PhaseCounter % slashTime == 0 && Main.netMode != NetmodeID.MultiplayerClient && 
                PhaseCounter >= 0)
            {
                // Temporary, make a different slash type for this 
                int slashType = ModContent.ProjectileType<BulletCut>();
                Vector2 offset = target.Center.DirectionTo(NPC.Center) * 224;
                Vector2 baseDir = Vector2.One.RotatedByRandom(MathHelper.TwoPi);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center,
                    baseDir, slashType, 80, 4f, ai0: NPC.whoAmI, ai1: (int)CurrentPhase);

                if (difficulty >= 2)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center,
                        baseDir.RotatedByRandom(MathHelper.PiOver4), slashType, 80, 4f, 
                        ai0: NPC.whoAmI, ai1: (int)CurrentPhase);
                }
            }


            if (PhaseCounter >= slashTime * 2)
            {
                PhaseCounter = 0;
            } else
            {
                PhaseCounter++;
            }
        }

        public void BackgroundCut(Player target, int difficulty)
        {
            int count = 3;
            if (difficulty >= 2) count = 2;
            // BackgroundModifications.drawOverBG = true;

            if (PhaseCounter % count == 0 && PhaseCounter < 180 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<MovingBullets>();

                Dictionary<int, Vector2> segments = new()
                {
                    [0] = new Vector2(0, -0.06f),
                    [1] = new Vector2(0, 0.06f)
                };

                switch (PhaseState)
                {
                    case 1:
                        segments = new()
                        {
                            [0] = new Vector2(-0.06f, 0),
                            [1] = new Vector2(0.06f, 0)
                        };
                        break;
                    case 2:
                        segments = new()
                        {
                            [0] = new Vector2(0, 0.06f),
                            [1] = new Vector2(0, -0.06f),
                            [2] = new Vector2(0, 0.06f),
                            [3] = new Vector2(0, -0.06f),
                            [4] = new Vector2(0, 0.06f),
                            [5] = new Vector2(0, -0.06f),
                            [6] = new Vector2(0, 0.06f),
                            [7] = new Vector2(0, -0.06f)
                        };
                        break;
                    case 3:
                        segments = new()
                        {
                            [0] = new Vector2(0.06f, 0),
                            [1] = new Vector2(-0.06f, 0),
                            [2] = new Vector2(0.06f, 0), 
                            [3] = new Vector2(-0.06f, 0),
                            [4] = new Vector2(0.06f, 0),
                            [5] = new Vector2(-0.06f, 0),
                            [6] = new Vector2(0.06f, 0),
                            [7] = new Vector2(-0.06f, 0)
                        };
                        break;
                }

                int amount = 5 + (2 * difficulty);

                for (int i = 0; i < amount; i++)
                {
                    Vector2 offset = new Vector2(-Main._rand.NextFloat(-1999, 1999),
                        Main._rand.NextFloat(-1999, 1999));
                    Vector2 pos = NPC.Center + offset;
                    
                    if (pos.Distance(target.Center) <= 100)
                    {
                        continue;
                    }

                    // Determine side
                    int index = 0;
                    int sides = segments.Count;
                    if (PhaseState % 2 == 0)
                    {
                        // Vertical cut
                        index = (int)float.Floor((offset.X + 2000f) / (4000 / sides));
                    }
                    else
                    {
                        // Horizontal cut
                        index = (int)float.Floor((offset.Y + 2000f) / (4000 / sides));
                    }

                    Vector2 vel = segments[index];

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, 
                        vel, type, 60, 3f, ai0: NPC.whoAmI, ai1: PhaseCounter, 
                        ai2: (int)CurrentPhase);
                }
            }

            if (PhaseCounter == 125 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int slashType = ModContent.ProjectileType<BulletCut>();

                Vector2 vel = new Vector2(0, 1);

                if (PhaseState % 2 != 0) vel = new Vector2(1, 0);

                switch (PhaseState)
                {
                    default:

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                            vel, slashType, 300, 4f, ai0: NPC.whoAmI,
                            ai1: (int)CurrentPhase);
                        break;
                    case 1:
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                            vel, slashType, 300, 4f, ai0: NPC.whoAmI,
                            ai1: (int)CurrentPhase);
                        break;
                    case 2:
                        for (int i = -2; i < 3; i++)
                        {
                            Vector2 offset = new Vector2(i * 2000f / 5f, 0);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset,
                                vel, slashType, 300, 4f, ai0: NPC.whoAmI,
                                ai1: (int)CurrentPhase);
                        }
                        break;
                    case 3:
                        for (int i = -2; i < 3; i++)
                        {
                            Vector2 offset = new Vector2(0, i * 2000f / 5f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset,
                                vel, slashType, 300, 4f, ai0: NPC.whoAmI,
                                ai1: (int)CurrentPhase);
                        }
                        break;
                }
            }

            if (PhaseCounter >= 580)
            {
                PhaseCounter = 0;
                PhaseState++;
            } else
            {
                PhaseCounter++;
            }

        }

        public void GridCut(Player target, int difficulty)
        {
            float separations = 13 + (2 * difficulty);
            float boundary = 2200;

            if (PhaseCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int slashType = ModContent.ProjectileType<GridCut>();

                for (int i = 0; i < separations; i++)
                {
                    Vector2 vel = new Vector2(0, 1);

                    Vector2 pos = NPC.Center + new Vector2(-boundary + (2 * i * boundary / separations), 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, new Vector2(0, 1), 
                        slashType, 90, 4f, ai0: NPC.whoAmI, ai1: (int)CurrentPhase);
                }

                for (int i = 0; i < separations; i++)
                {
                    Vector2 pos = NPC.Center + new Vector2(0, -boundary + (2 * i * boundary / separations));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, new Vector2(1, 0),
                        slashType, 90, 4f, ai0: NPC.whoAmI, ai1: (int)CurrentPhase);
                }
            }

            if (PhaseCounter == 120 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<MovingBullets>();
                Vector2 vel = new Vector2(0, 0.04f).RotatedByRandom(MathHelper.TwoPi);

                for (int i = 0; i < separations; i++)
                {
                    float posX = -boundary + (2 * i * boundary / separations);
                    for (int k = 0; k < separations; k++)
                    {
                        float posY = -boundary + (2 * k * boundary / separations);
                        Vector2 pos = NPC.Center + new Vector2(posX, posY);
                        int index = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos,
                            vel, type, 60, 3f, ai0: NPC.whoAmI, ai1: 120,
                            ai2: (int)CurrentPhase);

                        if (index != -1)
                        {
                            Projectile proj = Main.projectile[index];
                            if (proj.ModProjectile != null)
                            {
                                MovingBullets bullet = (MovingBullets)proj.ModProjectile;

                                bullet.spriteType = 1;
                                proj.netUpdate = true;
                            }
                        }
                    }
                }
            }

            if (PhaseCounter >= 380)
            {
                PhaseCounter = 0;
                PhaseState++;
            }
            else
            {
                PhaseCounter++;
            }
        }

        public void ThrillingCut(Player target, int difficulty)
        {
            if (PhaseCounter > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int counter = 24 - (difficulty * 6);
                if (PhaseCounter % counter == 0)
                {
                    int bulletType = ModContent.ProjectileType<ThrillBullet>();
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 offset = new Vector2(0, maxDistance).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 pos = NPC.Center + offset;
                        Vector2 vel = pos.DirectionTo(NPC.Center) * 0.06f;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, vel, bulletType, 
                            64, 2f, ai0: NPC.whoAmI, ai1: 8, ai2: (int)CurrentPhase);
                    }
                }

                float distance = float.Clamp(PhaseCounter - 180, 0, maxDistance - 256);
                dangerZone = distance;
                WeakRestrictAccess(distance, 100, 2f);
            }

            PhaseCounter++;
        }

        public void SingleGlimmer(Player target, int difficulty)
        {
            if (PhaseCounter % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<PunishmentCut>();

                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(Main._rand.NextFloat(-1200, 1200), 
                        Main._rand.NextFloat(-1200, 1200));

                    if (offset.Length() > 1200) continue;

                    Vector2 direction = new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset,
                        direction, type, 100, 3, ai0: -20);
                }
            }

            PhaseCounter++;
        }

        public override bool PreKill()
        {
            return NPC.Opacity <= 0;
        }

        public override void OnKill()
        {
            // Spawn Tasouken with an owner already set
            // Add a downed system for the Tasouken
            // Maybe it's not necessary for this particular boss
            //NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);
            Item tasouken = new Item();
            tasouken.SetDefaults(ModContent.ItemType<Tasouken>());
            Item.NewItem(NPC.GetSource_DropAsItem(OwnerName + ":" + !ownerFailed), NPC.Hitbox,
                ModContent.ItemType<Tasouken>());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {

        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(
                BossBorder.Value, 
                NPC.Center - Main.screenPosition, 
                BossBorder.Frame(), 
                Color.PaleGoldenrod * 0.6f, 
                0, 
                BossBorder.Size() / 2f, 
                new Vector2(2, 2), 
                SpriteEffects.None, 
                1
                );

            if (dangerZone > 0)
            {
                spriteBatch.Draw(
                    BossBorder.Value,
                    NPC.Center - Main.screenPosition,
                    BossBorder.Frame(),
                    Color.PaleVioletRed,
                    0,
                    BossBorder.Size() / 2f,
                    dangerZone / 600f,
                    SpriteEffects.None,
                    1
                    );
            }

            /*spriteBatch.Draw(
                BackgroundModifications.backgroundRT,
                NPC.Center - Main.screenPosition,
                new Rectangle(0, 400, 30, 30),
                Color.White,
                0,
                new Vector2(15, 15),
                new Vector2(1, 1),
                SpriteEffects.None,
                1
                );*/
        }
    }
}
