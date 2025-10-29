using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
    public static class CarotidUtils
    {
        public static float CalculateSeverityForPart(BodyPartRecord part, Pawn pawn)
        {
            float totalDamage = 0f;
            foreach (var hd in pawn.health.hediffSet.hediffs)
            {
                if (hd is Hediff_Injury injury && injury.Part == part)
                    totalDamage += injury.Severity;
            }

            float maxHealth = part.def.GetMaxHealth(pawn);
            return (maxHealth <= 0f) ? 0f : totalDamage / maxHealth;
        }
    }
}
