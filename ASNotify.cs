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

            List<TransferableOneWay> cachedtransferables = ASLibTransferUtility.MapTradables(map, true, TransferAsOneMode.PodsOrCaravanPacking);
            List<ANRule> templist = new List<ANRule>();
            List<Thing> looktargets = new List<Thing>();
            foreach(ANRule Rule in Rules)
            {
                if (!Rule.Active) continue;
                int count = 0;
                for (int i = 0; i < cachedtransferables.Count; i++)
                {
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
