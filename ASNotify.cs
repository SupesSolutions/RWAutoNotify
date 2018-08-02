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
        public static bool Notify(Map map, List<ANRule> Rules, ref bool Notified)
        {
            if (Rules.Count == 0 || !Rules.All(x => x.Active)) return false;
            //you can use ASLibTransferUtility.MapTradables to retrieve a tradables list from the specified map
            List<TransferableOneWay> cachedtransferables = ASLibTransferUtility.MapTradables(map, true, TransferAsOneMode.PodsOrCaravanPacking);
            List<ANRule> templist = new List<ANRule>();
            List<Thing> looktargets = new List<Thing>();

            //Loop thorough Rules
            foreach(ANRule Rule in Rules)
            {
                //Make sure rule is active
                if (!Rule.Active) continue;
                int count = 0;
                //loop through transferables
                for (int i = 0; i < cachedtransferables.Count; i++)
                {
                    //Rule.Nodes[0].RootNode.CheckGroup
                    //if you have multiple Roots, loop thorough Rule.Nodes
                    //then just call RootNode.CheckGroup
                    //
                    //success is different than the checkgroup result,
                    //success is false when a rule doesn't have viable filter data (for instance, it has groups but no filters, or no parts active with filter data)                    
                    if (Rule.Nodes[0].RootNode.CheckGroup(cachedtransferables[i], out bool success) && success)
                    {
                        count += cachedtransferables[i].MaxCount;
                        looktargets.AddRange(cachedtransferables[i].things);
                    }
                }

                if ((Rule.NotifyUnder && count < Rule.Quantity) || (!Rule.NotifyUnder && count > Rule.Quantity))
                {
                    templist.Add(Rule);
                }

            }

            if (templist.Count != 0)
            {
                if (!Notified)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("RWAutoSell.NotifyIn".Translate());
                    sb.AppendLine();
                    foreach (ANRule rule in templist)
                    {
                        rule.Active = false;
                        sb.AppendLine(rule.ToString());
                    }

                    Find.LetterStack.ReceiveLetter("RWAutoSell.Notification".Translate(), sb.ToString(), LetterDefOf.PositiveEvent, new LookTargets(looktargets));
                    
                }
                return true;
            }
            
            return false;
        }

    }
}
