using RimWorld;
using Verse;
using HarmonyLib;

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
}

    



