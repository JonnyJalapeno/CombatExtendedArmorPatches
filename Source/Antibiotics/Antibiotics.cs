using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(HediffComp_Immunizable), nameof(HediffComp_Immunizable.SeverityChangePerDay))]
    public static class Patch_Immunizable_Severity_Antibiotics
    {
        private static readonly HediffDef AntibioticsDef = DefDatabase<HediffDef>.GetNamed("AntibioticsHediff");

        public static void Postfix(HediffComp_Immunizable __instance, ref float __result)
        {
            var pawn = __instance?.Pawn;
            if (pawn == null) return;
            var set = pawn.health?.hediffSet;
            if (set == null) return;

            var antibiotic = set.GetFirstHediffOfDef(AntibioticsDef);
            if (antibiotic == null) return;

            var comp = antibiotic.TryGetComp<HediffComp_Antibiotic>();
            if (comp != null)
                __result *= comp.InfectionSlowdownFactor;
        }
    }
}
