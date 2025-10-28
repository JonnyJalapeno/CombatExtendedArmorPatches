using RimWorld;
using Verse;
using System.Linq;

namespace CombatExtendedArmorPatches
{
    public class Hediff_CarotidDamage : Hediff
    {
        private int ticksUntilNextUpdate;
        private const int updateInterval = 10; // every 10 ticks

        // Stores the flat severity reduction applied by tending
        private float tendingReduction = 0f;
        private bool tendedApplied = false;

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

            // Check if there is a tended injury on the carotid part
            var injury = pawn.health.hediffSet.hediffs
                .OfType<Hediff_Injury>()
                .FirstOrDefault(h => h.Part == part && h.IsTended());

            // Apply flat reduction **once** if not yet applied
            if (!tendedApplied && injury != null)
            {
                tendingReduction = 0.3f; // flat reduction
                tendedApplied = true;
            }

            // Calculate base severity
            float baseSeverity = CarotidUtils.CalculateSeverityForPart(part, pawn);

            // Apply reduction and clamp to >= 0
            Severity = baseSeverity - tendingReduction;
            if (Severity <= 0f)
            {
                pawn.health.RemoveHediff(this);
            }
        }
    }
}
