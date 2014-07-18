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
        
        //private Dictionary<int, KeyCode> Save10Keys =  new Dictionary<int, KeyCode>();

        public static Dictionary<int, bool> IsGroupToggle; //is group a toggle group?
        public static bool[,] ShowGroupInFlight; //Show group in flight?
        public static int ShowGroupInFlightCurrent;
        public static string[] ShowGroupInFlightNames;
        public bool ShowGroupsInFlightWindow = false;
        public Rect GroupsInFlightWin;
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
        //private Part PreviousSelectedPart = null;
        private bool SelPartsIncSym = true;
        private string BtnTxt;
        //private bool AGXDoLock = false;
        private Rect TestWin;
        private Part AGXRoot;

        //private bool NeedToSave = false;
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
        //private bool Trigger2;
        //private int Value = 0;
        //private string HexStr;

        private List<BaseAction> PartActionsList;



        //private bool ShipListOk = false;
        Texture2D BtnTexRed = new Texture2D(1, 1);
        Texture2D BtnTexGrn = new Texture2D(1, 1);
        public static Dictionary<int, string> AGXguiNames;
        public static Dictionary<int, KeyCode> AGXguiKeys;
        public int AGXCurActGroup = 1;
        List<string> KeyCodeNames = new List<string>();
        List<string> JoyStickCodes = new List<string>();
        //private bool ActionsListDirty = true; //is our actions requiring update?
       // private bool LoadGroupsOnceCheck = false;
        private bool ShowCurActsWin = true;
        private bool ShowAGXMod = true;
        private bool ShowAGXFlightWin = true;
        //private string ActivatedGroups = "";
        public static bool loadFinished = false;
        //private bool AGXShow = true;
        private int RightClickDelay = 0;
        private bool RightLickPartAdded = false;
       // private List<int> ActiveActions = new List<int>();
        public static List<AGXActionsState> ActiveActionsState = new List<AGXActionsState>();
        public static List<AGXActionsState> ActiveActionsStateToShow = new List<AGXActionsState>();
       // public bool ActiveActionsCalculated = false;
        private int actionsCheckFrameCount = 0;






       

        public void Start()
        {
            //Save10Keys = new Dictionary<int, KeyCode>();
            //foreach (Part p in 
            ActiveKeys = new List<KeyCode>();

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
            KeySetWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltKeySetX")), Convert.ToInt32(AGExtNode.GetValue("FltKeySetY")), 185, 335);
            CurActsWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltCurActsX")), Convert.ToInt32(AGExtNode.GetValue("FltCurActsY")), 345, 140);
            FlightWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltMainX")), Convert.ToInt32(AGExtNode.GetValue("FltMainY")), 215, 100);
            GroupsInFlightWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltMainX")), Convert.ToInt32(AGExtNode.GetValue("FltMainY")), 80, 110);
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

            IsGroupToggle = new Dictionary<int, bool>();
            ShowGroupInFlight = new bool[6,251];
            ShowGroupInFlightNames = new string[6];
            ShowGroupInFlightNames[1] = "Group 1";
            ShowGroupInFlightNames[2] = "Group 2";
            ShowGroupInFlightNames[3] = "Group 3";
            ShowGroupInFlightNames[4] = "Group 4";
            ShowGroupInFlightNames[5] = "Group 5";
            foreach (ModuleAGExtData agData in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
            {
                ShowGroupInFlightNames = agData.LoadShowGroupNames();
            }
            for (int i = 1; i <= 250; i++)
            {
                IsGroupToggle[i] = false;
                for (int i2 = 1; i2 <= 5;i2++)
                {
                    ShowGroupInFlight[i2, i] = true;
                }
            }
            ShowGroupInFlightCurrent = 1;
           // print("AGXStart " + Planetarium.GetUniversalTime());
            
            
           
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
            LoadActionGroups();
            LoadGroupVisibility();
            foreach (ModuleAGExtData agData in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
            {
                ShowGroupInFlightNames = agData.LoadShowGroupNames();
            }
            loadFinished = true;
        }

      


        private void UnbindDefaultKeys()
        {
           //Unbind default keys from Custom 1 through 10. Note this does not save across sessions and so gets reapplied.
           // Save10Keys[1] = GameSettings.CustomActionGroup1.primary;
            GameSettings.CustomActionGroup1.primary = KeyCode.None;
            //GameSettings.CustomActionGroup1.secondary = KeyCode.None;
           // Save10Keys[2] = GameSettings.CustomActionGroup2.primary;
            GameSettings.CustomActionGroup2.primary = KeyCode.None;
           // GameSettings.CustomActionGroup2.secondary = KeyCode.None;
           // Save10Keys[3] = GameSettings.CustomActionGroup3.primary;
            GameSettings.CustomActionGroup3.primary = KeyCode.None;
           // GameSettings.CustomActionGroup3.secondary = KeyCode.None;
           // Save10Keys[4] = GameSettings.CustomActionGroup4.primary;
            GameSettings.CustomActionGroup4.primary = KeyCode.None;
            //GameSettings.CustomActionGroup4.secondary = KeyCode.None;
            //Save10Keys[5] = GameSettings.CustomActionGroup5.primary;
            GameSettings.CustomActionGroup5.primary = KeyCode.None;
            //GameSettings.CustomActionGroup5.secondary = KeyCode.None;
           // Save10Keys[6] = GameSettings.CustomActionGroup6.primary;
            GameSettings.CustomActionGroup6.primary = KeyCode.None;
            //GameSettings.CustomActionGroup6.secondary = KeyCode.None;
           // Save10Keys[7] = GameSettings.CustomActionGroup7.primary;
            GameSettings.CustomActionGroup7.primary = KeyCode.None;
           // GameSettings.CustomActionGroup7.secondary = KeyCode.None;
           // Save10Keys[8] = GameSettings.CustomActionGroup8.primary;
            GameSettings.CustomActionGroup8.primary = KeyCode.None;
           // GameSettings.CustomActionGroup8.secondary = KeyCode.None;
           // Save10Keys[9] = GameSettings.CustomActionGroup9.primary;
            GameSettings.CustomActionGroup9.primary = KeyCode.None;
           // GameSettings.CustomActionGroup9.secondary = KeyCode.None;
           // Save10Keys[10] = GameSettings.CustomActionGroup10.primary;
            GameSettings.CustomActionGroup10.primary = KeyCode.None;
           // GameSettings.CustomActionGroup10.secondary = KeyCode.None;
        }

        //private void RebindDefaultKeys()
        //{
        //    GameSettings.CustomActionGroup1.primary = Save10Keys[1];
        //}

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

        public static string SaveGroupVisibilityNames(Part p, string str)
        {
            try
            {
                if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                {
                    string StringToSave = ShowGroupInFlightNames[1];
                    for (int i = 2; i <= 5; i++)
                    {
                        StringToSave = StringToSave + '\u2023' + ShowGroupInFlightNames[i];
                    }
                    return StringToSave;
                }
                else
                {
                    return str;
                }
            }

            catch
            {
                return str;
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
        SaveCurrentVesselActions();
        foreach (Part p in FlightGlobals.ActiveVessel.parts)
        {
            foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
            {
                ModuleAGExtData AGData = (ModuleAGExtData)pm;
                AGData.AGXNames = SaveGroupNames(p, AGData.AGXNames);
                AGData.AGXGroupStates = SaveGroupVisibility(p, AGData.AGXGroupStates);
                AGData.AGXGroupStateNames = SaveGroupVisibilityNames(p, AGData.AGXGroupStateNames);
            }
        }

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
                    GroupsInFlightWin.x = FlightWin.x + 215;
                    GroupsInFlightWin.y = FlightWin.y;
                    FlightWin = GUI.Window(673467788, FlightWin, FlightWindow, "Actions", AGXWinStyle);
                    //TrapMouse |= FlightWin.Contains(RealMousePos);
                   
                }
                if (ShowGroupsInFlightWindow)
                {
                    GroupsInFlightWin = GUI.Window(673461788, GroupsInFlightWin, GroupsInFlightWindow, "", AGXWinStyle);

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
                    
                    agAct.ba.Invoke(actParam);
                    foreach (AGXAction agxAct in CurrentVesselActions)
                    {
                        if(agxAct.ba == agAct.ba)
                        {
                            agxAct.activated = false;
                        }
                    }
                  
                }
                else
                {
                    KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate);
                    //agAct.activated = true;
                    agAct.ba.Invoke(actParam);
                    foreach (AGXAction agxAct in CurrentVesselActions)
                    {
                        if (agxAct.ba == agAct.ba)
                        {
                            agxAct.activated = true;
                        }
                    }
                  
                    
                }
                ModuleAGExtData pmAgx = agAct.ba.listParent.part.Modules.OfType<ModuleAGExtData>().First<ModuleAGExtData>();
                pmAgx.partAGActions.Clear();
                pmAgx.partAGActions.AddRange(CurrentVesselActions.Where(agp => agp.prt == agAct.ba.listParent.part));
                pmAgx.AGXData = pmAgx.SaveActionGroups();
            }
            CalculateActionsState();
        }

        public void GroupsInFlightWindow(int WindowID)
        {
            if (GUI.Button(new Rect(5, 5, 70, 20), ShowGroupInFlightNames[1]))
            {
                ShowGroupInFlightCurrent = 1;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 25, 70, 20), ShowGroupInFlightNames[2]))
            {
                ShowGroupInFlightCurrent = 2;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 45, 70, 20), ShowGroupInFlightNames[3]))
            {
                ShowGroupInFlightCurrent = 3;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 65, 70, 20), ShowGroupInFlightNames[4]))
            {
                ShowGroupInFlightCurrent = 4;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 85, 70, 20), ShowGroupInFlightNames[5]))
            {
                ShowGroupInFlightCurrent = 5;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
        }

        public void FlightWindow(int WindowID)
        {

            if (GUI.Button(new Rect(140, 5, 70, 20), ShowGroupInFlightNames[ShowGroupInFlightCurrent]))
            {
                
                ShowGroupsInFlightWindow = !ShowGroupsInFlightWindow;
            }
            if (GUI.Button(new Rect(5, 5, 75, 20), "Edit"))
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

                foreach (AGXAction agAct in CurrentVesselActions)
                {
                    //print("AGX " + Planetarium.GetUniversalTime() + " " + agAct.prt.ConstructID + " " + agAct.ba.name + " " + agAct.ba.guiName + " " + agAct.activated);
                }

                foreach (AGXActionsState agState in ActiveActionsStateToShow)
                {
                    //print("AGX " + agState.group + " " + agState.actionOff + " " + agState.actionOn);
                }
                string ToggleState = "";
                for (int i = 1; i <= 250; i++)
                {
                    if (IsGroupToggle[i])
                    {
                        ToggleState = ToggleState + "1";
                    }
                    else
                    {
                        ToggleState = ToggleState + "0";

                    }
                }
                //print("AGX " + ToggleState);

            }
           
            
            //for (int i = 1; i <= 250; i = i + 1)
            //{
            //    if(CurrentVesselActions.Any(a => a.group == i))
            //    {
                 
            //        ActiveActions.Add(i);
                   

                   
            //    }
            //    if (ActiveActions.Count > 0)
            //    {
            //        ActiveKeys.Clear();
            //        foreach (int i2 in ActiveActions)
            //        {
            //            ActiveKeys.Add(AGXguiKeys[i2]);
            //        }
            //    }
            //}

            //GUI.Box(new Rect(5, 25, 190, Math.Min(410,10+(20*Math.Max(1,ActiveActions.Count)))), "");
            GUI.Box(new Rect(5, 25, 190, Math.Min(410, 10 + (20 * Math.Max(1, ActiveActionsStateToShow.Count)))), "");
            //FlightWinScroll = GUI.BeginScrollView(new Rect(10, 30, 200, Math.Min(400,20+(20*(ActiveActions.Count-1)))), FlightWinScroll, new Rect(0, 0, 180, (20 * (ActiveActions.Count))));
            FlightWinScroll = GUI.BeginScrollView(new Rect(10, 30, 200, Math.Min(400, 20 + (20 * (ActiveActionsStateToShow.Count - 1)))), FlightWinScroll, new Rect(0, 0, 180, (20 * (ActiveActionsStateToShow.Count))));
            //FlightWin.height = Math.Min( Math.Max(60,40+(20*ActiveActions.Count)),440);
            Rect FlightWinOld = new Rect(FlightWin.x, FlightWin.y, FlightWin.width, FlightWin.height);
            FlightWin.height = Math.Min(Math.Max(60, 40 + (20 * ActiveActionsStateToShow.Count)), 440);
            if (FlightWin.y + FlightWin.height > Screen.height)
            {
                FlightWin.y = Screen.height - FlightWin.height;
            }
            if (FlightWin.y < 1)
            {
                FlightWin.y = 1;
            }
            if (FlightWinOld.height != FlightWin.height & FlightWinOld.y + FlightWinOld.height > Screen.height -20)
            {
               FlightWin.y = (FlightWinOld.height + FlightWinOld.y) - FlightWin.height;
            }


            //if (ActiveActions.Count > 0)
            if (ActiveActionsStateToShow.Count > 0)
            {
                
                //for (int i2 = 1; i2 <= ActiveActions.Count; i2 = i2 + 1)
                for (int i2 = 1; i2 <= ActiveActionsStateToShow.Count; i2 = i2 + 1)
                {
                    Color TxtClr = new Color();
                    TxtClr = GUI.contentColor;
                    TextAnchor TxtAnch4 = new TextAnchor();
                    TxtAnch4 = GUI.skin.button.alignment;
                    if (IsGroupToggle[ActiveActionsStateToShow.ElementAt((i2 - 1)).group])
                    {
                        

                        if (ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOn == true && ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOff == false)
                        {
                            GUI.contentColor = Color.green;
                        }
                        else if (ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOn == true && ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOff == true)
                        {
                            GUI.contentColor = Color.yellow;
                        }
                        else if (ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOn == false && ActiveActionsStateToShow.ElementAt((i2 - 1)).actionOff == true)
                        {
                            GUI.contentColor = Color.red;
                        }
                        else
                        {
                            GUI.contentColor = TxtClr;
                        }
                    }

                    GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    //if (GUI.Button(new Rect(0, 0 + (20 * (i2 - 1)), 110, 20), ActiveActions.ElementAt((i2 - 1)) + ": " + AGXguiNames[ActiveActions.ElementAt((i2 - 1))]))
                        if (GUI.Button(new Rect(0, 0 + (20 * (i2 - 1)), 110, 20), ActiveActionsStateToShow.ElementAt((i2 - 1)).group + ": " + AGXguiNames[ActiveActionsStateToShow.ElementAt((i2 - 1)).group]))
                    {
                        
                        //ActivateActionGroup(ActiveActions.ElementAt(i2 - 1));
                        ActivateActionGroup(ActiveActionsStateToShow.ElementAt(i2 - 1).group);
                    }
                    GUI.skin.button.alignment = TxtAnch4;
                    //if (GUI.Button(new Rect(110, 0 + (20 * (i2 - 1)), 70, 20), AGXguiKeys[ActiveActions.ElementAt((i2 - 1))].ToString()))
                        if (GUI.Button(new Rect(110, 0 + (20 * (i2 - 1)), 70, 20), AGXguiKeys[ActiveActionsStateToShow.ElementAt((i2 - 1)).group].ToString()))
                    {

                        //ActivateActionGroup(ActiveActions.ElementAt(i2 - 1));
                        ActivateActionGroup(ActiveActionsStateToShow.ElementAt(i2 - 1).group);
                    }
                        GUI.contentColor = TxtClr;
                }
                
            }
                GUI.EndScrollView();
            //if(ActiveActions.Count ==0)
                if (ActiveActionsState.Count == 0)
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
                    agpm.AGXData = agpm.SaveActionGroups();
                }
            }
            CalculateActiveActions();
        }
        public void SetDefaultAction(BaseAction ba, int group)
        {
            Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
            KSPActs[1] = KSPActionGroup.Custom01; //setup list to delete action from 
            KSPActs[2] = KSPActionGroup.Custom02;
            KSPActs[3] = KSPActionGroup.Custom03;
            KSPActs[4] = KSPActionGroup.Custom04;
            KSPActs[5] = KSPActionGroup.Custom05;
            KSPActs[6] = KSPActionGroup.Custom06;
            KSPActs[7] = KSPActionGroup.Custom07;
            KSPActs[8] = KSPActionGroup.Custom08;
            KSPActs[9] = KSPActionGroup.Custom09;
            KSPActs[10] = KSPActionGroup.Custom10;
            ba.actionGroup = ba.actionGroup | KSPActs[group];
        }

        public void RemoveDefaultAction(BaseAction ba, int group)
        {
            Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
            KSPActs[1] = KSPActionGroup.Custom01; //setup list to delete action from 
            KSPActs[2] = KSPActionGroup.Custom02;
            KSPActs[3] = KSPActionGroup.Custom03;
            KSPActs[4] = KSPActionGroup.Custom04;
            KSPActs[5] = KSPActionGroup.Custom05;
            KSPActs[6] = KSPActionGroup.Custom06;
            KSPActs[7] = KSPActionGroup.Custom07;
            KSPActs[8] = KSPActionGroup.Custom08;
            KSPActs[9] = KSPActionGroup.Custom09;
            KSPActs[10] = KSPActionGroup.Custom10;

            ba.actionGroup = ba.actionGroup & ~KSPActs[group];
            
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
                        int ToDel = 0;
                        foreach (AGXAction AGXToRemove in CurrentVesselActions)
                        {

                            if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                            {
                                
                                CurrentVesselActions.RemoveAt(ToDel);
                                goto BreakOutA;
                            }
                            ToDel = ToDel + 1;
                        }
                    BreakOutA:
                        SaveCurrentVesselActions();
                    if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                    {
                        RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                    }
                    }

                    if (GUI.Button(new Rect(100, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).prt.partInfo.title))
                    {
                        int ToDel = 0;
                        foreach (AGXAction AGXToRemove in CurrentVesselActions)
                        {

                            if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                            {

                                CurrentVesselActions.RemoveAt(ToDel);
                                goto BreakOutB;
                            }
                            ToDel = ToDel + 1;
                        }
                    BreakOutB:
                        SaveCurrentVesselActions();
                    if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                    {
                        RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                    }
                    }
                    try
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).ba.guiName))
                        {
                            int ToDel = 0;
                            foreach (AGXAction AGXToRemove in CurrentVesselActions)
                            {

                                if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                {

                                    CurrentVesselActions.RemoveAt(ToDel);
                                    goto BreakOutC;
                                }
                                ToDel = ToDel + 1;
                            }
                        BreakOutC:
                            SaveCurrentVesselActions();
                        if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                        {
                            RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                        }
                        }
                    }
                    catch
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), "Error"))
                        {
                            int ToDel = 0;
                            foreach (AGXAction AGXToRemove in CurrentVesselActions)
                            {

                                if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                {

                                    CurrentVesselActions.RemoveAt(ToDel);
                                    goto BreakOutD;
                                }
                                ToDel = ToDel + 1;
                            }
                        BreakOutD:
                            SaveCurrentVesselActions();
                        if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                        {
                            RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                        }
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

            Color TxtClr3 = GUI.contentColor;
            GUI.contentColor = new Color(0.5f, 1f, 0f,1f);
                GUI.skin.label.fontStyle = FontStyle.Bold;
                TextAnchor TxtAnc = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(5, 145, 175, 25), "Actiongroup Groups");
                GUI.skin.label.fontStyle = FontStyle.Normal;
                GUI.skin.label.alignment = TxtAnc;
                GUI.contentColor = TxtClr3;

            GUI.DrawTexture(new Rect(6, (ShowGroupInFlightCurrent * 25) + 141, 68, 18), BtnTexGrn);
            if (GUI.Button(new Rect(5, 165, 70, 20), "Group 1:"))
                {
                     ShowGroupInFlightCurrent = 1;
                }
                ShowGroupInFlightNames[1] = GUI.TextField(new Rect(80, 165, 100, 20), ShowGroupInFlightNames[1]);

                if (GUI.Button(new Rect(5, 190, 70, 20), "Group 2:"))
                {
                    ShowGroupInFlightCurrent = 2;
                }
                ShowGroupInFlightNames[2] = GUI.TextField(new Rect(80, 190, 100, 20), ShowGroupInFlightNames[2]);

                if (GUI.Button(new Rect(5, 215, 70, 20), "Group 3:"))
                {
                    ShowGroupInFlightCurrent = 3;
                }
                ShowGroupInFlightNames[3] = GUI.TextField(new Rect(80, 215, 100, 20), ShowGroupInFlightNames[3]);

                if (GUI.Button(new Rect(5, 240, 70, 20), "Group 4:"))
                {
                    ShowGroupInFlightCurrent = 4;
                }
                ShowGroupInFlightNames[4] = GUI.TextField(new Rect(80, 240, 100, 20), ShowGroupInFlightNames[4]);

                if (GUI.Button(new Rect(5, 265, 70, 20), "Group 5:"))
                {
                    ShowGroupInFlightCurrent = 5;
                }
                ShowGroupInFlightNames[5] = GUI.TextField(new Rect(80, 265, 100, 20), ShowGroupInFlightNames[5]);


            if (GUI.Button(new Rect(5, 300, 175, 30), "Close Window"))
            {

                AGExtNode.SetValue("KeySetName1", KeySetNames[0]);
                AGExtNode.SetValue("KeySetName2", KeySetNames[1]);
                AGExtNode.SetValue("KeySetName3", KeySetNames[2]);
                AGExtNode.SetValue("KeySetName4", KeySetNames[3]);
                AGExtNode.SetValue("KeySetName5", KeySetNames[4]);
                CurrentKeySetName = KeySetNames[CurrentKeySet - 1];
                AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                foreach (ModuleAGExtData pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
                     {
                         pm.AGXGroupStates = SaveGroupVisibility(FlightGlobals.ActiveVessel.rootPart, pm.AGXGroupStates);
                         pm.AGXGroupStateNames = SaveGroupVisibilityNames(FlightGlobals.ActiveVessel.rootPart, pm.AGXGroupStates);
                     }
                ShowKeySetWin = false;
                ShowSelectedWin = true;
            }

            GUI.DragWindow();
        }

        public static string SaveCurrentKeySet(Part p, string curKeySet)
        {

            try
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
            catch
            {
                print("AGX Save KeySet FAIL! (SaveCurrentKeySet)");
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
            if (CurrentKeySet == 1)
            {
                SaveDefaultCustomKeys();
            }
            
        }

        public void SaveDefaultCustomKeys()
        {
            GameSettings.CustomActionGroup1.primary = AGXguiKeys[1]; //copy keys to KSP itself
            GameSettings.CustomActionGroup2.primary = AGXguiKeys[2];
            GameSettings.CustomActionGroup3.primary = AGXguiKeys[3];
            GameSettings.CustomActionGroup4.primary = AGXguiKeys[4];
            GameSettings.CustomActionGroup5.primary = AGXguiKeys[5];
            GameSettings.CustomActionGroup6.primary = AGXguiKeys[6];
            GameSettings.CustomActionGroup7.primary = AGXguiKeys[7];
            GameSettings.CustomActionGroup8.primary = AGXguiKeys[8];
            GameSettings.CustomActionGroup9.primary = AGXguiKeys[9];
            GameSettings.CustomActionGroup10.primary = AGXguiKeys[10];
            GameSettings.SaveSettings(); //save keys to disk
            GameSettings.CustomActionGroup1.primary = KeyCode.None; //unbind keys so they don't conflict
            GameSettings.CustomActionGroup2.primary = KeyCode.None;
            GameSettings.CustomActionGroup3.primary = KeyCode.None;
            GameSettings.CustomActionGroup4.primary = KeyCode.None;
            GameSettings.CustomActionGroup5.primary = KeyCode.None;
            GameSettings.CustomActionGroup6.primary = KeyCode.None;
            GameSettings.CustomActionGroup7.primary = KeyCode.None;
            GameSettings.CustomActionGroup8.primary = KeyCode.None;
            GameSettings.CustomActionGroup9.primary = KeyCode.None;
            GameSettings.CustomActionGroup10.primary = KeyCode.None;
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
            int SelPartsLeft = new int(); //move everything left or right by tweaking this variable
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
                                List<BaseAction> ThisPartActionsList = new List<BaseAction>();
                                ThisPartActionsList.AddRange(agPrt.AGPart.Actions);
                                foreach (PartModule pm3 in agPrt.AGPart.Modules)
                                {
                                    ThisPartActionsList.AddRange(pm3.Actions);
                                }
                                AGXAction ToAdd = new AGXAction();
                                if (ThisPartActionsList.ElementAt(ActionsCount - 1).guiName == PartActionsList.ElementAt(ActionsCount - 1).guiName)
                                {
                                    ToAdd = new AGXAction() { prt = agPrt.AGPart, ba = ThisPartActionsList.ElementAt(ActionsCount - 1), group = AGXCurActGroup, activated = false };
                                }
                                else
                                {
                                    ToAdd = new AGXAction() { prt = agPrt.AGPart, ba = PartActionsList.ElementAt(ActionsCount - 1), group = AGXCurActGroup, activated = false };
                                }
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(CurrentVesselActions);
                                Checking.RemoveAll(p => p.group != ToAdd.group);
                                Checking.RemoveAll(p => p.prt != ToAdd.prt);
                                Checking.RemoveAll(p => p.ba != ToAdd.ba);
                                if (Checking.Count == 0)
                                {
                                    CurrentVesselActions.Add(ToAdd);
                                    SaveCurrentVesselActions();
                                }
                                PrtCnt = PrtCnt + 1;
                                if (ToAdd.group < 11)
                                {
                                    SetDefaultAction(ToAdd.ba, ToAdd.group);
                                }
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
            if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), AGXCurActGroup + ": " + AGXguiNames[AGXCurActGroup])) //current action group button
            {
                TempShowGroupsWin = true;
            }
            GUI.skin.button.alignment = TxtAnch2;
            if (IsGroupToggle[AGXCurActGroup])
            {
                
                Color TxtClr = GUI.contentColor;
                GUI.contentColor = Color.green;
                if (GUI.Button(new Rect(SelPartsLeft + 245, 160, 110, 22), "Toggle Grp: Yes"))
                {
                  
                    IsGroupToggle[AGXCurActGroup] = false;
                }
                GUI.contentColor = TxtClr;
            }
            else
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 160, 110, 22), "Toggle Grp: No"))
                {
                   
                    IsGroupToggle[AGXCurActGroup] = true;
                }
            }
            GUI.Label(new Rect(SelPartsLeft + 231, 183, 110, 22), "Show:");
            Color TxtClr2 = GUI.contentColor;
            
            if (ShowGroupInFlight[1,AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 271, 183, 20, 22), "1"))
            {
                ShowGroupInFlight[1, AGXCurActGroup] = !ShowGroupInFlight[1, AGXCurActGroup];
                CalculateActionsToShow();
            }

            if (ShowGroupInFlight[2, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 291, 183, 20, 22), "2"))
            {
                ShowGroupInFlight[2, AGXCurActGroup] = !ShowGroupInFlight[2, AGXCurActGroup];
                CalculateActionsToShow();
            }

            if (ShowGroupInFlight[3, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 311, 183, 20, 22), "3"))
            {
                ShowGroupInFlight[3, AGXCurActGroup] = !ShowGroupInFlight[3, AGXCurActGroup];
                CalculateActionsToShow();
            }

            if (ShowGroupInFlight[4, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 331, 183, 20, 22), "4"))
            {
                ShowGroupInFlight[4, AGXCurActGroup] = !ShowGroupInFlight[4, AGXCurActGroup];
                CalculateActionsToShow();
            }

            if (ShowGroupInFlight[5, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 351, 183, 20, 22), "5"))
            {
                ShowGroupInFlight[5, AGXCurActGroup] = !ShowGroupInFlight[5, AGXCurActGroup];
                CalculateActionsToShow();
            }
            GUI.contentColor = TxtClr2;
            GUI.Label(new Rect(SelPartsLeft + 245, 115, 110, 20), "Description:");
            CurGroupDesc = AGXguiNames[AGXCurActGroup];
            CurGroupDesc = GUI.TextField(new Rect(SelPartsLeft + 245, 135, 120, 22), CurGroupDesc);
            AGXguiNames[AGXCurActGroup] = CurGroupDesc;
            GUI.Label(new Rect(SelPartsLeft + 245, 203, 110, 25), "Keybinding:");
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
        public void LoadGroupVisibility()
        {
            try
            {
                foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
                {
                    string LoadString = (string)pm.Fields.GetValue("AGXGroupStates");
                   // print("AGXTogLoad" + pm.part.ConstructID + " " + LoadString);
                    if (LoadString.Length == 1501)
                    {
                        ShowGroupInFlightCurrent = Convert.ToInt32(LoadString.Substring(0, 1));
                        LoadString = LoadString.Substring(1);

                        for (int i = 1; i <= 250; i++)
                        {
                            if (LoadString[0] == '1')
                            {
                                IsGroupToggle[i] = true;
                            }
                            else
                            {
                                IsGroupToggle[i] = false;
                            }
                            LoadString = LoadString.Substring(1);
                            //ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                            for (int i2 = 1; i2 <= 5; i2++)
                            {
                                if (LoadString[0] == '1')
                                {
                                    ShowGroupInFlight[i2, i] = true;
                                }
                                else
                                {
                                    ShowGroupInFlight[i2, i] = false;
                                }
                                LoadString = LoadString.Substring(1);
                                //ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                            }
                        }
                    }
                    else
                    {
                        ShowGroupInFlightCurrent = 1;
                        for (int i = 1; i <= 250; i++)
                        {
                                IsGroupToggle[i] = false;
                            for (int i2 = 1; i2 <= 5; i2++)
                            {
                                ShowGroupInFlight[i2, i] = true;
                            }
                        }
                    }
                }
                CalculateActionsToShow();
               // print("AGXTogLoadFin: " + IsGroupToggle[1] + IsGroupToggle[2] + IsGroupToggle[3] + IsGroupToggle[4] + IsGroupToggle[5] + IsGroupToggle[6] + IsGroupToggle[7] + IsGroupToggle[8] + IsGroupToggle[9] + IsGroupToggle[10]);
            }
            catch
            {
                print("AGX Load Actions Visibility Fail!");
            }
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
                    loadFinished = false;
                    foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
                    {
                        CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));

                    }
                    if (CurrentKeySet == 0)
                    {
                        LoadDefaultActionGroups();
                    }

                    LoadCurrentKeySet();

                    
                    
                    LoadGroupNames();

                    LoadCurrentKeyBindings();
                    LoadActionGroups();
                    //LoadActivatedGroups();
                    LoadGroupVisibility();
                    foreach (ModuleAGExtData agData in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
            {
                ShowGroupInFlightNames = agData.LoadShowGroupNames();
            }


                    AGXRoot = FlightGlobals.ActiveVessel.rootPart;
                    loadFinished = true;

                }
            }
            if (LastPartCount != FlightGlobals.ActiveVessel.parts.Count) //parts count changed, remove any actions assigned to parts that have disconnected/been destroyed
            {
                print("Part count change, reload AGX");
                LoadActionGroups();
                
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
            //if (!ActiveActionsCalculated)
            //{
            //    CalculateActiveActions();
                
            //}
            if(Input.GetKeyDown(KeyCode.Mouse0) && ShowSelectedWin)
        {
            
                Part selPart = new Part();
            selPart = SelectPartUnderMouse();
            if(selPart != null)
            {
                AddSelectedPart(selPart);
        }
            
        }

            if (RightClickDelay < 3)
            {
                if (RightClickDelay == 2)
                {
                    UIPartActionWindow UIPartsListThing = new UIPartActionWindow();
                    UIPartsListThing = (UIPartActionWindow)FindObjectOfType(typeof(UIPartActionWindow));
                    //UnityEngine.Object[] TempObj = FindObjectsOfType(typeof(UIPartActionWindow));
                    //print(TempObj.Length);
                    try
                    {
                        if (UIPartsListThing != null)
                        {
                            AddSelectedPart(UIPartsListThing.part);
                        }
                       // print(UIPartsListThing.part.name); //finds part right-clicked on
                        RightLickPartAdded = true;
                    }
                    catch
                    {
                       // print("nope!");
                        RightLickPartAdded = true;
                    }
                }
                
                    RightClickDelay = RightClickDelay + 1; 
                

            }

            
            
            if (Input.GetKeyUp(KeyCode.Mouse1) && ShowSelectedWin && RightLickPartAdded == true)
            {
                RightClickDelay = 0;
                RightLickPartAdded = false;

            }


            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    foreach (PartModule pm in p.Modules)
            //    {
            //        foreach (BaseAction ba in pm.Actions)
            //        {
            //            print(p.partName + " " + pm.moduleName + " " + ba.name + " " + ba.guiName);
            //        }
            //    }
            //}
            if (actionsCheckFrameCount >= 20)
            {
                CheckActionsActive();
                actionsCheckFrameCount = 0;
            }
            else
            {
                actionsCheckFrameCount = actionsCheckFrameCount + 1;
            }
        }

        

        public void CalculateActiveActions()
        {
            
            //ActiveActions.Clear();
            ActiveActionsState.Clear();
            for (int i = 1; i <= 250; i = i + 1)
            {
                if (CurrentVesselActions.Any(a => a.group == i))
                {

                   // ActiveActions.Add(i);
                    ActiveActionsState.Add(new AGXActionsState() { group = i, actionOff = false, actionOn = false });



                }
                ActiveKeys.Clear();
                if (ActiveActionsState.Count > 0)
                    //if (ActiveActions.Count > 0)
                {
                    foreach (AGXActionsState i2 in ActiveActionsState)
                    //foreach (int i2 in ActiveActions)
                    {
                        //ActiveKeys.Add(AGXguiKeys[i2]);
                        ActiveKeys.Add(AGXguiKeys[i2.group]);
                    }
                }


                
            }
            if (ActiveActionsState.Count > 0)
            {
                CalculateActionsState();
                CalculateActionsToShow();
            }
           // ActiveActionsCalculated = true;
        }

        
        
        public static void CalculateActionsState() //flag each actiongroup as activated or not
        {
            //print("Calculate start");
            foreach (AGXActionsState actState in ActiveActionsState)
            {
                actState.actionOn = false;
                actState.actionOff = false;
            }
            
            foreach (AGXAction agxAct in CurrentVesselActions)
            {
                
                if (agxAct.activated)
                {
                    ActiveActionsState.Find(p => p.group == agxAct.group).actionOn = true;
                }
                else if (!agxAct.activated) 
                {
                    ActiveActionsState.Find(p => p.group == agxAct.group).actionOff = true;
                }
            }
            
        }

        public static string SaveGroupVisibility(Part p, string str)
        {

            try
            {
                if (p.missionID != FlightGlobals.ActiveVessel.rootPart.missionID)
                {
                   // print("AGXTogSave other vessel");

                    return str;
                }
                else
                {
                    string ReturnStr = ShowGroupInFlightCurrent.ToString(); //add currently show flightgroup
                    
                    for (int i = 1; i <= 250; i++)
                    {
                        ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2,i]).ToString(); //add flight state visibility for each group
                        }
                    }
                    //print("AGXTogSave: " + ReturnStr);    
                    return ReturnStr;
                }
            }
            catch
            {
                print("AGX Fail: SaveGroupVisibility");
                return str;
            }
        }

        public void CalculateActionsToShow()
        {
            ActiveActionsStateToShow.Clear();
            foreach (AGXActionsState actState in ActiveActionsState)
            {
                if (ShowGroupInFlight[ShowGroupInFlightCurrent, actState.group])
                {
                    ActiveActionsStateToShow.Add(actState);
                }
            }
        }

        public void LoadActionGroups()
        {
            
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

                    CurrentVesselActions.AddRange(agpm.LoadActionGroups());
                }
                
                
            }
            LastPartCount = FlightGlobals.ActiveVessel.parts.Count;

            for (int i = 1; i <= 10; i++)
            {
                if (CurrentVesselActions.Count(grp => grp.group == i) > 0)
                {
                    AGXAction TempAgxAct = CurrentVesselActions.Find(agx => agx.group == i);
                    Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
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
                    FlightGlobals.ActiveVessel.ActionGroups[KSPActs[TempAgxAct.group]] = TempAgxAct.activated;
                }
            }

            CalculateActiveActions();
        }
            //CurrentVesselActions.Clear();
           
            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    List<ModuleAGExtData> tempAGXData = p.Modules.OfType<ModuleAGExtData>();
                //List<BaseAction> pActions = new List<BaseAction>();
                //string LoadList = "";

             
                //foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
                //{

                //    LoadList = (string)pm.Fields.GetValue("AGXData");
                //    ModuleAGExtData AGpm = new ModuleAGExtData();
                //    AGpm = (ModuleAGExtData)pm;
                //    pActions.AddRange((List<BaseAction>)AGpm.partAllActions);
                //    //pActions.AddRange((List<BaseAction>)pm.Fields.GetValue("ThisPartActions"));
                   
                //}
               
                //if (LoadList.Length > 0)
                //{
                //    while (LoadList[0] == '\u2023')
                //    {
                 

                //        LoadList = LoadList.Substring(1);

                //        int KeyLength = new int();

                //        int ActGroup = new int();

                //        KeyLength = LoadList.IndexOf('\u2023');
                       
                //        if (KeyLength == -1)
                //        {
                          
                //            ActGroup = Convert.ToInt32(LoadList.Substring(0, 3));
                //            LoadList = LoadList.Substring(3);
                //            CurrentVesselActions.Add(new AGXAction() { group = ActGroup, prt = p, ba = pActions.Find(b => b.guiName == LoadList) });
                            
                //        }
                //        else
                //        {
                           
                //            ActGroup = Convert.ToInt32(LoadList.Substring(0, 3));
                //            LoadList = LoadList.Substring(3);
                //            CurrentVesselActions.Add(new AGXAction() { group = ActGroup, prt = p, ba = pActions.Find(b => b.guiName == LoadList.Substring(0, KeyLength - 3)) });
                //            LoadList = LoadList.Substring(KeyLength - 3);
                           
                //        }
                //    }
                //}
           // }
     
       // }



        public static string SaveGroupNames(Part p, String str)
        {
            try
            {
                
                string SaveStringNames = str;
                if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                {
                    SaveStringNames = "";
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
                
                //print(p.partName + " " + SaveStringNames);
                return SaveStringNames;
            }
            catch
            {
                print("AGX Save Group Names FAIL! (SaveGroupNames)");
                return str;
            }
        }

        //public void UpdateActionsListCheck()
        //{
        //    List<AGXAction> KnownGood = new List<AGXAction>();
        //    KnownGood = new List<AGXAction>();

        //    foreach (AGXAction agxAct in CurrentVesselActions)
        //    {
        //        if (FlightGlobals.ActiveVessel.Parts.Contains(agxAct.prt))
        //        {
        //            KnownGood.Add(agxAct);
        //        }
        //    }
        //    CurrentVesselActions = KnownGood;

        //    ActionsListDirty = false;
        //}

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

        public void CheckActionsActive() //monitor actions state, have to add them manually
        {
            foreach (AGXAction agAct in CurrentVesselActions)
            {
                if (agAct.ba.listParent.module.moduleName == "ModuleDeployableSolarPanel") //only one state on part
                {
                    if ((string)agAct.ba.listParent.module.Fields.GetValue("stateString") == "EXTENDED")
                    {
                        agAct.activated = true;
                    }
                    else
                    {
                        agAct.activated = false;
                    }
                }

                if (agAct.ba.listParent.module.moduleName == "ModuleLandingLeg") //all acts
                {
                    if (agAct.ba.name == "OnAction" || agAct.ba.name == "RaiseAction" || agAct.ba.name == "LowerAction")
                    {
                        if ((int)agAct.ba.listParent.module.Fields.GetValue("savedLegState") == 3)
                        {
                            agAct.activated = true;
                        }
                        else
                        {
                            agAct.activated = false;
                        }
                    }
                    if (agAct.ba.name == "ToggleSuspensionLockAction") //only act
                    {
                        if ((bool)agAct.ba.listParent.module.Fields.GetValue("suspensionLocked") == true)
                        {
                            agAct.activated = true;
                        }
                        else
                        {
                            agAct.activated = false;
                        }
                    }
                }

                    if (agAct.ba.listParent.module.moduleName == "ModuleEngines") //all acts not needed, checks bool directly
                    {
                        ModuleEngines agEng = (ModuleEngines)agAct.ba.listParent.module;
                        agAct.activated = agEng.isOperational;
                    }

                    if (agAct.ba.listParent.module.moduleName == "ModuleEnginesFX")//all acts not needed, checks bool directly
                    {
                        ModuleEnginesFX agEng = (ModuleEnginesFX)agAct.ba.listParent.module;
                        agAct.activated = agEng.isOperational;
                    }
                    if (agAct.ba.listParent.module.moduleName == "ModuleEnviroSensor")
                    {
                        if ((bool)agAct.ba.listParent.module.Fields.GetValue("sensorActive") == true)
                        {
                            agAct.activated = true;
                        }
                        else
                        {
                            agAct.activated = false;
                        }
                    }
                    if (agAct.ba.listParent.module.moduleName == "ModuleGenerator")
                    {
                        if ((bool)agAct.ba.listParent.module.Fields.GetValue("generatorIsActive") == true)
                        {
                            agAct.activated = true;
                        }
                        else
                        {
                            agAct.activated = false;
                        }
                    }
                    if (agAct.ba.listParent.module.moduleName == "ModuleGimbal") //other acts not needed, bool check
                    {
                        
                        if ((bool)agAct.ba.listParent.module.Fields.GetValue("gimbalLock") == false)
                        {
                            agAct.activated = true;
                        }
                        else
                        {
                            agAct.activated = false;
                        }
                    }
                    if (agAct.ba.listParent.module.moduleName == "ModuleLandingGear") //all acts
                    {
                        if (agAct.ba.name == "OnAction")
                        {
                            //print((string)agAct.ba.listParent.module.Fields.GetValue("storedGearState"));
                            if ((string)agAct.ba.listParent.module.Fields.GetValue("storedGearState") == "DEPLOYED")
                            {
                                agAct.activated = true;
                            }
                            else
                            {
                                agAct.activated = false;
                            }
                        }
                        else if (agAct.ba.name == "BrakesAction")
                        {
                            if ((bool)agAct.ba.listParent.module.Fields.GetValue("brakesEngaged") == true)
                            {
                                agAct.activated = true;
                            }
                            else
                            {
                                agAct.activated = false;
                            }
                        }
                        
                    }
                    if (agAct.ba.listParent.module.moduleName == "ModuleSteering") //all acts
                    {
                        if (agAct.ba.name == "InvertSteeringAction")
                        {
                            if ((bool)agAct.ba.listParent.module.Fields.GetValue("invertSteering") == true)
                            {
                                agAct.activated = true;
                            }
                            else
                            {
                                agAct.activated = false;
                            }
                        }
                        if (agAct.ba.name == "ToggleSteeringAction" || agAct.ba.name == "LockSteeringAction" || agAct.ba.name == "UnlockSteeringAction")
                        {
                            
                            if ((bool)agAct.ba.listParent.module.Fields.GetValue("steeringLocked") == false)
                            {
                                agAct.activated = true;
                            }
                            else
                            {
                                agAct.activated = false;
                            }
                        }
                    }
                        if (agAct.ba.listParent.module.moduleName == "ModuleLight") //all acts
                        {
                            if (agAct.ba.name == "ToggleLightAction" || agAct.ba.name == "LightOnAction" || agAct.ba.name == "LightOffAction")
                            { 
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("isOn") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "ModuleRCS") //all ats
                        {
                            
                            if (agAct.ba.name == "ToggleAction")
                            {
                                ModuleRCS rcsMdl = (ModuleRCS)agAct.ba.listParent.module;
                                if (rcsMdl.isEnabled)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "ModuleReactionWheel") //all acts
                        {
                            print((string)agAct.ba.listParent.module.Fields.GetValue("stateString"));
                            if (agAct.ba.name == "Toggle" || agAct.ba.name == "Activate" || agAct.ba.name == "Deactivate")
                            {
                                if ((string)agAct.ba.listParent.module.Fields.GetValue("stateString") == "Disabled")
                                {
                                    agAct.activated = false;
                                }
                                else
                                {
                                    agAct.activated = true;
                                }
                            }
                        }

                        if (agAct.ba.listParent.module.moduleName == "ModuleScienceExperiment") 
                        {
                            if (agAct.ba.name == "DeployAction" || agAct.ba.name == "ResetAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("Deployed") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "ModuleResourceIntake")
                        {
                            if (agAct.ba.name == "ToggleAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("intakeEnabled") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "ModuleWheel")
                        {
                            if (agAct.ba.name == "InvertSteeringAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("invertSteering") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            else if (agAct.ba.name == "ToggleSteeringAction" || agAct.ba.name == "LockSteeringAction" || agAct.ba.name == "UnlockSteeringAction")
                            {

                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("steeringLocked") == false)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            else if (agAct.ba.name == "BrakesAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("brakesEngaged") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            else if (agAct.ba.name == "ToggleMotorAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("motorEnabled") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "ModuleAnimateGeneric")
                        {
                           
                                ModuleAnimateGeneric animPM = (ModuleAnimateGeneric)agAct.ba.listParent.module;
                                //print(ba.name + " " + ba.guiName + " " + animPM.animationName + " " + animPM.animTime);
                            
                            if (agAct.ba.name == "ToggleAction")
                            {
                                if (animPM.animTime == 1f)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "SCANsat") //scansat mod
                        {
                            if (agAct.ba.name == "startScanAction" || agAct.ba.name == "stopScanAction" || agAct.ba.name == "toggleScanAction")
                            {

                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("scanning") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "KethaneConverter") 
                        {
                            if (agAct.ba.name == "ActivateConverterAction" || agAct.ba.name == "DeactivateConverterAction" || agAct.ba.name == "ToggleConverterAction")
                            {

                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "KethaneGenerator") 
                        {
                            if (agAct.ba.name == "EnableAction" || agAct.ba.name == "DisableAction" || agAct.ba.name == "ToggleAction")
                            {

                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("Enabled") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "KethaneDetector") 
                        {
                            if (agAct.ba.name == "EnableDetectionAction" || agAct.ba.name == "DisableDetectionAction" || agAct.ba.name == "ToggleDetectionAction")
                            {

                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsDetecting") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "KethaneExtractor") 
                        {
                            if (agAct.ba.name == "DeployDrillAction" || agAct.ba.name == "RetractDrillAction" || agAct.ba.name == "ToggleDrillAction")
                            {
                                if ((string)agAct.ba.listParent.module.Fields.GetValue("Status") == "Deployed")
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSswitchEngineThrustTransform")
                        {
                            if (agAct.ba.name == "switchTTAction" || agAct.ba.name == "reverseTTAction" || agAct.ba.name == "normalTTAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("isReversed") == false)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSairBrake")
                        {
                            if (agAct.ba.name == "toggleAirBrakeAction")
                            {
                                if ((float)agAct.ba.listParent.module.Fields.GetValue("targetAngle") == 0)
                                {
                                    agAct.activated = false;
                                }
                                else
                                {
                                    agAct.activated = true;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSwing")
                        {
                            if (agAct.ba.name == "toggleLeadingEdgeAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("leadingEdgeExtended") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            if (agAct.ba.name == "extendFlapAction" || agAct.ba.name == "retractFlapAction")
                            {
                                if ((float)agAct.ba.listParent.module.Fields.GetValue("flapMin") == (float)agAct.ba.listParent.module.Fields.GetValue("flapTarget"))
                                {
                                    agAct.activated = false;
                                }
                                else
                                {
                                    agAct.activated = true;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSwingletRangeAdjustment")
                        {
                            if (agAct.ba.name == "lockRangeAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("locked") == true)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSanimateGeneric")
                        {


                            if (agAct.ba.name == "toggleAction")
                            {
                                if ((float)agAct.ba.listParent.module.Fields.GetValue("animTime") == 1f)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FARControllableSurface")
                        {


                            if (agAct.ba.name == "IncreaseDeflect" || agAct.ba.name == "DecreaseDeflect")
                            {
                                if ((int)agAct.ba.listParent.module.Fields.GetValue("flapDeflectionLevel") == 0)
                                {
                                    agAct.activated = false;
                                }
                                else
                                {
                                    agAct.activated = true;
                                }
                            }
                            if (agAct.ba.name == "ActivateSpoiler")
                            {
                                if (FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Brakes])
                                {
                                    agAct.activated = false;
                                }
                                else
                                {
                                    agAct.activated = true;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSrotorTrim")
                        {


                            if (agAct.ba.name == "toggleSteeringAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("steeringEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSswitchEngineThrustTransform")
                        {


                            if (agAct.ba.name == "switchTTAction" || agAct.ba.name == "reverseTTAction" || agAct.ba.name == "normalTTAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("isReversed"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSVTOLrotator")
                        {
                            if (agAct.ba.name == "toggleVTOLAction" || agAct.ba.name == "raiseVTOLAction" || agAct.ba.name == "lowerVTOLAction")
                            {
                                if ((float)agAct.ba.listParent.module.Fields.GetValue("targetAngle") == Mathf.Abs((float)agAct.ba.listParent.module.Fields.GetValue("deployedAngle")))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            if (agAct.ba.name == "toggleVTOLsteeringAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("VTOLsteeringActive"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FScopterThrottle")
                        {
                            if (agAct.ba.name == "toggleHoverAction" || agAct.ba.name == "increaseHeightAction" || agAct.ba.name == "decreaseHeightAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("hoverActive"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSengineHover")
                        {
                            if (agAct.ba.name == "toggleHoverAction" || agAct.ba.name == "increaseVerticalSpeed" || agAct.ba.name == "decreaseVerticalSpeed")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("hoverActive"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FShoverThrottle")
                        {
                            if (agAct.ba.name == "toggleHoverAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("hoverActive"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSpartTurner")
                        {
                            if (agAct.ba.name == "toggleSteeringAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("steeringEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            if (agAct.ba.name == "toggleInvertAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("reversedInput"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FSwheel")
                        {
                            if (agAct.ba.name == "ToggleGearAction")
                            {
                                if ((float)agAct.ba.listParent.module.Fields.GetValue("animTime") == 1f)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            if (agAct.ba.name == "ReverseMotorAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("reverseMotor"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            if (agAct.ba.name == "ToggleMotorAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("motorEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            if (agAct.ba.name == "BrakesAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("brakesEngaged"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "MuMechToggle")
                        {
                            if (agAct.ba.name == "LockToggle")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("isMotionLock"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "DTMagnetometer")
                        {
                            if (agAct.ba.name == "ActivateMagnetometerAction" || agAct.ba.name == "DeactivateMagnetometerAction" || agAct.ba.name == "ToggleMagnetometerAction")
                            {
                                if (agAct.ba.listParent.module.isEnabled)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FNThermalHeatExchanger")
                        {
                            if (agAct.ba.name == "ActivateHeatExchangerAction" || agAct.ba.name == "DeactivateHeatExchangerAction" || agAct.ba.name == "ToggleHeatExchangerAction")
                            {
                                if (agAct.ba.listParent.module.isEnabled)
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "ISRUScoop")
                        {
                            if (agAct.ba.name == "ActivateScoopAction" || agAct.ba.name == "DisableScoopAction" || agAct.ba.name == "ToggleScoopAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("scoopIsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "MicrowavePowerTransmitter")
                        {
                            if (agAct.ba.name == "ActivateTransmitterAction" || agAct.ba.name == "DeactivateTransmitterAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                            if (agAct.ba.name == "ActivateRelayAction" || agAct.ba.name == "DeactivateRelayAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("relay"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "MicrowavePowerTransmitterBackup")
                        {
                            if (agAct.ba.name == "ActivateTransmitterAction" || agAct.ba.name == "DeactivateTransmitterAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "ModuleModableScienceGenerator")
                        {
                            if (agAct.ba.name == "DeployAction" || agAct.ba.name == "ResetAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("Deployed"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "AlcubierreDrive")
                        {
                            if (agAct.ba.name == "StartChargingAction" || agAct.ba.name == "StopChargingAction" || agAct.ba.name == "ToggleChargingAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsCharging"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "MicrowavePowerReceiverBackup")
                        {
                            if (agAct.ba.name == "ActivateReceiverAction" || agAct.ba.name == "DisableReceiverAction" || agAct.ba.name == "ToggleReceiverAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FNGenerator")
                        {
                            if (agAct.ba.name == "ActivateGeneratorAction" || agAct.ba.name == "DeactivateGeneratorAction" || agAct.ba.name == "ToggleGeneratorAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FNRadiator")
                        {
                            if (agAct.ba.name == "DeployRadiatorAction" || agAct.ba.name == "RetractRadiatorAction" || agAct.ba.name == "ToggleRadiatorAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("radiatorIsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "FNReactor")
                        {
                            if (agAct.ba.name == "ActivateReactorAction" || agAct.ba.name == "DeactivateReactorAction" || agAct.ba.name == "ToggleReactorAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("IsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "MicrowavePowerReceiver")
                        {
                            if (agAct.ba.name == "ActivateReceiverAction" || agAct.ba.name == "DisableReceiverAction" || agAct.ba.name == "ToggleReceiverAction")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("receiverIsEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                        if (agAct.ba.listParent.module.moduleName == "TacGenericConverter")
                        {
                            if (agAct.ba.name == "ToggleConverter")
                            {
                                if ((bool)agAct.ba.listParent.module.Fields.GetValue("converterEnabled"))
                                {
                                    agAct.activated = true;
                                }
                                else
                                {
                                    agAct.activated = false;
                                }
                            }
                        }
                       


                //else //all other modules, check the isEnabled bool
                //{
                //    agAct.activated = agAct.ba.listParent.module.isEnabled;
                //}
            }
            
            CalculateActionsState();
        }
    }
    
}