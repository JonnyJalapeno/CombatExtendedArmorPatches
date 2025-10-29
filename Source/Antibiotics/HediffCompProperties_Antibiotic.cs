using Verse;

namespace CombatExtendedArmorPatches
{
    public class HediffCompProperties_Antibiotic : HediffCompProperties
    {
        public float infectionSlowdownFactor = 0.5f; // default 50% slower

        public HediffCompProperties_Antibiotic()
        {
            this.compClass = typeof(HediffComp_Antibiotic);
        }
    }
}
