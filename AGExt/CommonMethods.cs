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
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public int currentKeyset = 1;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public string groupNames = "";
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public string groupVisibility = "";
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public string groupVisibilityNames = "Group1‣Group2‣Group3‣Group4‣Group5";
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public string DirectActionState = "";
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public bool hasData = false;

        public ConfigNode toggleNode;
        public ConfigNode holdNode;



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
            //Debug.Log("AGX Saving Module start");
            try
            {
                node.RemoveNodes("ACTION");
                //node.RemoveNodes("TOGGLE");
                //node.RemoveNodes("HOLD");
                ErrLine = "2";
                List<AGXAction> actsToSave = new List<AGXAction>();
                ErrLine = "2a";
                actsToSave.AddRange(agxActionsThisPart);
                ErrLine = "2b";
                if (HighLogic.LoadedSceneIsEditor)
                {
                    //Debug.Log("AGX Editor save called by partmodule!");

                    ErrLine = "2c";

                    foreach (AGXAction agActSD in StaticData.CurrentVesselActions.Where(act => act.ba.listParent.part == this.part))
                    {
                        if (!actsToSave.Contains(agActSD))
                        {
                            actsToSave.Add(agActSD);
                        }
                    }
                }
                else if (HighLogic.LoadedSceneIsFlight)
                {
                    ErrLine = "2d";

                    foreach (AGXAction agActSD in StaticData.CurrentVesselActions.Where(act => act.ba.listParent.part == this.part))
                    {
                        if (!actsToSave.Contains(agActSD))
                        {
                            actsToSave.Add(agActSD);
                        }
                    }
                    //Debug.Log("AGX Partmodule Save action saved okay");
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

                        actionNode = StaticData.SaveAGXActionVer2(agAct);
                    }
                    ErrLine = "6";
                    node.AddNode(actionNode);
                    ErrLine = "7";
                }
                //actions themselves are now saved
                ErrLine = "7a";
                if (HighLogic.LoadedSceneIsEditor)
                {
                    if (AGXEditor.LoadFinished)
                    {
                        //node.RemoveNodes("TOGGLE"); //not using confignode to save this stuff
                        //node.RemoveNodes("HOLD");
                        //ErrLine = "7b";

                        //ConfigNode toggleStates = new ConfigNode("TOGGLE");
                        //ConfigNode holdStates = new ConfigNode("HOLD");
                        //ErrLine = "7c";
                        //for (int i = 1; i <= 250; i++)
                        //{
                        //    ErrLine = "7ca";
                        //    if (AGXEditor.IsGroupToggle[i])
                        //    {
                        //        ErrLine = "7cb";
                        //        toggleStates.AddValue("toggle", i.ToString());
                        //    }
                        //    ErrLine = "7cc";
                        //    if (AGXEditor.isDirectAction[i])
                        //    {
                        //        ErrLine = "7cd";
                        //        holdStates.AddValue("hold", i.ToString());
                        //    }
                        //}
                        //ErrLine = "7d";
                        //node.AddNode(toggleStates);
                        //node.AddNode(holdStates);
                        //toggleNode = toggleStates;
                        //holdNode = holdStates;

                        node.SetValue("groupNames", AGXEditor.SaveGroupNames(""), true);
                        ErrLine = "7e";
                        node.SetValue("groupVisibility", AGXEditor.SaveGroupVisibility(""), true);
                        ErrLine = "7f";
                        node.SetValue("groupVisibilityNames", AGXEditor.SaveGroupVisibilityNames(""), true);
                        ErrLine = "7g";
                        node.SetValue("DirectActionState", AGXEditor.SaveDirectActionState(""), true);
                        node.SetValue("hasData", "true", true);
                    }
                }
                //else if (HighLogic.LoadedSceneIsFlight) //do not save data, we are not guaranteed to be the FLightGlobals.ActiveVessel
                //{
                //    //node.SetValue("groupNames", AGXFlight.SaveGroupNames(""), true);
                //    //ErrLine = "7e";
                //    //node.SetValue("groupVisibility", AGXFlight.SaveGroupVisibility(""), true);
                //    //ErrLine = "7f";
                //    //node.SetValue("groupVisibilityNames", AGXFlight.SaveGroupVisibilityNames(""), true);
                //    //ErrLine = "7g";
                //    //node.SetValue("DirectActionState", AGXFlight.SaveDirectActionState(""), true);
                //    //node.SetValue("hasData", "true", true);
                //}



                //Debug.Log("AGX PartModule Save Okay"); //temporary
                //Debug.Log("AGX Saving Module end" + StaticData.CurrentVesselActions.Count());
            }
            catch (Exception e)
            {
                print("AGX partModule OnSave fail: " + ErrLine + " " + e);
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            //Debug.Log("AGX Load Module" + node.ToString()); 
            string errLine = "1";
            try
            {
                errLine = "2";
                ConfigNode[] actionsNodes = node.GetNodes("ACTION");
                errLine = "3";
                //string[] toggles;
                //if(node.HasNode("TOGGLE")) //done in editor load now
                //{
                //    toggles = node.GetNode("TOGGLE").GetValues();
                //}
                //else
                //{ 
                //    toggles = new string[1];
                //}
                //errLine = "4";
                //string[] holds;
                //if(node.HasNode("HOLD"))
                //{
                //    holds = node.GetNode("HOLD").GetValues();
                //}
                //else
                //{
                //    holds = new string[1];
                //}
                errLine = "5";
                foreach (ConfigNode actionNode in actionsNodes)
                {
                    errLine = "6";
                    //if (HighLogic.LoadedSceneIsEditor && toggles.Contains(actionNode.GetValue("group")) && StaticData.CurrentVesselActions.FindAll(act => act.group.ToString() == actionNode.GetValue("group")).Count == 0)
                    //{
                    //    errLine = "7";
                    //    AGXEditor.IsGroupToggle[int.Parse(actionNode.GetValue("group"))] = true;
                    //}
                    //if (HighLogic.LoadedSceneIsEditor && holds.Contains(actionNode.GetValue("group")) && StaticData.CurrentVesselActions.FindAll(act => act.group.ToString() == actionNode.GetValue("group")).Count == 0)
                    //{
                    //    errLine = "8";
                    //    AGXEditor.isDirectAction[int.Parse(actionNode.GetValue("group"))] = true;
                    //}
                    errLine = "9";
                    // Debug.Log("Step 1 " + actionNode.ToString());
                    AGXAction actToAdd = StaticData.LoadAGXActionVer2(actionNode, this.part, false);
                    //Debug.Log("Step 2 " + actToAdd.ToString());
                    if (actToAdd != null && !agxActionsThisPart.Contains(actToAdd))
                    {
                        agxActionsThisPart.Add(actToAdd);
                    }
                }
                //.Log("AGX PartModule Load Okay"); //temporary
                //Debug.Log("AGX Load Module End" + agxActionsThisPart.Count()); 
            }
            catch (Exception e)
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
        public List<AGXAction> actionsList;
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
                            AGXAction newAct = StaticData.LoadAGXActionVer2(aNode, thisPrt, false);
                            if (newAct != null)
                            {
                                actionsList.Add(newAct);
                            }
                        }
                    }
                }
                vesselInstanceOK = true;
            }
        }

        private Dictionary<int, string> LoadGuiNames(string loadNames)
        {
            string LoadNames = loadNames;
            Dictionary<int, string> guiNames = new Dictionary<int, string>();
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

        public Dictionary<int, string> GetGroupNamesAll()
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
                    Debug.Log("AGX: RemoteTech local action");
                    AGXFlight.AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, actionsList.Find(act => act.group == group).grpName, thisVsl, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.COUNTDOWN));
                }
                else if (double.IsInfinity(AGXRemoteTechLinks.RTTimeDelay(thisVsl))) //remotetech returns positive infinity when a vessel is in local control so no delay, note that RT also returns positive infinity when a vessel has no connection so this check has to come second.
                {
                    Debug.Log("AGX: RemoteTech infinity");
                    AGXFlight.AGXRemoteTechQueue.Add(new AGXRemoteTechQueueItem(group, actionsList.Find(act => act.group == group).grpName, thisVsl, Planetarium.GetUniversalTime(), force, forceDir, AGXRemoteTechItemState.NOCOMMS));
                }
                else
                {
                    Debug.Log("AGX: RemoteTech normal " + AGXRemoteTechLinks.RTTimeDelay(thisVsl));
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
                Debug.Log("AGX OtherVsl ActivateActionGroup fail " + ErrLine + " " + e);

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
                // Debug.Log("AGX Group State FALSE due to vessel not loaded.");
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
                                partTemp.AddNode(StaticData.SaveAGXActionVer2(agxAct));
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

    public static class StaticData
    {
        public static List<AGXAction> CurrentVesselActions;

        static StaticData()
        {
            CurrentVesselActions = new List<AGXAction>();
        }

        public static string EditorHashShipName(string name, bool isVAB)
        {
            string hashedName = "";
            if (isVAB)
            {
                hashedName = "VAB";
            }
            else
            {
                hashedName = "SPH";
            }
            foreach (Char ch in name)
            {
                hashedName = hashedName + (int)ch;
            }
            //print("hashName " + hashedName);

            return hashedName;
        }

        public static AGXAction LoadAGXActionVer2(ConfigNode actNode, Part actPart, bool showAmbiguousMessage) //returns null on error, check where returned
        {
            string errLine = "1";
            //print("load action " + actPart.partName + " " + actNode);
            try
            {
                errLine = "2";
                AGXAction ActionToLoad = new AGXAction(); //create action we are loading
                errLine = "2aa";
                ActionToLoad.prt = actPart; //assign part
                errLine = "2bb";
                ActionToLoad.group = Convert.ToInt32(actNode.GetValue("group")); //assign group
                errLine = "2cc";
                if (actNode.HasValue("groupName"))
                {
                    ActionToLoad.grpName = actNode.GetValue("groupName"); //assign group
                }
                errLine = "2a";
                if (actNode.GetValue("activated") == "1") //assign activated
                {
                    ActionToLoad.activated = true;
                }
                else
                {
                    ActionToLoad.activated = false;
                }
                errLine = "2b";
                string pmName = actNode.GetValue("partModule");//get partModule name
                List<BaseAction> actsToCompare = new List<BaseAction>(); //create list of actions we will compare to
                errLine = "2c";
                if (actNode.HasValue("pmIndex"))
                {
                    //actPart is part
                    PartModule ourPM = PartModuleIndexToModule((string)actNode.GetValue("partModule"), Int32.Parse((string)actNode.GetValue("pmIndex")), actPart); //e(string pmName, int pmIndex, Part p)
                    ActionToLoad.ba = ourPM.Actions[(string)actNode.GetValue("actionName")];
                    Debug.Log("AGX New Load Okay " + ActionToLoad.ToString());
                    return ActionToLoad;
                }
                else //in theory this entire else statement is obsolete as of agx 1.34c and will never be run, leave for backwards compatibility
                {
                    if (pmName == "ModuleEnviroSensor")
                    {
                        string sensorType = actNode.GetValue("custom1");
                        foreach (PartModule pmSensor in actPart.Modules.OfType<ModuleEnviroSensor>())
                        {
                            ModuleEnviroSensor mesSensor = (ModuleEnviroSensor)pmSensor;
                            if (mesSensor.sensorType == sensorType)
                            {
                                actsToCompare.AddRange(mesSensor.Actions);
                            }
                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                    }
                    else if (pmName == "ModuleScienceExperiment")
                    {
                        string expID = actNode.GetValue("custom1");
                        foreach (PartModule pmSensor in actPart.Modules.OfType<ModuleScienceExperiment>())
                        {
                            ModuleScienceExperiment mesExp = (ModuleScienceExperiment)pmSensor;
                            if (mesExp.experimentID == expID)
                            {
                                actsToCompare.AddRange(mesExp.Actions);
                            }
                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        // actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                    }
                    else if (pmName == "ModuleAnimateGeneric")
                    {
                        string animName = actNode.GetValue("custom1");
                        foreach (PartModule pmSensor in actPart.Modules.OfType<ModuleAnimateGeneric>())
                        {
                            ModuleAnimateGeneric mesExp = (ModuleAnimateGeneric)pmSensor;
                            if (mesExp.animationName == animName)
                            {
                                actsToCompare.AddRange(mesExp.Actions);
                            }
                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                    }
                    else if (pmName == "FSanimateGeneric")
                    {
                        //print("load it");
                        string animName = actNode.GetValue("custom1");
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            //ModuleAnimateGeneric mesExp = (ModuleAnimateGeneric)pmSensor;
                            if (pm.moduleName == pmName)
                            {
                                if ((string)pm.Fields.GetValue("animationName") == animName)
                                {
                                    actsToCompare.AddRange(pm.Actions);
                                }
                            }
                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                    }
                    else if (pmName == "DMModuleScienceAnimate")
                    {
                        string startEventName = actNode.GetValue("custom1");
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }

                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                        actsToCompare.RemoveAll(b3 => (string)b3.listParent.module.Fields.GetValue("startEventGUIName") != (string)startEventName);
                    }
                    else if (pmName == "DMSolarCollector")
                    {
                        string startEventName = actNode.GetValue("custom1");
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }

                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                        actsToCompare.RemoveAll(b3 => (string)b3.listParent.module.Fields.GetValue("startEventGUIName") != (string)startEventName);
                    }
                    else if (pmName == "BTSMModuleReactionWheel")
                    {
                        //string startEventName = actNode.GetValue("custom1");
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }

                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                        //actsToCompare.RemoveAll(b3 => b3.listParent.module.Fields.GetValue("startEventGUIName") != startEventName);
                    }
                    else if (pmName == "BTSMModuleCrewReport" || pmName == "BTSMModuleScienceExperiment" || pmName == "BTSMModuleScienceExperimentWithTime")
                    {
                        string startEventName = actNode.GetValue("custom1");
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }

                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                        actsToCompare.RemoveAll(b3 => (string)b3.listParent.module.Fields.GetValue("experimentActionName") != (string)startEventName);
                    }
                    else if (pmName == "BTSMModuleResourceActionToggle")
                    {
                        string startEventName = actNode.GetValue("custom1");
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                                // print("Batest " + actNode.GetValue("actionName") + " " + pm.Fields.GetValue("resourceName") + " " + startEventName); 
                            }

                        }
                        //foreach (BaseAction ba6 in actsToCompare)
                        //{
                        //    print("1 " + ba6.name + " " + ba6.listParent.module.Fields.GetValue("resourceName"));
                        //}
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        //foreach (BaseAction ba6 in actsToCompare)
                        //{  
                        //    print("2 " + ba6.name + " " + ba6.listParent.module.Fields.GetValue("resourceName"));
                        //}
                        //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                        //print("2a " + startEventName);
                        actsToCompare.RemoveAll(b3 => (string)b3.listParent.module.Fields.GetValue("resourceName") != (string)startEventName);
                        //foreach (BaseAction ba6 in actsToCompare)
                        //{
                        //    print("3 " + ba6.name + " " + ba6.listParent.module.Fields.GetValue("resourceName"));
                        //}
                    }
                    else if (pmName == "Capacitor" || pmName == "DischargeCapacitor") //NearFutureElectrical
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == "Capacitor" || pm.moduleName == "DischargeCapacitor")
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }
                    }
                    else if (pmName == "FissionReprocessor" || pmName == "Nuclear Fuel Reprocessor") //NearFutureElectrical
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == "FissionReprocessor" || pm.moduleName == "Nuclear Fuel Reprocessor")
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }
                    }
                    else if (pmName == "FissionGenerator" || pmName == "Fission Reactor") //NearFutureElectrical
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == "FissionGenerator" || pm.moduleName == "Fission Reactor")
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }
                    }
                    else if (pmName == "ModuleCurvedSolarPanel" || pmName == "Curved Solar Panel") //NearFutureSolar
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == "ModuleCurvedSolarPanel" || pm.moduleName == "Curved Solar Panel")
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }
                    }
                    else if (pmName == "VariableISPEngine" || pmName == "Variable ISP Engine") //NearFutureSolar
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == "VariableISPEngine" || pm.moduleName == "Variable ISP Engine")
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }
                    }
                    else if (pmName == "ModuleRTAntenna") //Remotetech
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            // actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }
                    }
                    else if (pmName == "SCANsat") //Remotetech
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => (string)b2.listParent.module.Fields.GetValue("scanName") != (string)actNode.GetValue("custom1"));
                        }
                    }
                    else if (pmName == "ModuleEnginesFX")
                    {

                        foreach (ModuleEnginesFX pm in actPart.Modules.OfType<ModuleEnginesFX>()) //add actions to compare
                        {
                            //print("Fields " + (string)pm.Fields.GetValue("engineID") + "||" + (string)actNode.GetValue("custom1"));
                            if ((string)pm.Fields.GetValue("engineID") == (string)actNode.GetValue("custom1"))
                            {
                                actsToCompare.AddRange(pm.Actions);
                                //print("Acts to compare " + actsToCompare.Count + " " + pm.Actions.Count + pm.name + pm.moduleName);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));

                            //actsToCompare.RemoveAll(b2 => (string)b2.listParent.module.Fields.GetValue("scanName") != (string)actNode.GetValue("custom1"));
                        }
                    }
                    else if (pmName == "RealChuteModule")
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            //actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }
                    }
                    else if (pmName == "REGO_ModuleAnimationGroup")
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => (string)b2.listParent.module.Fields.GetValue("deployAnimationName") + (string)b2.listParent.module.Fields.GetValue("activeAnimationName") != (string)actNode.GetValue("custom1"));
                        }
                    }
                    else if (pmName == "REGO_ModuleResourceHarvester")
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => (string)b2.listParent.module.Fields.GetValue("RecipeInputs") + (string)b2.listParent.module.Fields.GetValue("ResourceName") != (string)actNode.GetValue("custom1"));
                        }
                    }
                    else if (pmName == "REGO_ModuleResourceConverter")
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => (string)b2.listParent.module.Fields.GetValue("RecipeInputs") + (string)b2.listParent.module.Fields.GetValue("RecipeOutputs") != (string)actNode.GetValue("custom1"));
                        }
                    }
                    else if (pmName == "REGO_ModuleAsteroidDrill")
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => (string)b2.listParent.module.Fields.GetValue("ImpactTransform") != (string)actNode.GetValue("custom1"));
                        }
                    }
                    else if (pmName == "ModuleModActions") //guiName is player editable so can't be used. 
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                        }
                    }
                    else if (pmName == "ModuleReactionWheel") //guiName is player editable so can't be used. 
                    {
                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                        }
                    }
                    else
                    {

                        foreach (PartModule pm in actPart.Modules) //add actions to compare
                        {
                            if (pm.moduleName == pmName)
                            {
                                actsToCompare.AddRange(pm.Actions);
                            }
                            //foreach(BaseAction ba in actsToCompare)
                            //{
                            //    Debug.Log("BA list " + ba.name + " " + ba.guiName);
                            //}
                            actsToCompare.RemoveAll(b => b.name != (string)actNode.GetValue("actionName"));
                            actsToCompare.RemoveAll(b2 => b2.guiName != (string)actNode.GetValue("actionGuiName"));
                        }

                    }
                }//close new if
                errLine = "3";
                //print("ActsCount " + actsToCompare.Count);
                if (actsToCompare.Count != 1)
                {
                    errLine = "4";
                    Debug.Log("AGX actsToCompare.count != 1 " + actsToCompare.Count + " Part: " + actPart.name + " Module: " + actNode.GetValue("partModule") + " " + actNode.GetValue("actionName") + " " + actNode.GetValue("actionGuiName"));
                    //if (showAmbiguousMessage)
                    //{
                    //    ScreenMessages.PostScreenMessage("AGX Load Action ambiguous. Count: " + actsToCompare.Count, 10F, ScreenMessageStyle.UPPER_CENTER);
                    //}
                    ActionToLoad = null;
                }
                errLine = "5";
                if (actsToCompare.Count > 0)
                {
                    // print("ActsCounta");
                    errLine = "6";
                    ActionToLoad.ba = actsToCompare.First(); //action to load assign action, ready to return
                }
                else
                {
                    errLine = "7";
                    // print("ActsCountb");
                    ActionToLoad = null;
                }
                errLine = "8";
                //print("load action2 " + ActionToLoad.ba.name + " " + ActionToLoad.group);
                //print("agx check " + actsToCompare.Count + " " + ActionToLoad.group + ActionToLoad.ba.name);
                //print("actual act " + ActionToLoad + " " + ActionToLoad.ba.name);
                //print("BA load " + ActionToLoad.ba.name + " " + ActionToLoad.ba.listParent.part.ConstructID + " " + ActionToLoad.prt.ConstructID);
                return ActionToLoad;

            }
            catch (Exception e)
            {
                Debug.Log("AGXLoadAGXAction2 FAIL " + errLine + " " + e);
                return null;
            }
        }

        public static ConfigNode SaveAGXActionVer2(AGXAction agxAct)
        {
            //print("Save called");
            string errLine = "1";
            try
            {
                ConfigNode actionNode = new ConfigNode("ACTION");
                errLine = "2";
                actionNode.AddValue("group", agxAct.group);
                errLine = "2a";
                actionNode.AddValue("groupName", agxAct.grpName);
                errLine = "3";
                actionNode.AddValue("activated", (agxAct.activated) ? "1" : "0");
                errLine = "4";
                actionNode.AddValue("partModule", agxAct.ba.listParent.module.GetType().Name);
                errLine = "5";
                actionNode.AddValue("actionGuiName", agxAct.ba.guiName);
                errLine = "6";
                actionNode.AddValue("actionName", agxAct.ba.name);
                errLine = "7";
                actionNode.AddValue("pmIndex", PartModuleModuleToIndex(agxAct.ba.listParent.module, agxAct.ba.listParent.part.Modules).ToString());

                if (agxAct.ba.listParent.module.moduleName == "ModuleEnviroSensor") //add this to the agxactions list somehow and add to save.load serialze
                {
                    errLine = "8";
                    ModuleEnviroSensor MSE = (ModuleEnviroSensor)agxAct.ba.listParent.module;
                    errLine = "9";
                    actionNode.AddValue("custom1", MSE.sensorType); //u2020 is envirosensor
                    errLine = "10";
                }
                else if (agxAct.ba.listParent.module.moduleName == "ModuleScienceExperiment") //add this to the agxactions list somehow and add to save.load serialze
                {
                    errLine = "11";
                    ModuleScienceExperiment MSE = (ModuleScienceExperiment)agxAct.ba.listParent.module; //all other modules use guiname
                    errLine = "12";
                    actionNode.AddValue("custom1", MSE.experimentID); //u2021 is sciencemodule
                    errLine = "13";
                }
                else if (agxAct.ba.listParent.module.moduleName == "ModuleAnimateGeneric") //add this to the agxactions list somehow and add to save.load serialze
                {
                    errLine = "14";
                    ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agxAct.ba.listParent.module; //all other modules use guiname
                    errLine = "15";
                    actionNode.AddValue("custom1", MAnim.animationName); //u2021 is sciencemodule
                    errLine = "16";
                    //print(MAnim.animationName);
                }
                else if (agxAct.ba.listParent.module.moduleName == "FSanimateGeneric") //add this to the agxactions list somehow and add to save.load serialze
                {
                    errLine = "14";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agxAct.ba.listParent.module; //all other modules use guiname
                    errLine = "15";
                    actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("animationName")); //u2021 is sciencemodule
                    errLine = "16";
                    //print(MAnim.animationName);
                }
                else if (agxAct.ba.listParent.module.moduleName == "DMModuleScienceAnimate") //DMagic orbital science mod
                {
                    errLine = "17";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "18";
                    actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("startEventGUIName")); //u2021 is sciencemodule
                    errLine = "19";
                }
                else if (agxAct.ba.listParent.module.moduleName == "DMSolarCollector") //DMagic orbital science mod
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("startEventGUIName")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "BTSMModuleCrewReport" || agxAct.ba.listParent.module.moduleName == "BTSMModuleScienceExperiment" || agxAct.ba.listParent.module.moduleName == "BTSMModuleScienceExperimentWithTime") //DMagic orbital science mod
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("experimentActionName")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "BTSMModuleResourceActionToggle") //
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("resourceName")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "SCANsat") //
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("scanName")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "ModuleEnginesFX") //
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("engineID")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "RealChuteModule") //
                { //RealChute needs no extra data saved, just add this for tracking so I know it is saving as exception.
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    //actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("engineID")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "REGO_ModuleAnimationGroup") //
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", (string)agxAct.ba.listParent.module.Fields.GetValue("deployAnimationName") + (string)agxAct.ba.listParent.module.Fields.GetValue("activeAnimationName")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "REGO_ModuleResourceHarvester") //
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", (string)agxAct.ba.listParent.module.Fields.GetValue("RecipeInputs") + (string)agxAct.ba.listParent.module.Fields.GetValue("ResourceName")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "REGO_ModuleResourceConverter") //
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", (string)agxAct.ba.listParent.module.Fields.GetValue("RecipeInputs") + (string)agxAct.ba.listParent.module.Fields.GetValue("RecipeOutputs")); //u2021 is sciencemodule
                    errLine = "22";
                }
                else if (agxAct.ba.listParent.module.moduleName == "REGO_ModuleAsteroidDrill") //
                {
                    errLine = "20";
                    //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                    errLine = "21";
                    actionNode.AddValue("custom1", (string)agxAct.ba.listParent.module.Fields.GetValue("ImpactTransform")); //u2021 is sciencemodule
                    errLine = "22";
                }

                //BTSMModuleReactionWheel does not need custom save, just load
                else //if (agAct.ba.listParent.module.moduleName == "ModuleScienceExperiment") //add this to the agxactions list somehow and add to save.load serialze
                {
                    errLine = "23";
                    //ModuleScienceExperiment MSE = (ModuleScienceExperiment)agAct.ba.listParent.module; //all other modules use guiname
                    actionNode.AddValue("custom1", "NA"); //u2021 is sciencemodule
                    errLine = "24";
                }

                return actionNode;
            }

            catch (Exception e)
            {
                Debug.Log("AGX SaveAGXAction2 FAIL " + errLine + " " + agxAct.prt.partName + " " + agxAct.ba.name + " " + e);
                return new ConfigNode();
            }

        }
        public static int PartModuleModuleToIndex(PartModule pm, PartModuleList pmList) //return index count of specific partmodule type only, not of entire part.modules list. Used in save routine
        {
            try
            {
                List<PartModule> pmListThisType = new List<PartModule>();
                foreach (PartModule pm2 in pmList)
                {
                    if (pm2.GetType().Name == pm.GetType().Name)
                    {
                        pmListThisType.Add(pm2);
                    }
                }
                return pmListThisType.IndexOf(pm);
            }
            catch
            {
                Debug.Log("AGX SavePMIndex Fail, using default");
                return 0;
            }
        }
        public static PartModule PartModuleIndexToModule(string pmName, int pmIndex, Part p) //used in load routine, convert index number to partModule reference
        {
            try
            {
                List<PartModule> pmThisType = new List<PartModule>();
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.GetType().Name == pmName)
                    {
                        pmThisType.Add(pm);
                    }
                }
                return pmThisType.ElementAt(pmIndex);

            }
            catch
            {
                Debug.Log("AGX Load Action Index fail, action probably lost");
                return new PartModule();
            }
        }
    }

}//name space closing bracket
