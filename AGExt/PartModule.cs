using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace ActionGroupsExtended
{
    
    public class ModuleAGExtData : PartModule
    {


        [KSPField(isPersistant = true, guiActive = false)]
        public string AGXData; //Serialzed string of actions and action groups

        [KSPField(isPersistant = true, guiActive = false)]
        public string AGXNames; //Serialized string of group names, only the root part matters but all parts get it to handle docking ships

        [KSPField(isPersistant = true, guiActive = false)]
        public string AGXKeySet; //current key set of ship, only used by rootpart

        [KSPField(isPersistant = true, guiActive = false)]
        public bool AGXLoaded = false; //OnSave can run before OnLoad has finished at times, error trap this

        [KSPField(isPersistant = true, guiActive = false)]
        public string AGXGroupStates; //string to save toggle state and group show state, first bit toggle, next 5 are show in those groups group names tie to key set, add to keyset window

        [KSPField(isPersistant = true, guiActive = false)]
        public string AGXGroupStateNames; //string to save names of group names

        public List<BaseAction> partAllActions; //List of all actions on part, generate at load

        public List<AGXAction> partAGActions; //list of Actions assigned to action groups

        public Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>(); //??

        public int partCurrentKeySet = 0; //working key set
        public int partAGLastCount = 0; 
        public bool CallBacksSet = false; //callbacks for add/removing parts in VAB.
      

    
         


    public string SaveActionGroups() 
    {
        
        try{//print("start save action groups " +this.part.ConstructID);
       
       //KSPActs[1] = KSPActionGroup.Custom01; //setup for saving to default action groups
       // KSPActs[2] = KSPActionGroup.Custom02;
       // KSPActs[3] = KSPActionGroup.Custom03;
       // KSPActs[4] = KSPActionGroup.Custom04;
       // KSPActs[5] = KSPActionGroup.Custom05;
       // KSPActs[6] = KSPActionGroup.Custom06;
       // KSPActs[7] = KSPActionGroup.Custom07;
       // KSPActs[8] = KSPActionGroup.Custom08;
       // KSPActs[9] = KSPActionGroup.Custom09;
       // KSPActs[10] = KSPActionGroup.Custom10;
        
        
        //foreach (BaseAction clrAct in partAllActions)//actual code to save to default action groups
        //    {
        //        for (int i = 1; i <= 10; i = i + 1)
        //        {
        //            //clrAct.actionGroup = clrAct.actionGroup &= KSPActs[i];  //actiongrouptest
        //        }
        //    }
        if (partAGActions.Count >= 1 && HighLogic.LoadedSceneIsEditor) //there is an action assigned to this part
        {
            
            //print("AGCnt " + partAGActions.ElementAt(0).prt.partInfo.name + " " +partAGActions.ElementAt(0).ba.name + " " +partAGActions.ElementAt(0).group.ToString() + " " +partAGActions.ElementAt(0).activated);
            //print("A");
            try
            {
                if (partAGActions.ElementAt(0).prt == null) //make sure we don't have an empty list, can happen if actions ar assigned and then all are deleted
                {

                }
                //print("not null");
            }
                catch //empty list with a null, refresh list
            {
                    //print("null?");
                partAGActions.Clear();
                partAGActions.AddRange(AGXEditor.AttachAGXPart(this.part, partAllActions, partAGActions));
                AGXEditor.NeedToLoadActions = true;
                
            }
            //print("b");
        }
        //print("part cnt " + partAGActions.Count);
            string SaveGroupsString = ""; //reset save string to blank
            
            //print(partAGActions.Count);

           // print("c");
            foreach (AGXAction agAct in partAGActions)
            {
                //print("d");
                //if (agAct == null)
                //{
                //    print("nell");
                //    goto BreakOut2;
                //}
                
                //print(this.part.symmetryCounterparts.Count);
                //foreach (Part p in this.part.symmetryCounterparts)
                //{
                //    print(p.name);
                //}
                //if(HighLogic.LoadedSceneIsFlight)
                //{
                //    if (AGXFlight.CurrentVesselActions.First(p => p.group == agAct.group).activated)
                //    {
                //        agAct.activated = true;
                        
                //    }
                //    else
                //    {
                //        agAct.activated = false;
                       
                //    }
                //    }
                SaveGroupsString = SaveGroupsString + '\u2023' + agAct.group.ToString("000"); //\u2023 is divider character (right arrow), make sure actiongroup number is 3 characters
             
                    if(agAct.activated==true) //is actiongroup activaed? then add 1, else add 0 to string
                    {
                     
                        SaveGroupsString = SaveGroupsString + "1";
                       
                    }
                    else
                    {
                       
                        SaveGroupsString = SaveGroupsString + "0";
                        
                    }

                    SaveGroupsString = SaveGroupsString + agAct.ba.name; //action name added to save string
                   //foreach (BaseAction ba2 in partAllActions)
                   //{
                       
                       if (agAct.ba.listParent.module.moduleName == "ModuleEnviroSensor") //add this to the agxactions list somehow and add to save.load serialze
                       {
                           ModuleEnviroSensor MSE = (ModuleEnviroSensor)agAct.ba.listParent.module;
                           SaveGroupsString = SaveGroupsString + '\u2020' + MSE.sensorType; //u2020 is envirosensor
                       }
                       else if (agAct.ba.listParent.module.moduleName == "ModuleScienceExperiment") //add this to the agxactions list somehow and add to save.load serialze
                       {
                           ModuleScienceExperiment MSE = (ModuleScienceExperiment)agAct.ba.listParent.module; //all other modules use guiname
                           SaveGroupsString = SaveGroupsString + '\u2022' +  MSE.experimentID; //u2021 is sciencemodule
                       }
                        else //if (agAct.ba.listParent.module.moduleName == "ModuleScienceExperiment") //add this to the agxactions list somehow and add to save.load serialze
                       {
                           //ModuleScienceExperiment MSE = (ModuleScienceExperiment)agAct.ba.listParent.module; //all other modules use guiname
                           SaveGroupsString = SaveGroupsString + '\u2021' +  agAct.ba.guiName; //u2021 is sciencemodule
                       }
                       //else if (agAct.ba.listParent.module.moduleName == "KethaneConverter") //add this to the agxactions list somehow and add to save.load serialze
                       //{
                       //   // print("Kethane converter found");
                       //    //foreach (BaseAction ba3 in agAct.ba.listParent.module.Actions)
                       //    //{
                       //    //    print("K1 " + ba3.guiName);
                       //    //}
                       //    //ModuleEnviroSensor MSE = (ModuleEnviroSensor)agAct.ba.listParent.module;
                       //    SaveGroupsString = SaveGroupsString + '\u2032' + agAct.ba.guiName; //u2021 is kethaneconverter
                       //}
                       //else
                       //{

                       //    SaveGroupsString = SaveGroupsString + '\u2022'; //all other modules on code u2022
                       //}
                   //}
                   //if (agAct.group <= 10) //will be where actions are saved to default action groups, currently not working
                  // {
                       //agAct.ba.actionGroup = (agAct.ba.actionGroup |= KSPActs[agAct.group]); //actiongrouptest
                   //}
                //default action groups set code is in SelParts window in Flight and Editor classes
                   
                }
        //BreakOut2:
            
                return SaveGroupsString; //return string to save to SetValue command that called this method.
    }
        catch
    {
        print("AGX Critical Fail: PartModule SaveActionGroups");
            return "";
    }
        }

    public void PartOnDetach() //callback that runs when a part is detached in the editor
    {

        AGXEditor.DetachedPartActions.AddRange(partAGActions); //add actiongroups on this part to List
        AGXEditor.DetachedPartReset.Stop(); //stop timer so it resets
        //print("Detach");
        
    }
    public void PartOnAttach() //callback that runs when part is attached in editor
    {
       AGXEditor.DetachedPartReset.Start(); //start timer to add actiongroups back onto symmetrical parts
       //AGXEditor.AttachAGXPart(this.part, partAllActions, partAGActions);
        //print("Attach");


    }
    public void OnDestroy()
    {
        if (CallBacksSet) //remove callbacks when part is destroyed. Not sure this is necessary but it does not hurt
        {
            this.part.OnEditorDetach -= PartOnDetach;
            this.part.OnEditorAttach -= PartOnAttach;
            CallBacksSet = false;
            //print("clear callbacks");

        }
    }

        public string[] LoadShowGroupNames()
        {
            string[] ReturnNames = new string[6];
            ReturnNames[1] = "Group 1";
            ReturnNames[2] = "Group 2";
            ReturnNames[3] = "Group 3";
            ReturnNames[4] = "Group 4";
            ReturnNames[5] = "Group 5";

            try
            {
                string LoadNames = AGXGroupStateNames;
                int KeyLength = LoadNames.IndexOf('\u2023');
                if (KeyLength > 0)
                {
                    ReturnNames[1] = LoadNames.Substring(0, KeyLength );
                        for(int i = 2;i <= 4;i++)
                        {
                            LoadNames = LoadNames.Substring(KeyLength+1);
                            KeyLength = LoadNames.IndexOf('\u2023');
                            ReturnNames[i] = LoadNames.Substring(0, KeyLength);
                        }
                        LoadNames = LoadNames.Substring(KeyLength + 1);
                        ReturnNames[5] = LoadNames;
                }
                return ReturnNames;

            }
            catch
            {
                return ReturnNames;
            }

        }

        public void Update()
{

    if (!CallBacksSet && HighLogic.LoadedSceneIsEditor) //set callbacks to handle placing symmetry parts with actions assigned
    {
        this.part.OnEditorDetach += PartOnDetach;
        this.part.OnEditorAttach += PartOnAttach;
        CallBacksSet = true;
    }
    if (CallBacksSet && !HighLogic.LoadedSceneIsEditor) //remove callbacks now that we are not in editor
    {
        this.part.OnEditorDetach -= PartOnDetach;
        this.part.OnEditorAttach -= PartOnAttach;
        CallBacksSet = false;
    }

    if (partAGActions != null) //actiongroup list is initialized?
    {
        if (partAGLastCount != partAGActions.Count) //has an actiongroup been added or deleted? 
        {
            if (HighLogic.LoadedSceneIsEditor) //it's the editor, refresh our actiongroups
            {
                AGXEditor.NeedToLoadActions = true;
                partAGLastCount = partAGActions.Count;
            }
        }
    }
}

public void DeleteAction(int delgroup, string baname)
{
    foreach (AGXAction agxact in partAGActions)
    {
        if (agxact.group == delgroup && agxact.ba.name == baname)
        {
            partAGActions.Remove(agxact); //delete the actiongroup if it is us
        }
        break; //list we are foreaching has changed, must exit or it will throw an error
    }

} //deleted an actiongroup

public List<AGXAction> LoadActionGroups()
{
    List<AGXAction> partAGActions2 = new List<AGXAction>();
    try
    {
       

        //partAGActions = new List<AGXAction>();
        partAllActions = new List<BaseAction>(); //populate all actions available on this part
        partAllActions.AddRange(this.part.Actions);

        foreach (PartModule pm in this.part.Modules)
        {
            partAllActions.AddRange(pm.Actions);

        }
        partCurrentKeySet = Convert.ToInt32(AGXKeySet);
        if (partCurrentKeySet < 1 | partCurrentKeySet > 5)
        {
            partCurrentKeySet = 1;
        }

        if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
        {

            

            
                //partAGActions.Clear();
                string LoadList = AGXData;
               
                if (LoadList.Length > 0)
                {
                    if (LoadList[0] == '\u2023')
                    {
                        bool AllDone = new bool();
                        AllDone = false;
                        while (!AllDone)
                        {
                            LoadList = LoadList.Substring(1);//remove leading u2023
                            int KeyLength = new int();
                            int ActGroup = new int();
                            bool Activated = new bool();
                            KeyLength = LoadList.IndexOf('\u2023'); //is there another action after this one?
                            if (KeyLength == -1) //no, last action in AGXData
                            {
                                KeyLength = LoadList.Length;
                            }

                            ActGroup = Convert.ToInt32(LoadList.Substring(0, 3)); //pull out actiongroup
                            LoadList = LoadList.Substring(3); //remove actiongroup from string

                            
                            if (LoadList[0] == '1') //set group activated
                            {
                                Activated = true;
                               
                            }
                            else
                            {
                                Activated = false;
                                
                            }
                            LoadList = LoadList.Substring(1); //remove Activated from string

                            //if (LoadList[KeyLength - 5] == '\u2022') //default part
                            //{

                            //    partAGActions.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = partAllActions.Find(b => b.name == LoadList.Substring(0, KeyLength - 5)), activated = Activated }); //add action
                            //    LoadList = LoadList.Substring(KeyLength - 4); //remove this action from load string

                            //}
                           // else 
                            
                             if (LoadList.Substring(0, KeyLength - 4).Contains('\u2020')) //science module action found
                            {
                              
                                string ActionName = LoadList.Substring(0, LoadList.IndexOf('\u2020')); //name of action

                                string ExperimentName = LoadList.Substring(LoadList.IndexOf('\u2020') + 1, KeyLength - 5 - LoadList.IndexOf('\u2020')); //name of partmodule, using .experimentName as identifier
                                
                                 List<ModuleEnviroSensor> SciPMList = new List<ModuleEnviroSensor>(); //list of science modules on part
                                foreach (PartModule pm in part.Modules.OfType<ModuleEnviroSensor>()) //get all scicne modules on part
                                {
                                    SciPMList.Add((ModuleEnviroSensor)pm);
                                }
                                List<BaseAction> SciModuleActs = new List<BaseAction>();
                                foreach (ModuleEnviroSensor mse in SciPMList) //find this actions science module and get actions list
                                {
                                    if (mse.sensorType == ExperimentName)
                                    {
                                        SciModuleActs.AddRange(mse.Actions);
                                        break; //break out of foreach, only want to find one science module
                                    }
                                }
                                partAGActions2.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = SciModuleActs.Find(b => b.name == ActionName), activated = Activated });
                                LoadList = LoadList.Substring(KeyLength - 4); //remove this action from load string
                            }
                             else if (LoadList.Substring(0, KeyLength - 4).Contains('\u2022')) //science part
                             {
                                
                                 string ActionName = LoadList.Substring(0, LoadList.IndexOf('\u2022')); //name of action

                                 string ExperimentID = LoadList.Substring(LoadList.IndexOf('\u2022') + 1, KeyLength - 5 - LoadList.IndexOf('\u2022')); //name of action shown on guiexperimentID
                                 
                                 List<ModuleScienceExperiment> SciPMList = new List<ModuleScienceExperiment>(); //list of science modules on part
                                 foreach (PartModule pm in part.Modules.OfType<ModuleScienceExperiment>()) //get all scicne modules on part
                                 {
                                     
                                     SciPMList.Add((ModuleScienceExperiment)pm);
                                     
                                 }
                                 List<BaseAction> SciActs = new List<BaseAction>();
                                
                                 
                                     SciActs.AddRange(SciPMList.Find(b => b.experimentID== ExperimentID).Actions);
                                     partAGActions2.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = SciActs.Find(b => b.name == ActionName), activated = Activated });
                                     LoadList = LoadList.Substring(KeyLength - 4); //remove this action from load string
                                 //foreach (BaseAction baList in SciActs) //find this actions science module and get actions list
                                 //{
                                 //    if (baList.name == ActionName && baList.exp == ActionGUIName)
                                 //    {
                                 //        partAGActions.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = baList, activated = Activated });
                                 //        break; //break out of foreach, only want to find one action
                                 //    }
                                 //}
                             }
                                 else if (LoadList.Substring(0, KeyLength - 4).Contains('\u2021')) //regular part
                             {
                                
                                     string ActionName = LoadList.Substring(0, LoadList.IndexOf('\u2021')); //name of action
                                 string ActionGUIName = LoadList.Substring(LoadList.IndexOf('\u2021') + 1, KeyLength - 5 - LoadList.IndexOf('\u2021')); //name of action shown on gui
                                 
                                 foreach (BaseAction baList in partAllActions) //find this actions science module and get actions list
                                 {
                                     if (baList.name == ActionName && baList.guiName == ActionGUIName)
                                     {
                                         
                                         partAGActions2.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = baList, activated = Activated });
                                         goto BreakOut; //break out of foreach, only want to find one action
                                     }
                                 }
                             BreakOut:
                                
                             
                             LoadList = LoadList.Substring(KeyLength - 4); //remove this action from load string
                                 }

                                // partAGActions.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = SciModuleActs.Find(b => b.name == ActionName), activated = Activated });
                                 //LoadList = LoadList.Substring(KeyLength - 4); //remove this action from load string
                             
                            //else if (kethane check)

                            else //no AGX symbol on the end, loading AGX version 1.4a or older 
                            {
                                
                                partAGActions2.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = partAllActions.Find(b => b.name == LoadList.Substring(0, KeyLength - 4)), activated = Activated });    //add action, old string format                  
                                LoadList = LoadList.Substring(KeyLength - 4); //remove this action from load string
                            }
                            
                             
                            if (LoadList.Length == 0)
                            {
                                AllDone = true;
                            }


                        }
                    }
                }
                //if (AGXFlight.CurrentVesselActions == null)
                //{
                  
                //    AGXFlight.CurrentVesselActions = new List<AGXAction>();
                //}
                //AGXFlight.CurrentVesselActions.AddRange(partAGActions);
               // AGXFlight.ActiveActionsCalculated = false; fix this

                //start default groups load, load everything, most will be duplicate though
                            

                    List<KSPActionGroup> CustomActions = new List<KSPActionGroup>();
                    CustomActions.Add(KSPActionGroup.Custom01); //how do you add a range from enum?
                    CustomActions.Add(KSPActionGroup.Custom02);
                    CustomActions.Add(KSPActionGroup.Custom03);
                    CustomActions.Add(KSPActionGroup.Custom04);
                    CustomActions.Add(KSPActionGroup.Custom05);
                    CustomActions.Add(KSPActionGroup.Custom06);
                    CustomActions.Add(KSPActionGroup.Custom07);
                    CustomActions.Add(KSPActionGroup.Custom08);
                    CustomActions.Add(KSPActionGroup.Custom09);
                    CustomActions.Add(KSPActionGroup.Custom10);


                    // string AddGroup = "";

                    foreach (BaseAction baLoad in partAllActions)
                    {
                        foreach (KSPActionGroup agrp in CustomActions)
                        {

                            if ((baLoad.actionGroup & agrp) == agrp)
                            {

                                ////AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) + 1).ToString("000") + baLoad.guiName;
                                //partAGActions2.Add(new AGXAction() { group = CustomActions.IndexOf(agrp) + 1, prt = this.part, ba = baLoad, activated = false });
                                AGXAction ToAdd = new AGXAction() { prt = this.part, ba = baLoad, group = CustomActions.IndexOf(agrp) + 1, activated = false };
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(partAGActions2);
                                Checking.RemoveAll(p => p.group != ToAdd.group);

                                Checking.RemoveAll(p => p.prt != ToAdd.prt);

                                Checking.RemoveAll(p => p.ba != ToAdd.ba);



                                if (Checking.Count == 0)
                                {

                                    partAGActions2.Add(ToAdd);
                                   
                                }
                            }
                        }
                    }


                    // AGXData = AddGroup;

                    
                    AGXLoaded = true;

                
            
        }
        return partAGActions2;
    }

    catch
    {
        print("AGX Critical Fail: PartModule LoadActionGroups");
        return partAGActions2;
    }
}
    public override void OnLoad(ConfigNode node)
    {

        partAGActions = new List<AGXAction>();
        partAGActions.AddRange(LoadActionGroups());

    }

        public override void OnSave(ConfigNode node)
        {
       
           
            if (!AGXLoaded)
            {
                partAGActions.Clear();
                partAGActions.AddRange(LoadActionGroups());
            }

            if (partCurrentKeySet != 0)
            {
                node.SetValue("AGXData", SaveActionGroups());
            }

            if (HighLogic.LoadedSceneIsFlight && AGXFlight.loadFinished)
            {
                node.SetValue("AGXNames", AGXFlight.SaveGroupNames(this.part, AGXNames));
                node.SetValue("AGXKeySet", AGXFlight.SaveCurrentKeySet(this.part, AGXKeySet));
                node.SetValue("AGXGroupStates", AGXFlight.SaveGroupVisibility(this.part, AGXGroupStates));
                node.SetValue("AGXGroupStateNames", AGXFlight.SaveGroupVisibilityNames(this.part, AGXGroupStateNames));
            }
            else if (HighLogic.LoadedSceneIsEditor && AGXEditor.LoadFinished)
            {
                node.SetValue("AGXNames", AGXEditor.SaveGroupNames(this.part, AGXNames));
                node.SetValue("AGXKeySet", AGXEditor.SaveCurrentKeySet(this.part, AGXKeySet));
                node.SetValue("AGXGroupStates", AGXEditor.SaveGroupVisibility(this.part, AGXGroupStates));
                node.SetValue("AGXGroupStateNames", AGXEditor.SaveGroupVisibilityNames(this.part, AGXGroupStateNames));
            }
  
        }


    }

  

    public class AGXPart
    {
        public Part AGPart;
        public List<BaseAction> AGba;

        public AGXPart()
        {
            AGba = new List<BaseAction>();
        }
        
        public AGXPart(Part p)
        {
            
            AGba = p.Actions;
            AGPart = p;

        }
        
    }

    public class AGXActionsState
    {
        public int group;
        public bool actionOff;
        public bool actionOn;
    }
  
    

    

}