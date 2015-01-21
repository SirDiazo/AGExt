//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace ActionGroupsExtended
//{
//    class OtherVessel : PartModule //stuff for a non-loaded vessel, this class assumes vessel is loaded, gate it behind the AGXFlight.IsVesselLoaded check
//    {
//        List<AGXAction> actsList;
//        Dictionary<int, string> grpNames;
//        bool actionsPresent = false; //is this loading of other vessel valid?
//        ConfigNode vslNode;


//        public OtherVessel(Vessel vsl) //initialize class for vessel we are going to use
//        {
//            InitializeThis(vsl, vsl.rootPart.flightID);
//        }

//        public OtherVessel(uint FlightID) //initialize class for vessel we are going to use
//        {
//            InitializeThis(FlightGlobals.Vessels.First(vsl2 => vsl2.rootPart.flightID == FlightID), FlightID);
//        }

//        private void InitializeThis(Vessel vsl, uint flightID) //actual constructor to initizliaze, do not call directly
//        {
//            actsList = new List<AGXAction>();
//            grpNames = new Dictionary<int, string>();
//            if (AGXFlight.AGXFlightNode.HasNode(flightID.ToString())) //get confignode
//            {
//                vslNode = AGXFlight.AGXFlightNode.GetNode(flightID.ToString());
//                actionsPresent = true;
//            }
//            else
//            {
//                actionsPresent = false;
//            }
//            if (actionsPresent) //only try to load actions if node found
//            {
//                if (vslNode.HasValue("groupNames")) //load group names for searching by name
//                {
//                    LoadGroupsNames(vslNode.GetValue("groupNames"));
//                }
//            }
//            foreach (ConfigNode pNode in vslNode.nodes) //start loading actions
//            {
//                Part gamePart = new Part(); //find part
//                if (pNode.HasValue("flightID"))
//                {
//                    uint flightIDFromFile = Convert.ToUInt32(pNode.GetValue("flightID"));
//                    gamePart = FlightGlobals.ActiveVessel.parts.First(prt => prt.flightID == flightIDFromFile);   
//                }
//                foreach(ConfigNode aNode in pNode.nodes)
//                {
//                    AGXAction actToAdd = AGextScenario.LoadAGXActionVer2(aNode, gamePart, false);
//                    if (actToAdd.ba != null)
//                    {
//                        actsList.Add(actToAdd);
//                    }
//                }
//            }

//            try //check we have a valid class to represent our other vessel
//            {
//                if (actsList.Count > 0)
//                {
//                    actionsPresent = true;
//                }
//                else
//                {
//                    actionsPresent = false;
//                }
//            }
//            catch //final error trap, if actsList is invalid this class does not have valid data
//            {
//                actionsPresent = false;
//            }
//            //initialization finished
//        }

//        private void LoadGroupsNames(string groupNames)
//        {

//            string LoadNames = groupNames;
          
         
//                    if (LoadNames.Length > 0)
//                    {
//                        while (LoadNames[0] == '\u2023')
//                        {
                        
//                            int groupNum = new int();
//                            string groupName = "";
//                            LoadNames = LoadNames.Substring(1);
//                            groupNum = Convert.ToInt32(LoadNames.Substring(0, 3));
//                            LoadNames = LoadNames.Substring(3);

//                            if (LoadNames.IndexOf('\u2023') == -1)
//                            {
                               
//                                groupName = LoadNames;
//                            }
//                            else
//                            {
                               
//                                groupName = LoadNames.Substring(0, LoadNames.IndexOf('\u2023'));
//                                LoadNames = LoadNames.Substring(LoadNames.IndexOf('\u2023'));
//                            }
//                                grpNames[groupNum] = groupName;
//                        }
//            }

//        } //parse groupNames string and load it

//        public List<Part> prtListInGroup(int i)
//        {
//            List<Part> prtListInGroup = new List<Part>();
//            foreach(AGXAction agAct in actsList.Where(agx => agx.group ==i))
//            {
//                if(!prtListInGroup.Contains(agAct.ba.listParent.part))
//                {
//                    prtListInGroup.Add(agAct.ba.listParent.part);
//                }
//            }
//            return prtListInGroup;
//        }

//        public List<PartModule> pmListInGroup(int i)
//        {
//            List<PartModule> prtListInGroup = new List<PartModule>();
//            foreach (AGXAction agAct in actsList.Where(agx => agx.group == i))
//            {
//                if (!prtListInGroup.Contains(agAct.ba.listParent.module))
//                {
//                    prtListInGroup.Add(agAct.ba.listParent.module);
//                }
//            }
//            return prtListInGroup;
//        }
        
//        public void ToggleGroup(string grpName) //toggle group by name
//        {
//            try
//            {
//                int group = grpNames.First(nm => nm.Value == grpName).Key;
//                if(group >= 1 && group <= 250)
//                {
//                ActivateGroup(group,false,false);
//                }
//                else
//                {
//                    print("AGX does not find called action group by name, no actions taken.");
//                }
//            }
//            catch
//            {
//                print("AGX does not find called action group by name, no actions taken.");
//            }
            
//        }
        
//        public void ToggleGroup(int group) //toggle action group, wrapper that points to ActivateGroup
//        {
//            ActivateGroup(group, false, false);
//        }
        
//        public void ActivateGroup(string strGroup, bool force, bool forceDir)
//        {
//            try
//            {
//                int group = grpNames.First(nm => nm.Value == strGroup).Key;
//                if(group >= 1 && group <= 250)
//                {
//                ActivateGroup(group,force,forceDir);
//                }
//                else
//                {
//                    print("AGX does not find called action group by name, no actions taken.");
//                }
//            }
//            catch
//            {
//                print("AGX does not find called action group by name, no actions taken.");
//            }
//        }

//        public void ActivateGroup(int group, bool force, bool forceDir) //actual group activation, other activation methods are wrappers pointing to here
//        {
            
//            Dictionary<int, KSPActionGroup> CustomActions = new Dictionary<int, KSPActionGroup>();
//            CustomActions.Add(1, KSPActionGroup.Custom01); //how do you add a range from enum?
//            CustomActions.Add(2, KSPActionGroup.Custom02);
//            CustomActions.Add(3, KSPActionGroup.Custom03);
//            CustomActions.Add(4, KSPActionGroup.Custom04);
//            CustomActions.Add(5, KSPActionGroup.Custom05);
//            CustomActions.Add(6, KSPActionGroup.Custom06);
//            CustomActions.Add(7, KSPActionGroup.Custom07);
//            CustomActions.Add(8, KSPActionGroup.Custom08);
//            CustomActions.Add(9, KSPActionGroup.Custom09);
//            CustomActions.Add(10, KSPActionGroup.Custom10);
        
//            foreach (AGXAction agAct in actsList.Where(agx => agx.group == group))
//            {

//                if (force) //are we forcing a direction or toggling?
//                {
//                    if (forceDir) //we are forcing a direction so set the agAct.activated to trigger the direction below correctly
//                    {
//                        agAct.activated = false; //we are forcing activation so activated is false
//                    }
//                }
                
//                if (agAct.activated) //activate the action
//                {
//                    KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Deactivate);
                    
//                    agAct.ba.Invoke(actParam);
//                    foreach (AGXAction agxAct in actsList)
//                    {
//                        if(agxAct.ba == agAct.ba)
//                        {
//                            agxAct.activated = false;
//                        }
//                    }
//                    if (group <= 10) //backsave group state to ksp
//                    {
//                        FlightGlobals.ActiveVessel.ActionGroups[CustomActions[group]] = false;
//                    }
                  
//                }
//                else //deactivate action
//                {
//                    KSPActionParam actParam = new KSPActionParam(KSPActionGroup.None, KSPActionType.Activate);
//                    agAct.ba.Invoke(actParam);
//                    foreach (AGXAction agxAct in actsList)
//                    {
//                        if (agxAct.ba == agAct.ba)
//                        {
//                            agxAct.activated = true;
//                        }
//                    }
//                    if (group <= 10)
//                    {
//                        FlightGlobals.ActiveVessel.ActionGroups[CustomActions[group]] = true;
//                    }
//                }
//                ConfigNode savePrt = vslNode.GetNodes("PART").First(nd => nd.GetValue("flightID") == agAct.prt.flightID.ToString()); //part confignode to save new action state to, possible to save directly to node somehow?
//                savePrt.RemoveNodes("ACTION"); //remove all action nodes
//                foreach (AGXAction agAct2 in actsList.Where(agx3 => agx3.ba.listParent.part == agAct.ba.listParent.part)) //get all actions on this part, don't have a unique key to identify action sub-nodes so do them all
//                {
//                    savePrt.AddNode(AGextScenario.SaveAGXActionVer2(agAct2));
//                }
//                //savePrt is ready to go

//                ConfigNode toDelete = null;

//                foreach(ConfigNode pTemp in vslNode.nodes) //walk the part nodes to find part node to delete
//                {
//                    if(Convert.ToUInt32(pTemp.GetValue("flightID")) == agAct.ba.listParent.part.flightID) //tag the part node to delete
//                    {
//                        toDelete = pTemp;
//                    }
//                }

//                if(toDelete != null) //check we actually delete part node, in theory it is impossible not too, but you never know
//                {
//                vslNode.nodes.Remove(toDelete); //remove the part node with old data on the action
//                }
//                vslNode.AddNode(savePrt); //add part with update aciton info back in

//                ////// old code
//                //////List<ConfigNode> tempToCopy = new List<ConfigNode>(); //make temp list to hold all other part nodes from vessel

//                //////foreach(ConfigNode pTemp in vslNode.nodes) //walk the part nodes
//                //////{
//                //////    if(Convert.ToUInt32(pTemp.GetValue("flightID")) != agAct.ba.listParent.part.flightID) //copy all nodes over, except the part we made the savePrt for above
//                //////    {
//                //////        tempToCopy.Add(pTemp);
//                //////    }
//                //////}
                

//                ////vslNode.RemoveNodes("PART");
//                ////foreach(ConfigNode cNode in tempToCopy)
//                ////{
//                ////    vslNode.AddNode(cNode);
//                ////}
//                ////vslNode.AddNode(savePrt);
//                //vslNode is ready to save back to AGXFlightNode
//                if(AGXFlight.AGXFlightNode.HasNode(vslNode.name))
//                {
//                    AGXFlight.AGXFlightNode.RemoveNode(vslNode.name);
//                }
//                AGXFlight.AGXFlightNode.AddNode(vslNode);
//                //and done, vessel node in agxflight is now up to date

            
//        }
//        }
        
//    }
//}
