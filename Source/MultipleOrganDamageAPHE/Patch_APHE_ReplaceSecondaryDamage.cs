using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using CombatExtended;
using System.Collections.Generic;

namespace CombatExtendedArmorPatches
{
    //Adds organ damage from APHE projectiles to other organs in the same body part group
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplyToPawn")]
    [HarmonyAfter("CombatExtended.HarmonyCE")]
    public static class Patch_APHE_ReplaceSecondaryDamage
    {
        [ThreadStatic]
        private static bool _isProcessing;

        private static bool Prefix(DamageInfo dinfo, Pawn pawn, ref DamageWorker.DamageResult __result)
        {
            if (_isProcessing) return true; // prevent recursion

            var projProps = dinfo.Weapon?.projectile as ProjectilePropertiesCE;
            if (projProps == null || projProps.secondaryDamage.NullOrEmpty())
                return true;

            if (!projProps.secondaryDamage.Any(sd => sd.def.defName == "Bomb_Secondary"))
                return true;

            var part = dinfo.HitPart;
            if (part == null) return true;

            __result = new DamageWorker.DamageResult();

            _isProcessing = true;
            try
            {
                // Apply primary damage
                var mainRes = pawn.TakeDamage(dinfo);
                __result.totalDamageDealt += mainRes.totalDamageDealt;

                // Apply secondary damage
                foreach (var sec in projProps.secondaryDamage)
                {
                    if (!Rand.Chance(sec.chance)) continue;

                    // Apply to the directly hit part only once, skip in the shock propagation
                    var parent = part.parent;
                    if (parent != null)
                    {
                        var candidates = parent.GetDirectChildParts()
                                               .Where(p => p != part && p.depth == BodyPartDepth.Inside)
                                               .ToList();

                        // Randomly select 1–3 organs
                        int count = Math.Min(Rand.RangeInclusive(1, 3), candidates.Count);
                        var selected = candidates.OrderBy(x => Rand.Value).Take(count);

                        foreach (var sib in selected)
                        {
                            var sibDinfo = sec.GetDinfo(dinfo);
                            sibDinfo.SetHitPart(sib);
                            sibDinfo.SetAmount(sibDinfo.Amount * Rand.Range(0f, 0.5f));
                            pawn.TakeDamage(sibDinfo);
                        }
                    }
                }
            }
            finally
            {
                _isProcessing = false;
            }

            return false;
        }
    }
}
