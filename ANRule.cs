using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RWASFilterLib;
using RWASWidgets;
using RimWorld.Planet;
using RimWorld;

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
        [AutoSellExportableList(SaveName = "Chain")]
        protected List<ANRule> RuleChain_ = new List<ANRule>();

        public override IEnumerable<IRule> RuleChain()
        {
            foreach (ANRule rule in RuleChain_)
            {
                yield return rule;
            }

            yield break;
        }

        public override void ChainedAdd(IRule Rule)
        {
            RuleChain_.Add((ANRule)Rule);
        }

        public override void ChainedReplace(IRule Origin, IRule Replacement)
        {
            if (Origin is ANRule o && Replacement is ANRule r)
            {
                int index = RuleChain_.IndexOf(o);
                if (index == -1)
                {
                    RuleChain_.Add(r);
                }
                RuleChain_[index] = r;
            }
        }

        public override void ChainedInsert(int i, IRule Rule)
        {
            if (Rule is ANRule r)
                RuleChain_.Insert(i, r);
        }

        public override void ChainedRemove(IRule Rule)
        {
            if (Rule is ANRule r)
                RuleChain_.Remove(r);
        }

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
                IsChain_ = IsChain_,
                RuleChain_ = (from rc in RuleChain_ select rc.DeepCopy() as ANRule).ToList(),
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


        public ANRule(TradeRequestComp Comp) : this()
        {
            Quant_ = Comp.requestCount;
            NotifyUnder_ = false;

            ThingDef def = Comp.requestThingDef;            
            RuleLabel = "Generated: " + Comp.CompInspectStringExtra();

            //Get type via label
            Type t = ASLibMod.GetSingleton.GetBaseFilters.First(x => x.GetType().FullName == "RWAutoSell.Filters.FilterCat").GetType();    //.First(x. => x.Label == "RWAutoSell.FilterCat".Translate()).GetType();

            //create filtercontainer using type, and populate data
            FilterContainer cat = new FilterContainer(t)
            {
                FilterData = new List<string>() { "thg." + def.defName }
            };

            Nodes_[0].RootNode.Filters.Add(cat);

            if (def.HasComp(typeof(CompQuality)))
            {
                //and another type/container pair for quality, underlying byte value for normal is '2'
                Type t2 = ASLibMod.GetSingleton.GetBaseFilters.First(x => x.GetType().FullName == "RWAutoSell.Filters.FilterQuality").GetType();
                //Type t2 = ASLibMod.GetSingleton.GetBaseFilters.First(x => x.Label == "Quality".Translate()).GetType();

                List<string> data = new List<string>();
                foreach (QualityCategory qc in QualityUtility.AllQualityCategories)
                {
                    if ((int)qc >= 2)
                    {
                        data.Add(((byte)qc).ToString());
                    }

                }

                FilterContainer qlt = new FilterContainer(t2)
                {
                    FilterData = data
                };
                Nodes_[0].RootNode.Filters.Add(qlt);
            }

            if (def.IsApparel)
            {
                Type t3 = ASLibMod.GetSingleton.GetBaseFilters.First(x => x.GetType().FullName == "RWAutoSell.Filters.FilterApparel").GetType();
                
                List<string> data = new List<string>();
                data.Add("tnt.1");

                FilterContainer app = new FilterContainer(t3)
                {
                    FilterData = data
                };
                Nodes_[0].RootNode.Filters.Add(app);

            }
            


            

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
            Scribe_Collections.Look(ref RuleChain_, "Chain");

            if (Nodes_ == null || Nodes_.Count == 0)
            {
                Nodes_ = new List<RuleRoot> { new RuleRoot("RWAutoSell.Notification".Translate()) };
            }

            if (RuleChain_ == null)
            {
                RuleChain_ = new List<ANRule>();
            }
        }       
    }
}
