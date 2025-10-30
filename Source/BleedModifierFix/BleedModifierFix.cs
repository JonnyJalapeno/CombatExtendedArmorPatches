using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace CombatExtendedArmorPatches
{
    // patching CE bleeding for missing parts so it returns to 1.0x base bleed rate
   [HarmonyPatch(typeof(CombatExtended.HediffComp_Stabilize), "get_BleedModifier")]
    public static class HediffComp_Stabilize_BleedModifier_Patch
    {
        static void Postfix(CombatExtended.HediffComp_Stabilize __instance, ref float __result)
        {
            // Remove the missing part halving
            if (__instance.parent is Hediff_MissingPart)
            {
                __result *= 2f; // undo the original *0.5f
            }
        }
    }
}