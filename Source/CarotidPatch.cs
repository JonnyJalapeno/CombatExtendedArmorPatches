using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury), nameof(DamageWorker_AddInjury.Apply))]
    static class DamageWorker_AddInjury_Apply_Patch
    {
        private static readonly HediffDef carotidHediffDef = 
            DefDatabase<HediffDef>.GetNamed("CarotidDamageHediffDef", false);

        [HarmonyPostfix]
        static void Postfix(DamageInfo dinfo, Thing thing)
        {
            if (!(thing is Pawn pawn) || dinfo.HitPart == null || carotidHediffDef == null)
                return;

            if (dinfo.HitPart.def.defName != "CarotidArtery")
                return;

            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(carotidHediffDef, false)
                         ?? pawn.health.AddHediff(carotidHediffDef, dinfo.HitPart);

            hediff.Severity = CarotidUtils.CalculateSeverityForPart(dinfo.HitPart, pawn);
        }
    }
}
