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

    public class AGXAction : MonoBehaviour
    {
        public Part prt;
        public BaseAction ba;
        public int group;
        public bool activated = false;
    }



}

