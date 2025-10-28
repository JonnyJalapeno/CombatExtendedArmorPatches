using RimWorld;
using Verse;
using System.Linq;

namespace CombatExtendedArmorPatches
{
    public class Hediff_CarotidDamage : Hediff
    {
        private int ticksUntilNextUpdate;
        private const int updateInterval = 10; // every 10 ticks

        public override void PostMake()
        {
            base.PostMake();
            ticksUntilNextUpdate = Find.TickManager.TicksGame + updateInterval;
        }

        public override void Tick()
        {
            int ticksGame = Find.TickManager.TicksGame;
            if (ticksGame < ticksUntilNextUpdate) return;
            ticksUntilNextUpdate = ticksGame + updateInterval;

            var part = pawn.health.hediffSet.GetNotMissingParts()
                .FirstOrDefault(p => p.def.defName == "CarotidArtery");

            if (part == null || (Severity = CarotidUtils.CalculateSeverityForPart(part, pawn)) <= 0f)
            {
                pawn.health.RemoveHediff(this);
            }
        }
    }
}
