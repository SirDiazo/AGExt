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

