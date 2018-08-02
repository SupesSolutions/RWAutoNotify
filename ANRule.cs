﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RWASFilterLib;
using RWASWidgets;

namespace RWAutoNotify
{
    public class ANRule : BaseRule, IExposable
    {
        [AutoSellExportableValue(SaveName = "NotifyUnder", DefaultValue = true)]
        private bool NotifyUnder_ = true;
        [AutoSellExportableValue(SaveName = "Quantiy")]
        private int Quant_;
        private string QCache;        

        public bool NotifyUnder
        {
            get { return NotifyUnder_; }
        }

        public int Quantity
        {
            get { return Quant_; }
        }

        public override IRule DeepCopy()
        {
            ANRule temp = new ANRule
            {
                Nodes_ = (from el in Nodes_ select el.DeepCopy()).ToList(),
                Active_ = Active_,
                RuleLabel = RuleLabel,
                NotifyUnder_ = NotifyUnder_,
                Quant_ = Quant_,                        
            };

            return temp;
        }

        public ANRule()
        {
            Nodes_ = new List<RuleRoot> { new RuleRoot("RWAutoSell.Notification".Translate()) };
        }


        public override void DrawProperties(Rect inrect)
        {
            base.DrawProperties(inrect);
            Rect tablerect = new Rect(inrect.x, inrect.y + 30f, inrect.width, 30f);
            ASTable table = new ASTable(tablerect, 5, 1, 5, 5, true);
            ASWidgets.AlignedTextBox<int>(table.GetRectangle(0, 0), "RWAutoSell.Quantity".Translate(), ref Quant_, ref QCache, 0, int.MaxValue);

            if (Widgets.RadioButtonLabeled(table.GetRectangle(1, 0), "RWAutoSell.NotifyUnder".Translate(), NotifyUnder_))
            {
                NotifyUnder_ = true;
            }

            if(Widgets.RadioButtonLabeled(table.GetRectangle(2, 0), "RWAutoSell.NotifyOver".Translate(), !NotifyUnder_))
            {
                NotifyUnder_ = false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look(ref Quant_, "Quantiy", 0);
            Scribe_Values.Look(ref NotifyUnder_, "NotifyUnder", true);
            
            if (Nodes_ == null || Nodes_.Count == 0)
            {
                Nodes_ = new List<RuleRoot> { new RuleRoot("RWAutoSell.Notification".Translate()) };
            }
        }       
    }
}