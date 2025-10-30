using RimWorld;
using Verse;
using System.Linq;

namespace CombatExtendedArmorPatches
{
    public class Hediff_CarotidDamage : Hediff
    {
        private int ticksUntilNextUpdate;
        private const int updateInterval = 60; // every 10 ticks
        private bool tendingApplied = false;
        private float tendingReduction = 0f; // proportional reduction once applied

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

            if (part == null)
            {
                pawn.health.RemoveHediff(this);
                return;
            }

            // Check for tended injury and apply proportional reduction once
            if (!tendingApplied)
            {
                var injury = pawn.health.hediffSet.hediffs
                    .OfType<Hediff_Injury>()
                    .FirstOrDefault(h => h.Part == part && h.IsTended());

                if (injury != null)
                {
                    // Proportional reduction, e.g., 30% of current severity
                    tendingReduction = 0.5f * Utils.CalculateSeverityForPart(part, pawn);
                    tendingApplied = true;
                }
            }

            // Calculate current severity from part health
            float baseSeverity = Utils.CalculateSeverityForPart(part, pawn);

            // Apply tending reduction, clamp at zero
            Severity = baseSeverity - tendingReduction;
            if (Severity <= 0f)
            {
                pawn.health.RemoveHediff(this);
            }
        }
    }
}
