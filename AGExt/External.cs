using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.IO;

using UnityEngine;

namespace ActionGroupsExtended
{

    public class AGExtExternal : MonoBehaviour
    {


        public static bool AGXInstalled() //works
        {

            return true;
        }


        public static List<BaseAction> AGX2VslAllActions(uint FlightID) //works //all actions on specific vessel
        {
            print("AGX Call: List all actions for vessel " + FlightID);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    return AGXFlight.GetActionsList();

                }
                else
                {
                    ScreenMessages.PostScreenMessage("AGX Action Fail, other vessels not implemented yet", 10F, ScreenMessageStyle.UPPER_CENTER);
                    return new List<BaseAction>();
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                return new List<BaseAction>();
            }
        } 

        public static List<BaseAction> AGX2VslGroupActions(uint FlightID, int group) //works //all actions in group on specific vessel
        {
            print("AGX Call: List actions in group for vessel " + FlightID + " " + group);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    return AGXFlight.GetActionsList(group);
                    
                }
                else
                {
                    ScreenMessages.PostScreenMessage("AGX Action Fail, other vessels not implemented yet", 10F, ScreenMessageStyle.UPPER_CENTER);
                    return new List<BaseAction>();
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                return new List<BaseAction>();
            }
        }

        public static List<BaseAction> AGXGroupActions(int group) //works //all actions on ActiveVessel in group
        {
            print("AGX Call: List actions in active vessel in group " + group); //works
            return AGXFlight.GetActionsList(group);
        }

        public static List<BaseAction> AGXAllActions()//works //all actions on activevessel
        {
            print("AGX Call: List all actions for active vessel");
            return AGXFlight.GetActionsList();
        } 

        public static bool AGX2VslToggleGroup(uint FlightID, int group) //7 on test, works
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
                    ScreenMessages.PostScreenMessage("AGX Action Fail, other vessels not implemented yet", 10F, ScreenMessageStyle.UPPER_CENTER);
                    return false;
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
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
                    ScreenMessages.PostScreenMessage("AGX Action Fail, other vessels not implemented yet", 10F, ScreenMessageStyle.UPPER_CENTER);
                    return false;
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }
        }
        
        public static bool AGX2VslActivateGroup(uint FlightID, int group, bool forceDir) //9 on test, works
        {
            print("AGX Call: Activate group for " + group + " for vessel " + FlightID + " in dir " + forceDir);
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.rootPart.flightID == FlightID)
                {
                    AGXFlight.ActivateActionGroup(group, true, forceDir);
                    return true;
                }
                else
                {
                    ScreenMessages.PostScreenMessage("AGX Action Fail, other vessels not implemented yet", 10F, ScreenMessageStyle.UPPER_CENTER);
                    return false;
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("AGX Action Not Activated, not in flight", 10F, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }
        }
        
        public static bool AGXActivateGroup(int i, bool forceDir) //works //activate action group it forceDir direction, true = force activate
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

        public static bool AGXToggleGroupName(string grpName) //untested
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
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public class AGXAction : MonoBehaviour //basic data class for AGX mod, used everywhere
    {
        public Part prt;
        public BaseAction ba;
        public int group;
        public bool activated = false;
        
    }

    public class AGXDefaultCheck : MonoBehaviour //used in Editor to monitor default action groups
    {
        public BaseAction ba;
        public KSPActionGroup agrp;
    }



}

//TO DO LIST


//-add staging


