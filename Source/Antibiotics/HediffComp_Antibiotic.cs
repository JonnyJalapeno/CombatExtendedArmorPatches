using Verse;

namespace CombatExtendedArmorPatches
{
    public class HediffComp_Antibiotic : HediffComp
    {
        private HediffCompProperties_Antibiotic cachedProps;
        private bool cached;

        public float InfectionSlowdownFactor
        {
            get
            {
                if (!cached)
                {
                    cachedProps = props as HediffCompProperties_Antibiotic;
                    cached = true;
                }
                return cachedProps != null ? cachedProps.infectionSlowdownFactor : 0.5f;
            }
        }
    }
}