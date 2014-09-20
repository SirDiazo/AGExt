using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;


namespace ActionGroupsExtended //add scenario module for data storage
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)] //install our scenario module. is a partmodule so all calculations can be done in it, no need for a seperate plugin class in the scene
    class AGExtMainMenu : MonoBehaviour
    {
        public void Start()
        {
            var game = HighLogic.CurrentGame;
            ProtoScenarioModule psm = game.scenarios.Find(s => s.moduleName == typeof(AGextScenario).Name);
            if (psm == null)
            {
                psm = game.AddProtoScenarioModule(typeof(AGextScenario), GameScenes.FLIGHT);
            }
            //ProtoScenarioModule psm2 = game.scenarios.Find(s2 => s2.moduleName == typeof(AGextScenarioEditor).Name);
            //if (psm2 == null)
            //{
            //    psm2 = game.AddProtoScenarioModule(typeof(AGextScenarioEditor), GameScenes.EDITOR,GameScenes.SPH);
            //}

            DeleteOldSaveGames(); //delete old AGext000000.cfg files
        }

        public void DeleteOldSaveGames()
        {
            //print("Deleteing old save games");

            string[] existingGames = Directory.GetFiles(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder); //full path of all files in save dir
            List<int> existingGamesNum = new List<int>(); //existing AGExt00000.cfg files, as number
            List<int> persistentGamesNum = new List<int>(); //number in the .sfs save files
            int dirLength = (new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder).Length; //character length of file path
            foreach (string fileName in existingGames) //cycle through found files
            {
                //print("gamename " + fileName.Substring(dirLength + 1));
                if(fileName.Substring(dirLength + 1,5) == "AGExt" && fileName.Trim().EndsWith(".cfg")) //is file an AGX file?
                {
                    //print("gamenameb " + fileName.Substring(dirLength + 6,5));
                    try //this will work if file fould is an AGX flight file
                    {
                        int gameNum = Convert.ToInt32(fileName.Substring(dirLength + 6, 5));
                        existingGamesNum.Add(gameNum);
                        //print("gameNumb " + gameNum);
                    }
                    catch //did not work, was not an AGX flight file, but not actually an error so silently fail
                    {
                    }
                }
                else if (fileName.Trim().EndsWith(".sfs")) //is file an .sfs file?
                {
                    try //this will work on KSP save files
                    {
                       // print("sfsa");
                        ConfigNode saveNode = ConfigNode.Load(fileName); //load the .sfs file
                        //print("sfsb");
                        if(saveNode.HasNode("GAME")) //is a KSP save file?//move from the 'root' to "GAME" node
                        {
                            ConfigNode saveNode2 = saveNode.GetNode("GAME");//move from the 'root' to "GAME" node
                            //print("sfsc");
                            foreach (ConfigNode scenNode in saveNode2.GetNodes("SCENARIO")) //cycle through all SCENARIO nodes
                            {
                                //print("sfsd");
                                if (scenNode.HasValue("name") && scenNode.GetValue("name") == "AGextScenario") //stop at AGExtScenario node
                                {
                                    //print("sfse");
                                    persistentGamesNum.Add(Convert.ToInt32(scenNode.GetValue("LastSave"))); //add lastsave num to list
                                    //print("sfsf");
                                }
                            }
                        }
                    }
                    catch //did not work, not an KSP save file, silently fail
                    {
                    }
                }
            }
            foreach (int iGame in existingGamesNum) //check each existing game
            {
                bool keep = false; // CCraigen - file should be kept
                //print("Games " + iGame);
                //if (!persistentGamesNum.Contains(i)) //is the AGX flight file found in a persistent file? if not, delete it. not sure what quicksave is doing, leave a one back file just in case
                foreach(int iPersist in persistentGamesNum)
                {
                    /*if (iGame != iPersist && iGame != iPersist - 1)
                    {
                        //print("Deleted " + new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + iGame.ToString("00000") + ".cfg");
                        File.Delete(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + iGame.ToString("00000") + ".cfg");
                    }*/


                    // CCraigen - slight change to logic to prevent a mass clobber of all files if there's significantly differing .sfs files present
                    if (iGame == iPersist || iGame == iPersist - 1)
                    {
                        keep = true;
                    }
                }

                if (!keep)
                {
                    File.Delete(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + iGame.ToString("00000") + ".cfg");
                }

                // CCraigen - new logic ends

                }
            

            
        }


    }

    //AGXData per part
    //AGXNames per vessel
    //AGXKeyset per vessel
    //AGXLOaded no longer needed?
    //AGXGroupStates per vessel
    //partAllActions per part, baseAction list
    
    
    //public class AGExtScenVessel
    //{
    //    string VslId = "";
    //    string AGXNames = "";
    //    int AGXKeySet = 1;
    //    List<AGXAction> VslActionsList;
    //}

    //public class AGextScenarioEditor : ScenarioModule //this runs on flight scene start
    //{
    //    public override void OnLoad(ConfigNode node)
    //    {
    //        AGXEditor.EditorLoadFromFile();
    //    }
    //}
    public class AGextScenario : ScenarioModule //this runs on flight scene start
    {
        //public ConfigNode AGXBaseNode = new ConfigNode();
        //public ConfigNode AGXFlightNode = new ConfigNode();
        //public ConfigNode AGXEditorNode = new ConfigNode();

        int lastAGXSave = 1;
        //bool loadFin = false;
        ConfigNode currentFlightNode = new ConfigNode();

        //public void Start()
        //{
        //    GameEvents.onGameStateSaved.Add(GameEventSave);
        //}

        //public void GameEventSave(Game gm)
        //{
        //    print("game " + gm.linkCaption + " " + gm.linkURL);
        //}
        public override void OnLoad(ConfigNode node)
        {
            
           
                //print("AGXFlight load");
                if (node.HasValue("LastSave"))
                {
                    lastAGXSave = Convert.ToInt32(node.GetValue("LastSave"));
                }
                else
                {
                    lastAGXSave = 0;
                }
                if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg"))
                {
                    currentFlightNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg");
                }
                else
                {
                    currentFlightNode = new ConfigNode("FLIGHT");
                    currentFlightNode.AddValue("name", "flight");
                }
                //print("AGXScenLoad " + lastAGXSave);
                AGXFlight.AGXFlightNode = currentFlightNode;
                if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtRootParts.cfg"))
                {
                    AGXFlight.RootParts = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExtRootParts.cfg");
                }
                else
                {
                    AGXFlight.RootParts = new ConfigNode("ROOTPARTS");
                    AGXFlight.RootParts.AddValue("name", "rootParts");
                }

                AGXFlight.flightNodeIsLoaded = true;
                //print("Node laeded! "+ currentFlightNode);
            
        }
 

        public override void OnSave(ConfigNode node)
        {

            //print("a");
            
                //print("AGXFlightSave " + currentFlightNode);
                if(node.HasValue("LastSave"))
                {
                   // print("c");
                    lastAGXSave = Convert.ToInt32(node.GetValue("LastSave"));
                   // print("d");
                    node.RemoveValue("LastSave");
                }
                //print("e");
                //print("scensave1 " + currentFlightNode);
                ConfigNode flightToSave = AGXFlight.FlightSaveToFile(currentFlightNode);
                //print("f");
                lastAGXSave = lastAGXSave + 1;
                while (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg"))
                {
                    lastAGXSave = lastAGXSave + 1;
                }
               // print("g " + flightToSave);
                flightToSave.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt" + lastAGXSave.ToString("00000") + ".cfg");
               //print("scensave2 " + flightToSave);
                node.AddValue("LastSave", lastAGXSave.ToString());
                //print("i");
            
           // print("j");
        }

        public static ConfigNode LoadBaseNode()
        {
            string errLine = "1";
            ConfigNode AGXBaseNode = new ConfigNode();
            try
            {
                if (File.Exists(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt.cfg"))
                {
                    errLine = "3";
                    AGXBaseNode = ConfigNode.Load(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt.cfg");
                    //print("AGX ConfigNode Load Okay!");
                }
                else
                {
                    errLine = "4";
                    print("AGX ConfigNode not found, creating.....");
                    errLine = "5";
                    AGXBaseNode.AddValue("name", "Action Groups Extended save file");
                    AGXBaseNode.AddNode("FLIGHT");
                    errLine = "6";
                    AGXBaseNode.AddNode("EDITOR");
                    errLine = "7";
                    AGXBaseNode.Save(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName + "saves/" + HighLogic.SaveFolder + "/AGExt.cfg");
                    errLine = "8";
                }
                return AGXBaseNode;
            }
            catch (Exception e)
            {
                print("AGXScen LoadBaseNode FAIL " + errLine + " " + e);
                    return new ConfigNode();
            }
        }

        

        public static string EditorHashShipName(string name, bool isVAB)
        {
            string hashedName = "";
            if (isVAB)
            {
                hashedName = "VAB";
            }
            else
            {
                hashedName = "SPH";
            }
            foreach (Char ch in name)
            {
                hashedName = hashedName + (int)ch;
            }
            //print("hashName " + hashedName);

            return hashedName;
        }

        public static AGXAction LoadAGXActionVer2(ConfigNode actNode, Part actPart)
        {
            string errLine ="1";
            //print("load action " + actPart.partName + " " + actNode);
            try
            {
                errLine = "2";
                AGXAction ActionToLoad = new AGXAction(); //create action we are loading
                ActionToLoad.prt = actPart; //assign part
                ActionToLoad.group = Convert.ToInt32(actNode.GetValue("group")); //assign group
                errLine = "2a";
                if (actNode.GetValue("activated") == "1") //assign activated
                {
                    ActionToLoad.activated = true;
                }
                else
                {
                    ActionToLoad.activated = false;
                }
                errLine = "2b";
                string pmName = actNode.GetValue("partModule");//get partModule name
                List<BaseAction> actsToCompare = new List<BaseAction>(); //create list of actions we will compare to
                errLine = "2c";
                if (pmName == "ModuleEnviroSensor")
                {
                    string sensorType = actNode.GetValue("custom1");
                    foreach (PartModule pmSensor in actPart.Modules.OfType<ModuleEnviroSensor>())
                    {
                        ModuleEnviroSensor mesSensor = (ModuleEnviroSensor)pmSensor;
                        if (mesSensor.sensorType == sensorType)
                        {
                            actsToCompare.AddRange(mesSensor.Actions);
                        }
                    }
                    actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                    actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                }
                else if (pmName == "ModuleScienceExperiment")
                {
                    string expID = actNode.GetValue("custom1");
                    foreach (PartModule pmSensor in actPart.Modules.OfType<ModuleScienceExperiment>())
                    {
                        ModuleScienceExperiment mesExp = (ModuleScienceExperiment)pmSensor;
                        if (mesExp.experimentID == expID)
                        {
                            actsToCompare.AddRange(mesExp.Actions);
                        }
                    }
                    actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                    actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                }
                else if (pmName == "ModuleAnimateGeneric")
                {
                    string animName = actNode.GetValue("custom1");
                    foreach (PartModule pmSensor in actPart.Modules.OfType <ModuleAnimateGeneric>())
                    {
                        ModuleAnimateGeneric mesExp = (ModuleAnimateGeneric)pmSensor;
                        if (mesExp.animationName == animName)
                        {
                            actsToCompare.AddRange(mesExp.Actions);
                        }
                    }
                    actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                    actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                }
                else if (pmName == "DMModuleScienceAnimate")
                {
                    string startEventName = actNode.GetValue("custom1");
                    foreach (PartModule pm in actPart.Modules) //add actions to compare
                    {
                        if (pm.moduleName == pmName)
                        {
                            actsToCompare.AddRange(pm.Actions);
                        }
                        
                    }
                    actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                    //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                    actsToCompare.RemoveAll(b3 => b3.listParent.module.Fields.GetValue("startEventGUIName") != startEventName);
                }
                else if (pmName == "DMSolarCollector")
                {
                    string startEventName = actNode.GetValue("custom1");
                    foreach (PartModule pm in actPart.Modules) //add actions to compare
                    {
                        if (pm.moduleName == pmName)
                        {
                            actsToCompare.AddRange(pm.Actions);
                        }

                    }
                    actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                    //actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                    actsToCompare.RemoveAll(b3 => b3.listParent.module.Fields.GetValue("startEventGUIName") != startEventName);
                }
                else
                {
                    foreach (PartModule pm in actPart.Modules) //add actions to compare
                    {
                        if (pm.moduleName == pmName)
                        {
                            actsToCompare.AddRange(pm.Actions);
                        }
                        actsToCompare.RemoveAll(b => b.name != actNode.GetValue("actionName"));
                        actsToCompare.RemoveAll(b2 => b2.guiName != actNode.GetValue("actionGuiName"));
                    }

                }
                errLine = "3";
                //print("ActsCount " + actsToCompare.Count);
                if (actsToCompare.Count != 1)
                {
                    errLine = "4";
                    print("AGX actsToCompare.count != 1 "+actsToCompare.Count);
                }
                errLine = "5";
                if (actsToCompare.Count > 0)
                {
                   // print("ActsCounta");
                    errLine = "6";
                    ActionToLoad.ba = actsToCompare.First(); //action to load assign action, ready to return
                }
                else
                {
                    errLine = "7";
                   // print("ActsCountb");
                    ActionToLoad.ba = null;
                }
                errLine = "8";
                //print("load action2 " + ActionToLoad.ba.name + " " + ActionToLoad.group);
                //print("agx check " + actsToCompare.Count + " " + ActionToLoad.group + ActionToLoad.ba.name);
                //print("actual act " + ActionToLoad + " " + ActionToLoad.ba.name);
                return ActionToLoad;

            }
            catch (Exception e)
            {
                print("AGXLoadAGXAction2 FAIL " + errLine + " " + e);
                AGXAction rtnAgx = new AGXAction();
                rtnAgx.ba = null;
                return rtnAgx;
            }
        }

        public static ConfigNode SaveAGXActionVer2(AGXAction agxAct)
        {
            string errLine ="1";
            try
            {
            ConfigNode actionNode = new ConfigNode("ACTION");
                errLine ="2";
            actionNode.AddValue("group", agxAct.group);
                errLine ="3";
            actionNode.AddValue("activated",(agxAct.activated) ? "1":"0");
                errLine ="4";
            actionNode.AddValue("partModule", agxAct.ba.listParent.module.moduleName);
                errLine ="5";
            actionNode.AddValue("actionGuiName", agxAct.ba.guiName);
                errLine ="6";
            actionNode.AddValue("actionName", agxAct.ba.name);
                errLine ="7";
            if (agxAct.ba.listParent.module.moduleName == "ModuleEnviroSensor") //add this to the agxactions list somehow and add to save.load serialze
            {
                errLine = "8";
                ModuleEnviroSensor MSE = (ModuleEnviroSensor)agxAct.ba.listParent.module;
                errLine = "9";
                actionNode.AddValue("custom1", MSE.sensorType); //u2020 is envirosensor
                errLine = "10";
            }
            else if (agxAct.ba.listParent.module.moduleName == "ModuleScienceExperiment") //add this to the agxactions list somehow and add to save.load serialze
            {
                errLine = "11";
                ModuleScienceExperiment MSE = (ModuleScienceExperiment)agxAct.ba.listParent.module; //all other modules use guiname
                errLine = "12";
                actionNode.AddValue("custom1", MSE.experimentID); //u2021 is sciencemodule
                errLine = "13";
            }
            else if (agxAct.ba.listParent.module.moduleName == "ModuleAnimateGeneric") //add this to the agxactions list somehow and add to save.load serialze
            {
                errLine = "14";
                ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agxAct.ba.listParent.module; //all other modules use guiname
                errLine = "15";
                actionNode.AddValue("custom1", MAnim.animationName); //u2021 is sciencemodule
                errLine = "16";
                //print(MAnim.animationName);
            }
            else if (agxAct.ba.listParent.module.moduleName == "DMModuleScienceAnimate") //DMagic orbital science mod
            {
                errLine = "17";
                //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                errLine = "18";
                actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("startEventGUIName")); //u2021 is sciencemodule
                errLine = "19";
            }
            else if (agxAct.ba.listParent.module.moduleName == "DMSolarCollector") //DMagic orbital science mod
            {
                errLine = "20";
                //ModuleAnimateGeneric MAnim = (ModuleAnimateGeneric)agAct.ba.listParent.module; //all other modules use guiname
                errLine = "21";
                actionNode.AddValue("custom1", agxAct.ba.listParent.module.Fields.GetValue("startEventGUIName")); //u2021 is sciencemodule
                errLine = "22";
            }
            else //if (agAct.ba.listParent.module.moduleName == "ModuleScienceExperiment") //add this to the agxactions list somehow and add to save.load serialze
            {
                errLine = "23";
                //ModuleScienceExperiment MSE = (ModuleScienceExperiment)agAct.ba.listParent.module; //all other modules use guiname
                actionNode.AddValue("custom1","NA"); //u2021 is sciencemodule
                errLine = "24";
            }
                return actionNode;
            }
            catch(Exception e)
            {
                print("AGX SaveAGXAction2 FAIL " + errLine+ " " + agxAct.prt.partName+" " + agxAct.ba.name+ " " + e);
                return new ConfigNode();
            }

        }

        
    }
}
