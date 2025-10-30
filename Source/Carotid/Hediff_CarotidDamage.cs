using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
    public class Hediff_CarotidDamage : Hediff
    {
        private int nextUpdateTick;
        private const int updateInterval = 60; // every 60 ticks
        private bool tendingApplied;
        private float tendingReduction;

        public override void PostMake()
        {
            base.PostMake();
            nextUpdateTick = Find.TickManager.TicksGame + updateInterval;
        }

        public override void Tick()
        {
            int ticksGame = Find.TickManager.TicksGame;
            if (ticksGame < nextUpdateTick) return;
            nextUpdateTick = ticksGame + updateInterval;

            var hediffSet = pawn.health?.hediffSet;
            if (hediffSet == null)
            {
                pawn.health.RemoveHediff(this);
                return;
            }

            BodyPartRecord carotidPart = null;
            foreach (var part in hediffSet.GetNotMissingParts())
            {
                if (part.def.defName == "CarotidArtery")
                {
                    carotidPart = part;
                    break;
                }
            }

            if (carotidPart == null)
            {
                pawn.health.RemoveHediff(this);
                return;
            }

            if (!tendingApplied)
            {
                foreach (var h in hediffSet.hediffs)
                {
                    if (h is Hediff_Injury injury && injury.Part == carotidPart && injury.IsTended())
                    {
                        tendingReduction = 0.5f * Utils.CalculateSeverityForPart(carotidPart, pawn);
                        tendingApplied = true;
                        break;
                    }
                }
            }

            float baseSeverity = Utils.CalculateSeverityForPart(carotidPart, pawn);
            Severity = baseSeverity - tendingReduction;
            if (Severity <= 0f)
                pawn.health.RemoveHediff(this);
        }
    }
}
