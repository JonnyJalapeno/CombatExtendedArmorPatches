using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatExtendedArmorPatches
{
    [HarmonyPatch(typeof(Hediff_MissingPart), "PostAdd")]
    static class Hediff_MissingPart_WaistKillPatch
    {
        static void Postfix(Hediff_MissingPart __instance)
        {
            if (__instance?.pawn == null || __instance.pawn.Dead) return;

            var tags = __instance.Part.def?.tags;
            if (tags == null) return;

            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i].defName == "isVital")
                {
                    __instance.pawn.Kill(null);
                    break;
                }
            }
        }
    }
}

    /*public class KillNextTickComponent : WorldComponent
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
    }*/

