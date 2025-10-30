using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(Hediff_MissingPart))]
    [HarmonyPatch("PostAdd")]
    static class Hediff_MissingPart_WaistKillPatch
    {
        static void Postfix(Hediff_MissingPart __instance)
        {
            if (!__instance.pawn.Dead && __instance.Part.def.tags.Any(t => t.defName == "isVital"))
            {
                Find.World.GetComponent<KillNextTickComponent>()
                    .ScheduleKill(__instance.pawn);
            }
        }
    }

    public class KillNextTickComponent : WorldComponent
    {
        private readonly System.Collections.Generic.List<Pawn> queue =
            new System.Collections.Generic.List<Pawn>();

        public KillNextTickComponent(World world) : base(world) { }

        public override void WorldComponentTick()
        {
            if (queue.Count == 0) return;

            for (int i = queue.Count - 1; i >= 0; i--)
            {
                var pawn = queue[i];
                if (pawn != null && !pawn.Dead)
                    pawn.Kill(null);
                queue.RemoveAt(i);
            }
        }

        public void ScheduleKill(Pawn pawn)
        {
            if (pawn != null && !pawn.Dead)
                queue.Add(pawn);
        }
    }
}
