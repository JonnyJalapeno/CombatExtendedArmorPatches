using HarmonyLib;
using Verse;
using CombatExtended.AI;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(CompUrgentWeaponPickup), nameof(CompUrgentWeaponPickup.Notify_BulletImpactNearBy))]
    static class Patch_CompUrgentWeaponPickup_Notify_BulletImpactNearBy
    {
        static bool Prefix(CompUrgentWeaponPickup __instance)
        {
            var pawn = __instance.SelPawn;
            if (pawn == null || pawn.Dead || pawn.Map == null)
                return false; // skip CE logic if pawn invalid
            return true; // allow original
        }
    }
}