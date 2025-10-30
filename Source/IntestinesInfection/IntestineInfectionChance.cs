using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace CombatExtendedArmorPatches
{
    // Adds increasing infection chance based on severity for intestine injuries
    // Patch to add IntestineSpill on hit
    [HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplyDamageToPart")]
    internal static class Harmony_DamageWorker_AddInjury_ApplyDamageToPart_Intestines
    {
        private static readonly HediffDef intestineHediffDef =
            DefDatabase<HediffDef>.GetNamed("IntestineSpill", false);

        [HarmonyPostfix]
        static void Postfix(Pawn pawn, DamageWorker.DamageResult result)
        {
            if (intestineHediffDef == null || pawn == null || result?.LastHitPart == null)
                return;

            foreach (var part in result.LastHitPart.parts)
            {
                if (part?.def?.defName != "Intestines") continue;
                if (pawn.health.hediffSet.PartIsMissing(part)) continue;

                if (!pawn.health.hediffSet.hediffs.Any(h => h.def == intestineHediffDef && h.Part == part))
                {
                    var hediff = pawn.health.AddHediff(intestineHediffDef, part);
                    if (hediff != null)
                        hediff.Severity = Utils.CalculateSeverityForPart(part, pawn);
                }
            }
        }
    }

    // Hediff class that updates severity every second (60 ticks)
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

    // Comp properties for scaling infection by severity
    public class HediffCompProperties_SeverityInfectionBoost : HediffCompProperties
    {
        public float baseMultiplier = 0.1f;
        public float maxMultiplier = 0.4f;

        public HediffCompProperties_SeverityInfectionBoost()
        {
            compClass = typeof(HediffComp_SeverityInfectionBoost);
        }
    }

    // Comp logic that scales infection chance with severity
    public class HediffComp_SeverityInfectionBoost : HediffComp
    {
        private static readonly FieldInfo fi_infectionChance =
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
            if (parent.Severity > 0f) // skip if no severity to save cycles
                UpdateInfectionChance();
        }

        private void UpdateInfectionChance()
        {
            if (parent == null || parent.comps.NullOrEmpty()) return;

            var infecter = parent.comps.Find(c => c is HediffComp_Infecter) as HediffComp_Infecter;
            if (infecter == null) return;

            var propsInfecter = (HediffCompProperties_Infecter)infecter.props;
            float factor = Props.baseMultiplier + (Props.maxMultiplier - Props.baseMultiplier) * parent.Severity;
            fi_infectionChance.SetValue(propsInfecter, Mathf.Clamp01(factor));
        }
    }
    
}
