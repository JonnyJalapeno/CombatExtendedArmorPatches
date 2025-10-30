using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplyDamageToPart")]
    internal static class Harmony_DamageWorker_AddInjury_ApplyDamageToPart
    {
        private static readonly HediffDef carotidHediffDef =
            DefDatabase<HediffDef>.GetNamed("CarotidDamageHediffDef", false);

        [HarmonyPostfix]
        static void Postfix(Pawn pawn, DamageWorker.DamageResult result)
        {
            if (carotidHediffDef == null || pawn == null || result == null || result?.LastHitPart?.parts == null)
                return;

            foreach (var part in result.LastHitPart.parts)
            {
                if (part == null || part.def == null)
                    continue;
                if (part?.def?.defName != "CarotidArtery")
                    continue;
                if (pawn.health.hediffSet.PartIsMissing(part))
                    continue; // <-- critical fix

                var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(carotidHediffDef, false)
                            ?? pawn.health.AddHediff(carotidHediffDef, part);

                hediff.Severity = Utils.CalculateSeverityForPart(part, pawn);
            }
        }
    }

}
