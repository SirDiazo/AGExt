using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;


namespace ActionGroupsExtended
{

    public class AGXMethods : PartModule
    {
        

        //public static void SettingsWindow(int WindowID)
        //{
        //    if(GUI.Button(new Rect(10, 25, 130, 40), "Show KeyCodes in\nFlight Window"))
        //    {
        //        AGXFlight.FlightWinShowKeycodes = !AGXFlight.FlightWinShowKeycodes;
        //    }


        //    GUI.DragWindow();
        //}
    }//AGXMethods closing bracket

    public class SettingsWindow : MonoBehaviour, IDrawable
    {
        public Rect SettingsWin = new Rect(0, 0, 150, 80);
        public Vector2 Draw(Vector2 position)
        {
            var oldSkin = GUI.skin;
            GUI.skin = HighLogic.Skin;

            SettingsWin.x = position.x;
            SettingsWin.y = position.y;

            GUI.Window(2233452, SettingsWin, DrawSettingsWin, "AGX Settings", AGXFlight.AGXWinStyle);
            //RCSlaWin = GUILayout.Window(42334567, RCSlaWin, DrawWin, (string)null, GUI.skin.box);
            //GUI.skin = oldSkin;

            return new Vector2(SettingsWin.width, SettingsWin.height);
        }

        public void DrawSettingsWin(int WindowID)
        {

            if (GUI.Button(new Rect(10, 25, 130, 25), "Show KeyCodes"))
            {
                AGXFlight.FlightWinShowKeycodes = !AGXFlight.FlightWinShowKeycodes;
                if (AGXFlight.FlightWinShowKeycodes)
                {
                    AGXFlight.AGExtNode.SetValue("FlightWinShowKeys", "1");
                }
                else
                {
                    AGXFlight.AGExtNode.SetValue("FlightWinShowKeys", "0");
                }
                AGXFlight.AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg"); 
            }

            if (GUI.Button(new Rect(10, 50, 130, 25), "Edit Actions"))
            {
                AGXFlight.ClickEditButton();
                //if (AGXFlight.ShowSelectedWin || ShowKeySetWin)
                //{

                //    SaveEverything();
                //    ShowSelectedWin = false;
                //    ShowKeySetWin = false;
                //}
                //else if (!ShowSelectedWin)
                //{
                //    ShowSelectedWin = true;
                //}
            }

            //GUI.DragWindow();

        }

        public void Update()
        {
            //print("Test");
        }
    }
}//name space closing bracket
