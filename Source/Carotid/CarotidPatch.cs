using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplyDamageToPart")]
    internal static class Harmony_DamageWorker_AddInjury_ApplyDamageToPart
    {
        private static readonly HediffDef CarotidHediffDef =
            DefDatabase<HediffDef>.GetNamed("CarotidDamageHediffDef", false);

        [HarmonyPostfix]
        static void Postfix(Pawn pawn, DamageWorker.DamageResult result)
        {
            if (CarotidHediffDef == null || pawn == null || result?.LastHitPart?.parts == null)
                return;

            var hediffSet = pawn.health?.hediffSet;
            if (hediffSet == null) return;

            foreach (var part in result.LastHitPart.parts)
            {
                if (part?.def?.defName != "CarotidArtery") continue;
                if (hediffSet.PartIsMissing(part)) continue;

                var hediff = hediffSet.GetFirstHediffOfDef(CarotidHediffDef, false)
                            ?? pawn.health.AddHediff(CarotidHediffDef, part);

                hediff.Severity = Utils.CalculateSeverityForPart(part, pawn);
            }
        }
    }
}
