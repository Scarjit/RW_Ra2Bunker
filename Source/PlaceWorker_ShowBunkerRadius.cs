using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Ra2Bunker
{
    public class PlaceWorker_ShowBunkerRadius : PlaceWorker
    {

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {

            Map currentMap = Find.CurrentMap;
            GenDraw.DrawFieldEdges(Toils_bunker.getAllEnterOutLoc(center), Color.magenta);
        }
    }
}
