using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DialogueSystem.DataStructure;

/// <summary>
/// Adding name space to avoid errors of potential there are already scripts in a project with the same name.
/// However, namespace must be included in script utlising the DataManager static script!
/// </summary>
namespace DialogueSystem.FileHandling
{
    public static class DataManager
    {
        /// <summary>
        /// This class is responsible for deserialzing the desired XML file data into the various dialogue data structures.
        /// </summary>
        /// <param name="fileName"> This is the file path to the XML file and is provided by the file browser within the inspector. 
        /// Note: must be nested within Streaming assets, or be within a folder nested within Streaming Assets!</param>
        /// <returns> Returns the Entity object which contains all sub-objects contanied with dialogue data </returns>
        public static Entity LoadXMLDialogueData(string fileName)
        {

            string path = Path.Combine(Application.streamingAssetsPath, fileName + ".xml"); // Need to do this to read from streaming assets


            XDocument xDoc = XDocument.Load(path);


            string id = xDoc.Root.Attribute("ID").Value;
            string name = xDoc.Root.Attribute("Name").Value;

            //Create new entity object and pass file root attributes to the objects constructor
            Entity entity = new(id, name);

            var line = xDoc.Root.Descendants("Line"); //Get all nodes called line

            //  line = line.
            Debug.Log("ID: " + entity.id);
            Debug.Log("Name: " + entity.name);

            //Iterate through each line node
            foreach (var lineNode in line)
            {
                //Create a new line object on each iteration
                Line lineObj = new Line();

                //Create XElements for each node type
                var key = lineNode.Descendants("Key");
                var _line = lineNode.Descendants("LineText"); 
                var conditions = lineNode.Descendants("Condition");

                //Set the line ID as the attribute of the XML Line node 
                lineObj.lineID = uint.Parse((string)lineNode.Attribute("id"));
                
                //Set keys
                foreach (var _key in key)
                {
                    lineObj.key = _key.Value;
                }

                //Set lines
                foreach (var nodeLine in _line)
                {
                    lineObj.line = nodeLine.Value;
                }


                //Set conditions
                foreach (var condition in conditions)
                {
                    //Create a new condition object on each iteration
                    Condition _condition = new();
                    var gameDataName = condition.Descendants("GameDataName");
                    foreach (var _gameDataLine in gameDataName)
                    {
                        _condition.gameDataName = _gameDataLine.Value;
                    }

                    var gameDataKey = condition.Descendants("GameDataKey");
                    foreach (var _gameDataKeyLine in gameDataKey)
                    {
                        _condition.gameDataKey = uint.Parse(_gameDataKeyLine.Value);
                    }
                    var triggerCondition = condition.Descendants("TriggerCondition");
                    foreach (var _triggerConditionLine in triggerCondition)
                    {
                        _condition.triggerCondition = StringValidation.GetConditionLogicType(_triggerConditionLine.Value, out string trimmedVal);
                        dynamic parsedVal = StringValidation.ConvertStringToDataType<dynamic>(trimmedVal);
                        _condition.conditionValue = parsedVal;
                    }

                    lineObj.conditions.Add(uint.Parse(condition.Attribute("id").Value), _condition);


                }

                //Pass the populated line object to the lines to the line list within the entity object
                entity.lines.Add((uint)lineObj.lineID, lineObj);

            }

            return entity;

        }
    }
}

/// <summary>
/// Adding name space to avoid errors of potential there are already scripts in a project with the same name, however namespace must be included in script utlising these objects
/// </summary>
namespace DialogueSystem.DataStructure
{

    /// <summary>
    /// These data structures will be used to contain an individual line within a xml file into a object, 
    /// and then use this to populate the lines list within the Entity object to be able to populate the entity object with all line info found within an xml file
    /// </summary>
    /// 
    [Serializable]
    public class Line
    {
        public uint lineID;
        public string key;
        public string line;
        public Dictionary<uint, Condition> conditions;

        public Line()
        {
            conditions = new Dictionary<uint, Condition>();
        }

    }

    //Used to define condition info for each line so we can store it in the hash table
    public class Condition
    {
        public string gameDataName;
        public ConditionalLogicType triggerCondition;
        public dynamic conditionValue;
        public uint gameDataKey;
    }

    //Used to store all sub=objects of an NPCs dialogue info into one object that can be stored into the dialogue hastable
    [Serializable]
    public class Entity
    {
        public uint id;
        public string name;
        public Dictionary<uint, Line> lines;

        public Entity(string _id, string _name)
        {
            id = uint.Parse(_id);
            name = _name;

            lines = new Dictionary<uint, Line>();
        }
    }
}