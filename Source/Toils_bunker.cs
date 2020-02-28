using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.AI;

namespace Ra2Bunker
{
    public static class Toils_bunker
    {

        public static Toil GotoThing(TargetIndex ind, PathEndMode peMode)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                actor.pather.StartPath(getBunkerNearCell(actor, actor.jobs.curJob.GetTarget(ind),ind,peMode), peMode);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDespawnedOrNull(ind);
            return toil;
        }


        private static LocalTargetInfo getBunkerNearCell(Pawn pawn,LocalTargetInfo bunker, TargetIndex ind, PathEndMode peMode)
        {
            Map map = bunker.Thing.Map;
            IntVec3 center = bunker.Cell;
            Building_Bunker bunk = (Building_Bunker)bunker.Thing;
            if (bunk == null) return null;
            int direc = bunk.direc;
            List<IntVec3> cells = new List<IntVec3>();
            for(int i = -2; i < 3; i++)
            {
                for(int j = -2; j < 3; j++)
                {

                    if ((Math.Abs(i) > 1 || Math.Abs(j) > 1) && (Math.Abs(i * j) == 2))
                    {

                        if ((direc == 0 && j > 0) || (direc == 1 && i > 0) || (direc == 2 && j < 0) || (direc == 3 && i < 0))
                        {
                            IntVec3 cel = new IntVec3(center.x + i, center.y, center.z + j);
                          
                             if (canOut(cel,map))
                               cells.Add(cel);
                        }
                    }
                }
            }

            return cells.RandomElement();

        }

        public static IntVec3 getEnterOutLoc(Building_Bunker bunker)
        {
            Map map = bunker.Map;
            IntVec3 center = bunker.Position;
            int direc = bunker.direc;
            List<IntVec3> cells = new List<IntVec3>();
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if ((Math.Abs(i) > 1 || Math.Abs(j) > 1)&&(Math.Abs(i*j)!=4))
                    {
                        if ((direc == 0 && j > 0) || (direc == 1 && i > 0) || (direc == 2 && j < 0) || (direc == 3 && i < 0))
                        {
                            IntVec3 cel = new IntVec3(center.x + i, center.y, center.z + j);
                            if(canOut(cel,map))
                                cells.Add(cel);
                        }
                    }
                }
            }
            return cells.RandomElement();
        }

        public static List<IntVec3> getAllEnterOutLoc(IntVec3 bunker)
        {
           
            IntVec3 center = bunker;
          
            List<IntVec3> cells = new List<IntVec3>();
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if ((Math.Abs(i) > 1 || Math.Abs(j) > 1) && (Math.Abs(i * j) != 4))
                    {
                        //if ((direc == 0 && j > 0) || (direc == 1 && i > 0) || (direc == 2 && j < 0) || (direc == 3 && i < 0))
                        {
                            IntVec3 cel = new IntVec3(center.x + i, center.y, center.z + j);
                            if(canOut(cel,Find.CurrentMap))
                            cells.Add(cel);
                        }
                    }
                }
            }
            return cells;
        }



        private static bool canOut(IntVec3 cell,Map map)
        {
            if (map.thingGrid.ThingsListAt(cell).FindAll(x => x.def.passability == Traversability.Impassable).Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}
