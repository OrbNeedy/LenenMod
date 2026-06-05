using lenen.Content.BossBars;
using lenen.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace lenen.Content.NPCs.TasoukenBoss
{
    // Consider replacing some of the harder ones for custom moves
    // Have an alternative for ThrillingCut if player doesn't have the thrill engine
    // Single Glimmer should stay the same always
    public enum TasoukenPhases
    {
        MultipleCuts, 
        CircleCut, 
        BackgroundCut, 
        GridCut, 
        ThrillingCut, 
        SingleGlimmer
    }

    [AutoloadBossHead]
    public class TasoukenBoss : ModNPC
    {
        public int OwnerIndex { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public TasoukenPhases LastPhase { get => (TasoukenPhases)NPC.ai[1]; set => NPC.ai[1] = (int)value; }
        public int LivingTime = 0;
        public bool ownerFailed = false;
        public string OwnerName = "";

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
                CustomTexturePath = "lenen/Content/Items/Weapons/Tasouken",
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 88;

            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 2000;
            NPC.statsAreScaledForThisManyPlayers = 0;

            NPC.HitSound = SoundID.NPCHit1; // Add custom sound
            NPC.DeathSound = SoundID.NPCDeath1; // Add custom sound
            NPC.knockBackResist = 0f;
            
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 20f; // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Default buff immunities should be set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
            // To dynamically adjust immunities of an active NPC, NPC.buffImmune[] can be changed in AI: NPC.buffImmune[BuffID.OnFire] = true;
            // This approach, however, will not preserve buff immunities. To preserve buff immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, as shown in the ApplySecondStageBuffImmunities method below.

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar
            NPC.BossBar = ModContent.GetInstance<TasoukenBossBar>();

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
        }

        // "PlayerName has proven their power."
        public override LocalizedText DeathMessage => 
            Language.GetText("Announcement.TasoukenDefeated").WithFormatArgs(OwnerName);

        public override void OnSpawn(IEntitySource source)
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.whoAmI == OwnerIndex)
                {
                    OwnerName = player.name;
                }
            }
        }

        public bool CheckOwner(int who)
        {
            if (who == -1) return false;

            Player owner = Main.player[who];

            return owner.active && owner.statLife > 0 && !owner.DeadOrGhost;
        }

        public override void AI()
        {
            // Despawn sequence
            if (NPC.life <= 1)
            {
                NPC.Opacity -= 0255f;

                if (NPC.Opacity <= 0)
                {
                    // Despawn
                }
                return;
            }

            // If owner died, despawn and set owner failed to true
            if (!CheckOwner(OwnerIndex))
            {
                NPC.life = 1;
                ownerFailed = true;
                return;
            }

            Move();

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

        public void Move()
        {
            Player owner = Main.player[OwnerIndex];
            float sineWave = MathF.Sin((NPC.lifeMax - NPC.life) * 0.04f);
            Vector2 offset = new Vector2(0, -96 + (sineWave * 16));

            NPC.Center = owner.Center + offset;
        }

        public void MultipleCuts()
        {
        }

        public void CircleCut()
        {

        }

        public void BackgroundCut()
        {
            // Use in the system that changes background
            /*MethodInfo backgroundDrawMethod = Main.instance.GetType().GetMethod("DrawBackground",
                BindingFlags.NonPublic | BindingFlags.Instance);
            // DrawBackground() no parameters
            backgroundDrawMethod.Invoke(Main.instance, []);*/
        }

        public void GridCut()
        {

        }

        public void ThrillingCut()
        {

        }

        public void SingleGlimmer()
        {

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
                tasouken);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {

        }
    }
}
