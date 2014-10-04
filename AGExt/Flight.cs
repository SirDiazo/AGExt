using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;


namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AGXFlight : PartModule
    {
        
        //Selected Parts Window Variables
        
        //private Dictionary<int, KeyCode> Save10Keys =  new Dictionary<int, KeyCode>();
        private string LastKeyCode = "";
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
        public static ConfigNode AGExtNode;
        string[] KeySetNames = new string[5];
        //private bool TrapMouse = false;
        private int LastPartCount = 0;
        public static List<AGXAction> CurrentVesselActions;
        //public static List<AGXAction> AllVesselsActions;

        private static Dictionary<int, bool> ActiveGroups;
        private List<KeyCode> ActiveKeys;
        private IButton AGXBtn;
        //public Dictionary<string, ConfigNode> loadedVessels;





        public Rect GroupsWin;
        public bool Trigger;
        //private bool Trigger2;
        //private int Value = 0;
        //private string HexStr;

        private List<BaseAction> PartActionsList;



        //private bool ShipListOk = false;
        Texture2D BtnTexRed = new Texture2D(1, 1);
        Texture2D BtnTexGrn = new Texture2D(1, 1);
        Texture2D ButtonTexture = new Texture2D(64, 64);
        Texture2D ButtonTextureRed = new Texture2D(64, 64);
        Texture2D ButtonTextureGreen = new Texture2D(64, 64);
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

        private static GUISkin AGXSkin;
        public static GUIStyle AGXWinStyle = null;
        //private static GUIStyle TWR1WinStyle = null; //window style
        private static GUIStyle AGXLblStyle = null; //window style
        public static GUIStyle AGXBtnStyle = null; //window style
        private static GUIStyle AGXFldStyle = null; //window style
        //private static GUIStyle AGXScrollStyle = null; //window style
        private bool ShowSettingsWin = false;
        private Rect SettingsWinRect;
        public static bool FlightWinShowKeycodes = true;
        static ConfigNode AGXBaseNode = new ConfigNode();
        public static ConfigNode AGXFlightNode = new ConfigNode();
        public static ConfigNode RootParts = new ConfigNode();
        static ConfigNode AGXEditorNode = new ConfigNode();
        List<AGXPartVesselCheck> partOldVessel = new List<AGXPartVesselCheck>();
        public static bool flightNodeIsLoaded = false;
        static List<Vessel> loadedVessels; //loaded vessels list, add vessels to this on OffRails, remove when OnRails
        bool highlightPartThisFrameSelWin = false;
        bool highlightPartThisFrameActsWin = false;
        Part partToHighlight = null;
        Texture2D PartCenter = new Texture2D(41, 41);
        
        bool showAllPartsList = false; //show list of all parts in group window?
        List<string> showAllPartsListTitles; //list of all parts with actions to show in group window

        bool defaultShowingNonNumeric = false; //are we in non-numeric (abort/brakes/gear/list) mode?
        KSPActionGroup defaultGroupToShow = KSPActionGroup.Abort; //which group is currently selected if showing non-numeric groups
        List<BaseAction> defaultActionsListThisType; //list of default actions showing in group win when non-numeric
        List<BaseAction> defaultActionsListAll; //list of all default actions on vessel, only used in non-numeric mode when going to other mode
        Vector2 groupWinScroll = new Vector2();
        bool highlightPartThisFrameGroupWin = false;
        private bool overrideRootChange = false; //if docking event, do NOT run root change code



        public void RefreshDefaultActionsListType()
        {
            defaultActionsListThisType.Clear();
            foreach (BaseAction act in defaultActionsListAll)
            {
                if ((act.actionGroup & defaultGroupToShow) == defaultGroupToShow)
                {
                    defaultActionsListThisType.Add(act);
                }
            }
        }
       

        public void Start()
        {
            defaultActionsListThisType = new List<BaseAction>(); //initialize list
            defaultActionsListAll = new List<BaseAction>(); //initialize list
            
            //Save10Keys = new Dictionary<int, KeyCode>();
            //foreach (Part p in 
            ActiveKeys = new List<KeyCode>();

            TestWin = new Rect(600, 300, 100, 100);
            RenderingManager.AddToPostDrawQueue(33, AGXOnDraw); //was 0, 33 random number to fix FAR issue
            AGEditorSelectedParts = new List<AGXPart>();
            PartActionsList = new List<BaseAction>();
            ScrollPosSelParts = Vector2.zero;
            ScrollPosSelPartsActs = Vector2.zero;
            ScrollGroups = Vector2.zero;
            CurGroupsWin = Vector2.zero;

            //AGXVsl = new AGXVessel();

            //AGXWinStyle = new GUIStyle(HighLogic.Skin.window);

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
            FlightWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltMainX")), Convert.ToInt32(AGExtNode.GetValue("FltMainY")), 235, 100);
            GroupsInFlightWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("FltMainX")), Convert.ToInt32(AGExtNode.GetValue("FltMainY")), 80, 110);
            ActiveGroups = new Dictionary<int, bool>();
            if(AGExtNode.GetValue("FlightWinShowKeys") == "1")
            {
                FlightWinShowKeycodes = true;
            }
            else
            {
                FlightWinShowKeycodes = false;
            }
            
           // UnbindDefaultKeys();  //obsolete with INputLockManager
            InputLockManager.SetControlLock(ControlTypes.CUSTOM_ACTION_GROUPS, "AGExtControlLock");

            if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
            {


                AGXBtn = ToolbarManager.Instance.add("AGX", "AGXBtn");
                AGXBtn.TexturePath = "Diazo/AGExt/icon_button";
                AGXBtn.ToolTip = "Action Groups Extended";
                AGXBtn.OnClick += (e) =>
                    {
                        if (e.MouseButton == 0)
                        {
                            ShowAGXMod = !ShowAGXMod;
                        }
                        if (e.MouseButton == 1)
                        {
                            //ShowSettingsWin = !ShowSettingsWin;
                            if (ShowSettingsWin)
                            {
                                AGXBtn.Drawable = null;
                                ShowSettingsWin = false;
                            }
                            else
                            {
                                SettingsWindow Settings = new SettingsWindow();
                                AGXBtn.Drawable = Settings;
                                ShowSettingsWin = true;
                            }
                            
                        }
                    };
                //{
                //    ShowAGXMod = !ShowAGXMod;
                //};
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
            //foreach (ModuleAGExtData agData in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
            //{
            //    ShowGroupInFlightNames = agData.LoadShowGroupNames();
            //}
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

            AGXSkin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
            AGXWinStyle = new GUIStyle(AGXSkin.window); 
            AGXLblStyle = new GUIStyle(AGXSkin.label);
            AGXFldStyle = new GUIStyle(AGXSkin.textField);
            AGXFldStyle.fontStyle = FontStyle.Normal;
            //AGXFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);
            AGXFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            AGXLblStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            AGXLblStyle.wordWrap = false;

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
            SettingsWinRect = new Rect(500, 500, 150, 75);
            partOldVessel = new List<AGXPartVesselCheck>();
            //GameEvents.onVesselGoOffRails.Add(VesselOffRails); //triggers when vessel loads at flight scene start also
            //GameEvents.onVesselGoOnRails.Add(VesselOnRails);
            //GameEvents.onPartDestroyed.Add(partDead);
            GameEvents.onPartDie.Add(partDead);
            //GameEvents.onPartExplode.Add(partDead2);
            GameEvents.onPartAttach.Add(DockingEvent);
            
            //AGXBaseNode = AGextScenario.LoadBaseNode();
            AGXFlightNode = new ConfigNode();
            if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg"))
            {
                AGXEditorNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg");
            }
            else
            {
                AGXEditorNode = new ConfigNode("EDITOR");
                AGXEditorNode.AddValue("name", "editor");
            }
            //AGXEditorNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtEditor.cfg"); 
            //AllVesselsActions = new List<AGXAction>();
            loadedVessels = new List<Vessel>();
            LoadCurrentKeyBindings();
            
           
        }

        
        

        public static ConfigNode FlightSaveToFile(ConfigNode origNode)  
        {
           // print("FlightSaveToFile ");
            
            string errLine = "1";
            try
            {
                if (loadFinished)
                {
                    //List<AGXAction> actionsToSave = new List<AGXAction>();
                    //actionsToSave.AddRange(AllVesselsActions.Where(ag => ag.ba != null));
                    //foreach (AGXAction agAct in actionsToSave)
                    //{
                    //    print("FlightSave " + agAct.ba.name + " " + agAct.ba.listParent.part.ConstructID);
                    //}
                    //if(loadedVessels.Contains(FlightGlobals.ActiveVessel))
                    //{
                    ConfigNode thisVsl = new ConfigNode(FlightGlobals.ActiveVessel.id.ToString());
                    ConfigNode thisRootPart = new ConfigNode(FlightGlobals.ActiveVessel.rootPart.flightID.ToString()); //copy values to root part save for future undockings
                    thisVsl.AddValue("name", FlightGlobals.ActiveVessel.vesselName);
                    thisRootPart.AddValue("name", thisVsl.GetValue("name")); //get the value from confignode rather then running all the calculations again, i think it's less processing
                    errLine = "13";
                    thisVsl.AddValue("currentKeyset", CurrentKeySet.ToString());
                    thisRootPart.AddValue("currentKeyset", thisVsl.GetValue("currentKeyset"));
                    errLine = "14";
                    thisVsl.AddValue("groupNames", SaveGroupNames(""));
                    thisRootPart.AddValue("groupNames", thisVsl.GetValue("groupNames"));
                    errLine = "15";
                    thisVsl.AddValue("groupVisibility", SaveGroupVisibility(""));
                    thisRootPart.AddValue("groupVisibility", thisVsl.GetValue("groupVisibility"));
                    errLine = "16";
                    thisVsl.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
                    thisRootPart.AddValue("groupVisibilityNames", thisVsl.GetValue("groupVisibilityNames"));
                    errLine = "17";

                        if(RootParts.HasNode(FlightGlobals.ActiveVessel.rootPart.flightID.ToString()))
                        {
                            RootParts.RemoveNode(FlightGlobals.ActiveVessel.rootPart.flightID.ToString());
                        }
                        RootParts.AddNode(thisRootPart);
                        RootParts.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtRootParts.cfg");

                        


                    foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                    {
                        List<AGXAction> thisPartsActions = new List<AGXAction>();
                        thisPartsActions.AddRange(CurrentVesselActions.FindAll(p2 => p2.ba.listParent.part == p));
                        errLine = "18";
                        if (thisPartsActions.Count > 0)
                        {
                            //print("acts count " + thisPartsActions.Count);
                            ConfigNode partTemp = new ConfigNode("PART");
                            errLine = "19";
                            partTemp.AddValue("name", p.vessel.name);
                            partTemp.AddValue("vesselID", p.vessel.id);
                            partTemp.AddValue("relLocX", FlightGlobals.ActiveVessel.rootPart.transform.InverseTransformPoint(p.transform.position).x);
                            partTemp.AddValue("relLocY", FlightGlobals.ActiveVessel.rootPart.transform.InverseTransformPoint(p.transform.position).y);
                            partTemp.AddValue("relLocZ", FlightGlobals.ActiveVessel.rootPart.transform.InverseTransformPoint(p.transform.position).z);
                            errLine = "20";
                            foreach (AGXAction agxAct in thisPartsActions)
                            {
                                //print("acts countb " + thisPartsActions.Count);
                                errLine = "21";
                                partTemp.AddNode(AGextScenario.SaveAGXActionVer2(agxAct));
                            }
                            errLine = "22";

                            thisVsl.AddNode(partTemp);
                            errLine = "23";
                        }
                    }
                    if (AGXFlightNode.HasNode(FlightGlobals.ActiveVessel.id.ToString()))
                    {
                        AGXFlightNode.RemoveNode(FlightGlobals.ActiveVessel.id.ToString());
                    }
                    //print("save node " + thisVsl);
                    AGXFlightNode.AddNode(thisVsl);
                    return AGXFlightNode;
            

                }
                else
                {
                    return origNode;
                }
            }
            catch (Exception e)
            {
                print("FlightSaveToFile error " + errLine + " " + e);
                return origNode;
            }
        }

        public void RefreshCurrentActions()
    {
        //CurrentVesselActions.Clear();
        //print("cleared " + CurrentVesselActions.Count());

        //foreach (AGXAction agAct in AllVesselsActions)
        //{
        //    if (FlightGlobals.ActiveVessel == agAct.ba.listParent.part.vessel)
        //    {
        //        CurrentVesselActions.Add(agAct);
        //        //print("Added " + agAct.ba.listParent.part.ConstructID + " " + agAct.ba.listParent.part.vessel.id.ToString() + " " + FlightGlobals.ActiveVessel.id.ToString());
        //    }
        //}
        CalculateActiveActions();
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

        //public void VesselOffRails(Vessel vsl) //load vessels here
        //{
        //    print("Vessel off rails! " + vsl.vesselName);
        //    ConfigNode vslNode = new ConfigNode();
        //    bool checkIsVab = true;
        //    try
        //    {
        //        if (vsl.landedAt == "Runway")
        //        {
        //            //print("Runway found");
        //            checkIsVab = false;
        //        }
        //        else
        //        {
        //            //print("runway not found");
        //            checkIsVab = true;
        //        }

        //    }
        //    catch
        //    {
        //        //print("runway iffy");
        //        checkIsVab = true;
        //    }
        //    if (AGXFlightNode.HasNode(vsl.id.ToString()))
        //    {
        //        //print("Vessel off rails! case 1 " + vsl.vesselName); 
        //        vslNode = AGXFlightNode.GetNode(vsl.id.ToString());
             
        //    }
        //    else if(AGXEditorNode.HasNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName,checkIsVab)))
        //    {
        //        //print("Vessel off rails! case2 " + vsl.vesselName);
        //        vslNode = AGXEditorNode.GetNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName,checkIsVab));
        //        vslNode.name = vsl.id.ToString();
        //        AGXFlightNode.AddNode(vslNode);
        //    }
        //    else if (AGXEditorNode.HasNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName, !checkIsVab)))
        //    {
        //        //print("Vessel off rails! case3 " + vsl.vesselName);
        //        vslNode = AGXEditorNode.GetNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName,!checkIsVab));
        //        vslNode.name = vsl.id.ToString();
        //        AGXFlightNode.AddNode(vslNode);
        //    }
        //    else
        //    {
        //       // print("Vessel off rails! case4 " + vsl.vesselName);
        //        vslNode = new ConfigNode(vsl.id.ToString());
        //        vslNode.AddValue("name", vsl.vesselName);
        //        vslNode.AddValue("currentKeyset", "1");
        //        vslNode.AddValue("groupNames", "");
        //        vslNode.AddValue("groupVisibility", "1011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111");
        //        vslNode.AddValue("groupVisibilityNames", "Group 1‣Group 2‣Group 3‣Group 4‣Group 5");
        //        AGXFlightNode.AddNode(vslNode);
        //    }



        //    foreach (ConfigNode prtNode in vslNode.nodes)
        //            {
        //                Vector3 partLoc = new Vector3((float)Convert.ToDouble(prtNode.GetValue("relLocX")), (float)Convert.ToDouble(prtNode.GetValue("relLocY")), (float)Convert.ToDouble(prtNode.GetValue("relLocZ")));
        //                float partDist = 100f;
        //                Part gamePart = new Part();
        //                foreach (Part p in vsl.parts) //do a distance compare check, floats do not guarantee perfect decimal accuray so use part with least distance, should be zero distance in most cases
        //                {
        //                    float thisPartDist = Vector3.Distance(partLoc, p.orgPos);
        //                    if (thisPartDist < partDist)
        //                    {
        //                        gamePart = p;
        //                        partDist = thisPartDist;
        //                    }
        //                }
        //                foreach (ConfigNode actNode in prtNode.nodes)
        //                {
        //                    //print("node " + actNode + " " + gamePart.ConstructID);
        //                    AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(actNode, gamePart);
        //                    //print("act to add " + actToAdd.ba);
        //                    if (actToAdd.ba != null)
        //                    {
        //                        AllVesselsActions.Add(actToAdd);
        //                    }
        //                }
        //            }

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

        //    //errLine = "16";
        //    // string AddGroup = "";
        //    List<BaseAction> partAllActions = new List<BaseAction>(); //is all vessel actions, copy pasting code
        //    foreach (Part p in vsl.parts)
        //    {
        //        partAllActions.AddRange(p.Actions);
        //        foreach (PartModule pm in p.Modules)
        //        {
        //            partAllActions.AddRange(pm.Actions);
        //        }

        //       // print("part orgpos " + p.ConstructID+ " "  + p.orgPos + " " + p.orgRot);
        //    }

        //    foreach (BaseAction baLoad in partAllActions)
        //    {
        //        foreach (KSPActionGroup agrp in CustomActions)
        //        {

        //            if ((baLoad.actionGroup & agrp) == agrp)
        //            {
        //               // errLine = "17";
        //                ////AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) + 1).ToString("000") + baLoad.guiName;
        //                //partAGActions2.Add(new AGXAction() { group = CustomActions.IndexOf(agrp) + 1, prt = this.part, ba = baLoad, activated = false });
        //                AGXAction ToAdd = new AGXAction() { prt = baLoad.listParent.part, ba = baLoad, group = CustomActions.IndexOf(agrp) + 1, activated = false };
        //                List<AGXAction> Checking = new List<AGXAction>();
        //                Checking.AddRange(AllVesselsActions);
        //                Checking.RemoveAll(p => p.group != ToAdd.group);

        //                Checking.RemoveAll(p => p.prt != ToAdd.prt);

        //                Checking.RemoveAll(p => p.ba != ToAdd.ba);



        //                if (Checking.Count == 0)
        //                {

        //                    AllVesselsActions.Add(ToAdd);

        //                }
        //            }
        //        }
        //       // errLine = "18";
        //    }


        //    if (vsl == FlightGlobals.ActiveVessel)
        //    {
        //        CurrentKeySet = Convert.ToInt32(vslNode.GetValue("currentKeyset"));
        //        LoadCurrentKeyBindings();
        //        LoadGroupNames(vslNode.GetValue("groupNames"));
        //        LoadGroupVisibility(vslNode.GetValue("groupVisibility"));
        //        LoadGroupVisibilityNames(vslNode.GetValue("groupVisibilityNames"));
        //        loadFinished = true;
        //    }


        //    RefreshCurrentActions();

        //    foreach (Part p in vsl.Parts) //add parts to checking list to see if the vessel cahnges (undock, break, etc.)
        //    {
        //        partOldVessel.Add(new AGXPartVesselCheck() { prt = p, pVsl = vsl });
        //    }
        //    loadedVessels.Add(vsl); //add this vessel to loaded vessels list
        //}

        //public void VesselOnRails(Vessel vsl)
        //{
        //    print("Vessel on rails! " + vsl.name );
        //    partOldVessel.RemoveAll(vsl2 => vsl2.pVsl == vsl);
        //    List<AGXAction> UnloadVslActionsCheck = new List<AGXAction>();
        //    UnloadVslActionsCheck.AddRange(AllVesselsActions.Where(agx => agx.ba.listParent.part.vessel == vsl));
        //    //print("count check " + UnloadVslActionsCheck.Count + vsl.id.ToString());
        //    //foreach (AGXAction agact in UnloadVslActionsCheck)
        //    //{
        //    //    print("actionunload " + agact.ba.listParent.part.vessel.id.ToString() + " " + agact.ba.listParent.part.ConstructID);
        //    //}
        //    if (UnloadVslActionsCheck.Count > 0)
        //    {
        //        ConfigNode vslToUnload = new ConfigNode(vsl.id.ToString());
        //        if(AGXFlightNode.HasNode(vsl.id.ToString()))
        //        {
        //            vslToUnload = AGXFlightNode.GetNode(vsl.id.ToString());
        //            vslToUnload.RemoveNodes("PART");
        //            AGXFlightNode.RemoveNode(vsl.id.ToString());
        //        }

        //        if (vsl == FlightGlobals.ActiveVessel)
        //        {
        //            if(vslToUnload.HasValue("name"))
        //            {
        //                vslToUnload.RemoveValue("name");
        //            }
        //            vslToUnload.AddValue("name",FlightGlobals.ActiveVessel.vesselName);
        //            if (vslToUnload.HasValue("currentKeyset"))
        //            {
        //                vslToUnload.RemoveValue("currentKeyset");
        //            }
        //            vslToUnload.AddValue("currentKeyset", CurrentKeySet.ToString());
        //            if (vslToUnload.HasValue("groupNames"))
        //            {
        //                vslToUnload.RemoveValue("groupNames");
        //            }
        //            vslToUnload.AddValue("groupNames", SaveGroupNames(""));
        //            if (vslToUnload.HasValue("groupVisibility"))
        //            {
        //                vslToUnload.RemoveValue("groupVisibility");
        //            }
        //            vslToUnload.AddValue("groupVisibility", SaveGroupVisibility(""));
        //            if (vslToUnload.HasValue("groupVisibilityNames"))
        //            {
        //                vslToUnload.RemoveValue("groupVisibilityNames");
        //            }
        //            vslToUnload.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
        //        }

        //        foreach (Part p in vsl.Parts)
        //        {
        //            List<AGXAction> thisPartsActions = new List<AGXAction>();
        //            thisPartsActions.AddRange(UnloadVslActionsCheck.FindAll(p2 => p2.ba.listParent.part == p));
        //            //errLine = "18";
        //            if (thisPartsActions.Count > 0)
        //            {
        //                ConfigNode partTemp = new ConfigNode("PART");
        //                //errLine = "19";
        //                partTemp.AddValue("name", p.vessel.vesselName);
        //                partTemp.AddValue("vesselID", p.vessel.id);
        //                partTemp.AddValue("relLocX", p.orgPos.x);
        //                partTemp.AddValue("relLocY", p.orgPos.y);
        //                partTemp.AddValue("relLocZ", p.orgPos.z);
        //                //errLine = "20";
        //                foreach (AGXAction agxAct in thisPartsActions)
        //                {
        //                    //errLine = "21";
        //                    partTemp.AddNode(AGextScenario.SaveAGXActionVer2(agxAct));
        //                }
        //               // errLine = "22";

        //                vslToUnload.AddNode(partTemp);
        //                //errLine = "23";
        //            }
        //        }
        //        AGXFlightNode.AddNode(vslToUnload);
        //    }
        //    AllVesselsActions.RemoveAll(act2 => act2.ba.listParent.part.vessel == vsl);
        //    loadedVessels.Remove(vsl); //remove vessel from loaded vessels list

        //}

        //public void LoadEverything()
        //{
        //    //foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
        //    //{
        //    //    CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));
        //    //    //ActivatedGroups = (string)pm.Fields.GetValue("AGXActivated");

        //    //}
        //    //if (CurrentKeySet == 0)
        //    //{
        //    //    CurrentKeySet = 1;
        //    //}

        //    //LoadGroupNames();
        //    LoadCurrentKeyBindings();
        //    //LoadActionGroups();
        //    //LoadGroupVisibility();
        //    //foreach (ModuleAGExtData agData in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
        //    //{
        //    //    ShowGroupInFlightNames = agData.LoadShowGroupNames();
        //    //}
        //    //loadFinished = true;
        //}

      


        private void UnbindDefaultKeys() //not called any more
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
            GameEvents.onPartAttach.Remove(DockingEvent);
            flightNodeIsLoaded = false;
            loadFinished = false;
            InputLockManager.RemoveControlLock("AGExtControlLock");

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
        ConfigNode thisRootPart = new ConfigNode(FlightGlobals.ActiveVessel.rootPart.flightID.ToString()); //copy values to root part save for future undockings
        thisRootPart.AddValue("name", FlightGlobals.ActiveVessel.vesselName);
        //thisRootPart.AddValue("name", thisVsl.GetValue("name")); //get the value from confignode rather then running all the calculations again, i think it's less processing
        //errLine = "13";
        thisRootPart.AddValue("currentKeyset", CurrentKeySet.ToString());
        //thisRootPart.AddValue("currentKeyset", thisVsl.GetValue("currentKeyset"));
        //errLine = "14";
        thisRootPart.AddValue("groupNames", SaveGroupNames(""));
        //thisRootPart.AddValue("groupNames", thisVsl.GetValue("groupNames"));
        //errLine = "15";
        thisRootPart.AddValue("groupVisibility", SaveGroupVisibility(""));
        //thisRootPart.AddValue("groupVisibility", thisVsl.GetValue("groupVisibility"));
        //errLine = "16";
        thisRootPart.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
        if (RootParts.HasNode(FlightGlobals.ActiveVessel.rootPart.flightID.ToString()))
        {
            RootParts.RemoveNode(FlightGlobals.ActiveVessel.rootPart.flightID.ToString());
        }
        RootParts.AddNode(thisRootPart);
        RootParts.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtRootParts.cfg");
        //thisRootPart.AddValue("groupVisibilityNames", thisVsl.GetValue("groupVisibilityNames"));
        //SaveCurrentVesselActions();
        //foreach (Part p in FlightGlobals.ActiveVessel.parts)
        //{
        //    foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
        //    {
        //        ModuleAGExtData AGData = (ModuleAGExtData)pm;
        //        AGData.AGXNames = SaveGroupNames(p, AGData.AGXNames);
        //        AGData.AGXGroupStates = SaveGroupVisibility(p, AGData.AGXGroupStates);
        //        AGData.AGXGroupStateNames = SaveGroupVisibilityNames(p, AGData.AGXGroupStateNames);
        //    }
        //}

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
            //if (ShowSettingsWin)
            //{
            //    SettingsWinRect = GUI.Window(673467780, SettingsWinRect, AGXMethods.SettingsWindow, "AGX Settings", AGXWinStyle);
            //}

            if (ShowAGXMod)
            {
                if (ShowAGXFlightWin)
                {
                    GroupsInFlightWin.x = FlightWin.x + 235;
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
            if (highlightPartThisFrameActsWin || highlightPartThisFrameSelWin || highlightPartThisFrameGroupWin)
            {
                //Camera edCam = EditorCamera.fe
                // print("mouse over");
                //print("screen pos " + EditorLogic.fetch.editorCamera.WorldToScreenPoint(partToHighlight.transform.position));
                //print("orgpos" + partToHighlight.);
                Vector3 partScreenPos = FlightCamera.fetch.mainCamera.WorldToScreenPoint(partToHighlight.transform.position);
                    //EditorLogic.fetch.editorCamera.WorldToScreenPoint(partToHighlight.transform.position);
                Rect partCenterWin = new Rect(partScreenPos.x - 20, (Screen.height - partScreenPos.y) - 20, 41, 41);
                partCenterWin = GUI.Window(673767790, partCenterWin, PartTarget, "", AGXWinStyle);


            }
        }

        public void PartTarget(int WindowID)
        {
            GUI.DrawTexture(new Rect(0, 0, 41, 41), PartCenter);
        }

        public static void ActivateActionGroup(int group)
        {

            Dictionary<int, KSPActionGroup> CustomActions = new Dictionary<int, KSPActionGroup>();
            CustomActions.Add(1, KSPActionGroup.Custom01); //how do you add a range from enum?
            CustomActions.Add(2, KSPActionGroup.Custom02);
            CustomActions.Add(3, KSPActionGroup.Custom03);
            CustomActions.Add(4, KSPActionGroup.Custom04);
            CustomActions.Add(5, KSPActionGroup.Custom05);
            CustomActions.Add(6, KSPActionGroup.Custom06);
            CustomActions.Add(7, KSPActionGroup.Custom07);
            CustomActions.Add(8, KSPActionGroup.Custom08);
            CustomActions.Add(9, KSPActionGroup.Custom09);
            CustomActions.Add(10, KSPActionGroup.Custom10);
        
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
                    if (group <= 10)
                    {
                        FlightGlobals.ActiveVessel.ActionGroups[CustomActions[group]] = false;
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
                    if (group <= 10)
                    {
                        FlightGlobals.ActiveVessel.ActionGroups[CustomActions[group]] = true;
                    }
                  
                    
                }
                //ModuleAGExtData pmAgx = agAct.ba.listParent.part.Modules.OfType<ModuleAGExtData>().First<ModuleAGExtData>();
                //pmAgx.partAGActions.Clear();
                //pmAgx.partAGActions.AddRange(CurrentVesselActions.Where(agp => agp.prt == agAct.ba.listParent.part));
                //pmAgx.AGXData = pmAgx.SaveActionGroups();
            }
            CalculateActionsState();
        }

        public void GroupsInFlightWindow(int WindowID)
        {
            if (GUI.Button(new Rect(5, 5, 70, 20), ShowGroupInFlightNames[1],AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 1;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 25, 70, 20), ShowGroupInFlightNames[2], AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 2;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 45, 70, 20), ShowGroupInFlightNames[3], AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 3;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 65, 70, 20), ShowGroupInFlightNames[4], AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 4;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
            if (GUI.Button(new Rect(5, 85, 70, 20), ShowGroupInFlightNames[5], AGXBtnStyle))
            {
                ShowGroupInFlightCurrent = 5;
                CalculateActionsToShow();
                ShowGroupsInFlightWindow = false;
            }
        }

        public void FlightWindow(int WindowID)
        {
            HighLogic.Skin.scrollView.normal.background = null;
            AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
            if (GUI.Button(new Rect(160, 5, 70, 20), ShowGroupInFlightNames[ShowGroupInFlightCurrent], AGXBtnStyle))
            {
                
                ShowGroupsInFlightWindow = !ShowGroupsInFlightWindow;
            }
            

            if (GUI.Button(new Rect(5, 5, 75, 20), "Edit", AGXBtnStyle))
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
            GUI.Box(new Rect(5, 25, 210, Math.Min(410, 10 + (20 * Math.Max(1, ActiveActionsStateToShow.Count)))), "", AGXBtnStyle);
            //FlightWinScroll = GUI.BeginScrollView(new Rect(10, 30, 200, Math.Min(400,20+(20*(ActiveActions.Count-1)))), FlightWinScroll, new Rect(0, 0, 180, (20 * (ActiveActions.Count))));
            FlightWinScroll = GUI.BeginScrollView(new Rect(10, 30, 220, Math.Min(400, 20 + (20 * (ActiveActionsStateToShow.Count - 1)))), FlightWinScroll, new Rect(0, 0, 180, (20 * (ActiveActionsStateToShow.Count))));
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

                    AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
                    //if (GUI.Button(new Rect(0, 0 + (20 * (i2 - 1)), 110, 20), ActiveActions.ElementAt((i2 - 1)) + ": " + AGXguiNames[ActiveActions.ElementAt((i2 - 1))]))
                    if (GUI.Button(new Rect(0, 0 + (20 * (i2 - 1)), FlightWinShowKeycodes? 110:200, 20), ActiveActionsStateToShow.ElementAt((i2 - 1)).group + ": " + AGXguiNames[ActiveActionsStateToShow.ElementAt((i2 - 1)).group], AGXBtnStyle))
                    {
                        
                        //ActivateActionGroup(ActiveActions.ElementAt(i2 - 1));
                        ActivateActionGroup(ActiveActionsStateToShow.ElementAt(i2 - 1).group);
                    }
                    AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                    //if (GUI.Button(new Rect(110, 0 + (20 * (i2 - 1)), 70, 20), AGXguiKeys[ActiveActions.ElementAt((i2 - 1))].ToString()))
                    if (FlightWinShowKeycodes)
                    {
                        if (GUI.Button(new Rect(110, 0 + (20 * (i2 - 1)), 90, 20), AGXguiKeys[ActiveActionsStateToShow.ElementAt((i2 - 1)).group].ToString(), AGXBtnStyle))
                        {

                            //ActivateActionGroup(ActiveActions.ElementAt(i2 - 1));
                            ActivateActionGroup(ActiveActionsStateToShow.ElementAt(i2 - 1).group);
                        }
                    }
                        GUI.contentColor = TxtClr;
                }
                
            }
                GUI.EndScrollView();
            //if(ActiveActions.Count ==0)

                if (ActiveActionsState.Count == 0)
                {

                    GUI.Label(new Rect(40, 30, 150, 20), "No actions available", AGXLblStyle);
                }
                else if(ActiveActionsStateToShow.Count ==0)
                {
                    GUI.Label(new Rect(40, 30, 150, 20), "Actions in other group", AGXLblStyle);
                }
          
                
            GUI.DragWindow();
       
        }
        //public void SaveCurrentVesselActions()
        //{
        //    string errLine = "1";
        //    try
        //    {
        //        errLine = "2";
        //        foreach (Part p in FlightGlobals.ActiveVessel.Parts)
        //        {
        //            errLine = "3";
        //            foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
        //            {
        //                errLine = "4";
        //                agpm.partAGActions.Clear();
        //                errLine = "5";
        //                agpm.partAGActions.AddRange(CurrentVesselActions.Where(agp => agp.prt == p));
        //                errLine = "6";
        //                agpm.AGXData = agpm.SaveActionGroups();
        //                errLine = "7";
        //            }
        //        }
        //        errLine = "8";
        //        CalculateActiveActions();
        //    }
        //    catch (Exception e)
        //    {
        //        print("AGX Fail (SaveCurrentVesselActions) " + errLine + " " + e);
        //    }
        //}
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
            HighLogic.Skin.scrollView.normal.background = null;
            List<AGXAction> ThisGroupActions = new List<AGXAction>();
            ThisGroupActions.AddRange(CurrentVesselActions.Where(p => p.group == AGXCurActGroup));
            GUI.Box(new Rect(5, 25, 310, 110), "");
            CurGroupsWin = GUI.BeginScrollView(new Rect(10, 30, 330, 100), CurGroupsWin, new Rect(0, 0, 310, Math.Max(100, 0 + (20 * (ThisGroupActions.Count)))));
            int RowCnt = new int();
            RowCnt = 1;
            highlightPartThisFrameActsWin = false;
            if (defaultShowingNonNumeric)
            {
                GUI.Label(new Rect(10, 30, 274, 30), "Abort/Brake/Gear/Lights actions\nshow in Groups window", AGXLblStyle);
            }
            else
            {
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

                        AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
                        if (GUI.Button(new Rect(0, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).group.ToString() + ": " + AGXguiNames[ThisGroupActions.ElementAt(RowCnt - 1).group], AGXBtnStyle))
                        {
                            CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                            RefreshCurrentActions();
                            ////BaseAction actToDel = ThisGroupActions.ElementAt(RowCnt - 1).ba;
                            ////int groupToDel = ThisGroupActions.ElementAt(RowCnt - 1).group;
                            ////List<AGXAction> actionsToDel = new List<AGXAction>();
                            ////actionsToDel.AddRange(AllVesselsActions);
                            ////actionsToDel.RemoveAll(gp => gp.group != groupToDel);
                            ////actionsToDel.RemoveAll(gp => gp.ba != actToDel);
                            ////if (actionsToDel.Count > 0)
                            ////{
                            ////    foreach (AGXAction agAct in actionsToDel)
                            ////    {
                            ////        print("del check " + agAct.ba.name + " " + agAct.group);
                            ////        AllVesselsActions.Remove(agAct);
                            ////        RefreshCurrentActions();
                            ////    }
                            ////}
                            //int ToDel = 0;
                            //foreach (AGXAction AGXToRemove in CurrentVesselActions)
                            //{

                            //    if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                            //    {

                            //        //CurrentVesselActions.RemoveAt(ToDel);
                            //        AGXAction delThis = CurrentVesselActions.ElementAt(ToDel);
                            //        AllVesselsActions.Remove(delThis);
                            //        RefreshCurrentActions();
                            //        goto BreakOutA;
                            //    }
                            //    ToDel = ToDel + 1;
                            //}
                            //BreakOutA:
                            //SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                        }

                        if (GUI.Button(new Rect(100, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).prt.partInfo.title, AGXBtnStyle))
                        {
                            CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                            RefreshCurrentActions();
                            //    int ToDel = 0;
                            //    foreach (AGXAction AGXToRemove in CurrentVesselActions)
                            //    {

                            //        if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                            //        {

                            //            //CurrentVesselActions.RemoveAt(ToDel);
                            //            AGXAction delThis = CurrentVesselActions.ElementAt(ToDel);
                            //            AllVesselsActions.Remove(delThis);
                            //            RefreshCurrentActions();
                            //            goto BreakOutB;
                            //        }
                            //        ToDel = ToDel + 1;
                            //    }
                            //BreakOutB:
                            // SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                        }
                        try
                        {
                            if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).ba.guiName, AGXBtnStyle))
                            {
                                CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                                RefreshCurrentActions();
                                //    int ToDel = 0;
                                //    foreach (AGXAction AGXToRemove in CurrentVesselActions)
                                //    {

                                //        if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                //        {

                                //            //CurrentVesselActions.RemoveAt(ToDel);
                                //            AGXAction delThis = CurrentVesselActions.ElementAt(ToDel);
                                //            AllVesselsActions.Remove(delThis);
                                //            RefreshCurrentActions();
                                //            goto BreakOutC;
                                //        }
                                //        ToDel = ToDel + 1;
                                //    }
                                //BreakOutC:
                                //SaveCurrentVesselActions();
                                if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                                {
                                    RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                                }
                            }
                        }
                        catch
                        {
                            if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), "Error", AGXBtnStyle))
                            {
                                CurrentVesselActions.RemoveAll(ag => ag.group == ThisGroupActions.ElementAt(RowCnt - 1).group & ag.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba);
                                RefreshCurrentActions();
                                //    int ToDel = 0;
                                //    foreach (AGXAction AGXToRemove in CurrentVesselActions)
                                //    {

                                //        if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                //        {

                                //            //CurrentVesselActions.RemoveAt(ToDel);
                                //            AGXAction delThis = CurrentVesselActions.ElementAt(ToDel);
                                //            AllVesselsActions.Remove(delThis);
                                //            RefreshCurrentActions();
                                //            goto BreakOutD;
                                //        }
                                //        ToDel = ToDel + 1;
                                //    }
                                //BreakOutD:
                                //SaveCurrentVesselActions();
                                if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                                {
                                    RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                                }
                            }
                        }

                        AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                        RowCnt = RowCnt + 1;
                    }
                }
                else
                {
                    TextAnchor TxtAnch5 = new TextAnchor();

                    TxtAnch5 = GUI.skin.label.alignment;

                    AGXLblStyle.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(new Rect(10, 30, 274, 30), "No actions", AGXLblStyle);
                    AGXLblStyle.alignment = TextAnchor.MiddleLeft;
                }
            }
                GUI.EndScrollView();
            
            GUI.DragWindow();
        }
       
      
        public void KeySetWindow(int WindowID)
        {



            GUI.DrawTexture(new Rect(6, (CurrentKeySet * 25) + 1, 68, 18), BtnTexGrn);

            if (GUI.Button(new Rect(5, 25, 70, 20), "Select 1:", AGXBtnStyle))
            {
                
                SaveCurrentKeyBindings();
               
                CurrentKeySet = 1;
               
                //SaveCurrentKeySet();
                //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                //{
                //    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                //    {
                //        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                //        {
                //            pm.AGXKeySet=CurrentKeySet.ToString();
                //        }
                //    }
                //}
                LoadCurrentKeyBindings();
               
            }
            KeySetNames[0] = GUI.TextField(new Rect(80, 25, 100, 20), KeySetNames[0]);

            if (GUI.Button(new Rect(5, 50, 70, 20), "Select 2:", AGXBtnStyle))
            {

                SaveCurrentKeyBindings();
                CurrentKeySet = 2;
                //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                //{
                //    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                //    {
                //        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                //        {
                //            pm.AGXKeySet=CurrentKeySet.ToString();
                //        }
                //    }
                //}
                LoadCurrentKeyBindings();
            }
            KeySetNames[1] = GUI.TextField(new Rect(80, 50, 100, 20), KeySetNames[1]);
            if (GUI.Button(new Rect(5, 75, 70, 20), "Select 3:", AGXBtnStyle))
            {

                SaveCurrentKeyBindings();
                CurrentKeySet = 3;
                //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                //{
                //    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                //    {
                //        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                //        {
                //            pm.AGXKeySet=CurrentKeySet.ToString();
                //        }
                //    }
                //}
                LoadCurrentKeyBindings();
            }
            KeySetNames[2] = GUI.TextField(new Rect(80, 75, 100, 20), KeySetNames[2]);
            if (GUI.Button(new Rect(5, 100, 70, 20), "Select 4:", AGXBtnStyle))
            {
                SaveCurrentKeyBindings();
                CurrentKeySet = 4;
                //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                //{
                //    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                //    {
                //        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                //        {
                //            pm.AGXKeySet=CurrentKeySet.ToString();
                //        }
                //    }
                //}
                LoadCurrentKeyBindings();
            }
            KeySetNames[3] = GUI.TextField(new Rect(80, 100, 100, 20), KeySetNames[3]);
            if (GUI.Button(new Rect(5, 125, 70, 20), "Select 5:", AGXBtnStyle))
            {
                SaveCurrentKeyBindings();
                CurrentKeySet = 5;
                //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                //{
                //    if (p.missionID == FlightGlobals.ActiveVessel.rootPart.missionID)
                //    {
                //        foreach(ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                //        {
                //            pm.AGXKeySet=CurrentKeySet.ToString();
                //        }
                //    }
                //}
                LoadCurrentKeyBindings();
            }
            KeySetNames[4] = GUI.TextField(new Rect(80, 125, 100, 20), KeySetNames[4]);

            Color TxtClr3 = GUI.contentColor;
            GUI.contentColor = new Color(0.5f, 1f, 0f,1f);
                AGXLblStyle.fontStyle = FontStyle.Bold;
                TextAnchor TxtAnc = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(5, 145, 175, 25), "Actiongroup Groups",AGXLblStyle);
                AGXLblStyle.fontStyle = FontStyle.Normal;
                AGXLblStyle.alignment = TextAnchor.MiddleLeft;
                GUI.contentColor = TxtClr3;

            GUI.DrawTexture(new Rect(6, (ShowGroupInFlightCurrent * 25) + 141, 68, 18), BtnTexGrn);
            if (GUI.Button(new Rect(5, 165, 70, 20), "Group 1:", AGXBtnStyle))
                {
                     ShowGroupInFlightCurrent = 1;
                }
                ShowGroupInFlightNames[1] = GUI.TextField(new Rect(80, 165, 100, 20), ShowGroupInFlightNames[1]);

                if (GUI.Button(new Rect(5, 190, 70, 20), "Group 2:", AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 2;
                }
                ShowGroupInFlightNames[2] = GUI.TextField(new Rect(80, 190, 100, 20), ShowGroupInFlightNames[2]);

                if (GUI.Button(new Rect(5, 215, 70, 20), "Group 3:", AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 3;
                }
                ShowGroupInFlightNames[3] = GUI.TextField(new Rect(80, 215, 100, 20), ShowGroupInFlightNames[3]);

                if (GUI.Button(new Rect(5, 240, 70, 20), "Group 4:", AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 4;
                }
                ShowGroupInFlightNames[4] = GUI.TextField(new Rect(80, 240, 100, 20), ShowGroupInFlightNames[4]);

                if (GUI.Button(new Rect(5, 265, 70, 20), "Group 5:", AGXBtnStyle))
                {
                    ShowGroupInFlightCurrent = 5;
                }
                ShowGroupInFlightNames[5] = GUI.TextField(new Rect(80, 265, 100, 20), ShowGroupInFlightNames[5]);


                if (GUI.Button(new Rect(5, 300, 175, 30), "Close Window", AGXBtnStyle))
            {

                AGExtNode.SetValue("KeySetName1", KeySetNames[0]);
                AGExtNode.SetValue("KeySetName2", KeySetNames[1]);
                AGExtNode.SetValue("KeySetName3", KeySetNames[2]);
                AGExtNode.SetValue("KeySetName4", KeySetNames[3]);
                AGExtNode.SetValue("KeySetName5", KeySetNames[4]);
                CurrentKeySetName = KeySetNames[CurrentKeySet - 1];
                AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                //foreach (ModuleAGExtData pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
                //     {
                //         pm.AGXGroupStates = SaveGroupVisibility(FlightGlobals.ActiveVessel.rootPart, pm.AGXGroupStates);
                //         pm.AGXGroupStateNames = SaveGroupVisibilityNames(FlightGlobals.ActiveVessel.rootPart, pm.AGXGroupStates);
                //     }
                ShowKeySetWin = false;
                ShowSelectedWin = true;
            }

            GUI.DragWindow();
        }

        public static string SaveCurrentKeySet(Part p, string curKeySet)
        {
            string errLine = "1";
            bool OkayToSave = true;
            try
            {
                errLine = "2";
                try
                {
                    if (p.missionID != FlightGlobals.ActiveVessel.rootPart.missionID)
                    {
                        OkayToSave = false;
                    }
                }
                catch(Exception e)
                {
                    print("AGX Save KeySet FAIL! (SaveCurrentKeySet) " + errLine + " " + e);
                }

                if (OkayToSave)
                {
                    errLine = "3";
                    return CurrentKeySet.ToString();

                }
                

                else
                {
                    errLine = "4";
                    return curKeySet;
                }
                errLine = "5";
            }
            catch (Exception e)
            {
                print("AGX Save KeySet FAIL! (SaveCurrentKeySet) " + errLine + " " + e);
                return curKeySet;
            }
            
            

          
        }

        //public void LoadCurrentKeySet()
        //{

           
        //        foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
        //        {

                  
        //            CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));

        //        }
         
            
          
        //    if (CurrentKeySet >= 1 && CurrentKeySet <= 5)
        //    {
          
        //    }
        //    else
        //    {
        //        CurrentKeySet = 1;
              
        //    }
           
       
        //    CurrentKeySetName = AGExtNode.GetValue("KeySetName" + CurrentKeySet);

        //}

        public void LoadCurrentKeyBindings()
        {


            
            String LoadString = AGExtNode.GetValue("KeySet" + CurrentKeySet.ToString());
            //print("Keyset load " + CurrentKeySet + " " + LoadString);

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
            //print("Saving current keybinds");
       
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
            if (GUI.Button(new Rect(5, 3, 100, 25), "None", AGXBtnStyle))
            {
                AGXguiKeys[AGXCurActGroup] = KeyCode.None;
                ShowKeyCodeWin = false;

            }

            if (ShowJoySticks)
            {
                GUI.DrawTexture(new Rect(281, 3, 123, 18), BtnTexGrn);
            }

            if (GUI.Button(new Rect(280, 2, 125, 20), "Show JoySticks", AGXBtnStyle))
            {
                ShowJoySticks = !ShowJoySticks;
            }
            if (!ShowJoySticks)
            {
                int KeyListCount = new int();
                KeyListCount = 0;
                while (KeyListCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (KeyListCount * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 69)
                {
                    if (GUI.Button(new Rect(105, 25 + ((KeyListCount - 35) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 104)
                {
                    if (GUI.Button(new Rect(205, 25 + ((KeyListCount - 70) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 139)
                {
                    if (GUI.Button(new Rect(305, 25 + ((KeyListCount - 105) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
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
                    if (GUI.Button(new Rect(5, 25 + (JoyStickCount * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 69)
                {
                    if (GUI.Button(new Rect(130, 25 + ((JoyStickCount - 35) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 99)
                {
                    if (GUI.Button(new Rect(255, 25 + ((JoyStickCount - 70) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), JoyStickCodes.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

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

        public void SelParts(int WindowID)
        {

            AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
            HighLogic.Skin.scrollView.normal.background = null;
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
                highlightPartThisFrameSelWin = false;
                //GUI.Box(new Rect(SelPartsLeft, 25, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10), "");
                while (ButtonCount <= SelectedPartsCount)
                {

                    if (Mouse.screenPos.y >= SelPartsWin.y + 30 && Mouse.screenPos.y <= SelPartsWin.y + 140 && new Rect(SelPartsWin.x + SelPartsLeft + 25, (SelPartsWin.y + 30 + ((ButtonCount - 1) * 20)) - ScrollPosSelParts.y, 190, 20).Contains(Mouse.screenPos))
                    {
                        highlightPartThisFrameSelWin = true;
                        //print("part found to highlight " + AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.ConstructID);
                        partToHighlight = AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart;
                    }

                    
                    //highlight code here
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

            else //no parts selected, show list all parts button
            {
                if (GUI.Button(new Rect(SelPartsLeft + 50, 45, 140, 70), "Show list of\nall parts?", AGXBtnStyle)) //button itself
                {
                    showAllPartsListTitles = new List<string>(); //generate list of all parts 
                    showAllPartsListTitles.Clear(); //this probably isn't needed, but it works as is, not messing with it
                    foreach (Part p in FlightGlobals.ActiveVessel.parts) //cycle all parts
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



            if (GUI.Button(new Rect(SelPartsLeft + 245, 25, 110, 25), BtnTxt,AGXBtnStyle))
            {
                SelPartsIncSym = !SelPartsIncSym;

            }
            AGXBtnStyle.normal.background = ButtonTexture;
            if (GUI.Button(new Rect(SelPartsLeft + 245, 55, 110, 25), "Clear List",AGXBtnStyle))
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

                                        }
                                    }

                                }
                                RefreshDefaultActionsListType();
                            }
                            else
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
                                        //CurrentVesselActions.Add(ToAdd);
                                        CurrentVesselActions.Add(ToAdd);
                                        RefreshCurrentActions();
                                        // SaveCurrentVesselActions();
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
                        if (GUI.Button(new Rect(SelPartsLeft + 30, 190,185, 40), "No actions found.\r\nRefresh?", AGXBtnStyle))
                        {
                            PartActionsList.Clear();
                            PartActionsList.AddRange(AGEditorSelectedParts.First().AGPart.Actions);
                            foreach (PartModule pm in AGEditorSelectedParts.First().AGPart.Modules)
                            {
                                PartActionsList.AddRange(pm.Actions);
                            }
                            print("AGX Actions refresh found actions: " + PartActionsList.Count);
                        }
                    }
                }
            }
            else
            {
                TextAnchor TxtAnch = new TextAnchor();

                TxtAnch = GUI.skin.label.alignment;

                AGXLblStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(SelPartsLeft + 20, 180, 190, 40), "Select parts of\nthe same type",AGXLblStyle);
                


                AGXLblStyle.alignment = TextAnchor.MiddleLeft;

            }

            TextAnchor TxtAnch2 = new TextAnchor();
            AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
            //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            if (defaultShowingNonNumeric)
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), defaultGroupToShow.ToString(), AGXBtnStyle)) //current action group button
                {
                    //TempShowGroupsWin = true;
                }
            }
            else
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), AGXCurActGroup + ": " + AGXguiNames[AGXCurActGroup], AGXBtnStyle)) //current action group button
                {
                    TempShowGroupsWin = true;
                }

                //GUI.skin.button.alignment = TxtAnch2;
                AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                if (IsGroupToggle[AGXCurActGroup])
                {

                    Color TxtClr = GUI.contentColor;
                    GUI.contentColor = Color.green;
                    if (GUI.Button(new Rect(SelPartsLeft + 245, 160, 110, 22), "Toggle Grp: Yes", AGXBtnStyle))
                    {

                        IsGroupToggle[AGXCurActGroup] = false;
                    }
                    GUI.contentColor = TxtClr;
                }
                else
                {
                    if (GUI.Button(new Rect(SelPartsLeft + 245, 160, 110, 22), "Toggle Grp: No", AGXBtnStyle))
                    {

                        IsGroupToggle[AGXCurActGroup] = true;
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
                if (GUI.Button(new Rect(SelPartsLeft + 291, 183, 20, 22), "2", AGXBtnStyle))
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
                if (GUI.Button(new Rect(SelPartsLeft + 311, 183, 20, 22), "3", AGXBtnStyle))
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
                if (GUI.Button(new Rect(SelPartsLeft + 331, 183, 20, 22), "4", AGXBtnStyle))
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
                if (GUI.Button(new Rect(SelPartsLeft + 351, 183, 20, 22), "5", AGXBtnStyle))
                {
                    ShowGroupInFlight[5, AGXCurActGroup] = !ShowGroupInFlight[5, AGXCurActGroup];
                    CalculateActionsToShow();
                }
                GUI.contentColor = TxtClr2;
                GUI.Label(new Rect(SelPartsLeft + 245, 115, 110, 20), "Description:", AGXLblStyle);
                CurGroupDesc = AGXguiNames[AGXCurActGroup];
                CurGroupDesc = GUI.TextField(new Rect(SelPartsLeft + 245, 135, 120, 22), CurGroupDesc, AGXFldStyle);
                AGXguiNames[AGXCurActGroup] = CurGroupDesc;
                GUI.Label(new Rect(SelPartsLeft + 245, 203, 110, 25), "Keybinding:", AGXLblStyle);
                if (GUI.Button(new Rect(SelPartsLeft + 245, 222, 120, 20), AGXguiKeys[AGXCurActGroup].ToString(), AGXBtnStyle))
                {
                    ShowKeyCodeWin = true;
                }
            }
            if (GUI.Button(new Rect(SelPartsLeft + 245, 244, 120, 20), CurrentKeySetName, AGXBtnStyle))
            {
                //print("1a");
                SaveCurrentKeyBindings();
                //print("2a");
                KeySetNames[0] = AGExtNode.GetValue("KeySetName1");
               // /print("3a");
                KeySetNames[1] = AGExtNode.GetValue("KeySetName2");
                //print("4a");
                KeySetNames[2] = AGExtNode.GetValue("KeySetName3");
               // //print("5a");
                KeySetNames[3] = AGExtNode.GetValue("KeySetName4");
                //print("6a");
                KeySetNames[4] = AGExtNode.GetValue("KeySetName5");
                //print("7a");
                KeySetNames[CurrentKeySet - 1] = CurrentKeySetName;
                //print("8a");
                ShowKeySetWin = true;
                //print("9a");
            }


            GUI.DragWindow();
        }

        public void GroupsWindow(int WindowID)
        {

            AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
            HighLogic.Skin.scrollView.normal.background = null;
            if (AutoHideGroupsWin)
            {
                //GUI.DrawTexture(new Rect(6, 4, 78, 18), BtnTexRed);
                AGXBtnStyle.normal.background = ButtonTextureRed;
                AGXBtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(5, 3, 75, 20), "Auto-Hide", AGXBtnStyle))
            {
                AutoHideGroupsWin = !AutoHideGroupsWin;

            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;
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
            if(GUI.Button(new Rect(80,3,40,20),"Other",AGXBtnStyle))
            {
                defaultShowingNonNumeric = !defaultShowingNonNumeric;
                if (defaultShowingNonNumeric)
                {
                    defaultActionsListAll.Clear();
                    {
                        foreach(Part p in FlightGlobals.ActiveVessel.parts)
                        {
                            defaultActionsListAll.AddRange(p.Actions);
                            foreach(PartModule pm in p.Modules)
                            {
                                defaultActionsListAll.AddRange(pm.Actions);
                            }
                        }
                    }
                    RefreshDefaultActionsListType();
                }
            }
           // for (int i = 1; i <= 5; i = i + 1)
         //   {
               // if (PageGrn[i - 1] == true && GroupsPage != i)
               // {
                //    GUI.DrawTexture(new Rect(96 + (i * 25), 4, 23, 18), BtnTexGrn);
               // }
           // }
            //GUI.DrawTexture(new Rect(96 + (GroupsPage * 25), 4, 23, 18), BtnTexRed);
           // bool[5] PageBtnGrn = new bool[]();
            if (PageGrn[0]) AGXBtnStyle.normal.background = ButtonTextureGreen;
            if (PageGrn[0]) AGXBtnStyle.hover.background = ButtonTextureGreen;
            if (GroupsPage == 1) AGXBtnStyle.normal.background = ButtonTextureRed;
            if (GroupsPage == 1) AGXBtnStyle.hover.background = ButtonTextureRed;
            if (GUI.Button(new Rect(120, 3, 25, 20), "1", AGXBtnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 1;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;
            if (PageGrn[1]) AGXBtnStyle.normal.background = ButtonTextureGreen;
            if (PageGrn[1]) AGXBtnStyle.hover.background = ButtonTextureGreen;
            if (GroupsPage == 2) AGXBtnStyle.normal.background = ButtonTextureRed;
            if (GroupsPage == 2) AGXBtnStyle.hover.background = ButtonTextureRed;
            if (GUI.Button(new Rect(145, 3, 25, 20), "2", AGXBtnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 2;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;
            if (PageGrn[2]) AGXBtnStyle.normal.background = ButtonTextureGreen;
            if (PageGrn[2]) AGXBtnStyle.hover.background = ButtonTextureGreen;
            if (GroupsPage == 3) AGXBtnStyle.normal.background = ButtonTextureRed;
            if (GroupsPage == 3) AGXBtnStyle.hover.background = ButtonTextureRed;
            if (GUI.Button(new Rect(170, 3, 25, 20), "3", AGXBtnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 3;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;
            if (PageGrn[3]) AGXBtnStyle.normal.background = ButtonTextureGreen;
            if (PageGrn[3]) AGXBtnStyle.normal.background = ButtonTextureGreen;
            if (GroupsPage == 4) AGXBtnStyle.normal.background = ButtonTextureRed;
            if (GroupsPage == 4) AGXBtnStyle.hover.background = ButtonTextureRed;
            if (GUI.Button(new Rect(195, 3, 25, 20), "4", AGXBtnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 4;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;
            if (PageGrn[4]) AGXBtnStyle.normal.background = ButtonTextureGreen;
            if (PageGrn[4]) AGXBtnStyle.normal.background = ButtonTextureGreen;
            if (GroupsPage == 5) AGXBtnStyle.normal.background = ButtonTextureRed;
            if (GroupsPage == 5) AGXBtnStyle.hover.background = ButtonTextureRed;
            if (GUI.Button(new Rect(220, 3, 25, 20), "5", AGXBtnStyle))
            {
                defaultShowingNonNumeric = false;
                GroupsPage = 5;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            AGXBtnStyle.hover.background = ButtonTexture;
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
                        foreach (Part p in FlightGlobals.ActiveVessel.parts) //add all Parts with matching title to selected parts list, converting from string to Part
                        {
                            if (p.partInfo.title == partNameToSelect)
                            {
                                AGEditorSelectedParts.Add(new AGXPart(p));
                            }
                        }
                        AGEditorSelectedParts.RemoveAll(p2 => p2.AGPart.name != AGEditorSelectedParts.First().AGPart.name); //error trap just incase two parts have the same title, they can't have the same name
                        PartActionsList.Clear(); //populate actions list from selected parts
                        PartActionsList.AddRange(AGEditorSelectedParts.First().AGPart.Actions);
                        foreach (PartModule pm in AGEditorSelectedParts.First().AGPart.Modules)
                        {
                            PartActionsList.AddRange(pm.Actions);
                        }
                        //ScrollGroups = Vector2.zero;
                        showAllPartsList = false; //exit show all parts mode
                        TempShowGroupsWin = false; //hide window if auto hide enabled
                        AGEEditorSelectedPartsSame = true; //all selected parts are the same type as per the check above
                    }

                    listCount = listCount + 1; //moving to next button
                }

                GUI.EndScrollView();
            }
            else if (defaultShowingNonNumeric)
            {
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
                if (GUI.Button(new Rect(64, 25,58, 20), "Brakes", AGXBtnStyle)) //button code
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
                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
                //add vector2
                groupWinScroll = GUI.BeginScrollView(new Rect(5, 50, 240, 475), groupWinScroll, new Rect(0, 0, 240, Mathf.Max(475, defaultActionsListThisType.Count * 20)));
                int listCount2 = 1;
                highlightPartThisFrameGroupWin = false;
                while (listCount2 <= defaultActionsListThisType.Count)
                {
                    if (Mouse.screenPos.y >= GroupsWin.y + 50 && Mouse.screenPos.y <= GroupsWin.y + 525 && new Rect(GroupsWin.x + 5, (GroupsWin.y + 50 + ((listCount2 - 1) * 20)) - groupWinScroll.y, 240, 20).Contains(Mouse.screenPos))
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
            }
            else
            {

                AGXBtnStyle.normal.background = ButtonTexture;
                AGXBtnStyle.hover.background = ButtonTexture;
                ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollPosSelParts, new Rect(0, 0, 240, 500));

                int ButtonID = new int();
                ButtonID = 1 + (50 * (GroupsPage - 1));
                int ButtonPos = new int();
                ButtonPos = 1;
                TextAnchor TxtAnch3 = new TextAnchor();
                TxtAnch3 = GUI.skin.button.alignment;
                AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
                while (ButtonPos <= 25)
                {
                    if (ShowKeySetWin)
                    {
                        if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + " Key: " + AGXguiKeys[ButtonID].ToString(), AGXBtnStyle))
                        {

                            AGXCurActGroup = ButtonID;
                            ShowKeyCodeWin = true;
                        }
                    }

                    else
                    {
                        if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID)) AGXBtnStyle.normal.background = ButtonTextureGreen;
                        if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID)) AGXBtnStyle.hover.background = ButtonTextureGreen;
                        //{
                        //    GUI.DrawTexture(new Rect(1, ((ButtonPos - 1) * 20) + 1, 118, 18), BtnTexGrn);
                        //}


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
                while (ButtonPos <= 50)
                {
                    if (ShowKeySetWin)
                    {
                        if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + " Key: " + AGXguiKeys[ButtonID].ToString(), AGXBtnStyle))
                        {
                            AGXCurActGroup = ButtonID;
                            ShowKeyCodeWin = true;
                        }
                    }
                    else
                    {
                        if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID)) AGXBtnStyle.normal.background = ButtonTextureGreen;
                        if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID)) AGXBtnStyle.hover.background = ButtonTextureGreen;
                        //{
                        //    GUI.DrawTexture(new Rect(121, ((ButtonPos - 26) * 20) + 1, 118, 18), BtnTexGrn);
                        //}
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
                GUI.skin.button.alignment = TxtAnch3;

                GUI.EndScrollView();

            }
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
        public void LoadGroupVisibility(string stringToLoad)
        {
            try
            {
                //foreach (PartModule pm in FlightGlobals.ActiveVessel.rootPart.Modules.OfType<ModuleAGExtData>())
                //{
                string LoadString = stringToLoad;// (string)pm.Fields.GetValue("AGXGroupStates");
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
                //}
                CalculateActionsToShow();
               // print("AGXTogLoadFin: " + IsGroupToggle[1] + IsGroupToggle[2] + IsGroupToggle[3] + IsGroupToggle[4] + IsGroupToggle[5] + IsGroupToggle[6] + IsGroupToggle[7] + IsGroupToggle[8] + IsGroupToggle[9] + IsGroupToggle[10]);
            }
            catch
            {
                print("AGX Load Actions Visibility Fail!");
            }
        }

        public void LoadGroupNames(string namesToLoad)
        {
            
           

            for (int i = 1; i <= 250; i = i + 1)
            {
                AGXguiNames[i] = "";
            }
          

            string LoadNames = namesToLoad;
          
            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
                
                //foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())                   c cc 
                //{
                  

                    //LoadNames = (string)pm.Fields.GetValue("AGXNames");
                    //print("AGX Load Name: "+ p.partName+ " " + LoadNames);

                    if (LoadNames.Length > 0) //also update PartVesselCheck method
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
                   // }
               // }
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
                //foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
                //{
                //    pm.Fields.SetValue("AGXData", AddGroup);
                  


                //}

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

            string errLine = "1";
            try
            {
            bool RootPartExists = new bool();
            errLine = "2";
            try
            {
                errLine = "3";
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    errLine = "4";
                }
                errLine = "5";
                RootPartExists = true;
            }
            catch
            {
                errLine = "6";
                RootPartExists = false;
            }
            errLine = "7";

            if (flightNodeIsLoaded)
            {
                if (RootPartExists)
                {
                    errLine = "8";
                    if (AGXRoot != FlightGlobals.ActiveVessel.rootPart) //load keyset also
                    {
                        print("Root part changed, AGX reloading");
                        //print("Root prt ch");
                        if(!overrideRootChange)
                        {

                        print("Root part changed, AGX reloading B");
                        //loadFinished = false;
                        if (AGXRoot != null)
                        {
                            errLine = "9";
                            // print("Root part changed, AGX reloadinga");
                            ConfigNode oldVsl = new ConfigNode(AGXRoot.vessel.id.ToString());
                            if (AGXFlightNode.HasNode(AGXRoot.vessel.id.ToString()))
                            {
                                errLine = "10";
                                //print("Root part changed, AGX reloadingb");
                                oldVsl = AGXFlightNode.GetNode(AGXRoot.vessel.id.ToString());
                                AGXFlightNode.RemoveNode(AGXRoot.vessel.id.ToString());
                            }
                            errLine = "11";
                            //print("Root part changed, AGX reloadingc");
                            if (oldVsl.HasValue("name")) ;
                            {
                                oldVsl.RemoveValue("name");
                            }
                            oldVsl.AddValue("name", AGXRoot.vessel.vesselName);
                            errLine = "12";
                            // errLine = "13";
                            if (oldVsl.HasValue("currentKeyset")) ;
                            {
                                oldVsl.RemoveValue("currentKeyset");
                            }
                            oldVsl.AddValue("currentKeyset", CurrentKeySet.ToString());
                            errLine = "13";
                            //errLine = "14";
                            if (oldVsl.HasValue("groupNames")) ;
                            {
                                oldVsl.RemoveValue("groupNames");
                            }
                            oldVsl.AddValue("groupNames", SaveGroupNames(""));
                            errLine = "14";
                            //errLine = "15";
                            if (oldVsl.HasValue("groupVisibility")) ;
                            {
                                oldVsl.RemoveValue("groupVisibility");
                            }
                            oldVsl.AddValue("groupVisibility", SaveGroupVisibility(""));
                            errLine = "15";
                            //errLine = "16";
                            if (oldVsl.HasValue("groupVisibilityNames")) ;
                            {
                                oldVsl.RemoveValue("groupVisibilityNames");
                            }
                            oldVsl.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
                            errLine = "16";

                            oldVsl.RemoveNodes("PART");

                            foreach (Part p in AGXRoot.vessel.Parts)
                            {
                                List<AGXAction> thisPartsActions = new List<AGXAction>();
                                thisPartsActions.AddRange(CurrentVesselActions.FindAll(p2 => p2.ba.listParent.part == p));
                                //errLine = "18";
                                if (thisPartsActions.Count > 0)
                                {
                                    ConfigNode partTemp = new ConfigNode("PART");
                                    //errLine = "19";
                                    partTemp.AddValue("name", p.vessel.vesselName);
                                    partTemp.AddValue("vesselID", p.vessel.id);
                                    partTemp.AddValue("relLocX", AGXRoot.vessel.rootPart.transform.InverseTransformPoint(p.transform.position).x);
                                    partTemp.AddValue("relLocY", AGXRoot.vessel.rootPart.transform.InverseTransformPoint(p.transform.position).y);
                                    partTemp.AddValue("relLocZ", AGXRoot.vessel.rootPart.transform.InverseTransformPoint(p.transform.position).z);
                                    //errLine = "20";
                                    foreach (AGXAction agxAct in thisPartsActions)
                                    {
                                        //errLine = "21";
                                        partTemp.AddNode(AGextScenario.SaveAGXActionVer2(agxAct));
                                    }
                                    // errLine = "22";

                                    oldVsl.AddNode(partTemp);
                                    //errLine = "23";
                                }
                            }



                            AGXFlightNode.AddNode(oldVsl);
                            //print("Root part changed, AGX reloadingd " + oldVsl.GetValue("groupNames"));
                        }
                        CurrentVesselActions.Clear(); //we have saved old ship so clear actions

                        errLine = "17";
                        bool checkIsVab = true;
                        ConfigNode vslNode = new ConfigNode();
                        try
                        {
                            if (FlightGlobals.ActiveVessel.landedAt == "Runway")
                            {
                                //print("Runway found");
                                checkIsVab = false;
                            }
                            else
                            {
                                //print("runway not found");
                                checkIsVab = true;
                            }

                        }
                        catch
                        {
                            //print("runway iffy");
                            checkIsVab = true;
                        }

                        if (AGXFlightNode.HasNode(FlightGlobals.ActiveVessel.id.ToString()))
                        {
                            //print("node found"); 
                            vslNode = AGXFlightNode.GetNode(FlightGlobals.ActiveVessel.id.ToString());

                        }
                        else if (AGXEditorNode.HasNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName, checkIsVab)))
                        {
                            //print("vab1"); 
                            vslNode = AGXEditorNode.GetNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName, checkIsVab));
                            vslNode.name = FlightGlobals.ActiveVessel.id.ToString();
                            AGXFlightNode.AddNode(vslNode);
                        }
                        else if (AGXEditorNode.HasNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName, !checkIsVab)))
                        {
                            //print("vab2");
                            vslNode = AGXEditorNode.GetNode(AGextScenario.EditorHashShipName(FlightGlobals.ActiveVessel.vesselName, !checkIsVab));
                            vslNode.name = FlightGlobals.ActiveVessel.id.ToString();
                            AGXFlightNode.AddNode(vslNode);
                        }
                        else
                        {
                            //print("notfound");
                            vslNode = new ConfigNode(FlightGlobals.ActiveVessel.id.ToString());
                            vslNode.AddValue("name", FlightGlobals.ActiveVessel.vesselName);
                            vslNode.AddValue("currentKeyset", "1");
                            vslNode.AddValue("groupNames", "");
                            vslNode.AddValue("groupVisibility", "1011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111011111");
                            vslNode.AddValue("groupVisibilityNames", "Group 1‣Group 2‣Group 3‣Group 4‣Group 5");
                            AGXFlightNode.AddNode(vslNode);
                        }

                        CurrentKeySet = Convert.ToInt32(vslNode.GetValue("currentKeyset"));
                        LoadGroupNames(vslNode.GetValue("groupNames"));
                        LoadGroupVisibility(vslNode.GetValue("groupVisibility"));
                        LoadGroupVisibilityNames(vslNode.GetValue("groupVisibilityNames"));

                        foreach (ConfigNode prtNode in vslNode.nodes)
                        {
                            Vector3 partLoc = new Vector3((float)Convert.ToDouble(prtNode.GetValue("relLocX")), (float)Convert.ToDouble(prtNode.GetValue("relLocY")), (float)Convert.ToDouble(prtNode.GetValue("relLocZ")));
                            float partDist = 100f;
                            Part gamePart = new Part();
                            foreach (Part p in FlightGlobals.ActiveVessel.parts) //do a distance compare check, floats do not guarantee perfect decimal accuray so use part with least distance, should be zero distance in most cases
                            {
                                float thisPartDist = Vector3.Distance(partLoc, FlightGlobals.ActiveVessel.rootPart.transform.InverseTransformPoint(p.transform.position));
                                if (thisPartDist < partDist)
                                {
                                    gamePart = p;
                                    partDist = thisPartDist;
                                }
                            }
                            foreach (ConfigNode actNode in prtNode.nodes)
                            {
                                //print("node " + actNode + " " + gamePart.ConstructID);
                                AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(actNode, gamePart);
                                //print("act to add " + actToAdd.ba);
                                if (actToAdd.ba != null)
                                {
                                    CurrentVesselActions.Add(actToAdd);
                                }
                            }
                        }

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

                        //errLine = "16";
                        // string AddGroup = "";
                        List<BaseAction> partAllActions = new List<BaseAction>(); //is all vessel actions, copy pasting code
                        foreach (Part p in FlightGlobals.ActiveVessel.parts)
                        {
                            partAllActions.AddRange(p.Actions);
                            foreach (PartModule pm in p.Modules)
                            {
                                partAllActions.AddRange(pm.Actions);
                            }

                            //foreach (BaseAction ba in partAllActions)
                            //{
                            //    print(ba.listParent.part + " " + ba.listParent.module.moduleName + " " + ba.name + " " + ba.guiName);
                            //}
                            // print("part orgpos " + p.ConstructID+ " "  + p.orgPos + " " + p.orgRot);
                        }

                        foreach (BaseAction baLoad in partAllActions)
                        {
                            foreach (KSPActionGroup agrp in CustomActions)
                            {

                                if ((baLoad.actionGroup & agrp) == agrp)
                                {
                                    // errLine = "17";
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
                            // errLine = "18";
                        }
                    }


                        errLine = "22";


                        AGXRoot = FlightGlobals.ActiveVessel.rootPart;

                       

                        overrideRootChange = false;
                        LastPartCount = FlightGlobals.ActiveVessel.parts.Count;
                        AGEditorSelectedParts.Clear();
                        PartActionsList.Clear();
                        RefreshCurrentActions();
                        loadFinished = true;
                        //print("sit " + FlightGlobals.ActiveVessel.situation.ToString());
                        errLine = "23";
                    }
                }
                errLine = "24";
                if (LastPartCount != FlightGlobals.ActiveVessel.parts.Count) //parts count changed, remove any actions assigned to parts that have disconnected/been destroyed
                {
                    print("Part count change, reload AGX");
                    if (LastPartCount > FlightGlobals.ActiveVessel.parts.Count) //new count is larger was a docking op, see the dock gameevent to handle that
                    {
                        CheckListForMultipleVessels();
                    }
                    AGEditorSelectedParts.Clear();
                    PartActionsList.Clear();
                    //LoadActionGroups();
                    RefreshCurrentActions();
                    LastPartCount = FlightGlobals.ActiveVessel.parts.Count;
                    errLine = "25";

                }
            }
            errLine = "26";
            foreach (KeyCode KC in ActiveKeys)
            {
                errLine = "27";
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
            errLine = "28";
            //if (!ActiveActionsCalculated)
            //{
            //    CalculateActiveActions();
                
            //}
            if(Input.GetKeyDown(KeyCode.Mouse0) && ShowSelectedWin)
        {
            errLine = "29";
                Part selPart = new Part();
            selPart = SelectPartUnderMouse();
            if(selPart != null)
            {
                AddSelectedPart(selPart);
        }
            errLine = "30";
        }
            errLine = "31";
            if (RightClickDelay < 3)
            {
                errLine = "32";
                if (RightClickDelay == 2)
                {
                    errLine = "33";
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
                
                errLine = "34";
            }

            errLine = "35";
            
            if (Input.GetKeyUp(KeyCode.Mouse1) && ShowSelectedWin && RightLickPartAdded == true)
            {
                RightClickDelay = 0;
                RightLickPartAdded = false;

            }
            errLine = "36";

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
                //PartVesselChangeCheck();
                actionsCheckFrameCount = 0;
            }
            else
            {
                actionsCheckFrameCount = actionsCheckFrameCount + 1;
            }
            errLine = "37";
            //print("vessel " + FlightGlobals.ActiveVessel.id.ToString());
            //print("uid " + FlightGlobals.ActiveVssel.rootPart.uid.ToString());
            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    print("PartLoc " + p.ConstructID + " " + p.orgPos);
            //}
        //    print("Vessel Id " +FlightGlobals.ActiveVessel.id);
        //    print("Part Flight Id " + FlightGlobals.ActiveVessel.rootPart.flightID);
        //    print("Part UID " + FlightGlobals.ActiveVessel.rootPart.uid);
        //    print("Part mission id " + FlightGlobals.ActiveVessel.rootPart.missionID);
            //if (TimeWarp.CurrentRate != 1)
            //{
            //    if (ShowSelectedWin || ShowKeySetWin)
            //    {

            //        SaveEverything();
            //        ShowSelectedWin = false;
            //        ShowKeySetWin = false;
            //    }

            //}
            //PrintPartPos();
            //PrintPartActs();
            //print("Acts " + FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Custom01]);
        }
            catch(Exception e)
            {
                print("AGX Update error: " + errLine + " " + e);
            }
        }

        public void PrintPartActs()
        {
            try
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    List<BaseAction> partActs = new List<BaseAction>();
                    partActs.AddRange(p.Actions);
                    foreach (PartModule pm in p.Modules)
                    {
                        partActs.AddRange(pm.Actions);
                    }
                    print(p.ConstructID);
                    foreach (BaseAction ba in partActs)
                    {
                        print(ba.listParent.module.moduleName + " " + ba.name + " " + ba.guiName);
                    }
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
                foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                {
                    print(p.ConstructID + " " + p.orgPos + " " + p.vessel.rootPart.transform.InverseTransformPoint(p.transform.position) + " " + p.orgRot);

                }
            }
            catch
            {
                print("Print fail!");
            }
        }

        public void partDead(Part p)
        {
            CurrentVesselActions.RemoveAll(act => act.ba.listParent.part == p);
            RefreshCurrentActions();

            if(RootParts.HasNode(p.flightID.ToString()))
            {
                RootParts.RemoveNode(p.flightID.ToString());
                RootParts.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtRootParts.cfg");
            }
        }



        public void CheckListForMultipleVessels() //call this when part count on vessel decrease to check for actions on split vessels
        {
            List<Vessel> curActsVessels = new List<Vessel>(); //find out if actions exist on vessel that left
            foreach (AGXAction agAct in CurrentVesselActions)
            {
                if(!curActsVessels.Contains(agAct.ba.listParent.part.vessel))
                {
                    curActsVessels.Add(agAct.ba.listParent.part.vessel); //make a list of all vessels from actions in currentVesselActions list
                }
            }

            foreach (Vessel vsl2 in curActsVessels) //our list of vessels from actions in currentVesselActions lsit
            {
                if (vsl2 != FlightGlobals.ActiveVessel) //this runs only on the seperated vessel, not our focus vessel
                {
                    ConfigNode vsl2node = new ConfigNode(vsl2.id.ToString()); //make our confignode
                    if (AGXFlightNode.HasNode(vsl2.id.ToString())) //does this vessel exist in flight node?
                    {
                        vsl2node = AGXFlightNode.GetNode(vsl2.id.ToString()); //vessel exists?  load existing node, this should not happend but might if the same vessel docks/undocks in the same scene multiple times
                        vsl2node.RemoveNodes("PART");
                        AGXFlightNode.RemoveNode(vsl2.id.ToString());
                    }
                    //RootPart elseif check here
                    else //new vessel, copy current values
                    {
                        vsl2node.AddValue("name", FlightGlobals.ActiveVessel.vesselName);
                        vsl2node.AddValue("currentKeyset", CurrentKeySet.ToString());
                        vsl2node.AddValue("groupNames", SaveGroupNames(""));
                        vsl2node.AddValue("groupVisibility", SaveGroupVisibility(""));
                        vsl2node.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
                    }

                    foreach (Part p in vsl2.Parts) //cycle parts in separated vessel to find actions
                    {
                        List<AGXAction> thisPartsActions = new List<AGXAction>();
                        thisPartsActions.AddRange(CurrentVesselActions.FindAll(p2 => p2.ba.listParent.part == p));
                        //errLine = "18";
                        if (thisPartsActions.Count > 0)
                        {
                            //print("acts count " + thisPartsActions.Count);
                            ConfigNode partTemp = new ConfigNode("PART");
                            //errLine = "19";
                            partTemp.AddValue("name", p.vessel.name);
                            partTemp.AddValue("vesselID", p.vessel.id);
                            partTemp.AddValue("relLocX", vsl2.rootPart.transform.InverseTransformPoint(p.transform.position).x);
                            partTemp.AddValue("relLocY", vsl2.rootPart.transform.InverseTransformPoint(p.transform.position).y);
                            partTemp.AddValue("relLocZ", vsl2.rootPart.transform.InverseTransformPoint(p.transform.position).z);
                            //errLine = "20";
                            foreach (AGXAction agxAct in thisPartsActions)
                            {
                                //print("acts countb " + thisPartsActions.Count);
                                //errLine = "21";
                                partTemp.AddNode(AGextScenario.SaveAGXActionVer2(agxAct));
                            }
                            //errLine = "22";

                            vsl2node.AddNode(partTemp);
                            //errLine = "23";
                        }
                    }

                    AGXFlightNode.AddNode(vsl2node);
                    CurrentVesselActions.RemoveAll(ag => ag.ba.listParent.part.vessel == vsl2);


                }
            }
            RefreshCurrentActions();
        }

        public void DockingEvent(GameEvents.HostTargetAction<Part, Part> htAct) //docking event happend, merge two vessel actions
        {
            try
            {
               // print("Docking event!");
                if (loadFinished)
                {
                    Vessel vsl1 = htAct.host.vessel;
                    Vessel vsl2 = htAct.target.vessel;
                    //print(vsl1.id + " " + vsl2.id);
                    if (vsl1 != vsl2) //check to make sure this is not the same vessel docking to itself somehow, both vsl1 and vsl2 would be FG.AC then.
                    {
                        print("Docking event!");
                        overrideRootChange = true;
                        if (vsl1 == FlightGlobals.ActiveVessel || vsl2 == FlightGlobals.ActiveVessel) //check to make sure at least one vessel is FG.AC  Not sure how a docking event could happen when neither vessel is active but make sure
                        {
                            if (AGXFlightNode.HasNode(vsl1.id.ToString()))
                            {
                                // print("vsl1 found");
                                ConfigNode vsl1Node = AGXFlightNode.GetNode(vsl1.id.ToString());

                                foreach (ConfigNode prtNode in vsl1Node.nodes)
                                {
                                    Vector3 partLoc = new Vector3((float)Convert.ToDouble(prtNode.GetValue("relLocX")), (float)Convert.ToDouble(prtNode.GetValue("relLocY")), (float)Convert.ToDouble(prtNode.GetValue("relLocZ")));
                                    float partDist = 100f;
                                    Part gamePart = new Part();
                                    foreach (Part p in vsl1.parts) //do a distance compare check, floats do not guarantee perfect decimal accuray so use part with least distance, should be zero distance in most cases
                                    {
                                        float thisPartDist = Vector3.Distance(partLoc, vsl1.rootPart.transform.InverseTransformPoint(p.transform.position));
                                        if (thisPartDist < partDist)
                                        {
                                            gamePart = p;
                                            partDist = thisPartDist;
                                        }
                                    }
                                    foreach (ConfigNode actNode in prtNode.nodes)
                                    {
                                        //print("node " + actNode + " " + gamePart.ConstructID);
                                        AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(actNode, gamePart);
                                        //print("act to add " + actToAdd.ba);
                                        if (actToAdd.ba != null)
                                        {
                                            List<AGXAction> Checking = new List<AGXAction>();
                                            Checking.AddRange(CurrentVesselActions);
                                            Checking.RemoveAll(p => p.group != actToAdd.group);

                                            Checking.RemoveAll(p => p.prt != actToAdd.prt);

                                            Checking.RemoveAll(p => p.ba != actToAdd.ba);



                                            if (Checking.Count == 0)
                                            {

                                                CurrentVesselActions.Add(actToAdd);

                                            }
                                        }

                                    }
                                }
                            }

                            if (AGXFlightNode.HasNode(vsl2.id.ToString()))
                            {
                                //print("vsl2 found");
                                ConfigNode vsl2Node = AGXFlightNode.GetNode(vsl2.id.ToString());

                                foreach (ConfigNode prtNode in vsl2Node.nodes)
                                {
                                    Vector3 partLoc = new Vector3((float)Convert.ToDouble(prtNode.GetValue("relLocX")), (float)Convert.ToDouble(prtNode.GetValue("relLocY")), (float)Convert.ToDouble(prtNode.GetValue("relLocZ")));
                                    float partDist = 100f;
                                    Part gamePart = new Part();
                                    foreach (Part p in vsl2.parts) //do a distance compare check, floats do not guarantee perfect decimal accuray so use part with least distance, should be zero distance in most cases
                                    {
                                        float thisPartDist = Vector3.Distance(partLoc, vsl2.rootPart.transform.InverseTransformPoint(p.transform.position));
                                        if (thisPartDist < partDist)
                                        {
                                            gamePart = p;
                                            partDist = thisPartDist;
                                        }
                                    }
                                    foreach (ConfigNode actNode in prtNode.nodes)
                                    {
                                        //print("node " + actNode + " " + gamePart.ConstructID);
                                        AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(actNode, gamePart);
                                        //print("act to add " + actToAdd.ba);
                                        if (actToAdd.ba != null)
                                        {
                                            List<AGXAction> Checking = new List<AGXAction>();
                                            Checking.AddRange(CurrentVesselActions);
                                            Checking.RemoveAll(p => p.group != actToAdd.group);

                                            Checking.RemoveAll(p => p.prt != actToAdd.prt);

                                            Checking.RemoveAll(p => p.ba != actToAdd.ba);



                                            if (Checking.Count == 0)
                                            {

                                                CurrentVesselActions.Add(actToAdd);

                                            }
                                        }

                                    }
                                }
                            }



                        }
                    }
                    RefreshCurrentActions();
                }
            }
            catch (Exception e)
            {
                print("Docking fail. Ignore this error if you did not just dock. " + e);
            }
        }

        public void PartCheckTemp()
        {

            
            
            foreach (AGXPartVesselCheck prtCheck in partOldVessel)
            {
                if (prtCheck.prt == null)
                {
                    
                    
                    
                    partOldVessel.Remove(prtCheck);
                    goto BreakOut;
                }
                else if (prtCheck.prt.vessel != prtCheck.pVsl)
                {
                    if (prtCheck.prt.vessel == FlightGlobals.ActiveVessel && AGXFlightNode.HasNode(prtCheck.prt.vessel.id.ToString())) //if the part changing is part of the active vessel, ensure the save node is up to date
                    {
                        ConfigNode vslUpdate = AGXFlightNode.GetNode(prtCheck.prt.vessel.id.ToString());
                        if (vslUpdate.HasValue("name"))
                        {
                            vslUpdate.RemoveValue("name");
                        }
                        vslUpdate.AddValue("name", FlightGlobals.ActiveVessel.vesselName);
                        if (vslUpdate.HasValue("currentKeyset"))
                        {
                            vslUpdate.RemoveValue("currentKeyset");
                        }
                        vslUpdate.AddValue("name", FlightGlobals.ActiveVessel.vesselName);
                        if (vslUpdate.HasValue("groupNames"))
                        {
                            vslUpdate.RemoveValue("groupNames");
                        }
                        vslUpdate.AddValue("groupNames", SaveGroupNames(""));
                        if (vslUpdate.HasValue("groupVisibility"))
                        {
                            vslUpdate.RemoveValue("groupVisibility");
                        }
                        vslUpdate.AddValue("groupVisibility", SaveGroupVisibility(""));
                        if (vslUpdate.HasValue("groupVisibilityNames"))
                        {
                            vslUpdate.RemoveValue("groupVisibilityNames");
                        }
                        vslUpdate.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
                        AGXFlightNode.RemoveNode(prtCheck.prt.vessel.id.ToString());
                        AGXFlightNode.AddNode(vslUpdate);
                    }

                    if (prtCheck.pVsl == FlightGlobals.ActiveVessel && AGXFlightNode.HasNode(prtCheck.pVsl.id.ToString())) //if the part changing is part of the active vessel, ensure the save node is up to date
                    {
                        ConfigNode vslUpdate = AGXFlightNode.GetNode(prtCheck.pVsl.id.ToString());
                        if (vslUpdate.HasValue("name"))
                        {
                            vslUpdate.RemoveValue("name");
                        }
                        vslUpdate.AddValue("name", FlightGlobals.ActiveVessel.vesselName);
                        if (vslUpdate.HasValue("currentKeyset"))
                        {
                            vslUpdate.RemoveValue("currentKeyset");
                        }
                        vslUpdate.AddValue("currentKeyset", CurrentKeySet.ToString());
                        if (vslUpdate.HasValue("groupNames"))
                        {
                            vslUpdate.RemoveValue("groupNames");
                        }
                        vslUpdate.AddValue("groupNames", SaveGroupNames(""));
                        if (vslUpdate.HasValue("groupVisibility"))
                        {
                            vslUpdate.RemoveValue("groupVisibility");
                        }
                        vslUpdate.AddValue("groupVisibility", SaveGroupVisibility(""));
                        if (vslUpdate.HasValue("groupVisibilityNames"))
                        {
                            vslUpdate.RemoveValue("groupVisibilityNames");
                        }
                        vslUpdate.AddValue("groupVisibilityNames", SaveGroupVisibilityNames(""));
                        AGXFlightNode.RemoveNode(prtCheck.pVsl.id.ToString());
                        AGXFlightNode.AddNode(vslUpdate);
                    }
                    
                    if(AGXFlightNode.HasNode(prtCheck.prt.vessel.id.ToString()) && AGXFlightNode.HasNode(prtCheck.pVsl.id.ToString()))
                    {
                        //both ships exist in node, combine groupnames
                        ConfigNode mainVsl = AGXFlightNode.GetNode(prtCheck.prt.vessel.id.ToString());
                        ConfigNode secVsl = AGXFlightNode.GetNode(prtCheck.pVsl.id.ToString());
                        string LoadNames1 = mainVsl.GetValue("groupNames");
                        string LoadNames2 = secVsl.GetValue("groupNames");
                        Dictionary<int, string> Names1 = new Dictionary<int,string>();
                        Dictionary<int, string> Names2 = new Dictionary<int,string>();
                        for(int i = 1;i <= 250; i++)
                        {
                            Names1[i] = "";
                            Names2[i] = "";
                        }

                        if (LoadNames1.Length > 0)
                        {
                            while (LoadNames1[0] == '\u2023')
                            {

                                int groupNum = new int();
                                string groupName = "";
                                LoadNames1 = LoadNames1.Substring(1);
                                groupNum = Convert.ToInt32(LoadNames1.Substring(0, 3));
                                LoadNames1 = LoadNames1.Substring(3);

                                if (LoadNames1.IndexOf('\u2023') == -1)
                                {

                                    groupName = LoadNames1;
                                }
                                else
                                {

                                    groupName = LoadNames1.Substring(0, LoadNames1.IndexOf('\u2023'));
                                    LoadNames1 = LoadNames1.Substring(LoadNames1.IndexOf('\u2023'));
                                }


                                Names1[groupNum] = groupName;

                            }
                            // }
                            // }
                        }

                        if (LoadNames2.Length > 0)
                        {
                            while (LoadNames2[0] == '\u2023')
                            {

                                int groupNum = new int();
                                string groupName = "";
                                LoadNames2 = LoadNames2.Substring(1);
                                groupNum = Convert.ToInt32(LoadNames2.Substring(0, 3));
                                LoadNames2 = LoadNames2.Substring(3);

                                if (LoadNames2.IndexOf('\u2023') == -1)
                                {

                                    groupName = LoadNames2;
                                }
                                else
                                {

                                    groupName = LoadNames2.Substring(0, LoadNames2.IndexOf('\u2023'));
                                    LoadNames2 = LoadNames2.Substring(LoadNames2.IndexOf('\u2023'));
                                }


                                Names2[groupNum] = groupName;

                            }
                            // }
                            // }
                        }
                        for (int i = 1; i <= 250; i++)
                        {
                            if (Names1[i].Length == 0 && Names2[i].Length > 0)
                            {
                                Names1[i] = Names2[i];
                            }
                        }
                        if (prtCheck.prt.vessel == FlightGlobals.ActiveVessel)
                        {
                            AGXguiNames = Names1;
                        }


                        prtCheck.pVsl = prtCheck.prt.vessel;
                    }

                    else if (AGXFlightNode.HasNode(prtCheck.pVsl.id.ToString()))
                    {
                     ConfigNode newVsl = new ConfigNode(prtCheck.prt.vessel.id.ToString());
                        //if(RootParts.HasNode(prtCheck.prt.vessel.rootPart.flightID.ToString()))
                        //{
                        //    ConfigNode existRoot = RootParts.GetNode(prtCheck.prt.vessel.rootPart.flightID.ToString());
                        //    newVsl.AddValue("currentKeyset",existRoot.GetValue("currentKeyset"));
                        //newVsl.AddValue("groupNames", existRoot.GetValue("groupNames"));
                        //newVsl.AddValue("groupVisibility", existRoot.GetValue("groupVisibility"));
                        //newVsl.AddValue("groupVisibilityNames", existRoot.GetValue("groupVisibilityNames"));
                        //}

                        //else
                        //{
                        ConfigNode oldVsl = AGXFlightNode.GetNode(prtCheck.pVsl.id.ToString());
                        
                        newVsl.AddValue("currentKeyset",oldVsl.GetValue("currentKeyset"));
                        newVsl.AddValue("groupNames", oldVsl.GetValue("groupNames"));
                        newVsl.AddValue("groupVisibility", oldVsl.GetValue("groupVisibility"));
                        newVsl.AddValue("groupVisibilityNames", oldVsl.GetValue("groupVisibilityNames"));
                        //}
                        AGXFlightNode.AddNode(newVsl);
                        loadedVessels.Add(prtCheck.prt.vessel);
                        //print("part change case 2 " +newVsl);
                        prtCheck.pVsl = prtCheck.prt.vessel;
                    }

                    else if (AGXFlightNode.HasNode(prtCheck.prt.vessel.id.ToString()))
                    {
                        ConfigNode newVsl = new ConfigNode(prtCheck.pVsl.id.ToString());
                        //if (RootParts.HasNode(prtCheck.pVsl.rootPart.flightID.ToString()))
                        //{
                        //    ConfigNode existRoot = RootParts.GetNode(prtCheck.pVsl.rootPart.flightID.ToString());
                        //    newVsl.AddValue("currentKeyset", existRoot.GetValue("currentKeyset"));
                        //    newVsl.AddValue("groupNames", existRoot.GetValue("groupNames"));
                        //    newVsl.AddValue("groupVisibility", existRoot.GetValue("groupVisibility"));
                        //    newVsl.AddValue("groupVisibilityNames", existRoot.GetValue("groupVisibilityNames"));
                        //}
                        //else
                        //{
                           
                            ConfigNode oldVsl = AGXFlightNode.GetNode(prtCheck.prt.vessel.id.ToString());
                            newVsl.AddValue("currentKeyset", oldVsl.GetValue("currentKeyset"));
                            newVsl.AddValue("groupNames", oldVsl.GetValue("groupNames"));
                            newVsl.AddValue("groupVisibility", oldVsl.GetValue("groupVisibility"));
                            newVsl.AddValue("groupVisibilityNames", oldVsl.GetValue("groupVisibilityNames"));
                       // }
                        AGXFlightNode.AddNode(newVsl);
                        loadedVessels.Add(prtCheck.pVsl);
                       // print("part change case 3 " + newVsl);
                        prtCheck.pVsl = prtCheck.prt.vessel;
                    }
                    //else  //incomplete code, one of the two vessels in this call should always exist, if we hit this else statement something else has seriously gone wrong.
                    //{
                    //    if (RootParts.HasNode(prtCheck.pVsl.rootPart.flightID.ToString()))
                    //    {
                    //        ConfigNode newVsl = new ConfigNode(prtCheck.pVsl.id.ToString());
                    //        ConfigNode existRoot = RootParts.GetNode(prtCheck.pVsl.rootPart.flightID.ToString());
                    //        newVsl.AddValue("currentKeyset", existRoot.GetValue("currentKeyset"));
                    //        newVsl.AddValue("groupNames", existRoot.GetValue("groupNames"));
                    //        newVsl.AddValue("groupVisibility", existRoot.GetValue("groupVisibility"));
                    //        newVsl.AddValue("groupVisibilityNames", existRoot.GetValue("groupVisibilityNames"));
                    //    }
                    //}
                }
            }
        BreakOut:
            if (true == true)//can't have a } right after a : for some reason
            {

            }
           // print("Done");
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
            //if (ActiveActionsState.Count > 0)
            //{
                CalculateActionsState();
                CalculateActionsToShow();
           // }
           // ActiveActionsCalculated = true;
        }

        
        
        public static void CalculateActionsState() //flag each actiongroup as activated or not
        {
          // print("Calculate start");
            string errLine = "1";
            try
            {
                errLine = "2";
                foreach (AGXActionsState actState in ActiveActionsState)
                {
                    errLine = "3";
                    actState.actionOn = false;
                    actState.actionOff = false;
                }
                errLine = "4";
                //print("Calculate start2");
                foreach (AGXAction agxAct in CurrentVesselActions)
                {
                   // print("actions " + CurrentVesselActions.Count + " " + ActiveActionsState.Count);
                    errLine = "5";
                    if (agxAct.activated)
                    {
                        errLine = "6";
                        ActiveActionsState.Find(p => p.group == agxAct.group).actionOn = true;
                    }
                    else if (!agxAct.activated)
                    {
                        errLine = "7";
                        ActiveActionsState.Find(p => p.group == agxAct.group).actionOff = true;
                    }
                    errLine = "8";
                }
                errLine = "9";
            }
            catch (Exception e)
            {
                print("AGXCalculateActionsState " + errLine + e);
            }
            //print("Calculate start4");
        }

        public static string SaveGroupVisibility(string str)
        {
            string errLine = "1";
            bool OkayToProceed = true;
            try
            {
                errLine = "2";
                //try
                //{
                //if (p.missionID != FlightGlobals.ActiveVessel.rootPart.missionID)
                //{
                //    errLine = "3";
                //   // print("AGXTogSave other vessel");
                //    OkayToProceed = false;
                    
                //}
                //}
                //catch (Exception e)
                //{
                //    print("AGX Error: SaveGroupVisibility no root part  " + errLine + " " + e);
                //    OkayToProceed = true;
                //}

                //if (!OkayToProceed)
                //{
                //    return str;
                //}
                //else
                //{
                    errLine = "4";
                    string ReturnStr = ShowGroupInFlightCurrent.ToString(); //add currently show flightgroup
                    errLine = "5";
                    for (int i = 1; i <= 250; i++)
                    {
                        errLine = "6";
                        ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                        errLine = "7";
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            errLine = "8";
                            ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                        }
                    }
                    //print("AGXTogSave: " + ReturnStr);    
                    errLine = "9";
                    return ReturnStr;
                //}
            }
            catch (Exception e)
            {
                print("AGX Fail: SaveGroupVisibility " + errLine + " " + e);
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
            
            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
                
            //    foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
            //    {

            //        CurrentVesselActions.AddRange(agpm.LoadActionGroups());
            //    }
                
                
            //}
           // LastPartCount = FlightGlobals.ActiveVessel.parts.Count;

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



        public static string SaveGroupNames(String str)
        {
            string errStep = "1";
            //bool OkayToProceed = true;
            try
            {
                errStep = "2";
                string SaveStringNames = "";
                errStep = "3";
                //try
                //{
                //    if (p.missionID != FlightGlobals.ActiveVessel.rootPart.missionID)
                //    {
                //        OkayToProceed = false;
                //    }
                //}
                //catch (Exception e)
                //{
                //    print("AGX Error (SaveGroupNames) No root part " + errStep + " " + e);
                //}
                //if (OkayToProceed)
                //{
                    errStep = "4";
                    SaveStringNames = "";
                    errStep = "5";
                    int GroupCnt = new int();
                    errStep = "6";
                    GroupCnt = 1;
                    errStep = "7";
                    while (GroupCnt <= 250)
                    {
                        errStep = "8";
                        if (AGXguiNames[GroupCnt].Length >= 1)
                        {
                            errStep = "9";
                            SaveStringNames = SaveStringNames + '\u2023' + GroupCnt.ToString("000") + AGXguiNames[GroupCnt];
                            errStep = "10";
                        }
                        errStep = "11";
                        GroupCnt = GroupCnt + 1;
                        errStep = "12";
                    }
                    errStep = "13";
                //}
                
                //print(p.partName + " " + SaveStringNames);
                    //print("Savegroup return " + SaveStringNames);
                return SaveStringNames;
            }
            catch (Exception e)
            {
                print("AGX Save Group Names FAIL! (SaveGroupNames) " + errStep + " " + e);
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
                            //print((string)agAct.ba.listParent.module.Fields.GetValue("stateString"));
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