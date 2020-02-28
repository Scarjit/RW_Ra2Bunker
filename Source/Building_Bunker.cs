

using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Ra2Bunker
{
    public class Building_Bunker: Building_TurretGun, IThingHolder
    {

        public Building_Bunker()
        {
            this.innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
        }


        public bool HasAnyContents
        {
            get
            {
                return this.innerContainer.Count > 0;
            }
        }


        public Thing ContainedThing
        {
            get
            {
                return (this.innerContainer.Count != 0) ? this.innerContainer[0] : null;
            }
        }


        public bool CanOpen
        {
            get
            {
                return this.HasAnyContents;
            }
        }


        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }
        public ThingOwner<Pawn> GetInner()
        {
            return this.innerContainer;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }


        public override void TickRare()
        {
            base.TickRare();
            this.innerContainer.ThingOwnerTickRare(true);
        }


        public override void Tick()
        {
            if (this.innerContainer.Count < 1) return;
            base.Tick();
            this.innerContainer.ThingOwnerTick(true);
        }

        public virtual void Open()
        {
            if (!this.HasAnyContents)
            {
                return;
            }
            this.EjectContents();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.direc,"direc",0,false);
            Scribe_Deep.Look<ThingOwner<Pawn>>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
           
        }

      

        public override bool ClaimableBy(Faction fac)
        {
            if (this.innerContainer.Any)
            {
                for (int i = 0; i < this.innerContainer.Count; i++)
                {
                    if (this.innerContainer[i].Faction == fac)
                    {
                        return true;
                    }
                }
                return false;
            }
            return base.ClaimableBy(fac);
        }
        public virtual bool Accepts(Thing thing)
        {
            return this.innerContainer.CanAcceptAnyOf(thing, true);
        }


        public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (!this.Accepts(thing))
            {
                return false;
            }
            bool flag;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, this.innerContainer, thing.stackCount, true);
                flag = true;
            }
            else
            {
                flag = this.innerContainer.TryAdd(thing, true);
            }
            if (flag)
            {
               
                return true;
            }
            return false;
        }


        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.innerContainer.Count > 0 && (mode == DestroyMode.Deconstruct || mode == DestroyMode.KillFinalize))
            {
                
                this.EjectContents();
            }
            this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            base.Destroy(mode);
        }


        public virtual void EjectContents()
        {
            (this.AttackVerb as Verb_Bunker).resetVerb();
            this.innerContainer.TryDropAll(Toils_bunker.getEnterOutLoc(this), base.Map, ThingPlaceMode.Near, null, null);
           
        }

        // Token: 0x060024FE RID: 9470 RVA: 0x00116EF0 File Offset: 0x001152F0
        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            string str;

            //   str = this.innerContainer.ContentsString;
            str = this.innerContainer.Count +"/"+ this.maxCount;
            
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            return text + "CasketContains".Translate() + ": " + str.CapitalizeFirst()+((this.innerContainer.Count==this.maxCount)?"(Full)":"");
        }



        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(myPawn))
            {
                yield return o;
            }
            if (this.innerContainer.Count <maxCount)
            {
                if (Toils_bunker.getEnterOutLoc(this)==null)//!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    FloatMenuOption failer = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    yield return failer;
                }
                else
                {
          
                    
                        JobDef jobDef = DefDatabase<JobDef>.GetNamed("EnterRa2Bunker", true);//JobDefOf.EnterCryptosleepCasket;
                        string jobStr = "EnterRa2Bunker".Translate();
                        Action jobAction = delegate
                        {
                           
                                Job job = new Job(jobDef, this);
                                myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                            
                        };
                        yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, jobAction, MenuOptionPriority.Default, null, null, 0f, null, null), myPawn, this, "ReservedBy");
                   


                }
            }
            yield break;
        }

        // Token: 0x06002515 RID: 9493 RVA: 0x00116FCC File Offset: 0x001153CC
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }
            if (base.Faction == Faction.OfPlayer && this.innerContainer.Count > 0 )
            {
                Command_Action eject = new Command_Action();
                eject.action = new Action(this.EjectContents);
                eject.defaultLabel = "CommandPodEject".Translate();
                eject.defaultDesc = "CommandPodEjectDesc".Translate();
                if (this.innerContainer.Count == 0)
                {
                    eject.Disable("CommandPodEjectFailEmpty".Translate());
                }
                eject.hotKey = KeyBindingDefOf.Misc1;
                eject.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject", true);
                yield return eject;   
            }
            String[] direcs = { "North","East","South","West"};

            Command_Action dire = new Command_Action();
            dire.defaultLabel = "NowDirection:"+direcs[this.direc];
            dire.defaultDesc = "ClickToChangeEnterDirection";
            dire.icon = TexCommand.GatherSpotActive;
            dire.action = delegate ()
            {
                if (this.direc > 2) this.direc = 0;
                else
                this.direc++;

            };

            yield return dire;

            Command_Action resettar = new Command_Action();
            resettar.defaultLabel = "ClearTarget";
            resettar.defaultDesc = "ClickToClearTarget";
            resettar.icon = TexCommand.ClearPrioritizedWork;
            resettar.action = delegate ()
            {
                this.currentTargetInt = LocalTargetInfo.Invalid;
                this.burstWarmupTicksLeft = 0;
            };

            yield return resettar;


            yield break;
        }


        // Token: 0x040014D5 RID: 5333
        protected ThingOwner<Pawn> innerContainer;

        public int maxCount = 6;

        public int direc = 0;
    }
}
