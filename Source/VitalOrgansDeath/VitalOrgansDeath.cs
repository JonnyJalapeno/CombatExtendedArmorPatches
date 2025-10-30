using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(Hediff_MissingPart))]
    [HarmonyPatch("PostAdd")]
    static class Hediff_MissingPart_WaistKillPatch
    {
        static void Postfix(Hediff_MissingPart __instance)
        {
            if (__instance.Part.def.tags.Any(tag => tag.defName == "isVital") && !__instance.pawn.Dead)
            {
                __instance.pawn.Kill(null);
            }
        }
    }
}