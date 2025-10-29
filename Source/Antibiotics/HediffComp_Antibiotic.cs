using Verse;

namespace CombatExtendedArmorPatches
{
    public class HediffComp_Antibiotic : HediffComp
    {
        public float InfectionSlowdownFactor => (props as HediffCompProperties_Antibiotic)?.infectionSlowdownFactor ?? 0.5f;
    }
}
