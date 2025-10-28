using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace CombatExtendedArmorPatches
{
    [StaticConstructorOnStartup]
    public static class LoadoutFilterPatches
    {
        static LoadoutFilterPatches()
        {
            var harmony = new Harmony("com.yourname.CombatExtendedArmorPatches");
            harmony.PatchAll();
            Log.Message("[CombatExtendedArmorPatches] patch applied");
        }
    }

    /*[HarmonyPatch(typeof(Hediff_MissingPart), "get_BleedRate")]
    static class Hediff_MissingPart_BleedRate_ScalePatch
    {
        // Define per-part multipliers here
        static readonly Dictionary<string, float> PartBleedMultipliers = new Dictionary<string, float>
        {
            { "Waist", 0.5f },
            { "Arm", 2f },
            { "Leg", 1.0f } // default multiplier for demonstration
        };

        static void Postfix(Hediff_MissingPart __instance, ref float __result)
        {
            if (__instance.IsFreshNonSolidExtremity) // only scale bleeding for freshly missing parts
            {
                if (PartBleedMultipliers.TryGetValue(__instance.Part.def.defName, out float multiplier))
                {
                    __result *= multiplier;
                }
            }
        }
    }*/

    /*[HarmonyPatch(typeof(HediffGiverUtility), "TryApply")]
    public static class HediffGiverUtility_TryApply_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn pawn, out Dictionary<BodyPartRecord, BodyPartDepth> originalDepths)
        {
            originalDepths = new Dictionary<BodyPartRecord, BodyPartDepth>();

            if (pawn == null) return;

            // Get all genital body parts
            var genitalParts = pawn.RaceProps.body.GetPartsWithDef(DefDatabase<BodyPartDef>.GetNamed("Genitals"));
            if (!genitalParts.Any()) return;

            // Store original depth and set per gender
            foreach (var part in genitalParts)
            {
                originalDepths[part] = part.depth; // store original
                part.depth = pawn.gender == Gender.Female ? BodyPartDepth.Inside : BodyPartDepth.Outside;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Dictionary<BodyPartRecord, BodyPartDepth> originalDepths)
        {
            // Restore original depth
            if (originalDepths == null) return;

            foreach (var kvp in originalDepths)
                kvp.Key.depth = kvp.Value;
        }
    }*/

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
    // unfucking CE bleeding for missing parts so it returns to 1.0x base bleed rate
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
    // add bleeding equivalent to aortic bleeding when the limb was freshly lost and had an artery
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


