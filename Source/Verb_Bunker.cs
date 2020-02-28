
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Verse;

namespace Ra2Bunker
{
    public class Verb_Bunker:Verb_LaunchProjectile
    {
        private Building_Bunker bunker;
        private List<Verb> verbss;
        public override void Reset()
        {
            base.Reset();
            this.bunker = (Building_Bunker)this.caster;
        }

        public void resetVerb() {
            if (this.bunker == null)
            {
                this.bunker = (Building_Bunker)this.caster;
            }
            foreach (Pawn pawn in this.bunker.GetInner().InnerListForReading)
            {
                
                if (pawn.TryGetAttackVerb(this.currentTarget.Thing) !=null)
                {
                    //Log.Warning(pawn.TryGetAttackVerb(this.currentTarget.Thing).TryStartCastOn(this.currentTarget) + "OH" + pawn.Name);
                    pawn.TryGetAttackVerb(this.currentTarget.Thing).caster = pawn;
                }
                
            }
        }
        protected override bool TryCastShot()
        {
            verbss = new List<Verb>();
          
            if (this.bunker == null)
            {
                this.bunker = (Building_Bunker)this.caster;
            }
           foreach(Pawn pawn in this.bunker.GetInner().InnerListForReading)
            {
                if (pawn.TryGetAttackVerb(this.currentTarget.Thing) != null)
                {
                    //Log.Warning(pawn.TryGetAttackVerb(this.currentTarget.Thing).TryStartCastOn(this.currentTarget) + "OH" + pawn.Name);
                    verbss.Add(pawn.TryGetAttackVerb(this.currentTarget.Thing));
                    
                }
                else {
                  //  Log.Warning(pawn.Name+" no weapon");
                }
            }
           
            foreach (Verb vb in this.verbss) {
              
               // Thing tmpCaster = vb.caster;
                vb.caster = this.caster;
                bool fired =vb.TryStartCastOn(this.currentTarget);
               
                //vb.caster = tmpCaster;
            }
            return true;
        }
    }
}
