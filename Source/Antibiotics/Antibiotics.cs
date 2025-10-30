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
            if (__instance?.Pawn?.health?.hediffSet == null) return;

            var antibioticHediff = __instance.Pawn.health.hediffSet.GetFirstHediffOfDef(AntibioticsDef);
            if (antibioticHediff != null)
            {
                var comp = antibioticHediff.TryGetComp<HediffComp_Antibiotic>();
                if (comp != null)
                    __result *= comp.InfectionSlowdownFactor;
            }
        }
    }
}
