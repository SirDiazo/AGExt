using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;
using System.Reflection;

namespace ExternalTest
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class Class1 : PartModule
    {
        private Rect TestWin;
        private static GUIStyle AGXWinStyle = null;
        public void Start()
        {
            RenderingManager.AddToPostDrawQueue(0, OnDraw2);
            TestWin = new Rect(600, 300, 100, 200);
            AGXWinStyle = new GUIStyle(HighLogic.Skin.window);
        }
        public void OnDraw2()
        {
            TestWin = GUI.Window(673467666, TestWin, TestingWindow, "Test", AGXWinStyle);
        }
        public bool AGExtInstalled()
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");

            return (bool)calledType.InvokeMember("AGXInstalled", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, null);
        }
        public void AGExtToggleGroup(int group)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            int Group1 = new int();
            Group1 = 10;
            calledType.InvokeMember("AGXToggleGroup", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { Group1 });
        }
        public List<BaseAction> AGExtGet2VslAllActions(uint FlightID)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //uint FlightID = 4198041784;
            List<BaseAction> RetActs = (List<BaseAction>)calledType.InvokeMember("AGX2VslAllActions", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { FlightID });
            foreach (BaseAction ba in RetActs)
            {
                print(ba.guiName);
            }
            return RetActs;
        }
        public List<BaseAction> AGExtGet2VslGroupActions(uint FlightID, int group)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
            List<BaseAction> RetActs = (List<BaseAction>)calledType.InvokeMember("AGX2VslGroupActions", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { FlightID, group });
            foreach (BaseAction ba in RetActs)
            {
                print(ba.guiName);
            }
            return RetActs;
        }
        public List<BaseAction> AGXGroupActions(int group)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
            List<BaseAction> RetActs = (List<BaseAction>)calledType.InvokeMember("AGXGroupActions", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { group });
            foreach (BaseAction ba in RetActs)
            {
                print(ba.guiName);
            }
            return RetActs;
        }
        public List<BaseAction> AGXAllActions()
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
            List<BaseAction> RetActs = (List<BaseAction>)calledType.InvokeMember("AGXAllActions", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { });
            foreach (BaseAction ba in RetActs)
            {
                print(ba.guiName);
            }
            return RetActs;
        }
        public bool AGX2VslToggleGroup(uint FlightID, int group)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
            bool GroupAct = (bool)calledType.InvokeMember("AGX2VslToggleGroup", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { FlightID, group });
            print(GroupAct);
            return GroupAct;
        }

        public bool AGX2VslGroupState(uint FlightID, int group)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
            bool GroupAct = (bool)calledType.InvokeMember("AGX2VslGroupState", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { FlightID, group });
            print(GroupAct);
            return GroupAct;
        }

        public bool AGX2VslActivateGroup(uint FlightID, int group, bool forceDir)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
            bool GroupAct = (bool)calledType.InvokeMember("AGX2VslActivateGroup", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { FlightID, group, forceDir });
            print(GroupAct);
            return GroupAct;
        }
        public void AGXActivateGroup(int group, bool forceDir)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
           calledType.InvokeMember("AGXActivateGroup", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { group, forceDir });
           
        }
        public bool AGXGroupState(int group)
        {
            Type calledType = Type.GetType("ActionGroupsExtended.AGExtExternal, AGExt");
            //FlightID = 4198041784;
            //group = 10;
            bool GroupAct = (bool)calledType.InvokeMember("AGXGroupState", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new System.Object[] { group});
            print(GroupAct);
            return GroupAct;
        }

        

        public void TestingWindow(int WindowID)
        {
            if (GUI.Button(new Rect(0, 30, 30, 30), "1"))
            {
                bool AGXInstalled2 = new bool();
                AGXInstalled2 = false;
                AGXInstalled2 = AGExtInstalled();
                print(AGXInstalled2);
            }
            if (GUI.Button(new Rect(30, 30, 30, 30), "2"))
            {
                int Group = 15;
                AGExtToggleGroup(Group);
                //print("call made");
            }
            if (GUI.Button(new Rect(60, 30, 30, 30), "3"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                AGExtGet2VslAllActions(FlightID);
                //print("call made");
            }
            if (GUI.Button(new Rect(0, 60, 30, 30), "4"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                int group = 15;
                AGExtGet2VslGroupActions(FlightID, group);
                //print("call made");
            }
            if (GUI.Button(new Rect(30, 60, 30, 30), "5"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                int group = 15;
                AGXGroupActions(group);
                //print("call made");
            }
            if (GUI.Button(new Rect(60, 60, 30, 30), "6"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                //uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                //int group = 15;
                AGXAllActions();
                //print("call made");
            }
            if (GUI.Button(new Rect(0, 90, 30, 30), "7"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                int group = 15;
                AGX2VslToggleGroup(FlightID, group);
                //print("call made");
            }
            if (GUI.Button(new Rect(30, 90, 30, 30), "8"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                int group = 15;
                AGX2VslGroupState(FlightID, group);
                //print("call made");
            }
            if (GUI.Button(new Rect(60, 90, 30, 30), "9"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                int group = 15;
                AGX2VslActivateGroup(FlightID, group, true);
                //print("call made");
            }
            if (GUI.Button(new Rect(0, 120, 30, 30), "10"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                //uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                int group = 15;
                AGXActivateGroup(group, true);
                //print("call made");
            }
            if (GUI.Button(new Rect(30, 120, 30, 30), "11"))
            {
                //int Group1 = new int();
                //Group1 = 10;
                //uint FlightID = FlightGlobals.ActiveVessel.rootPart.flightID;
                int group = 15;
                AGXGroupState(group);
                //print("call made");
            }
        }
    }
}
