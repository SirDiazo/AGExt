using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using UnityEngine;






namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AGXEditor : PartModule
    {
        bool showCareerStockAGs = false; //support locking action groups in early career
        bool showCareerCustomAGs = false;
        bool showAGXRightClickMenu = false; //show stock toolbar right click menu?
        ApplicationLauncherButton AGXAppEditorButton = null; //stock toolbar button instance
        bool EditorShowToolbarPopout = false;
        bool defaultShowingNonNumeric = false; //are we in non-numeric (abort/brakes/gear/list) mode?
        List<BaseAction> defaultActionsListThisType; //list of default actions showing in group win when non-numeric
        List<BaseAction> defaultActionsListAll; //list of all default actions on vessel, only used in non-numeric mode when going to other mode
        KSPActionGroup defaultGroupToShow = KSPActionGroup.Abort; //which group is currently selected if showing non-numeric groups
        List<AGXAction> ThisGroupActions;
        //private Part AGXRootPart = null;
        private string LastKeyCode = "";
        public static Dictionary<int, bool> IsGroupToggle; //is group a toggle group?
        public static bool[,] ShowGroupInFlight; //Show group in flight?

        public static string[] ShowGroupInFlightNames;
        public static int ShowGroupInFlightCurrent = 1;

        private List<Part> SelectedWithSym; //selected parts from last frame for Default actions monitoring
        private List<AGXDefaultCheck> SelectedWithSymActions; //monitor baseaction.actiongroups of selected parts
        public static bool NeedToLoadActions = true;
        public static bool LoadFinished = false;
        //Selected Parts Window Variables
        public static Rect SelPartsWin;
        private static Vector2 ScrollPosSelParts;
        public static Vector2 ScrollPosSelPartsActs;
        public static Vector2 ScrollGroups;
        public static Vector2 CurGroupsWin;
        private static List<AGXPart> AGEditorSelectedParts;
        private static bool AGEEditorSelectedPartsSame = false;
        //private static GUIStyle AGXWinStyle = null;
        private static int SelectedPartsCount = 0;
        private static bool ShowSelectedWin = true;
        private static Part PreviousSelectedPart = null;
        private static bool SelPartsIncSym = true;
        private static string BtnTxt;
        private static bool AGXDoLock = false;
        private static Rect TestWin;
        private static Part AGXRoot;
        public static List<AGXAction> DetachedPartActions;
        public static Timer DetachedPartReset;

        //private static bool NeedToSave = false;
        private static int GroupsPage = 1;
        private static string CurGroupDesc;
        private static bool AutoHideGroupsWin = false;
        private static bool TempShowGroupsWin = false;
        private static Rect KeyCodeWin;
        private static Rect CurActsWin;
        private static bool ShowKeyCodeWin = false;
        private static bool ShowJoySticks = false;
        private static bool ShowKeySetWin = false;
        private static int CurrentKeySet = 1;
        private static string CurrentKeySetName;
        private static Rect KeySetWin;
        private static ConfigNode AGExtNode;
        static string[] KeySetNames = new string[5];
        private static bool TrapMouse = false;
        //private static int LastPartCount = 0;
        public static List<AGXAction> CurrentVesselActions;
        // private static bool MouseOverExitBtns = false;

        private IButton AGXBtn;



        public static Rect GroupsWin;
        public static bool Trigger;
        //private static bool Trigger2;
        //private static int Value = 0;
        public static string HexStr = "Load";

        private static List<BaseAction> PartActionsList;

        public static ConfigNode AGXEditorNode;
        Vector2 groupWinScroll = new Vector2();
        bool highlightPartThisFrameGroupWin = false;
        static Texture2D BtnTexRed = new Texture2D(1, 1);
        static Texture2D BtnTexGrn = new Texture2D(1, 1);
        public static Dictionary<int, string> AGXguiNames;
        public static Dictionary<int, KeyCode> AGXguiKeys;
        public static Dictionary<int, bool> AGXguiMod1Groups;
        public static Dictionary<int, bool> AGXguiMod2Groups;
        public static KeyCode AGXguiMod1Key = KeyCode.None;
        public static KeyCode AGXguiMod2Key = KeyCode.None;
        private bool AGXguiMod1KeySelecting = false;
        private bool AGXguiMod2KeySelecting = false;
        public static int AGXCurActGroup = 1;
        static List<string> KeyCodeNames = new List<string>();
        static List<string> JoyStickCodes = new List<string>();
        private static bool ActionsListDirty = true; //is our actions requiring update?
        private static bool ShowCurActsWin = true;
        public static Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
        public static Dictionary<int, bool> isDirectAction = new Dictionary<int, bool>();
        private bool AGXShow = false;
        private static GUISkin AGXSkin;
        public static GUIStyle AGXWinStyle = null;
        //private static GUIStyle TWR1WinStyle = null; //window style
        private static GUIStyle AGXLblStyle = null; //window style
        private static GUIStyle AGXBtnStyle = null; //window style
        private static GUIStyle AGXFldStyle = null; //window style
        static Texture2D ButtonTexture = new Texture2D(64, 64);
        static Texture2D ButtonTextureRed = new Texture2D(64, 64);
        static Texture2D ButtonTextureGreen = new Texture2D(64, 64);
        bool checkShipsExist = false; //flag to check existing ships on load window open
        int checkShipsExistDelay = 0;//delay timer to wait after opening load ship window
        static bool inVAB = true; //true if in VAB, flase in SPH
        bool highlightPartThisFrameSelWin = false;
        bool highlightPartThisFrameActsWin = false;
        Part partToHighlight = null;
        Texture2D PartCenter = new Texture2D(41, 41);
        Texture2D PartCross = new Texture2D(21, 21);
        Texture2D PartPlus = new Texture2D(21, 21);
        bool showAllPartsList = false; //show list of all parts in group window?
        List<string> showAllPartsListTitles; //list of all parts with actions to show in group window
        KSPActionGroup KSPDefaultLastActionGroup = KSPActionGroup.Custom01;
        //static Part partLastHighlight = null;
        ////static Color partHighlighLastColor;
        //static Part.HighlightType partHighlightLastType;
        //static Material[] partHighlightLastMaterial;


        public class SettingsWindowEditor : MonoBehaviour, IDrawable
        {
            public Rect SettingsWinEditor = new Rect(0, 0, 150, 85);
            public Vector2 Draw(Vector2 position)
            {
                var oldSkin = GUI.skin;
                GUI.skin = HighLogic.Skin;

                SettingsWinEditor.x = position.x;
                SettingsWinEditor.y = position.y;

                GUI.Window(2233452, SettingsWinEditor, DrawSettingsWinEditor, "AGX Settings", AGXEditor.AGXWinStyle);
                //RCSlaWin = GUILayout.Window(42334567, RCSlaWin, DrawWin, (string)null, GUI.skin.box);
                //GUI.skin = oldSkin;

                return new Vector2(SettingsWinEditor.width, SettingsWinEditor.height);
            }

            public void DrawSettingsWinEditor(int WindowID)
            {

                if (GUI.Button(new Rect(10, 25, 130, 25), "Reset Windows"))
                {
                    KeySetWin.x = 250;
                    KeySetWin.y = 250;
                    GroupsWin.x = 350;
                    GroupsWin.y = 350;
                    SelPartsWin.x = 200;
                    SelPartsWin.y = 200;
                    KeyCodeWin.x = 300;
                    KeyCodeWin.y = 300;
                    CurActsWin.x = 150;
                    CurActsWin.y = 150;


                }
                AGXBtnStyle.normal.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                AGXBtnStyle.hover.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                if (GUI.Button(new Rect(10, 50, 130, 25), "Auto-Hide Groups", AGXBtnStyle))
                {
                    AutoHideGroupsWin = !AutoHideGroupsWin;
                }
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
            }
            public void Update()
            {

            }
        }
        public void DrawSettingsWinEditor(int WindowID)
        {

            if (GUI.Button(new Rect(10, 25, 130, 25), "Reset Windows"))
            {
                KeySetWin.x = 250;
                KeySetWin.y = 250;
                GroupsWin.x = 350;
                GroupsWin.y = 350;
                SelPartsWin.x = 200;
                SelPartsWin.y = 200;
                KeyCodeWin.x = 300;
                KeyCodeWin.y = 300;
                CurActsWin.x = 150;
                CurActsWin.y = 150;


            }
            AGXBtnStyle.normal.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
            AGXBtnStyle.hover.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
            if (GUI.Button(new Rect(10, 50, 130, 25), "Auto-Hide Groups", AGXBtnStyle))
            {
                AutoHideGroupsWin = !AutoHideGroupsWin;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;

        }
        public void Start()
        {
            ShowKeyCodeWin = false;
            ShowKeySetWin = false;
            AGXguiMod1Groups = new Dictionary<int, bool>();
            AGXguiMod2Groups = new Dictionary<int, bool>();
            for (int i = 1; i <= 250; i++)
            {
                AGXguiMod1Groups[i] = false;
                AGXguiMod2Groups[i] = false;
            }
            defaultActionsListThisType = new List<BaseAction>(); //initialize list
            defaultActionsListAll = new List<BaseAction>(); //initialize list
            //foreach (Part p in 
            string errLine = "1";
            ThisGroupActions = new List<AGXAction>();
            //var EdPnl = EditorPanels.Instance.actions;
            //EditorActionGroups.Instance.groupActionsList.AddValueChangedDelegate(OnGroupActionsListChange);
            try
            {
                errLine = "2";
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

                TestWin = new Rect(600, 300, 100, 200);
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
                errLine = "3";

                KeyCodeNames = new List<String>();
                KeyCodeNames.AddRange(Enum.GetNames(typeof(KeyCode)));
                KeyCodeNames.Remove("None");
                JoyStickCodes.AddRange(KeyCodeNames.Where(JoySticks));
                KeyCodeNames.RemoveAll(JoySticks);
                //AGExtNode = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                AGExtNode = AGXStaticData.LoadBaseConfigNode();
                errLine = "4";
                if (AGExtNode.GetValue("EditShow") == "0")
                {
                    AGXShow = false;
                }
                else
                {
                    AGXShow = true;
                }
                errLine = "5";
                CurrentKeySet = 1;
                errLine = "6";
                //LoadCurrentKeySet();
                CurrentKeySetName = (string)AGExtNode.GetValue("KeySetName" + CurrentKeySet);
                KeySetNames[0] = (string)AGExtNode.GetValue("KeySetName1");
                KeySetNames[1] = (string)AGExtNode.GetValue("KeySetName2");
                KeySetNames[2] = (string)AGExtNode.GetValue("KeySetName3");
                KeySetNames[3] = (string)AGExtNode.GetValue("KeySetName4");
                KeySetNames[4] = (string)AGExtNode.GetValue("KeySetName5");
                errLine = "7";
                CurrentVesselActions = new List<AGXAction>();
                errLine = "8";
                AGXRoot = null;
                errLine = "9";
                StartLoadWindowPositions();
                errLine = "14";
                //print("a");
                if (AGExtNode.HasValue("OverrideCareer")) //are action groups unlocked?
                {
                    //print("b");
                    if ((string)AGExtNode.GetValue("OverrideCareer") == "1")
                    {
                        //print("c");
                        showCareerCustomAGs = true;
                        showCareerStockAGs = true;
                    }
                    else
                    {
                        //print("d");
                        float facilityLevel = 0f;
                        if (EditorDriver.editorFacility == EditorFacility.SPH)
                        {
                            // print("e");
                            facilityLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar);
                            //print("AGX Career check SPH: " + facilityLevel);
                        }
                        else
                        {
                            //print("f");
                            facilityLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                            //print("AGX Career check VAB: " + facilityLevel);
                        }

                        if (GameVariables.Instance.UnlockedActionGroupsCustom(facilityLevel))
                        {
                            // print("g");
                            showCareerStockAGs = true;
                            showCareerCustomAGs = true;
                        }
                        else if (GameVariables.Instance.UnlockedActionGroupsStock(facilityLevel))
                        {
                            //print("h");
                            showCareerStockAGs = true;
                            showCareerCustomAGs = false;
                        }
                        else
                        {
                            //print("i");
                            showCareerStockAGs = false;
                            showCareerCustomAGs = false;
                        }
                    }
                }
                else
                {
                    // print("j");
                    float facilityLevel2 = 0f;
                    if (EditorDriver.editorFacility == EditorFacility.SPH)
                    {
                        // print("k");
                        facilityLevel2 = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar);
                    }
                    else
                    {
                        //print("l");
                        facilityLevel2 = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                    }

                    if (GameVariables.Instance.UnlockedActionGroupsCustom(facilityLevel2))
                    {
                        //print("m");
                        showCareerStockAGs = true;
                        showCareerCustomAGs = true;
                    }
                    else if (GameVariables.Instance.UnlockedActionGroupsStock(facilityLevel2))
                    {
                        //print("n");
                        showCareerStockAGs = true;
                        showCareerCustomAGs = false;
                    }
                    else
                    {
                        //print("o");
                        showCareerStockAGs = false;
                        showCareerCustomAGs = false;
                    }
                }
                //print("startd " + showCareerCustomAGs);


                //LoadCurrentKeyBindings();

                errLine = "15";
                if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
                {

                    AGXBtn = ToolbarManager.Instance.add("AGX", "AGXBtn");
                    AGXBtn.TexturePath = "Diazo/AGExt/icon_button";
                    AGXBtn.ToolTip = "Action Groups Extended";
                    AGXBtn.OnClick += (e) =>
                    {
                        //List<UnityEngine.Transform> UIPanelList = new List<UnityEngine.Transform>(); //setup list to find Editor Actions UI transform into a list. Could not figure out how to find just a transform
                        //UIPanelList.AddRange(FindObjectsOfType<UnityEngine.Transform>().Where(n => n.name == "PanelActionGroups")); //actual find command
                        if (e.MouseButton == 0)
                        {
                            onLeftButtonClick();
                        }
                        if (e.MouseButton == 1)
                        {
                            if (EditorLogic.fetch.editorScreen == EditorScreen.Actions)
                            {
                                if (EditorShowToolbarPopout)
                                {
                                    AGXBtn.Drawable = null;
                                    EditorShowToolbarPopout = false;
                                }
                                else
                                {
                                    SettingsWindowEditor SettingsEditor = new SettingsWindowEditor();
                                    AGXBtn.Drawable = SettingsEditor;
                                    EditorShowToolbarPopout = true;
                                }
                            }
                        }
                    };
                }
                else
                {
                    //AGXShow = true; //toolbar not installed, show AGX regardless

                    //now using stock toolbar as fallback
                    AGXAppEditorButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, (Texture)GameDatabase.Instance.GetTexture("Diazo/AGExt/icon_button", false));
                }
                errLine = "16";

                DetachedPartActions = new List<AGXAction>();

                DetachedPartReset = new Timer();
                DetachedPartReset.Interval = 500;

                DetachedPartReset.Stop();
                DetachedPartReset.AutoReset = true;

                DetachedPartReset.Elapsed += new ElapsedEventHandler(ResetDetachedParts);

                SelectedWithSym = new List<Part>();
                SelectedWithSymActions = new List<AGXDefaultCheck>();
                errLine = "17";
                EditorPanels.Instance.actions.AddValueChangedDelegate(OnUIChanged); //detect when EditorPanel moves. this ONLY detects editor panel, going from parts to crew will NOT trigger this
                EditorLogic.fetch.crewPanelBtn.AddValueChangedDelegate(OnOtherButtonClick); //detect when Part button clicked at top of screen
                EditorLogic.fetch.partPanelBtn.AddValueChangedDelegate(OnOtherButtonClick); //detect when Crew button clicked at top of screen
                EditorLogic.fetch.loadBtn.AddValueChangedDelegate(OnLoadButtonClick); //load button clicked to check for deleted ships
                EditorLogic.fetch.saveBtn.AddValueChangedDelegate(OnSaveButtonClick); //run save when save button clicked. auto-save from Scenario module only runs on leaving editor! not on clicking save button
                EditorLogic.fetch.launchBtn.AddValueChangedDelegate(OnSaveButtonClick);
                EditorLogic.fetch.exitBtn.AddValueChangedDelegate(OnSaveButtonClick);
                EditorLogic.fetch.newBtn.AddValueChangedDelegate(OnSaveButtonClick);

                //GameEvents.onGameSceneLoadRequested.Add(LeavingEditor);
                errLine = "18";
                IsGroupToggle = new Dictionary<int, bool>();
                ShowGroupInFlight = new bool[6, 251];
                ShowGroupInFlightNames = new string[6];

                ShowGroupInFlightNames[1] = "Group 1";
                ShowGroupInFlightNames[2] = "Group 2";
                ShowGroupInFlightNames[3] = "Group 3";
                ShowGroupInFlightNames[4] = "Group 4";
                ShowGroupInFlightNames[5] = "Group 5";

                errLine = "19";

                for (int i = 1; i <= 250; i++)
                {
                    IsGroupToggle[i] = false;
                    for (int i2 = 1; i2 <= 5; i2++)
                    {
                        ShowGroupInFlight[i2, i] = true;
                    }
                }
                AGXSkin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
                AGXWinStyle = new GUIStyle(AGXSkin.window);
                AGXLblStyle = new GUIStyle(AGXSkin.label);
                AGXFldStyle = new GUIStyle(AGXSkin.textField);
                AGXFldStyle.fontStyle = FontStyle.Normal;
                //AGXFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);
                AGXFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                AGXLblStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                AGXLblStyle.wordWrap = false;
                errLine = "20";
                AGXBtnStyle = new GUIStyle(AGXSkin.button);
                AGXBtnStyle.fontStyle = FontStyle.Normal;
                AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                AGXBtnStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                //AGXScrollStyle.normal.background = null;
                //print("AGX " + AGXBtnStyle.normal.background);
                byte[] importTxt = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/ButtonTexture.png");
                byte[] importTxtRed = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/ButtonTextureRed.png");
                byte[] importTxtGreen = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/ButtonTextureGreen.png");
                byte[] importPartCenter = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/PartLocationCross.png");
                byte[] importPartCross = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/PartLocCross.png");
                byte[] importPartPlus = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/PartLocPlus.png");
                //byte[] testXport = AGXBtnStyle.normal.background.EncodeToPNG();
                //File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", testXport);
                ButtonTexture.LoadImage(importTxt);
                ButtonTexture.Apply();
                ButtonTextureRed.LoadImage(importTxtRed);
                ButtonTextureRed.Apply();
                ButtonTextureGreen.LoadImage(importTxtGreen);
                ButtonTextureGreen.Apply();
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.onNormal.background = ButtonTexture;
                AGXBtnStyle.onActive.background = ButtonTexture;
                AGXBtnStyle.onFocused.background = ButtonTexture;
                AGXBtnStyle.onHover.background = ButtonTexture;
                AGXBtnStyle.active.background = ButtonTexture;
                AGXBtnStyle.focused.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
                PartCenter.LoadImage(importPartCenter);
                PartCenter.Apply();
                PartCross.LoadImage(importPartCross);
                PartCross.Apply();
                PartPlus.LoadImage(importPartPlus);
                PartPlus.Apply();
                //EditorLoadFromFile();
                //if (HighLogic.LoadedScene == GameScenes.EDITOR)
                if (EditorDriver.editorFacility == EditorFacility.VAB)
                {
                    inVAB = true;
                }
                else
                {
                    inVAB = false;
                }
                GameEvents.onPartAttach.Add(PartAttaching);// this game event only fires for part removed, not child parts
                GameEvents.onPartRemove.Add(PartRemove);
                GameEvents.onEditorShipModified.Add(VesselChanged);
                isDirectAction = new Dictionary<int, bool>();
                CurrentVesselActions.Clear();
                EditorLoadFromFile();
                EditorLoadFromNode();
                errLine = "21";

                //print("Loading now");
                //EditorActionGroups.Instance.groupActionsList.AddValueChangedDelegate(OnGroupActionsListChange);
                LoadFinished = true;
                Debug.Log("AGX Editor Start Okay");
            }
            catch (Exception e)
            {
                print("AGX Editor Start Fail " + errLine + " " + e);
                print("AGX AGExt node dump: " + AGExtNode);
            }
        }

        //public void OnGroupActionsListChange(IUIObject obj)
        //{
        //    print("List change " + obj.GetType());
        //    UIScrollList lst = (UIScrollList)obj;
        //    print(lst.
        //}

        public void StartLoadWindowPositions()
        {
            string errLine = "1";
            try
            {
                int WinX;
                int WinY;
                if (Int32.TryParse((string)AGExtNode.GetValue("EdGroupsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdGroupsY"), out WinY))
                {
                    GroupsWin = new Rect(WinX, WinY, 250, 530);
                }
                else
                {
                    GroupsWin = new Rect(100, 100, 250, 530);
                }
                errLine = "10";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdSelPartsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdSelPartsY"), out WinY))
                {
                    SelPartsWin = new Rect(WinX, WinY, 365, 270);
                }
                else
                {
                    SelPartsWin = new Rect(100, 100, 365, 270);
                }
                errLine = "11";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdKeyCodeX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdKeyCodeY"), out WinY))
                {
                    KeyCodeWin = new Rect(WinX, WinY, 410, 730);
                }
                else
                {
                    KeyCodeWin = new Rect(100, 100, 410, 730);
                }
                errLine = "12";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdKeySetX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdKeySetY"), out WinY))
                {
                    KeySetWin = new Rect(WinX, WinY, 185, 335);
                }
                else
                {
                    KeySetWin = new Rect(100, 100, 185, 335);
                }
                errLine = "13";
                if (Int32.TryParse((string)AGExtNode.GetValue("EdCurActsX"), out WinX) && Int32.TryParse((string)AGExtNode.GetValue("EdCurActsY"), out WinY))
                {
                    CurActsWin = new Rect(WinX, WinY, 345, 140);
                }
                else
                {
                    CurActsWin = new Rect(100, 100, 345, 140);
                }
            }
            catch (Exception e)
            {
                print("AGX StartLoadWindowPostion Error, Recovered. " + errLine + " " + e);
                GroupsWin = new Rect(100, 100, 250, 530);
                SelPartsWin = new Rect(100, 100, 365, 270);
                KeyCodeWin = new Rect(100, 100, 410, 730);
                KeySetWin = new Rect(100, 100, 185, 335);
                CurActsWin = new Rect(100, 100, 345, 140);
            }
        }

        public void DummyVoid()
        {

        }
        public void onStockToolbarClick()
        {
            if (showCareerStockAGs)
            {
                //print("mouse " + Input.GetMouseButtonUp(1) + Input.GetMouseButtonDown(1));
                if (Input.GetMouseButtonUp(1))
                {
                    onRightButtonStockClick();
                }
                else
                {
                    onLeftButtonClick();
                }
            }
        }

        public void onRightButtonStockClick()
        {
            showAGXRightClickMenu = !showAGXRightClickMenu;
        }

        public void onLeftButtonClick()
        {
            if (showCareerStockAGs)
            {
                if (EditorLogic.fetch.editorScreen == EditorScreen.Actions)
                {
                    if (AGXShow)
                    {
                        //UIPanelList.First().Translate(new Vector3(500f, 0, 0), UIPanelList.First().parent.transform); //hide UI panel
                        AGXShow = false;
                        AGExtNode.SetValue("EditShow", "0");
                        EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions);

                    }
                    else
                    {
                        // UIPanelList.First().Translate(new Vector3(-500f, 0, 0), UIPanelList.First().parent.transform); //show UI panel
                        AGXShow = true;
                        AGExtNode.SetValue("EditShow", "1");
                        EditorPanels.Instance.panelManager.Dismiss();
                    }
                    //AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                    AGXStaticData.SaveBaseConfigNode(AGExtNode);
                }
                else
                {
                    EditorLogic.fetch.SelectPanelActions();
                }
            }
        }


        public void PartAttaching(GameEvents.HostTargetAction<Part, Part> host_target)
        {
            string ErrLine = "1";
            try
            {
                ErrLine = "2";
                //// print("Part attached! " + host_target.host.ConstructID + " " + host_target.target.ConstructID);
                // //this.part, partAllActions, partAGActions);
                // ErrLine = "3";
                // foreach (Part p in host_target.host.FindChildParts<Part>(true)) //action only fires for part clicked on, have to parse all child parts this way
                // {
                //     ErrLine = "4";
                //    // print("Part atached2! " + p.ConstructID);

                //     ErrLine = "5";
                // }
                ErrLine = "6";

                ErrLine = "7";
                ModuleAGX agxMod = host_target.host.Modules.OfType<ModuleAGX>().First();
                ErrLine = "8";
                foreach (AGXAction agAct in agxMod.agxActionsThisPart)
                {

                    ErrLine = "9";

                    if (!CurrentVesselActions.Contains(agAct))
                    {
                        //print("part attached detect");
                        //Debug.Log("part attached detect"); 
                        ErrLine = "10";
                        CurrentVesselActions.Add(agAct);
                        //DetachedPartActions.Add(agAct);
                        ErrLine = "11";

                        ErrLine = "11b";
                        if (AGXguiNames[agAct.group].Length == 0)
                        {
                            ErrLine = "12";
                            AGXguiNames[agAct.group] = agAct.grpName;
                            ErrLine = "13";
                        }

                        ErrLine = "14";
                    }
                    else
                    {
                        //print("part attached not detect");
                        //Debug.Log("part attached not detect"); 
                    }
                    ErrLine = "15";
                    DetachedPartActions.Add(agAct);
                }
                ErrLine = "16";
                AttachAGXPart(host_target.host);
                foreach (Part p in host_target.host.FindChildParts<Part>(true)) //action only fires for part clicked on, have to parse all child parts this way
                {
                    ErrLine = "17";
                    agxMod = p.Modules.OfType<ModuleAGX>().First();
                    ErrLine = "18";
                    foreach (AGXAction agAct in agxMod.agxActionsThisPart)
                    {
                        ErrLine = "19";
                        DetachedPartActions.Add(agAct);
                        if (!CurrentVesselActions.Contains(agAct))
                        {
                            //print("adding action " + agAct.ba.guiName + agAct.group);
                            ErrLine = "20";
                            CurrentVesselActions.Add(agAct);

                            ErrLine = "21";
                            if (AGXguiNames[agAct.group].Length == 0)
                            {
                                ErrLine = "22";
                                AGXguiNames[agAct.group] = agAct.grpName;
                                ErrLine = "23";
                            }
                        }
                    }
                    AttachAGXPart(p);
                }
                DetachedPartReset.Start();
                //RefreshDefaultActionsList();

            }
            catch (Exception e)
            {
                print("AGX Part Attaching Fail: " + ErrLine + " " + e);
            }
        }

        public void PartRemove(GameEvents.HostTargetAction<Part, Part> host_target)
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                UpdateAGXActionGroupNames();
                errLine = "3";
                //print("Part detached! " + host_target.target.ConstructID);
                DetachedPartActions.AddRange(CurrentVesselActions.Where(p3 => p3.ba.listParent.part == host_target.target)); //add actiongroups on this part to List
                errLine = "4";
                foreach (Part p in host_target.target.FindChildParts<Part>(true)) //action only fires for part clicked on, have to parse all child parts this way
                {
                    errLine = "5";
                    // print("Part detached2! " + p.ConstructID);
                    DetachedPartActions.AddRange(CurrentVesselActions.Where(p3 => p3.ba.listParent.part == p)); //add parts to list
                    errLine = "6";
                }
                errLine = "7";
                DetachedPartReset.Stop(); //stop timer so it resets
                //        //print("Detach");
                //start subassembly stuff
                errLine = "8";
                ModuleAGX agxMod = host_target.target.Modules.OfType<ModuleAGX>().First();
                errLine = "9";
                agxMod.agxActionsThisPart.AddRange(CurrentVesselActions.Where(p3 => p3.ba.listParent.part == host_target.target));
                errLine = "10";
                foreach (Part p in host_target.target.FindChildParts<Part>(true)) //action only fires for part clicked on, have to parse all child parts this way
                {
                    errLine = "11";
                    agxMod = p.Modules.OfType<ModuleAGX>().First();
                    errLine = "12";
                    agxMod.agxActionsThisPart.AddRange(CurrentVesselActions.Where(p3 => p3.ba.listParent.part == p)); //add parts to list
                    errLine = "13";
                }
                errLine = "14";
            }
            catch (Exception e)
            {
                print("AGX PartRemove FAIL " + errLine + " " + e);
            }

        }

        public static void UpdateAGXActionGroupNames()
        {
            foreach (AGXAction agAct in CurrentVesselActions)
            {
                try
                {
                    agAct.grpName = AGXguiNames[agAct.group];
                }
                catch
                {
                    //empty, silently fail
                }
            }
        }

        public void CheckExistingShips()
        {
            List<string> existingShipsList = new List<string>();
            string fileDir = new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/Ships/VAB";
            int fileLen = fileDir.Length;
            string[] fileList = Directory.GetFiles(fileDir);
            //print("sc3 " + loadShipList.Length);
            foreach (string file in fileList)
            {
                existingShipsList.Add(AGextScenario.EditorHashShipName(file.Substring(fileLen + 1, file.Length - fileLen - 7), true));
            }
            fileDir = new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/Ships/SPH";

            fileLen = fileDir.Length;
            fileList = Directory.GetFiles(fileDir);
            foreach (string file in fileList)
            {
                existingShipsList.Add(AGextScenario.EditorHashShipName(file.Substring(fileLen + 1, file.Length - fileLen - 7), false));
            }
            fileDir = new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "Ships/SPH";
            fileLen = fileDir.Length;
            fileList = Directory.GetFiles(fileDir);
            foreach (string file in fileList)
            {
                existingShipsList.Add(AGextScenario.EditorHashShipName(file.Substring(fileLen + 1, file.Length - fileLen - 7), false));
            }
            fileDir = new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "Ships/VAB";
            fileLen = fileDir.Length;
            fileList = Directory.GetFiles(fileDir);
            foreach (string file in fileList)
            {
                existingShipsList.Add(AGextScenario.EditorHashShipName(file.Substring(fileLen + 1, file.Length - fileLen - 7), true));
            }
            //ConfigNode AGXBaseNode = AGextScenario.LoadBaseNode();
            ConfigNode AGXEditorNode = new ConfigNode("EDITOR");
            AGXEditorNode.AddValue("name", "editor");
            if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg"))
            {
                AGXEditorNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
            }
            foreach (ConfigNode VslNode in AGXEditorNode.nodes)
            {
                if (!existingShipsList.Contains(VslNode.name))
                {
                    AGXEditorNode.RemoveNode(VslNode.name);
                    //AGXBaseNode.RemoveNode("EDITOR");
                    //AGXBaseNode.AddNode(AGXEditorNode);
                    AGXEditorNode.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
                    //print("Existing ship check node save " + AGXEditorNode);
                    goto BreakOut;
                }
            }
        BreakOut:
            fileList = null;


        }

        public void OnSaveButtonClick(IUIObject obj)
        {
            EditorSaveToFile();
        }

        public void OnLoadButtonClick(IUIObject obj)
        {
            EditorSaveToFile();
            checkShipsExist = true;
            //print("ship count1 ");
            //EditorStartPodDialog loadShipWin = FindObjectOfType<EditorStartPodDialog>();
            //print("ship count " + loadShipWin.availablePods.Count);
        }

        public static void LoadGroupVisibility(string LoadString)
        {
            string errLine = "1";
            try
            {

                if (LoadString.Length == 1501)
                {
                    errLine = "15";
                    ShowGroupInFlightCurrent = Convert.ToInt32(LoadString.Substring(0, 1));
                    errLine = "16";
                    LoadString = LoadString.Substring(1);
                    errLine = "17";
                    for (int i = 1; i <= 250; i++)
                    {
                        errLine = "18";
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
                    errLine = "19";

                }

                else
                {
                    for (int i = 1; i <= 250; i++)
                    {
                        errLine = "20";
                        IsGroupToggle[i] = false;
                        for (int i2 = 1; i2 <= 5; i2++)
                        {

                            ShowGroupInFlight[i2, i] = true;
                        }
                    }
                }
                errLine = "21";
            }
            catch (Exception e)
            {
                print("AGX LoadGroupVisibility Fail " + errLine + " " + e);
            }
        }

        public void OnOtherButtonClick(IUIObject obj) //reset EditorPanel if needed
        {
            //only run this if the Action Panel was hidden by other code
            if (AGXShow)
            {
                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions);
            }
            AGEditorSelectedParts.Clear();
            EditorSaveToNode();

        }

        public void OnUIChanged(IUIObject obj)
        {

            if (EditorLogic.fetch.editorScreen == EditorScreen.Actions) //we in action groups mode?
            {
                if (AGXShow)
                {
                    EditorPanels.Instance.panelManager.Dismiss(); //show AGX? hide panel
                }
                else
                {
                    EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions); //hide AGX? show panel
                }
            }
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

        public static void SetDefaultActionStatic(BaseAction ba, int group)
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

        public static void ResetDetachedParts(object source, ElapsedEventArgs e)
        {

            DetachedPartReset.Stop();
            foreach (AGXAction agAct in DetachedPartActions)
            {
                foreach (Part p in agAct.prt.symmetryCounterparts)
                {
                    AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(AGextScenario.SaveAGXActionVer2(agAct), p, false);
                    if (!CurrentVesselActions.Contains(actToAdd))
                    {
                        CurrentVesselActions.Add(actToAdd);
                        //print("add act");
                    }
                    p.Modules.OfType<ModuleAGX>().First().agxActionsThisPart.Clear();
                }
            }
            DetachedPartActions.Clear();
            EditorSaveToNode();

        }

        public void VesselChanged(ShipConstruct sc)
        {
            //print("vessel change fire");
            RefreshDefaultActionsList();
        }

        public static void AttachAGXPart(Part p)
        {
            //print("d1");
            if (DetachedPartActions.Count(a => a.ba.listParent.part == p) == 0) //part has no actions in list, may be a clone
            {
                //print("d2");
                foreach (Part p2 in p.symmetryCounterparts) //check any symmetry counterparts
                {
                    //print("d3");
                    if (DetachedPartActions.Count(a => a.ba.listParent.part == p2) > 0) //symmetry counterpart has at least one action
                    {
                        //print("d4");
                        foreach (AGXAction agAct in DetachedPartActions.Where(p3 => p3.ba.listParent.part == p2))
                        {
                            //print("d5");
                            AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(AGextScenario.SaveAGXActionVer2(agAct), p, false);
                            if (actToAdd.ba != null)
                            {
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(CurrentVesselActions);
                                Checking.RemoveAll(p4 => p4.ba != actToAdd.ba);
                                Checking.RemoveAll(p5 => p5.group != actToAdd.group);
                                if (Checking.Count == 0)
                                {
                                    CurrentVesselActions.Add(actToAdd);
                                    //print("d6");
                                    if (actToAdd.group < 11)
                                    {
                                        SetDefaultActionStatic(actToAdd.ba, actToAdd.group);
                                    }
                                }
                                else
                                {
                                    //print("d6 fail");
                                }
                            }
                        }
                    }
                }
            }
        }


        //public static List<AGXAction> AttachAGXPart(Part p, List<BaseAction> baList, List<AGXAction> agxList)
        //{
        //    List<AGXAction> RetActs = new List<AGXAction>();
        //    List<Part> symParts = new List<Part>();
        //    symParts.Add(p);
        //    symParts.AddRange(p.symmetryCounterparts);
        //    foreach (AGXAction agAct in DetachedPartActions)
        //    {
        //        if (symParts.Contains(agAct.prt))
        //        {
        //            AGXAction ToAdd = new AGXAction() { prt = p, ba = baList.First(b => b.name == agAct.ba.name), group = agAct.group };
        //                RetActs.Add(ToAdd);
        //        }
        //    }
        //    return RetActs;
        //}
        public void LeavingEditor(GameScenes gScn)
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                //print("LEaving editor");
                EditorSaveToFile();
            }
        }

        public void OnDisable()
        {


            SaveCurrentKeyBindings();
            SaveWindowPositions();
            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
            {
                AGXBtn.Destroy();
            }
            else
            {
                ApplicationLauncher.Instance.RemoveModApplication(AGXAppEditorButton);
            }
            //EditorSaveToFile(); //some of my data has already been deleted by this point
            GameEvents.onPartAttach.Remove(PartAttaching);
            GameEvents.onPartRemove.Remove(PartRemove);
            GameEvents.onEditorShipModified.Remove(VesselChanged);
            //GameEvents.onGameSceneLoadRequested.Remove(LeavingEditor);


            EditorPanels.Instance.actions.RemoveValueChangedDelegate(OnUIChanged); //detect when EditorPanel moves. this ONLY detects editor panel, going from parts to crew will NOT trigger this
            EditorLogic.fetch.crewPanelBtn.RemoveValueChangedDelegate(OnOtherButtonClick); //detect when Part button clicked at top of screen
            EditorLogic.fetch.partPanelBtn.RemoveValueChangedDelegate(OnOtherButtonClick); //detect when Crew button clicked at top of screen
            EditorLogic.fetch.loadBtn.RemoveValueChangedDelegate(OnLoadButtonClick); //load button clicked to check for deleted ships
            EditorLogic.fetch.saveBtn.RemoveValueChangedDelegate(OnSaveButtonClick); //run save when save button clicked. auto-save from Scenario module only runs on leaving editor! not on clicking save button
            EditorLogic.fetch.launchBtn.RemoveValueChangedDelegate(OnSaveButtonClick);
            EditorLogic.fetch.exitBtn.RemoveValueChangedDelegate(OnSaveButtonClick);
            EditorLogic.fetch.newBtn.RemoveValueChangedDelegate(OnSaveButtonClick);
        }

        public static void SaveWindowPositions()
        {


            AGExtNode.SetValue("EdGroupsX", GroupsWin.x.ToString());
            AGExtNode.SetValue("EdGroupsY", GroupsWin.y.ToString());
            AGExtNode.SetValue("EdSelPartsX", SelPartsWin.x.ToString());
            AGExtNode.SetValue("EdSelPartsY", SelPartsWin.y.ToString());
            AGExtNode.SetValue("EdKeyCodeX", KeyCodeWin.x.ToString());
            AGExtNode.SetValue("EdKeyCodeY", KeyCodeWin.y.ToString());
            AGExtNode.SetValue("EdKeySetX", KeySetWin.x.ToString());
            AGExtNode.SetValue("EdKeySetY", KeySetWin.y.ToString());
            AGExtNode.SetValue("EdCurActsX", CurActsWin.x.ToString());
            AGExtNode.SetValue("EdCurActsY", CurActsWin.y.ToString());
            //AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
            AGXStaticData.SaveBaseConfigNode(AGExtNode);


        }



        private static bool JoySticks(String s)
        {
            return s.StartsWith("Joystick");
        }

        public void AGXOnDraw()
        {
            //print("start ondraw draw");
            Vector3 RealMousePos = new Vector3();
            RealMousePos = Input.mousePosition;
            RealMousePos.y = Screen.height - Input.mousePosition.y;



            //if (!ToolbarManager.ToolbarAvailable) //stock toolbar now added
            //{
            //    AGXShow = true; //no toolbar so show AGX with KSP actions editor still up
            //}



            if (AGXShow)
            {
                // print("Test call 3" + showCareerCustomAGs);
                if (!showCareerCustomAGs)
                {
                    //print("Test call");
                    defaultShowingNonNumeric = true;
                }
                if (ShowKeySetWin)
                {
                    KeySetWin = GUI.Window(673467792, KeySetWin, KeySetWindow, "Keysets", AGXWinStyle);
                    TrapMouse = KeySetWin.Contains(RealMousePos);
                    if (!AutoHideGroupsWin)
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", AGXWinStyle);
                    }
                    ShowCurActsWin = false;

                }

                if (ShowSelectedWin)
                {
                    //print("start selparts draw");
                    SelPartsWin = GUI.Window(673467794, SelPartsWin, SelParts, "AGExt Selected parts: " + AGEditorSelectedParts.Count(), AGXWinStyle);
                    // print("end selparts draw");
                    ShowCurActsWin = true;
                    TrapMouse = SelPartsWin.Contains(RealMousePos);
                    if (AutoHideGroupsWin && !TempShowGroupsWin)
                    {
                    }
                    else
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", AGXWinStyle);
                        TrapMouse |= GroupsWin.Contains(RealMousePos);
                    }

                    if (ShowKeyCodeWin)
                    {
                        KeyCodeWin = GUI.Window(673467793, KeyCodeWin, KeyCodeWindow, "                Keycodes", AGXWinStyle);
                        TrapMouse |= KeyCodeWin.Contains(RealMousePos);
                    }

                }
                if (ShowCurActsWin && ShowSelectedWin)
                {
                    CurActsWin = GUI.Window(673467790, CurActsWin, CurrentActionsWindow, "Actions (This group): " + CurrentVesselActions.FindAll(p => p.group == AGXCurActGroup).Count.ToString(), AGXWinStyle);
                    TrapMouse |= CurActsWin.Contains(RealMousePos);
                }
                string ErrLine = "1";
                try
                {
                    ErrLine = "2";

                    ErrLine = "7";
                    if (ShowSelectedWin)
                    {

                        if (defaultShowingNonNumeric)
                        {
                            foreach (BaseAction Act in defaultActionsListThisType)
                            {
                                ErrLine = "8";
                                Vector3 partScreenPosD = EditorLogic.fetch.editorCamera.WorldToScreenPoint(Act.listParent.part.transform.position);
                                ErrLine = "9";
                                Rect partCenterWinD = new Rect(partScreenPosD.x - 10, (Screen.height - partScreenPosD.y) - 10, 21, 21);
                                ErrLine = "10";
                                GUI.DrawTexture(partCenterWinD, PartPlus);
                            }
                        }
                        else
                        {
                            foreach (AGXAction agAct in ThisGroupActions)
                            {
                                ErrLine = "8";
                                Vector3 partScreenPosC = EditorLogic.fetch.editorCamera.WorldToScreenPoint(agAct.ba.listParent.part.transform.position);
                                ErrLine = "9";
                                Rect partCenterWinC = new Rect(partScreenPosC.x - 10, (Screen.height - partScreenPosC.y) - 10, 21, 21);
                                ErrLine = "10";
                                GUI.DrawTexture(partCenterWinC, PartPlus);
                            }
                        }
                        foreach (AGXPart agPrt in AGEditorSelectedParts)
                        {
                            ErrLine = "3";
                            Vector3 partScreenPosB = EditorLogic.fetch.editorCamera.WorldToScreenPoint(agPrt.AGPart.transform.position);
                            ErrLine = "4";
                            Rect partCenterWinB = new Rect(partScreenPosB.x - 10, (Screen.height - partScreenPosB.y) - 10, 21, 21);
                            ErrLine = "5";
                            GUI.DrawTexture(partCenterWinB, PartCross);
                            ErrLine = "6";
                        }
                    }
                    ErrLine = "11";
                    if (showAGXRightClickMenu)
                    {
                        Rect SettingsWinEditor = new Rect(Screen.width - 200, Screen.height - 125, 150, 85);
                        GUI.Window(2233452, SettingsWinEditor, DrawSettingsWinEditor, "AGX Settings", AGXEditor.AGXWinStyle);
                    }
                }
                catch (Exception e)
                {
                    print("AGX Draw cross fail. " + ErrLine + " " + e);
                }
            }
            // print("Truth check " + highlightPartThisFrameActsWin + " " + highlightPartThisFrameSelWin);
            if (highlightPartThisFrameActsWin || highlightPartThisFrameSelWin || highlightPartThisFrameGroupWin)//highlight mouse over cross
            {
                //Camera edCam = EditorCamera.fe
                // print("mouse over");
                //print("screen pos " + EditorLogic.fetch.editorCamera.WorldToScreenPoint(partToHighlight.transform.position));
                //print("orgpos" + partToHighlight.);
                Vector3 partScreenPos = EditorLogic.fetch.editorCamera.WorldToScreenPoint(partToHighlight.transform.position);
                Rect partCenterWin = new Rect(partScreenPos.x - 20, (Screen.height - partScreenPos.y) - 20, 41, 41);
                // partCenterWin = GUI.Window(673767790, partCenterWin, PartTarget, "", AGXWinStyle);
                GUI.DrawTexture(partCenterWin, PartCenter);


            }
            //print("end ondraw draw");
        }

        //public void PartTarget(int WindowID)
        //{
        //    GUI.DrawTexture(new Rect(0, 0, 41, 41), PartCenter);
        //}

        public void CurrentActionsWindow(int WindowID)
        {
            HighLogic.Skin.scrollView.normal.background = null;
            ThisGroupActions = new List<AGXAction>();
            ThisGroupActions.AddRange(CurrentVesselActions.Where(p => p.group == AGXCurActGroup));
            GUI.Box(new Rect(5, 25, 310, 110), "");
            CurGroupsWin = GUI.BeginScrollView(new Rect(10, 30, 330, 100), CurGroupsWin, new Rect(0, 0, 310, Math.Max(100, 0 + (20 * (ThisGroupActions.Count)))));
            int RowCnt = new int();
            RowCnt = 1;
            highlightPartThisFrameActsWin = false;
            if (ThisGroupActions.Count > 0)
            {

                while (RowCnt <= ThisGroupActions.Count)
                {

                    if (Mouse.screenPos.y >= CurActsWin.y + 30 && Mouse.screenPos.y <= CurActsWin.y + 130 && new Rect(CurActsWin.x + 10, (CurActsWin.y + 30 + ((RowCnt - 1) * 20)) - CurGroupsWin.y, 300, 20).Contains(Mouse.screenPos))
                    {
                        highlightPartThisFrameActsWin = true;
                        //print("part found to highlight acts " + highlightPartThisFrameActsWin + " " + ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.transform.position);
                        partToHighlight = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part;
                    }


                    TextAnchor TxtAnch4 = new TextAnchor();
                    TxtAnch4 = GUI.skin.button.alignment;
                    // GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
                    if (GUI.Button(new Rect(0, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).group.ToString() + ": " + AGXguiNames[ThisGroupActions.ElementAt(RowCnt - 1).group], AGXBtnStyle))
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
                        //SaveCurrentVesselActions();
                        if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                        {
                            RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                        }
                        ModuleAGX agxMod = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                        agxMod.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));

                    }

                    if (GUI.Button(new Rect(100, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).prt.partInfo.title, AGXBtnStyle))
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
                        //SaveCurrentVesselActions();
                        if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                        {
                            RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                        }

                        ModuleAGX agxMod2 = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                        agxMod2.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                    }
                    try
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).ba.guiName, AGXBtnStyle))
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
                            //SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                            ModuleAGX agxMod3 = ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.Modules.OfType<ModuleAGX>().First();
                            agxMod3.agxActionsThisPart.RemoveAll(p => p == ThisGroupActions.ElementAt(RowCnt - 1));
                        }
                    }
                    catch
                    {
                        if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), "error", AGXBtnStyle))
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
                            //SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                        }
                    }

                    AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                    //GUI.skin.button.alignment = TxtAnch4;
                    RowCnt = RowCnt + 1;
                }


            }

            else
            {
                TextAnchor TxtAnch5 = new TextAnchor();

                TxtAnch5 = GUI.skin.label.alignment;

                //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                AGXLblStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(10, 30, 274, 30), "No actions", AGXLblStyle);
                //GUI.skin.label.alignment = TxtAnch5;
                AGXLblStyle.alignment = TextAnchor.MiddleLeft;
            }
            GUI.EndScrollView();

            GUI.DragWindow();
        }

        public void KeySetWindow(int WindowID)
        {



            //GUI.DrawTexture(new Rect(6, (CurrentKeySet * 25) +1, 68, 18), BtnTexGrn,if();

            AGXBtnStyle.normal.background = CurrentKeySet == 1 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 25, 70, 20), "Select 1:", AGXBtnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();

                CurrentKeySet = 1;
                CurrentKeySetName = KeySetNames[0];
                LoadCurrentKeyBindings();
            }
            KeySetNames[0] = GUI.TextField(new Rect(80, 25, 100, 20), KeySetNames[0], AGXFldStyle);
            AGXBtnStyle.normal.background = CurrentKeySet == 2 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 50, 70, 20), "Select 2:", AGXBtnStyle))
            {

                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 2;
                CurrentKeySetName = KeySetNames[1];
                LoadCurrentKeyBindings();
            }
            KeySetNames[1] = GUI.TextField(new Rect(80, 50, 100, 20), KeySetNames[1], AGXFldStyle);
            AGXBtnStyle.normal.background = CurrentKeySet == 3 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 75, 70, 20), "Select 3:", AGXBtnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 3;
                CurrentKeySetName = KeySetNames[2];
                LoadCurrentKeyBindings();
            }
            KeySetNames[2] = GUI.TextField(new Rect(80, 75, 100, 20), KeySetNames[2], AGXFldStyle);
            AGXBtnStyle.normal.background = CurrentKeySet == 4 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 100, 70, 20), "Select 4:", AGXBtnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 4;
                CurrentKeySetName = KeySetNames[3];
                LoadCurrentKeyBindings();
            }
            KeySetNames[3] = GUI.TextField(new Rect(80, 100, 100, 20), KeySetNames[3], AGXFldStyle);
            AGXBtnStyle.normal.background = CurrentKeySet == 5 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 125, 70, 20), "Select 5:", AGXBtnStyle))
            {
                EditorSaveKeysetStuff();
                SaveCurrentKeyBindings();
                CurrentKeySet = 5;
                CurrentKeySetName = KeySetNames[4];
                LoadCurrentKeyBindings();
            }
            KeySetNames[4] = GUI.TextField(new Rect(80, 125, 100, 20), KeySetNames[4], AGXFldStyle);
            AGXBtnStyle.normal.background = ButtonTexture;

            Color TxtClr3 = GUI.contentColor;
            GUI.contentColor = new Color(0.5f, 1f, 0f, 1f);
            //GUI.skin.label.fontStyle = FontStyle.Bold;
            AGXLblStyle.fontStyle = FontStyle.Bold;
            TextAnchor TxtAnc = GUI.skin.label.alignment;
            //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            AGXLblStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(5, 145, 175, 25), "Actiongroup Groups", AGXLblStyle);
            //GUI.skin.label.fontStyle = FontStyle.Normal;
            //GUI.skin.label.alignment = TxtAnc;
            AGXLblStyle.alignment = TextAnchor.MiddleLeft;
            AGXLblStyle.fontStyle = FontStyle.Normal;
            GUI.contentColor = TxtClr3;

            //GUI.DrawTexture(new Rect(6, (ShowGroupInFlightCurrent * 25) + 141, 68, 18), BtnTexGrn);
            AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 1 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 165, 70, 20), "Group 1:", AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 1;
            }
            ShowGroupInFlightNames[1] = GUI.TextField(new Rect(80, 165, 100, 20), ShowGroupInFlightNames[1]);
            AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 2 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 190, 70, 20), "Group 2:", AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 2;
            }
            ShowGroupInFlightNames[2] = GUI.TextField(new Rect(80, 190, 100, 20), ShowGroupInFlightNames[2]);
            AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 3 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 215, 70, 20), "Group 3:", AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 3;
            }
            ShowGroupInFlightNames[3] = GUI.TextField(new Rect(80, 215, 100, 20), ShowGroupInFlightNames[3]);
            AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 4 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 240, 70, 20), "Group 4:", AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 4;
            }
            ShowGroupInFlightNames[4] = GUI.TextField(new Rect(80, 240, 100, 20), ShowGroupInFlightNames[4]);
            AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 5 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 265, 70, 20), "Group 5:", AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 5;

            }
            ShowGroupInFlightNames[5] = GUI.TextField(new Rect(80, 265, 100, 20), ShowGroupInFlightNames[5]);
            AGXBtnStyle.normal.background = ButtonTexture;
            if (GUI.Button(new Rect(5, 300, 175, 30), "Close Window", AGXBtnStyle))
            {

                EditorSaveKeysetStuff();
                //superceeded by v2 save
                //foreach (Part p in EditorLogic.fetch.getSortedShipList())
                //{
                //    foreach (ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                //    {
                //        pm.AGXGroupStates = SaveGroupVisibility(EditorLogic.RootPart, pm.AGXGroupStates); /ver2 done
                //        pm.AGXGroupStateNames = SaveGroupVisibilityNames(EditorLogic.RootPart, pm.AGXGroupStates); /ver2 done
                //    }
                //}
                ShowKeySetWin = false;
            }

            GUI.DragWindow();
        }

        //public static string SaveCurrentKeySet(Part p, String CurKey)
        //{

        //    if (LoadFinished)
        //        return CurrentKeySet.ToString();
        //    else
        //    {
        //        return CurKey;
        //    }


        //        }


        //public void LoadCurrentKeySet()
        //{


        //    string errLine = "1";
        //    try
        //    {
        //        errLine = "2";
        //        //ConfigNode AGXBaseNode = AGextScenario.LoadBaseNode();
        //        errLine = "3";
        //        ConfigNode AGXEditorNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
        //        errLine = "4";

        //        errLine = "5";
        //        string hashedShipName = AGextScenario.EditorHashShipName(EditorLogic.fetch.shipNameField.Text, inVAB);
        //        errLine = "6";

        //        if (AGXEditorNode.CountNodes >= 1)
        //        {
        //            errLine = "7";
        //            if(AGXEditorNode.nodes.Contains(hashedShipName))
        //        {
        //            errLine = "8";
        //            ConfigNode vslNode = AGXEditorNode.GetNode(hashedShipName);
        //            errLine = "9";
        //            CurrentKeySet = Convert.ToInt32(vslNode.GetValue("AGXKeySet"));
        //            errLine = "10";
        //            if (CurrentKeySet >= 1 && CurrentKeySet <= 5)
        //            {
        //            }
        //            else
        //            {
        //                CurrentKeySet = 1;
        //            }
        //            errLine = "11";
        //        }

        //        //else if (EditorLogic.RootPart.Modules.Contains("ModuleAGExtData")) //v2 done
        //        //{
        //        //    errLine = "12";
        //        //    bool ShipListOk3 = new bool();
        //        //    ShipListOk3 = false;
        //        //    try
        //        //    {


        //        //        if (EditorLogic.SortedShipList.Count >= 1)
        //        //        {
        //        //            foreach (Part p in EditorLogic.SortedShipList)
        //        //            {
        //        //            }
        //        //            ShipListOk3 = true;
        //        //        }
        //        //    }
        //        //    catch
        //        //    {

        //        //        ShipListOk3 = false;
        //        //        CurrentKeySet = 1;
        //        //    }

        //        //    if (ShipListOk3)
        //        //    {
        //        //        foreach (PartModule pm in EditorLogic.RootPart.Modules.OfType<ModuleAGExtData>()) //ver2 okay
        //        //        {
        //        //            CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));

        //        //        }

        //        //    }

        //        //    if (CurrentKeySet >= 1 && CurrentKeySet <= 5)
        //        //    {
        //        //    }
        //        //    else
        //        //    {
        //        //        CurrentKeySet = 1;
        //        //    }

        //        //    //if (ShipListOk3)
        //        //    //{
        //        //    //    foreach (Part p in EditorLogic.SortedShipList)
        //        //    //    {
        //        //    //        foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>()) //not needed v2
        //        //    //        {
        //        //    //            agpm.partCurrentKeySet = CurrentKeySet;
        //        //    //        }
        //        //    //    }
        //        //    //}
        //        //}
        //    }
        //        errLine = "13";
        //       // CurrentKeySetName = AGExtNode.GetValue("KeySetName" + CurrentKeySet);
        //    }
        //    catch (Exception e)
        //    {
        //        print("AGXEd LoadCurrentKeySet Fail " + errLine+ " "+e);
        //    }
        //}




        public static void LoadCurrentKeyBindings()
        {

            //print("loading key set " + CurrentKeySet.ToString());          
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
            String GroupKeysMod1ToLoad = AGExtNode.GetValue("KeySetMod1Group" + CurrentKeySet.ToString());
            String GroupKeysMod2ToLoad = AGExtNode.GetValue("KeySetMod2Group" + CurrentKeySet.ToString());
            for (int i = 0; i <= 249; i++)
            {
                if (GroupKeysMod1ToLoad[i] == '1')
                {
                    AGXguiMod1Groups[i + 1] = true;
                }
                else
                {
                    AGXguiMod1Groups[i + 1] = false;
                }
                if (GroupKeysMod2ToLoad[i] == '1')
                {
                    AGXguiMod2Groups[i + 1] = true;
                }
                else
                {
                    AGXguiMod2Groups[i + 1] = false;
                }
            }
            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), AGExtNode.GetValue("KeySetModKey1" + CurrentKeySet.ToString()));
            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), AGExtNode.GetValue("KeySetModKey2" + CurrentKeySet.ToString()));
        }

        public static void SaveCurrentKeyBindings()
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


            string GroupsMod1ToSave = "";
            string GroupsMod2ToSave = "";
            for (int i = 1; i <= 250; i++)
            {
                if (AGXguiMod1Groups[i])
                {
                    GroupsMod1ToSave = GroupsMod1ToSave + "1";
                }
                else
                {
                    GroupsMod1ToSave = GroupsMod1ToSave + "0";
                }
                if (AGXguiMod2Groups[i])
                {
                    GroupsMod2ToSave = GroupsMod2ToSave + "1";
                }
                else
                {
                    GroupsMod2ToSave = GroupsMod2ToSave + "0";
                }
            }
            AGExtNode.SetValue("KeySetMod1Group" + CurrentKeySet.ToString(), GroupsMod1ToSave);
            AGExtNode.SetValue("KeySetMod2Group" + CurrentKeySet.ToString(), GroupsMod2ToSave);
            AGExtNode.SetValue("KeySetModKey1" + CurrentKeySet.ToString(), AGXguiMod1Key.ToString());
            AGExtNode.SetValue("KeySetModKey2" + CurrentKeySet.ToString(), AGXguiMod2Key.ToString());



            //AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
            //if (CurrentKeySet == 1)
            //{
            //    SaveDefaultCustomKeys();
            //}


        }
        //public static void SaveDefaultCustomKeys()
        //{
        //    GameSettings.CustomActionGroup1.primary = AGXguiKeys[1]; //copy keys to KSP itself
        //    GameSettings.CustomActionGroup2.primary = AGXguiKeys[2];
        //    GameSettings.CustomActionGroup3.primary = AGXguiKeys[3];
        //    GameSettings.CustomActionGroup4.primary = AGXguiKeys[4];
        //    GameSettings.CustomActionGroup5.primary = AGXguiKeys[5];
        //    GameSettings.CustomActionGroup6.primary = AGXguiKeys[6];
        //    GameSettings.CustomActionGroup7.primary = AGXguiKeys[7];
        //    GameSettings.CustomActionGroup8.primary = AGXguiKeys[8];
        //    GameSettings.CustomActionGroup9.primary = AGXguiKeys[9];
        //    GameSettings.CustomActionGroup10.primary = AGXguiKeys[10];
        //    GameSettings.SaveSettings(); //save keys to disk
        //    GameSettings.CustomActionGroup1.primary = KeyCode.None; //unbind keys so they don't conflict
        //    GameSettings.CustomActionGroup2.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup3.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup4.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup5.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup6.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup7.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup8.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup9.primary = KeyCode.None;
        //    GameSettings.CustomActionGroup10.primary = KeyCode.None;
        //}
        public void KeyCodeWindow(int WindowID)
        {
            if (GUI.Button(new Rect(5, 3, 80, 25), "Clear Key", AGXBtnStyle))
            {
                if (AGXguiMod1KeySelecting)
                {
                    AGXguiMod1Key = KeyCode.None;
                    AGXguiMod1KeySelecting = false;
                }
                else if (AGXguiMod2KeySelecting)
                {
                    AGXguiMod2Key = KeyCode.None;
                    AGXguiMod2KeySelecting = false;
                }
                else
                {
                    AGXguiKeys[AGXCurActGroup] = KeyCode.None;
                    ShowKeyCodeWin = false;
                }
            }

            if (AGXguiMod1KeySelecting)
            {
                AGXBtnStyle.normal.background = ButtonTextureRed;
                AGXBtnStyle.hover.background = ButtonTextureRed;
            }
            else if (AGXguiMod1Groups[AGXCurActGroup] == true)
            {
                AGXBtnStyle.normal.background = ButtonTextureGreen;
                AGXBtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
            }
            if (GUI.Button(new Rect(80, 3, 60, 25), AGXguiMod1Key.ToString(), AGXBtnStyle))
            {
                if (Event.current.button == 0)
                {
                    AGXguiMod1Groups[AGXCurActGroup] = !AGXguiMod1Groups[AGXCurActGroup];
                }
                if (Event.current.button == 1)
                {
                    AGXguiMod1KeySelecting = true;
                    AGXguiMod2KeySelecting = false;
                }
            }
            if (AGXguiMod2KeySelecting)
            {
                AGXBtnStyle.normal.background = ButtonTextureRed;
                AGXBtnStyle.hover.background = ButtonTextureRed;
            }
            else if (AGXguiMod2Groups[AGXCurActGroup] == true)
            {
                AGXBtnStyle.normal.background = ButtonTextureGreen;
                AGXBtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
            }
            if (GUI.Button(new Rect(140, 3, 60, 25), AGXguiMod2Key.ToString(), AGXBtnStyle))
            {
                if (Event.current.button == 0)
                {
                    AGXguiMod2Groups[AGXCurActGroup] = !AGXguiMod2Groups[AGXCurActGroup];
                }
                if (Event.current.button == 1)
                {
                    AGXguiMod2KeySelecting = true;
                    AGXguiMod1KeySelecting = false;
                }
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;
            //if (ShowJoySticks)
            //{
            //    GUI.DrawTexture(new Rect(281, 3, 123, 18), BtnTexGrn);
            //}
            AGXBtnStyle.normal.background = ShowJoySticks ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(280, 2, 125, 20), "Show JoySticks", AGXBtnStyle))
            {
                ShowJoySticks = !ShowJoySticks;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            if (!ShowJoySticks)
            {
                int KeyListCount = new int();
                KeyListCount = 0;
                while (KeyListCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (KeyListCount * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                        }

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 69)
                {
                    if (GUI.Button(new Rect(105, 25 + ((KeyListCount - 35) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                        }

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 104)
                {
                    if (GUI.Button(new Rect(205, 25 + ((KeyListCount - 70) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                        }

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 139)
                {
                    if (GUI.Button(new Rect(305, 25 + ((KeyListCount - 105) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                            ShowKeyCodeWin = false;
                        }

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
                    if (GUI.Button(new Rect(5, 25 + (JoyStickCount * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            ShowKeyCodeWin = false;
                        }

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 69)
                {
                    if (GUI.Button(new Rect(130, 25 + ((JoyStickCount - 35) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            ShowKeyCodeWin = false;
                        }

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 99)
                {
                    if (GUI.Button(new Rect(255, 25 + ((JoyStickCount - 70) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        if (AGXguiMod1KeySelecting)
                        {
                            AGXguiMod1Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod1KeySelecting = false;
                        }
                        else if (AGXguiMod2KeySelecting)
                        {
                            AGXguiMod2Key = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            AGXguiMod2KeySelecting = false;
                        }
                        else
                        {
                            AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                            ShowKeyCodeWin = false;
                        }

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                GUI.Label(new Rect(260, 665, 120, 20), "Button test:", AGXLblStyle);
                if (Event.current.keyCode != KeyCode.None)
                {
                    LastKeyCode = Event.current.keyCode.ToString();
                }
                GUI.TextField(new Rect(260, 685, 130, 20), LastKeyCode, AGXFldStyle);
            }
            GUI.DragWindow();
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

        public void SelParts(int WindowID)
        {
            AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
            HighLogic.Skin.scrollView.normal.background = null;

            SelectedPartsCount = AGEditorSelectedParts.Count;
            int SelPartsLeft = new int();
            SelPartsLeft = -10;


            //GUI.DrawTexture(new Rect(25, 30, 80, PartsScrollHeight), TexBlk, ScaleMode.StretchToFill, false);
            //AGXPart FirstPart = new AGXPart();
            // FirstPart = AGEditorSelectedParts.First().AGPart.name;
            GUI.Box(new Rect(SelPartsLeft + 20, 25, 200, 110), "");
            highlightPartThisFrameSelWin = false;
            if (AGEditorSelectedParts != null && AGEditorSelectedParts.Count > 0)
            {

                int ButtonCount = new int();
                ButtonCount = 1;

                ScrollPosSelParts = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 30, 220, 110), ScrollPosSelParts, new Rect(0, 0, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10));

                //GUI.Box(new Rect(SelPartsLeft, 25, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10), "");
                while (ButtonCount <= SelectedPartsCount)
                {
                    //Rect buttonRect = new Rect(5, 0 + ((ButtonCount - 1) * 20), 190, 20); //need this rectangle twice
                    if (Mouse.screenPos.y >= SelPartsWin.y + 30 && Mouse.screenPos.y <= SelPartsWin.y + 140 && new Rect(SelPartsWin.x + SelPartsLeft + 25, (SelPartsWin.y + 30 + ((ButtonCount - 1) * 20)) - ScrollPosSelParts.y, 190, 20).Contains(Mouse.screenPos))
                    {
                        highlightPartThisFrameSelWin = true;
                        //print("part found to highlight " + AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.ConstructID);
                        partToHighlight = AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart;
                    }



                    if (GUI.Button(new Rect(5, 0 + ((ButtonCount - 1) * 20), 190, 20), AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.partInfo.title, AGXBtnStyle))
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
            else //no parts selected, show list all parts button?
            {
                if (GUI.Button(new Rect(SelPartsLeft + 50, 45, 140, 70), "Show list of\nall parts?", AGXBtnStyle)) //button itself
                {
                    showAllPartsListTitles = new List<string>(); //generate list of all parts 
                    showAllPartsListTitles.Clear(); //this probably isn't needed, but it works as is, not messing with it
                    foreach (Part p in EditorLogic.SortedShipList) //cycle all parts
                    {
                        List<BaseAction> actCheck = new List<BaseAction>(); //start check to see if p has any actions
                        actCheck.AddRange(p.Actions); //add actions on part
                        foreach (PartModule pm in p.Modules) //add actions from each partModule on part
                        {
                            actCheck.AddRange(pm.Actions);
                        }
                        if (actCheck.Count > 0) //only add part to showAllPartsListTitles if part has actions on it
                        {
                            if (!showAllPartsListTitles.Contains(p.partInfo.title))
                            {
                                showAllPartsListTitles.Add(p.partInfo.title);
                            }
                        }
                    }
                    showAllPartsListTitles.Sort(); //sort alphabetically
                    //ScrollGroups = Vector2.zero;
                    showAllPartsList = true; //change groups win to all parts list
                    TempShowGroupsWin = true; // if autohide enabled, show group win
                }
            }

            if (SelPartsIncSym)
            {
                //GUI.DrawTexture(new Rect(SelPartsLeft + 246, 26, 108, 23), BtnTexGrn, ScaleMode.StretchToFill, false);
                AGXBtnStyle.normal.background = ButtonTextureGreen;
                BtnTxt = "Symmetry? Yes";
            }
            else
            {
                //GUI.DrawTexture(new Rect(SelPartsLeft + 246, 26, 108, 23), BtnTexRed, ScaleMode.StretchToFill, false);
                AGXBtnStyle.normal.background = ButtonTextureRed;
                BtnTxt = "Symmetry? No";
            }



            if (GUI.Button(new Rect(SelPartsLeft + 245, 25, 110, 25), BtnTxt, AGXBtnStyle))
            {
                SelPartsIncSym = !SelPartsIncSym;

            }
            AGXBtnStyle.normal.background = ButtonTexture;
            if (GUI.Button(new Rect(SelPartsLeft + 245, 55, 110, 25), "Clear List", AGXBtnStyle))
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

                        if (GUI.Button(new Rect(5, 0 + ((ActionsCount - 1) * 20), 190, 20), PartActionsList.ElementAt(ActionsCount - 1).guiName, AGXBtnStyle))
                        {
                            if (defaultShowingNonNumeric)
                            {
                                string baname = PartActionsList.ElementAt(ActionsCount - 1).name;
                                string moduleName = PartActionsList.ElementAt(ActionsCount - 1).listParent.module.name;
                                foreach (AGXPart agP in AGEditorSelectedParts)
                                {
                                    List<BaseAction> actsToCheck = new List<BaseAction>();
                                    if (moduleName.Length > 0)
                                    {
                                        foreach (PartModule pm in agP.AGPart.Modules)
                                        {
                                            if (pm.name == moduleName)
                                            {
                                                actsToCheck.AddRange(pm.Actions);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        actsToCheck.AddRange(agP.AGPart.Actions);
                                    }
                                    foreach (BaseAction ba in actsToCheck)
                                    {
                                        if (ba.name == baname)
                                        {
                                            ba.actionGroup = ba.actionGroup | defaultGroupToShow;
                                            Debug.Log("adding act");

                                        }
                                    }

                                }
                                RefreshDefaultActionsListType();
                            }
                            else
                            {
                                int PrtCnt = new int();
                                PrtCnt = 0;
                                foreach (AGXPart agPrt in AGEditorSelectedParts)
                                {
                                    List<BaseAction> ThisPartActionsList = new List<BaseAction>();
                                    ThisPartActionsList.AddRange(agPrt.AGPart.Actions.Where(a => a.active == true));
                                    foreach (PartModule pm3 in agPrt.AGPart.Modules)
                                    {
                                        ThisPartActionsList.AddRange(pm3.Actions.Where(a => a.active == true));
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
                                        //SaveCurrentVesselActions();
                                    }
                                    PrtCnt = PrtCnt + 1;
                                    if (ToAdd.group < 11)
                                    {
                                        SetDefaultAction(ToAdd.ba, ToAdd.group);
                                    }
                                }
                            }

                        }

                        ActionsCount = ActionsCount + 1;
                    }
                    GUI.EndScrollView();
                }
                else
                {
                    if (AGEditorSelectedParts.Count >= 1)
                    {
                        if (GUI.Button(new Rect(SelPartsLeft + 30, 190, 185, 40), "No actions found.\r\nRefresh?", AGXBtnStyle))
                        {
                            PartActionsList.Clear();
                            PartActionsList.AddRange(AGEditorSelectedParts.First().AGPart.Actions.Where(ba => ba.active == true));
                            foreach (PartModule pm in AGEditorSelectedParts.First().AGPart.Modules)
                            {
                                PartActionsList.AddRange(pm.Actions.Where(ba => ba.active == true));
                            }
                            print("AGX Actions refresh found actions: " + PartActionsList.Count);
                        }
                    }
                }

                // 
            }
            else
            {
                TextAnchor TxtAnch = new TextAnchor();

                TxtAnch = GUI.skin.label.alignment;

                //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                AGXLblStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(SelPartsLeft + 20, 180, 190, 40), "Select parts of\nthe same type", AGXLblStyle);


                AGXLblStyle.alignment = TextAnchor.MiddleLeft;
                //GUI.skin.label.alignment = TxtAnch;

            }


            TextAnchor TxtAnch2 = new TextAnchor();
            TxtAnch2 = GUI.skin.button.alignment;
            //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            AGXBtnStyle.alignment = TextAnchor.MiddleLeft;

            if (defaultShowingNonNumeric)
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), defaultGroupToShow.ToString(), AGXBtnStyle)) //current action group button
                {
                    //TempShowGroupsWin = true;
                }
            }
            else
            {

                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), AGXCurActGroup + ": " + AGXguiNames[AGXCurActGroup], AGXBtnStyle))
                {
                    TempShowGroupsWin = true;
                }
                //GUI.skin.button.alignment = TxtAnch2;
                AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                if (IsGroupToggle[AGXCurActGroup])
                {

                    Color TxtClr = GUI.contentColor;
                    GUI.contentColor = Color.green;
                    if (GUI.Button(new Rect(SelPartsLeft + 235, 155, 80, 22), "Toggle:Yes", AGXBtnStyle))
                    {

                        IsGroupToggle[AGXCurActGroup] = false;
                    }
                    GUI.contentColor = TxtClr;
                }
                else
                {
                    if (GUI.Button(new Rect(SelPartsLeft + 235, 155, 80, 22), "Toggle:No", AGXBtnStyle))
                    {

                        IsGroupToggle[AGXCurActGroup] = true;
                    }
                }

                if (isDirectAction[AGXCurActGroup])
                {
                    Color btnClr = AGXBtnStyle.normal.textColor;
                    AGXBtnStyle.normal.textColor = Color.red;
                    AGXBtnStyle.hover.textColor = Color.red;
                    if (GUI.Button(new Rect(SelPartsLeft + 315, 155, 55, 22), "Hold", AGXBtnStyle))
                    {
                        isDirectAction[AGXCurActGroup] = false;
                    }
                    AGXBtnStyle.normal.textColor = btnClr;
                    AGXBtnStyle.hover.textColor = btnClr;
                }
                else
                {
                    if (GUI.Button(new Rect(SelPartsLeft + 315, 155, 55, 22), "Tap", AGXBtnStyle))
                    {
                        isDirectAction[AGXCurActGroup] = true;
                    }
                }


                GUI.Label(new Rect(SelPartsLeft + 231, 183, 110, 22), "Show:", AGXLblStyle);
                Color TxtClr2 = GUI.contentColor;

                if (ShowGroupInFlight[1, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }
                if (GUI.Button(new Rect(SelPartsLeft + 271, 183, 20, 22), "1", AGXBtnStyle))
                {
                    ShowGroupInFlight[1, AGXCurActGroup] = !ShowGroupInFlight[1, AGXCurActGroup];
                }

                if (ShowGroupInFlight[2, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }
                if (GUI.Button(new Rect(SelPartsLeft + 291, 183, 20, 22), "2", AGXBtnStyle))
                {
                    ShowGroupInFlight[2, AGXCurActGroup] = !ShowGroupInFlight[2, AGXCurActGroup];
                    //CalculateActionsToShow();
                }

                if (ShowGroupInFlight[3, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }
                if (GUI.Button(new Rect(SelPartsLeft + 311, 183, 20, 22), "3", AGXBtnStyle))
                {
                    ShowGroupInFlight[3, AGXCurActGroup] = !ShowGroupInFlight[3, AGXCurActGroup];
                    //CalculateActionsToShow();
                }

                if (ShowGroupInFlight[4, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }
                if (GUI.Button(new Rect(SelPartsLeft + 331, 183, 20, 22), "4", AGXBtnStyle))
                {
                    ShowGroupInFlight[4, AGXCurActGroup] = !ShowGroupInFlight[4, AGXCurActGroup];
                    // CalculateActionsToShow();
                }

                if (ShowGroupInFlight[5, AGXCurActGroup])
                {
                    GUI.contentColor = Color.green;
                }
                else
                {
                    GUI.contentColor = Color.red;
                }
                if (GUI.Button(new Rect(SelPartsLeft + 351, 183, 20, 22), "5", AGXBtnStyle))
                {
                    ShowGroupInFlight[5, AGXCurActGroup] = !ShowGroupInFlight[5, AGXCurActGroup];
                    // CalculateActionsToShow();
                }
                GUI.contentColor = TxtClr2;
                GUI.Label(new Rect(SelPartsLeft + 245, 115, 110, 20), "Description:", AGXLblStyle);
                CurGroupDesc = AGXguiNames[AGXCurActGroup];
                CurGroupDesc = GUI.TextField(new Rect(SelPartsLeft + 245, 135, 120, 22), CurGroupDesc, AGXFldStyle);
                AGXguiNames[AGXCurActGroup] = CurGroupDesc;
                GUI.Label(new Rect(SelPartsLeft + 245, 203, 110, 25), "Keybinding:", AGXLblStyle);
                string btnName = "";
                if (AGXguiMod1Groups[AGXCurActGroup] && AGXguiMod2Groups[AGXCurActGroup])
                {
                    btnName = '\u00bd' + AGXguiKeys[AGXCurActGroup].ToString();
                }
                else if (AGXguiMod1Groups[AGXCurActGroup])
                {
                    btnName = '\u2474' + AGXguiKeys[AGXCurActGroup].ToString();
                }
                else if (AGXguiMod2Groups[AGXCurActGroup])
                {
                    btnName = '\u2475' + AGXguiKeys[AGXCurActGroup].ToString();
                }
                else
                {
                    btnName = AGXguiKeys[AGXCurActGroup].ToString();
                }
                if (GUI.Button(new Rect(SelPartsLeft + 245, 222, 120, 20), btnName, AGXBtnStyle))
                {
                    ShowKeyCodeWin = true;
                }
            }
            if (GUI.Button(new Rect(SelPartsLeft + 245, 244, 120, 20), CurrentKeySetName, AGXBtnStyle))
            {
                //print("1a");
                //SaveCurrentKeyBindings();
                //print("2a");
                // KeySetNames[0] = AGExtNode.GetValue("KeySetName1");
                //print("3a");
                // KeySetNames[1] = AGExtNode.GetValue("KeySetName2");
                //KeySetNames[2] = AGExtNode.GetValue("KeySetName3");
                //KeySetNames[3] = AGExtNode.GetValue("KeySetName4");
                //KeySetNames[4] = AGExtNode.GetValue("KeySetName5");
                //print("4a");
                //print("cure key " + CurrentKeySet);
                //KeySetNames[CurrentKeySet - 1] = CurrentKeySetName;
                //print("5a");
                ShowKeySetWin = true;
                //print("6a");
            }


            GUI.DragWindow();
        }

        //public void SaveCurrentVesselActions() //no longer used in V2
        //{
        //    string errLine = "1";
        //    try
        //    {
        //        errLine = "2";
        //        foreach (Part p in EditorLogic.SortedShipList)
        //        {
        //            errLine = "3";
        //            foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
        //            {
        //                errLine = "4";
        //                agpm.partAGActions.Clear();
        //                errLine = "5";
        //                agpm.partAGActions.AddRange(CurrentVesselActions.Where(agp => agp.prt == p));
        //                errLine = "6";
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        print("AGX Editor Fail (SaveCurrentVesselActions) " + errLine + " " + e);
        //    }
        //}


        public static void LoadDirectActionState(string DirectActions)
        {
            try
            {
                isDirectAction = new Dictionary<int, bool>();
                if (DirectActions.Length == 250)
                {
                    for (int i = 1; i <= 250; i++)
                    {
                        if (DirectActions[0] == '1')
                        {
                            isDirectAction[i] = true;
                        }
                        else
                        {
                            isDirectAction[i] = false;
                        }
                        DirectActions = DirectActions.Substring(1);
                    }
                }
                else
                {
                    for (int i = 1; i <= 250; i++)
                    {
                        isDirectAction[i] = false;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("AGX LoadDirectActions Fail " + e);
                for (int i = 1; i <= 251; i++)
                {
                    isDirectAction[i] = false;
                }
            }
        }

        public static string SaveDirectActionState(string str)
        {
            try
            {

                string ReturnStr = "";

                for (int i = 1; i <= 250; i++)
                {
                    if (isDirectAction[i])
                    {
                        ReturnStr = ReturnStr + "1";
                    }
                    else
                    {
                        ReturnStr = ReturnStr + "0";
                    }

                }
                return ReturnStr;

            }
            catch
            {
                return str;
            }
        }

        public void RefreshDefaultActionsList()
        {
            string errLine = "1";
            try
            {
                defaultActionsListAll.Clear();
                errLine = "2";
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    errLine = "3";
                    defaultActionsListAll.AddRange(p.Actions);
                    errLine = "4";
                    foreach (PartModule pm in p.Modules)
                    {
                        errLine = "5";
                        defaultActionsListAll.AddRange(pm.Actions);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Log("AGX RefDefActsList Error " + errLine + " " + e);
            }

        }

        public void RefreshDefaultActionsListType()
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                defaultActionsListThisType.Clear();
                errLine = "3";
                foreach (BaseAction act in defaultActionsListAll)
                {
                    errLine = "4";
                    if ((act.actionGroup & defaultGroupToShow) == defaultGroupToShow)
                    {
                        errLine = "5";
                        defaultActionsListThisType.Add(act);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Log("AGX RefDefActsListType Error " + errLine + " " + e);
            }
        }


        public void GroupsWindow(int WindowID)
        {

            string ErrLine = "1";
            try
            {

                //if (AutoHideGroupsWin)
                //{
                //    GUI.DrawTexture(new Rect(6, 4, 78, 18), BtnTexRed);
                //}
                //AGXBtnStyle.normal.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                //AGXBtnStyle.onHover.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                //AGXBtnStyle.hover.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
                //if (GUI.Button(new Rect(15, 3, 70, 20), "Auto-Hide", AGXBtnStyle))
                //{
                //    AutoHideGroupsWin = !AutoHideGroupsWin;

                //}
                ErrLine = "2";
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
                bool[] PageGrn = new bool[5];
                ErrLine = "3";
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
                ErrLine = "4";

                if (showCareerCustomAGs)
                {
                    if (GUI.Button(new Rect(20, 3, 80, 20), "Other", AGXBtnStyle))
                    {
                        ErrLine = "4a";
                        defaultShowingNonNumeric = !defaultShowingNonNumeric;
                        ErrLine = "4b";
                        if (defaultShowingNonNumeric)
                        {
                            ErrLine = "4c";
                            //defaultActionsListAll.Clear();
                            //ErrLine = "4d";
                            //{
                            //    ErrLine = "4e";
                            //    foreach (Part p in EditorLogic.SortedShipList)
                            //    {
                            //        ErrLine = "4f";
                            //        defaultActionsListAll.AddRange(p.Actions);
                            //        ErrLine = "4g";
                            //        foreach (PartModule pm in p.Modules)
                            //        {
                            //            ErrLine = "4h";
                            //            defaultActionsListAll.AddRange(pm.Actions);

                            //        }
                            //        ErrLine = "4i";
                            //    }
                            //    ErrLine = "4j";
                            //}

                            RefreshDefaultActionsList();
                            ErrLine = "4k";
                            RefreshDefaultActionsListType();
                            ErrLine = "4l";
                        }
                        ErrLine = "4m";
                    }
                }
                ErrLine = "5";
                //for (int i = 1; i <= 5; i = i + 1)
                //{
                //    if (PageGrn[i - 1] == true && GroupsPage != i)
                //    {
                //        GUI.DrawTexture(new Rect(96 + (i * 25), 4, 23, 18), BtnTexGrn);
                //    }
                //}

                AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                //GUI.DrawTexture(new Rect(96 + (GroupsPage * 25), 4, 23, 18), BtnTexRed);
                //AGXBtnStyle.normal.background = PageGrn[0] ? ButtonTextureGreen : ButtonTexture;
                //AGXBtnStyle.normal.background = GroupsPage == 1 ? ButtonTextureRed : ButtonTexture;
                if (GroupsPage == 1)
                {
                    AGXBtnStyle.normal.background = ButtonTextureRed;
                    AGXBtnStyle.hover.background = ButtonTextureRed;
                }
                else if (PageGrn[0])
                {
                    AGXBtnStyle.normal.background = ButtonTextureGreen;
                    AGXBtnStyle.hover.background = ButtonTextureGreen;

                }
                else
                {
                    AGXBtnStyle.normal.background = ButtonTexture;
                    AGXBtnStyle.hover.background = ButtonTexture;

                }
                ErrLine = "6";
                if (GUI.Button(new Rect(120, 3, 25, 20), "1", AGXBtnStyle))
                {
                    GroupsPage = 1;
                }
                //AGXBtnStyle.normal.background = PageGrn[1] ? ButtonTextureGreen : ButtonTexture;
                //AGXBtnStyle.normal.background = GroupsPage == 2 ? ButtonTextureRed : ButtonTexture;
                if (GroupsPage == 2)
                {
                    AGXBtnStyle.normal.background = ButtonTextureRed;
                    AGXBtnStyle.hover.background = ButtonTextureRed;
                }
                else if (PageGrn[1])
                {
                    AGXBtnStyle.normal.background = ButtonTextureGreen;
                    AGXBtnStyle.hover.background = ButtonTextureGreen;
                }
                else
                {
                    AGXBtnStyle.normal.background = ButtonTexture;
                    AGXBtnStyle.hover.background = ButtonTexture;
                }
                if (GUI.Button(new Rect(145, 3, 25, 20), "2", AGXBtnStyle))
                {
                    GroupsPage = 2;
                }
                //AGXBtnStyle.normal.background = PageGrn[2] ? ButtonTextureGreen : ButtonTexture;
                //AGXBtnStyle.normal.background = GroupsPage == 3 ? ButtonTextureRed : ButtonTexture;
                if (GroupsPage == 3)
                {
                    AGXBtnStyle.normal.background = ButtonTextureRed;
                    AGXBtnStyle.hover.background = ButtonTextureRed;
                }
                else if (PageGrn[2])
                {
                    AGXBtnStyle.normal.background = ButtonTextureGreen;
                    AGXBtnStyle.hover.background = ButtonTextureGreen;
                }
                else
                {
                    AGXBtnStyle.normal.background = ButtonTexture;
                    AGXBtnStyle.hover.background = ButtonTexture;
                }
                ErrLine = "7";
                if (GUI.Button(new Rect(170, 3, 25, 20), "3", AGXBtnStyle))
                {
                    GroupsPage = 3;
                }
                //AGXBtnStyle.normal.background = PageGrn[3] ? ButtonTextureGreen : ButtonTexture;
                //AGXBtnStyle.normal.background = GroupsPage == 4 ? ButtonTextureRed : ButtonTexture;
                if (GroupsPage == 4)
                {
                    AGXBtnStyle.normal.background = ButtonTextureRed;
                    AGXBtnStyle.hover.background = ButtonTextureRed;
                }
                else if (PageGrn[3])
                {
                    AGXBtnStyle.normal.background = ButtonTextureGreen;
                    AGXBtnStyle.hover.background = ButtonTextureGreen;
                }
                else
                {
                    AGXBtnStyle.normal.background = ButtonTexture;
                    AGXBtnStyle.hover.background = ButtonTexture;
                }
                if (GUI.Button(new Rect(195, 3, 25, 20), "4", AGXBtnStyle))
                {
                    GroupsPage = 4;
                }
                //AGXBtnStyle.normal.background = PageGrn[4] ? ButtonTextureGreen : ButtonTexture;
                //AGXBtnStyle.normal.background = GroupsPage == 5 ? ButtonTextureRed : ButtonTexture;
                if (GroupsPage == 5)
                {
                    AGXBtnStyle.normal.background = ButtonTextureRed;
                    AGXBtnStyle.hover.background = ButtonTextureRed;
                }
                else if (PageGrn[4])
                {
                    AGXBtnStyle.normal.background = ButtonTextureGreen;
                    AGXBtnStyle.hover.background = ButtonTextureGreen;
                }
                else
                {
                    AGXBtnStyle.normal.background = ButtonTexture;
                    AGXBtnStyle.hover.background = ButtonTexture;
                }
                if (GUI.Button(new Rect(220, 3, 25, 20), "5", AGXBtnStyle))
                {
                    GroupsPage = 5;
                }
                ErrLine = "8";
                if (showAllPartsList) //show all parts list is clicked so change to that mode
                {
                    ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollGroups, new Rect(0, 0, 240, Mathf.Max(500, showAllPartsListTitles.Count * 20))); //scroll view just in case there are a lot of parts to list
                    int listCount = 1; //track which button we are on in list
                    while (listCount <= showAllPartsListTitles.Count) //procedurally generate buttons
                    {
                        if (GUI.Button(new Rect(0, (listCount - 1) * 20, 240, 20), showAllPartsListTitles.ElementAt(listCount - 1), AGXBtnStyle)) //button code
                        {
                            string partNameToSelect = showAllPartsListTitles.ElementAt(listCount - 1); //title of part clicked on as string, not a Part object
                            AGEditorSelectedParts.Clear(); //selected parts list should be clear if we are in this mode, but check anyways
                            foreach (Part p in EditorLogic.SortedShipList) //add all Parts with matching title to selected parts list, converting from string to Part
                            {
                                if (p.partInfo.title == partNameToSelect)
                                {

                                    AGEditorSelectedParts.Add(new AGXPart(p));

                                }
                            }
                            //AGEditorSelectedParts.RemoveAll(p2 => p2.AGPart.name != AGEditorSelectedParts.First().AGPart.name); //error trap just incase two parts have the same title, they can't have the same name
                            PartActionsList.Clear(); //populate actions list from selected parts
                            PartActionsList.AddRange(AGEditorSelectedParts.First().AGPart.Actions.Where(ba => ba.active == true));
                            foreach (PartModule pm in AGEditorSelectedParts.First().AGPart.Modules)
                            {
                                PartActionsList.AddRange(pm.Actions.Where(ba => ba.active == true));
                            }
                            //ScrollGroups = Vector2.zero;
                            showAllPartsList = false; //exit show all parts mode
                            TempShowGroupsWin = false; //hide window if auto hide enabled
                            AGEEditorSelectedPartsSame = true; //all selected parts are the same type as per the check above
                        }

                        listCount = listCount + 1; //moving to next button
                    }

                    GUI.EndScrollView();
                    ErrLine = "9";
                }
                else if (defaultShowingNonNumeric)
                {
                    ErrLine = "10";
                    if (defaultGroupToShow == KSPActionGroup.Abort)
                    {
                        AGXBtnStyle.normal.background = ButtonTextureGreen;
                        AGXBtnStyle.hover.background = ButtonTextureGreen;
                    }
                    else
                    {
                        AGXBtnStyle.normal.background = ButtonTexture;
                        AGXBtnStyle.hover.background = ButtonTexture;
                    }
                    ErrLine = "11";
                    if (GUI.Button(new Rect(5, 25, 58, 20), "Abort", AGXBtnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Abort;
                        RefreshDefaultActionsListType();
                    }
                    if (defaultGroupToShow == KSPActionGroup.Brakes)
                    {
                        AGXBtnStyle.normal.background = ButtonTextureGreen;
                        AGXBtnStyle.hover.background = ButtonTextureGreen;
                    }
                    else
                    {
                        AGXBtnStyle.normal.background = ButtonTexture;
                        AGXBtnStyle.hover.background = ButtonTexture;
                    }
                    ErrLine = "12";
                    if (GUI.Button(new Rect(64, 25, 58, 20), "Brakes", AGXBtnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Brakes;
                        RefreshDefaultActionsListType();
                    }
                    if (defaultGroupToShow == KSPActionGroup.Gear)
                    {
                        AGXBtnStyle.normal.background = ButtonTextureGreen;
                        AGXBtnStyle.hover.background = ButtonTextureGreen;
                    }
                    else
                    {
                        AGXBtnStyle.normal.background = ButtonTexture;
                        AGXBtnStyle.hover.background = ButtonTexture;
                    }
                    if (GUI.Button(new Rect(122, 25, 59, 20), "Gear", AGXBtnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Gear;
                        RefreshDefaultActionsListType();
                    }
                    if (defaultGroupToShow == KSPActionGroup.Light)
                    {
                        AGXBtnStyle.normal.background = ButtonTextureGreen;
                        AGXBtnStyle.hover.background = ButtonTextureGreen;
                    }
                    else
                    {
                        AGXBtnStyle.normal.background = ButtonTexture;
                        AGXBtnStyle.hover.background = ButtonTexture;
                    }
                    if (GUI.Button(new Rect(182, 25, 58, 20), "Lights", AGXBtnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Light;
                        RefreshDefaultActionsListType();
                    }

                    if (defaultGroupToShow == KSPActionGroup.RCS)
                    {
                        AGXBtnStyle.normal.background = ButtonTextureGreen;
                        AGXBtnStyle.hover.background = ButtonTextureGreen;
                    }
                    else
                    {
                        AGXBtnStyle.normal.background = ButtonTexture;
                        AGXBtnStyle.hover.background = ButtonTexture;
                    }
                    if (GUI.Button(new Rect(5, 45, 76, 20), "RCS", AGXBtnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.RCS;
                        RefreshDefaultActionsListType();
                    }
                    if (defaultGroupToShow == KSPActionGroup.SAS)
                    {
                        AGXBtnStyle.normal.background = ButtonTextureGreen;
                        AGXBtnStyle.hover.background = ButtonTextureGreen;
                    }
                    else
                    {
                        AGXBtnStyle.normal.background = ButtonTexture;
                        AGXBtnStyle.hover.background = ButtonTexture;
                    }
                    if (GUI.Button(new Rect(81, 45, 76, 20), "SAS", AGXBtnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.SAS;
                        RefreshDefaultActionsListType();
                    }
                    if (defaultGroupToShow == KSPActionGroup.Stage)
                    {
                        AGXBtnStyle.normal.background = ButtonTextureGreen;
                        AGXBtnStyle.hover.background = ButtonTextureGreen;
                    }
                    else
                    {
                        AGXBtnStyle.normal.background = ButtonTexture;
                        AGXBtnStyle.hover.background = ButtonTexture;
                    }
                    if (GUI.Button(new Rect(157, 45, 76, 20), "Stage", AGXBtnStyle)) //button code
                    {
                        defaultGroupToShow = KSPActionGroup.Stage;
                        RefreshDefaultActionsListType();
                    }

                    ErrLine = "17";


                    AGXBtnStyle.normal.background = ButtonTexture;
                    AGXBtnStyle.hover.background = ButtonTexture;
                    //add vector2
                    groupWinScroll = GUI.BeginScrollView(new Rect(5, 70, 240, 455), groupWinScroll, new Rect(0, 0, 240, Mathf.Max(455, defaultActionsListThisType.Count * 20)));
                    int listCount2 = 1;
                    highlightPartThisFrameGroupWin = false;
                    ErrLine = "18";
                    while (listCount2 <= defaultActionsListThisType.Count)
                    {
                        if (Mouse.screenPos.y >= GroupsWin.y + 70 && Mouse.screenPos.y <= GroupsWin.y + 525 && new Rect(GroupsWin.x + 5, (GroupsWin.y + 70 + ((listCount2 - 1) * 20)) - groupWinScroll.y, 240, 20).Contains(Mouse.screenPos))
                        {
                            highlightPartThisFrameGroupWin = true;
                            //print("part found to highlight acts " + highlightPartThisFrameActsWin + " " + ThisGroupActions.ElementAt(RowCnt - 1).ba.listParent.part.transform.position);
                            partToHighlight = defaultActionsListThisType.ElementAt(listCount2 - 1).listParent.part;
                        }

                        if (GUI.Button(new Rect(0, (listCount2 - 1) * 20, 240, 20), defaultActionsListThisType.ElementAt(listCount2 - 1).listParent.part.partInfo.title + " " + defaultActionsListThisType.ElementAt(listCount2 - 1).guiName, AGXBtnStyle)) //button code
                        {
                            defaultActionsListThisType.ElementAt(listCount2 - 1).actionGroup = defaultActionsListThisType.ElementAt(listCount2 - 1).actionGroup & ~defaultGroupToShow;
                            RefreshDefaultActionsListType();
                        }
                        listCount2 = listCount2 + 1;
                    }
                    GUI.EndScrollView();
                    ErrLine = "19";
                }
                else
                {
                    ErrLine = "20";
                    AGXBtnStyle.normal.background = ButtonTexture;
                    AGXBtnStyle.hover.background = ButtonTexture;
                    ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollGroups, new Rect(0, 0, 240, 500));

                    int ButtonID = new int();
                    ButtonID = 1 + (50 * (GroupsPage - 1));
                    int ButtonPos = new int();
                    ButtonPos = 1;
                    TextAnchor TxtAnch3 = new TextAnchor();
                    TxtAnch3 = GUI.skin.button.alignment;
                    //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
                    while (ButtonPos <= 25)
                    {
                        if (ShowKeySetWin)
                        {
                            string btnName = "";
                            if (AGXguiMod1Groups[ButtonID] && AGXguiMod2Groups[ButtonID])
                            {
                                btnName = '\u00bd' + AGXguiKeys[ButtonID].ToString();
                            }
                            else if (AGXguiMod1Groups[ButtonID])
                            {
                                btnName = '\u2474' + AGXguiKeys[ButtonID].ToString();
                            }
                            else if (AGXguiMod2Groups[ButtonID])
                            {
                                btnName = '\u2475' + AGXguiKeys[ButtonID].ToString();
                            }
                            else
                            {
                                btnName = AGXguiKeys[ButtonID].ToString();
                            }

                            //btnName = btnName + AGXguiKeys[ButtonID].ToString();
                            if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + " Key: " + btnName, AGXBtnStyle))
                            {

                                AGXCurActGroup = ButtonID;
                                ShowKeyCodeWin = true;
                            }
                        }

                        else
                        {
                            //if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID))
                            //{
                            //    GUI.DrawTexture(new Rect(1, ((ButtonPos - 1) * 20) + 1, 118, 18), BtnTexGrn);
                            //}

                            AGXBtnStyle.normal.background = CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? ButtonTextureGreen : ButtonTexture;
                            AGXBtnStyle.hover.background = CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? ButtonTextureGreen : ButtonTexture;
                            if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID], AGXBtnStyle))
                            {
                                AGXCurActGroup = ButtonID;
                                TempShowGroupsWin = false;
                            }
                            AGXBtnStyle.normal.background = ButtonTexture;
                            AGXBtnStyle.hover.background = ButtonTexture;
                        }
                        ButtonPos = ButtonPos + 1;
                        ButtonID = ButtonID + 1;
                    }
                    ErrLine = "21";
                    while (ButtonPos <= 50)
                    {
                        if (ShowKeySetWin)
                        {

                            string btnName2 = "";
                            if (AGXguiMod1Groups[ButtonID] && AGXguiMod2Groups[ButtonID])
                            {
                                btnName2 = '\u00bd' + AGXguiKeys[ButtonID].ToString();
                            }
                            else if (AGXguiMod1Groups[ButtonID])
                            {
                                btnName2 = '\u2474' + AGXguiKeys[ButtonID].ToString();
                            }
                            else if (AGXguiMod2Groups[ButtonID])
                            {
                                btnName2 = '\u2475' + AGXguiKeys[ButtonID].ToString();
                            }
                            else
                            {
                                btnName2 = AGXguiKeys[ButtonID].ToString();
                            }
                            if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + " Key: " + btnName2, AGXBtnStyle))
                            {
                                AGXCurActGroup = ButtonID;
                                ShowKeyCodeWin = true;
                            }
                        }
                        else
                        {
                            //if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID))
                            //{
                            //    GUI.DrawTexture(new Rect(121, ((ButtonPos - 26) * 20) + 1, 118, 18), BtnTexGrn);
                            //}
                            AGXBtnStyle.normal.background = CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? ButtonTextureGreen : ButtonTexture;
                            AGXBtnStyle.hover.background = CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? ButtonTextureGreen : ButtonTexture;
                            if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID], AGXBtnStyle))
                            {


                                AGXCurActGroup = ButtonID;
                                TempShowGroupsWin = false;

                            }
                            AGXBtnStyle.normal.background = ButtonTexture;
                            AGXBtnStyle.hover.background = ButtonTexture;
                        }
                        ButtonPos = ButtonPos + 1;
                        ButtonID = ButtonID + 1;
                    }
                    //GUI.skin.button.alignment = TxtAnch3;
                    AGXBtnStyle.alignment = TextAnchor.MiddleCenter;

                    GUI.EndScrollView();

                }
                GUI.DragWindow();
                AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                ErrLine = "22";
            }
            catch (Exception e)
            {
                print("AGX GroupsWin Error " + ErrLine + " " + e);
            }
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
                PartActionsList.AddRange(p.Actions.Where(ba => ba.active == true));
                foreach (PartModule pm in p.Modules)
                {
                    PartActionsList.AddRange(pm.Actions.Where(ba => ba.active == true));
                }

            }
            return RetLst;

        }

        public static void LoadGroupNames(string LoadNames) //v2 done
        {

            string errLine = "1";
            try
            {
                for (int i = 1; i <= 250; i = i + 1)
                {
                    AGXguiNames[i] = "";
                }

                errLine = "13";
                //print("AGX Load Name: " + EditorLogic.RootPart.partName+" "+ LoadNames);

                if (LoadNames.Length > 0)
                {
                    errLine = "14";
                    while (LoadNames[0] == '\u2023')
                    {
                        errLine = "15";
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

            catch (Exception e)
            {
                print("AGXED LoadGroupNames Fail " + errLine + " " + e);
            }
        }







        //public void LoadDefaultActionGroups() //not used?
        //{

        //    List<KSPActionGroup> CustomActions = new List<KSPActionGroup>();
        //    CustomActions.Add(KSPActionGroup.Custom01); //how do you add a range from enum?
        //    CustomActions.Add(KSPActionGroup.Custom02);
        //    CustomActions.Add(KSPActionGroup.Custom03);
        //    CustomActions.Add(KSPActionGroup.Custom04);
        //    CustomActions.Add(KSPActionGroup.Custom05);
        //    CustomActions.Add(KSPActionGroup.Custom06);
        //    CustomActions.Add(KSPActionGroup.Custom07);
        //    CustomActions.Add(KSPActionGroup.Custom08);
        //    CustomActions.Add(KSPActionGroup.Custom09);
        //    CustomActions.Add(KSPActionGroup.Custom10);

        //    foreach (Part p in EditorLogic.SortedShipList)
        //    {
        //        string AddGroup = "";
        //        foreach (PartModule pm in p.Modules)
        //        {
        //            foreach (BaseAction ba in pm.Actions)
        //            {
        //                foreach (KSPActionGroup agrp in CustomActions)
        //                {

        //                    if ((ba.actionGroup & agrp) == agrp)

        //                    {

        //                        AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) +1).ToString("000") + ba.guiName;
        //                    }
        //                }
        //            }
        //        }
        //        foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
        //        {
        //            pm.Fields.SetValue("AGXData", AddGroup);
        //        }
        //    }
        //}


        public void Update()
        {

            if (checkShipsExist)
            {
                if (checkShipsExistDelay >= 30)
                {
                    checkShipsExist = false;
                    checkShipsExistDelay = 0;
                    CheckExistingShips();
                }
                else
                {
                    checkShipsExistDelay = checkShipsExistDelay + 1;
                }
            }



            EditorLogic ELCur = new EditorLogic();
            ELCur = EditorLogic.fetch;//get current editor logic instance



            if (AGXDoLock && ELCur.editorScreen != EditorScreen.Actions)
            {
                ELCur.Unlock("AGXLock");
                AGXDoLock = false;
            }
            else if (AGXDoLock && !TrapMouse)
            {
                ELCur.Unlock("AGXLock");
                AGXDoLock = false;
            }
            else if (!AGXDoLock && TrapMouse && ELCur.editorScreen == EditorScreen.Actions)
            {
                ELCur.Lock(false, false, false, "AGXLock");
                AGXDoLock = true;
            }


            if (ELCur.editorScreen == EditorScreen.Actions) //only show mod if on actions editor screen
            {

                ShowSelectedWin = true;
            }
            else
            {

                ShowSelectedWin = false;




                AGEditorSelectedParts.Clear();//mod is hidden, clear list so parts don't get orphaned in it

                PartActionsList.Clear();

                ActionsListDirty = true;
            }


            if (ShowSelectedWin)
            {



                if (EditorActionGroups.Instance.GetSelectedParts() != null) //on first run, list is null
                {

                    if (ActionsListDirty)
                    {
                        UpdateActionsListCheck();

                    }
                    if (EditorActionGroups.Instance.GetSelectedParts().Count > 0) //are there parts selected?
                    {


                        if (PreviousSelectedPart != EditorActionGroups.Instance.GetSelectedParts().First()) //has selected part changed?
                        {

                            if (!AGEditorSelectedParts.Any(p => p.AGPart == EditorActionGroups.Instance.GetSelectedParts().First())) //make sure selected part is not already in AGEdSelParts
                            {

                                if (AGEditorSelectedParts.Count == 0) //no items in Selected Parts list, so just add selection
                                {
                                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));


                                }
                                else if (AGEditorSelectedParts.First().AGPart.name == EditorActionGroups.Instance.GetSelectedParts().First().name) //selected part matches first part already in selected parts list, so just add selected part
                                {
                                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));


                                }
                                else //part does not match first part in list, clear list before adding part
                                {

                                    AGEditorSelectedParts.Clear();
                                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(), SelPartsIncSym));



                                }
                            }
                            PreviousSelectedPart = EditorActionGroups.Instance.GetSelectedParts().First(); //remember selected part so logic does not run unitl another part selected
                        }

                    }
                }

            }


            if (EditorLogic.fetch.editorScreen == EditorScreen.Actions && !AGXShow)
            {
                MonitorDefaultActions();
            }

            if (AGXRoot != EditorLogic.RootPart)
            {
                // print("Root diff");
                EditorLoadFromNode();
            }
            //print("detach " + DetachedPartActions.Count);
            //foreach (Part p in EditorLogic.SortedShipList)
            //{
            //    print(p.name + " " + p.symmetryCounterparts.Count + " " + p.GetHashCode());
            //}

            // print("test " + FindObjectsOfType<EditorSubassemblyItem>().Count());
        } //close Update()
        //if(needToAddStockButton)
        //{
        //    if (ApplicationLauncher.Ready)
        //    {
        //        print("AppLaunc ready");
        //        AGXAppEditorButton = ApplicationLauncher.Instance.AddModApplication(onLeftButtonClick, onLeftButtonClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, (Texture)GameDatabase.Instance.GetTexture("Diazo/AGExt/icon_button", false));
        //        needToAddStockButton = false;
        //    }
        //    else
        //    {
        //        print("Applaunch not ready");
        //    }
        //}

        //foreach (Part p in EditorLogic.SortedShipList)
        //{
        //    print("PartLoc " + p.ConstructID + " " + p.orgPos);
        //}
        //PrintPartPos();
        //PrintPartActs();
        //PrintSelectedPart();
        //print("Keyset " + CurrentKeySet);



        public void PrintSelectedPart()
        {
            if (DateTime.Now.Second.ToString().Substring(DateTime.Now.Second.ToString().Length - 1, 1) == "0")
            {
                print("All true");
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    foreach (PartModule pm in p.Modules)
                    {
                        foreach (BaseAction ba in pm.Actions)
                        {
                            ba.active = true;
                        }
                    }
                }
            }
            if (DateTime.Now.Second.ToString().Substring(DateTime.Now.Second.ToString().Length - 1, 1) == "5")
            {
                print("All false");
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    foreach (PartModule pm in p.Modules)
                    {
                        foreach (BaseAction ba in pm.Actions)
                        {
                            ba.active = false;
                        }
                    }
                }
            }
        }

        public void PrintPartActs()
        {
            try
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {

                    //List<BaseAction> partActs = new List<BaseAction>();
                    // partActs.AddRange(p.Actions);
                    foreach (PartModule pm in p.Modules)
                    {
                        print(pm.moduleName);
                        //partActs.AddRange(pm.Actions);
                    }
                    //print(p.ConstructID);
                    //foreach (BaseAction ba in partActs)
                    //{
                    //    print(ba.listParent.module.moduleName + " " + ba.name + " " + ba.guiName);
                    //}
                }


            }
            catch
            {
                print("Print fail!");
            }
        }

        public void PrintPartPos()
        {
            try
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    print(p.partInfo.title + " " + p.orgPos + " " + EditorLogic.RootPart.transform.InverseTransformPoint(p.transform.position) + " " + p.orgRot);

                }
            }
            catch
            {
                print("Print fail!");
            }
        }
        public void MonitorDefaultActions()
        {
            //print("2a");
            KSPActionGroup KSPDefaultActionGroupThisFrame = KSPActionGroup.Custom01;
            try //find which action group is selected in default ksp editor this pass
            {
                string grpText = "None";
                for (int i = 0; i < EditorActionGroups.Instance.actionGroupList.Count; i++)
                {
                    IUIListObject lObj = EditorActionGroups.Instance.actionGroupList.GetItem(i);
                    UIListItem LstItem = (UIListItem)lObj;
                    if (LstItem.controlState == UIButton.CONTROL_STATE.ACTIVE)
                    {
                        grpText = LstItem.Text;
                    }
                }

                KSPDefaultActionGroupThisFrame = (KSPActionGroup)Enum.Parse(typeof(KSPActionGroup), grpText);
                //print("Selected group " + KSPDefaultLastActionGroup);

            }
            catch
            {
                print("AGX Monitor default list fail");
            }
            string errLine = "1";
            try
            {
                errLine = "2";
                if (EditorActionGroups.Instance.GetSelectedParts() != null) //is a part selected?
                {
                    errLine = "3";
                    if (EditorActionGroups.Instance.GetSelectedParts().Count > 0) //list can exist with no parts in it if you selected then unselect one
                    {
                        errLine = "4";
                        if (SelectedWithSym.Count == 0 || SelectedWithSym.First() != EditorActionGroups.Instance.GetSelectedParts().First() || KSPDefaultActionGroupThisFrame != KSPDefaultLastActionGroup) //check if there is a previously selected part, if so check if its changed
                        {
                            errLine = "5";
                            //print("2b");
                            //parts are different
                            SelectedWithSym.Clear(); //reset lastpart list
                            SelectedWithSym.AddRange(EditorActionGroups.Instance.GetSelectedParts());
                            SelectedWithSym.AddRange(EditorActionGroups.Instance.GetSelectedParts().First().symmetryCounterparts);
                            SelectedWithSymActions.Clear(); //reset actions list
                            //print("2c");
                            errLine = "6";
                            foreach (Part prt in SelectedWithSym)
                            {
                                //  print("2d");
                                errLine = "7";
                                foreach (BaseAction bap in prt.Actions) //get part actions
                                {
                                    SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bap, agrp = bap.actionGroup }); //add actiongroup separate otherwise it links and so have nothing to compare
                                }
                                errLine = "8";
                                foreach (PartModule pm in prt.Modules) //add actions from all partmodules
                                {
                                    errLine = "9";
                                    foreach (BaseAction bapm in pm.Actions)
                                    {
                                        errLine = "10";
                                        SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bapm, agrp = bapm.actionGroup });
                                    }
                                }
                            }
                            errLine = "11";
                            int groupToAdd = 1;
                            switch (KSPDefaultActionGroupThisFrame)
                            {
                                case KSPActionGroup.Custom01:
                                    groupToAdd = 1;
                                    break;
                                case KSPActionGroup.Custom02:
                                    groupToAdd = 2;
                                    break;
                                case KSPActionGroup.Custom03:
                                    groupToAdd = 3;
                                    break;
                                case KSPActionGroup.Custom04:
                                    groupToAdd = 4;
                                    break;
                                case KSPActionGroup.Custom05:
                                    groupToAdd = 5;
                                    break;
                                case KSPActionGroup.Custom06:
                                    groupToAdd = 6;
                                    break;
                                case KSPActionGroup.Custom07:
                                    groupToAdd = 7;
                                    break;
                                case KSPActionGroup.Custom08:
                                    groupToAdd = 8;
                                    break;
                                case KSPActionGroup.Custom09:
                                    groupToAdd = 9;
                                    break;
                                case KSPActionGroup.Custom10:
                                    groupToAdd = 10;
                                    break;
                            }
                            errLine = "12";
                            foreach (AGXAction agact2 in CurrentVesselActions.Where(ag => ag.group == groupToAdd))
                            {
                                SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = agact2.ba, agrp = agact2.ba.actionGroup });
                            }
                            errLine = "13";
                            KSPDefaultLastActionGroup = KSPDefaultActionGroupThisFrame;
                            //foreach (AGXDefaultCheck dC in SelectedWithSymActions)
                            //{
                            //    print("Acts " + dC.ba.name); 
                            //}
                            // print("2e");
                        }
                        else //selected part is the same a previously selected part
                        {
                            errLine = "14";
                            //print("2f");
                            List<Part> PartsThisFrame = new List<Part>(); //get list of parts this update frame
                            PartsThisFrame.AddRange(EditorActionGroups.Instance.GetSelectedParts());
                            PartsThisFrame.AddRange(EditorActionGroups.Instance.GetSelectedParts().First().symmetryCounterparts);
                            // print("2g");
                            List<BaseAction> ThisFrameActions = new List<BaseAction>(); //get actions fresh again this update frame
                            foreach (Part prt in PartsThisFrame)
                            {
                                errLine = "15";
                                // print("2h"); 
                                foreach (BaseAction bap in prt.Actions)
                                {
                                    ThisFrameActions.Add(bap);
                                }
                                errLine = "16";
                                foreach (PartModule pm in prt.Modules)
                                {
                                    foreach (BaseAction bapm in pm.Actions)
                                    {
                                        ThisFrameActions.Add(bapm);
                                    }
                                }
                            }
                            errLine = "17";
                            int groupToAdd = 1;
                            switch (KSPDefaultActionGroupThisFrame)
                            {
                                case KSPActionGroup.Custom01:
                                    groupToAdd = 1;
                                    break;
                                case KSPActionGroup.Custom02:
                                    groupToAdd = 2;
                                    break;
                                case KSPActionGroup.Custom03:
                                    groupToAdd = 3;
                                    break;
                                case KSPActionGroup.Custom04:
                                    groupToAdd = 4;
                                    break;
                                case KSPActionGroup.Custom05:
                                    groupToAdd = 5;
                                    break;
                                case KSPActionGroup.Custom06:
                                    groupToAdd = 6;
                                    break;
                                case KSPActionGroup.Custom07:
                                    groupToAdd = 7;
                                    break;
                                case KSPActionGroup.Custom08:
                                    groupToAdd = 8;
                                    break;
                                case KSPActionGroup.Custom09:
                                    groupToAdd = 9;
                                    break;
                                case KSPActionGroup.Custom10:
                                    groupToAdd = 10;
                                    break;
                            }

                            foreach (AGXAction agact2 in CurrentVesselActions.Where(ag => ag.group == groupToAdd))
                            {
                                ThisFrameActions.Add(agact2.ba);
                            }

                            errLine = "18";


                            //print("2i");

                            foreach (BaseAction ba2 in ThisFrameActions) //check each action's actiongroup enum against last update frames actiongroup enum, this adds/removes a group to default KSP when added/removed in agx
                            {
                                //print("2j");
                                errLine = "19";
                                AGXDefaultCheck ActionLastFrame = new AGXDefaultCheck();
                                //print("2j1");
                                ActionLastFrame = SelectedWithSymActions.Find(a => a.ba == ba2);
                                // print("2j2");
                                errLine = "20";
                                if (ActionLastFrame.agrp != ba2.actionGroup) //actiongroup enum is different
                                {
                                    //  print("2j3");
                                    int NewGroup = 0; //which actiongroup changed?
                                    if (KSPActionGroup.Custom01 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 1;
                                    }
                                    else if (KSPActionGroup.Custom02 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 2;
                                    }
                                    else if (KSPActionGroup.Custom03 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 3;
                                    }
                                    else if (KSPActionGroup.Custom04 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 4;
                                    }
                                    else if (KSPActionGroup.Custom05 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 5;
                                    }
                                    else if (KSPActionGroup.Custom06 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 6;
                                    }
                                    else if (KSPActionGroup.Custom07 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 7;
                                    }
                                    else if (KSPActionGroup.Custom08 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 8;
                                    }
                                    else if (KSPActionGroup.Custom09 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 9;
                                    }
                                    else if (KSPActionGroup.Custom10 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                    {
                                        NewGroup = 10;
                                    }

                                    // print("2k");
                                    errLine = "21";

                                    if (NewGroup != 0) //if one of the other actiongroups (gear, lights) has changed, ignore it. newgroup will be the actiongroup if I want to process it.
                                    {
                                        // print("Newgroup called on " + NewGroup);
                                        errLine = "22";
                                        if (Mouse.screenPos.x >= 130 && Mouse.screenPos.x <= 280)
                                        {
                                            //print("remove actions");
                                            //AGXAction ToRemove = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = NewGroup, activated = false };
                                            CurrentVesselActions.RemoveAll(ag3 => ag3.ba == ba2 && ag3.group == NewGroup);
                                        }
                                        else
                                        {
                                            errLine = "23";
                                            //print("add actions");
                                            AGXAction ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = NewGroup, activated = false };
                                            List<AGXAction> Checking = new List<AGXAction>();
                                            Checking.AddRange(CurrentVesselActions);
                                            Checking.RemoveAll(p => p.group != ToAdd.group);
                                            Checking.RemoveAll(p => p.prt != ToAdd.prt);
                                            Checking.RemoveAll(p => p.ba != ToAdd.ba);

                                            if (Checking.Count == 0)
                                            {
                                                CurrentVesselActions.Add(ToAdd);
                                                //SaveCurrentVesselActions();
                                            }
                                        }

                                    }
                                    errLine = "24";
                                    ActionLastFrame.agrp = KSPActionGroup.None;
                                    ActionLastFrame.agrp = ActionLastFrame.agrp | ba2.actionGroup;
                                    //print("2l");
                                }

                            }
                            SelectedWithSymActions.Clear(); //reset actions list as one of the enums changed.
                            //print("2k");
                            errLine = "25";
                            foreach (Part prt in SelectedWithSym)
                            {
                                errLine = "26";
                                foreach (BaseAction bap in prt.Actions) //get part actions
                                {
                                    SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bap, agrp = bap.actionGroup }); //add actiongroup separate otherwise it links and so have nothing to compare
                                }
                                errLine = "27";
                                foreach (PartModule pm in prt.Modules) //add actions from all partmodules
                                {
                                    foreach (BaseAction bapm in pm.Actions)
                                    {
                                        SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bapm, agrp = bapm.actionGroup });
                                    }
                                }
                            }
                            errLine = "28";
                            foreach (AGXAction agact2 in CurrentVesselActions.Where(ag => ag.group == groupToAdd))
                            {
                                SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = agact2.ba, agrp = agact2.ba.actionGroup });
                            }

                            errLine = "29";

                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Log("AGX Monitor Default Actions Error " + errLine + " " + e);
            }
            //print("2l " + Mouse.screenPos);

        }

        public static void EditorLoadFromFile()
        {
            //print("EDITORLoadFromFile called");
            string errLine = "1";
            //CurrentVesselActions.Clear();
            try
            {
                //if (EditorLogic.SortedShipList.Count > 0)
                //{
                //ConfigNode AGXBaseNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
                errLine = "2";

                errLine = "9";
                AGXEditorNode = new ConfigNode("EDITOR");
                AGXEditorNode.AddValue("name", "editor");

                //print("Load 2");
                errLine = "9a";
                try
                {
                    if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg"))
                    {
                        //print("Load 3");
                        errLine = "9b";
                        AGXEditorNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
                        if (!AGXEditorNode.HasValue("name"))
                        {
                            AGXEditorNode.AddValue("name", "editor");
                        }
                        //print("Load 4");
                    }
                    else
                    {
                        errLine = "9c";
                        AGXEditorNode = new ConfigNode("EDITOR");
                        AGXEditorNode.AddValue("name", "editor");
                    }
                }
                catch
                {
                    errLine = "9d";
                    AGXEditorNode = new ConfigNode("EDITOR");
                    AGXEditorNode.AddValue("name", "editor");
                    print("AGX Load Editor Node FAILED, resetting it");
                }
                //}
            }
            catch (Exception e)
            {
                print("AGX EditorLoadFromFile Fail " + errLine + " " + e);
            }
        }

        public static void EditorLoadFromNode()
        {
            //print("LoadFromNode Called");
            string errLine = "1";
            try
            {
                errLine = "10";
                string hashedShipName = AGextScenario.EditorHashShipName(EditorLogic.fetch.shipNameField.Text, inVAB);
                errLine = "10a";
                //print(hashedShipName);
                ConfigNode thisVsl = new ConfigNode();
                errLine = "10b";
                // print(AGXEditorNode);
                if (AGXEditorNode == null)
                {
                    EditorLoadFromFile();
                    print("AGX EditorNode is Null, recovering....");
                }
                errLine = "10bc";
                if (AGXEditorNode.HasNode(hashedShipName))
                {
                    errLine = "11";
                    thisVsl = AGXEditorNode.GetNode(hashedShipName);
                }
                errLine = "12";
                if (thisVsl.HasValue("currentKeyset"))
                {
                    CurrentKeySet = Convert.ToInt32((string)thisVsl.GetValue("currentKeyset"));
                    //print("curkey a " + CurrentKeySet + " " + thisVsl.GetValue("currentKeyset"));
                }
                else
                {
                    CurrentKeySet = 1;
                    //print("curkey b " + CurrentKeySet);
                }
                if (CurrentKeySet < 1 || CurrentKeySet > 5)
                {
                    //print("curkey c " + CurrentKeySet);
                    CurrentKeySet = 1;
                }
                LoadCurrentKeyBindings();
                CurrentKeySetName = KeySetNames[CurrentKeySet - 1];
                errLine = "13";
                if (thisVsl.HasValue("groupNames"))
                {
                    LoadGroupNames(thisVsl.GetValue("groupNames"));
                }
                else
                {
                    LoadGroupNames("");
                }
                errLine = "14";
                if (thisVsl.HasValue("groupVisibility"))
                {
                    LoadGroupVisibility(thisVsl.GetValue("groupVisibility"));
                }
                else
                {
                    LoadGroupVisibility("");
                }
                errLine = "15";
                if (thisVsl.HasValue("groupVisibilityNames"))
                {
                    LoadGroupVisibilityNames(thisVsl.GetValue("groupVisibilityNames"));
                }
                else
                {
                    LoadGroupVisibilityNames("Group1" + '\u2023' + "Group2" + '\u2023' + "Group3" + '\u2023' + "Group4" + '\u2023' + "Group5");
                }
                if (thisVsl.HasValue("DirectActionState"))
                {
                    LoadDirectActionState(thisVsl.GetValue("DirectActionState"));
                }
                else
                {
                    LoadDirectActionState("");
                }
                errLine = "15a";
                //print("adfg " + thisVsl.CountNodes);
                foreach (ConfigNode prtNode in thisVsl.nodes)
                {
                    Vector3 partLoc = new Vector3((float)Convert.ToDouble(prtNode.GetValue("relLocX")), (float)Convert.ToDouble(prtNode.GetValue("relLocY")), (float)Convert.ToDouble(prtNode.GetValue("relLocZ")));
                    float partDist = 100f;
                    Part gamePart = new Part();
                    try
                    {
                        foreach (Part p in EditorLogic.SortedShipList) //do a distance compare check, floats do not guarantee perfect decimal accuray so use part with least distance, should be zero distance in most cases
                        {
                            float thisPartDist = Vector3.Distance(partLoc, EditorLogic.RootPart.transform.InverseTransformPoint(p.transform.position));
                            if (thisPartDist < partDist)
                            {
                                gamePart = p;
                                partDist = thisPartDist;
                            }
                        }
                        bool ShowAmbiguousMessage = true;
                        if (partDist < 0.3f) //do not show it if part found is more then 0.3meters off
                        {
                            ShowAmbiguousMessage = true;
                        }
                        else
                        {
                            ShowAmbiguousMessage = false;
                        }
                        foreach (ConfigNode actNode in prtNode.nodes)
                        {
                            AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(actNode, gamePart, ShowAmbiguousMessage);
                            if (actToAdd.ba != null)
                            {
                                CurrentVesselActions.Add(actToAdd);
                            }
                        }
                    }
                    catch
                    {
                        //Silently fail, if we hit this it is because EditorLogic.sorted ship list is not valid
                    }
                }
                errLine = "15b";
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

                errLine = "16";
                // string AddGroup = "";
                List<BaseAction> partAllActions = new List<BaseAction>(); //is all vessel actions, copy pasting code
                try
                {
                    foreach (Part p in EditorLogic.SortedShipList)
                    {
                        partAllActions.AddRange(p.Actions);
                        foreach (PartModule pm in p.Modules)
                        {
                            partAllActions.AddRange(pm.Actions);
                        }
                    }

                    foreach (BaseAction baLoad in partAllActions)
                    {
                        foreach (KSPActionGroup agrp in CustomActions)
                        {

                            if ((baLoad.actionGroup & agrp) == agrp)
                            {
                                errLine = "17";
                                ////AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) + 1).ToString("000") + baLoad.guiName;
                                //partAGActions2.Add(new AGXAction() { group = CustomActions.IndexOf(agrp) + 1, prt = this.part, ba = baLoad, activated = false });
                                AGXAction ToAdd = new AGXAction() { prt = baLoad.listParent.part, ba = baLoad, group = CustomActions.IndexOf(agrp) + 1, activated = false };
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(CurrentVesselActions);
                                Checking.RemoveAll(p => p.group != ToAdd.group);

                                Checking.RemoveAll(p => p.prt != ToAdd.prt);

                                Checking.RemoveAll(p => p.ba != ToAdd.ba);



                                if (Checking.Count == 0)
                                {

                                    CurrentVesselActions.Add(ToAdd);

                                }
                            }
                        }
                        errLine = "18";
                    }

                }
                catch
                {
                    //silently fail, if we hit this EditorLogic.sortedShipList is not valid
                }
                AGXRoot = EditorLogic.RootPart;
            }


            catch (Exception e)
            {
                print("AGX EditorLoadFromNode Fail " + errLine + " " + e);
            }
        }


        //public void LoadActionGroups()
        //{

        //    if(CurrentVesselActions == null)
        //    {

        //        CurrentVesselActions = new List<AGXAction>();
        //    }
        //    else
        //    {

        //    CurrentVesselActions.Clear();
        //    }

        //    bool RootPartExists = new bool();
        //    try
        //    {
        //        if (EditorLogic.SortedShipList.Count >= 1)
        //        {
        //        }
        //        RootPartExists = true;
        //    }
        //    catch
        //    {
        //        RootPartExists = false;
        //    }


        //    if (RootPartExists)
        //    {
        //        foreach (Part p in EditorLogic.SortedShipList)
        //        {

        //            foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
        //            {
        //                CurrentVesselActions.AddRange(agpm.partAGActions);
        //            }
        //        }
        //    }
        //    NeedToLoadActions = false;
        //}

        public static string SaveGroupNames(Part p, string str)
        {
            return str;
        }

        public static string SaveGroupNames(string str)
        {
            string errLine = "1";
            bool OkayToSave = true;
            try
            {
                errLine = "2";

                if (OkayToSave)
                {
                    errLine = "3";
                    string SaveStringNames = "";
                    errLine = "4";

                    int GroupCnt = new int();
                    errLine = "5";
                    GroupCnt = 1;
                    errLine = "6";
                    while (GroupCnt <= 250)
                    {
                        errLine = "7";
                        if (AGXguiNames[GroupCnt].Length >= 1)
                        {
                            errLine = "8";
                            SaveStringNames = SaveStringNames + '\u2023' + GroupCnt.ToString("000") + AGXguiNames[GroupCnt];
                        }
                        errLine = "9";
                        GroupCnt = GroupCnt + 1;
                    }
                    errLine = "10";
                    return SaveStringNames;
                }
                else
                {
                    errLine = "11";
                    return str;
                }
            }
            catch (Exception e)
            {
                print("AGX Editor Fail (SaveGroupNames) " + errLine + " " + e);
                return str;
            }
        }

        public void UpdateActionsListCheck()
        {
            List<AGXAction> KnownGood = new List<AGXAction>();
            KnownGood = new List<AGXAction>();

            foreach (AGXAction agxAct in CurrentVesselActions)
            {
                if (EditorLogic.SortedShipList.Contains(agxAct.prt))
                {
                    KnownGood.Add(agxAct);
                }
            }
            CurrentVesselActions = KnownGood;

            ActionsListDirty = false;
        }

        public void AGXResetPartsList() //clear selected parts list and populate with newly selected part(s)
        {
            AGEEditorSelectedPartsSame = true;
            AGEditorSelectedParts.Clear();
            foreach (Part p in EditorLogic.SortedShipList) //add all parts to list
            {
                AGEditorSelectedParts.Add(new AGXPart(p));
            }


            AGXPart AGPcompare = new AGXPart();
            AGPcompare = AGEditorSelectedParts.First(); //set first part in selected parts list to compare

            foreach (AGXPart p in AGEditorSelectedParts)
            {
                if (p.AGPart.name != AGPcompare.AGPart.name) //remove all parts that are of a different type
                {
                    AGEEditorSelectedPartsSame = false;
                }
            }
        }

        public static void LoadGroupVisibilityNames(string LoadString) //ver2 only
        {
            for (int i = 1; i <= 4; i++)
            {
                int KeyLength = LoadString.IndexOf('\u2023');
                ShowGroupInFlightNames[i] = LoadString.Substring(0, KeyLength);
                LoadString = LoadString.Substring(KeyLength + 1);
            }
            ShowGroupInFlightNames[5] = LoadString;
        }

        public static string SaveGroupVisibilityNames(Part p, string str)
        {
            return str;
        }
        public static string SaveGroupVisibilityNames(string str)
        {
            try
            {
                string StringToSave = ShowGroupInFlightNames[1];
                for (int i = 2; i <= 5; i++)
                {
                    StringToSave = StringToSave + '\u2023' + ShowGroupInFlightNames[i];
                }
                return StringToSave;
            }

            catch
            {
                return str;
            }
        }
        public static string SaveGroupVisibility(Part p, string str)
        {
            return str;
        }
        public static string SaveGroupVisibility(string str)
        {

            try
            {

                string ReturnStr = ShowGroupInFlightCurrent.ToString(); //add currently show flightgroup

                for (int i = 1; i <= 250; i++)
                {
                    ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                    for (int i2 = 1; i2 <= 5; i2++)
                    {
                        ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                    }
                }
                return ReturnStr;

            }
            catch
            {
                return str;
            }
        }

        public static void EditorSaveToFile()
        {
            EditorSaveToNode();
            EditorSaveGlobalInfo();
            EditorWriteNodeToFile();
            // print("name check " + KeySetNames[1]);

        }

        public static void EditorSaveGlobalInfo()
        {
            SaveCurrentKeyBindings();
            EditorSaveKeysetStuff();
        }

        public static void EditorSaveKeysetStuff()
        {
            AGExtNode.SetValue("KeySetName1", KeySetNames[0]);
            AGExtNode.SetValue("KeySetName2", KeySetNames[1]);
            AGExtNode.SetValue("KeySetName3", KeySetNames[2]);
            AGExtNode.SetValue("KeySetName4", KeySetNames[3]);
            AGExtNode.SetValue("KeySetName5", KeySetNames[4]);
            CurrentKeySetName = KeySetNames[CurrentKeySet - 1];
            //AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
            AGXStaticData.SaveBaseConfigNode(AGExtNode);
        }

        public static void EditorWriteNodeToFile()
        {
            try
            {
                AGXEditorNode.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
            }
            catch (Exception e)
            {
                print("AGX EditorWriteNodeToFileFail " + e);
            }

        }

        public static void EditorSaveToNode()
        {
            //print("AGX EditorSaveToFile called"); 
            string errLine = "1";
            bool okayToProceed = false;
            try
            {
                errLine = "2";
                if (EditorLogic.SortedShipList.Count > 0)
                {
                    okayToProceed = true;
                }
                else
                {
                    okayToProceed = false;
                }
                errLine = "3";
            }
            catch
            {
                errLine = "4";
                okayToProceed = false;
            }
            errLine = "5";
            try
            {

                if (okayToProceed)
                {
                    //print("let's save");
                    errLine = "6";

                    errLine = "10";
                    string hashedShipName = AGextScenario.EditorHashShipName(EditorLogic.fetch.shipNameField.Text, inVAB);
                    errLine = "11";
                    ConfigNode thisVsl = new ConfigNode(hashedShipName);
                    errLine = "12";
                    thisVsl.AddValue("name", EditorLogic.fetch.shipNameField.Text);
                    errLine = "13";
                    thisVsl.AddValue("currentKeyset", CurrentKeySet.ToString());
                    errLine = "14";
                    thisVsl.AddValue("groupNames", SaveGroupNames(""));
                    errLine = "15";
                    thisVsl.AddValue("groupVisibility", SaveGroupVisibility(""));
                    errLine = "16";
                    thisVsl.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
                    errLine = "17";
                    thisVsl.AddValue("DirectActionState", SaveDirectActionState(""));
                    errLine = "17a";
                    UpdateAGXActionGroupNames();
                    try
                    {
                        errLine = "17c";
                        foreach (Part p in EditorLogic.SortedShipList)
                        {
                            errLine = "17d";
                            List<AGXAction> thisPartsActions = new List<AGXAction>();
                            thisPartsActions.AddRange(CurrentVesselActions.FindAll(p2 => p2.prt == p));
                            errLine = "18";
                            if (thisPartsActions.Count > 0)
                            {
                                ConfigNode partTemp = new ConfigNode("PART");
                                errLine = "19";
                                partTemp.AddValue("name", p.name);
                                partTemp.AddValue("vesselID", "0");
                                //partTemp.AddValue("relLocX", (p.transform.position - EditorLogic.RootPart.transform.position).x);
                                //if (!inVAB)
                                //{
                                //    partTemp.AddValue("relLocZ", ((p.transform.position - EditorLogic.RootPart.transform.position).y) * -1f);
                                //    partTemp.AddValue("relLocY", (p.transform.position - EditorLogic.RootPart.transform.position).z);
                                //}
                                //else
                                //{
                                //    partTemp.AddValue("relLocY", (p.transform.position - EditorLogic.RootPart.transform.position).y);
                                //    partTemp.AddValue("relLocZ", (p.transform.position - EditorLogic.RootPart.transform.position).z);
                                //}
                                partTemp.AddValue("relLocX", (EditorLogic.RootPart.transform.InverseTransformPoint(p.transform.position)).x);
                                partTemp.AddValue("relLocY", (EditorLogic.RootPart.transform.InverseTransformPoint(p.transform.position)).y);
                                partTemp.AddValue("relLocZ", (EditorLogic.RootPart.transform.InverseTransformPoint(p.transform.position)).z);
                                errLine = "20";
                                foreach (AGXAction agxAct in thisPartsActions)
                                {
                                    errLine = "21";
                                    partTemp.AddNode(AGextScenario.SaveAGXActionVer2(agxAct));
                                }
                                errLine = "22";

                                thisVsl.AddNode(partTemp);
                                errLine = "23";
                            }
                            // print("part OrgPart "+ p.ConstructID+" " + p.orgPos + " " + p.orgRot);
                        }


                    }
                    catch (Exception e)
                    {
                        print("AGExt No parts to save " + errLine + " " + e);
                    }
                    errLine = "23";
                    if (AGXEditorNode.HasNode(hashedShipName))
                    {
                        errLine = "23";
                        AGXEditorNode.RemoveNode(hashedShipName);
                    }
                    errLine = "24";
                    AGXEditorNode.AddNode(thisVsl);
                    errLine = "25";
                    //AGXBaseNode.RemoveNode("EDITOR");
                    errLine = "26";
                    //AGXBaseNode.AddNode(AGXEditorNode);
                    errLine = "27";
                    // AGXEditorNode.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
                    //print("Saved this node " + AGXEditorNode);
                    errLine = "28";
                }
            }

            catch (Exception e)
            {
                print("AGX EditorSaveToFile FAIL " + errLine + " " + e);
            }
        }

    }
}



