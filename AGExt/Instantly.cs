using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class AGXMainMenu :PartModule
    {

        //abandoned module, no longer needed for key rebinding
        public void Start()
        {
            //AGXEventHandler.init();
            print("AGExt Ver. 2.2 loaded");

            
        }

    }

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AGXInstantly : PartModule
    {

     
        public void Start()
        {
            Debug.Log("AGX Firing event init");
            AGXEventHandler.myEventHandler = new ActionGroupsExtended.AGXEventHandler();
            AGXEventHandler.myEventHandler.init();


        }

    }
    //public static class StaticMethods
    //{
    //    public static GUISkin ourSkin;
    //    public static void initSkin()
    //    {
    //        ourSkin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
    //    }
    //}
}
