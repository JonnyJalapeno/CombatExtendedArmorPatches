/*using System;
using HarmonyLib;
using Verse;

namespace CombatExtendedArmorPatches
{
    // Defensive finalizer: catch and log exceptions bubbling out of CombatExtended's
    // tactical notify path so they don't spam the RimWorld error log and cascade.
    // This is a temporary mitigation; the root cause should be fixed in CombatExtended.
    [HarmonyPatch(typeof(CombatExtended.CompTacticalManager), "Notify_BulletImpactNearby")]
    public static class CompTacticalManager_Notify_BulletImpactNearby_Patch
    {
        // Harmony finalizer can return an Exception. Returning null swallows the exception.
        static Exception Finalizer(Exception __exception)
        {
            if (__exception == null) return null;

            // Log once to avoid flooding logs
            try
            {
                Log.ErrorOnce("[CombatExtendedArmorPatches] Caught exception in CombatExtended.CompTacticalManager.Notify_BulletImpactNearby: " + __exception.ToString(), 17654321);
            }
            catch
            {
                // Swallow any logging issues
            }

            // Swallow the exception so it doesn't bubble to the engine tick loop.
            return null;
        }
    }
}*/
