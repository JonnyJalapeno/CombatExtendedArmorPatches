using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace CombatExtendedArmorPatches
{
    [StaticConstructorOnStartup]
    public static class Initialize
    {
        static Initialize()
        {
            var harmony = new Harmony("com.Vril.CombatExtendedArmorPatches");
            harmony.PatchAll();
            Log.Message("[CombatExtendedArmorPatches] patch applied");
        }
    }

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
    
}


