using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using RWASFilterLib;

namespace RWAutoNotify
{
    /// <summary>
    /// This class acts as a container for your rules, you can also place map specific tickers to
    /// process the rules in here, must be public and inherit from BaseRuleMapComp and use the rule type as the generic
    /// </summary>
    public class ANMapComp : BaseRuledMapComp<ANRule>
    {
        static string name = "RWAutoSell.TabNotify";
        int notifyticks = 0;
        public bool Notified = false;

        /// <summary>
        /// controls the order of how rule types appear, higher number appear further right
        /// </summary>
        public override int Priority { get { return int.MinValue + 2; } }

        /// <summary>
        /// if overridden, it shows an additional button next to the create button
        /// </summary>
        public override string AdditonalName
        {
            get
            {
                return "RWAutoSell.NotifyTradeRequests".Translate();
            }
        }

        /// <summary>
        /// Controls what window is show by the additional button
        /// </summary>
        public override Window AdditionalWindow()
        {
            return new ANWizDialog(map);
        }

        public ANMapComp(Map map) : base(map)
        {
            
        }

        /// <summary>
        /// This is what will be shown on the overview
        /// </summary>
        public override string TabName { get { return name.Translate(); } }

        public static ANMapComp GetSingleton(Map map)
        {
            return map.GetComponent<ANMapComp>();
        }

        public static ANMapComp GetSingleton()
        {
            return Find.CurrentMap.GetComponent<ANMapComp>();
        }

        /// <summary>
        /// this method can be used to process your rules
        /// </summary>
        public override void MapComponentTick()
        {
            if (map.IsPlayerHome)
            {
                if (notifyticks < 0)
                {
                    if (!ASNotify.Notify(this, Rules, ref Notified))
                        Notified = false;
                    notifyticks = 3600;
                }
                notifyticks--;
            }

        }

        public override void ExposeData()
        {
            base.ExposeData();
        }  
    }
}
