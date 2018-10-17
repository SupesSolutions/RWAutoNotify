using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using RimWorld.Planet;


namespace RWAutoNotify
{
    public class ANWizDialog : Window
    {
        ASListBox<TradeRequestComp> CompBox;
        private readonly List<TradeRequestComp> Comps = new List<TradeRequestComp>();
        Map map;


        public ANWizDialog(Map map)
        {
            this.absorbInputAroundWindow = true;
            this.forcePause = true;
            this.map = map;
            CompBox = new ASListBox<TradeRequestComp>(25, 4, false, false);
            //get list of possible trade request quests
            Comps = ASNotify.GetRequests();
            //populate ASListBox with trade comps, using custom label and suppressing any further label changes
            foreach (TradeRequestComp comp in Comps)
            {
                CompBox.Add(new SelectedItem<TradeRequestComp>() { Item = comp, label = comp.requestCount.ToString() + " " + comp.requestThingDef.LabelCap, surpresslabelupdate = true } );
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            //create a 2 by 1 table
            ASTable table = new ASTable(inRect,2,1, 4, 0);

            Rect selectable = table.GetRectangle(0, 0);

            //seperate bottom part for cancel button
            Rect cancelrect = selectable.BottomPartPixels(30f);

            selectable.height -= 30f;

            CompBox.DrawListBox(selectable);

            if (Widgets.ButtonText(cancelrect, "Close".Translate()))
            {
                Close();
            }

            //if something is selected, get comp, create label;
            if (CompBox.GetSelectedIndex != -1)
            {
                TradeRequestComp tc = CompBox.GetSelected;
                Widgets.Label(table.GetRectangle(1, 0),

                    "RWAutoSell.NotifySummary".Translate(tc.parent.Faction.Name, tc.requestCount + "x " + tc.requestThingDef.LabelCap, tc.rewards.ContentsString)

                    //"From: " + Environment.NewLine + tc.parent.Faction.Name + Environment.NewLine + Environment.NewLine +
                    //"Request: " + Environment.NewLine + tc.requestCount + "x " + tc.requestThingDef.LabelCap + Environment.NewLine + Environment.NewLine +
                    //"Reward: " + Environment.NewLine + tc.rewards.ContentsString
                    );

                //create rule, add rule to comp, notify Maintab needs a refresh, close dialog
                if(Widgets.ButtonText(table.GetRectangle(1, 0).BottomPartPixels(30f), "Create"))
                {
                    ANRule newrule = new ANRule(CompBox.GetSelected);
                    map.GetComponent<ANMapComp>().Add(newrule);
                    ASLibMod.GuiRefresh = true;
                    Close();
                }
            }


        }
    }
}
