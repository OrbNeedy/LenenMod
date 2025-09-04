using lenen.Content.NPCs;
using lenen.Content.Projectiles;
using lenen.Content.Projectiles.BulletHellProjectiles;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.Systems
{
    public class GravityExceptions : ModSystem
    {
        public static GravityExceptions Instance;

        public List<int> gravityResistantNPCs = new();
        public List<int> gravityAffectedProjectiles = new();

        public override void Load()
        {
            Instance = this;
        }

        public override void SetStaticDefaults()
        {
            gravityResistantNPCs.AddRange([
                ModContent.NPCType<CurtainOfAwakening>(), NPCID.TargetDummy, NPCID.GolemFistLeft,
                NPCID.GolemFistRight, NPCID.GolemHeadFree, NPCID.GolemHead, NPCID.MartianDrone, NPCID.MartianProbe,
                NPCID.MartianWalker, NPCID.MothronEgg, NPCID.PlanterasHook, NPCID.PlanterasTentacle,
                NPCID.SkeletronHand, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice,
                NPCID.WallofFleshEye, NPCID.DD2LanePortal, NPCID.CultistTablet, NPCID.AncientDoom,
                NPCID.AncientLight, NPCID.CultistBossClone, NPCID.GiantWormBody, NPCID.GiantWormTail, NPCID.StardustWormBody,
                NPCID.StardustWormTail, NPCID.BloodEelBody, NPCID.BloodEelTail, NPCID.CultistDragonBody1,
                NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4,
                NPCID.CultistDragonTail, NPCID.BoneSerpentBody, NPCID.BoneSerpentTail, NPCID.DevourerBody,
                NPCID.DevourerTail, NPCID.DiggerBody, NPCID.DiggerTail, NPCID.DuneSplicerBody,
                NPCID.DuneSplicerTail, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.LeechBody,
                NPCID.LeechTail, NPCID.SeekerBody, NPCID.SeekerTail, NPCID.SolarCrawltipedeBody,
                NPCID.SolarCrawltipedeTail, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail,NPCID.TombCrawlerBody,
                NPCID.TombCrawlerTail, NPCID.WyvernBody, NPCID.WyvernBody2, NPCID.WyvernBody3, NPCID.WyvernTail,
                NPCID.MartianSaucerCannon, NPCID.MartianSaucer, NPCID.MartianSaucerTurret
            ]);

            gravityAffectedProjectiles.AddRange([
                ProjectileID.WoodenArrowFriendly, ProjectileID.FireArrow, ProjectileID.Shuriken, ProjectileID.UnholyArrow,
                ProjectileID.Bullet, ProjectileID.BallofFire, ProjectileID.DirtBall, ProjectileID.Bone, ProjectileID.WaterStream,
                ProjectileID.SpikyBall, ProjectileID.WaterBolt, ProjectileID.Bomb, ProjectileID.Dynamite, ProjectileID.Grenade,
                ProjectileID.SandBallFalling, ProjectileID.HarpyFeather, ProjectileID.MudBall, ProjectileID.AshBallFalling,
                ProjectileID.HellfireArrow, ProjectileID.SandBallGun, ProjectileID.ThrowingKnife, ProjectileID.Glowstick,
                ProjectileID.Seed, ProjectileID.PoisonedKnife, ProjectileID.Stinger, ProjectileID.EbonsandBallFalling,
                ProjectileID.EbonsandBallGun, ProjectileID.PearlSandBallFalling, ProjectileID.PearlSandBallGun,
                ProjectileID.HolyWater, ProjectileID.UnholyWater, ProjectileID.HappyBomb, ProjectileID.FlamingArrow,
                ProjectileID.CrystalBullet, ProjectileID.CrystalShard, ProjectileID.CrystalStorm, ProjectileID.CursedFlameFriendly,
                ProjectileID.CursedFlameHostile, ProjectileID.PoisonDart, ProjectileID.EyeFire, ProjectileID.BombSkeletronPrime,
                ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.SnowBallHostile, ProjectileID.BulletSnowman,
                ProjectileID.BoneArrow, ProjectileID.FrostArrow, ProjectileID.GrenadeI, ProjectileID.RocketI,
                ProjectileID.ProximityMineI, ProjectileID.GrenadeII, ProjectileID.RocketII, ProjectileID.ProximityMineII,
                ProjectileID.GrenadeIII, ProjectileID.RocketIII, ProjectileID.ProximityMineIII, ProjectileID.GrenadeIV,
                ProjectileID.RocketIV, ProjectileID.ProximityMineIV, ProjectileID.BeachBall, ProjectileID.CopperCoin,
                ProjectileID.SilverCoin, ProjectileID.GoldCoin, ProjectileID.PlatinumCoin, ProjectileID.CannonballFriendly,
                ProjectileID.Flare, ProjectileID.SnowBallFriendly, ProjectileID.RopeCoil, ProjectileID.ConfettiGun,
                ProjectileID.BulletDeadeye, ProjectileID.Bee, ProjectileID.Beenade, ProjectileID.PoisonDartTrap,
                ProjectileID.SpikyBallTrap, ProjectileID.Wasp, ProjectileID.SmokeBomb, ProjectileID.Leaf,
                ProjectileID.SporeCloud, ProjectileID.ChlorophyteOrb, ProjectileID.RainCloudMoving, ProjectileID.CannonballHostile,
                ProjectileID.CrimsandBallFalling, ProjectileID.BloodCloudMoving, ProjectileID.Stynger,
                ProjectileID.FlowerPowPetal, ProjectileID.StyngerShrapnel, ProjectileID.BallofFrost, ProjectileID.PoisonFang,
                ProjectileID.PoisonDartBlowgun, ProjectileID.SeedPlantera, ProjectileID.PoisonSeedPlantera, ProjectileID.IchorArrow,
                ProjectileID.IchorBullet, ProjectileID.GoldenShowerFriendly, ProjectileID.ExplosiveBunny, ProjectileID.VenomArrow,
                ProjectileID.VenomBullet, ProjectileID.PartyBullet, ProjectileID.NanoBullet, ProjectileID.ExplosiveBullet,
                ProjectileID.GoldenBullet, ProjectileID.GoldenShowerHostile, ProjectileID.ConfettiMelee, ProjectileID.RocketSkeleton,
                ProjectileID.TinyEater, ProjectileID.BlueFlare, ProjectileID.CandyCorn, ProjectileID.JackOLantern,
                ProjectileID.Bat, ProjectileID.RottenEgg, ProjectileID.Stake, ProjectileID.FlamingWood,
                ProjectileID.OrnamentFriendly, ProjectileID.PineNeedleFriendly, ProjectileID.NorthPoleSnowflake,
                ProjectileID.PineNeedleHostile, ProjectileID.OrnamentHostile, ProjectileID.OrnamentHostileShrapnel,
                ProjectileID.Missile, ProjectileID.Present, ProjectileID.Spike, ProjectileID.CrimsandBallGun,
                ProjectileID.VenomFang, ProjectileID.WaterGun, ProjectileID.SpiderEgg, ProjectileID.BabySpider,
                ProjectileID.VenomSpider, ProjectileID.JumperSpider, ProjectileID.DangerousSpider, ProjectileID.StickyGrenade,
                ProjectileID.MolotovCocktail, ProjectileID.SlimeGun, ProjectileID.Bubble, ProjectileID.CopperCoinsFalling,
                ProjectileID.SilverCoinsFalling, ProjectileID.GoldCoinsFalling, ProjectileID.PlatinumCoinsFalling,
                ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.Xenopopper,
                ProjectileID.SaucerMissile, ProjectileID.SaucerScrap, ProjectileID.BeeArrow, ProjectileID.StickyDynamite,
                ProjectileID.SkeletonBone, ProjectileID.WebSpit, ProjectileID.SpelunkerGlowstick, ProjectileID.BoneArrowFromMerchant,
                ProjectileID.VineRopeCoil, ProjectileID.CrystalDart, ProjectileID.CursedDart, ProjectileID.IchorDart,
                ProjectileID.CursedDartFlame, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn, ProjectileID.Hellwing,
                ProjectileID.ShadowFlameArrow, ProjectileID.Nail, ProjectileID.DrManFlyFlask, ProjectileID.SilkRopeCoil,
                ProjectileID.WebRopeCoil, ProjectileID.JavelinFriendly, ProjectileID.JavelinHostile, ProjectileID.ToxicFlask,
                ProjectileID.ToxicCloud, ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud3, ProjectileID.NailFriendly,
                ProjectileID.BouncyGlowstick, ProjectileID.BouncyBomb, ProjectileID.BouncyGrenade, ProjectileID.FrostDaggerfish,
                ProjectileID.CrystalPulse, ProjectileID.CrystalPulse2, ProjectileID.ToxicBubble, ProjectileID.IchorSplash,
                ProjectileID.BoneGloveProj, ProjectileID.GiantBee, ProjectileID.SporeTrap, ProjectileID.SporeTrap2,
                ProjectileID.SporeGas, ProjectileID.SporeGas2, ProjectileID.SporeGas3, ProjectileID.PainterPaintball,
                ProjectileID.PartyGirlGrenade, ProjectileID.SantaBombs, ProjectileID.TruffleSpore, ProjectileID.DesertDjinnCurse,
                ProjectileID.BoneDagger, ProjectileID.BloodWater, ProjectileID.BouncyDynamite, ProjectileID.Ale,
                ProjectileID.QueenBeeStinger, ProjectileID.BlueDungeonDebris, ProjectileID.GreenDungeonDebris,
                ProjectileID.PinkDungeonDebris, ProjectileID.RollingCactusSpike, ProjectileID.Geode, ProjectileID.ScarabBomb,
                ProjectileID.ClusterRocketI, ProjectileID.ClusterGrenadeI, ProjectileID.ClusterMineI, ProjectileID.ClusterFragmentsI,
                ProjectileID.ClusterRocketII, ProjectileID.ClusterGrenadeII, ProjectileID.ClusterMineII,
                ProjectileID.ClusterFragmentsII, ProjectileID.WetRocket, ProjectileID.WetGrenade, ProjectileID.WetMine, 
                ProjectileID.LavaRocket, ProjectileID.LavaGrenade, ProjectileID.LavaMine, ProjectileID.HoneyRocket, 
                ProjectileID.HoneyGrenade, ProjectileID.HoneyMine, ProjectileID.MiniNukeRocketI, ProjectileID.MiniNukeGrenadeI,
                ProjectileID.MiniNukeGrenadeI, ProjectileID.MiniNukeRocketII, ProjectileID.MiniNukeGrenadeII,
                ProjectileID.MiniNukeGrenadeII, ProjectileID.DryRocket, ProjectileID.DryGrenade, ProjectileID.DryMine,
                ProjectileID.BloodArrow, ProjectileID.DandelionSeed, ProjectileID.ReleaseDoves, ProjectileID.ReleaseLantern,
                ProjectileID.ClusterFragmentsI, ProjectileID.ClusterFragmentsII, ProjectileID.FairyGlowstick, ProjectileID.WetBomb,
                ProjectileID.LavaBomb, ProjectileID.HoneyBomb, ProjectileID.DryBomb, ProjectileID.OrnamentStar, 
                ProjectileID.RockGolemRock, ProjectileID.DirtBomb, ProjectileID.DirtStickyBomb, 
                ProjectileID.QueenSlimeMinionBlueSpike, ProjectileID.QueenSlimeMinionPinkBall, ProjectileID.QueenSlimeGelAttack,
                ProjectileID.GelBalloon, ProjectileID.VolatileGelatinBall, ProjectileID.DeerclopsRangedProjectile,
                ProjectileID.BladeOfGrass, ProjectileID.VenomDartTrap, ProjectileID.SilverBullet, ProjectileID.MiniBoulder,
                ProjectileID.ShimmerArrow, ProjectileID.GasTrap, ProjectileID.SpelunkerFlare, ProjectileID.CursedFlare,
                ProjectileID.RainbowFlare, ProjectileID.ShimmerFlare, 
            ]);
            gravityAffectedProjectiles.AddRange([
                ModContent.ProjectileType<BasicBullet>(), ModContent.ProjectileType<SpiritFlame>(), 
                ModContent.ProjectileType<SlowedBullet>()
            ]);
        }
    }
}
