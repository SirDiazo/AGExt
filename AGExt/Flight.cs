using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AGXFlight : PartModule
    {
        //Selected Parts Window Variables
        public static Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
        public Rect SelPartsWin;
        public Rect FlightWin;
        private Vector2 ScrollPosSelParts;
        public Vector2 ScrollPosSelPartsActs;
        public Vector2 ScrollGroups;
        public Vector2 CurGroupsWin;
        public Vector2 FlightWinScroll;
        private List<AGXPart> AGEditorSelectedParts;
        private bool AGEEditorSelectedPartsSame = false;
        private static GUIStyle AGXWinStyle = null;
        private int SelectedPartsCount = 0;
        private bool ShowSelectedWin = false;
        private Part PreviousSelectedPart = null;
        private bool SelPartsIncSym = true;
        private string BtnTxt;
        private bool AGXDoLock = false;
        private Rect TestWin;
        private Part AGXRoot;

        private bool NeedToSave = false;
        private int GroupsPage = 1;
        private string CurGroupDesc;
        private bool AutoHideGroupsWin = false;
        private bool TempShowGroupsWin = false;
        private Rect KeyCodeWin;
        private Rect CurActsWin;
        private bool ShowKeyCodeWin = false;
        private bool ShowJoySticks = false;
        private bool ShowKeySetWin = false;
        private static int CurrentKeySet = 1;
        private string CurrentKeySetName;
        private Rect KeySetWin;
        private ConfigNode AGExtNode;
        string[] KeySetNames = new string[5];
        //private bool TrapMouse = false;
        private int LastPartCount = 0;
        public static List<AGXAction> CurrentVesselActions;

        private static Dictionary<int, bool> ActiveGroups;
        private List<KeyCode> ActiveKeys;
        private IButton AGXBtn;





        public Rect GroupsWin;
        public bool Trigger;
        private bool Trigger2;
        private int Value = 0;
        private string HexStr;

        private List<BaseAction> PartActionsList;



        private bool ShipListOk = false;
        Texture2D BtnTexRed = new Texture2D(1, 1);
        Texture2D BtnTexGrn = new Texture2D(1, 1);
        public static Dictionary<int, string> AGXguiNames;
        public static Dictionary<int, KeyCode> AGXguiKeys;
        public int AGXCurActGroup = 1;
        List<string> KeyCodeNames = new List<string>();
        List<string> JoyStickCodes = new List<string>();
        private bool ActionsListDirty = true; //is our actions requiring update?
        private bool LoadGroupsOnceCheck = false;
        private bool ShowCurActsWin = true;
        private bool ShowAGXMod = true;
        private bool ShowAGXFlightWin = true;
        private string ActivatedGroups = "";
        public static bool loadFinished = false;
        private bool AGXShow = true;








        public void Start()
        {
            //foreach (Part p in 
            ActiveKeys = new List<KeyCode>();
            KSPActs[1] = KSPActionGroup.Custom01;
            KSPActs[2] = KSPActionGroup.Custom02;
            KSPActs[3] = KSPActionGroup.Custom03;
            KSPActs[4] = KSPActionGroup.Custom04;
            KSPActs[5] = KSPActionGroup.Custom05;
            KSPActs[6] = KSPActionGroup.Custom06;
            KSPActs[7] = KSPActionGroup.Custom07;
            KSPActs[8] = KSPActionGroup.Custom08;
            KSPActs[9] = KSPActionGroup.Custom09;
            KSPActs[10] = KSPActionGroup.Custom10;

            TestWin = new Rect(600, 300, 100, 100);
            RenderingManager.AddToPostDrawQueue(0, AGXOnDraw);
            AGEditorSelectedParts = new List<AGXPart>();
            PartActionsList = new List<BaseAction>();
            ScrollPosSelParts = Vector2.zero;
            ScrollPosSelPartsActs = Vector2.zero;
            ScrollGroups = Vector2.zero;
            CurGroupsWin = Vector2.zero;

            //AGXVsl = new AGXVessel();

            AGXWinStyle = new GUIStyle(HighLogic.Skin.window);

            BtnTexRed.SetPixel(0, 0, new Color(1, 0, 0, .5f));
            BtnTexRed.Apply();
            BtnTexGrn.SetPixel(0, 0, new Color(0, 1, 0, .5f));
            BtnTexGrn.Apply();

            AGXguiNames = new Dictionary<int, string>();


            AGXguiKeys = new Dictionary<int, KeyCode>();



            for (int i = 1; i <= 250; i = i + 1)
            {
                AGXguiNames[i] = "";
                AGXguiKeys[i] = KeyCode.None;
            }


            KeyCodeNames = new List<String>();
            KeyCodeNames.AddRange(Enum.GetNames(typeof(KeyCode)));
            KeyCodeNames.Remove("None");
            JoyStickCodes.AddRange(KeyCodeNames.Where(JoySticks));
            KeyCodeNames.RemoveAll(JoySticks);
            AGExtNode = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
            CurrentKeySet = Convert.ToInt32(AGExtNode.GetValue("ActiveKeySet"));
            CurrentKeySetName = AGExtNode.GetValue("KeySetName" + CurrentKeySet);
            CurrentVesselActions = new List<AGXAction>();
            AGXRoot = null;
            GroupsWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltGroupsX")), Convert.ToInt32(AGExtNode.GetValue("FltGroupsY")), 250, 530);
            SelPartsWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltSelPartsX")), Convert.ToInt32(AGExtNode.GetValue("FltSelPartsY")), 365, 270);
            KeyCodeWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltKeyCodeX")), Convert.ToInt32(AGExtNode.GetValue("FltKeyCodeY")), 410, 730);
            KeySetWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltKeySetX")), Convert.ToInt32(AGExtNode.GetValue("FltKeySetY")), 185, 185);
            CurActsWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltCurActsX")), Convert.ToInt32(AGExtNode.GetValue("FltCurActsY")), 345, 140);
            FlightWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltMainX")), Convert.ToInt32(AGExtNode.GetValue("FltMainY")), 215, 100);
            ActiveGroups = new Dictionary<int, bool>();
            
            UnbindDefaultKeys();

            if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
            {


                AGXBtn = ToolbarManager.Instance.add("AGX", "AGXBtn");
                AGXBtn.TexturePath = "Diazo/AGExt/icon_button";
                AGXBtn.ToolTip = "Action Groups Extended";
                AGXBtn.OnClick += (e) =>
                {
                    ShowAGXMod = !ShowAGXMod;
                };
            }

            if (AGExtNode.GetValue("FltShow") == "0")
            {
                ShowAGXMod = false;
            }
            

           
           
        }

        public void LoadEverything()
        {
            foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
            {
                CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));
                //ActivatedGroups = (string)pm.Fields.GetValue("AGXActivated");

            }
            if (CurrentKeySet == 0)
            {
                CurrentKeySet = 1;
            }

            LoadGroupNames();
            LoadCurrentKeyBindings();
            //LoadActivatedGroups();
            loadFinished = true;
        }

      


        private void UnbindDefaultKeys()
        {
           //Unbind default keys from Custom 1 through 10. Note this does not save across sessions and so gets reapplied.
            GameSettings.CustomActionGroup1.primary = KeyCode.None;
            GameSettings.CustomActionGroup1.secondary = KeyCode.None;
            GameSettings.CustomActionGroup2.primary = KeyCode.None;
            GameSettings.CustomActionGroup2.secondary = KeyCode.None;
            GameSettings.CustomActionGroup3.primary = KeyCode.None;
            GameSettings.CustomActionGroup3.secondary = KeyCode.None;
            GameSettings.CustomActionGroup4.primary = KeyCode.None;
            GameSettings.CustomActionGroup4.secondary = KeyCode.None;
            GameSettings.CustomActionGroup5.primary = KeyCode.None;
            GameSettings.CustomActionGroup5.secondary = KeyCode.None;
            GameSettings.CustomActionGroup6.primary = KeyCode.None;
            GameSettings.CustomActionGroup6.secondary = KeyCode.None;
            GameSettings.CustomActionGroup7.primary = KeyCode.None;
            GameSettings.CustomActionGroup7.secondary = KeyCode.None;
            GameSettings.CustomActionGroup8.primary = KeyCode.None;
            GameSettings.CustomActionGroup8.secondary = KeyCode.None;
            GameSettings.CustomActionGroup9.primary = KeyCode.None;
            GameSettings.CustomActionGroup9.secondary = KeyCode.None;
            GameSettings.CustomActionGroup10.primary = KeyCode.None;
            GameSettings.CustomActionGroup10.secondary = KeyCode.None;
            
        }
 

        public void SaveWindowPositions()
        {
            AGExtNode.SetValue("FltGroupsX", GroupsWin.x.ToString());
            AGExtNode.SetValue("FltGroupsY", GroupsWin.y.ToString());
            AGExtNode.SetValue("FltSelPartsX", SelPartsWin.x.ToString());
            AGExtNode.SetValue("FltSelPartsY", SelPartsWin.y.ToString());
            AGExtNode.SetValue("FltKeyCodeX", KeyCodeWin.x.ToString());
            AGExtNode.SetValue("FltKeyCodeY", KeyCodeWin.y.ToString());
            AGExtNode.SetValue("FltKeySetX", KeySetWin.x.ToString());
            AGExtNode.SetValue("FltKeySetY", KeySetWin.y.ToString());
            AGExtNode.SetValue("FltCurActsX", CurActsWin.x.ToString());
            AGExtNode.SetValue("FltCurActsY", FlightWin.y.ToString());
            AGExtNode.SetValue("FltMainX", FlightWin.x.ToString());
            AGExtNode.SetValue("FltMainY", CurActsWin.y.ToString());
            if (!ShowAGXMod)
            {
               //rint("No show");
                AGExtNode.SetValue("FltShow", "0");
            }
            else
            {
               // print("show");
                AGExtNode.SetValue("FltShow", "1");
            }
            AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
        }

        public void OnDisable()
        {
           
            SaveWindowPositions();
            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
            {
                AGXBtn.Destroy();
            }

        }
        
        public void SaveEverything()
        {
            
            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    SaveActionGroups(p);
            //}

           // SaveCurrentKeySet();
            SaveCurrentKeyBindings();
            
            //SaveGroupNames();
        SaveWindowPositions();
       

        }




        private static bool JoySticks(String s)
        {
            return s.StartsWith("Joystick");
        }



        public void AGXOnDraw()
        {

            //Vector3 RealMousePos = new Vector3();
            //RealMousePos = Input.mousePosition;
            //RealMousePos.y = Screen.height - Input.mousePosition.y;

    
            //TestWin = GUI.Window(673467791, TestWin, TestingWindow, "Test", AGXWinStyle);
            if (ShowAGXMod)
            {
                if (ShowAGXFlightWin)
                {
                    
                    FlightWin = GUI.Window(673467788, FlightWin, FlightWindow, "Actions", AGXWinStyle);
                    //TrapMouse |= FlightWin.Contains(RealMousePos);
                }
               
                    if (ShowKeySetWin)
                    {
                        KeySetWin = GUI.Window(673467792, KeySetWin, KeySetWindow, "Keysets", AGXWinStyle);
                        //TrapMouse = KeySetWin.Contains(RealMousePos);
                        //ShowSelectedWin = false;
                        if (!AutoHideGroupsWin)
                        {
                            GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", AGXWinStyle);
                        }
                        ShowCurActsWin = false;

                    }

                    if (ShowSelectedWin)
                    {

                        SelPartsWin = GUI.Window(673467794, SelPartsWin, SelParts, "AGExt Selected parts: " + AGEditorSelectedParts.Count(), AGXWinStyle);
                        ShowCurActsWin = true;
                        //TrapMouse = SelPartsWin.Contains(RealMousePos);
                        if (AutoHideGroupsWin && !TempShowGroupsWin)
                        {
                        }
                        else
                        {
                            GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", AGXWinStyle);
                           // TrapMouse |= GroupsWin.Contains(RealMousePos);
                        }

                        if (ShowKeyCodeWin)
                        {
                            KeyCodeWin = GUI.Window(673467793, KeyCodeWin, KeyCodeWindow, "Keycodes", AGXWinStyle);
                            //TrapMouse |= KeyCodeWin.Contains(RealMousePos);
                        }

                    }
                    if (ShowCurActsWin && ShowSelectedWin)
                    {
                        CurActsWin = GUI.Window(673467790, CurActsWin, CurrentActionsWindow, "Actions (This group): " + CurrentVesselActions.FindAll(p => p.group == AGXCurActGroup).Count.ToString(), AGXWinStyle);
                       // TrapMouse |= CurActsWin.Contains(RealMousePos);
                    }
                
            }
        }

        public static void ActivateActionGroup(int group)
        {

           
        
            foreach (AGXAction agAct in CurrentVesselActions.Where(agx => agx.group == group))
            {
               
                if (agAct.activated)
                {
                    KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Deactivate);
                    agAct.activated = false;
                    agAct.ba.Invoke(actParam);
                }
                else
                {
                    KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate);
                    agAct.activated = true;
                    agAct.ba.Invoke(actParam);
                    
                }
               
            }
           
        }

        public void FlightWindow(int WindowID)
        {
            
            if (GUI.Button(new Rect(150, 5, 60, 20), "Edit"))
            {

                if (ShowSelectedWin || ShowKeySetWin)
                {
                  
                    SaveEverything();
                    ShowSelectedWin = false;
                    ShowKeySetWin = false;
                }
                else if (!ShowSelectedWin)
                {
                    ShowSelectedWin = true;
                }
            }
           
            List<int> ActiveActions = new List<int>();
            for (int i = 1; i <= 250; i = i + 1)
            {
                if(CurrentVesselActions.Any(a => a.group == i))
                {
                 
                    ActiveActions.Add(i);
                   

                   
                }
                if (ActiveActions.Count > 0)
                {
                    ActiveKeys.Clear();
                    foreach (int i2 in ActiveActions)
                    {
                        ActiveKeys.Add(AGXguiKeys[i2]);
                    }
                }
            }

            GUI.Box(new Rect(5, 25, 190, Math.Min(410,10+(20*Math.Max(1,ActiveActions.Count)))), "");
            FlightWinScroll = GUI.BeginScrollView(new Rect(10, 30, 200, Math.Min(400,20+(20*(ActiveActions.Count-1)))), FlightWinScroll, new Rect(0, 0, 180, (20 * (ActiveActions.Count))));
            FlightWin.height = Math.Min( Math.Max(60,40+(20*ActiveActions.Count)),440);
           

            if (ActiveActions.Count > 0)
            {
                
                for (int i2 = 1; i2 <= ActiveActions.Count; i2 = i2 + 1)
                {
                    
                    TextAnchor TxtAnch4 = new TextAnchor();

                    TxtAnch4 = GUI.skin.button.alignment;

                    GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    if (GUI.Button(new Rect(0, 0 + (20 * (i2 - 1)), 110, 20), ActiveActions.ElementAt((i2 - 1)) + ": " + AGXguiNames[ActiveActions.ElementAt((i2 - 1))]))
                    {
                        
                        ActivateActionGroup(ActiveActions.ElementAt(i2 - 1));
                    }
                    GUI.skin.button.alignment = TxtAnch4;
                    if (GUI.Button(new Rect(110, 0 + (20 * (i2 - 1)), 70, 20), AGXguiKeys[ActiveActions.ElementAt((i2 - 1))].ToString()))
                    {

                        ActivateActionGroup(ActiveActions.ElementAt(i2 - 1));
                    }
                }
            }
                GUI.EndScrollView();
            if(ActiveActions.Count ==0)
            {
              
                GUI.Label(new Rect(40, 30, 150,20), "No actions available");
            }
          
                
            GUI.DragWindow();
       
        }
        public void SaveCurrentVesselActions()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
                {
                    agpm.partAGActions.Clear();
                    agpm.partAGActions.AddRange(CurrentVesselActions.Where(agp => agp.prt == p));
                }
            }
        }
        public void CurrentActionsWindow(int WindowID)
        {
            List<AGXAction> ThisGroupActions = new List<AGXAction>();
            ThisGroupActions.AddRange(CurrentVesselActions.Where(p => p.group == AGXCurActGroup));
            GUI.Box(new Rect(5, 25, 310, 110), "");
            CurGroupsWin = GUI.BeginScrollView(new Rect(10, 30, 330, 100), CurGroupsWin, new Rect(0, 0, 310, Math.Max(100, 0 + (20 * (ThisGroupActions.Count)))));
            int RowCnt = new int();
            RowCnt = 1;

            if (ThisGroupActions.Count > 0)
            {
                while (RowCnt <= ThisGroupActions.Count)
                {

                    TextAnchor TxtAnch4 = new TextAnchor();

                    TxtAnch4 = GUI.skin.button.alignment;

                    GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    if (GUI.Button(new Rect(0, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).group.ToString() + ": " + AGXguiNames[ThisGroupActions.ElementAt(RowCnt - 1).group]))
                    {
                        AGXAction agtemp = new AGXAction();
                        agtemp = ThisGroupActions.ElementAt(RowCnt - 1);
                        foreach (ModuleAGExtData agpm in ThisGroupActions.ElementAt(RowCnt - 1).prt.Modules.OfType<ModuleAGExtData>())
                        {
                            int agcnt = new int();
                            agcnt = 0;
                            foreach (AGXAction agxact in agpm.partAGActions)
                            {
                                if (agxact.group == agtemp.group && agxact.ba.name == agtemp.ba.name)
                                {
                                    agpm.partAGActions.RemoveAt(agcnt);
                                    goto BreakOut;
                                }
                                agcnt = agcnt + 1;
                            }
                        }
                    BreakOut:
                        print("ActionDeleted");
                    }

                    if (GUI.Button(new Rect(100, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).prt.partInfo.title))
                    {
                        AGXAction agtemp = new AGXAction();
                        agtemp = ThisGroupActions.ElementAt(RowCnt - 1);
                        foreach (ModuleAGExtData agpm in ThisGroupActions.ElementAt(RowCnt - 1).prt.Modules.OfType<ModuleAGExtData>())
                        {
                            int agcnt = new int();
                            agcnt = 0;
                            foreach (AGXAction agxact in agpm.partAGActions)
                            {
                                if (agxact.group == agtemp.group && agxact.ba.name == agtemp.ba.name)
                                {
                                    agpm.partAGActions.RemoveAt(agcnt);
                                    goto BreakOut;
                                }
                                agcnt = agcnt + 1;
                            }
                        }
                    BreakOut:
                        print("ActionDeleted");
                    }
                    try
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).ba.guiName))
                        {
                            AGXAction agtemp = new AGXAction();
                            agtemp = ThisGroupActions.ElementAt(RowCnt - 1);
                            foreach (ModuleAGExtData agpm in ThisGroupActions.ElementAt(RowCnt - 1).prt.Modules.OfType<ModuleAGExtData>())
                            {
                                int agcnt = new int();
                                agcnt = 0;
                                foreach (AGXAction agxact in agpm.partAGActions)
                                {
                                    if (agxact.group == agtemp.group && agxact.ba.name == agtemp.ba.name)
                                    {
                                        agpm.partAGActions.RemoveAt(agcnt);
                                        goto BreakOut;
                                    }
                                    agcnt = agcnt + 1;
                                }
                            }
                        BreakOut:
                            print("ActionDeleted");
                        }
                    }
                    catch
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), "Error"))
                        {
                            AGXAction agtemp = new AGXAction();
                            agtemp = ThisGroupActions.ElementAt(RowCnt - 1);
                            foreach (ModuleAGExtData agpm in ThisGroupActions.ElementAt(RowCnt - 1).prt.Modules.OfType<ModuleAGExtData>())
                            {
                                int agcnt = new int();
                                agcnt = 0;
                                foreach (AGXAction agxact in agpm.partAGActions)
                                {
                                    if (agxact.group == agtemp.group && agxact.ba.name == agtemp.ba.name)
                                    {
                                        agpm.partAGActions.RemoveAt(agcnt);
                                        goto BreakOut;
                                    }
                                    agcnt = agcnt + 1;
                                }
                            }
                        BreakOut:
                            print("ActionDeleted");
                        }
                    }

                    GUI.skin.button.alignment = TxtAnch4;
                    RowCnt = RowCnt + 1;
                }
            }
            else
            {
                TextAnchor TxtAnch5 = new TextAnchor();

                TxtAnch5 = GUI.skin.label.alignment;

                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(10, 30, 274, 30), "No actions");
                GUI.skin.label.alignment = TxtAnch5;
            }
            GUI.EndScrollView();

            GUI.DragWindow();
        }
       
      
        public void KeySetWindow(int WindowID)
        {



            GUI.DrawTexture(new Rect(6, (CurrentKeySet * 25) + 1, 68, 18), BtnTexGrn);

            if (GUI.Button(new Rect(5, 25, 70, 20), "Select 1:"))
            {
                
                SaveCurrentKeyBindings();
               
                CurrentKeySet = 1;
               
                //SaveCurrentKeySet();
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                    {
                        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                        {
                            pm.AGXKeySet=CurrentKeySet.ToString();
                        }
                    }
                }
                LoadCurrentKeyBindings();
               
            }
            KeySetNames[0] = GUI.TextField(new Rect(80, 25, 100, 20), KeySetNames[0]);

            if (GUI.Button(new Rect(5, 50, 70, 20), "Select 2:"))
            {

                SaveCurrentKeyBindings();
                CurrentKeySet = 2;
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                    {
                        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                        {
                            pm.AGXKeySet=CurrentKeySet.ToString();
                        }
                    }
                }
                LoadCurrentKeyBindings();
            }
            KeySetNames[1] = GUI.TextField(new Rect(80, 50, 100, 20), KeySetNames[1]);
            if (GUI.Button(new Rect(5, 75, 70, 20), "Select 3:"))
            {

                SaveCurrentKeyBindings();
                CurrentKeySet = 3;
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                    {
                        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                        {
                            pm.AGXKeySet=CurrentKeySet.ToString();
                        }
                    }
                }
                LoadCurrentKeyBindings();
            }
            KeySetNames[2] = GUI.TextField(new Rect(80, 75, 100, 20), KeySetNames[2]);
            if (GUI.Button(new Rect(5, 100, 70, 20), "Select 4:"))
            {
                SaveCurrentKeyBindings();
                CurrentKeySet = 4;
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                    {
                        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                        {
                            pm.AGXKeySet=CurrentKeySet.ToString();
                        }
                    }
                }
                LoadCurrentKeyBindings();
            }
            KeySetNames[3] = GUI.TextField(new Rect(80, 100, 100, 20), KeySetNames[3]);
            if (GUI.Button(new Rect(5, 125, 70, 20), "Select 5:"))
            {
                SaveCurrentKeyBindings();
                CurrentKeySet = 5;
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                    {
                        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                        {
                            pm.AGXKeySet=CurrentKeySet.ToString();
                        }
                    }
                }
                LoadCurrentKeyBindings();
            }
            KeySetNames[4] = GUI.TextField(new Rect(80, 125, 100, 20), KeySetNames[4]);
            if (GUI.Button(new Rect(5, 150, 175, 30), "Close Window"))
            {

                AGExtNode.SetValue("KeySetName1", KeySetNames[0]);
                AGExtNode.SetValue("KeySetName2", KeySetNames[1]);
                AGExtNode.SetValue("KeySetName3", KeySetNames[2]);
                AGExtNode.SetValue("KeySetName4", KeySetNames[3]);
                AGExtNode.SetValue("KeySetName5", KeySetNames[4]);
                CurrentKeySetName = KeySetNames[CurrentKeySet - 1];
                AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                ShowKeySetWin = false;
                ShowSelectedWin = true;
            }

            GUI.DragWindow();
        }

        public static string SaveCurrentKeySet(Part p, string curKeySet)
        {


            if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
            {
                return CurrentKeySet.ToString();

            }
            else
            {
                return curKeySet;
            }
            
            

          
        }

        public void LoadCurrentKeySet()
        {

           
                foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
                {

                  
                    CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));

                }
         
            
          
            if (CurrentKeySet >= 1 && CurrentKeySet <= 5)
            {
          
            }
            else
            {
                CurrentKeySet = 1;
              
            }
           
       
            CurrentKeySetName = AGExtNode.GetValue("KeySetName" + CurrentKeySet);

        }

        public void LoadCurrentKeyBindings()
        {



            String LoadString = AGExtNode.GetValue("KeySet" + CurrentKeySet.ToString());

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

        public void SaveCurrentKeyBindings()
        {

       
            AGExtNode.SetValue("KeySetName" + CurrentKeySet, CurrentKeySetName);
            string SaveString = "";
            int KeyID = new int();
            KeyID = 1;
            while (KeyID <= 250)
            {
                if (AGXguiKeys[KeyID] != KeyCode.None)
                {
                    SaveString = SaveString + '\u2023' + KeyID.ToString("000") + AGXguiKeys[KeyID].ToString();
                }
                KeyID = KeyID + 1;
            }

            AGExtNode.SetValue("KeySet" + CurrentKeySet.ToString(), SaveString);
            AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");

            
        }
        public void KeyCodeWindow(int WindowID)
        {
            if (GUI.Button(new Rect(5, 3, 100, 25), "None"))
            {
                AGXguiKeys[AGXCurActGroup] = KeyCode.None;
                ShowKeyCodeWin = false;

            }

            if (ShowJoySticks)
            {
                GUI.DrawTexture(new Rect(281, 3, 123, 18), BtnTexGrn);
            }

            if (GUI.Button(new Rect(280, 2, 125, 20), "Show JoySticks"))
            {
                ShowJoySticks = !ShowJoySticks;
            }
            if (!ShowJoySticks)
            {
                int KeyListCount = new int();
                KeyListCount = 0;
                while (KeyListCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (KeyListCount * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount)))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 69)
                {
                    if (GUI.Button(new Rect(105, 25 + ((KeyListCount - 35) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount)))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 104)
                {
                    if (GUI.Button(new Rect(205, 25 + ((KeyListCount - 70) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount)))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 139)
                {
                    if (GUI.Button(new Rect(305, 25 + ((KeyListCount - 105) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount)))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
            }
            else
            {
                int JoyStickCount = new int();
                JoyStickCount = 0;
                while (JoyStickCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (JoyStickCount * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount)))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 69)
                {
                    if (GUI.Button(new Rect(130, 25 + ((JoyStickCount - 35) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount)))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 99)
                {
                    if (GUI.Button(new Rect(255, 25 + ((JoyStickCount - 70) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount)))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
            }
            GUI.DragWindow();
        }

        public void SelParts(int WindowID)
        {



            SelectedPartsCount = AGEditorSelectedParts.Count;
            int SelPartsLeft = new int();
            SelPartsLeft = -10;


            //GUI.DrawTexture(new Rect(25, 30, 80, PartsScrollHeight), TexBlk, ScaleMode.StretchToFill, false);
            //AGXPart FirstPart = new AGXPart();
            // FirstPart = AGEditorSelectedParts.First().AGPart.name;
            GUI.Box(new Rect(SelPartsLeft + 20, 25, 200, 110), "");
            if (AGEditorSelectedParts != null && AGEditorSelectedParts.Count > 0)
            {

                int ButtonCount = new int();
                ButtonCount = 1;

                ScrollPosSelParts = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 30, 220, 110), ScrollPosSelParts, new Rect(0, 0, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10));

                //GUI.Box(new Rect(SelPartsLeft, 25, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10), "");
                while (ButtonCount <= SelectedPartsCount)
                {

                    if (GUI.Button(new Rect(5, 0 + ((ButtonCount - 1) * 20), 190, 20), AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.partInfo.title))
                    {

                        AGEditorSelectedParts.RemoveAt(ButtonCount - 1);
                        if (AGEditorSelectedParts.Count == 0)
                        {
                            AGEEditorSelectedPartsSame = false;
                        }
                        return;
                    }

                    ButtonCount = ButtonCount + 1;
                }

                GUI.EndScrollView();



            }

            if (SelPartsIncSym)
            {
                GUI.DrawTexture(new Rect(SelPartsLeft + 246, 26, 108, 23), BtnTexGrn, ScaleMode.StretchToFill, false);
                BtnTxt = "Symmetry? Yes";
            }
            else
            {
                GUI.DrawTexture(new Rect(SelPartsLeft + 246, 26, 108, 23), BtnTexRed, ScaleMode.StretchToFill, false);
                BtnTxt = "Symmetry? No";
            }



            if (GUI.Button(new Rect(SelPartsLeft + 245, 25, 110, 25), BtnTxt))
            {
                SelPartsIncSym = !SelPartsIncSym;

            }

            if (GUI.Button(new Rect(SelPartsLeft + 245, 55, 110, 25), "Clear List"))
            {
                AGEditorSelectedParts.Clear();
                PartActionsList.Clear();
                AGEEditorSelectedPartsSame = false;

            }

            GUI.Box(new Rect(SelPartsLeft + 20, 150, 200, 110), "");
            
            if (AGEEditorSelectedPartsSame)
            {

                if (PartActionsList.Count > 0)
                {
                    int ActionsCount = new int();
                    int ActionsCountTotal = new int();
                    ActionsCount = 1;
                    ActionsCountTotal = PartActionsList.Count;
                    ScrollPosSelPartsActs = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 155, 220, 110), ScrollPosSelPartsActs, new Rect(0, 0, 200, (20 * Math.Max(5, ActionsCountTotal)) + 10));

                    while (ActionsCount <= ActionsCountTotal)
                    {

                        if (GUI.Button(new Rect(5, 0 + ((ActionsCount - 1) * 20), 190, 20), PartActionsList.ElementAt(ActionsCount - 1).guiName))
                        {
                            
                            int PrtCnt = new int();
                            PrtCnt = 0;
                            string baname = PartActionsList.ElementAt(ActionsCount - 1).name;

                            foreach (AGXPart agPrt in AGEditorSelectedParts)
                            {
                                
                                List<BaseAction> baList = new List<BaseAction>();
                                
                                foreach (ModuleAGExtData agpm in agPrt.AGPart.Modules.OfType<ModuleAGExtData>())
                                {
                                    
                                    baList.AddRange(agpm.partAllActions);
                                   
                                    
                                }
                                AGXAction ToAdd = new AGXAction() { prt = agPrt.AGPart, ba = baList.First(ba3 => ba3.name == baname) , group = AGXCurActGroup };
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(CurrentVesselActions);
                                Checking.RemoveAll(p => p.group != AGXCurActGroup);
                                Checking.RemoveAll(p => p.prt != AGEditorSelectedParts.ElementAt(PrtCnt).AGPart);
                                Checking.RemoveAll(p => p.ba.guiName != PartActionsList.ElementAt(ActionsCount - 1).guiName);
                                if (Checking.Count == 0)
                                {
                                    CurrentVesselActions.Add(ToAdd);
                                    SaveCurrentVesselActions();
                                }
                                PrtCnt = PrtCnt + 1;
                            }
                        }
                        ActionsCount = ActionsCount + 1;
                    }
                    GUI.EndScrollView();
                }
            }
            else
            {
                TextAnchor TxtAnch = new TextAnchor();

                TxtAnch = GUI.skin.label.alignment;

                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(SelPartsLeft + 20, 180, 190, 40), "Select parts of\nthe same type");



                GUI.skin.label.alignment = TxtAnch;

            }

            TextAnchor TxtAnch2 = new TextAnchor();
            TxtAnch2 = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            if (GUI.Button(new Rect(SelPartsLeft + 245, 130, 120, 30), AGXCurActGroup + ": " + AGXguiNames[AGXCurActGroup]))
            {
                TempShowGroupsWin = true;
            }
            GUI.skin.button.alignment = TxtAnch2;



            GUI.Label(new Rect(SelPartsLeft + 245, 160, 110, 20), "Description:");
            CurGroupDesc = AGXguiNames[AGXCurActGroup];
            CurGroupDesc = GUI.TextField(new Rect(SelPartsLeft + 245, 180, 120, 22), CurGroupDesc);
            AGXguiNames[AGXCurActGroup] = CurGroupDesc;
            GUI.Label(new Rect(SelPartsLeft + 245, 202, 110, 25), "Keybinding:");
            if (GUI.Button(new Rect(SelPartsLeft + 245, 222, 120, 20), AGXguiKeys[AGXCurActGroup].ToString()))
            {
                ShowKeyCodeWin = true;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 245, 244, 120, 20), CurrentKeySetName))
            {
                SaveCurrentKeyBindings();
                KeySetNames[0] = AGExtNode.GetValue("KeySetName1");
                KeySetNames[1] = AGExtNode.GetValue("KeySetName2");
                KeySetNames[2] = AGExtNode.GetValue("KeySetName3");
                KeySetNames[3] = AGExtNode.GetValue("KeySetName4");
                KeySetNames[4] = AGExtNode.GetValue("KeySetName5");
                KeySetNames[CurrentKeySet - 1] = CurrentKeySetName;
                ShowKeySetWin = true;
            }


            GUI.DragWindow();
        }

        public void GroupsWindow(int WindowID)
        {


            if (AutoHideGroupsWin)
            {
                GUI.DrawTexture(new Rect(6, 4, 78, 18), BtnTexRed);
            }
            if (GUI.Button(new Rect(5, 3, 80, 20), "Auto-Hide"))
            {
                AutoHideGroupsWin = !AutoHideGroupsWin;

            }
            bool[] PageGrn = new bool[5];
            foreach (AGXAction AGact in CurrentVesselActions)
            {
                if (AGact.group <= 50)
                {
                    PageGrn[0] = true;
                }
                if (AGact.group >= 51 && AGact.group <= 100)
                {
                    PageGrn[1] = true;
                }
                if (AGact.group >= 101 && AGact.group <= 150)
                {
                    PageGrn[2] = true;
                }
                if (AGact.group >= 151 && AGact.group <= 200)
                {
                    PageGrn[3] = true;
                }
                if (AGact.group >= 201 && AGact.group <= 250)
                {
                    PageGrn[4] = true;
                }
            }

            for (int i = 1; i <= 5; i = i + 1)
            {
                if (PageGrn[i - 1] == true && GroupsPage != i)
                {
                    GUI.DrawTexture(new Rect(96 + (i * 25), 4, 23, 18), BtnTexGrn);
                }
            }
            GUI.DrawTexture(new Rect(96 + (GroupsPage * 25), 4, 23, 18), BtnTexRed);
           // bool[5] PageBtnGrn = new bool[]();

            
            if (GUI.Button(new Rect(120, 3, 25, 20), "1"))
            {
                GroupsPage = 1;
            }
            if (GUI.Button(new Rect(145, 3, 25, 20), "2"))
            {
                GroupsPage = 2;
            }
            if (GUI.Button(new Rect(170, 3, 25, 20), "3"))
            {
                GroupsPage = 3;
            }
            if (GUI.Button(new Rect(195, 3, 25, 20), "4"))
            {
                GroupsPage = 4;
            }
            if (GUI.Button(new Rect(220, 3, 25, 20), "5"))
            {
                GroupsPage = 5;
            }
            ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollPosSelParts, new Rect(0, 0, 240, 500));

            int ButtonID = new int();
            ButtonID = 1 + (50 * (GroupsPage - 1));
            int ButtonPos = new int();
            ButtonPos = 1;
            TextAnchor TxtAnch3 = new TextAnchor();
            TxtAnch3 = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            while (ButtonPos <= 25)
            {
                if (ShowKeySetWin)
                {
                    if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + " Key: " + AGXguiKeys[ButtonID].ToString()))
                    {
                        
                        AGXCurActGroup = ButtonID;
                        ShowKeyCodeWin = true;
                    }
                }

                else
                {
                    if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID))
                    {
                        GUI.DrawTexture(new Rect(1, ((ButtonPos - 1) * 20) + 1, 118, 18), BtnTexGrn);
                    }


                    if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID]))
                    {
                        AGXCurActGroup = ButtonID;
                        TempShowGroupsWin = false;
                    }
                }
                ButtonPos = ButtonPos + 1;
                ButtonID = ButtonID + 1;
            }
            while (ButtonPos <= 50)
            {
                if (ShowKeySetWin)
                {
                    if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + " Key: " + AGXguiKeys[ButtonID].ToString()))
                    {
                        AGXCurActGroup = ButtonID;
                        ShowKeyCodeWin = true;
                    }
                }
                else
                {
                    if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID))
                    {
                        GUI.DrawTexture(new Rect(121, ((ButtonPos - 26) * 20) + 1, 118, 18), BtnTexGrn);
                    }
                    if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID]))
                    {


                        AGXCurActGroup = ButtonID;
                        TempShowGroupsWin = false;

                    }
                }
                ButtonPos = ButtonPos + 1;
                ButtonID = ButtonID + 1;
            }
            GUI.skin.button.alignment = TxtAnch3;

            GUI.EndScrollView();


            GUI.DragWindow();
        }


        public List<AGXPart> AGXAddSelectedPart(Part p, bool Sym)
        {
            List<Part> ToAdd = new List<Part>();
            List<AGXPart> RetLst = new List<AGXPart>();
            ToAdd.Add(p);
            if (Sym)
            {
                ToAdd.AddRange(p.symmetryCounterparts);
            }
            foreach (Part prt in ToAdd)
            {
                if (!AGEditorSelectedParts.Any(prt2 => prt2.AGPart == prt))
                {
                    RetLst.Add(new AGXPart(prt));
                }
            }
            AGEEditorSelectedPartsSame = true;
            foreach (AGXPart aprt in RetLst)
            {
                if (aprt.AGPart.partInfo.title != RetLst.First().AGPart.partInfo.title)
                {
                    AGEEditorSelectedPartsSame = false;
                }
            }
            if (AGEEditorSelectedPartsSame)
            {
                PartActionsList.Clear();
                PartActionsList.AddRange(p.Actions);
                foreach (PartModule pm in p.Modules)
                {
                    PartActionsList.AddRange(pm.Actions);
                }

            }
            return RetLst;

        }

        public void LoadGroupNames()
        {
            
            for (int i = 1; i <= 250; i = i + 1)
            {
                AGXguiNames[i] = "";
            }
          

            string LoadNames = "";
          
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                
                foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
                {
                  

                    LoadNames = (string)pm.Fields.GetValue("AGXNames");
                   

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

                          
                            AGXguiNames[groupNum] = groupName;

                        }
                    }
                }
            }
          

        }


        public void LoadDefaultActionGroups()
        {
           
            List<KSPActionGroup> CustomActions = new List<KSPActionGroup>();
            CustomActions.Add(KSPActionGroup.Custom01); //how do you add a range from enum?
            CustomActions.Add(KSPActionGroup.Custom02);
            CustomActions.Add(KSPActionGroup.Custom03);
            CustomActions.Add(KSPActionGroup.Custom04);
            CustomActions.Add(KSPActionGroup.Custom05);
            CustomActions.Add(KSPActionGroup.Custom06);
            CustomActions.Add(KSPActionGroup.Custom07);
            CustomActions.Add(KSPActionGroup.Custom08);
            CustomActions.Add(KSPActionGroup.Custom09);
            CustomActions.Add(KSPActionGroup.Custom10);

            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                string AddGroup = "";
                foreach (PartModule pm in p.Modules)
                {
                    foreach (BaseAction ba in pm.Actions)
                    {
                        foreach (KSPActionGroup agrp in CustomActions)
                        {

                            if ((ba.actionGroup & agrp) == agrp)
                            {
                                AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) + 1).ToString("000") + ba.guiName;
                            }
                        }
                    }
                }
                foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
                {
                    pm.Fields.SetValue("AGXData", AddGroup);
                  


                }

            }

        }
        public Part SelectPartUnderMouse()
        {
            FlightCamera CamTest = new FlightCamera();
            CamTest = FlightCamera.fetch;
            Ray ray = CamTest.mainCamera.ScreenPointToRay(Input.mousePosition);
            LayerMask RayMask = new LayerMask();
            RayMask = 1 << 0;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,Mathf.Infinity,RayMask))
            {
               
               return FlightGlobals.ActiveVessel.Parts.Find(p => p.gameObject == hit.transform.gameObject);
            }
            return null;
        }

        public void AddSelectedPart(Part p)
        {
            if (!AGEditorSelectedParts.Any(prt => prt.AGPart == p))
            {
                if (AGEditorSelectedParts.Count == 0)
                {
                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(p, SelPartsIncSym));
                }
                else if (AGEditorSelectedParts.First().AGPart.name == p.name)
                {
                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(p, SelPartsIncSym));
                }
                else
                {
                    AGEditorSelectedParts.Clear();
                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(p, SelPartsIncSym));
                }
            }
        }


        public void Update()
        {

           
            //    ActionsCheck:
            //if (CurrentVesselActions.Count > 0)
            //{
            //    foreach (AGXAction AGXact in CurrentVesselActions)
            //    {
            //        if (!FlightGlobals.ActiveVessel.Parts.Contains(AGXact.prt))
            //        {
                        
            //            CurrentVesselActions.RemoveAll(p => p.prt == AGXact.prt);
            //            goto ActionsCheck;
            //        }
            //    }

            //}
            if (CurrentVesselActions == null)
            {

                CurrentVesselActions = new List<AGXAction>();
            }
            else
            {

                CurrentVesselActions.Clear();
            }

            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {

                foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
                {

                    CurrentVesselActions.AddRange(agpm.partAGActions);
                }
            }

            foreach (KeyCode KC in ActiveKeys)
            {
                if(Input.GetKeyDown(KC))
                {
                for (int i = 1; i <= 250; i = i + 1)
                {
                    if (AGXguiKeys[i] == KC)
                    {
                        ActivateActionGroup(i);
                    }
                }
                }
            }
            if(Input.GetKeyDown(KeyCode.Mouse0) && ShowSelectedWin)
        {
            
                Part selPart = new Part();
            selPart = SelectPartUnderMouse();
            if(selPart != null)
            {
                AddSelectedPart(selPart);
        }
            
        }


            bool RootPartExists = new bool();
            try
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                }
                RootPartExists = true;
            }
            catch
            {
                RootPartExists = false;
            }

           
            if (RootPartExists)
            {

                if (AGXRoot != FlightGlobals.ActiveVessel.rootPart) //load keyset also
                {
                    print("Root part changed, AGX reloading");
                    foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
                    {
                        CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));

                    }
                    if (CurrentKeySet == 0)
                    {
                        LoadDefaultActionGroups();
                    }

                    LoadCurrentKeySet();

                    LoadActionGroups();

                    LoadGroupNames();

                    LoadCurrentKeyBindings();
                    //LoadActivatedGroups();




                    AGXRoot = FlightGlobals.ActiveVessel.rootPart;

                }

               

               // return;




                //ShowSelectedWin = false; //temp bypass

                //if (ShowSelectedWin)
                //{



                //    if (FlightGlobals.ActiveVessel.parts != null) //on first run, list is null
                //    {


                //        if (FlightGlobals.ActiveVessel.parts.Count > 0) //are there parts selected?
                //        {


                //            if (PreviousSelectedPart != FlightGlobals.ActiveVessel.rootPart) //has selected part changed?
                //            {

                //                if (!AGEditorSelectedParts.Any(p => p.AGPart == EditorActionGroups.Instance.GetSelectedParts().First())) //make sure selected part is not already in AGEdSelParts
                //                {

                //                    if (AGEditorSelectedParts.Count == 0) //no items in Selected Parts list, so just add selection
                //                    {
                //                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));


                //                    }
                //                    else if (AGEditorSelectedParts.First().AGPart.name == EditorActionGroups.Instance.GetSelectedParts().First().name) //selected part matches first part already in selected parts list, so just add selected part
                //                    {
                //                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));


                //                    }
                //                    else //part does not match first part in list, clear list before adding part
                //                    {

                //                        AGEditorSelectedParts.Clear();
                //                        AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));



                //                    }
                //                }
                //                PreviousSelectedPart = EditorActionGroups.Instance.GetSelectedParts().First(); //remember selected part so logic does not run unitl another part selected
                //            }

                //        }
                //    }

                //}





                //try
                //{
                //    if (FlightGlobals.ActiveVessel.Parts.Count >= 1)
                //    {
                     
                //            ShipListOk = true;
                //    }
                  
                //}
                //catch
                //{
              
                //    ShipListOk = false;
                //}

           





                //if (ShipListOk)
                //{
                   
                  
                 
                //}
              
            }
        }
        //public static void ClearAction(BaseAction act)
        //{

          
        //    for (int i = 1; i <= 10; i = i + 1)
        //    {

        //        act.actionGroup = act.actionGroup &= KSPActs[i];
                
        //    }
    
        //}
        //public static void SaveActionGroups(Part p)
        //{


     

        //    foreach (BaseAction clrAct in p.Actions)
        //    {

        //        ClearAction(clrAct);

        //    }
            
        //    foreach (PartModule pm in p.Modules)
        //    {
        //        foreach (BaseAction clrActPM in pm.Actions)
        //        {
        //            ClearAction(clrActPM);
        //        }
        //    }
           
           
        //    List<AGXAction> ThisPartAgxActs = new List<AGXAction>();
        //    ThisPartAgxActs.AddRange(CurrentVesselActions.Where(agprt => agprt.prt == p));
          
        //    string SaveGroupsString = "";
         
        //    foreach (AGXAction agAct in ThisPartAgxActs)
        //    {
        //        SaveGroupsString = SaveGroupsString + '\u2023' + agAct.group.ToString("000") + agAct.ba.guiName;
              
        //        if (agAct.group <= 10)
             
        //        {

        //            agAct.ba.actionGroup = (agAct.ba.actionGroup |= KSPActs[agAct.group]);
            

        //        }
               
        //    }


        //    foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
        //    {
        //        ModuleAGExtData AGpm = new ModuleAGExtData();
        //        AGpm = (ModuleAGExtData)pm;
        //        AGpm.AGXData = SaveGroupsString;

        //    }
     
            
        //}
        
        public void LoadActionGroups()
        {
            CurrentVesselActions.Clear();
           
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
              
                List<BaseAction> pActions = new List<BaseAction>();
                string LoadList = "";

             
                foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
                {

                    LoadList = (string)pm.Fields.GetValue("AGXData");
                    ModuleAGExtData AGpm = new ModuleAGExtData();
                    AGpm = (ModuleAGExtData)pm;
                    pActions.AddRange((List<BaseAction>)AGpm.partAllActions);
                    //pActions.AddRange((List<BaseAction>)pm.Fields.GetValue("ThisPartActions"));
                   
                }
               
                if (LoadList.Length > 0)
                {
                    while (LoadList[0] == '\u2023')
                    {
                 

                        LoadList = LoadList.Substring(1);

                        int KeyLength = new int();

                        int ActGroup = new int();

                        KeyLength = LoadList.IndexOf('\u2023');
                       
                        if (KeyLength == -1)
                        {
                          
                            ActGroup = Convert.ToInt32(LoadList.Substring(0, 3));
                            LoadList = LoadList.Substring(3);
                            CurrentVesselActions.Add(new AGXAction() { group = ActGroup, prt = p, ba = pActions.Find(b => b.guiName == LoadList) });
                            
                        }
                        else
                        {
                           
                            ActGroup = Convert.ToInt32(LoadList.Substring(0, 3));
                            LoadList = LoadList.Substring(3);
                            CurrentVesselActions.Add(new AGXAction() { group = ActGroup, prt = p, ba = pActions.Find(b => b.guiName == LoadList.Substring(0, KeyLength - 3)) });
                            LoadList = LoadList.Substring(KeyLength - 3);
                           
                        }
                    }
                }
            }
     
        }



        public static string SaveGroupNames(Part p, String str)
        {
            string SaveStringNames = "";
            if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
            {

                int GroupCnt = new int();
                GroupCnt = 1;
                while (GroupCnt <= 250)
                {
                    if (AGXguiNames[GroupCnt].Length >= 1)
                    {
                        SaveStringNames = SaveStringNames + '\u2023' + GroupCnt.ToString("000") + AGXguiNames[GroupCnt];
                    }
                    GroupCnt = GroupCnt + 1;
                }
            }
            else
            {
                SaveStringNames = str;
            }
            return SaveStringNames;
        }

        public void UpdateActionsListCheck()
        {
            List<AGXAction> KnownGood = new List<AGXAction>();
            KnownGood = new List<AGXAction>();

            foreach (AGXAction agxAct in CurrentVesselActions)
            {
                if (FlightGlobals.ActiveVessel.Parts.Contains(agxAct.prt))
                {
                    KnownGood.Add(agxAct);
                }
            }
            CurrentVesselActions = KnownGood;

            ActionsListDirty = false;
        }

        public void AGXResetPartsList()
        {
            AGEEditorSelectedPartsSame = true;
            AGEditorSelectedParts.Clear();
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                AGEditorSelectedParts.Add(new AGXPart(p));
            }


            AGXPart AGPcompare = new AGXPart();
            AGPcompare = AGEditorSelectedParts.First();

            foreach (AGXPart p in AGEditorSelectedParts)
            {
                if (p.AGPart.ConstructID != AGPcompare.AGPart.ConstructID)
                {
                    AGEEditorSelectedPartsSame = false;
                }
            }
        }
    }
}