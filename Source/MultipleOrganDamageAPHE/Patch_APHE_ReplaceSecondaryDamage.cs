using System;
using HarmonyLib;
using RimWorld;
using Verse;
using CombatExtended;
using System.Collections.Generic;

namespace CombatExtendedArmorPatches
{
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
            if (projProps == null || projProps.secondaryDamage == null || projProps.secondaryDamage.Count == 0)
                return true;

            bool hasBombSecondary = false;
            foreach (var sd in projProps.secondaryDamage)
            {
                if (sd.def.defName == "Bomb_Secondary")
                {
                    hasBombSecondary = true;
                    break;
                }
            }
            if (!hasBombSecondary) return true;

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
                    if (pawn.Dead) break;
                    if (Rand.Chance(sec.chance))
                    {
                        var parent = part.parent;
                        if (parent == null) continue;

                        var candidates = new List<BodyPartRecord>();
                        foreach (var p in parent.GetDirectChildParts())
                        {
                            if (p != part && p.depth == BodyPartDepth.Inside)
                                candidates.Add(p);
                        }

                        if (candidates.Count == 0) continue;

                        int count = Math.Min(Rand.RangeInclusive(1, 3), candidates.Count);

                        for (int i = 0; i < count; i++)
                        {
                            int index = Rand.Range(0, candidates.Count);
                            var sib = candidates[index];
                            candidates.RemoveAt(index); // ensure unique selection

                            if (pawn.Dead) break;

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
