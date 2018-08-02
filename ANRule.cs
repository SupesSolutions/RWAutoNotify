using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RWASFilterLib;
using RWASWidgets;

namespace RWAutoNotify
{
    /// <summary>
    /// should always inherit from BaseRule or IRule
    /// This is the rule type    
    /// </summary>
    public class ANRule : BaseRule, IExposable
    {
        // AutoSellExportable attributes are used to control how data is imported from old saves, match up the attribs on the data you save through ExposeData
        // AutoSellExportableValue is used for value types or strings, you can also include a default value.
        // AutoSellExportableList can be used for some list types
        // AutoSellExportableDeep can be used on custom classes, but make sure that those classes have approriate AutoSellExportable attributes on data within those classes
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

        /// <summary>
        /// must be included, used for copying nodes/groups
        /// 
        /// should also include inherited members Nodes_, Active_ and RuleLabel
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Initalise how many root nodes you want to use here, along with their name using a list of RuleRoot objects
        /// 
        /// </summary>
        public ANRule()
        {
            Nodes_ = new List<RuleRoot> { new RuleRoot("RWAutoSell.Notification".Translate()) };
        }

        /// <summary>
        /// draw your rule options here.  the first 30 points are reserved for Rule Description, leaving around 60 points height to play with
        /// </summary>
        /// <param name="inrect"></param>
        public override void DrawProperties(Rect inrect)
        {
            base.DrawProperties(inrect);
            Rect tablerect = new Rect(inrect.x, inrect.y + 30f, inrect.width, 30f);
            // here I use an ASTable object to segment each option into its own cell without having to control the width manually,
            // use a reference to RWASWidgets from RWAutoSeller if you wish to use it
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
