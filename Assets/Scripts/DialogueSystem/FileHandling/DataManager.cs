
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;



public static class DataManager
{



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

        int i = 1;

        //Iterate through each line node
        foreach (var lineNode in line)
        {

            Line lineObj = new Line();

            Debug.Log(i);

            var key = lineNode.Descendants("Key");
            var _line = lineNode.Descendants("LineText"); //Try use this as iterator to make it not 6
            var conditions = lineNode.Descendants("Condition");

            lineObj.lineID = uint.Parse((string)lineNode.Attribute("id"));

            foreach (var _key in key)
            {
                ///   Debug.Log("Line Key: " + _key.Value);
                lineObj.key = _key.Value;
            }

            foreach (var nodeLine in _line)
            {
                //  Debug.Log("Line: " + nodeLine.Value);
                lineObj.line = nodeLine.Value;
            }

            foreach (var condition in conditions)
            {
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

            i++;
            //Pass the populated line object to the lines to the line list within the entity object
            entity.lines.Add((uint)lineObj.lineID, lineObj);

        }

        return entity;

    }


}

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
