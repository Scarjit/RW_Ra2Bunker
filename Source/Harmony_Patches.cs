using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Ra2Bunker
{
    [StaticConstructorOnStartup]
    class Harmony_Patches
    {
        static Harmony_Patches()
        {
            Log.Message("Hello World!");
            Harmony harmony = new Harmony(id: "rimworld.scarjit.ra2bunker");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(GameEnder))]
        [HarmonyPatch("CheckOrUpdateGameOver")]
        [HarmonyPrefix]
        public void CheckOrUpdateGameOver_Prefix()
        {
            List<Map> maps = Find.Maps;
            foreach (Map map in maps)
            {
                List<Thing> thingList = map.listerThings.ThingsInGroup(ThingRequestGroup.ThingHolder);
                foreach (Thing thing in thingList)
                {
                    if (thing is Building_Bunker bunker && bunker.HasAnyContents)
                    {
                        return;
                    }
                }
            }
        }
    }
}
