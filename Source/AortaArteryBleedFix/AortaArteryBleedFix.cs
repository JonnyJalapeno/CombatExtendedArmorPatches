using RimWorld;
using Verse;
using HarmonyLib;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(Hediff_MissingPart), "get_BleedRate")]
    static class Hediff_MissingPart_BleedRate_ScalePatch
    {
        static readonly string Artery = "Artery";
        static readonly string Aorta = "Aorta";

        static void Postfix(Hediff_MissingPart __instance, ref float __result)
        {
            if (!__instance.IsFreshNonSolidExtremity) return;
            var part = __instance.Part;
            var children = part.GetDirectChildParts();
            if (children == null) return;

            float baseBleed = part.def.bleedRate;
            if (baseBleed <= 0f) return;

            foreach (var c in children)
            {
                var name = c.def.defName;
                if (name.IndexOf(Artery) >= 0 || name.IndexOf(Aorta) >= 0)
                {
                    float newBleed = c.def.bleedRate;
                    if (newBleed > 0f) __result *= newBleed / baseBleed;
                    break;
                }
            }
        }
    }
}
