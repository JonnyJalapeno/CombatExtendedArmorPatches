using RimWorld;
using Verse;
using HarmonyLib;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(CombatExtended.HediffComp_Stabilize), "get_BleedModifier")]
    public static class HediffComp_Stabilize_BleedModifier_Patch
    {
        static void Postfix(CombatExtended.HediffComp_Stabilize __instance, ref float __result)
        {
            if (__instance.parent is Hediff_MissingPart)
                __result *= 2f;
        }
    }
}
