using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;


namespace ActionGroupsExtended
{

    public enum AGXRemoteTechItemState
    {
        COUNTDOWN,
        GOOD,
        FAILED,
        NOCOMMS,
    }
    public class AGXRemoteTechQueueItem //queue for remotetech action groups
    {
        public AGXRemoteTechItemState state;
        public int group;
        public string grpName;
        public Vessel vsl;
        public double timeToActivate;
        public bool forcing;
        public bool forceDir;

        public AGXRemoteTechQueueItem(int Group, string GroupName, Vessel vessel, double actTime, bool force, bool ForceDir, AGXRemoteTechItemState State)
        {
            state = State;
            group = Group;
            grpName = GroupName;
            vsl = vessel;
            timeToActivate = actTime;
            forcing = force;
            forceDir = ForceDir;
        }

    }


    public static class AGXStaticData
    {
        public static bool cleanupAlreadyRun = false;
        public static ConfigNode AGExtConfig;
        public static bool nodeLoaded = false;
        public static ConfigNode LoadBaseConfigNode()
        {
            if (nodeLoaded)
            {
                return AGExtConfig;
            }
            else
            {
                ConfigNode nodeLoad = new ConfigNode("AGExtConfig");
                nodeLoad = GameDatabase.Instance.GetConfigNode("Diazo/AGExt/AGExt/AGExtConfig");
                if (nodeLoad == null)
                {
                    nodeLoad = new ConfigNode("AGExtConfig");
                }
                if (!nodeLoad.HasValue("name"))
                {
                    nodeLoad.AddValue("name", "AGExtConfig");
                }
                if (!nodeLoad.HasValue("ActiveKeySet"))
                {
                    nodeLoad.AddValue("ActiveKeySet", "1");
                }
                if (!nodeLoad.HasValue("KeySet1"))
                {
                    nodeLoad.AddValue("KeySet1", "‣001Alpha1‣002Alpha2‣003Alpha3‣004Alpha4‣005Alpha5‣006Alpha6‣007Alpha7‣008Alpha8‣009Alpha9‣010Alpha0");
                }
                if (!nodeLoad.HasValue("KeySet2"))
                {
                    nodeLoad.AddValue("KeySet2", "‣001Alpha1‣002Alpha2‣003Alpha3‣004Alpha4‣005Alpha5‣006Alpha6‣007Alpha7‣008Alpha8‣009Alpha9‣010Alpha0");
                }
                if (!nodeLoad.HasValue("KeySet3"))
                {
                    nodeLoad.AddValue("KeySet3", "‣001Alpha1‣002Alpha2‣003Alpha3‣004Alpha4‣005Alpha5‣006Alpha6‣007Alpha7‣008Alpha8‣009Alpha9‣010Alpha0");
                }
                if (!nodeLoad.HasValue("KeySet4"))
                {
                    nodeLoad.AddValue("KeySet4", "‣001Alpha1‣002Alpha2‣003Alpha3‣004Alpha4‣005Alpha5‣006Alpha6‣007Alpha7‣008Alpha8‣009Alpha9‣010Alpha0");
                }
                if (!nodeLoad.HasValue("KeySet5"))
                {
                    nodeLoad.AddValue("KeySet5", "‣001Alpha1‣002Alpha2‣003Alpha3‣004Alpha4‣005Alpha5‣006Alpha6‣007Alpha7‣008Alpha8‣009Alpha9‣010Alpha0");
                }
                if (!nodeLoad.HasValue("KeySetName1"))
                {
                    nodeLoad.AddValue("KeySetName1", "KeySet1");
                }
                if (!nodeLoad.HasValue("KeySetName2"))
                {
                    nodeLoad.AddValue("KeySetName2", "KeySet2");
                }
                if (!nodeLoad.HasValue("KeySetName3"))
                {
                    nodeLoad.AddValue("KeySetName3", "KeySet3");
                }
                if (!nodeLoad.HasValue("KeySetName4"))
                {
                    nodeLoad.AddValue("KeySetName4", "KeySet4");
                }
                if (!nodeLoad.HasValue("KeySetName5"))
                {
                    nodeLoad.AddValue("KeySetName5", "KeySet5");
                }
                if (!nodeLoad.HasValue("EdSelPartsX"))
                {
                    nodeLoad.AddValue("EdSelPartsX", "100");
                }
                if (!nodeLoad.HasValue("EdSelPartsY"))
                {
                    nodeLoad.AddValue("EdSelPartsY", "100");
                }
                if (!nodeLoad.HasValue("EdKeySetX"))
                {
                    nodeLoad.AddValue("EdKeySetX", "120");
                }
                if (!nodeLoad.HasValue("EdKeySetY"))
                {
                    nodeLoad.AddValue("EdKeySetY", "120");
                }
                if (!nodeLoad.HasValue("EdGroupsX"))
                {
                    nodeLoad.AddValue("EdGroupsX", "140");
                }
                if (!nodeLoad.HasValue("EdGroupsY"))
                {
                    nodeLoad.AddValue("EdGroupsY", "140");
                }
                if (!nodeLoad.HasValue("EdKeyCodeX"))
                {
                    nodeLoad.AddValue("EdKeyCodeX", "160");
                }

                if (!nodeLoad.HasValue("EdKeyCodeY"))
                {
                    nodeLoad.AddValue("EdKeyCodeY", "160");
                }
                if (!nodeLoad.HasValue("EdCurActsX"))
                {
                    nodeLoad.AddValue("EdCurActsX", "180");
                }
                if (!nodeLoad.HasValue("EdCurActsY"))
                {
                    nodeLoad.AddValue("EdCurActsY", "180");
                }
                if (!nodeLoad.HasValue("FltSelPartsX"))
                {
                    nodeLoad.AddValue("FltSelPartsX", "100");
                }
                if (!nodeLoad.HasValue("FltSelPartsY"))
                {
                    nodeLoad.AddValue("FltSelPartsY", "100");
                }
                if (!nodeLoad.HasValue("FltKeySetX"))
                {
                    nodeLoad.AddValue("FltKeySetX", "120");
                }
                if (!nodeLoad.HasValue("FltKeySetY"))
                {
                    nodeLoad.AddValue("FltKeySetY", "120");
                }
                if (!nodeLoad.HasValue("FltGroupsX"))
                {
                    nodeLoad.AddValue("FltGroupsX", "140");
                }
                if (!nodeLoad.HasValue("FltGroupsY"))
                {
                    nodeLoad.AddValue("FltGroupsY", "140");
                }
                if (!nodeLoad.HasValue("FltKeyCodeX"))
                {
                    nodeLoad.AddValue("FltKeyCodeX", "160");
                }
                if (!nodeLoad.HasValue("FltKeyCodeY"))
                {
                    nodeLoad.AddValue("FltKeyCodeY", "160");
                }
                if (!nodeLoad.HasValue("FltCurActsX"))
                {
                    nodeLoad.AddValue("FltCurActsX", "180");
                }
                if (!nodeLoad.HasValue("FltCurActsY"))
                {
                    nodeLoad.AddValue("FltCurActsY", "180");
                }
                if (!nodeLoad.HasValue("FltMainX"))
                {
                    nodeLoad.AddValue("FltMainX", "200");
                }
                if (!nodeLoad.HasValue("FltMainY"))
                {
                    nodeLoad.AddValue("FltMainY", "200");
                }
                if (!nodeLoad.HasValue("RTWinX"))
                {
                    nodeLoad.AddValue("RTWinX", "220");
                }
                if (!nodeLoad.HasValue("RTWinY"))
                {
                    nodeLoad.AddValue("RTWinY", "220");
                }
                if (!nodeLoad.HasValue("FltShow"))
                {
                    nodeLoad.AddValue("FltShow", "1");
                }
                if (!nodeLoad.HasValue("EditShow"))
                {
                    nodeLoad.AddValue("EditShow", "1");
                }
                if (!nodeLoad.HasValue("RTWinShow"))
                {
                    nodeLoad.AddValue("RTWinShow", "True");
                }
                if (!nodeLoad.HasValue("FlightWinShowKeys"))
                {
                    nodeLoad.AddValue("FlightWinShowKeys", "1");
                }
                if (!nodeLoad.HasValue("DeleteOldSaves"))
                {
                    nodeLoad.AddValue("DeleteOldSaves", "1");
                }
                if (!nodeLoad.HasValue("LockOutKSPManager"))
                {
                    nodeLoad.AddValue("LockOutKSPManager", "1");
                }
                if (!nodeLoad.HasValue("KeySetMod1Group1"))
                {
                    nodeLoad.AddValue("KeySetMod1Group1", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod2Group1"))
                {
                    nodeLoad.AddValue("KeySetMod2Group1", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod1Group2"))
                {
                    nodeLoad.AddValue("KeySetMod1Group2", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod2Group2"))
                {
                    nodeLoad.AddValue("KeySetMod2Group2", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod1Group3"))
                {
                    nodeLoad.AddValue("KeySetMod1Group3", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod2Group3"))
                {
                    nodeLoad.AddValue("KeySetMod2Group3", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod1Group4"))
                {
                    nodeLoad.AddValue("KeySetMod1Group4", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod2Group4"))
                {
                    nodeLoad.AddValue("KeySetMod2Group4", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod1Group5"))
                {
                    nodeLoad.AddValue("KeySetMod1Group5", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetMod2Group5"))
                {
                    nodeLoad.AddValue("KeySetMod2Group5", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                }
                if (!nodeLoad.HasValue("KeySetModKey11"))
                {
                    nodeLoad.AddValue("KeySetModKey11", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey21"))
                {
                    nodeLoad.AddValue("KeySetModKey21", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey12"))
                {
                    nodeLoad.AddValue("KeySetModKey12", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey22"))
                {
                    nodeLoad.AddValue("KeySetModKey22", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey13"))
                {
                    nodeLoad.AddValue("KeySetModKey13", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey23"))
                {
                    nodeLoad.AddValue("KeySetModKey23", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey14"))
                {
                    nodeLoad.AddValue("KeySetModKey14", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey24"))
                {
                    nodeLoad.AddValue("KeySetModKey24", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey15"))
                {
                    nodeLoad.AddValue("KeySetModKey15", "None");
                }
                if (!nodeLoad.HasValue("KeySetModKey25"))
                {
                    nodeLoad.AddValue("KeySetModKey25", "None");
                }
                if (!nodeLoad.HasValue("ActivationCooldown"))
                {
                    nodeLoad.AddValue("ActivationCooldown", "5");
                }
                if (!nodeLoad.HasValue("OverrideCareer"))
                {
                    nodeLoad.AddValue("OverrideCareer", "0");
                }
                AGExtConfig = nodeLoad;
                nodeLoaded = true;
                return nodeLoad;
            }
        }

        public static void SaveBaseConfigNode(ConfigNode cNode)
        {
            ConfigNode toSave = new ConfigNode("AGExtConfig");
            toSave.AddNode(cNode);
            toSave.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");

        }
    }



    public class ModuleAGX : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public string placeHolder = "Hello World"; //Config nodes can behave wierd when empty, a part with no actions on it will have no data besides this line
        //config nodes get added/removed via OnLoad/OnSave here
        public List<AGXAction> agxActionsThisPart = new List<AGXAction>();

        public override void OnStart(StartState state)
        {
            if (agxActionsThisPart == null)
            {
                agxActionsThisPart = new List<AGXAction>();
            }
        }

        public override void OnSave(ConfigNode node)
        {
            string ErrLine = "1";
            //Debug.Log("AGX Saving Module");
            try
            {
                node.RemoveNodes("ACTION");
                node.RemoveNodes("TOGGLE");
                node.RemoveNodes("HOLD");
                ErrLine = "2";
                List<AGXAction> actsToSave = new List<AGXAction>();
                actsToSave.AddRange(agxActionsThisPart);
                if(HighLogic.LoadedSceneIsEditor)
                {
                    actsToSave.AddRange(AGXEditor.CurrentVesselActions.Where(act => act.ba.listParent.part == this.part));
                }
                else if(HighLogic.LoadedSceneIsFlight)
                {
                    actsToSave.AddRange(AGXFlight.CurrentVesselActions.Where(act => act.ba.listParent.part == this.part));
                }
                
                    ErrLine = "3";
                    foreach (AGXAction agAct in actsToSave)
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
                
                if (HighLogic.LoadedSceneIsEditor)
                {
                    ConfigNode toggleStates = new ConfigNode("TOGGLE");
                    ConfigNode holdStates = new ConfigNode("HOLD");
                    for (int i = 1; i <= 250; i++)
                    {
                        if (AGXEditor.IsGroupToggle[i])
                        {
                            toggleStates.AddValue("toggle", i.ToString());
                        }
                        if (AGXEditor.isDirectAction[i])
                        {
                            holdStates.AddValue("hold", i.ToString());
                        }
                    }
                    node.AddNode(toggleStates);
                    node.AddNode(holdStates);
                }
            }
            catch (Exception e)
            {
                print("AGX partModule OnSave fail: " + ErrLine + " " + e);
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            //Debug.Log("Part Load " + this.part.partName); 
            string errLine = "1";
            try
            {
                errLine = "2";
                ConfigNode[] actionsNodes = node.GetNodes("ACTION"); 
                errLine = "3";
                string[] toggles;
                if(node.HasNode("TOGGLE"))
                {
                    toggles = node.GetNode("TOGGLE").GetValues();
                }
                else
                { 
                    toggles = new string[1];
                }
                errLine = "4";
                string[] holds;
                if(node.HasNode("HOLD"))
                {
                    holds = node.GetNode("HOLD").GetValues();
                }
                else
                {
                    holds = new string[1];
                }
                errLine = "5";
                foreach (ConfigNode actionNode in actionsNodes)
                {
                    errLine = "6";
                    if (HighLogic.LoadedSceneIsEditor && toggles.Contains(actionNode.GetValue("group")) && AGXEditor.CurrentVesselActions.FindAll(act => act.group.ToString() == actionNode.GetValue("group")).Count == 0)
                    {
                        errLine = "7";
                        AGXEditor.IsGroupToggle[int.Parse(actionNode.GetValue("group"))] = true;
                    }
                    if (HighLogic.LoadedSceneIsEditor && holds.Contains(actionNode.GetValue("group")) && AGXEditor.CurrentVesselActions.FindAll(act => act.group.ToString() == actionNode.GetValue("group")).Count == 0)
                    {
                        errLine = "8";
                        AGXEditor.isDirectAction[int.Parse(actionNode.GetValue("group"))] = true;
                    }
                    errLine = "9";
                    agxActionsThisPart.Add(AGextScenario.LoadAGXActionVer2(actionNode, this.part, false));
                }
            }
            catch(Exception e)
            {
                Debug.Log("AGX Module OnLoad Error " + errLine + " " + e);
            }
            //print("Load called " + agxActionsThisPart.Count);
        }

    }//ModuleAGX

    public class AGXOtherVessel //data class for a vessel that does not have focus
    {
        private uint flightID; //
        private bool vesselInstanceOK = false; //is this vessel loadable? if this is false, run nothing
        private List<AGXAction> actionsList;
        private Vessel thisVsl;
        private Dictionary<int, string> guiNames;

        public AGXOtherVessel(uint FlightID)
        {
            flightID = FlightID;
            //Debug.Log("Before");
            List<Vessel> loadedVessels = FlightGlobals.Vessels.FindAll(vsl => vsl.loaded == true);
            thisVsl = loadedVessels.Find(ves => ves.rootPart.flightID == flightID);
            //Debug.Log("After");
            actionsList = new List<AGXAction>();
            guiNames = new Dictionary<int, string>();
            if (thisVsl == null) //check vessel is loaded
            {
                vesselInstanceOK = false;
                //ScreenMessages.PostScreenMessage("AGX cannot activate actions on unloaded vessels.", 10F, ScreenMessageStyle.UPPER_CENTER);
                return;
            }
            ConfigNode nodeLoad = new ConfigNode();
            
            if (AGXFlight.AGXFlightNode != null)
            {
                if (AGXFlight.AGXFlightNode.HasNode(flightID.ToString()))
                {
                    nodeLoad = AGXFlight.AGXFlightNode.GetNode(flightID.ToString());
                    guiNames = LoadGuiNames(nodeLoad.GetValue("groupNames"));
                    ConfigNode[] partNodes = nodeLoad.GetNodes("PART");
                    foreach (ConfigNode prtNode in partNodes)
                    {
                        Part thisPrt = thisVsl.Parts.Find(p => p.flightID == Convert.ToUInt32(prtNode.GetValue("flightID")));
                        ConfigNode[] actionNodes = prtNode.GetNodes("ACTION");
                        foreach (ConfigNode aNode in actionNodes)
                        {
                            actionsList.Add(AGextScenario.LoadAGXActionVer2(aNode, thisPrt, false));
                        }
                    }
                }
                vesselInstanceOK = true;
            }
        }

        private Dictionary<int,string> LoadGuiNames(string loadNames)
        {
            string LoadNames = loadNames;
            Dictionary<int, string> guiNames  = new Dictionary<int, string>();
            if (LoadNames.Length > 0) 
            {
                while (LoadNames[0] == '\u2023')
                {
                    int groupNum = new int();
                    string groupName = "";
                    LoadNames = LoadNames.Substring(1);
                    groupNum = Convert.ToInt32(LoadNames.Substring(0, 3));
                    LoadNames = LoadNames.Substring(3);
                    if (LoadNames.IndexOf('\u2023') == -1)
                    {
                        groupName = LoadNames;
                    }
                    else
                    {
                        groupName = LoadNames.Substring(0, LoadNames.IndexOf('\u2023'));
                        LoadNames = LoadNames.Substring(LoadNames.IndexOf('\u2023'));
                    }
                        guiNames[groupNum] = groupName;
                }
            }
            return guiNames;
        }

        public string GetGroupName(int group)
        {
            return guiNames[group];
        }
        
        public Dictionary<int,string> GetGroupNamesAll()
        {
            return guiNames;
        }
        
        public List<AGXAction> GetAssignedActions()
        {
            if (vesselInstanceOK)
            {
                return actionsList;
            }
            else
            {
                return new List<AGXAction>();
            }
        }

        public List<Part> prtListInGroup(int i)
        {
            List<Part> prtListInGroup = new List<Part>();
            foreach (AGXAction agAct in actionsList.Where(agx => agx.group == i))
            {
                if (!prtListInGroup.Contains(agAct.ba.listParent.part))
                {
                    prtListInGroup.Add(agAct.ba.listParent.part);
                }
            }
            return prtListInGroup;
        }

        public List<PartModule> pmListInGroup(int i)
        {
            List<PartModule> prtListInGroup = new List<PartModule>();
            foreach (AGXAction agAct in actionsList.Where(agx => agx.group == i))
            {
                if (!prtListInGroup.Contains(agAct.ba.listParent.module))
                {
                    prtListInGroup.Add(agAct.ba.listParent.module);
                }
            }
            return prtListInGroup;
        }

        public List<AGXAction> GetAssignedActionsGroup(int group)
        {
            if (vesselInstanceOK)
            {
                return (List<AGXAction>)actionsList.Where(act => act.group == group);
            }
            else
            {
                return new List<AGXAction>();
            }
        }

        public void ActivateActionString(string groupStr, bool force, bool forceDir) //main activation method
        {
            if (thisVsl.HoldPhysics)
            {
                ScreenMessages.PostScreenMessage("AGX cannot activate actions while under timewarp.", 10F, ScreenMessageStyle.UPPER_CENTER);
            }
            else if (vesselInstanceOK)
            {
                int group = actionsList.Find(agx => agx.grpName == groupStr).group;
                ActivateActionGroup(group, force, forceDir);

            }
        }

        public void ActivateActionStringActivation(string groupStr, bool force, bool forceDir) //main activation method
        {
            if (thisVsl.HoldPhysics)
            {
                ScreenMessages.PostScreenMessage("AGX cannot activate actions while under timewarp.", 10F, ScreenMessageStyle.UPPER_CENTER);
            }
            else if (vesselInstanceOK)
            {
                int group = actionsList.Find(agx => agx.grpName == groupStr).group;
                ActivateActionGroupActivation(group, force, forceDir);

            }
        }

        public void ActivateActionGroup(int group, bool force, bool forceDir)
        {

            if (AGXFlight.RTFound)
            {
                // Debug.Log("RemoteTech found");
                //Debug.Log("delay " + AGXRemoteTechLinks.RTTimeDelay(FlightGlobals.ActiveVessel));
                //Debug.Log("in local " + AGXRemoteTechLinks.InLocal(FlightGlobals.ActiveVessel));
                //double curDelay = AGXRemoteTechLinks.RTTimeDelay(FlightGlobals.ActiveVessel);
                //print("cur delay" + curDelay);
                if (thisVsl.Parts.Any(p => p.protoModuleCrew.Any() && p.Modules.Contains("ModuleCommand"))) //are we in local control? Kerbal on board on a part with command abilities?
                {
                    Debug.Log("RemoteTech local");
                    AGXFlight.AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, actionsList.Find(act => act.group == group).grpName, thisVsl, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.COUNTDOWN));
                }
                else if (double.IsInfinity(AGXRemoteTechLinks.RTTimeDelay(thisVsl))) //remotetech returns positive infinity when a vessel is in local control so no delay, note that RT also returns positive infinity when a vessel has no connection so this check has to come second.
                {
                    Debug.Log("RemoteTech infinity");
                    AGXFlight.AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, actionsList.Find(act => act.group == group).grpName, thisVsl, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.NOCOMMS));
                }
                else
                {
                    Debug.Log("RemoteTech normal " + AGXRemoteTechLinks.RTTimeDelay(thisVsl));
                    if (AGXFlight.useRT)
                    {
                        AGXFlight.AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, actionsList.Find(act => act.group == group).grpName, thisVsl, Planetarium.GetUniversalTime() + AGXRemoteTechLinks.RTTimeDelay(thisVsl), force, forceDir, AGXRemoteTechItemState.COUNTDOWN));
                    }
                    else
                    {
                        AGXFlight.AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, actionsList.Find(act => act.group == group).grpName, thisVsl, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.COUNTDOWN));
                    }
                }

            }
            else
            {
                ActivateActionGroupActivation(group, force, forceDir);
            }
        }

        public void ActivateActionGroupActivation(int group, bool force, bool forceDir) //main activation method
        {
            string ErrLine = "1";
            try
            {
                if (thisVsl.HoldPhysics)
                {
                    ScreenMessages.PostScreenMessage("AGX cannot activate actions while under timewarp.", 10F, ScreenMessageStyle.UPPER_CENTER);
                }
                else if (vesselInstanceOK)
                {
                    ErrLine = "2";
                    Dictionary<int, KSPActionGroup> CustomActions = new Dictionary<int, KSPActionGroup>();
                    CustomActions.Add(1, KSPActionGroup.Custom01); //how do you add a range from enum?
                    CustomActions.Add(2, KSPActionGroup.Custom02);
                    CustomActions.Add(3, KSPActionGroup.Custom03);
                    CustomActions.Add(4, KSPActionGroup.Custom04);
                    CustomActions.Add(5, KSPActionGroup.Custom05);
                    CustomActions.Add(6, KSPActionGroup.Custom06);
                    CustomActions.Add(7, KSPActionGroup.Custom07);
                    CustomActions.Add(8, KSPActionGroup.Custom08);
                    CustomActions.Add(9, KSPActionGroup.Custom09);
                    CustomActions.Add(10, KSPActionGroup.Custom10);
                    ErrLine = "3";
                    foreach (AGXAction agAct in actionsList.Where(agx => agx.group == group))
                    {
                        ErrLine = "4";

                        if (force) //are we forcing a direction or toggling?
                        {
                            ErrLine = "5";
                            if (forceDir) //we are forcing a direction so set the agAct.activated to trigger the direction below correctly
                            {
                                agAct.activated = false; //we are forcing activation so activated is false
                            }
                            else
                            {
                                agAct.activated = true;
                            }
                        }
                        ErrLine = "6";
                        if (agAct.activated)
                        {
                            ErrLine = "7";
                            KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Deactivate);
                            //print("AGX action deactivate FIRE! " + agAct.ba.guiName);
                            ErrLine = "8";
                            // Debug.Log("act it " + agAct.ba.active);
                            agAct.ba.Invoke(actParam);
                            agAct.activated = false;
                            ErrLine = "9";
                            if (group <= 10)
                            {
                                ErrLine = "10";
                                thisVsl.ActionGroups[CustomActions[group]] = false;
                            }
                            ErrLine = "11";
                        }
                        else
                        {
                            ErrLine = "12";
                            KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate);
                            //agAct.activated = true;
                            //print("AGX action activate FIRE!" + agAct.ba.guiName);
                            ErrLine = "13";
                            //Debug.Log("act it2 " + agAct.ba.active);
                            agAct.ba.Invoke(actParam);
                            agAct.activated = true;
                            ErrLine = "14";
                            if (group <= 10)
                            {
                                ErrLine = "15";
                                thisVsl.ActionGroups[CustomActions[group]] = true;
                            }
                            ErrLine = "16";
                        }

                        if (agAct.ba.name != "kOSVoidAction")
                        {
                            foreach (AGXAction agActCheck in actionsList.Where(ag => ag.ba == agAct.ba))
                            {
                                agActCheck.activated = agAct.activated;
                            }
                        }

                        if (agAct.ba.listParent.module.moduleName == "ModuleEngines" && agAct.ba.name == "ActivateAction" || agAct.ba.listParent.module.moduleName == "ModuleEngines" && agAct.ba.name == "OnAction")
                        {
                            ErrLine = "17";
                            //overide to activate part when activating an engine so gimbals come on
                            agAct.ba.listParent.part.force_activate();
                            //print("Force act");

                        }
                        ErrLine = "18";
                        if (agAct.ba.listParent.module.moduleName == "ModuleEnginesFX" && agAct.ba.name == "ActivateAction" || agAct.ba.listParent.module.moduleName == "ModuleEnginesFX" && agAct.ba.name == "OnAction")
                        {
                            ErrLine = "19";
                            //overide to activate part when activating an engine so gimbals come on
                            agAct.ba.listParent.part.force_activate();
                            //print("Force act");
                            ErrLine = "20";
                        }
                        ErrLine = "21";
                    }
                    ErrLine = "22";
                }
                SaveThisVessel();
            }
            catch (Exception e)
            {
                Debug.Log("AGX OtherVsl ActivateActionGroup " + ErrLine + " " + e);

            }

        }

        public bool StateCheckGroup(int group)
        {
            if (vesselInstanceOK)
            {
                actionsList = AGXFlight.CheckActionsActiveActualCode(actionsList);
                bool groupState = true;
                if (actionsList.Where(ag => ag.group == group).Count() >= 1)
                {
                    foreach (AGXAction agAct in actionsList.Where(ag => ag.group == group))
                    {
                        if (!agAct.activated)
                        {
                            groupState = false;
                        }
                    }
                }
                else
                {
                    groupState = false;
                }
                return groupState;
            }
            else
            {
                Debug.Log("AGX Group State FALSE due to vessel not loaded.");
                return false;
            }
        }

        public void SaveThisVessel()
        {
            string errLine = "1";
            try
            {
                if (vesselInstanceOK)
                {
                    ConfigNode thisVslNode = new ConfigNode(flightID.ToString());
                    errLine = "2";
                    if (AGXFlight.AGXFlightNode.HasNode(flightID.ToString()))
                    {
                        errLine = "3";
                        thisVslNode = AGXFlight.AGXFlightNode.GetNode(thisVsl.rootPart.flightID.ToString());
                        errLine = "4";
                        AGXFlight.AGXFlightNode.RemoveNode(thisVsl.rootPart.flightID.ToString());
                    }
                    errLine = "5";
                    thisVslNode.RemoveNodes("PART");
                    errLine = "6";
                    foreach (Part p in thisVsl.Parts)
                    {
                        errLine = "7";
                        List<AGXAction> thisPartsActions = new List<AGXAction>();
                        errLine = "8";
                        thisPartsActions.AddRange(actionsList.FindAll(p2 => p2.ba.listParent.part == p));
                        errLine = "18";
                        if (thisPartsActions.Count > 0)
                        {
                            //print("acts count " + thisPartsActions.Count);
                            ConfigNode partTemp = new ConfigNode("PART");
                            errLine = "19";
                            partTemp.AddValue("name", p.partInfo.name);
                            partTemp.AddValue("vesselName", p.vessel.vesselName);
                            //partTemp.AddValue("relLocX", FlightGlobals.ActiveVessel.rootPart.transform.InverseTransformPoint(p.transform.position).x);
                            //partTemp.AddValue("relLocY", FlightGlobals.ActiveVessel.rootPart.transform.InverseTransformPoint(p.transform.position).y);
                            //partTemp.AddValue("relLocZ", FlightGlobals.ActiveVessel.rootPart.transform.InverseTransformPoint(p.transform.position).z);
                            partTemp.AddValue("flightID", p.flightID.ToString());
                            errLine = "20";
                            foreach (AGXAction agxAct in thisPartsActions)
                            {
                                //print("acts countb " + thisPartsActions.Count);
                                errLine = "21";
                                partTemp.AddNode(AGextScenario.SaveAGXActionVer2(agxAct));
                            }
                            errLine = "22";

                            thisVslNode.AddNode(partTemp);
                            errLine = "23";
                        }
                        errLine = "24";
                    }
                    errLine = "25";
                    if (AGXFlight.AGXFlightNode.HasNode(thisVsl.id.ToString()))
                    {
                        errLine = "26";
                        AGXFlight.AGXFlightNode.RemoveNode(thisVsl.id.ToString());
                    }
                    errLine = "27";
                    if (AGXFlight.AGXFlightNode.HasNode(thisVsl.rootPart.flightID.ToString()))
                    {
                        errLine = "28";
                        AGXFlight.AGXFlightNode.RemoveNode(thisVsl.rootPart.flightID.ToString());
                    }
                    errLine = "29";
                    //print("save node " + thisVsl);
                    AGXFlight.AGXFlightNode.AddNode(thisVslNode);
                }
            }
            catch (Exception e)
            {
                Debug.Log("AGX OtherVslSaveNode Fail " + errLine + " " + e);
            }

        }

    }


}//name space closing bracket
