using RimWorld;
using Verse;
using HarmonyLib;
using System;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplyDamageToPart")]
    internal static class Harmony_DamageWorker_AddInjury_ApplyDamageToPart_Intestines
    {
        private static readonly HediffDef IntestineHediffDef =
            DefDatabase<HediffDef>.GetNamed("IntestineSpill", false);

        [HarmonyPostfix]
        static void Postfix(Pawn pawn, DamageWorker.DamageResult result)
        {
            if (IntestineHediffDef == null || pawn == null || result?.LastHitPart?.parts == null)
                return;

            var hediffSet = pawn.health?.hediffSet;
            if (hediffSet == null) return;

            foreach (var part in result.LastHitPart.parts)
            {
                if (part?.def?.defName != "Intestines") continue;
                if (hediffSet.PartIsMissing(part)) continue;

                bool hasHediff = false;
                foreach (var h in hediffSet.hediffs)
                {
                    if (h.def == IntestineHediffDef && h.Part == part)
                    {
                        hasHediff = true;
                        break;
                    }
                }
                if (hasHediff) continue;

                var hediff = pawn.health.AddHediff(IntestineHediffDef, part);
                if (hediff != null)
                    hediff.Severity = Utils.CalculateSeverityForPart(part, pawn);
            }
        }
    }

    public class Hediff_IntestineSpill : HediffWithComps
    {
        private float maxHealth;
        private int tickCounter;

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (pawn != null && Part != null)
                maxHealth = Part.def.GetMaxHealth(pawn);
        }

        public override void PostTick()
        {
            base.PostTick();
            if (pawn == null || Part == null) return;

            if (++tickCounter < 60) return;
            tickCounter = 0;

            Severity = Utils.CalculateSeverityForPart(Part, pawn);
        }
    }

    public class HediffCompProperties_SeverityInfectionBoost : HediffCompProperties
    {
        public float baseMultiplier = 0.1f;
        public float maxMultiplier = 0.4f;

        public HediffCompProperties_SeverityInfectionBoost()
        {
            compClass = typeof(HediffComp_SeverityInfectionBoost);
        }
    }

    public class HediffComp_SeverityInfectionBoost : HediffComp
    {
        private static readonly System.Reflection.FieldInfo fi_infectionChance =
            typeof(HediffCompProperties_Infecter).GetField("infectionChance",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        public HediffCompProperties_SeverityInfectionBoost Props => (HediffCompProperties_SeverityInfectionBoost)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            UpdateInfectionChance();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (parent.Severity > 0f)
                UpdateInfectionChance();
        }

        private void UpdateInfectionChance()
        {
            if (parent == null || parent.comps.NullOrEmpty()) return;

            HediffComp_Infecter infecter = null;
            foreach (var c in parent.comps)
            {
                if (c is HediffComp_Infecter ic)
                {
                    infecter = ic;
                    break;
                }
            }
            if (infecter == null) return;

            var propsInfecter = (HediffCompProperties_Infecter)infecter.props;
            float factor = Props.baseMultiplier + (Props.maxMultiplier - Props.baseMultiplier) * parent.Severity;
            float clampedFactor = Math.Max(0f, Math.Min(1f, factor));
            fi_infectionChance.SetValue(propsInfecter, clampedFactor);
        }
    }
}
