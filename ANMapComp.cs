using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using RWASFilterLib;

namespace RWAutoNotify
{
    public class ANMapComp : BaseRuledMapComp<ANRule>
    {
        static string name = "RWAutoSell.TabNotify";
        int notifyticks = 0;
        public bool Notified = false;

        public override int Priority { get { return int.MinValue + 2; } }

        public ANMapComp(Map map) : base(map)
        {
            
        }

        public override string TabName { get { return name.Translate(); } }

        public static ANMapComp GetSingleton(Map map)
        {
            return map.GetComponent<ANMapComp>();
        }

        public static ANMapComp GetSingleton()
        {
            return Find.CurrentMap.GetComponent<ANMapComp>();
        }

        public override void MapComponentTick()
        {
            if (map.IsPlayerHome)
            {
                if (notifyticks < 0)
                {
                    if (!ASNotify.Notify(map, Rules, ref Notified))
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
