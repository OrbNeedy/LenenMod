using lenen.Content.NPCs;
using lenen.Content.Projectiles.BulletHellProjectiles;
using lenen.Content.Projectiles;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.Systems
{
    public class TeleportExceptions : ModSystem
    {
        public static TeleportExceptions Instance;

        public List<int> teleportImmuneProjectiles = new();

        public override void Load()
        {
            Instance = this;
        }

        public override void SetStaticDefaults()
        {
            teleportImmuneProjectiles.AddRange([
                ProjectileID.LastPrismLaser, ProjectileID.MoonlordTurretLaser, ProjectileID.DeathLaser, 
                ProjectileID.VortexLaser, ProjectileID.UFOLaser, ProjectileID.SaucerLaser, ProjectileID.PetLizard,
                ProjectileID.BerniePet, ProjectileID.BlueChickenPet, ProjectileID.BrainOfCthulhuPet, 
                ProjectileID.ChesterPet, ProjectileID.DD2BetsyPet, ProjectileID.DD2OgrePet, ProjectileID.DD2PetDragon,
                ProjectileID.DD2PetGato, ProjectileID.DD2PetGhost, ProjectileID.DeerclopsPet, 
                ProjectileID.DestroyerPet, ProjectileID.DukeFishronPet, ProjectileID.EaterOfWorldsPet, 
                ProjectileID.EverscreamPet, ProjectileID.EyeOfCthulhuPet, ProjectileID.FairyQueenPet, 
                ProjectileID.GlommerPet, ProjectileID.GolemPet, ProjectileID.IceQueenPet, ProjectileID.JunimoPet,
                ProjectileID.KingSlimePet, ProjectileID.LunaticCultistPet, ProjectileID.MartianPet, 
                ProjectileID.MoonLordPet, ProjectileID.PigPet, ProjectileID.PlanteraPet, ProjectileID.PumpkingPet,
                ProjectileID.QueenBeePet, ProjectileID.QueenSlimePet, ProjectileID.SkeletronPet, 
                ProjectileID.SkeletronPrimePet, ProjectileID.TwinsPet, ProjectileID.Sharknado, 
                ProjectileID.SpiritHeal, ProjectileID.VampireHeal
            ]);
            teleportImmuneProjectiles.AddRange([
                ModContent.ProjectileType<KuroLaser>(), ModContent.ProjectileType<InfiniteLaser>(),
                ModContent.ProjectileType<LumenLaser>(), ModContent.ProjectileType<KuroLaserHoldout>(),
                ModContent.ProjectileType<Laser>(), ModContent.ProjectileType<HaniwaCannon>(),
                ModContent.ProjectileType<HaniwaFist>(), ModContent.ProjectileType<StrikeHaniwa>()
            ]);
        }
    }
}
