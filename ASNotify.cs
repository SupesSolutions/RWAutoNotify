using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RWASFilterLib;
using RimWorld.Planet;

namespace RWAutoNotify
{
    static class ASNotify
    {
        private static HashSet<Thing> CacheLookList = null;        
        private static List<ANRule> CacheRuleMatchList = null;
        static int CacheCount = 0;

        /// <summary>
        /// used as a delegate for GetMatchedTradables, this method adds a rule if it matches.
        /// It also adds all Things within the transferables onto the look list that matched.
        /// 
        /// Worth noting that GetMatchedTradables ActionOnRule runs whether matched or not, but the Transferables list would be empty if no matches occured.
        /// </summary>
        /// <param name="Transferables"></param>
        /// <param name="Rule"></param>
        public static void DoOnRule(List<TransferableOneWay> Transferables, ANRule Rule)
        {
            if ((Rule.NotifyUnder && CacheCount < Rule.Quantity) || (!Rule.NotifyUnder && CacheCount > Rule.Quantity))
            {
                CacheRuleMatchList.Add(Rule);
                CacheLookList.AddRange((from el in Transferables from li in el.things select li).ToList());
            }
            CacheCount = 0;
        }

        /// <summary>
        /// used as a delegate for GetMatchedTradables, this method simply increments if a transferable matches a rule.
        /// 
        /// unlike ActionOnRule delegates, ActionOnTransferable delegates only runs if a match occurs
        /// </summary>
        /// <param name="Transferable"></param>
        /// <param name="Rule"></param>
        public static void TransferableMatched(TransferableOneWay Transferable, ANRule Rule)
        {
            CacheCount += Transferable.MaxCount;            
        }

        public static bool Notify(ANMapComp mapcomp, List<ANRule> Rules, ref bool Notified)
        {
            
            if (Rules.Count == 0 || !Rules.All(x => x.Active)) return false;
            //you can use ASLibTransferUtility.MapTradables to retrieve a transferable list from the specified map
            List<TransferableOneWay> cachedtransferables = ASLibTransferUtility.MapTradables(mapcomp.map, true, TransferAsOneMode.PodsOrCaravanPacking);
            CacheRuleMatchList = new List<ANRule>();
            CacheLookList = new HashSet<Thing>();

            //This call to ASLibTransferUtility.GetMatchedTradables Loops thorough Rules, the function also deals with Inactive rules 
            //this function can only be used if the MapComponent passed in inherits from BaseRuledMapComp<>
            //Any actions we want to do during are done through delegate methods TransferableMatched and DoOnRule,
            //lambda expression are also a viable option using (T, I) => {} and (List<T>, I) => {}
            ASLibTransferUtility.GetMatchedTradables(cachedtransferables, mapcomp, 0, TransferableMatched, DoOnRule);

            
            



            if (CacheRuleMatchList.Count != 0)
            {
                //don't trigger a notification if already notified earlier, causes doubles number of ticks til next letter
                if (!Notified)
                {
                    //create string for letter to display, include information like which rule triggered
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("RWAutoSell.NotifyIn".Translate());
                    sb.AppendLine();
                    foreach (ANRule rule in CacheRuleMatchList)
                    {
                        rule.Active = false;
                        sb.AppendLine(rule.ToString());
                    }
                    //display letter
                    Find.LetterStack.ReceiveLetter("RWAutoSell.Notification".Translate(), sb.ToString(), LetterDefOf.PositiveEvent, new LookTargets(CacheLookList));                    
                }

                ResetLists();                

                return true;
            }

            ResetLists();

            return false;
        }

        private static void ResetLists()
        {
            //allow GC to free memory
            CacheLookList = null;
            CacheRuleMatchList = null;
        }

        public static List<TradeRequestComp> GetRequests()
        {
            //List<ANRule> possiblerules = new List<ANRule>();
            List<TradeRequestComp> comps = new List<TradeRequestComp>();
            //ANRule rule = new ANRule();

            foreach (WorldObject wo in Find.WorldObjects.AllWorldObjects)
            {
                TradeRequestComp tr = wo.GetComponent<TradeRequestComp>();
                if (tr != null && tr.ActiveRequest)
                {
                    comps.Add(tr); //new ANRule(tr));


                }
               

            }

            return comps;


        }



    }
}
