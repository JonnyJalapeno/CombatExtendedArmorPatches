using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
   public static class Utils
    {
        public static float CalculateSeverityForPart(BodyPartRecord part, Pawn pawn)
        {
            if (pawn == null || part == null) return 0f;

            float totalDamage = 0f;
            var hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i] is Hediff_Injury injury && injury.Part == part)
                    totalDamage += injury.Severity;
            }

            float maxHealth = part.def.GetMaxHealth(pawn);
            return (maxHealth > 0f) ? totalDamage / maxHealth : 0f;
        }
    }
}


