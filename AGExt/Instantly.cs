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

        private static ConfigNode AGExtNode; 
        public static Dictionary<int, KeyCode> AGXguiKeys;

        public void Start()
        {
            print("AGExt Ver. 1.25a loaded");
            //below no longer needed with InputLockManager
            //AGXguiKeys = new Dictionary<int, KeyCode>();
            //AGExtNode = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
            //LoadCurrentKeyBindings(); //load keyset1 keybindings to assign keys 1 to 10 from KSP settings. need to load entire keyset because it saves as a single string. loading only first 10 would lose keybinds on groups 11-250 when save happens
            //KeysWriteKSPtoAGX(); //copy over keybinds
            //SaveCurrentKeyBindings(); //save keyset1 back to disk

          

        }

        


        public static void SaveCurrentKeyBindings()
        {
            string SaveString = ""; //initialize empty string
            int KeyID = new int();
            KeyID = 1;
            while (KeyID <= 250) //this should be a for loop, not going to mess with it now though
            {
                if (AGXguiKeys[KeyID] != KeyCode.None) //only add to string if there is a keycode
                {
                    SaveString = SaveString + '\u2023' + KeyID.ToString("000") + AGXguiKeys[KeyID].ToString();
                }
                KeyID = KeyID + 1;
            }
            AGExtNode.SetValue("KeySet1", SaveString); //save string to confignode
            AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg"); //save config node to disk
        }

        //public void KeysWriteKSPtoAGX()
        //{

        //    AGXguiKeys[1] = GameSettings.CustomActionGroup1.primary;
        //    AGXguiKeys[2] = GameSettings.CustomActionGroup2.primary;
        //    AGXguiKeys[3] = GameSettings.CustomActionGroup3.primary;
        //    AGXguiKeys[4] = GameSettings.CustomActionGroup4.primary;
        //    AGXguiKeys[5] = GameSettings.CustomActionGroup5.primary;
        //    AGXguiKeys[6] = GameSettings.CustomActionGroup6.primary;
        //    AGXguiKeys[7] = GameSettings.CustomActionGroup7.primary;
        //    AGXguiKeys[8] = GameSettings.CustomActionGroup8.primary;
        //    AGXguiKeys[9] = GameSettings.CustomActionGroup9.primary;
        //    AGXguiKeys[10] = GameSettings.CustomActionGroup10.primary;
            
        //}

        public void LoadCurrentKeyBindings()
        {
            String LoadString = AGExtNode.GetValue("KeySet1");
            
            for (int i = 1; i <= 250; i++)
            {
                AGXguiKeys[i] = KeyCode.None;
            }
            
            if (LoadString.Length > 0)
            {
                
                while (LoadString[0] == '\u2023')
                {
                    LoadString = LoadString.Substring(1);

                    int KeyLength = new int();
                    int KeyIndex = new int();
                    KeyCode LoadKey = new KeyCode();
                    KeyLength = LoadString.IndexOf('\u2023');
                    
                    if (KeyLength == -1)
                    {

                        KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));

                        LoadString = LoadString.Substring(3);

                        LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString);
                        AGXguiKeys[KeyIndex] = LoadKey;
                    }
                    else
                    {
                        KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));
                        LoadString = LoadString.Substring(3);
                        LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString.Substring(0, KeyLength - 3));
                        LoadString = LoadString.Substring(KeyLength - 3);
                        AGXguiKeys[KeyIndex] = LoadKey;
                    }
                }
            }
        }
    }
}
