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
            if(agxActionsThisPart.Count > 0)
            {
                foreach (AGXAction agAct in agxActionsThisPart)
                {
                    ConfigNode actionNode = new ConfigNode("ACTION");
                    actionNode = AGextScenario.SaveAGXActionVer2(agAct);
                    node.AddNode(actionNode);
                }
            }
        }

            public override void OnLoad(ConfigNode node)
            {
                ConfigNode[] actionsNodes = node.GetNodes("ACTION");
                foreach(ConfigNode actionNode in actionsNodes)
                {
                    agxActionsThisPart.Add(AGextScenario.LoadAGXActionVer2(actionNode,this.part,false));
                }
            }
       
    }//ModuleAGX

    


   
    
}//name space closing bracket
