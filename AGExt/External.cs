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


        public static bool AGXInstalled()
        {

            return true;
        }

        public static void AGXActivateGroup(int i)
        {

            if (HighLogic.LoadedSceneIsFlight)
            {
                AGXFlight.ActivateActionGroup(i);
            }
        }
        public static bool AGXGroupState(int i)
        {

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
//-Remove ModuleManger dependancy
//-check joystick keybinding assignment
//-add staging
//-shrink FlightWin keybind to Numpad 0
//add option to show/hide flightwin keybind
//add option to size flightwin