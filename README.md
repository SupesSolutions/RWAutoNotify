# RWAutoNotify
This is an example extension to RWAutoSeller

The fewest files required for an extension is two files, these files are Rule Storage (IRuleComp) and Rule Class (IRule)

even though AutoSeller looks for IRuleComp, inherit the abstract class BaseRuleMapComp<T> to implement most of the storage control for you. T is your IRule Class.  the property TabName must be overridden, but everything else is completely optional.  BaseRuleMapComp<T> implements most of what is required for a functional container
  
  TabName
  This gets displayed on the AutoSeller Overview Window as a button for your rules list, must be implemented
  
  Priority
  this controls the order the tabs are displayed on the overview window, higher is further right, is completely optional, but if not overridden, will default to 100
 
 
 BaseRuleMapComp<T> inherits from MapComponent, which has ticker functionality for a per map basis, since there is only one per map, it makes for great per map settings storage
  
 
 for your IRule class, inherit from BaseRule will again help with implementing a chunk of IRule, you're left with DeepCompy and the constructor as the only methods required to implement.
 
 in the Constructor be sure to add something along the following below.  you can have as many RuleRoots as you like
 Nodes_ = new List<RuleRoot> { new RuleRoot("Name of Root Here") };
 
 Active
 this can be used to suspend the rule
 
 DeepCopy
 this must be implemented, it deals with when a rule is copyed, and is also used for editing rules.  make sure Nodes_, Active_ amd RuleLabel are copied into the new rule, along with any other rule settings you want copied.
 
 DrawProperties
 this gets displayed when editing a rule, BaseRule uses the first 30f from the bottom of the Root Node Select, leaving around 60f for anything else, width can be variable depending on monitor size.
 
 
 AutoSellExportable Attributes
 
 There are three Attributes that come with AutoSeller, AutoSellExportableValue, AutoSellExportableList and AutoSellExportableDeep.  These control how importing is done from old saves, use this on any settings in your IRule class that you save in rimworld with ExposeData.  All AutoSellExportable attributes have a SaveName, make this the same as the name you use in IExposable.ExposeData and can only be used on Fields.
 
 AutoSellExportableValue
 used on value types as well as strings (even though string is a reference type), contains DefaultValue which can be used to specify the value it can have it it isn't present in the save file.
 
 AutoSellExportableList
 used on Lists, though I haven't tried it yet, it may also work on other List Types (such as Hash Lists) but certain lists like dictionarys are not supported at this time.
 
 AutoSellExportableDeep
 can be used on classes, these classes MUST use AutoSellExportable attributes on their own fields that are saved through IExposable.ExposeData
