using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ActionGroupsExtended
{
    public class AGXEventHandler
    {
        //Guid is Vessel.id, int is action group number, bool is state
        private static Dictionary<Guid, Dictionary<int, bool>> masterActionGroupStates; //master container for referencing action group states by other mods
        public static Dictionary<Guid, Dictionary<int, bool>> MasterActionGroupStates //this is the reference passed to other mods by Event, read-only
        {
            get
            {
                return masterActionGroupStates;
            }
        }
        //Guid is Vessel.id, int is action group number, List<BaseAction> are all actions assigned to that group.
        private static Dictionary<Guid, Dictionary<int, List<BaseAction>>> masterAssignedActions; //master container for referencing assigned actions by other mods. This uses BaseAction, NOT AGXAction
        public static Dictionary<Guid, Dictionary<int, List<BaseAction>>> MasterAssignedActions //this is the reference passed to other mods by Event, read-only
        {
            get
            {
                return masterAssignedActions;
            }
        }
        //Guid is Vessel.id, int is action group number, List<BaseAction> are all actions assigned to that group.
        private static Dictionary<Guid,int> masterCurrentKeySet; //currently active key set that will be checked against keyboard input
        public static Dictionary<Guid, int> MasterCurrentKeySet //this is the reference passed to other mods by Event, read-only
        {
            get
            {
                return masterCurrentKeySet;
            }
        }
        //Guid is Vessel.id, uint is current FocusFlightID of current vessel.
        private static Dictionary<Guid, uint> masterFocusFlightID; //currently active focusflightID for the vessel
        public static Dictionary<Guid, uint> MasterFocusFlightID //this is the reference passed to other mods by Event, read-only
        {
            get
            {
                return masterFocusFlightID;
            }
        }
        //int is KeySet number, string is name. These are global, not tied to vessel
        private static Dictionary<int, string> masterKeySetNames;
        public static Dictionary<int, string> MasterKeySetNames
        {
            get
            {
                return masterKeySetNames;
            }
        }
        //Guid is Vessel.id, uint is focusFlightID, int is action group number, string is group name
        private static Dictionary<Guid, Dictionary<uint, Dictionary<int, string>>> masterActionGroupNames;
        public static Dictionary<Guid, Dictionary<uint, Dictionary<int, string>>> MasterActionGroupNames
        {
            get
            {
                return masterActionGroupNames;
            }
        }
        //Guid is Vessel.id, uint is focusFlightID, int is action group number, bool is direct action state, false is toggle behavior, true is hold behavior
        private static Dictionary<Guid, Dictionary<uint, Dictionary<int, bool>>> masterActionGroupDirectActionState;
        public static Dictionary<Guid, Dictionary<uint, Dictionary<int, bool>>> MasterActionGroupDirectActionState
        {
            get
            {
                return masterActionGroupDirectActionState;
            }
        }
        //Guid is Vessel.id, uint is focusFlightID, bool array param 1 is visibilty states 1 to 5, param 2 is action group number
        private static Dictionary<Guid, Dictionary<uint, bool[,]>> masterActionGroupVisibility;
        public static Dictionary<Guid, Dictionary<uint, bool[,]>> MasterActionGroupVisibility
        {
            get
            {
                return masterActionGroupVisibility;
            }
        }

        public static EventData<Action<Dictionary<Guid, Dictionary<int, bool>>>> onAGXMasterActionGroupStates;
        public static EventData<Guid, Action<Dictionary<int, bool>>> onAGXVesselActionGroupStates;
        public static EventData<Guid, int, Action<bool>> onAGXActionGroupState;
        public static EventData<Action<Dictionary<Guid, Dictionary<int, List<BaseAction>>>>> onAGXMasterAssignedActions;
        public static EventData<Guid, Action<Dictionary<int, List<BaseAction>>>> onAGXVesselAssignedActions;
        public static EventData<Guid, int, List<BaseAction>> onAGXActionGroupAssignedActions;
        public static EventData< //on masterCurrentKeyset


        public static EventData<Action<Dictionary<Guid, Dictionary<int, bool>>>> onTestEvent;
        public static List<float> numList;

        public static AGXEventHandler myEventHandler;
        public static AGXFlight myFlightAddon;
        public static AGXEditor myEditorAddon;

        public static void addTestValue()
        {
            Dictionary<int, bool> tempDict = new Dictionary<int, bool>();
            tempDict.Add(1, false);
            masterActionGroupStates.Add(Guid.NewGuid(), tempDict);
        }

        public void init()
        {
            
            Debug.Log("AGX event init fired!");
            onTestEvent = new EventData<Action<Dictionary<Guid, Dictionary<int, bool>>>>("onTestEvent");
            masterActionGroupStates = new Dictionary<Guid, Dictionary<int, bool>>();
            Debug.Log("AGX event init 1fired!");
            //numList = new List<float>();
            Debug.Log("AGX event init 1A fired!");
            //numList.Add(Time.realtimeSinceStartup);
            masterActionGroupStates.Add(Guid.NewGuid(), new Dictionary<int, bool> { { 1, false } });
            Debug.Log("AGX event init2fired!");
            onTestEvent.Add(myEventHandler.testAGXmethod);
            Debug.Log("AGX event init 3fired!");
        }

        public void testAGXmethod(Action<Dictionary<Guid, Dictionary<int, bool>>> myObj)
        {
            Debug.Log("AGX sees event fire!" + MasterActionGroupStates.Count + "|" + masterActionGroupStates.Count);
            myObj(MasterActionGroupStates);
        }
    }
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AGXEventTestFlight : PartModule
    {
       
        public void Start()
        {
           // AGXEventHandler.Init(); // initialize the event handler
            DontDestroyOnLoad(this); // coroutines stop when the object is destroyed, this keeps it running constantly
            StartCoroutine("numCounter");
        }

        public IEnumerator numCounter()
        {
            while (true) // rather than starting a new coroutine every 5s, just let this one loop
            {
                AGXEventHandler.addTestValue();
                Debug.Log("AGX test " + AGXEventHandler.MasterActionGroupStates.Keys.Count);
                yield return new WaitForSeconds(5f);
            }
        }

        //public void Update()
        //{
            
        //}
    }

    
   
    }
