using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace CombatExtendedArmorPatches
{
    //Adds bleeding equal to aorta/artery bleed rate for missing parts that had arteries/aorta when freshly lost
   [HarmonyPatch(typeof(Hediff_MissingPart), "get_BleedRate")]
    static class Hediff_MissingPart_BleedRate_ScalePatch
    {
        static void Postfix(Hediff_MissingPart __instance, ref float __result)
        {
            if (!__instance.IsFreshNonSolidExtremity)
                return;

            var bodyparts = __instance.Part.GetDirectChildParts();
            if (bodyparts == null)
                return;

            foreach (var part in bodyparts)
            {
                if (part.def.defName.Contains("Artery") || part.def.defName.Contains("Aorta"))
                {
                    float originalBleedRate = __instance.Part.def.bleedRate;
                    float newBleedRate = part.def.bleedRate;
                    if (originalBleedRate > 0f)
                        __result *= newBleedRate / originalBleedRate;
                    break;
                }
            }
        }
    } 
}