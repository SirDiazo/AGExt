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
        public string AGXData;

        [KSPField(isPersistant = true, guiActive = false)]
        public string AGXNames;

        [KSPField(isPersistant = true, guiActive = false)]
        public string AGXKeySet;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool AGXLoaded = false;

        public List<BaseAction> partAllActions;

        public List<AGXAction> partAGActions;

        public Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();

        public int partCurrentKeySet = 0;
        public int partAGLastCount = 0;
        public bool CallBacksSet = false;
      

    
         


    public string SaveActionGroups() 
    {
        //print("start save action groups " +this.part.ConstructID);
       
       KSPActs[1] = KSPActionGroup.Custom01;
        KSPActs[2] = KSPActionGroup.Custom02;
        KSPActs[3] = KSPActionGroup.Custom03;
        KSPActs[4] = KSPActionGroup.Custom04;
        KSPActs[5] = KSPActionGroup.Custom05;
        KSPActs[6] = KSPActionGroup.Custom06;
        KSPActs[7] = KSPActionGroup.Custom07;
        KSPActs[8] = KSPActionGroup.Custom08;
        KSPActs[9] = KSPActionGroup.Custom09;
        KSPActs[10] = KSPActionGroup.Custom10;
        
        
        foreach (BaseAction clrAct in partAllActions)
            {
                for (int i = 1; i <= 10; i = i + 1)
                {
                    //clrAct.actionGroup = clrAct.actionGroup &= KSPActs[i];  //actiongrouptest
                }
            }
        if (partAGActions.Count >= 1 && HighLogic.LoadedSceneIsEditor)
        {
            
            //print("AGCnt " + partAGActions.ElementAt(0).prt.partInfo.name + " " +partAGActions.ElementAt(0).ba.name + " " +partAGActions.ElementAt(0).group.ToString() + " " +partAGActions.ElementAt(0).activated);
            //print("A");
            try
            {
                if (partAGActions.ElementAt(0).prt == null)
                {

                }
                //print("not null");
            }
                catch
            {
                    //print("null?");
                partAGActions.Clear();
                partAGActions.AddRange(AGXEditor.AttachAGXPart(this.part, partAllActions, partAGActions));
                AGXEditor.NeedToLoadActions = true;
                
            }
            //print("b");
        }
        //print("part cnt " + partAGActions.Count);
            string SaveGroupsString = "";
            
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
                SaveGroupsString = SaveGroupsString + '\u2023' + agAct.group.ToString("000");
             
                    if(agAct.activated==true)
                    {
                     
                        SaveGroupsString = SaveGroupsString + "1";
                    }
                    else
                    {
                       
                        SaveGroupsString = SaveGroupsString + "0";
                    }
                   
                   SaveGroupsString = SaveGroupsString + agAct.ba.name;
                  
                   if (agAct.group <= 10)
                   {
                       //agAct.ba.actionGroup = (agAct.ba.actionGroup |= KSPActs[agAct.group]); //actiongrouptest
                   }
                   
                }
        //BreakOut2:
            
                return SaveGroupsString;
        }

    public void PartOnDetach()
    {

        AGXEditor.DetachedPartActions.AddRange(partAGActions);
        AGXEditor.DetachedPartReset.Stop();
        //print("Detach");
        
    }
    public void PartOnAttach()
    {
       AGXEditor.DetachedPartReset.Start();
       //AGXEditor.AttachAGXPart(this.part, partAllActions, partAGActions);
        //print("Attach");


    }
    public void OnDestroy()
    {
        if (CallBacksSet && HighLogic.LoadedSceneIsEditor)
        {
            this.part.OnEditorDetach -= PartOnDetach;
            this.part.OnEditorAttach -= PartOnAttach;
            CallBacksSet = false;
            //print("clear callbacks");

        }
    }

        public void Update()
{

    if (!CallBacksSet && HighLogic.LoadedSceneIsEditor)
    {
        this.part.OnEditorDetach += PartOnDetach;
        this.part.OnEditorAttach += PartOnAttach;
        CallBacksSet = true;
        
    }
    //print(this.part.name + " " + this.part.isConnected);
    if (partAGActions != null)
    {
        if (partAGLastCount != partAGActions.Count)
        {
            if (HighLogic.LoadedSceneIsEditor)
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
            partAGActions.Remove(agxact);
        }
        break;
    }

}
        
        public void LoadActionGroups()
    {

        KSPActs[1] = KSPActionGroup.Custom01;
        KSPActs[2] = KSPActionGroup.Custom02;
        KSPActs[3] = KSPActionGroup.Custom03;
        KSPActs[4] = KSPActionGroup.Custom04;
        KSPActs[5] = KSPActionGroup.Custom05;
        KSPActs[6] = KSPActionGroup.Custom06;
        KSPActs[7] = KSPActionGroup.Custom07;
        KSPActs[8] = KSPActionGroup.Custom08;
        KSPActs[9] = KSPActionGroup.Custom09;
        KSPActs[10] = KSPActionGroup.Custom10;

        partAGActions = new List<AGXAction>();
        partAllActions = new List<BaseAction>();
        partAllActions.AddRange(this.part.Actions);

        foreach (PartModule pm in this.part.Modules)
        {
            partAllActions.AddRange(pm.Actions);

        }
        partCurrentKeySet = Convert.ToInt32(AGXKeySet);
        
        if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
        {
          
            if (!AGXLoaded)
            {
              
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


                string AddGroup = "";

                foreach (BaseAction baLoad in partAllActions)
                {
                    foreach (KSPActionGroup agrp in CustomActions)
                    {

                        if ((baLoad.actionGroup & agrp) == agrp)
                        {

                            AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) + 1).ToString("000") + baLoad.guiName;
                            partAGActions.Add(new AGXAction() { group = CustomActions.IndexOf(agrp) + 1, prt = this.part, ba = baLoad, activated = false });
                        }
                    }
                }


                AGXData = AddGroup;

                AGXKeySet = "1";
                partCurrentKeySet = 1;
                AGXLoaded = true;

            }

            else
            {
                partAGActions.Clear();
                string LoadList = AGXData;
                if (LoadList.Length > 0)
                {
                    if (LoadList[0] == '\u2023')
                    {
                        bool AllDone = new bool();
                        AllDone = false;
                        while (!AllDone)
                        {
                            
                            LoadList = LoadList.Substring(1);
                            int KeyLength = new int();
                            int ActGroup = new int();
                            bool Activated = new bool();
                            KeyLength = LoadList.IndexOf('\u2023');
                            if (KeyLength == -1)
                            {
                                KeyLength = LoadList.Length;
                            }

                            ActGroup = Convert.ToInt32(LoadList.Substring(0, 3));
                            LoadList = LoadList.Substring(3);
                            
                            if (Convert.ToInt32(LoadList[0]) == 1)
                            {
                                Activated = true;
                            }
                            else
                            {
                                Activated = false;
                            }
                            
                            
                            partAGActions.Add(new AGXAction() { group = ActGroup, prt = this.part, ba = partAllActions.Find(b => b.name == LoadList.Substring(1, KeyLength - 4)), activated = Activated });
                                                      
                            LoadList = LoadList.Substring(KeyLength - 3);
                            
                            if (LoadList.Length == 0)
                            {
                                AllDone = true;
                            }


                        }
                    }
                }
            
            }
        }
    }
    public override void OnLoad(ConfigNode node)
    {
        
        LoadActionGroups();

    }

        public override void OnSave(ConfigNode node)
        {
       
           
            if (!AGXLoaded)
            {
                LoadActionGroups();
            }

            if (partCurrentKeySet != 0)
            {
                node.SetValue("AGXData", SaveActionGroups());
            }

            if (HighLogic.LoadedSceneIsFlight && AGXFlight.loadFinished)
            {
                node.SetValue("AGXNames", AGXFlight.SaveGroupNames(this.part, AGXNames));
                node.SetValue("AGXKeySet", AGXFlight.SaveCurrentKeySet(this.part, AGXKeySet));
            }
            else if (HighLogic.LoadedSceneIsEditor && AGXEditor.LoadFinished)
            {
                node.SetValue("AGXNames", AGXEditor.SaveGroupNames(this.part, AGXNames));
                node.SetValue("AGXKeySet", AGXEditor.SaveCurrentKeySet(this.part, AGXKeySet));
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
  
    

    

}