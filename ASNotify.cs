using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RWASFilterLib;

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
            //you can use ASLibTransferUtility.MapTradables to retrieve a tradables list from the specified map
            List<TransferableOneWay> cachedtransferables = ASLibTransferUtility.MapTradables(mapcomp.map, true, TransferAsOneMode.PodsOrCaravanPacking);
            CacheRuleMatchList = new List<ANRule>();
            CacheLookList = new HashSet<Thing>();

            //Loop thorough Rules, here this is done using the GetMatchedTradables lib function, this can only be done if the mapcomp inherits from BaseRuledMapComp<>
            //Any actions we want to do during are done through delegate methods TransferableMatched and DoOnRule
            ASLibTransferUtility.GetMatchedTradables(cachedtransferables, mapcomp, 0, TransferableMatched, DoOnRule);      

            if (CacheRuleMatchList.Count != 0)
            {
                //don't trigger a notification if already notified earlier
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

                //allow GC to free memory
                CacheLookList = null;
                CacheRuleMatchList = null;

                return true;
            }

            //allow GC to free memory
            CacheLookList = null;
            CacheRuleMatchList = null;

            return false;
        }

    }
}
