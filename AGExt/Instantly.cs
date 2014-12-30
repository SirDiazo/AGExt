using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AGXMainMenu :PartModule
    {

        //private static ConfigNode AGExtNode; 
        //public static Dictionary<int, KeyCode> AGXguiKeys;

        public void Start()
        {
            print("AGExt Ver. 1.28 loaded");
         

        }

        


        //public void LoadCurrentKeyBindings()
        //{
        //    String LoadString = AGExtNode.GetValue("KeySet1");
            
        //    for (int i = 1; i <= 250; i++)
        //    {
        //        AGXguiKeys[i] = KeyCode.None;
        //    }
            
        //    if (LoadString.Length > 0)
        //    {
                
        //        while (LoadString[0] == '\u2023')
        //        {
        //            LoadString = LoadString.Substring(1);

        //            int KeyLength = new int();
        //            int KeyIndex = new int();
        //            KeyCode LoadKey = new KeyCode();
        //            KeyLength = LoadString.IndexOf('\u2023');
                    
        //            if (KeyLength == -1)
        //            {

        //                KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));

        //                LoadString = LoadString.Substring(3);

        //                LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString);
        //                AGXguiKeys[KeyIndex] = LoadKey;
        //            }
        //            else
        //            {
        //                KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));
        //                LoadString = LoadString.Substring(3);
        //                LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString.Substring(0, KeyLength - 3));
        //                LoadString = LoadString.Substring(KeyLength - 3);
        //                AGXguiKeys[KeyIndex] = LoadKey;
        //            }
        //        }
        //    }
        //}
    }
}
