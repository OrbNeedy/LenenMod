using lenen.Content.NPCs;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace lenen.Common.Systems
{
    public class SoulExceptions : ModSystem
    {
        public static SoulExceptions instance;

        public List<int> soulessNPCs = new List<int>();
        public List<int> undetectableNPCs = new List<int>();

        public override void Load()
        {
            instance = this;
        }

        public override void SetStaticDefaults()
        {
            soulessNPCs.AddRange(new int[] {
                ModContent.NPCType<CurtainOfAwakening>(), NPCID.TargetDummy, NPCID.GolemFistLeft,
                NPCID.GolemFistRight, NPCID.GolemHeadFree, NPCID.GolemHead, NPCID.MartianDrone, NPCID.MartianProbe,
                NPCID.MartianWalker, NPCID.MothronEgg, NPCID.PlanterasHook, NPCID.PlanterasTentacle,
                NPCID.SkeletronHand, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice,
                NPCID.WallofFleshEye, NPCID.DD2LanePortal, NPCID.CultistTablet, NPCID.AncientDoom, 
                NPCID.AncientLight, NPCID.CultistBossClone
            });

            undetectableNPCs.AddRange(new int[] {
                NPCID.TargetDummy, NPCID.GiantWormBody, NPCID.GiantWormTail, NPCID.StardustWormBody, 
                NPCID.StardustWormTail, NPCID.BloodEelBody, NPCID.BloodEelTail, NPCID.CultistDragonBody1, 
                NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, 
                NPCID.CultistDragonTail, NPCID.BoneSerpentBody, NPCID.BoneSerpentTail, NPCID.DevourerBody, 
                NPCID.DevourerTail, NPCID.DiggerBody, NPCID.DiggerTail, NPCID.DuneSplicerBody, 
                NPCID.DuneSplicerTail, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.LeechBody, 
                NPCID.LeechTail, NPCID.SeekerBody, NPCID.SeekerTail, NPCID.SolarCrawltipedeBody, 
                NPCID.SolarCrawltipedeTail, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail,NPCID.TombCrawlerBody, 
                NPCID.TombCrawlerTail, NPCID.WyvernBody, NPCID.WyvernBody2, NPCID.WyvernBody3, NPCID.WyvernTail, 
                NPCID.MartianSaucerCannon, NPCID.MartianSaucer, NPCID.MartianSaucerTurret
            });
        }
    }
}
