using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(HediffComp_Infecter), "CompPostPostAdd")]
    public static class Patch_HediffComp_Infecter_CompPostPostAdd_Transpiler
    {
        // The transpiler
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt
                    && codes[i].operand is System.Reflection.MethodInfo method
                    && method.Name == "get_infectionChance")
                {
                    // Inject call to MultiplyIfIntestines
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0)); // __instance
                    codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_1)); // dinfo
                    codes.Insert(i + 3, new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Patch_HediffComp_Infecter_CompPostPostAdd_Transpiler),
                                           nameof(MultiplyIfIntestines))));
                    i += 3;
                }
            }

            return codes; // <- always return
        }

        // Must be a static method in the class
        public static float MultiplyIfIntestines(float originalChance, HediffComp_Infecter instance, DamageInfo? dinfo)
        {
            if (dinfo.HasValue && instance?.parent?.Part != null && instance.parent.Part.def.defName.Contains("Intestines"))
            {
                return originalChance * 2f;
            }
            return originalChance;
        }
    }
}
