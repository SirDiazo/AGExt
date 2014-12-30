using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;


namespace ActionGroupsExtended
{

    

   
    public class ModuleAGX : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)] 
        public string placeHolder = "Hello World"; //Config nodes can behave wierd when empty, a part with no actions on it will have no data besides this line
        //config nodes get added/removed via OnLoad/OnSave here
        public List<AGXAction> agxActionsThisPart = new List<AGXAction>();

        public override void OnStart(StartState state)
        {
            if(agxActionsThisPart == null)
            {
                agxActionsThisPart = new List<AGXAction>();
            }
        }
        
        public override void OnSave(ConfigNode node)
        {
            string ErrLine = "1";
            try
            {
                ErrLine = "2";
                if(agxActionsThisPart.Count > 0)
                {
                    ErrLine = "3";
                foreach (AGXAction agAct in agxActionsThisPart)
                {
                    ErrLine = "4";
                    ConfigNode actionNode = new ConfigNode("ACTION");
                    ErrLine = "5";
                    if (agAct != null)
                    {
                        ErrLine = "5a";
                        actionNode = AGextScenario.SaveAGXActionVer2(agAct);
                    }
                    ErrLine = "6";
                    node.AddNode(actionNode);
                    ErrLine = "7";
                }
                }
            }
        catch(Exception e)
        {
            print("AGX partModule OnSave fail: " + ErrLine + " " + e);
        }
        }

            public override void OnLoad(ConfigNode node)
            {
                
                ConfigNode[] actionsNodes = node.GetNodes("ACTION");
                foreach(ConfigNode actionNode in actionsNodes)
                {
                    agxActionsThisPart.Add(AGextScenario.LoadAGXActionVer2(actionNode,this.part,false));
                }
                print("Load called " + agxActionsThisPart.Count);
            }
       
    }//ModuleAGX

    


   
    
}//name space closing bracket
