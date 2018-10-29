using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.IO;
using System.Reflection;
using AGExt;
using KSP.Localization;
using UnityEngine;

namespace ActionGroupsExtended
{

    public class AGExtExternal : MonoBehaviour
    {
        //All methods start with "AGX" as an identifer.
        //Except for AGXInstalled (that can be called at any time), valid data is returned in the Editor and Flight scenes only, null or empty data is returned in other scenes depending on the requested data type
        //"2Vsl" methods can be called in the editor but only on the current vessel. Only in flight can "2Vsl" methods be called on non-focus vessels.
        //Methods that start with "AGX2Vsl" require a uint identifier that is the part.FlightID of the ROOT part of the vessel. This allows the method to query/control vessels that are not the active vessel.
        //"AGX2Vsl" methods can be safely passed the FlightID of the currently active vessel.
        //AGXInstalled returns True if AGX installed.
        //Other methods that return a bool are status checks. Return true if the method worked, return false if it did not. This is NOT an activation status, an activation method can return True that
        //everything worked fine, but an outside issue prevented the action from activating, such as lack of power.
        //
        //CAUTION: There are two types of activation offered. Local Control and Remote Control.
        //Local Control: Actions activated by something on the vessel, such as the Smart Parts mod. These actions activate immediately without checking for a delay due to RemoteTech.
        //Local Control: Just the method name is this type of control, so "AGXToggleGroup" is a Local Control method.
        //Remote Control: Actions activated remotely by the player, such as keyboard input. This method will apply RemoteTech signal delay if RemoteTech is installed, otherwise it activates immediately.
        //Remote Control: Methods with "DelayCheck" suffixes are Remote, so "AGXToggleGroupDelayCheck" would delay activating the action group for 5 seconds if the current RemoteTech signal delay is 5 seconds.

        public static bool AGXInstalled() //works
        {

            return true;
        }

        public static List<AGXAction> AGXAllActions()//works //all actions on activevessel, returns AGXAction, can other mods even use?
        {
            print("AGX Call: List all actions for active vessel");
            try
            {
                return StaticData.CurrentVesselActions;
            }
            catch
            {
                return new List<AGXAction>();
            }

        }

        public static List<AGXAction> AGX2VslAllAssignedActions(uint FlightID) //works //all actions on specific vessel, returns AGXAction, can other mods use this?
        {
            print("AGX Call: List all assigned actions for vessel " + FlightID);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    return StaticData.CurrentVesselActions;

                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    return otherVsl.GetAssignedActions();
                }
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                return StaticData.CurrentVesselActions;
            }
            else
            {
                return new List<AGXAction>();
            }
        }

        public static List<AGXAction> AGX2VslGroupActions(uint FlightID, int group) //works //all actions in group on specific vessel, returns AGXAction, other mods use?
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    return StaticData.CurrentVesselActions.FindAll(ag => ag.group == group);

                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    return otherVsl.GetAssignedActionsGroup(group);
                }
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                return StaticData.CurrentVesselActions.FindAll(ag => ag.group == group);
            }
            else
            {
                return new List<AGXAction>();
            }
        }

        public static List<AGXAction> AGXGroupActions(int group) //works //all actions on ActiveVessel in group
        {
            print("AGX Call: List actions in active vessel in group " + group); //works
            try
            {
                return StaticData.CurrentVesselActions.FindAll(ag => ag.group == group);
            }
            catch
            {
                return new List<AGXAction>();
            }

        }

        public static string AGXGroupNameSingle(int group) //return the name of the specified action group
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                return AGXFlight.AGXguiNames[group];
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                return AGXEditor.AGXguiNames[group];
            }
            else
            {
                return "";
            }
        }

        public static string AGX2VslGroupNameSingle(uint FlightID, int group) //return name of specific action group on specified vessel
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    return AGXFlight.AGXguiNames[group];

                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    return otherVsl.GetGroupName(group);
                }
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                if (EditorLogic.RootPart.flightID == FlightID)
                {
                    return AGXEditor.AGXguiNames[group];

                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public static Dictionary<int, string> AGXGroupNamesAll() //return dictionary of group names, returns dictionary of 250 items, only groups with defined names will have strings
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                return AGXFlight.AGXguiNames;
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                return AGXEditor.AGXguiNames;
            }
            else
            {
                return new Dictionary<int, string>();
            }
        }

        public static Dictionary<int, string> AGX2VslGroupNamesAll(uint FlightID) //return dictionary of group names
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightID == FlightGlobals.ActiveVessel.rootPart.flightID)
                {
                    return AGXFlight.AGXguiNames;
                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    return otherVsl.GetGroupNamesAll();
                }
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                if (FlightID == EditorLogic.RootPart.flightID)
                {
                    return AGXEditor.AGXguiNames;
                }
                else
                {
                    return new Dictionary<int, string>();
                }
            }
            else
            {
                return new Dictionary<int, string>();
            }
        }

        public static bool AGX2VslToggleGroup(uint FlightID, int group) //other vessel direct toggle activate
        {
            print("AGX Call: toggle action " + group + " for vessel " + FlightID);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    AGXFlight.ActivateActionGroupActivation(group, false, false);
                    return true;
                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    otherVsl.ActivateActionGroupActivation(group, false, false);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool AGX2VslToggleGroupDelayCheck(uint FlightID, int group) //other vessel toggle, delay check
        {
            print("AGX Call: toggle action " + group + " for vessel " + FlightID);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    AGXFlight.ActivateActionGroup(group);
                    return true;
                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    otherVsl.ActivateActionGroup(group, false, false);
                    return true;
                }
            }
            else
            {
                // ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(Localizer.Format("#AGEXT_UI_SCREEN_MESSAGE_3"), 10F, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }
        }

        public static bool AGX2VslGroupState(uint FlightID, int group) //8 on test, owrks
        {
            print("AGX Call: group state for " + group + " for vessel " + FlightID);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    return AGXGroupState(group);

                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    return otherVsl.StateCheckGroup(group);
                }
            }
            else
            {
                // ScreenMessages.PostScreenMessage("AGX Action not checked, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(Localizer.Format("#AGEXT_UI_SCREEN_MESSAGE_4"), 10F, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }
        }

        public static bool AGX2VslActivateGroup(uint FlightID, int group, bool forceDir) //other vessel, direct activate group
        {
            print("AGX Call: Activate group for " + group + " for vessel " + FlightID + " in dir " + forceDir);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    //print("this vsl");
                    AGXFlight.ActivateActionGroupActivation(group, true, forceDir);
                    return true;
                }
                else
                {
                    //print("other vsl");
                    //ScreenMessages.PostScreenMessage("AGX Action Fail, other vessels not implemented yet", 10F, ScreenMessageStyle.UPPER_CENTER);
                    //return false;
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    otherVsl.ActivateActionGroupActivation(group, true, forceDir);
                    return true;
                }
            }
            else
            {
                // ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(Localizer.Format("#AGEXT_UI_SCREEN_MESSAGE_3"), 10F, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }
        }

        public static bool AGX2VslActivateGroupDelayCheck(uint FlightID, int group, bool forceDir) //other vessel, with delay check
        {
            print("AGX Call: Activate group for " + group + " for vessel " + FlightID + " in dir " + forceDir);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    //print("this vsl");
                    AGXFlight.ActivateActionGroup(group, true, forceDir);
                    return true;
                }
                else
                {
                    //print("other vsl");
                    //ScreenMessages.PostScreenMessage("AGX Action Fail, other vessels not implemented yet", 10F, ScreenMessageStyle.UPPER_CENTER);
                    //return false;
                    AGXOtherVessel otherVsl = new AGXOtherVessel(FlightID);
                    otherVsl.ActivateActionGroup(group, true, forceDir);
                    return true;
                }
            }
            else
            {
                // ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(Localizer.Format("#AGEXT_UI_SCREEN_MESSAGE_3"), 10F, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }
        }

        public static bool AGXActivateGroup(int i, bool forceDir) //activate action group it forceDir direction, true = force activate
        {
            print("AGX Call: activate group for " + i + " for active vessel in dir " + forceDir);
            if (HighLogic.LoadedSceneIsFlight)
            {
                AGXFlight.ActivateActionGroupActivation(i, true, forceDir);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AGXActivateGroupDelayCheck(int i, bool forceDir) //activate action group it forceDir direction, true = force activate with delay check
        {
            print("AGX Call: activate group for " + i + " for active vessel in dir " + forceDir);
            if (HighLogic.LoadedSceneIsFlight)
            {
                AGXFlight.ActivateActionGroup(i, true, forceDir);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AGXToggleGroupName(string grpName) //untested, 
        {
            try
            {
                print("AGX Call: toggle group by name for " + grpName + " for active vessel");
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    int grp = 0;
                    try
                    {
                        grp = AGXFlight.AGXguiNames.First(pair => pair.Value.ToLower() == grpName.ToLower()).Key; //compare strings, case does not matter
                    }
                    catch//poor man's error trap, name was not found. what is the correct way to do this?
                    {
                        return false;
                    }
                    if (grp >= 1 && grp <= 250) //check grp is valid
                    {
                        AGXFlight.ActivateActionGroupActivation(grp, false, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //not in flight
                {
                    return false;
                }
            }
            catch
            {
                print("AGX Call FAIL! Catch block hit");
                return false;
            }
        }

        public static bool AGXToggleGroupNameDelayCheck(string grpName) //untested
        {
            try
            {
                print("AGX Call: toggle group by name for " + grpName + " for active vessel");
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    int grp = 0;
                    try
                    {
                        grp = AGXFlight.AGXguiNames.First(pair => pair.Value.ToLower() == grpName.ToLower()).Key; //compare strings, case does not matter
                    }
                    catch//poor man's error trap, name was not found. what is the correct way to do this?
                    {
                        return false;
                    }
                    if (grp >= 1 && grp <= 250) //check grp is valid
                    {
                        AGXFlight.ActivateActionGroup(grp, false, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //not in flight
                {
                    return false;
                }
            }
            catch
            {
                print("AGX Call FAIL! Catch block hit");
                return false;
            }
        }

        public static bool AGX2VslToggleGroupName(uint flightID, string grpName, bool forceDir)
        {
            try
            {
                print("AGX Call: toggle group by name for " + grpName + " for " + flightID + " in dir " + forceDir);
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    if (FlightGlobals.ActiveVessel.rootPart.flightID == flightID)
                    {
                        return AGXToggleGroupName(grpName);
                    }
                    else
                    {
                        AGXOtherVessel otherVsl = new AGXOtherVessel(flightID);
                        otherVsl.ActivateActionStringActivation(grpName, false, false);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool AGX2VslToggleGroupNameDelayCheck(uint flightID, string grpName, bool forceDir)
        {
            try
            {
                print("AGX Call: toggle group by name for " + grpName + " for " + flightID + " in dir " + forceDir);
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    if (FlightGlobals.ActiveVessel.rootPart.flightID == flightID)
                    {
                        return AGXToggleGroupNameDelayCheck(grpName);
                    }
                    else
                    {
                        AGXOtherVessel otherVsl = new AGXOtherVessel(flightID);
                        otherVsl.ActivateActionString(grpName, false, false);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool AGX2VslActivateGroupNameDelayCheck(uint flightID, string grpName, bool forceDir)
        {
            try
            {
                print("AGX Call: activate group by name for " + grpName + " for " + flightID + " in dir " + forceDir);
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    if (FlightGlobals.ActiveVessel.rootPart.flightID == flightID)
                    {
                        return AGXActivateGroupNameDelayCheck(grpName, forceDir);
                    }
                    else
                    {
                        AGXOtherVessel otherVsl = new AGXOtherVessel(flightID);
                        otherVsl.ActivateActionString(grpName, true, forceDir);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool AGX2VslActivateGroupName(uint flightID, string grpName, bool forceDir)
        {
            try
            {
                print("AGX Call: activate group by name for " + grpName + " for " + flightID + " in dir " + forceDir);
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    if (FlightGlobals.ActiveVessel.rootPart.flightID == flightID)
                    {
                        return AGXActivateGroupName(grpName, forceDir);
                    }
                    else
                    {
                        AGXOtherVessel otherVsl = new AGXOtherVessel(flightID);
                        otherVsl.ActivateActionStringActivation(grpName, true, forceDir);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool AGXActivateGroupName(string grpName, bool forceDir) //untested
        {
            try
            {
                print("AGX Call: activate group by name for " + grpName + " for active vessel in dir " + forceDir);
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    int grp = 0;
                    try
                    {
                        grp = AGXFlight.AGXguiNames.First(pair => pair.Value.ToLower() == grpName.ToLower()).Key; //compare strings, case does not matter
                    }
                    catch//poor man's error trap, name was not found. what is the correct way to do this?
                    {
                        return false;
                    }
                    if (grp >= 1 && grp <= 250) //check grp is valid
                    {
                        AGXFlight.ActivateActionGroupActivation(grp, true, forceDir);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //not in flight
                {
                    return false;
                }
            }
            catch
            {
                print("AGX Call FAIL! Catch block hit");
                return false;
            }
        }

        public static bool AGXActivateGroupNameDelayCheck(string grpName, bool forceDir) //untested
        {
            try
            {
                print("AGX Call: activate group by name for " + grpName + " for active vessel in dir " + forceDir);
                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    int grp = 0;
                    try
                    {
                        grp = AGXFlight.AGXguiNames.First(pair => pair.Value.ToLower() == grpName.ToLower()).Key; //compare strings, case does not matter
                    }
                    catch//poor man's error trap, name was not found. what is the correct way to do this?
                    {
                        return false;
                    }
                    if (grp >= 1 && grp <= 250) //check grp is valid
                    {
                        AGXFlight.ActivateActionGroup(grp, true, forceDir);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //not in flight
                {
                    return false;
                }
            }
            catch
            {
                print("AGX Call FAIL! Catch block hit");
                return false;
            }
        }

        public static bool AGXToggleGroup(int i) //2 on test, works //toggle action group on activevessel
        {
            print("AGX Call: toggle group " + i + " for active vessel");
            if (HighLogic.LoadedSceneIsFlight)
            {
                AGXFlight.ActivateActionGroupActivation(i, false, false);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AGXToggleGroupDelayCheck(int i) //2 on test, works //toggle action group on activevessel
        {
            print("AGX Call: toggle group " + i + " for active vessel");
            if (HighLogic.LoadedSceneIsFlight)
            {
                AGXFlight.ActivateActionGroup(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AGXGroupState(int i) //is a group activated?
        {
            print("AGX Call: group gtate " + i + " for active vessel");
            if (HighLogic.LoadedSceneIsFlight)
            {
                try
                {

                    if (AGXFlight.ActiveActionsState.Find(g => g.group == i).actionOn)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return AGXFlight.groupActivatedState[i];
                }
            }
            else
            {
                return false;
            }
        }

        public static List<PartModule> AGX2VslListOfPartModulesInGroup(uint flightID, int i)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (flightID == FlightGlobals.ActiveVessel.rootPart.flightID)
                {
                    return AGXListOfPartModulesInGroup(i);
                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(flightID);
                    return otherVsl.pmListInGroup(i);
                }
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                if (EditorLogic.RootPart.flightID == flightID)
                {
                    return AGXListOfPartModulesInGroup(i);
                }
                else
                {
                    return new List<PartModule>();
                }
            }
            return new List<PartModule>();
        }

        public static List<Part> AGX2VslListOfPartsInGroup(uint flightID, int i)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (flightID == FlightGlobals.ActiveVessel.rootPart.flightID)
                {
                    return AGXListOfPartsInGroup(i);
                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(flightID);
                    return otherVsl.prtListInGroup(i);
                }
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                return AGXListOfPartsInGroup(i);
            }
            return new List<Part>();
        }

        public static List<PartModule> AGXListOfPartModulesInGroup(int i)
        {
            print("AGX Call: list of partModules in " + i + " for active vessel");
            if (HighLogic.LoadedSceneIsFlight)
            {
                List<PartModule> prtList = new List<PartModule>();
                foreach (AGXAction agAct in StaticData.CurrentVesselActions.Where(agx => agx.group == i))
                {
                    if (!prtList.Contains(agAct.ba.listParent.module))
                    {
                        prtList.Add(agAct.ba.listParent.module);
                    }
                }
                return prtList;
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                List<PartModule> prtList = new List<PartModule>();
                foreach (AGXAction agAct in StaticData.CurrentVesselActions.Where(agx => agx.group == i))
                {
                    if (!prtList.Contains(agAct.ba.listParent.module))
                    {
                        prtList.Add(agAct.ba.listParent.module);
                    }
                }
                return prtList;
            }
            else
            {
                return new List<PartModule>();
            }
        }

        public static List<Part> AGXListOfPartsInGroup(int i)
        {
            print("AGX Call: list of parts in " + i + " for active vessel");
            if (HighLogic.LoadedSceneIsFlight)
            {
                List<Part> prtList = new List<Part>();
                foreach (AGXAction agAct in StaticData.CurrentVesselActions.Where(agx => agx.group == i))
                {
                    if (!prtList.Contains(agAct.ba.listParent.part))
                    {
                        prtList.Add(agAct.ba.listParent.part);
                    }
                }
                return prtList;
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                List<Part> prtList = new List<Part>();
                foreach (AGXAction agAct in StaticData.CurrentVesselActions.Where(agx => agx.group == i))
                {
                    if (!prtList.Contains(agAct.ba.listParent.part))
                    {
                        prtList.Add(agAct.ba.listParent.part);
                    }
                }
                return prtList;
            }
            else
            {
                return new List<Part>();
            }
        }

        public static Dictionary<int,string> AGXListOfAssignedGroups()
        {
            print("AGX Call: list of groups with assigned actions");
            if (HighLogic.LoadedSceneIsFlight)
            {
                Dictionary<int,string> grpList = new Dictionary<int,string>();
                foreach (AGXAction agAct in StaticData.CurrentVesselActions)
                {
                    if(!grpList.ContainsKey(agAct.group))
                    //if (!grpList.Concat(agAct.group))
                    {
                        grpList.Add(agAct.group,AGXFlight.AGXguiNames[agAct.group]);
                    }
                }
                //dictionary isn't sortable, you sort the results when you access it which has to happen in the other mod. 
                return grpList;
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                Dictionary<int, string> grpList = new Dictionary<int, string>();
                foreach (AGXAction agAct in StaticData.CurrentVesselActions)
                {
                    if (!grpList.ContainsKey(agAct.group))
                    {
                        grpList.Add(agAct.group, AGXEditor.AGXguiNames[agAct.group]);
                    }
                }
                return grpList;
            }
            else
            {
                return new Dictionary<int, string>();
            }
        }

        public static Dictionary<int,string> AGX2VslListOfAssignedGroups(uint flightID)
        {
            print("AGX Call: list of groups with assigned actions on vessel " + flightID);
            try
            {

                if (HighLogic.LoadedSceneIsFlight) //only workes in flight
                {
                    if (FlightGlobals.ActiveVessel.rootPart.flightID == flightID)
                    {
                        return AGXListOfAssignedGroups();
                    }
                    else
                    {
                        AGXOtherVessel otherVsl = new AGXOtherVessel(flightID);
                        Dictionary<int, string> grpList = new Dictionary<int, string>();
                        foreach (AGXAction agAct in otherVsl.actionsList)
                        {
                            if (!grpList.ContainsKey(agAct.group))
                            {
                                grpList.Add(agAct.group, AGXEditor.AGXguiNames[agAct.group]);
                            }
                        }
                        return grpList;

                    }
                }
                else if (HighLogic.LoadedSceneIsEditor)
                {
                    return AGXListOfAssignedGroups();
                }

                else
                {
                    return new Dictionary<int, string>();
                }
            }
            catch
            {
                return new Dictionary<int, string>();
            }
        }



    }

    //Being internal methods that are not intended for external interface. (Yay for evolving code.)
    public class AGXAction : IEquatable<AGXAction>, IEqualityComparer<AGXAction> //basic data class for AGX mod, used everywhere
    {
        public Part prt;
        public BaseAction ba;
        public int group;
        public bool activated = false;
        public string grpName = "";

        public override bool Equals(object o)
        {
            Log.Info("Obj compare");
            if (o == null)
            {
                Log.Info("Obj compare null");
                return false;
            }
            AGXAction agxCheck = o as AGXAction;
            Log.Info("compare1a" + this.prt.partInfo.name + " " + this.ba.guiName + " " + this.group);
            // Log.Info("compare2a" + agxCheck.prt.partInfo.name + " " + agxCheck.ba.guiName + " " + agxCheck.group);
            if ((object)agxCheck == null)
            {
                return false;
            }
            if (this.prt == agxCheck.prt && this.ba == agxCheck.ba && this.group == agxCheck.group)
            {
                return true;
            }
            return false;
        }
        public bool Equals(AGXAction obj)
        {
            //print("AGX Compare");
            Log.Info("compare1" + this.prt.partInfo.name + this.prt.GetHashCode() + " " + this.ba.guiName + this.ba.GetHashCode() + " " + this.group);
            Log.Info("compare2" + obj.prt.partInfo.name +  obj.prt.GetHashCode() + " " + obj.ba.guiName +  obj.ba.GetHashCode() + " " + obj.group);
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (this.prt.Equals(obj.prt) && this.ba.Equals(obj.ba) && this.group.Equals(obj.group))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool Equals(AGXAction obj1, AGXAction obj2)
        {
            //print("AGX Compare");
            Log.Info("compare1c" + obj1.prt.partInfo.name + " " + obj1.ba.guiName + " " + obj1.group);
            Log.Info("compare2c" + obj2.prt.partInfo.name + " " + obj2.ba.guiName + " " + obj2.group);
            if (obj1 == null)
            {
                return false;
            }
            if (obj2 == null)
            {
                return false;
            }
            else
            {
                if (obj1.prt == obj2.prt && obj1.ba == obj2.ba && obj1.group == obj2.group)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override int GetHashCode()
        {
            Log.Info("get hash local");
            return ((int)ba.GetHashCode() + (int)prt.GetHashCode()) ^ group;
        }
        public int GetHashCode(AGXAction obj)
        {
            Log.Info("get has remote");
            return ((int)obj.ba.GetHashCode() + (int)obj.prt.GetHashCode()) ^ obj.group;
        }

        public override string ToString()
        {
            string str = "";
            if (prt == null)
            {
                str = str + "PartNull";
            }
            else
            {
                str = str + prt.partInfo.name;
            }
            if (ba == null)
            {
                str = str + ":BANull";
            }
            else
            {
                str = str + ":" + ba.name;
            }
            //if (group == null)
            //{
            //    str = str + ":GRPnull";
            //}
            //else
            //{
            str = str + ":" + group.ToString();
            //}
            return str;
        }
    }
    public class AGXDefaultCheck : MonoBehaviour //used in Editor to monitor default action groups
    {
        public BaseAction ba;
        public KSPActionGroup agrp;
    }
    public class AGXCooldown : MonoBehaviour
    {
        public uint vslFlightID;
        public int actGroup;
        public int delayLeft;

        public AGXCooldown(uint ID, int grp, int delay)
        {
            vslFlightID = ID;
            actGroup = grp;
            delayLeft = delay;
        }
    }
    //End Internal methods

    //Dedicated external links for RemoteTech, usable by other mods but I can't think of a scenario where they would be
    public static class AGXRemoteTechLinks
    {
        public static double RTTimeDelay(Vessel vsl) //gets delay for directly activated actions
        {
            try
            {
                Type calledType = Type.GetType("RemoteTech.API.API, RemoteTech");
                return (double)calledType.InvokeMember("GetShortestSignalDelay", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { vsl.id });

            }
            catch
            {
                return 0;
            }
        }

        public static bool RTConnection(Vessel vsl) //is this vessel connected?
        {
            try
            {
                Type calledType = Type.GetType("RemoteTech.API.API, RemoteTech");
                return (bool)calledType.InvokeMember("HasAnyConnection", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { vsl.id });

            }
            catch
            {
                return false;
            }
        }

        public static bool InLocal(Vessel vsl) //is this vessel in local control?
        {
            try
            {
                Type calledType = Type.GetType("RemoteTech.API.API, RemoteTech");
                return (bool)calledType.InvokeMember("InLocalControl", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { vsl.id });

            }
            catch
            {
                return false;
            }
        }

        public static bool RTDataReceive(ConfigNode node) //receive data back from RT
        {
            Log.Info("Call: RemoteTechCallback");
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == Convert.ToUInt32(node.GetValue("FlightID")))
                {

                    AGXFlight.ActivateActionGroupActivation(Convert.ToInt32(node.GetValue("Group")), Convert.ToBoolean(node.GetValue("Force")), Convert.ToBoolean(node.GetValue("ForceDir")));

                }
                else
                {
                    AGXOtherVessel otherVsl = new AGXOtherVessel(Convert.ToUInt32(node.GetValue("FlightID")));
                    otherVsl.ActivateActionGroupActivation(Convert.ToInt32(node.GetValue("Group")), Convert.ToBoolean(node.GetValue("Force")), Convert.ToBoolean(node.GetValue("ForceDir")));
                }
            }
            else
            {
                // ScreenMessages.PostScreenMessage("AGX Action Not Activated, Remotetech passed invalid vessel", 10F, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(Localizer.Format("#AGEXT_UI_SCREEN_MESSAGE_5"), 10F, ScreenMessageStyle.UPPER_CENTER);
            }
            return false;
        }
    }

}



