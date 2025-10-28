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
            if (carotidHediffDef == null || pawn == null || result == null)
                return;

            foreach (var part in result.LastHitPart.parts)
            {
                if (part?.def?.defName != "CarotidArtery")
                    continue;

                var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(carotidHediffDef, false)
                             ?? pawn.health.AddHediff(carotidHediffDef, part);

                hediff.Severity = CarotidUtils.CalculateSeverityForPart(part, pawn);
            }
        }
    }
}
